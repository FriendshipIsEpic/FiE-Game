/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for adding CGF tool menu , menu context, and gameobejct items.
*******************************************************************************************/
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CircularGravityForce
{
    public class CGF_Tool : EditorWindow
    {
        #region Enumes

        enum CGFOptions
        {
            _3D,
            _2D
        }

        //Constructor
        public CGF_Tool()
        {
        }

        #endregion

        #region MenuItems / ToolBar

        [MenuItem("GameObject/3D Object/CGF", false, 0)]
        public static void GameObject_3D_Object_CGF()
        {
            CreateSimpleCGF();
        }

        [MenuItem("GameObject/2D Object/CGF 2D", false, 0)]
        public static void GameObject_2D_Object_CGF()
        {
            CreateSimpleCGF2D();
        }

        [MenuItem("CONTEXT/CGF/Effects->Add 'AlignToForce'", false)]
        public static void AddAlignToForce(MenuCommand command)
        {
            CGF cgf = (CGF)command.context;
            cgf.gameObject.AddComponent<CGF_AlignToForce>();
        }

        [MenuItem("CONTEXT/CGF/Effects->Add 'NoGravity'", false)]
        public static void AddNoGravity(MenuCommand command)
        {
            CGF cgf = (CGF)command.context;
            cgf.gameObject.AddComponent<CGF_NoGravity>();
        }

        [MenuItem("CONTEXT/CGF/Mods->Add 'Pulse'", false)]
        public static void AddPulse(MenuCommand command)
        {
            CGF cgf = (CGF)command.context;
            cgf.gameObject.AddComponent<CGF_Pulse>();
        }

        [MenuItem("CONTEXT/CGF/Mods->Add 'SizeByRaycast'", false)]
        public static void AddSizeByRaycast(MenuCommand command)
        {
            CGF cgf = (CGF)command.context;
            cgf.gameObject.AddComponent<CGF_SizeByRaycast>();
        }

        [MenuItem("CONTEXT/CGF2D/Effects->Add 2D 'AlignToForce'", false)]
        public static void AddAlignToForce2D(MenuCommand command)
        {
            CGF2D cgf = (CGF2D)command.context;
            cgf.gameObject.AddComponent<CGF_AlignToForce2D>();
        }

        [MenuItem("CONTEXT/CGF2D/Effects->Add 2D 'NoGravity'", false)]
        public static void AddNoGravity2D(MenuCommand command)
        {
            CGF2D cgf = (CGF2D)command.context;
            cgf.gameObject.AddComponent<CGF_NoGravity2D>();
        }

        [MenuItem("CONTEXT/CGF2D/Mods->Add 2D 'Pulse'", false)]
        public static void AddPulse2D(MenuCommand command)
        {
            CGF2D cgf = (CGF2D)command.context;
            cgf.gameObject.AddComponent<CGF_Pulse2D>();
        }

        [MenuItem("CONTEXT/CGF2D/Mods->Add 2D 'SizeByRaycast'", false)]
        public static void AddSizeByRaycast2D(MenuCommand command)
        {
            CGF2D cgf = (CGF2D)command.context;
            cgf.gameObject.AddComponent<CGF_SizeByRaycast2D>();
        }

        [MenuItem("CONTEXT/CGF/Triggers->Add 'Enable'", false)]
        static void CONTEXT_CircularGravity_Create_Enable(MenuCommand command)
        {
            CGF cgf = (CGF)command.context;
            CreateEnableTrigger(cgf.gameObject, cgf);
        }

        [MenuItem("CONTEXT/CGF2D/Triggers->Add 2D 'Enable'", false)]
        static void CONTEXT_CircularGravity2D_Create_Enable(MenuCommand command)
        {
            CGF2D cgf = (CGF2D)command.context;
            CreateEnableTrigger2D(cgf.gameObject, cgf);
        }

        [MenuItem("CONTEXT/CGF/Triggers->Add 'Hover'", false)]
        static void CONTEXT_CircularGravity_Create_Hover(MenuCommand command)
        {
            CGF cgf = (CGF)command.context;
            CreateHoverTrigger(cgf.gameObject, cgf);
        }

        [MenuItem("CONTEXT/CGF2D/Triggers->Add 2D 'Hover'", false)]
        static void CONTEXT_CircularGravity2D_Create_Hover(MenuCommand command)
        {
            CGF2D cgf = (CGF2D)command.context;
            CreateHoverTrigger2D(cgf.gameObject, cgf);
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Quick CGF &q", false, 1)]
        public static void QuickCGF()
        {
            CreateSimpleCGF();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Quick CGF 2D #&q", false, 2)]
        public static void QuickCGF2D()
        {
            CreateSimpleCGF2D();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Effects/Add 'AlignToForce'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Effects/Add 'NoGravity'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Mods/Add 'Pulse'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Mods/Add 'SizeByRaycast'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/Add 'Enable'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/Add 'Hover'", true)]
        public static bool CGF_Validation()
        {
            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponent<CGF>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Effects/Add 2D 'AlignToForce'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Effects/Add 2D 'NoGravity'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Mods/Add 2D 'Pulse'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Mods/Add 2D 'SizeByRaycast'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/Add 2D 'Enable'", true)]
        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/Add 2D 'Hover'", true)]
        public static bool CGF_Validation2D()
        {
            if (Selection.activeGameObject != null)
            {
                if (Selection.activeGameObject.GetComponent<CGF2D>() != null)
                {
                    return true;
                }
            }

            return false;
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Effects/Add 'AlignToForce'", false)]
        public static void AddAlignToForce()
        {
            Selection.activeGameObject.AddComponent<CGF_AlignToForce>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Effects/Add 'NoGravity'", false)]
        public static void AddNoGravity()
        {
            Selection.activeGameObject.AddComponent<CGF_NoGravity>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Mods/Add 'Pulse'", false)]
        public static void AddPulse()
        {
            Selection.activeGameObject.AddComponent<CGF_Pulse>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Mods/Add 'SizeByRaycast'", false)]
        public static void AddSizeByRaycast()
        {
            Selection.activeGameObject.AddComponent<CGF_SizeByRaycast>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Effects/Add 2D 'AlignToForce'", false)]
        public static void AddAlignToForce2D()
        {
            Selection.activeGameObject.AddComponent<CGF_AlignToForce2D>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Effects/Add 2D 'NoGravity'", false)]
        public static void AddNoGravity2D()
        {
            Selection.activeGameObject.AddComponent<CGF_NoGravity2D>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Mods/Add 2D 'Pulse'", false)]
        public static void Add2DPulse()
        {
            Selection.activeGameObject.AddComponent<CGF_Pulse2D>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Mods/Add 2D 'SizeByRaycast'", false)]
        public static void Add2DSizeByRaycast()
        {
            Selection.activeGameObject.AddComponent<CGF_SizeByRaycast2D>();
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/Add 'Enable'", false)]
        public static void Trigger_AddEnable()
        {
            bool isCreated = false;

            if (Selection.activeGameObject != null)
            {
                var selectedObject = Selection.activeGameObject;

                if (selectedObject.GetComponent<CGF>() != null)
                {
                    var cgf = selectedObject;
                    var circularGravity = selectedObject.GetComponent<CGF>();

                    CreateEnableTrigger(cgf, circularGravity);

                    isCreated = true;
                }
            }
            if (!isCreated)
            {
                var cgf = CreateSimpleCGF();
                CreateEnableTrigger(cgf, cgf.GetComponent<CGF>());
            }
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/Add 'Hover'", false)]
        public static void Trigger_AddHover()
        {
            bool isCreated = false;

            if (Selection.activeGameObject != null)
            {
                var selectedObject = Selection.activeGameObject;

                if (selectedObject.GetComponent<CGF>() != null)
                {
                    var cgf = selectedObject;
                    var circularGravity = selectedObject.GetComponent<CGF>();

                    CreateHoverTrigger(cgf, circularGravity);

                    isCreated = true;
                }
            }
            if (!isCreated)
            {
                var cgf = CreateSimpleCGF();
                CreateHoverTrigger(cgf, cgf.GetComponent<CGF>());
            }
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/Add 2D 'Enable'", false)]
        public static void Trigger2D_AddEnable()
        {
            bool isCreated = false;

            if (Selection.activeGameObject != null)
            {
                var selectedObject = Selection.activeGameObject;

                if (selectedObject.GetComponent<CGF2D>() != null)
                {
                    var cgf = selectedObject;
                    var circularGravity = selectedObject.GetComponent<CGF2D>();

                    CreateEnableTrigger2D(cgf, circularGravity);

                    isCreated = true;
                }
            }
            if (!isCreated)
            {
                var cgf = CreateSimpleCGF2D();
                CreateEnableTrigger2D(cgf, cgf.GetComponent<CGF2D>());
            }
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Triggers/Add 2D 'Hover'", false)]
        public static void Trigger2D_AddHover()
        {
            bool isCreated = false;

            if (Selection.activeGameObject != null)
            {
                var selectedObject = Selection.activeGameObject;

                if (selectedObject.GetComponent<CGF2D>() != null)
                {
                    var cgf = selectedObject;
                    var circularGravity = selectedObject.GetComponent<CGF2D>();

                    CreateHoverTrigger2D(cgf, circularGravity);

                    isCreated = true;
                }
            }
            if (!isCreated)
            {
                var cgf = CreateSimpleCGF2D();
                CreateHoverTrigger2D(cgf, cgf.GetComponent<CGF2D>());
            }
        }

        [MenuItem("Tools/Resurgam Studios/CGF/Support/Unity Form", false)]
        public static void SupportUnityForm()
        {
            Application.OpenURL("http://forum.unity3d.com/threads/circular-gravity-force.217100/");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Support/Manual", false)]
        public static void SupportManual()
        {
            Application.OpenURL("https://www.resurgamstudios.com/cgf-manual");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Support/Asset Store", false)]
        public static void SupportAssetStore()
        {
            Application.OpenURL("https://www.assetstore.unity3d.com/#!/content/8181");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Support/Website", false)]
        public static void SupportWebsite()
        {
            Application.OpenURL("https://www.resurgamstudios.com/cgf");
        }
        [MenuItem("Tools/Resurgam Studios/CGF/Support/Version History", false)]
        public static void SupportVersionHistory()
        {
            Application.OpenURL("https://www.resurgamstudios.com/cgf-versionhistory");
        }

        #endregion

        #region Events

        private static GameObject CreateSimpleCGF()
        {
            var cgf = CGF.CreateCGF();

            FocusGameObject(cgf);

            return cgf;
        }
        private static GameObject CreateSimpleCGF2D()
        {
            var cgf = CGF2D.CreateCGF();

            FocusGameObject(cgf, true);

            return cgf;
        }

        private static void CreateEnableTrigger(GameObject cgf = null, CGF circularGravity = null)
        {
            GameObject triggerEnableObj = new GameObject();
            triggerEnableObj.name = "Trigger Enable";
            if (circularGravity != null)
            {
                triggerEnableObj.AddComponent<CGF_EnableTrigger>().Cgf = circularGravity;
            }
            else
            {
                triggerEnableObj.AddComponent<CGF_EnableTrigger>();
            }
            if (cgf != null)
            {
                triggerEnableObj.transform.SetParent(cgf.transform, false);
            }
            triggerEnableObj.transform.position = triggerEnableObj.transform.position + Vector3.right * 6f;
            triggerEnableObj.transform.rotation = Quaternion.Euler(0, 90, 0);

            if (cgf == null)
            {
                FocusGameObject(triggerEnableObj);
            }
        }

        private static void CreateHoverTrigger(GameObject cgf = null, CGF circularGravity = null)
        {
            GameObject triggerEnableObj = new GameObject();
            triggerEnableObj.name = "Trigger Hover";
            if (circularGravity != null)
            {
                triggerEnableObj.AddComponent<CGF_HoverTrigger>().Cgf = circularGravity;
            }
            else
            {
                triggerEnableObj.AddComponent<CGF_HoverTrigger>();
            }
            if (cgf != null)
            {
                triggerEnableObj.transform.SetParent(cgf.transform, false);
            }
            triggerEnableObj.transform.position = triggerEnableObj.transform.position + Vector3.left * 6f;
            triggerEnableObj.transform.rotation = Quaternion.Euler(-180, 0, 0);

            if (cgf == null)
            {
                FocusGameObject(triggerEnableObj);
            }
        }

        private static void CreateEnableTrigger2D(GameObject cgf = null, CGF2D circularGravity = null)
        {
            GameObject triggerEnableObj = new GameObject();
            triggerEnableObj.name = "Trigger Enable";
            if (circularGravity != null)
            {
                triggerEnableObj.AddComponent<CGF_EnableTrigger2D>().Cgf = circularGravity;
            }
            else
            {
                triggerEnableObj.AddComponent<CGF_EnableTrigger2D>();
            }
            if (cgf != null)
            {
                triggerEnableObj.transform.SetParent(cgf.transform, false);
            }
            triggerEnableObj.transform.position = triggerEnableObj.transform.position + new Vector3(0, 6f);

            if (cgf == null)
            {
                FocusGameObject(triggerEnableObj, true);
            }
        }

        private static void CreateHoverTrigger2D(GameObject cgf = null, CGF2D circularGravity = null)
        {
            GameObject triggerEnableObj = new GameObject();
            triggerEnableObj.name = "Trigger Hover";
            if (circularGravity != null)
            {
                triggerEnableObj.AddComponent<CGF_HoverTrigger2D>().Cgf = circularGravity;
            }
            else
            {
                triggerEnableObj.AddComponent<CGF_HoverTrigger2D>();
            }
            if (cgf != null)
            {
                triggerEnableObj.transform.SetParent(cgf.transform, false);
            }
            triggerEnableObj.transform.position = triggerEnableObj.transform.position + new Vector3(0, -6f);
            triggerEnableObj.transform.rotation = Quaternion.Euler(0, 0, 180f);

            if (cgf == null)
            {
                FocusGameObject(triggerEnableObj, true);
            }
        }

        private static void FocusGameObject(GameObject focusGameObject, bool in2D = false)
        {
            //Sets the create location for the Circular Gravity Force gameobject
            if (SceneView.lastActiveSceneView != null)
            {
                if (in2D)
                    focusGameObject.transform.position = new Vector3(SceneView.lastActiveSceneView.pivot.x, SceneView.lastActiveSceneView.pivot.y, 0f);
                else
                    focusGameObject.transform.position = SceneView.lastActiveSceneView.pivot;

                //Sets the Circular Gravity Force gameobject selected in the hierarchy
                Selection.activeGameObject = focusGameObject;

                //focus the editor camera on the Circular Gravity Force gameobject
                SceneView.lastActiveSceneView.FrameSelected();
            }
            else
            {
                focusGameObject.transform.position = Vector3.zero;
            }
        }

        #endregion
    }
}