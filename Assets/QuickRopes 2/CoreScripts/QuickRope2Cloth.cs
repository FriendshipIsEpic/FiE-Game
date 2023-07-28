using UnityEngine;
using System.Collections;

[RequireComponent(typeof(QuickRope2))]
[ExecuteInEditMode()]
public class QuickRope2Cloth : MonoBehaviour
{
    public int maxRadius = 5;
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, .3f), new Keyframe(1, .3f));
    public int crossSegments = 6;

    [SerializeField]
    private RopeTubeRenderer tube;
    private QuickRope2 rope;
    private Cloth mFilter;
    private SkinnedMeshRenderer mRender;

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
        rope.GenerateJointObjects();

        if (tube == null)
            tube = new RopeTubeRenderer(gameObject, true);

        mRender = gameObject.GetComponent<SkinnedMeshRenderer>();
        if (mRender == null)
            mRender = gameObject.AddComponent<SkinnedMeshRenderer>();

        mFilter = gameObject.GetComponent<Cloth>();
        if (mFilter == null)
            mFilter = gameObject.AddComponent<Cloth>();

        GenerateMesh();

        //if (useAutoTextureTiling)
        //    gameObject.GetComponent<ClothRenderer>().sharedMaterial.mainTextureScale = new Vector2(rope.Joints.Count / 2f, 1);

        if (gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial == null)
            gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial = (Material)Resources.Load("Materials/Rope", typeof(Material));
    }

    public void GenerateMesh()
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
        mRender.sharedMesh = tube.mesh;
    }
}
