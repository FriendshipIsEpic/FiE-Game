using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BasicRopeTypes
{
    NONE,
    Line,
    Prefab,
    Mesh,
    Cloth
}

public enum RopeConstraint
{
    NONE,
    X_Y,
    Y_Z,
    Z_X
}

public enum RopeColliderType
{
    DEFAULT,
    Sphere,
    Capsule
}

public enum RopeAttachmentJointType
{
    Fixed,
    Hinge
    //Character
}

[System.Serializable]
public class RopeAttachedObject
{
    public GameObject go;
    public RopeAttachmentJointType jointType = RopeAttachmentJointType.Fixed;
    public int jointIndex = 0;
    public Vector3 hingeAxis = Vector3.forward;
    public Joint jointRef = null;
}


[System.Serializable]
public class QuickRope2 : MonoBehaviour
{
    #region BASIC ROPE VARIABLES
    public const float PRECISION = 0.0001f;
    public const int MAX_JOINT_COUNT = 500;
    public const float MAX_JOINT_SPACING = 50;
    public const float MIN_JOINT_SPACING = 0.1f;

    public GameObject ropeEnd = null;
    public LayerMask layer;
    public float jointSpacing = 1f;
    public bool showJoints = false;

    public event System.Action OnInitializeMesh;

    [HideInInspector()]
    [SerializeField]
    private GameObject jointPrefab = null;
    public GameObject JointPrefab { get { return jointPrefab; } set { jointPrefab = value; } }

    [HideInInspector()]
    [SerializeField]
    private float jointScale = 1;
    public float JointScale { get { return jointScale; } set { jointScale = value; } }

    [HideInInspector()]
    [SerializeField]
    private bool alternateJoints = false;
    public bool AlternateJoints { get { return alternateJoints; } set { alternateJoints = value; } }

    [HideInInspector()]
    [SerializeField]
    private bool firstJointAlternated = false;
    public bool FirstJointAlternated { get { return firstJointAlternated; } set { firstJointAlternated = value; } }

    [HideInInspector()]
    [SerializeField]
    private List<Vector3> controlPoints = new List<Vector3>();
    public List<Vector3> ControlPoints { get { return controlPoints; } set { controlPoints = value; } }

    [HideInInspector()]
    [SerializeField]
    private List<Vector3> splinePoints = new List<Vector3>();
    public List<Vector3> SplinePoints { get { return splinePoints; } }

    [HideInInspector()]
    [SerializeField]
    public List<GameObject> Joints = new List<GameObject>();

    [HideInInspector()]
    [SerializeField]
    private List<Vector3> calculatedPositions = new List<Vector3>();

    [HideInInspector()]
    [SerializeField]
    private Quaternion[] calculatedRotations;

    [HideInInspector()]
    [SerializeField]
    public List<RopeAttachedObject> attachedObjects = new List<RopeAttachedObject>();

    public Vector3[] JointPositions
    {
        get
        {
            if (Joints.Count == 0)
                return new Vector3[] { Vector3.zero };

            Vector3[] pos = new Vector3[Joints.Count];
            for (int i = 0; i < pos.Length; i++)
            {
                pos[i] = Joints[i].transform.position;
            }
            return pos;
        }
    }
    public Quaternion[] JointRotations
    {
        get
        {
            Quaternion[] rots = new Quaternion[Joints.Count];
            rots[0] = Quaternion.LookRotation(Joints[0].transform.position - Joints[1].transform.position);
            for (int i = 1; i < rots.Length; i++)
            {
                rots[i] = Quaternion.LookRotation(Joints[i-1].transform.position - Joints[i].transform.position);
            }
            return rots;
        }
    }
    private int prevJointcount = 0;
    private float ropeLength = 0;
    public float RopeLength
    {
        get
        {
            if (prevJointcount != Joints.Count)
            {
                ropeLength = 0;
                for (int i = 1; i < Joints.Count; i++)
                {
                    ropeLength += Vector3.Distance(Joints[i - 1].transform.position, Joints[i].transform.position);
                }

                prevJointcount = Joints.Count;
            }

            return ropeLength;
        }
    }

    private bool freeFallMode = false;
    public bool FreeFallMode
    {
        get { return freeFallMode; }
        set
        {
            freeFallMode = value;

            if (value)
            {
                Joints[0].GetComponent<ConfigurableJoint>().connectedBody = null;
                Joints[1].transform.parent = Joints[0].transform;
                //Joints[1].rigidbody.isKinematic = true;
            }
            else
            {
                Joints[0].GetComponent<ConfigurableJoint>().connectedBody = Joints[1].GetComponent<Rigidbody>();
                Joints[1].transform.parent = null;
                //Joints[1].rigidbody.isKinematic = false;
            }
        }
    }

    private Vector3 pastUp = Vector3.zero;
    private Vector3 pRopeEndPos = Vector3.zero;
    private bool initialized = false;
    #endregion

    #region PHYSICS VARIABLES
    public bool enablePhysics = false;
    public RopeColliderType colliderType = RopeColliderType.DEFAULT;
    public PhysicMaterial physicsMaterial = null;
    public float colliderRadius = 0.25f;

    public RopeConstraint constraint = RopeConstraint.NONE;

    public float mass = 1;
    public float drag = 0.2f;
    public float angDrag = 0.05f;
    public bool useGravity = true;

    public float LowAngXLimit = -60;
    public float HighAngXLimit = 60;
    public float LTLBounce = 0;
    public float LTLSpring = 0;
    public float LTLDamper = 0;

    public float AngYLimit = 35;
    public float AngZLimit = 35;
    public float S1LBounce = 0;
    public float S1LSpring = 0;
    public float S1LDamper = 0;

    public float breakForce = Mathf.Infinity;
    public float breakTorque = Mathf.Infinity;

    public int solverOverride = -1;
    #endregion

    #region CONTROLLER VARIABLES
    private float distBetweenJoints = 0;
    private float currentVelocity = 0;

    public bool enableRopeController = false;
    public KeyCode extendRopeKey = KeyCode.DownArrow;
    public KeyCode retractRopeKey = KeyCode.UpArrow;
    public float acceleration = 10;
    public float dampening = 0.96f;
    public float sleepVelocity = 0.5f;
    public float minRopeLength = 5;
    public float maxRopeLength = 25;
    public float maxVelocity = 5;
    #endregion

    [HideInInspector]
    public int EDITOR_TAB_SELECTED = 0;
    [HideInInspector]
    public static float EDITOR_GUI_SCALE = 0.5f;
    [HideInInspector]
    public bool EDITOR_SHOW_RIGIDBODY = true;
    [HideInInspector]
    public bool EDITOR_SHOW_JOINTSETTINGS = false;
    [HideInInspector]
    public bool EDITOR_SHOW_COLLIDERSETTINGS = false;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        for (int i = 2; i < splinePoints.Count - 1; i++)
            Gizmos.DrawLine(splinePoints[i], splinePoints[i - 1]);

        for (int i = 1; i < controlPoints.Count; i++)
            Gizmos.DrawLine(controlPoints[i], controlPoints[i - 1]);

        if (enablePhysics && colliderType != RopeColliderType.DEFAULT)
        {
            Gizmos.color = new Color(0.1f, 0.7f, 0.4f);
            foreach (GameObject go in Joints)
            {
                Gizmos.DrawWireSphere(go.transform.position, colliderRadius);
            }
        }
    }
    void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        if (splinePoints.Count > 3)
        {
            Gizmos.color = Color.black;
            Vector3 prevPt = CalcPositionAtTime(0);

            for (int i = 1; i <= 100; i++)
            {
                float pm = (float)i / (float)100;
                Vector3 currPt = CalcPositionAtTime(pm);
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }

            Gizmos.color = Color.white;
        }

        if (ropeEnd && QuickRope2Helper.HasMoved(ref pRopeEndPos, ropeEnd.transform.position))
        {
            ApplyRopeSettings();
        }
    }

    void OnDestroy()
    {
        if (enablePhysics && Application.isPlaying)
        {
            foreach (RopeAttachedObject ao in attachedObjects)
                Destroy(ao.jointRef);
        }

        ClearJointObjects();
    }
    void Start()
    {
        if (ropeEnd == null)
            return;

        if (!initialized)
        {
            ApplyRopeSettings();
            AttachObjects();
        }
        distBetweenJoints = Vector3.Distance(Joints[0].transform.position, Joints[1].transform.position);
    }
    void Update()
    {
        if (!enablePhysics)
            return;

        if (freeFallMode)
        {
            UpdateFreeFall();
            return;
        }

        if (!enableRopeController)
            return;

        bool applyingForces = false;
        if (Input.GetKey(extendRopeKey))
        {
            currentVelocity += acceleration * Time.deltaTime;
            applyingForces = true;
        }
        if (Input.GetKey(retractRopeKey))
        {
            currentVelocity -= acceleration * Time.deltaTime;
            applyingForces = true;
        }

        currentVelocity = Mathf.Clamp(currentVelocity, -maxVelocity, maxVelocity);

        if ((RopeLength < minRopeLength && currentVelocity < 0) || (RopeLength > maxRopeLength && currentVelocity > 0))
        {
            currentVelocity = 0;
        }

        if (currentVelocity > 0)
            ExtendRope(currentVelocity);

        if (currentVelocity < 0)
            RetractRope(currentVelocity);

        if (!applyingForces)
        {
            currentVelocity *= dampening;
            if (currentVelocity != 0 && Mathf.Abs(currentVelocity) < sleepVelocity)
            {
                currentVelocity = 0;
                //Joints[0].GetComponent<ConfigurableJoint>().connectedBody = Joints[1].rigidbody;
                //Joints[1].transform.parent = null;
                //Joints[1].rigidbody.isKinematic = false;
            }
        }
    }
    void RetractRope(float velocity)
    {
        Joints[1].transform.parent = Joints[0].transform;
        Joints[0].GetComponent<ConfigurableJoint>().connectedBody = null;
        Joints[1].GetComponent<Rigidbody>().isKinematic = true;
        Joints[1].transform.position = Vector3.MoveTowards(Joints[1].transform.position, Joints[0].transform.position, Time.deltaTime * velocity * -1);

        if (Vector3.Distance(Joints[1].transform.position, Joints[0].transform.position) <= 0.001f)
        {
            GameObject go = Joints[1];
            Joints.RemoveAt(1);
            Joints.TrimExcess();
            Destroy(go);
            //Joints[1].transform.parent = Joints[0].transform;
            //Joints[1].rigidbody.isKinematic = true;
        }

        Joints[0].GetComponent<ConfigurableJoint>().connectedBody = Joints[1].GetComponent<Rigidbody>();
        Joints[1].GetComponent<Rigidbody>().isKinematic = false;
    }
    void ExtendRope(float velocity)
    {
        Joints[0].GetComponent<ConfigurableJoint>().connectedBody = null;
        Joints[1].GetComponent<Rigidbody>().isKinematic = true;
        //Joints[1].transform.position = Vector3.MoveTowards(Joints[1].transform.position, Joints[1].transform.position + (Joints[1].transform.position - Joints[0].transform.position).normalized, Time.deltaTime * velocity);
        Joints[1].transform.position = Vector3.MoveTowards(Joints[1].transform.position, Joints[1].transform.position - (Joints[0].transform.position - Joints[2].transform.position).normalized , Time.deltaTime * velocity);

        //Debug.DrawRay(Joints[1].transform.position, (Joints[0].transform.position - Joints[1].transform.position).normalized * 10);
        //Joints[1].transform.LookAt(Joints[0].transform.position);

        if (Vector3.Distance(Joints[1].transform.position, Joints[0].transform.position) > (distBetweenJoints*1.5f))
        {
            GameObject go;
            if (JointPrefab != null)
            {
                go = (GameObject)Instantiate(JointPrefab, Joints[1].transform.position - ((Joints[1].transform.position - Joints[0].transform.position).normalized * distBetweenJoints), Quaternion.LookRotation(Joints[0].transform.position - Joints[1].transform.position));// Quaternion.LookRotation(Joints[0].transform.position - Joints[1].transform.position));
                float ang = (alternateJoints) ? ((firstJointAlternated) ? ((Joints.Count % 2 == 0) ? 90 : 0) : ((Joints.Count % 2 == 0) ? 0 : 90)) : 0;
                go.transform.Rotate(0, 0, ang);
                go.transform.localScale = Vector3.one * jointScale;
            }
            else
            {
                go = new GameObject("Jnt_" + Joints.Count);
                go.transform.position = Joints[1].transform.position - ((Joints[1].transform.position - Joints[0].transform.position).normalized * distBetweenJoints);
                go.transform.rotation = Quaternion.LookRotation(Joints[0].transform.position - Joints[1].transform.position);
                float ang = (alternateJoints) ? ((firstJointAlternated) ? ((Joints.Count % 2 == 0) ? 0 : 90) : ((Joints.Count % 2 == 0) ? 90 : 0)) : 0;
                go.transform.Rotate(0, 0, ang);
            }

            go.layer = gameObject.layer;
            go.tag = gameObject.tag;

            if (!showJoints)
                go.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;

            if (go.GetComponent<Collider>())
                go.GetComponent<Collider>().enabled = false;

            AddConfigJoint(go).connectedBody = Joints[1].GetComponent<Rigidbody>();

            switch (colliderType)
            {
                case RopeColliderType.Sphere:
                    SphereCollider sc;
                        sc = go.AddComponent<SphereCollider>();
                        sc.radius = colliderRadius;
                        sc.center = Vector3.zero;

                        if (physicsMaterial != null)
                            sc.sharedMaterial = physicsMaterial;
                    break;
                case RopeColliderType.Capsule:
                    CapsuleCollider cc;
                        float len = Vector3.Distance(go.transform.position, Joints[1].transform.position);

                        cc = go.AddComponent<CapsuleCollider>();
                        cc.radius = colliderRadius;
                        cc.center = new Vector3(0, 0, len / 2f);
                        cc.direction = 2;
                        cc.height = len + (cc.radius + cc.radius);

                        if (physicsMaterial != null)
                            cc.sharedMaterial = physicsMaterial;
                    break;
            }

            Joints[1].GetComponent<Rigidbody>().isKinematic = false;
            Joints.Insert(1, go);
            Joints.TrimExcess();
        }

        Joints[1].GetComponent<Rigidbody>().isKinematic = false;
        Joints[0].GetComponent<ConfigurableJoint>().connectedBody = Joints[1].GetComponent<Rigidbody>();
    }
    void UpdateFreeFall()
    {
        //Joints[1].transform.position = Vector3.MoveTowards(Joints[1].transform.position, Joints[1].transform.position + (Joints[1].transform.position - Joints[0].transform.position).normalized, Time.deltaTime * velocity);
        if (RopeLength > maxRopeLength)
        {
            FreeFallMode = false;
            return;
        }

        if (Vector3.Distance(Joints[1].transform.position, Joints[0].transform.position) > distBetweenJoints)
        {
            GameObject go;
            if (JointPrefab != null)
            {
                go = (GameObject)Instantiate(JointPrefab, Joints[1].transform.position - ((Joints[1].transform.position - Joints[0].transform.position).normalized * distBetweenJoints), Quaternion.identity);// Quaternion.LookRotation(Joints[0].transform.position - Joints[1].transform.position));
                go.transform.LookAt(Joints[0].transform.position);
                //go.transform.Rotate(0, 0, Joints[1].transform.eulerAngles.z + 90);
                float ang = (alternateJoints) ? ((firstJointAlternated) ? ((Joints.Count % 2 == 0) ? 90 : 0) : ((Joints.Count % 2 == 0) ? 0 : 90)) : 0;
                go.transform.Rotate(0, 0, ang);
                go.transform.localScale = Vector3.one * jointScale;
            }
            else
            {
                go = new GameObject("Jnt_NULL");
                go.transform.position = Joints[1].transform.position - ((Joints[1].transform.position - Joints[0].transform.position).normalized * distBetweenJoints);
                go.transform.rotation = Quaternion.identity;
            }

            go.layer = gameObject.layer;
            go.tag = gameObject.tag;

            if (!showJoints)
                go.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;

            if (go.GetComponent<Collider>())
                go.GetComponent<Collider>().enabled = false;

            AddConfigJoint(go).connectedBody = Joints[1].GetComponent<Rigidbody>();
            Joints[1].transform.parent = null;
            //Joints[1].rigidbody.isKinematic = false;
            Joints.Insert(1, go);
            Joints.TrimExcess();
            Joints[1].transform.parent = Joints[0].transform;
            //Joints[1].rigidbody.isKinematic = true;
        }
    }

    public void GenerateJointObjects()
    {
        ClearJointObjects();

        Joints.Add(gameObject);

        for (int i = 1; i < SplinePoints.Count - 1; i++)
        {
            GameObject go;
            if (JointPrefab != null)
            {
                go = (GameObject)Instantiate(JointPrefab, SplinePoints[i], calculatedRotations[i]);

                if (alternateJoints)
                {
                    int comparitor = (firstJointAlternated) ? 1 : 0;
                    go.transform.Rotate(0, 0, (i % 2 == comparitor) ? 90 : 0);
                }

                go.transform.localScale = jointPrefab.transform.localScale * jointScale;
            }
            else
            {
                go = new GameObject("Jnt_" + i.ToString());
                go.transform.position = SplinePoints[i];
                go.transform.rotation = calculatedRotations[i];
            }

            go.layer = gameObject.layer;
            go.tag = gameObject.tag;

            if (!Application.isPlaying)
                go.transform.parent = transform;

            if (!showJoints)
                go.hideFlags = HideFlags.HideInHierarchy | HideFlags.NotEditable;

            if (go.GetComponent<Collider>())
                go.GetComponent<Collider>().enabled = false;

            Joints.Add(go);
        }

        Joints.Add(ropeEnd);
    }
    public void ClearJointObjects()
    {
        for(int i = 0; i < Joints.Count; i++)
        {
            if (Joints[i].GetInstanceID() == gameObject.GetInstanceID() || Joints[i].GetInstanceID() == ropeEnd.GetInstanceID())
                continue;

            if (Application.isPlaying)
            {
                Destroy(Joints[i]);
            }
            else
            {
                DestroyImmediate(Joints[i]);
            }
        }

        Joints.Clear();
        Joints.TrimExcess();
    }
    private void PreCalculateRotations()
    {
        calculatedRotations = new Quaternion[SplinePoints.Count];
        calculatedRotations[0] = Quaternion.LookRotation(SplinePoints[0] - SplinePoints[1]);
        for (int i = 1; i < calculatedRotations.Length; i++)
        {
            calculatedRotations[i] = Quaternion.LookRotation(SplinePoints[i - 1] - SplinePoints[i]);
        }
    }
    public Quaternion[] GetRotations(Vector3[] points)
    {
        Vector3[] directions = new Vector3[points.Length]; ;
        Quaternion[] rotations = new Quaternion[points.Length]; ;
        Vector3 forward, up;

        for (int p = 0; p < points.Length - 1; p++)
            directions[p] = points[p + 1] - points[p];

        directions[points.Length - 1] = directions[points.Length - 2];

        if (pastUp == Vector3.zero)
        {
            up = directions[0].x == 0 && directions[0].z == 0 ? Vector3.right : Vector3.up;
        }
        else
        {
            up = pastUp;
        }

        for (int p = 0; p < points.Length; p++)
        {
            if (p != 0 && p != points.Length - 1)
            {
                forward = directions[p] + directions[p - 1];
            }
            else
            {
                if (points[0] == points[points.Length - 1]) forward = directions[points.Length - 1] + directions[0];
                else forward = directions[p];
            }

            if (forward == Vector3.zero)
            {
                rotations[p] = Quaternion.identity;
                continue;
            }

            forward.Normalize();
            Vector3 right = Vector3.Cross(up, forward);
            if (right == Vector3.zero)
                right = Vector3.Cross(new Vector3(-0.3f, 0.1f, 0), new Vector3(0, 0, 0.4f));

            up = Vector3.Cross(forward, right);

            if (p == 0)
                pastUp = up;

            if (right != Vector3.zero)
                rotations[p].SetLookRotation(-right, up);
        }

        return rotations;
    }
    private Vector3 CalcPositionAtTime(float t)
    {
        int numSections = calculatedPositions.Count - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
        float u = t * (float)numSections - (float)currPt;

        Vector3 a = calculatedPositions[currPt];
        Vector3 b = calculatedPositions[currPt + 1];
        Vector3 c = calculatedPositions[currPt + 2];
        Vector3 d = calculatedPositions[currPt + 3];

        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
        );
    }

    #region PHYSICS - Settings
    void UpdatePhysics()
    {
        if (Joints.Count == 0 || !enablePhysics)
            return;

        // ====== ADD RIGIDBODIES =========
        for (int r = 0; r < Joints.Count; r++)
        {
            GameObject go = Joints[r];

            if (go == null)
                return;

            if (go.GetComponent<Rigidbody>() == null)
                go.AddComponent<Rigidbody>();

            switch (constraint)
            {
                case RopeConstraint.NONE:
                    go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    break;
                case RopeConstraint.X_Y:
                    go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionZ;
                    break;
                case RopeConstraint.Y_Z:
                    go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX;
                    break;
                case RopeConstraint.Z_X:
                    go.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY;
                    break;
            }

            if (solverOverride != -1 && solverOverride > 1)
                go.GetComponent<Rigidbody>().solverIterationCount = solverOverride;

            go.GetComponent<Rigidbody>().mass = mass;
            go.GetComponent<Rigidbody>().angularDrag = angDrag;
            go.GetComponent<Rigidbody>().drag = drag;
            go.GetComponent<Rigidbody>().useGravity = useGravity;
        }

        if (Application.isPlaying)
        {
            // ========= ADD CONFIG JOINTS ============
            for (int c = 0; c < Joints.Count - 1; c++)
                AddConfigJoint(Joints[c]).connectedBody = Joints[c + 1].GetComponent<Rigidbody>();

            // ========= ADD COLLIDERS ==============
            if (colliderType != RopeColliderType.DEFAULT)
                AddColliders();
        }
    }
    ConfigurableJoint AddConfigJoint(GameObject joint)
    {
        if (Application.isPlaying)
            Destroy(joint.GetComponent<ConfigurableJoint>());
        else
            DestroyImmediate(joint.GetComponent<ConfigurableJoint>());

        if (Application.isPlaying && joint.GetComponent<Collider>() && colliderType == RopeColliderType.DEFAULT)
            joint.GetComponent<Collider>().enabled = true;

        ConfigurableJoint cj = joint.AddComponent<ConfigurableJoint>();

        cj.anchor = Vector3.zero;
        cj.xMotion = ConfigurableJointMotion.Locked;
        cj.yMotion = ConfigurableJointMotion.Locked;
        cj.zMotion = ConfigurableJointMotion.Locked;

        cj.angularXMotion = ConfigurableJointMotion.Limited;
        cj.angularYMotion = ConfigurableJointMotion.Limited;
        cj.angularZMotion = ConfigurableJointMotion.Limited;

        cj.lowAngularXLimit = new SoftJointLimit() { limit = LowAngXLimit, bounciness = LTLBounce/*, spring = LTLSpring, damper = LTLDamper*/ };
        cj.highAngularXLimit = new SoftJointLimit() { limit = HighAngXLimit, bounciness = LTLBounce/*, spring = LTLSpring, damper = LTLDamper*/ };
        cj.angularYLimit = new SoftJointLimit() { limit = AngYLimit, bounciness = S1LBounce/*, spring = S1LSpring, damper = S1LDamper*/ };
        cj.angularZLimit = new SoftJointLimit() { limit = AngZLimit, bounciness = S1LBounce/*, spring = S1LSpring, damper = S1LDamper*/ };


        if (!enableRopeController)
        {
            cj.breakForce = breakForce;
            cj.breakTorque = breakTorque;
        }

        return cj;
    }
    void AddColliders()
    {
        if (!Application.isPlaying)
            return;

        switch (colliderType)
        {
            case RopeColliderType.Sphere:
                SphereCollider sc;
                for (int i = 1; i < Joints.Count - 1; i++)
                {
                    sc = Joints[i].AddComponent<SphereCollider>();
                    sc.radius = colliderRadius;
                    sc.center = Vector3.zero;

                    if (physicsMaterial != null)
                        sc.sharedMaterial = physicsMaterial;
                }
                break;
            case RopeColliderType.Capsule:
                CapsuleCollider cc;
                for (int i = 1; i < Joints.Count; i++)
                {
                    float len = Vector3.Distance(Joints[i].transform.position, Joints[i - 1].transform.position);

                    if (len < colliderRadius)
                        continue;

                    cc = Joints[i].AddComponent<CapsuleCollider>();
                    cc.radius = colliderRadius;
                    cc.center = new Vector3(0, 0, len / 2f);
                    cc.direction = 2;
                    cc.height = len + (cc.radius + cc.radius);

                    if (physicsMaterial != null)
                        cc.sharedMaterial = physicsMaterial;
                }
                break;
        }
    }
    #endregion

    void AttachObjects()
    {
        if (enablePhysics && Application.isPlaying)
        {
            foreach (RopeAttachedObject ao in attachedObjects)
            {
                if (ao.go == null)
                    continue;

                if (ao.jointIndex > Joints.Count - 1)
                    ao.jointIndex = Joints.Count - 1;

                switch (ao.jointType)
                {
                    case RopeAttachmentJointType.Fixed:
                        ao.jointRef = Joints[ao.jointIndex].AddComponent<FixedJoint>();
                        ao.jointRef.connectedBody = ao.go.GetComponent<Rigidbody>();
                        break;
                    case RopeAttachmentJointType.Hinge:
                        ao.jointRef = ao.go.AddComponent<HingeJoint>();//ao.jointRef = Joints[ao.jointIndex].AddComponent<HingeJoint>();
                        (ao.jointRef as HingeJoint).axis = ao.hingeAxis;
                        ao.jointRef.connectedBody = Joints[ao.jointIndex].GetComponent<Rigidbody>();//ao.jointRef.connectedBody = ao.go.GetComponent<Rigidbody>();
                        break;
                }
            }
        }
    }

    public void ApplyRopeSettings()
    {
        if (ropeEnd == null)
            return;

        // ============== Setup For Spline Point Generation
        calculatedPositions.Clear(); calculatedPositions.TrimExcess();
        calculatedPositions.Add(transform.position);
        calculatedPositions.AddRange(controlPoints);
        calculatedPositions.Add(ropeEnd.transform.position);
        calculatedPositions.Insert(0, transform.position - (calculatedPositions[1] - calculatedPositions[0]).normalized);
        calculatedPositions.Add(calculatedPositions[calculatedPositions.Count - 1] + (calculatedPositions[calculatedPositions.Count - 1] - calculatedPositions[calculatedPositions.Count - 2]).normalized);
        // ================================================

        float time = 0f;
        Vector3 nsPosOffset = Vector3.zero;

        splinePoints.Clear(); splinePoints.TrimExcess();
        splinePoints.Add(CalcPositionAtTime(time));

        while (time <= 1f)
        {
            Vector3 p = CalcPositionAtTime(time);
            if (Vector3.Distance(splinePoints[splinePoints.Count - 1], (p + nsPosOffset)) >= jointSpacing)
                splinePoints.Add(p);

            time += PRECISION;
        }

        splinePoints.Add(CalcPositionAtTime(1));

        PreCalculateRotations();

        transform.rotation = calculatedRotations[0];
        ropeEnd.transform.rotation = calculatedRotations[calculatedRotations.Length - 1];

        if (gameObject.GetComponent<QuickRope2Prefab>() == null && gameObject.GetComponent<QuickRope2Cloth>() == null)
            GenerateJointObjects();

        JointPrefab = null;
        if (OnInitializeMesh != null)
            OnInitializeMesh();

        if (gameObject.GetComponent<QuickRope2Cloth>() == null)
            UpdatePhysics();

        initialized = true;
    }
    public void RebuildMesh()
    {
        if (!initialized)
            return;

        if (OnInitializeMesh != null)
            OnInitializeMesh();
    }

    public void AttachObject(GameObject obj, int jointIndex, RopeAttachmentJointType jointType, Vector3 hingeAxis, bool centerOnIndex)
    {
        if (gameObject.GetComponent<QuickRope2Cloth>())
        {
            Debug.LogError("You must use the \"Cloth\" component to attach objects when using the Cloth mesh type.");
            return;
        }

        RopeAttachedObject rao = new RopeAttachedObject();
        rao.go = obj;
        rao.jointIndex = jointIndex;
        rao.jointType = jointType;
        rao.hingeAxis = hingeAxis;

        if (centerOnIndex)
            rao.go.transform.position = Joints[rao.jointIndex].transform.position;

        attachedObjects.Add(rao);

        if (Application.isPlaying)
        {
            if (rao.go == null)
                return;

            switch (rao.jointType)
            {
                case RopeAttachmentJointType.Fixed:
                    rao.jointRef = Joints[rao.jointIndex].AddComponent<FixedJoint>();
                    rao.jointRef.connectedBody = rao.go.GetComponent<Rigidbody>();
                    break;
                case RopeAttachmentJointType.Hinge:
                    //rao.jointRef = Joints[rao.jointIndex].AddComponent<HingeJoint>();
                    //(rao.jointRef as HingeJoint).axis = rao.hingeAxis;
                    //rao.jointRef.connectedBody = rao.go.GetComponent<Rigidbody>();

                    rao.jointRef = rao.go.AddComponent<HingeJoint>();//ao.jointRef = Joints[ao.jointIndex].AddComponent<HingeJoint>();
                    (rao.jointRef as HingeJoint).axis = rao.hingeAxis;
                    rao.jointRef.connectedBody = Joints[rao.jointIndex].GetComponent<Rigidbody>();//ao.jointRef.connectedBody = ao.go.GetComponent<Rigidbody>();

                    break;
            }
        }
    }
    public void AttachObject(GameObject obj, int jointIndex, RopeAttachmentJointType jointType, bool centerOnIndex)
    {
        AttachObject(obj, jointIndex, jointType, Vector3.forward, centerOnIndex);
    }
    public void AttachObject(GameObject obj, int jointIndex, bool centerOnIndex)
    {
        AttachObject(obj, jointIndex, RopeAttachmentJointType.Fixed, centerOnIndex);
    }
    public void DetachObject(GameObject obj)
    {
        foreach (RopeAttachedObject ao in attachedObjects)
        {
            if (ao.go.GetInstanceID() == obj.GetInstanceID())
            {
                if (Application.isPlaying)
                    Destroy(ao.jointRef);
                else
                    DestroyImmediate(ao.jointRef);

                attachedObjects.Remove(ao);
                attachedObjects.TrimExcess();
                return;
            }
        }
    }

    public static QuickRope2 Create(GameObject pointA, GameObject pointB, List<Vector3> curvePoints, BasicRopeTypes ropeType)
    {
        QuickRope2 qr = pointA.AddComponent<QuickRope2>();
        qr.ropeEnd = pointB;

        switch (ropeType)
        {
            case BasicRopeTypes.NONE:
                break;
            case BasicRopeTypes.Line:
                qr.gameObject.AddComponent<QuickRope2Line>();
                break;
            case BasicRopeTypes.Prefab:
                qr.gameObject.AddComponent<QuickRope2Prefab>();
                break;
            case BasicRopeTypes.Mesh:
                qr.gameObject.AddComponent<QuickRope2Mesh>();
                break;
            case BasicRopeTypes.Cloth:
                qr.gameObject.AddComponent<QuickRope2Cloth>();
                break;
        }

        qr.ApplyRopeSettings();

        return qr;
    }
    public static QuickRope2 Create(Vector3 pointA, Vector3 pointB, List<Vector3> curvePoints, BasicRopeTypes ropeType)
    {
        GameObject ob1 = new GameObject("Rope");
        GameObject ob2 = new GameObject("Rope_End");

        ob1.transform.position = pointA;
        ob2.transform.position = pointB;

        return Create(ob1, ob2, curvePoints, ropeType);
    }
    public static QuickRope2 Create(GameObject pointA, Vector3 pointB, List<Vector3> curvePoints, BasicRopeTypes ropeType)
    {
        GameObject ob1 = new GameObject("Rope_End");

        ob1.transform.position = pointB;

        return Create(pointA, ob1, curvePoints, ropeType);
    }
    public static QuickRope2 Create(Vector3 pointA, GameObject pointB, List<Vector3> curvePoints, BasicRopeTypes ropeType)
    {
        GameObject ob1 = new GameObject("Rope");

        ob1.transform.position = pointA;

        return Create(ob1, pointB, curvePoints, ropeType);
    }
    public static QuickRope2 Create(GameObject pointA, GameObject pointB, BasicRopeTypes ropeType)
    {
        return Create(pointA, pointB, null, ropeType);
    }
    public static QuickRope2 Create(Vector3 pointA, GameObject pointB, BasicRopeTypes ropeType)
    {
        return Create(pointA, pointB, null, ropeType);
    }
    public static QuickRope2 Create(GameObject pointA, Vector3 pointB, BasicRopeTypes ropeType)
    {
        return Create(pointA, pointB, null, ropeType);
    }
    public static QuickRope2 Create(Vector3 pointA, Vector3 pointB, BasicRopeTypes ropeType)
    {
        return Create(pointA, pointB, null, ropeType);
    }
}
