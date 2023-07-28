using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(QuickRope2Prefab))]
public class QuickRope2PrefabEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //QuickRope2Prefab t = (QuickRope2Prefab)target;

        //t.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", t.prefab, typeof(GameObject), false);
        //t.jointScale = EditorGUILayout.FloatField("Scale", t.jointScale);
        //if (t.alternateJoints = EditorGUILayout.Toggle("Alternate Joints", t.alternateJoints))
        //{
        //    EditorGUILayout.BeginHorizontal();
        //    GUILayout.Space(25);
        //    t.firstJointAlternated = EditorGUILayout.Toggle("First Joint Alternated", t.firstJointAlternated);
        //    EditorGUILayout.EndHorizontal();
        //}

        //if (GUI.changed)
        //    t.OnInitializeMesh();
    }
}