using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(QuickRope2Mesh))]
public class QuickRope2MeshEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        /*
        QuickRope2Mesh t = (QuickRope2Mesh)target;

        t.meshStatic = EditorGUILayout.Toggle(new GUIContent("Mesh Static","If checked, the mesh will not automatically update during the \"Update\" method."), t.meshStatic);
        t.crossSegments = (int)Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Sides", "The number of sides in your rope.\n\nMAX: " + MAX_CROSS_SEGMENTS), t.crossSegments), 3, MAX_CROSS_SEGMENTS);
        t.maxRadius = (int)Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Max Radius","The maximum radius allowed to be set by the curve."), t.maxRadius), 5, Mathf.Infinity);
        t.curve = EditorGUILayout.CurveField(new GUIContent("Radius Curve","The ropes radius will be defined by the shape of this curve."), t.curve, Color.white, new Rect(0, 0, 1, t.maxRadius));
        t.textureTiling = EditorGUILayout.FloatField(new GUIContent("Texture Tiling", "Sets the Y value of the texture loaded on this rope."), t.textureTiling);

        if (GUI.changed)
            t.OnInitializeMesh();
    
         */
    }
}
