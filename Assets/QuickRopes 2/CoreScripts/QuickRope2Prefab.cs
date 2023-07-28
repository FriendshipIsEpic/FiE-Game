using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(QuickRope2))]
[ExecuteInEditMode()]
public class QuickRope2Prefab : MonoBehaviour
{
    public GameObject prefab = null;
    public float jointScale = 1;
    public bool alternateJoints = true;
    public bool firstJointAlternated = false;

    private QuickRope2 rope;

    void OnEnable()
    {
        rope = GetComponent<QuickRope2>();

        if (prefab == null)
            prefab = (GameObject)Resources.Load("Link", typeof(GameObject));

        rope.OnInitializeMesh += OnInitializeMesh;
    }

    void OnDisable()
    {
        rope.OnInitializeMesh -= OnInitializeMesh;
        if (rope != null)
            rope.ClearJointObjects();
    }

    void OnDestroy()
    {
        rope.OnInitializeMesh -= OnInitializeMesh;
        if (rope != null)
            rope.ClearJointObjects();
    }

    public void OnInitializeMesh()
    {
        if (!prefab)
            return;

        //rope.FirstJointAlternated = firstJointAlternated;
        //rope.AlternateJoints = alternateJoints;
        rope.JointPrefab = prefab;
        //rope.AlternateJoints = true;
        rope.JointScale = jointScale;
        rope.GenerateJointObjects();
    }

    /*
    public void OnInsertJointAtStart()
    {
        Vector3 insertPos = rope.Joints[0].transform.position - ((rope.Joints[0].transform.position - rope.Joints[1].transform.position).normalized * rope.jointSpacing);

        GameObject go;
        go = (GameObject)Instantiate(prefab, insertPos, rope.Joints[1].transform.rotation * Quaternion.Euler(0, 0, 90));
        go.transform.localScale = Vector3.one * jointScale;

        if (!rope.showJoints)
            go.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;

        if (go.collider)
            go.collider.enabled = false;

        rope.Joints.Insert(1, go);

        rope.RebuildPhysics();
    }
    */
}
