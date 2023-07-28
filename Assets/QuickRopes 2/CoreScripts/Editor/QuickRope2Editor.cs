using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(QuickRope2))]
public class QuickRope2Editor : Editor
{
    QuickRope2 r;
    private int currentActiveHandle = -1;
    private float guiScale = 0.5f;
    private Vector3 prevRopePos = Vector3.zero;

    public const int MAX_CROSS_SEGMENTS = 24;
    public const float MIN_RADIUS = 0.001f;
    public const float MAX_RADIUS = 500;

    private Vector2 aoScrollPosition = Vector2.zero;

    void OnEnable()
    {
        r = (QuickRope2)target;

        if (Application.isPlaying)
            return;

        CleanupObject();
        r.ApplyRopeSettings(); //r.SetControlPoints(r.ControlPoints);
    }

    public override void OnInspectorGUI()
    {
        r.EDITOR_TAB_SELECTED = GUILayout.Toolbar(r.EDITOR_TAB_SELECTED, new string[] { "Basics", "Physics", "Attach", "Control" });
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        bool isCloth = (r.GetComponent<QuickRope2Cloth>() != null);

        switch (r.EDITOR_TAB_SELECTED)
        {
            case 0:
                r.ropeEnd = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Rope End", "The defined end of this rope."), r.ropeEnd, typeof(GameObject), true);
                r.jointSpacing = EditorGUILayout.FloatField(new GUIContent("Joint Spacing", "The amount of distance between each joint in the rope object.\n\n  Max = " + QuickRope2.MAX_JOINT_SPACING + "\n  Min = " + QuickRope2.MIN_JOINT_SPACING + "\n\nTo set a custom the min/max distance, alter the const value in the QuickRopes2.cs script."), r.jointSpacing);
                r.showJoints = EditorGUILayout.Toggle(new GUIContent("Preview Joints", "If checked, all joints created will be shown in the Hierarchy. Useful if you are trying to debug custom scripts."), r.showJoints);
                QuickRope2.EDITOR_GUI_SCALE = EditorGUILayout.Slider("Editor GUI Scale", QuickRope2.EDITOR_GUI_SCALE, 0.1f, 5f);
                guiScale = QuickRope2.EDITOR_GUI_SCALE;

                EditorGUILayout.Separator();
                EditorGUILayout.PrefixLabel("Set Mesh Type");
                EditorGUILayout.BeginHorizontal(); EditorGUILayout.Space(); int val = GUILayout.Toolbar(-1, new string[] { "None", "Line", "Prefab", "Mesh", "Cloth" }); EditorGUILayout.Space(); EditorGUILayout.EndHorizontal();
                switch (val)
                {
                    case 0:
                        ClearMeshTypes();
                        break;
                    case 1:
                        ClearMeshTypes();
                        r.gameObject.AddComponent<QuickRope2Line>();
                        break;
                    case 2:
                        ClearMeshTypes();
                        r.gameObject.AddComponent<QuickRope2Prefab>();
                        break;
                    case 3:
                        ClearMeshTypes();
                        r.gameObject.AddComponent<QuickRope2Mesh>();
                        break;
                    case 4:
                        ClearMeshTypes();
                        r.gameObject.AddComponent<QuickRope2Cloth>();
                        break;
                }


                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUILayout.PrefixLabel("Mesh Type Settings");

                if (r.GetComponent<QuickRope2Line>() != null)
                {
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); EditorGUILayout.LabelField("Use \"Line Renderer\" component below."); EditorGUILayout.EndHorizontal();
                }

                if (r.GetComponent<QuickRope2Prefab>() != null)
                {
                    QuickRope2Prefab t = r.GetComponent<QuickRope2Prefab>();

                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); t.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", t.prefab, typeof(GameObject), false); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); t.jointScale = EditorGUILayout.FloatField("Scale", t.jointScale); EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); r.AlternateJoints = EditorGUILayout.Toggle("Alternate Joints", r.AlternateJoints); EditorGUILayout.EndHorizontal();
                    if (r.AlternateJoints)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(35);
                        r.FirstJointAlternated = EditorGUILayout.Toggle("First Joint Alternated", r.FirstJointAlternated);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                if(r.GetComponent<QuickRope2Mesh>()!=null)
                {
                    QuickRope2Mesh t = r.GetComponent<QuickRope2Mesh>();

                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); t.meshStatic = EditorGUILayout.Toggle(new GUIContent("Mesh Static", "If checked, the mesh will not automatically update during the \"Update\" method."), t.meshStatic); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); t.crossSegments = (int)Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Sides", "The number of sides in your rope.\n\nMAX: " + MAX_CROSS_SEGMENTS), t.crossSegments), 3, MAX_CROSS_SEGMENTS); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); t.maxRadius = (int)Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Curve Scale", "The scale of the curve graph below."), t.maxRadius), 1, Mathf.Infinity); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); t.curve = EditorGUILayout.CurveField(new GUIContent("Radius Curve", "The ropes radius will be defined by the shape of this curve."), t.curve, Color.white, new Rect(0, 0, 1, t.maxRadius)); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); t.textureTiling = EditorGUILayout.FloatField(new GUIContent("Texture Tiling", "Sets the Y value of the texture loaded on this rope."), t.textureTiling); EditorGUILayout.EndHorizontal();

                }

                if (r.GetComponent<QuickRope2Cloth>() != null)
                {
                    QuickRope2Cloth t = r.GetComponent<QuickRope2Cloth>();

                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); t.crossSegments = (int)Mathf.Clamp(t.crossSegments, 3, MAX_CROSS_SEGMENTS); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); t.maxRadius = (int)Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Max Radius", "The scale of the curve graph below."), t.maxRadius), 1, Mathf.Infinity); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); t.curve = EditorGUILayout.CurveField(new GUIContent("Radius Curve", "The ropes radius will be defined by the shape of this curve."), t.curve, Color.white, new Rect(0, 0, 1, t.maxRadius)); EditorGUILayout.EndHorizontal();

                }

                break;

            case 1:
                /* BEGIN PHYSICS REGION */
                #region Physics

                if (r.enablePhysics && isCloth)
                    r.enablePhysics = false;

                if (isCloth)
                {
                    GUILayout.Label("You cannot use traditional physics\non the Cloth type.");
                    EditorGUILayout.Separator();
                    GUI.enabled = false;
                }

                r.enablePhysics = EditorGUILayout.Toggle("Use Physics", r.enablePhysics);

                GUI.enabled = r.enablePhysics;
                if (r.EDITOR_SHOW_RIGIDBODY = EditorGUILayout.Foldout(r.EDITOR_SHOW_RIGIDBODY, "Rigidbody Settings"))
                {
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.constraint = (RopeConstraint)EditorGUILayout.EnumPopup(new GUIContent("Constraints", ""), r.constraint); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.solverOverride = (int)Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Solver Override", ""), r.solverOverride), -1, 255); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.mass = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Mass", ""), r.mass), 0.0001f, Mathf.Infinity); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.drag = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Drag", ""), r.drag), 0, Mathf.Infinity); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.angDrag = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Angular Drag", ""), r.angDrag), 0, Mathf.Infinity); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.useGravity = EditorGUILayout.Toggle(new GUIContent("Use Gravity", ""), r.useGravity); EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Separator();

                if (r.EDITOR_SHOW_JOINTSETTINGS = EditorGUILayout.Foldout(r.EDITOR_SHOW_JOINTSETTINGS, "Joint Settings"))
                {

                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.LowAngXLimit = (r.AngYLimit = r.HighAngXLimit = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Swing Limit", ""), r.AngYLimit), 0, 180)) * -1; EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(40); r.LTLDamper = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Dampen", ""), r.LTLDamper), 0, 500); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(40); r.LTLSpring = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Spring", ""), r.LTLSpring), 0, 500); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(40); r.LTLBounce = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Bounce", ""), r.LTLBounce), 0, 500); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();

                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.AngZLimit = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Twist Limit", ""), r.AngZLimit), 0, 180); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(40); r.S1LDamper = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Dampen", ""), r.S1LDamper), 0, 500); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(40); r.S1LSpring = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Spring", ""), r.S1LSpring), 0, 500); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(40); r.S1LBounce = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Bounce", ""), r.S1LBounce), 0, 500); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();

                    GUI.enabled = ((r.GetComponent<QuickRope2Cloth>() == null) && (r.GetComponent<QuickRope2Mesh>() == null) && (r.GetComponent<QuickRope2Line>() == null));
                        EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.breakForce = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Break Force", ""), r.breakForce), 0, Mathf.Infinity); EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.breakTorque = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Break Torque", ""), r.breakTorque), 0, Mathf.Infinity); EditorGUILayout.EndHorizontal();
                    GUI.enabled = true;
                }
                EditorGUILayout.Separator();

                if (r.EDITOR_SHOW_COLLIDERSETTINGS = EditorGUILayout.Foldout(r.EDITOR_SHOW_COLLIDERSETTINGS, "Collider Settings"))
                {
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.colliderType = (RopeColliderType)EditorGUILayout.EnumPopup(new GUIContent("Collider Type", ""), r.colliderType); EditorGUILayout.EndHorizontal();
                    GUI.enabled = (r.colliderType != RopeColliderType.DEFAULT);
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.colliderRadius = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Collider Radius", ""), r.colliderRadius), 0, Mathf.Infinity); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.physicsMaterial = (PhysicMaterial)EditorGUILayout.ObjectField(new GUIContent("Physics Material", ""), r.physicsMaterial, typeof(PhysicMaterial), false); EditorGUILayout.EndHorizontal();
                    GUI.enabled = true;
                }
                GUI.enabled = true;
                #endregion

                break;
            case 2:
                if (isCloth)
                {
                    GUILayout.Label("You must attach objects via the \n\"Interactive Cloth\" component below.");
                    EditorGUILayout.Separator();
                    GUI.enabled = false;
                }

                if (GUILayout.Button(new GUIContent("Attach GameObject", "Adds a new attachment variable. Assign the variable with the object you would like to attach and the joint index you would like your object to attach to. Attachment is represented by a blue arrow pointing from object to joint index in the scene.")))
                {
                    r.attachedObjects.Add(new RopeAttachedObject());
                }

                EditorGUILayout.Separator();

                if (r.attachedObjects.Count == 0)
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("No objects have been attached.\n\nPress \"Attach GameObject\" button above.", GUILayout.Height(55));
                }

                aoScrollPosition = GUILayout.BeginScrollView(aoScrollPosition, GUILayout.MaxHeight(200), GUILayout.MinHeight(200));
                foreach (RopeAttachedObject ao in r.attachedObjects)
                {
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(5); ao.go = (GameObject)EditorGUILayout.ObjectField((ao.go==null) ? "Object" : ao.go.name, ao.go, typeof(GameObject), true); EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); ao.jointType = (RopeAttachmentJointType)EditorGUILayout.EnumPopup("Con. Type", ao.jointType); EditorGUILayout.EndHorizontal();

                    if (ao.jointType == RopeAttachmentJointType.Hinge)
                    {
                        EditorGUILayout.BeginHorizontal(); GUILayout.Space(35); ao.hingeAxis = EditorGUILayout.Vector3Field("Axis", ao.hingeAxis); EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); ao.jointIndex = (int)Mathf.Clamp(EditorGUILayout.IntField("Index", ao.jointIndex), 0, r.Joints.Count-1); EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    if (ao.go != null)
                    {
                        GUILayout.Space(Screen.width / 3f); if (GUILayout.Button("Center On Index")) { ao.go.transform.position = r.Joints[ao.jointIndex].transform.position; }
                    }
                    if (GUILayout.Button("Remove")) { r.attachedObjects.Remove(ao); r.attachedObjects.TrimExcess(); return; }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
                GUILayout.EndScrollView();
                GUI.enabled = true;
                break;

            case 3:

                if (r.enablePhysics && isCloth)
                    r.enablePhysics = false;

                if (isCloth)
                {
                    GUILayout.Label("You cannot use the controller\non the Cloth type.");
                    EditorGUILayout.Separator();
                    r.enableRopeController = false;
                    GUI.enabled = false;
                }

                r.enableRopeController = EditorGUILayout.Toggle(new GUIContent("Enable Conroller", ""), r.enableRopeController);
                EditorGUILayout.Separator();

                GUI.enabled = r.enableRopeController;
                EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.extendRopeKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Extend Key", ""), r.extendRopeKey); EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.retractRopeKey = (KeyCode)EditorGUILayout.EnumPopup(new GUIContent("Retract Key", ""), r.retractRopeKey); EditorGUILayout.EndHorizontal();

                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); EditorGUILayout.LabelField("Current Rope Length: " + r.RopeLength); EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.maxRopeLength = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Max Length", ""), r.maxRopeLength), r.jointSpacing * 3f, Mathf.Infinity); EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.minRopeLength = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Min Length", ""), r.minRopeLength), r.jointSpacing * 3f, Mathf.Infinity); EditorGUILayout.EndHorizontal();

                EditorGUILayout.Separator();
                EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.acceleration = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Acceleration", ""), r.acceleration), 0, Mathf.Infinity); EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.dampening = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Dampening", ""), r.dampening), 0, 1); EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.maxVelocity = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Max Velocity", ""), r.maxVelocity), 0, Mathf.Infinity); EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(); GUILayout.Space(25); r.sleepVelocity = Mathf.Clamp(EditorGUILayout.FloatField(new GUIContent("Sleep Velocity", ""), r.sleepVelocity), 0, Mathf.Infinity); EditorGUILayout.EndHorizontal();

                EditorGUILayout.Separator();

                GUI.enabled = true;
                break;
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (!Application.isPlaying && GUI.changed)
            UpdateRope();
    }

    void CleanupObject()
    {
        if (r.ropeEnd == null)
            return;

        if (!r.enablePhysics)
        {
            DestroyImmediate(r.ropeEnd.GetComponent<ConfigurableJoint>());
            DestroyImmediate(r.ropeEnd.GetComponent<Rigidbody>());
            DestroyImmediate(r.GetComponent<ConfigurableJoint>());
            DestroyImmediate(r.GetComponent<Rigidbody>());
        }

        if (r.GetComponent<QuickRope2Line>() == null)
        {
            DestroyImmediate(r.GetComponent<LineRenderer>());
        }

        if (r.GetComponent<QuickRope2Mesh>() == null)
        {
            //if(r.GetComponent<QuickRope2Cloth>()==null)
            //    DestroyImmediate(r.GetComponent<RopeTubeRenderer>());

            DestroyImmediate(r.GetComponent<MeshFilter>());
            DestroyImmediate(r.GetComponent<MeshRenderer>());
        }

        if (r.GetComponent<QuickRope2Cloth>() == null)
        {
            //if (r.GetComponent<QuickRope2Mesh>() == null)
            //    DestroyImmediate(r.GetComponent<RopeTubeRenderer>());

            DestroyImmediate(r.GetComponent<SkinnedMeshRenderer>());
            DestroyImmediate(r.GetComponent<Cloth>());
        }
    }

    void ClearMeshTypes()
    {
        if (r.GetComponent<QuickRope2Line>())
        {
            DestroyImmediate(r.GetComponent<QuickRope2Line>());
        }

        if (r.GetComponent<QuickRope2Mesh>())
        {
            DestroyImmediate(r.GetComponent<QuickRope2Mesh>());
        }

        if (r.GetComponent<QuickRope2Cloth>())
        {
            DestroyImmediate(r.GetComponent<QuickRope2Cloth>());
        }

        if (r.GetComponent<QuickRope2Prefab>())
        {
            DestroyImmediate(r.GetComponent<QuickRope2Prefab>());
        }
    }

    bool rebuildAfterCompile = false;
    void OnSceneGUI()
    {
        r = (QuickRope2)target;
        if (Application.isPlaying || r.ropeEnd == null)
            return;

        if (EditorApplication.isCompiling)
        {
            rebuildAfterCompile = true;
        }
        else if (rebuildAfterCompile)
        {
            rebuildAfterCompile = false;
            UpdateRope();
            //return;
        }

        Quaternion camLookAt = SceneView.currentDrawingSceneView.camera.transform.rotation;
        Vector3 camNormal = SceneView.currentDrawingSceneView.camera.transform.forward;


        //==============  Add Control Point Button ==================

        if (r.ControlPoints.Count == 0 && PlusButton(r.transform.position + (r.ropeEnd.transform.position - r.transform.position) / 2, camLookAt))
        {
            r.ControlPoints.Add(r.transform.position + (r.ropeEnd.transform.position - r.transform.position) / 2);
        }

        if(r.ControlPoints.Count > 0)
        {
            // First Add Button
            if (PlusButton(r.transform.position + (r.ControlPoints[0] - r.transform.position)/2, camLookAt))
            {
                r.ControlPoints.Insert(0, r.transform.position + (r.ControlPoints[0] - r.transform.position) / 2);
                currentActiveHandle = 0;
            }

            // Last Add Button
            if (PlusButton(r.ControlPoints[r.ControlPoints.Count - 1] + (r.ropeEnd.transform.position - r.ControlPoints[r.ControlPoints.Count-1])/2, camLookAt))
            {
                r.ControlPoints.Insert(r.ControlPoints.Count, r.ControlPoints[r.ControlPoints.Count - 1] + (r.ropeEnd.transform.position - r.ControlPoints[r.ControlPoints.Count - 1]) / 2);
                currentActiveHandle = r.ControlPoints.Count - 1;
            }

            for (int i = 0; i < r.ControlPoints.Count - 1; i++)
            {
                Vector3 btnPos = r.ControlPoints[i] + ((r.ControlPoints[i + 1] - r.ControlPoints[i]) / 2);

                if (PlusButton(btnPos, camLookAt))
                {
                    r.ControlPoints.Insert(i + 1, btnPos);
                    currentActiveHandle = i + 1;
                }
            }
        }

        // ==================
        if (r.ControlPoints == null)
        {
            UpdateRope();
            return;
        }

        // =================
        for (int i = 0; i < r.ControlPoints.Count; i++)
        {
            Handles.color = new Color(0, 0, 0, 1);
            if (Handles.Button(r.ControlPoints[i], camLookAt, 0.5f * guiScale, 0.5f * guiScale, Handles.CircleCap))
                currentActiveHandle = i;

            if (i == currentActiveHandle)
            {
                Handles.color = new Color(0.3f, 0.3f, 0.8f);
                Handles.DrawSolidDisc(r.ControlPoints[i], camNormal, 0.4f * guiScale);

                Handles.color = Color.white;
                r.ControlPoints[i] = Handles.PositionHandle(r.ControlPoints[i], Quaternion.identity);

                if (Event.current.type == EventType.keyDown)
                {
                    if (Event.current.keyCode == KeyCode.P)
                    {
                        if (i > 0 && i < r.ControlPoints.Count - 2)
                            r.ControlPoints.Insert(i + 1, r.ControlPoints[i] + ((r.ControlPoints[i + 1] - r.ControlPoints[i]) / 2));
                        //UpdateRope();
                    }

                    if (Event.current.keyCode == KeyCode.Backspace)
                    {
                        r.ControlPoints.RemoveAt(currentActiveHandle);

                        currentActiveHandle--;
                        if (currentActiveHandle == -1 && r.ControlPoints.Count != 0)
                            currentActiveHandle = 0;

                        r.ControlPoints.TrimExcess();
                        UpdateRope();
                        return;
                    }
                }
            }
            else
            {
                Handles.color = new Color(0.2f, 0.2f, 0.4f, 0.8f);
                Handles.DrawSolidDisc(r.ControlPoints[i], camNormal, 0.4f * guiScale);
            }
        }

        //Deal with rope end
        {
            Handles.color = new Color(0, 0, 0, 1);
            if (Handles.Button(r.ropeEnd.transform.position, camLookAt, 0.5f * guiScale, 0.5f * guiScale, Handles.CircleCap))
                currentActiveHandle = 5000;

            if (currentActiveHandle == 5000)

            {
                Handles.color = new Color(0.3f, 0.3f, 0.8f);
                Handles.DrawSolidDisc(r.ropeEnd.transform.position, camNormal, 0.4f * guiScale);

                Handles.color = Color.white;
                r.ropeEnd.transform.position = Handles.PositionHandle(r.ropeEnd.transform.position, Quaternion.identity);
            }
            else
            {
                Handles.color = new Color(0.2f, 0.2f, 0.4f, 0.8f);
                Handles.DrawSolidDisc(r.ropeEnd.transform.position, camNormal, 0.4f * guiScale);
            }
        }

        foreach (RopeAttachedObject ao in r.attachedObjects)
        {
            if (ao.go == null)
                continue;

            if (ao.jointIndex > r.Joints.Count - 1)
                ao.jointIndex = r.Joints.Count - 1;

            Handles.color = new Color(0.8f, 0.4f, 0f, 0.8f);
            Handles.DrawSolidDisc(ao.go.transform.position, camNormal, 0.2f * guiScale);
            Handles.DrawSolidDisc(r.Joints[ao.jointIndex].transform.position, camNormal, 0.2f * guiScale);

            float dist = Vector3.Distance(r.Joints[ao.jointIndex].transform.position, ao.go.transform.position);

            if (dist > 0.5f)
                Handles.ArrowCap(0, ao.go.transform.position, Quaternion.LookRotation((r.Joints[ao.jointIndex].transform.position - ao.go.transform.position).normalized), dist * 0.8f);

            Handles.color = new Color(0.8f, 0f, 0f, 0.8f);
            if (ao.jointType == RopeAttachmentJointType.Hinge && (ao.hingeAxis != Vector3.zero))
                Handles.ArrowCap(0, ao.go.transform.position, ao.go.transform.rotation * Quaternion.LookRotation(ao.hingeAxis), 1);
        }

        winRect = GUI.Window(0, winRect, EditWindowFunc, "Edit Selected Control Point");

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            UpdateRope();
        }
        else if (QuickRope2Helper.HasMoved(ref prevRopePos, r.transform.position))
            UpdateRope();
    }

    void UpdateRope()
    {
        r.jointSpacing = Mathf.Clamp(r.jointSpacing, QuickRope2.MIN_JOINT_SPACING, QuickRope2.MAX_JOINT_SPACING);
        if (r.jointSpacing >= QuickRope2.MIN_JOINT_SPACING)
        {
            r.ApplyRopeSettings(); //r.SetControlPoints(r.ControlPoints);
        }
    }

    bool PlusButton(Vector3 position, Quaternion direction)
    {
        Handles.color = new Color(0, 0.8f, 0, 1f);
        Handles.DotCap(0, position, direction, 0.2f * guiScale);
        return Handles.Button(position, direction, 0.25f * guiScale, 0.25f * guiScale, Handles.RectangleCap);
    }

    Vector3 curSelVect = Vector3.zero;
    static Rect winRect = new Rect(5, 25, 260, 125);
    void EditWindowFunc(int winID)
    {
        if (currentActiveHandle < 0)
        {
            GUI.Label(new Rect(45, 55, 260, 125), "Add or select a control point!");
            return;
        }

        if (currentActiveHandle == 5000)
            curSelVect = r.ropeEnd.transform.position;
        else
            curSelVect = r.ControlPoints[currentActiveHandle];

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(30);
        EditorGUILayout.LabelField("X: ", GUILayout.Width(50));
        float x = EditorGUILayout.FloatField(curSelVect.x, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(30);
        EditorGUILayout.LabelField("Y: ", GUILayout.Width(50));
        float y = EditorGUILayout.FloatField(curSelVect.y, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(30);
        EditorGUILayout.LabelField("Z: ", GUILayout.Width(50));
        float z = EditorGUILayout.FloatField(curSelVect.z, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();

        if (currentActiveHandle == 5000)
        {
            r.ropeEnd.transform.position = new Vector3(x, y, z);
            GUI.enabled = false;
        }
        else
        {
            r.ControlPoints[currentActiveHandle] = new Vector3(x, y, z);
        }

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Average"))
        {
            if (r.ControlPoints.Count == 1)
            {
                r.ControlPoints[currentActiveHandle] = (r.ropeEnd.transform.position - r.transform.position) / 2f;
            }
            else
            {
                if (currentActiveHandle > 0 && currentActiveHandle < (r.ControlPoints.Count-1))
                {
                    r.ControlPoints[currentActiveHandle] = (r.ControlPoints[currentActiveHandle - 1] + r.ControlPoints[currentActiveHandle + 1]) / 2f;
                }
                else if(currentActiveHandle == 0)
                {
                    r.ControlPoints[currentActiveHandle] = (r.transform.position + r.ControlPoints[currentActiveHandle + 1]) / 2f;
                }
                else if(currentActiveHandle == (r.ControlPoints.Count-1))
                {
                    r.ControlPoints[currentActiveHandle] = (r.ControlPoints[currentActiveHandle - 1] + r.ropeEnd.transform.position) / 2f;
                }
            }

            UpdateRope();
        }
        if (GUILayout.Button("Zero Vector"))
        {
            r.ControlPoints[currentActiveHandle] = Vector3.zero;
            UpdateRope();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Delete [Backspace]"))
        {
            r.ControlPoints.RemoveAt(currentActiveHandle);

            currentActiveHandle--;
            if (currentActiveHandle == -1 && r.ControlPoints.Count != 0)
                currentActiveHandle = 0;

            r.ControlPoints.TrimExcess();
            UpdateRope();
        }

        if (GUI.changed)
            UpdateRope();

        GUI.enabled = true;
        GUI.DragWindow();
    }
}
