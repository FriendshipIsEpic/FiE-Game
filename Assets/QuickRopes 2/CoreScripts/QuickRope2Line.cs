using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[RequireComponent(typeof(QuickRope2))]
[ExecuteInEditMode()]
public class QuickRope2Line : MonoBehaviour 
{
    private QuickRope2 rope;
    private LineRenderer line;
    public bool useAutoTextureTiling = true;

    void OnEnable()
    {
        rope = GetComponent<QuickRope2>();

        if (gameObject.GetComponent<LineRenderer>() == null)
            line = gameObject.AddComponent<LineRenderer>();
        else
            line = gameObject.GetComponent<LineRenderer>();

        if (line.sharedMaterial == null)
            line.sharedMaterial = (Material)Resources.Load("Materials/RopeLineMaterial", typeof(Material));

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
        if (rope == null)
            return;

        rope.GenerateJointObjects();
        Update();

        if (useAutoTextureTiling)
            gameObject.GetComponent<LineRenderer>().sharedMaterial.mainTextureScale = new Vector2(rope.Joints.Count / 2f, 1);
    }

    void Update()
    {
        if (line == null)
            return;

        if (useAutoTextureTiling && Application.isPlaying)
            line.material.mainTextureScale = new Vector2(rope.Joints.Count / 2f, 1);

        int index = 0;
        line.SetVertexCount(rope.Joints.Count);
        foreach (GameObject go in rope.Joints)
            line.SetPosition(index++, go.transform.position);

        //line.SetPosition(index++, rope.ropeEnd.transform.position);
    }
}
