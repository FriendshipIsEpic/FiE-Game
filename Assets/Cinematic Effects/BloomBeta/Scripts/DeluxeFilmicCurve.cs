using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class DeluxeFilmicCurve
{
    [SerializeField]
    public float m_BlackPoint = 0.0f;

    [SerializeField]
    public float m_WhitePoint = 1.0f;

    [SerializeField]
    public float m_CrossOverPoint = 0.3f;

    [SerializeField]
    public float m_ToeStrength = 0.98f;

    [SerializeField]
    public float m_ShoulderStrength = 0.0f;

    [SerializeField]
    public float m_Highlights = 0.5f;

    public float m_k;
    public Vector4 m_ToeCoef;
    public Vector4 m_ShoulderCoef;

    public float GetExposure()
    {
        float highlights = m_Highlights;
        float exposure = 2.0f + (1.0f - highlights) * 20.0f;
        return (exposure * Mathf.Exp(-2.0f));
    }

    public float ComputeK(float t, float c, float b, float s, float w)
    {
        float num = (1 - t) * (c - b);
        float denom = (1 - s) * (w - c) + (1 - t) * (c - b);

        return num / denom;
    }

    public float Toe(float x, float t, float c, float b, float s, float w, float k)
    {
        float xnum = m_ToeCoef.x * x;
        float xdenom = m_ToeCoef.y * x;

        return (xnum + m_ToeCoef.z) / (xdenom + m_ToeCoef.w);

        /*float num = k * (1 - t) * (x - b);
        float denom = c - (1 - t) * b - t * x;

        return num / denom;*/
    }

    public float Shoulder(float x, float t, float c, float b, float s, float w, float k)
    {
        float xnum = m_ShoulderCoef.x * x;
        float xdenom = m_ShoulderCoef.y * x;

        return (xnum + m_ShoulderCoef.z) / (xdenom + m_ShoulderCoef.w) + k;

        /*float num = (1 - k) * (x - c);
        float denom = s*x + (1 - s) * w - c;

        return num / denom + k;*/
    }

    public float Graph(float x, float t, float c, float b, float s, float w, float k)
    {
        if (x <= m_CrossOverPoint)
            return Toe(x, t, c, b, s, w, k);

        return Shoulder(x, t, c, b, s, w, k);
    }

    public void StoreK()
    {
        m_k = ComputeK(m_ToeStrength, m_CrossOverPoint, m_BlackPoint, m_ShoulderStrength, m_WhitePoint);
    }

    public void ComputeShaderCoefficients(float t, float c, float b, float s, float w, float k)
    {
        {
            float xNumMul = k * (1 - t);
            float numAdd = k * (1 - t) * -b;
            float xDenomMul = -t;
            float denomAdd = c - (1 - t) * b;
            m_ToeCoef = new Vector4(xNumMul, xDenomMul, numAdd, denomAdd);
        }

        {
            float xNumMul = (1 - k);
            float numAdd = (1 - k) * -c;
            float xDenomMul = s;
            float denomAdd = (1 - s) * w - c;
            m_ShoulderCoef = new Vector4(xNumMul, xDenomMul, numAdd, denomAdd);
        }
    }

    public void UpdateCoefficients()
    {
        StoreK();
        ComputeShaderCoefficients(m_ToeStrength, m_CrossOverPoint, m_BlackPoint, m_ShoulderStrength, m_WhitePoint, m_k);
    }




    Vector3[] m_CurvePoints;

    void DrawCurve()
    {
        //Rect rr = GUILayoutUtility.GetRect(Mathf.Min(r.width, 60), 60);
        //if (m_CurvePoints != null)
        {
            const int h = 100;
            const int h1 = h - 1;
            Rect rect;

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            rect = GUILayoutUtility.GetRect(Mathf.Max(EditorGUIUtility.currentViewWidth - 50.0f, 10.0f), h);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUI.Box(rect, GUIContent.none);

            int nbPoint = 40;
            int w = Mathf.FloorToInt(rect.width);

            Vector3[] points = new Vector3[nbPoint];

            for (int i = 0; i < nbPoint; i++)
            {
                float norm = (float)i / (float)nbPoint;
                float value = Graph(norm * m_WhitePoint, m_ToeStrength, m_CrossOverPoint, m_BlackPoint, m_ShoulderStrength, m_WhitePoint, m_k);
                value = Mathf.Clamp01(value);
                points[i] = new Vector3(rect.x + i * (float)w / (float)(nbPoint - 1), rect.y + (h - value * h1), 0f);
            }


            Handles.color = Color.green;
            Handles.DrawAAPolyLine(2f, points);
        }
        //EditorGUI.CurveField(rr, m_Curve);
    }

    public void OnGUI()
    {
        //SetupCurve();

        float denom = m_WhitePoint - m_BlackPoint;

        float co = (m_CrossOverPoint - m_BlackPoint) / denom;
        if (Mathf.Abs(denom) < 0.001f)
            co = 0.5f;

        EditorGUILayout.LabelField("Curve Parameters", EditorStyles.boldLabel);
        m_WhitePoint = 1.0f;
        m_BlackPoint = 0.0f;
        co = DoSlider("Middle", co, 0.0f, 1.0f);
        m_ToeStrength = -1.0f * DoSlider("Dark", -1.0f * m_ToeStrength, -0.99f, 0.99f);
        m_ShoulderStrength = DoSlider("Bright", m_ShoulderStrength, -0.99f, 0.99f);
        m_Highlights = DoSlider("Highlights", m_Highlights, 0.0f, 1.0f);

        m_CrossOverPoint = co * (m_WhitePoint - m_BlackPoint) + m_BlackPoint;
        UpdateCoefficients();

        EditorGUILayout.BeginVertical(GUILayout.MinHeight(60));
        // Curve drawing
        DrawCurve();
        EditorGUILayout.EndVertical();
    }

    AnimationCurve m_Curve;

    private static float CalculateLinearTangent(AnimationCurve curve, int index, int toIndex)
    {
        return (float)(((double)curve[index].value - (double)curve[toIndex].value) / ((double)curve[index].time - (double)curve[toIndex].time));
    }

    void SetupCurve()
    {
        m_Curve = new AnimationCurve();

        DeluxeFilmicCurve dt = this;

        float min = dt.m_BlackPoint;
        float max = dt.m_WhitePoint;

        int nbFrame = 40;
        float step = (max - min) / nbFrame;

        m_CurvePoints = new Vector3[nbFrame + 1];

        float curr = min;
        float k = dt.ComputeK(dt.m_ToeStrength, dt.m_CrossOverPoint, dt.m_BlackPoint, dt.m_ShoulderStrength, dt.m_WhitePoint);

        dt.StoreK();
        dt.ComputeShaderCoefficients(dt.m_ToeStrength, dt.m_CrossOverPoint, dt.m_BlackPoint, dt.m_ShoulderStrength, dt.m_WhitePoint, k);

        for (int i = 0; i < nbFrame + 1; ++i)
        {
            float value = dt.Graph(curr, dt.m_ToeStrength, dt.m_CrossOverPoint, dt.m_BlackPoint, dt.m_ShoulderStrength, dt.m_WhitePoint, k);

            m_CurvePoints[i] = new Vector3(curr, value);

            m_Curve.AddKey(new Keyframe(curr, value));

            curr += step;
        }

        for (int i = 0; i < m_Curve.keys.Length - 1; ++i)
        {
            float tangent = CalculateLinearTangent(m_Curve, i, i + 1);
            m_Curve.keys[i].inTangent = tangent;
            m_Curve.keys[i].outTangent = tangent;

            m_Curve.SmoothTangents(i, 0.0f);
        }
    }

    float DoSlider(string label, float value, float min, float max)
    {
        float v = value;
        EditorGUILayout.BeginHorizontal();
        v = Mathf.Clamp(EditorGUILayout.FloatField(label, v), min, max);
        v = GUILayout.HorizontalSlider(v, min, max);
        EditorGUILayout.EndHorizontal();

        return v;
    }

}
