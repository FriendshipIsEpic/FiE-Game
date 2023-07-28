using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
[RequireComponent(typeof(QuickRope2))]
[ExecuteInEditMode()]
public class QuickRope2Mesh : MonoBehaviour
{
    public bool meshStatic = false;
    public int maxRadius = 5;
    public float textureTiling = 1;
    [SerializeField]
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, .3f), new Keyframe(1, .3f));
    public Color[] grad;
    public int crossSegments = 6;

    [SerializeField]
    public RopeTubeRenderer tube;
    [SerializeField]
    private QuickRope2 rope;
    [SerializeField]
    private MeshFilter mFilter;

    void OnEnable()
    {
        rope = GetComponent<QuickRope2>();
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
        //rope.GenerateJointObjects();

        if (tube == null)
            tube = new RopeTubeRenderer(gameObject, false);

        tube.calculateTangents = true;

        UpdateMesh();
    }

    public void UpdateMesh()
    {
        tube.SetPointsAndRotations(rope.JointPositions, rope.GetRotations(rope.JointPositions));
        tube.SetEdgeCount(crossSegments);

        float[] rads = new float[rope.JointPositions.Length];
        for (int i = 0; i < rads.Length; i++)
        {
            rads[i] = curve.Evaluate(i * (1f / rads.Length));
        }

        tube.SetRadiuses(rads);
        tube.Update();

        gameObject.GetComponent<Renderer>().sharedMaterial.mainTextureScale = new Vector2(rope.Joints.Count *
        textureTiling, 1);
    }

    void Update()
    {
        if (meshStatic || !Application.isPlaying)
            return;

        float[] rads = new float[rope.JointPositions.Length];
        for (int i = 0; i < rads.Length; i++)
        {
            rads[i] = curve.Evaluate(i * (1f / rads.Length));
        }

        tube.SetPointsAndRotations(rope.JointPositions, rope.GetRotations(rope.JointPositions));
        tube.SetRadiuses(rads);
        tube.Update();

        gameObject.GetComponent<Renderer>().material.mainTextureScale = new Vector2(rope.Joints.Count * textureTiling, 1);
    }
}
