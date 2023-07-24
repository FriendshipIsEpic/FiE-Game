/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used overwriting the Inspector GUI, and Scene GUI
*******************************************************************************************/
using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using CircularGravityForce;

namespace CircularGravityForce
{
    [CustomEditor(typeof(CGF2D)), CanEditMultipleObjects]
    public class CGF2D_Editor : Editor
    {
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private TitleImage titleImage;

        private SerializedProperty enable_property;
        private SerializedProperty shape_property;
        private SerializedProperty forceType_property;
        private SerializedProperty forceMode_property;
        private SerializedProperty projectRight_property;
        private SerializedProperty forcePower_property;
        private SerializedProperty velocityDamping_property;
        private SerializedProperty angularVelocityDamping_property;
        private SerializedProperty size_property;
        private SerializedProperty boxSize_property;

        private SerializedProperty toggleTransformProperties;
        private SerializedProperty transformProperties_overridePosition;
        private SerializedProperty transformProperties_localPosition;
        private SerializedProperty transformProperties_positionValue;
        private SerializedProperty transformProperties_overrideRotation;
        private SerializedProperty transformProperties_localRotation;
        private SerializedProperty transformProperties_rotationValue;

        private SerializedProperty toggleForcePositionProperties;
        private SerializedProperty forcePositionProperties_forcePosition;
        private SerializedProperty forcePositionProperties_closestColliders;
        private SerializedProperty forcePositionProperties_useEffectedClosestPoint;
        private SerializedProperty forcePositionProperties_heightOffset;

        private SerializedProperty toggleEventProperties;
        private SerializedProperty eventProperties_enableEvents;
        private SerializedProperty eventProperties_enableSendMessage;

        private SerializedProperty toggleMemoryProperties_property;
        private SerializedProperty memoryProperties_colliderLayerMask_property;
        private SerializedProperty memoryProperties_seeAffectedColliders_property;
        private SerializedProperty memoryProperties_seeAffectedRaycastHits_property;
        private SerializedProperty memoryProperties_nonAllocPhysics_property;
        private SerializedProperty memoryProperties_colliderBuffer_property;
        private SerializedProperty memoryProperties_raycastHitBuffer_property;

        private SerializedProperty colliderListCount_property;
        private SerializedProperty raycastHitListCount_property;

        private CGF2D cgf;

        private bool change = false;
        private float spacing = 10f;

        private Color redBar = new Color(1f, 0f, 0f, .4f);

        private string helpText;
        private MessageType messageType = MessageType.Info;

        public void OnEnable()
        {
            //Title Image Resource
            titleImage = new TitleImage("Assets/ResurgamStudios/CircularGravityForce Package/Gizmos/CGF Title.png");

            enable_property = serializedObject.FindProperty("enable");
            shape_property = serializedObject.FindProperty("shape2D");
            forceType_property = serializedObject.FindProperty("forceType2D");
            forceMode_property = serializedObject.FindProperty("forceMode2D");
            projectRight_property = serializedObject.FindProperty("projectRight");
            forcePower_property = serializedObject.FindProperty("forcePower");
            velocityDamping_property = serializedObject.FindProperty("velocityDamping");
            angularVelocityDamping_property = serializedObject.FindProperty("angularVelocityDamping");
            size_property = serializedObject.FindProperty("size");
            boxSize_property = serializedObject.FindProperty("boxSize");

            toggleTransformProperties = serializedObject.FindProperty("transformProperties.toggleTransformProperties");
            transformProperties_overridePosition = serializedObject.FindProperty("transformProperties.overridePosition");
            transformProperties_localPosition = serializedObject.FindProperty("transformProperties.localPosition");
            transformProperties_positionValue = serializedObject.FindProperty("transformProperties.positionValue");
            transformProperties_overrideRotation = serializedObject.FindProperty("transformProperties.overrideRotation");
            transformProperties_localRotation = serializedObject.FindProperty("transformProperties.localRotation");
            transformProperties_rotationValue = serializedObject.FindProperty("transformProperties.rotationValue");

            toggleForcePositionProperties = serializedObject.FindProperty("forcePositionProperties.toggleForcePositionProperties");
            forcePositionProperties_forcePosition = serializedObject.FindProperty("forcePositionProperties.forcePosition");
            forcePositionProperties_closestColliders = serializedObject.FindProperty("forcePositionProperties.closestColliders");
            forcePositionProperties_useEffectedClosestPoint = serializedObject.FindProperty("forcePositionProperties.useEffectedClosestPoint");
            forcePositionProperties_heightOffset = serializedObject.FindProperty("forcePositionProperties.heightOffset");

            toggleEventProperties = serializedObject.FindProperty("eventProperties.toggleEventProperties");
            eventProperties_enableEvents = serializedObject.FindProperty("eventProperties.enableEvents");
            eventProperties_enableSendMessage = serializedObject.FindProperty("eventProperties.enableSendMessage");

            toggleMemoryProperties_property = serializedObject.FindProperty("memoryProperties.toggleMemoryProperties");
            memoryProperties_colliderLayerMask_property = serializedObject.FindProperty("memoryProperties.colliderLayerMask");
            memoryProperties_seeAffectedColliders_property = serializedObject.FindProperty("memoryProperties.seeColliders");
            memoryProperties_seeAffectedRaycastHits_property = serializedObject.FindProperty("memoryProperties.seeRaycastHits");
            memoryProperties_nonAllocPhysics_property = serializedObject.FindProperty("memoryProperties.nonAllocPhysics");
            memoryProperties_colliderBuffer_property = serializedObject.FindProperty("memoryProperties.colliderBuffer");
            memoryProperties_raycastHitBuffer_property = serializedObject.FindProperty("memoryProperties.raycastHitBuffer");
            colliderListCount_property = serializedObject.FindProperty("colliderListCount");
            raycastHitListCount_property = serializedObject.FindProperty("raycastHitListCount");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.padding = new RectOffset(panelStyle.padding.left + 10, panelStyle.padding.right, panelStyle.padding.top, panelStyle.padding.bottom);
            EditorGUILayout.BeginVertical(panelStyle);

            /*<----------------------------------------------------------------------------------------------------------*/
            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.alignment = TextAnchor.UpperCenter;
            GUILayout.BeginVertical();
            GUILayout.Box(titleImage.content, titleStyle);
            GUILayout.EndVertical();
            /*<----------------------------------------------------------------------------------------------------------*/

#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
            if (shape_property.enumValueIndex == (int)CGF2D.Shape2D.Box)
                EditorGUILayout.HelpBox(CGF.WarningMessageBoxUnity_5_3, MessageType.Warning, true);
#endif

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(enable_property, new GUIContent("Enable"));
            if (EditorGUI.EndChangeCheck())
            {
                enable_property.boolValue = EditorGUILayout.Toggle(enable_property.boolValue);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(shape_property, new GUIContent("Shape"));
            if (EditorGUI.EndChangeCheck())
            {
                shape_property.enumValueIndex = Convert.ToInt32(EditorGUILayout.EnumPopup((CGF2D.Shape2D)shape_property.enumValueIndex));
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(forceType_property, new GUIContent("Force Type"));
            if (EditorGUI.EndChangeCheck())
            {
                forceType_property.enumValueIndex = Convert.ToInt32(EditorGUILayout.EnumPopup((CGF.ForceType)forceType_property.enumValueIndex));
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            if (helpText != string.Empty)
                EditorGUILayout.HelpBox(helpText, messageType, true);

            switch (forceType_property.enumValueIndex)
            {
                case (int)CGF2D.ForceType2D.ForceAtPosition:
                    helpText = "Applies force at position. As a result this will apply a torque and force on the objects.";
                    break;
                case (int)CGF2D.ForceType2D.Force:
                    helpText = "Adds a force to the objects.";
                    break;
                case (int)CGF2D.ForceType2D.Torque:
                    helpText = "Adds a torque to the objects.";
                    break;
                case (int)CGF2D.ForceType2D.GravitationalAttraction:
                    helpText = "Adds gravitational attraction based off newtonian gravity.";
                    break;
            }

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(forceMode_property, new GUIContent("Force Mode"));
            if (EditorGUI.EndChangeCheck())
            {
                forceMode_property.enumValueIndex = Convert.ToInt32(EditorGUILayout.EnumPopup((ForceMode)forceMode_property.enumValueIndex));
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            if (shape_property.enumValueIndex != (int)CGF2D.Shape2D.Raycast)
            {
                switch (forceType_property.enumValueIndex)
                {
                    case (int)CGF2D.ForceType2D.ForceAtPosition:
                    case (int)CGF2D.ForceType2D.Force:
                    case (int)CGF2D.ForceType2D.GravitationalAttraction:
                        /*<----------------------------------------------------------------------------------------------------------*/
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(projectRight_property, new GUIContent("Project Right"));
                        if (EditorGUI.EndChangeCheck())
                        {
                            projectRight_property.boolValue = EditorGUILayout.Toggle(projectRight_property.boolValue);
                            change = true;
                        }
                        /*<----------------------------------------------------------------------------------------------------------*/

                        break;
                }
            }

            switch (forceType_property.enumValueIndex)
            {
                case (int)CGF2D.ForceType2D.ForceAtPosition:
                case (int)CGF2D.ForceType2D.Force:
                case (int)CGF2D.ForceType2D.GravitationalAttraction:
                    /*<----------------------------------------------------------------------------------------------------------*/
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(velocityDamping_property, new GUIContent("Velocity Damping"));
                    if (EditorGUI.EndChangeCheck())
                    {
                        velocityDamping_property.floatValue = EditorGUILayout.FloatField(velocityDamping_property.floatValue);
                        change = true;
                    }
                    /*<----------------------------------------------------------------------------------------------------------*/

                    break;
                case (int)CGF.ForceType.Torque:

                    /*<----------------------------------------------------------------------------------------------------------*/
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(angularVelocityDamping_property, new GUIContent("Angular Velocity Damping"));
                    if (EditorGUI.EndChangeCheck())
                    {
                        angularVelocityDamping_property.floatValue = EditorGUILayout.FloatField(angularVelocityDamping_property.floatValue);
                        change = true;
                    }
                    /*<----------------------------------------------------------------------------------------------------------*/

                    break;
            }

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(forcePower_property, new GUIContent("Force Power"));
            if (EditorGUI.EndChangeCheck())
            {
                forcePower_property.floatValue = EditorGUILayout.FloatField(forcePower_property.floatValue);
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            if (shape_property.enumValueIndex == (int)CGF2D.Shape2D.Box)
            {
                /*<----------------------------------------------------------------------------------------------------------*/
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(boxSize_property, true, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    change = true;
                }
                /*<----------------------------------------------------------------------------------------------------------*/
            }
            else
            {
                /*<----------------------------------------------------------------------------------------------------------*/
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(size_property, new GUIContent("Size"));
                if (EditorGUI.EndChangeCheck())
                {
                    if (size_property.floatValue >= 0)
                    {
                        size_property.floatValue = EditorGUILayout.FloatField(size_property.floatValue);
                    }
                    else
                    {
                        size_property.floatValue = 0;
                    }

                    change = true;
                }
                /*<----------------------------------------------------------------------------------------------------------*/
            }

            toggleTransformProperties.boolValue = EditorGUILayout.Foldout(toggleTransformProperties.boolValue, "Transform Properties");
            if (toggleTransformProperties.boolValue)
            {
                /*<----------------------------------------------------------------------------------------------------------*/
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(transformProperties_overridePosition, true, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    change = true;
                }
                GUILayout.EndHorizontal();
                /*<----------------------------------------------------------------------------------------------------------*/

                if (transformProperties_overridePosition.boolValue)
                {
                    /*<----------------------------------------------------------------------------------------------------------*/
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(transformProperties_localPosition, true, GUILayout.ExpandWidth(true));
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        change = true;
                    }
                    GUILayout.EndHorizontal();
                    /*<----------------------------------------------------------------------------------------------------------*/

                    /*<----------------------------------------------------------------------------------------------------------*/
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(transformProperties_positionValue, true, GUILayout.ExpandWidth(true));
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        change = true;
                    }
                    GUILayout.EndHorizontal();
                    /*<----------------------------------------------------------------------------------------------------------*/
                }

                /*<----------------------------------------------------------------------------------------------------------*/
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(transformProperties_overrideRotation, true, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    change = true;
                }
                GUILayout.EndHorizontal();
                /*<----------------------------------------------------------------------------------------------------------*/

                if (transformProperties_overrideRotation.boolValue)
                {
                    /*<----------------------------------------------------------------------------------------------------------*/
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(transformProperties_localRotation, true, GUILayout.ExpandWidth(true));
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        change = true;
                    }
                    GUILayout.EndHorizontal();
                    /*<----------------------------------------------------------------------------------------------------------*/

                    /*<----------------------------------------------------------------------------------------------------------*/
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(transformProperties_rotationValue, true, GUILayout.ExpandWidth(true));
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        change = true;
                    }
                    GUILayout.EndHorizontal();
                    /*<----------------------------------------------------------------------------------------------------------*/
                }
            }

            switch (forceType_property.enumValueIndex)
            {
                case (int)CGF2D.ForceType2D.ForceAtPosition:
                case (int)CGF2D.ForceType2D.Force:
                case (int)CGF2D.ForceType2D.GravitationalAttraction:

                    toggleForcePositionProperties.boolValue = EditorGUILayout.Foldout(toggleForcePositionProperties.boolValue, "Force Position Properties");
                    if (toggleForcePositionProperties.boolValue)
                    {
                        /*<----------------------------------------------------------------------------------------------------------*/
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(forcePositionProperties_forcePosition, true, GUILayout.ExpandWidth(true));
                        if (EditorGUI.EndChangeCheck())
                        {
                            serializedObject.ApplyModifiedProperties();
                            change = true;
                        }
                        GUILayout.EndHorizontal();
                        /*<----------------------------------------------------------------------------------------------------------*/

                        if (forcePositionProperties_forcePosition.enumValueIndex == (int)CGF.ForcePosition.ClosestCollider)
                        {
                            /*<----------------------------------------------------------------------------------------------------------*/
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(forcePositionProperties_closestColliders, true, GUILayout.ExpandWidth(true));
                            if (EditorGUI.EndChangeCheck())
                            {
                                serializedObject.ApplyModifiedProperties();
                                change = true;
                            }
                            GUILayout.EndHorizontal();
                            /*<----------------------------------------------------------------------------------------------------------*/

                            /*<----------------------------------------------------------------------------------------------------------*/
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(forcePositionProperties_useEffectedClosestPoint, true, GUILayout.ExpandWidth(true));
                            if (EditorGUI.EndChangeCheck())
                            {
                                serializedObject.ApplyModifiedProperties();
                                change = true;
                            }
                            GUILayout.EndHorizontal();
                            /*<----------------------------------------------------------------------------------------------------------*/

                            /*<----------------------------------------------------------------------------------------------------------*/
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(forcePositionProperties_heightOffset, true, GUILayout.ExpandWidth(true));
                            if (EditorGUI.EndChangeCheck())
                            {
                                serializedObject.ApplyModifiedProperties();
                                change = true;
                            }
                            GUILayout.EndHorizontal();
                            /*<----------------------------------------------------------------------------------------------------------*/
                        }
                    }
                    break;
            }

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("filterProperties"), true, GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            toggleEventProperties.boolValue = EditorGUILayout.Foldout(toggleEventProperties.boolValue, "Event Properties");
            if (toggleEventProperties.boolValue)
            {
                /*<----------------------------------------------------------------------------------------------------------*/
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(eventProperties_enableEvents, true, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    change = true;
                }
                GUILayout.EndHorizontal();
                /*<----------------------------------------------------------------------------------------------------------*/

                if (eventProperties_enableEvents.boolValue)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.TextArea("//Enable Events Example:\n\nusing CircularGravityForce;\n\nvoid Awake()\n{\n   CGF2D.OnApplyCGFEvent += CGF_OnApplyCGFEvent;\n}\n\nvoid OnDestroy()\n{\n   CGF2D.OnApplyCGFEvent -= CGF_OnApplyCGFEvent;\n}\n\nprivate void CGF_OnApplyCGFEvent(CGF2D cgf, Rigidbody2D rigid, Collider2D coll)\n{\n   Debug.Log(\"Hello World\");\n}", GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));
                    GUILayout.EndHorizontal();
                }

                /*<----------------------------------------------------------------------------------------------------------*/
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(eventProperties_enableSendMessage, true, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    change = true;
                }
                GUILayout.EndHorizontal();
                /*<----------------------------------------------------------------------------------------------------------*/

                if (eventProperties_enableSendMessage.boolValue)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.TextArea("//Enable Events Example:\n\nusing CircularGravityForce;\n\nprivate void OnApplyCGF(CGF2D cgf)\n{\n   Debug.Log(\"Hello World\");\n}\n", GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));
                    GUILayout.EndHorizontal();
                }
            }

            /*<----------------------------------------------------------------------------------------------------------*/
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("drawGravityProperties"), true, GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                change = true;
            }
            /*<----------------------------------------------------------------------------------------------------------*/

            toggleMemoryProperties_property.boolValue = EditorGUILayout.Foldout(toggleMemoryProperties_property.boolValue, "Memory Properties");
            if (toggleMemoryProperties_property.boolValue)
            {
                /*<----------------------------------------------------------------------------------------------------------*/
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(memoryProperties_colliderLayerMask_property, true, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    change = true;
                }
                GUILayout.EndHorizontal();
                /*<----------------------------------------------------------------------------------------------------------*/

                if (shape_property.enumValueIndex == (int)CGF.Shape.Sphere || shape_property.enumValueIndex == (int)CGF.Shape.Box)
                {
                    /*<----------------------------------------------------------------------------------------------------------*/
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(memoryProperties_seeAffectedColliders_property, true, GUILayout.ExpandWidth(true));
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        change = true;
                    }
                    GUILayout.EndHorizontal();
                    /*<----------------------------------------------------------------------------------------------------------*/
                }

                if (shape_property.enumValueIndex == (int)CGF.Shape.Capsule || shape_property.enumValueIndex == (int)CGF.Shape.Raycast)
                {
                    /*<----------------------------------------------------------------------------------------------------------*/
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(memoryProperties_seeAffectedRaycastHits_property, true, GUILayout.ExpandWidth(true));
                    if (EditorGUI.EndChangeCheck())
                    {
                        serializedObject.ApplyModifiedProperties();
                        change = true;
                    }
                    GUILayout.EndHorizontal();
                    /*<----------------------------------------------------------------------------------------------------------*/
                }

                if (Application.isPlaying)
                    GUI.enabled = false;

                /*<----------------------------------------------------------------------------------------------------------*/
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(memoryProperties_nonAllocPhysics_property, true, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    change = true;
                }
                GUILayout.EndHorizontal();
                /*<----------------------------------------------------------------------------------------------------------*/

                GUI.enabled = true;

                if (memoryProperties_nonAllocPhysics_property.boolValue)
                {

                    if (shape_property.enumValueIndex == (int)CGF2D.Shape2D.Sphere)
                    {
                        if (Application.isPlaying)
                            GUI.enabled = false;

                        /*<----------------------------------------------------------------------------------------------------------*/
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(memoryProperties_colliderBuffer_property, true, GUILayout.ExpandWidth(true));
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (memoryProperties_colliderBuffer_property.intValue >= 1)
                            {
                                memoryProperties_colliderBuffer_property.intValue = EditorGUILayout.IntField(memoryProperties_colliderBuffer_property.intValue);
                            }
                            else
                            {
                                memoryProperties_colliderBuffer_property.intValue = 1;
                            }

                            change = true;
                        }
                        GUILayout.EndHorizontal();
                        /*<----------------------------------------------------------------------------------------------------------*/

                        GUI.enabled = true;

                        float collidersUsed = (float)colliderListCount_property.intValue;
                        float collidersNotUsed = (float)memoryProperties_colliderBuffer_property.intValue;
                        float collidersUsedPercent = collidersUsed / collidersNotUsed;

                        if (collidersUsedPercent == 1f)
                            GUI.backgroundColor = redBar;

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.ProgressBar(EditorGUILayout.BeginVertical(), collidersUsedPercent, string.Format("{0} : {1}", collidersUsed, collidersNotUsed));
                        GUILayout.Space(16);
                        EditorGUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                    }

                    if (shape_property.enumValueIndex == (int)CGF2D.Shape2D.Raycast || shape_property.enumValueIndex == (int)CGF2D.Shape2D.Box)
                    {
                        if (Application.isPlaying)
                            GUI.enabled = false;

                        /*<----------------------------------------------------------------------------------------------------------*/
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(memoryProperties_raycastHitBuffer_property, true, GUILayout.ExpandWidth(true));
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (memoryProperties_raycastHitBuffer_property.intValue >= 1)
                            {
                                memoryProperties_raycastHitBuffer_property.intValue = EditorGUILayout.IntField(memoryProperties_raycastHitBuffer_property.intValue);
                            }
                            else
                            {
                                memoryProperties_raycastHitBuffer_property.intValue = 1;
                            }

                            change = true;
                        }
                        GUILayout.EndHorizontal();
                        /*<----------------------------------------------------------------------------------------------------------*/

                        GUI.enabled = true;

                        float raycastHitsUsed = (float)raycastHitListCount_property.intValue;
                        float raycastHitsNotUsed = (float)memoryProperties_raycastHitBuffer_property.intValue;
                        float raycastHitsUsedPercent = raycastHitsUsed / raycastHitsNotUsed;

                        if (raycastHitsUsedPercent == 1f)
                            GUI.backgroundColor = redBar;

                        if (raycastHitsUsedPercent == 1f)
                            GUI.color = Color.magenta;

                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("", GUILayout.Width(spacing));
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.ProgressBar(EditorGUILayout.BeginVertical(), raycastHitsUsedPercent, string.Format("{0} : {1}", raycastHitsUsed, raycastHitsNotUsed));
                        GUILayout.Space(16);
                        EditorGUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                    }
                }
            }

            EditorGUILayout.EndVertical();

            if (change)
            {
                change = false;
            }

            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI()
        {
            cgf = (CGF2D)target;

            if (cgf._drawGravityProperties.DrawGizmo)
            {

                Color mainColor;
                Color tranMainColor;

                if (cgf.Enable)
                {
                    if (cgf.ForcePower == 0)
                    {
                        mainColor = Color.white;
                        tranMainColor = Color.white;
                    }
                    else if (cgf.ForcePower > 0)
                    {
                        mainColor = Color.green;
                        tranMainColor = Color.green;
                    }
                    else
                    {
                        mainColor = Color.red;
                        tranMainColor = Color.red;
                    }
                }
                else
                {
                    mainColor = Color.white;
                    tranMainColor = Color.white;
                }

                tranMainColor.a = .1f;

                Handles.color = mainColor;

                float gizmoSize = 0f;
                float gizmoOffset = 0f;

                if (mainColor == Color.green)
                {
                    gizmoSize = (cgf.Size / 8f);
                    if (gizmoSize > .5f)
                        gizmoSize = .5f;
                    else if (gizmoSize < -.5f)
                        gizmoSize = -.5f;
                    gizmoOffset = -gizmoSize / 1.5f;
                }
                else if (mainColor == Color.red)
                {
                    gizmoSize = -(cgf.Size / 8f);
                    if (gizmoSize > .5f)
                        gizmoSize = .5f;
                    else if (gizmoSize < -.5f)
                        gizmoSize = -.5f;
                    gizmoOffset = gizmoSize / 1.5f;
                }

                Quaternion qUp = cgf.transform.transform.rotation;
                qUp.SetLookRotation(cgf.transform.rotation * Vector3.up);
                Quaternion qDown = cgf.transform.transform.rotation;
                qDown.SetLookRotation(cgf.transform.rotation * Vector3.down);
                Quaternion qLeft = cgf.transform.transform.rotation;
                qLeft.SetLookRotation(cgf.transform.rotation * Vector3.forward);
                Quaternion qRight = cgf.transform.transform.rotation;
                qRight.SetLookRotation(cgf.transform.rotation * Vector3.back);
                Quaternion qForward = cgf.transform.transform.rotation;
                qForward.SetLookRotation(cgf.transform.rotation * Vector3.right);
                Quaternion qBack = cgf.transform.transform.rotation;
                qBack.SetLookRotation(cgf.transform.rotation * Vector3.left);

                float dotSpace = 10f;
                float sizeValue = cgf.Size;
                float sizeBoxValueX = cgf.BoxSize.x;
                float sizeBoxValueY = cgf.BoxSize.y;

                switch (cgf._shape2D)
                {
                    case CGF2D.Shape2D.Sphere:

                        Handles.color = tranMainColor;
                        Handles.color = mainColor;

                        if ((cgf._forceType2D == CGF2D.ForceType2D.ForceAtPosition ||
                            cgf._forceType2D == CGF2D.ForceType2D.GravitationalAttraction ||
                            cgf._forceType2D == CGF2D.ForceType2D.Force) &&
                            !cgf._projectRight)
                        {
                            if (cgf._forcePositionProperties.ForcePosition == CGF.ForcePosition.ThisTransform)
                            {
                                DrawConeCap(GetVector(Vector3.up, cgf.Size + gizmoOffset, 1f), qUp, gizmoSize);
                                DrawConeCap(GetVector(Vector3.down, cgf.Size + gizmoOffset, 1f), qDown, gizmoSize);
                                DrawConeCap(GetVector(Vector3.left, cgf.Size + gizmoOffset, 1f), qBack, gizmoSize);
                            }
                        }
                        else if (cgf._forceType2D == CGF2D.ForceType2D.Torque)
                        {
                            DrawConeCap(GetVector(Vector3.up, cgf.Size + gizmoOffset, 1f), qForward, gizmoSize);
                            DrawConeCap(GetVector(Vector3.down, cgf.Size + gizmoOffset, 1f), qBack, gizmoSize);
                            DrawConeCap(GetVector(Vector3.right, cgf.Size + gizmoOffset, 1f), qDown, gizmoSize);
                            DrawConeCap(GetVector(Vector3.left, cgf.Size + gizmoOffset, 1f), qUp, gizmoSize);
                        }
                        else
                        {
                            if (cgf._forcePositionProperties.ForcePosition == CGF.ForcePosition.ThisTransform)
                            {
                                DrawConeCap(GetVector(Vector3.left, cgf.Size + gizmoOffset, 1f), qBack, -gizmoSize);
                            }
                        }

                        if (cgf._forceType2D != CGF2D.ForceType2D.Torque)
                        {
                            if (cgf._forcePositionProperties.ForcePosition == CGF.ForcePosition.ThisTransform)
                            {
                                DrawConeCap(GetVector(Vector3.right, cgf.Size + gizmoOffset, 1f), qForward, gizmoSize);
                            }
                        }

                        Handles.DrawDottedLine(GetVector(Vector3.up, cgf.Size, 1), cgf.transform.position, dotSpace);
                        Handles.DrawDottedLine(GetVector(Vector3.down, cgf.Size, 1), cgf.transform.position, dotSpace);
                        Handles.DrawDottedLine(GetVector(Vector3.left, cgf.Size, 1), cgf.transform.position, dotSpace);
                        Handles.DrawDottedLine(GetVector(Vector3.right, cgf.Size, 1), cgf.transform.position, dotSpace);

                        DrawCircleCap(cgf.transform.position, qLeft, cgf.Size);

                        Handles.color = mainColor;
                        sizeValue = cgf.Size;
                        sizeValue = DrawScaleValueHandle(sizeValue, GetVector(Vector3.up, cgf.Size, 1f), cgf.transform.rotation, gizmoSize);
                        sizeValue = DrawScaleValueHandle(sizeValue, GetVector(Vector3.down, cgf.Size, 1f), cgf.transform.rotation, gizmoSize);
                        sizeValue = DrawScaleValueHandle(sizeValue, GetVector(Vector3.left, cgf.Size, 1f), cgf.transform.rotation, gizmoSize);
                        sizeValue = DrawScaleValueHandle(sizeValue, GetVector(Vector3.right, cgf.Size, 1f), cgf.transform.rotation, gizmoSize);
                        if (sizeValue < 0)
                            cgf.Size = 0;
                        else
                            cgf.Size = sizeValue;

                        break;
                    case CGF2D.Shape2D.Raycast:

                        Handles.DrawDottedLine(cgf.transform.position + ((cgf.transform.rotation * Vector3.right) * cgf.Size), cgf.transform.position, dotSpace);

                        if (cgf._forceType2D != CGF2D.ForceType2D.Torque)
                        {
                            if (cgf._forcePositionProperties.ForcePosition == CGF.ForcePosition.ThisTransform)
                            {
                                DrawConeCap(GetVector(Vector3.right, cgf.Size + gizmoOffset, 1f), qForward, gizmoSize);
                            }
                        }
                        else
                        {
                            DrawConeCap(GetVector(Vector3.right, cgf.Size + gizmoOffset, 1f), qDown, gizmoSize);
                        }

                        Handles.color = mainColor;
                        sizeValue = cgf.Size;
                        sizeValue = DrawScaleValueHandle(sizeValue, GetVector(Vector3.right, cgf.Size, 1f), cgf.transform.rotation, gizmoSize);
                        if (sizeValue < 0)
                            cgf.Size = 0;
                        else
                            cgf.Size = sizeValue;

                        break;
                    case CGF2D.Shape2D.Box:

                        if ((cgf._forceType2D == CGF2D.ForceType2D.ForceAtPosition ||
                            cgf._forceType2D == CGF2D.ForceType2D.GravitationalAttraction ||
                            cgf._forceType2D == CGF2D.ForceType2D.Force) &&
                            !cgf._projectRight)
                        {
                            if (cgf._forcePositionProperties.ForcePosition == CGF.ForcePosition.ThisTransform)
                            {
                                DrawConeCap(GetVector(Vector3.up, CGF_Editor.GetArrowOffsetForBox(mainColor, cgf.BoxSize.y, gizmoSize), 1f), qUp, CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.y));
                                DrawConeCap(GetVector(Vector3.down, CGF_Editor.GetArrowOffsetForBox(mainColor, cgf.BoxSize.y, gizmoSize), 1f), qDown, CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.y));
                                DrawConeCap(GetVector(Vector3.left, CGF_Editor.GetArrowOffsetForBox(mainColor, cgf.BoxSize.x, gizmoSize), 1f), qBack, CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.x));
                            }
                        }
                        else if (cgf._forceType2D == CGF2D.ForceType2D.Torque)
                        {
                            DrawConeCap(GetVector(Vector3.up, CGF_Editor.GetArrowOffsetForBox(mainColor, cgf.BoxSize.y, gizmoSize), 1f), qForward, CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.y));
                            DrawConeCap(GetVector(Vector3.down, CGF_Editor.GetArrowOffsetForBox(mainColor, cgf.BoxSize.y, gizmoSize), 1f), qBack, CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.y));
                            DrawConeCap(GetVector(Vector3.right, CGF_Editor.GetArrowOffsetForBox(mainColor, cgf.BoxSize.x, gizmoSize), 1f), qDown, CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.x));
                            DrawConeCap(GetVector(Vector3.left, CGF_Editor.GetArrowOffsetForBox(mainColor, cgf.BoxSize.x, gizmoSize), 1f), qUp, CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.x));
                        }
                        else
                        {
                            if (cgf._forcePositionProperties.ForcePosition == CGF.ForcePosition.ThisTransform)
                            {
                                DrawConeCap(GetVector(Vector3.left, CGF_Editor.GetArrowOffsetForBox(mainColor, cgf.BoxSize.x, gizmoSize), 1f), qBack, -CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.x));
                            }
                        }

                        if (cgf._forceType2D != CGF2D.ForceType2D.Torque)
                        {
                            if (cgf._forcePositionProperties.ForcePosition == CGF.ForcePosition.ThisTransform)
                            {
                                DrawConeCap(GetVector(Vector3.right, CGF_Editor.GetArrowOffsetForBox(mainColor, cgf.BoxSize.x, gizmoSize), 1f), qForward, CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.x));
                            }
                        }

                        Handles.DrawDottedLine(GetVector(Vector3.up, cgf.BoxSize.y, 1), cgf.transform.position, dotSpace);
                        Handles.DrawDottedLine(GetVector(Vector3.down, cgf.BoxSize.y, 1), cgf.transform.position, dotSpace);
                        Handles.DrawDottedLine(GetVector(Vector3.left, cgf.BoxSize.x, 1), cgf.transform.position, dotSpace);
                        Handles.DrawDottedLine(GetVector(Vector3.right, cgf.BoxSize.x, 1), cgf.transform.position, dotSpace);

                        Handles.color = mainColor;
                        sizeBoxValueX = cgf.BoxSize.x;
                        sizeBoxValueY = cgf.BoxSize.y;
                        sizeBoxValueY = DrawScaleValueHandle(sizeBoxValueY, GetVector(Vector3.up, cgf.BoxSize.y, 1f), cgf.transform.rotation, CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.y));
                        sizeBoxValueY = DrawScaleValueHandle(sizeBoxValueY, GetVector(Vector3.down, cgf.BoxSize.y, 1f), cgf.transform.rotation, CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.y));
                        sizeBoxValueX = DrawScaleValueHandle(sizeBoxValueX, GetVector(Vector3.left, cgf.BoxSize.x, 1f), cgf.transform.rotation, CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.x));
                        sizeBoxValueX = DrawScaleValueHandle(sizeBoxValueX, GetVector(Vector3.right, cgf.BoxSize.x, 1f), cgf.transform.rotation, CGF_Editor.GetGismoSizeForBox(mainColor, gizmoSize, cgf.BoxSize.x));
                        if (sizeBoxValueX < 0)
                            cgf.BoxSize = new Vector3(0f, cgf.BoxSize.y);
                        else
                            cgf.BoxSize = new Vector3(sizeBoxValueX, cgf.BoxSize.y);
                        if (sizeBoxValueY < 0)
                            cgf.BoxSize = new Vector3(cgf.BoxSize.x, 0f);

                        else
                            cgf.BoxSize = new Vector3(cgf.BoxSize.x, sizeBoxValueY);
                        break;
                }

                if (cgf._forcePositionProperties.ForcePosition == CGF.ForcePosition.ThisTransform)
                {
                    DrawSphereCap(cgf.transform.position, cgf.transform.rotation, gizmoSize / 2f);
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        Vector3 GetVector(Vector3 vector, float size, float times)
        {
            return cgf.transform.position + (((cgf.transform.rotation * vector) * size) / times);
        }

        void DrawConeCap(Vector3 position, Quaternion rotation, float gizmoSize)
        {
#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5)
            Handles.ConeCap(0, position, rotation, gizmoSize);
#else
            if (Event.current.type == EventType.Repaint)
            {
                Handles.ConeHandleCap(0, position, rotation, gizmoSize, EventType.Repaint);
            }
#endif
        }

        void DrawCircleCap(Vector3 position, Quaternion rotation, float gizmoSize)
        {
#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5)
            Handles.CircleCap(0, position, rotation, gizmoSize);
#else
            if (Event.current.type == EventType.Repaint)
            {
                Handles.CircleHandleCap(0, position, rotation, gizmoSize, EventType.Repaint);
            }
#endif
        }

        float DrawScaleValueHandle(float value, Vector3 position, Quaternion rotation, float gizmoSize)
        {
#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5)
            return Handles.ScaleValueHandle(value, position, rotation, gizmoSize, Handles.DotCap, .25f);
#else
            return Handles.ScaleValueHandle(value, position, rotation, gizmoSize, Handles.DotHandleCap, .25f);
#endif
        }

        void DrawSphereCap(Vector3 position, Quaternion rotation, float gizmoSize)
        {
#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5)
            Handles.SphereCap(0, position, rotation, gizmoSize / 2f);
#else
            if (Event.current.type == EventType.Repaint)
            {
                Handles.SphereHandleCap(0, position, rotation, gizmoSize / 2f, EventType.Repaint);
            }
#endif
        }
    }
}