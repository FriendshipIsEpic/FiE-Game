using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(QuickRope2Cloth))]
public class QuickRope2ClothEditor : Editor 
{
    public const int MAX_CROSS_SEGMENTS = 24;
    public const float MIN_RADIUS = 0.001f;
    public const float MAX_RADIUS = 500;

    public override void OnInspectorGUI()
    {
        //QuickRope2Cloth t = (QuickRope2Cloth)target;

        //t.crossSegments = (int)Mathf.Clamp(t.crossSegments, 3, MAX_CROSS_SEGMENTS);
        //t.maxRadius = (int)Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Max Radius", "The maximum radius allowed to be set by the curve."), t.maxRadius), 5, Mathf.Infinity);
        //t.curve = EditorGUILayout.CurveField(new GUIContent("Radius Curve", "The ropes radius will be defined by the shape of this curve."), t.curve, Color.white, new Rect(0, 0, 1, t.maxRadius));

        //if (GUI.changed)
        //    t.OnInitializeMesh();
    }
}