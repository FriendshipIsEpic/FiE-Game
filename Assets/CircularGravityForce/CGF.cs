/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Core logic for Circular Gravity Force.
*******************************************************************************************/
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;

#if (UNITY_EDITOR)
using UnityEditor;
#endif

namespace CircularGravityForce
{
    [AddComponentMenu("Physics/Circular Gravity Force", -1)]
    public class CGF : MonoBehaviour
    {
        #region Events

        public delegate void ApplyCGFEvent(CGF cgf, Rigidbody rigid, Collider coll);
        public static event ApplyCGFEvent OnApplyCGFEvent;

        #endregion

        #region Enums

        //Force Types
        public enum ForceType
        {
            ForceAtPosition,
            Force,
            Torque,
            ExplosionForce,
            GravitationalAttraction,
        }

        //Force Types
        public enum Shape
        {
            Sphere,
            Capsule,
            Raycast,
            Box,
        }

        public enum ForcePosition
        {
            ThisTransform,
            ClosestCollider,
        }

        #endregion

        #region Classes

        //Manages all transform properties
        [System.Serializable]
        public class TransformProperties
        {
            [SerializeField, Tooltip("Used for toggling transform properties.")]
            private bool toggleTransformProperties;
            public bool ToggleTransformProperties
            {
                get { return toggleTransformProperties; }
                set { toggleTransformProperties = value; }
            }

            [SerializeField, Header("Position"), Tooltip("Toggles to override the transforms position.")]
            private bool overridePosition = false;
            public bool OverridePosition
            {
                get { return overridePosition; }
                set { overridePosition = value; }
            }
            [SerializeField, Tooltip("Toggles to use local position.")]
            private bool localPosition = false;
            public bool LocalPosition
            {
                get { return localPosition; }
                set { localPosition = value; }
            }
            [SerializeField, Tooltip("Override value for the transforms position.")]
            private Vector3 positionValue = Vector3.zero;
            public Vector3 PositionValue
            {
                get { return positionValue; }
                set { positionValue = value; }
            }

            [SerializeField, Header("Rotation"), Tooltip("Toggles to override the transforms rotation.")]
            private bool overrideRotation = false;
            public bool OverrideRotation
            {
                get { return overrideRotation; }
                set { overrideRotation = value; }
            }
            [SerializeField, Tooltip("Toggles to use local rotation.")]
            private bool localRotation = false;
            public bool LocalRotation
            {
                get { return localRotation; }
                set { localRotation = value; }
            }
            [SerializeField, Tooltip("Override value for the transforms rotation.")]
            private Vector3 rotationValue = Vector3.zero;
            public Vector3 RotationValue
            {
                get { return rotationValue; }
                set { rotationValue = value; }
            }

            //Validate the given transform.
            public void ValidateTransform(Transform transform)
            {
                if (OverridePosition)
                {
                    if (LocalPosition)
                    {
                        transform.localPosition = PositionValue;
                    }
                    else
                    {
                        transform.position = PositionValue;
                    }
                }

                if (OverrideRotation)
                {
                    if (LocalRotation)
                    {
                        transform.localRotation = Quaternion.Euler(RotationValue);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Euler(RotationValue);
                    }
                }
            }
        }

        //Manages all force position properties
        [System.Serializable]
        public class ForcePositionProperties
        {
            [SerializeField, Tooltip("Used for toggling force position properties.")]
            private bool toggleForcePositionProperties;
            public bool ToggleForcePositionProperties
            {
                get { return toggleForcePositionProperties; }
                set { toggleForcePositionProperties = value; }
            }

            [SerializeField, Tooltip("Force position options.")]
            private ForcePosition forcePosition = ForcePosition.ThisTransform;
            public ForcePosition ForcePosition
            {
                get { return forcePosition; }
                set { forcePosition = value; }
            }

            [SerializeField, Tooltip("Colliders for when using 'Closest Collider' for 'Force Position'")]
            private List<Collider> closestColliders;
            public List<Collider> ClosestColliders
            {
                get { return closestColliders; }
                set { closestColliders = value; }
            }

            [SerializeField, Tooltip("Finds the closest point on effected object and uses that as the force pivot.")]
            private bool useEffectedClosestPoint = false;
            public bool UseEffectedClosestPoint
            {
                get { return useEffectedClosestPoint; }
                set { useEffectedClosestPoint = value; }
            }

            [SerializeField, Tooltip("Height offset for closest colliders.")]
            private float heightOffset = 0f;
            public float HeightOffset
            {
                get { return heightOffset; }
                set { heightOffset = value; }
            }

            public ForcePositionProperties()
            {
                ClosestColliders = new List<Collider>();
            }
        }

        //Manages all force type properties
        [System.Serializable]
        public class ForceTypeProperties
        {
            [SerializeField, Tooltip("Adjustment to the apparent position of the explosion to make it seem to lift objects.")]
            private float explosionForceUpwardsModifier = 0f;
            public float ExplosionForceUpwardsModifier
            {
                get { return explosionForceUpwardsModifier; }
                set { explosionForceUpwardsModifier = value; }
            }

            [SerializeField, Tooltip("The maximimum angular velocity of the rigidbody.")]
            private float torqueMaxAngularVelocity = 7f;
            public float TorqueMaxAngularVelocity
            {
                get { return torqueMaxAngularVelocity; }
                set { torqueMaxAngularVelocity = value; }
            }
        }

        //Manages all filter type properties
        [System.Serializable]
        public class FilterProperties
        {
            //Filter effect types
            public enum EffectType
            {
                Effect,
                DontEffect,
            }

            //Tag filter properties
            [System.Serializable]
            public class TagFilter
            {
                [SerializeField, Tooltip("Effect type for tag filter options.")]
                private EffectType effectType = EffectType.Effect;
                public EffectType _effectType
                {
                    get { return effectType; }
                    set { effectType = value; }
                }

                [SerializeField, Tooltip("Tag name used for filter options.")]
                private string tag = string.Empty;
                public string Tag
                {
                    get { return tag; }
                    set { tag = value; }
                }
            }

            //GameObject filter properties
            [System.Serializable]
            public class GameObjectFilter
            {
                [SerializeField, Tooltip("Effect type for Gameobject filter options.")]
                private EffectType effectType = EffectType.Effect;
                public EffectType _effectType
                {
                    get { return effectType; }
                    set { effectType = value; }
                }

                [SerializeField, Tooltip("Gameobject used for filter options.")]
                private GameObject gameObject;
                public GameObject _gameObject
                {
                    get { return gameObject; }
                    set { gameObject = value; }
                }
            }

            //Collider filter properties
            [System.Serializable]
            public class BoundsFilter
            {
                [SerializeField, Tooltip("Effect type for bounds collider filter options.")]
                private EffectType effectType = EffectType.Effect;
                public EffectType _effectType
                {
                    get { return effectType; }
                    set { effectType = value; }
                }

                [SerializeField, Tooltip("Bound collider used for filter options.")]
                private Collider collider;
                public Collider _collider
                {
                    get { return collider; }
                    set { collider = value; }
                }
            }

            //Physic material filter properties
            [System.Serializable]
            public class PhysicMaterialFilter
            {
                [SerializeField, Tooltip("Effect type for physic material filter options.")]
                private EffectType effectType = EffectType.Effect;
                public EffectType _effectType
                {
                    get { return effectType; }
                    set { effectType = value; }
                }

                [SerializeField, Tooltip("Physic material used for filter options.")]
                private PhysicMaterial physicMaterial;
                public PhysicMaterial _physicMaterial
                {
                    get { return physicMaterial; }
                    set { physicMaterial = value; }
                }
            }

            // Filter Properties Constructor
            public FilterProperties()
            {
                gameObjectFilter = new List<GameObjectFilter>();
                tagFilter = new List<TagFilter>();
                boundsFilter = new List<BoundsFilter>();
                physicMaterialFilter = new List<PhysicMaterialFilter>();
            }

            [SerializeField, Tooltip("Used to filter out gameobjects, has authority over tags, colliders, physicMaterial, and layermasks.")]
            private List<GameObjectFilter> gameObjectFilter;
            public List<GameObjectFilter> _gameObjectFilter
            {
                get { return gameObjectFilter; }
                set { gameObjectFilter = value; }
            }

            [SerializeField, Tooltip("Used to filter out tags, has authority over colliders, physicMaterial, and layermasks.")]
            private List<TagFilter> tagFilter;
            public List<TagFilter> _tagFilter
            {
                get { return tagFilter; }
                set { tagFilter = value; }
            }

            [SerializeField, Tooltip("Used to filter out colliders, has authority over physicMaterial, and layermasks.")]
            public List<BoundsFilter> boundsFilter;
            public List<BoundsFilter> _boundsFilter
            {
                get { return boundsFilter; }
                set { boundsFilter = value; }
            }

            [SerializeField, Tooltip("Used to filter out physic material, has authority over layermasks.")]
            public List<PhysicMaterialFilter> physicMaterialFilter;
            public List<PhysicMaterialFilter> _physicMaterialFilter
            {
                get { return physicMaterialFilter; }
                set { physicMaterialFilter = value; }
            }

            [SerializeField, Tooltip("Used for fildering LayerMasks.")]
            private LayerMask layerMaskFilter = -1;
            public LayerMask _layerMaskFilter
            {
                get { return layerMaskFilter; }
                set { layerMaskFilter = value; }
            }

            //ValidateFilters all filter options
            public bool ValidateFilters(Rigidbody rigid, Collider coll)
            {
                bool value = true;

                if (_gameObjectFilter.Count > 0)
                {
                    for (int i = 0; i < _gameObjectFilter.Count; i++)
                    {
                        switch (_gameObjectFilter[i]._effectType)
                        {
                            case EffectType.Effect:
                                if (_gameObjectFilter[i]._gameObject == rigid.gameObject)
                                {
                                    return true;
                                }
                                break;
                            case EffectType.DontEffect:
                                if (_gameObjectFilter[i]._gameObject == rigid.gameObject)
                                {
                                    return false;
                                }
                                break;
                        }
                    }
                }

                if (_tagFilter.Count > 0)
                {
                    for (int i = 0; i < _tagFilter.Count; i++)
                    {
                        switch (_tagFilter[i]._effectType)
                        {
                            case EffectType.Effect:
                                if (rigid.transform.gameObject.CompareTag(_tagFilter[i].Tag))
                                {
                                    return true;
                                }
                                break;
                            case EffectType.DontEffect:
                                if (rigid.transform.gameObject.CompareTag(_tagFilter[i].Tag))
                                {
                                    return false;
                                }
                                break;
                        }
                    }
                }

                if (_boundsFilter.Count > 0)
                {
                    for (int i = 0; i < _boundsFilter.Count; i++)
                    {
                        switch (_boundsFilter[i]._effectType)
                        {
                            case EffectType.Effect:
                                if (_boundsFilter[i]._collider.bounds.Contains(rigid.position))
                                {
                                    return true;
                                }
                                break;
                            case EffectType.DontEffect:
                                if (_boundsFilter[i]._collider.bounds.Contains(rigid.position))
                                {
                                    return false;
                                }
                                break;
                        }
                    }
                }

                if (_physicMaterialFilter.Count > 0)
                {
                    for (int i = 0; i < _physicMaterialFilter.Count; i++)
                    {
                        switch (_physicMaterialFilter[i]._effectType)
                        {
                            case EffectType.Effect:
                                if (_physicMaterialFilter[i]._physicMaterial == coll.sharedMaterial)
                                {
                                    return true;
                                }
                                break;
                            case EffectType.DontEffect:
                                if (_physicMaterialFilter[i]._physicMaterial == coll.sharedMaterial)
                                {
                                    return false;
                                }
                                break;
                        }
                    }
                }

                if (((1 << rigid.transform.gameObject.layer) & _layerMaskFilter) != 0)
                {
                    value = true;
                }
                else if (((1 << rigid.transform.gameObject.layer) & _layerMaskFilter) == 0)
                {
                    value = false;
                }

                return value;
            }
        }

        //Manages all event properties
        [System.Serializable]
        public class EventProperties
        {
            [SerializeField, Tooltip("Used for toggling event properties.")]
            private bool toggleEventProperties;
            public bool ToggleEventProperties
            {
                get { return toggleEventProperties; }
                set { toggleEventProperties = value; }
            }

            [SerializeField, Tooltip("Enables delegate / events.")]
            private bool enableEvents = true;
            public bool EnableEvents
            {
                get { return enableEvents; }
                set { enableEvents = value; }
            }

            [SerializeField, Tooltip("Enables SendMessage. Syntax:'void OnApplyCGF(CircularGravityForce.CGF cgf)'")]
            private bool enableSendMessage = false;
            public bool EnableSendMessage
            {
                get { return enableSendMessage; }
                set { enableSendMessage = value; }
            }
        }

        //Draw gravity properties
        [System.Serializable]
        public class DrawGravityProperties
        {
            [SerializeField, Header("Runtime"), Tooltip("Enable/Disables drawing gravity force lines.")]
            private bool drawGravityForce = false;
            public bool DrawGravityForce
            {
                get { return drawGravityForce; }
                set { drawGravityForce = value; }
            }

            [SerializeField, Tooltip("Thinkness of the line drawn.")]
            private float thickness = 0.05f;
            public float Thickness
            {
                get { return thickness; }
                set { thickness = value; }
            }

            [SerializeField, Tooltip("Material on line.")]
            private Material gravityLineMaterial;
            public Material GravityLineMaterial
            {
                get { return gravityLineMaterial; }
                set { gravityLineMaterial = value; }
            }



            [SerializeField, Header("Editor"), Tooltip("Enable/Disables unity editor gizmo for CGF.")]
            private bool drawGizmo = true;
            public bool DrawGizmo
            {
                get { return drawGizmo; }
                set { drawGizmo = value; }
            }

            //Used to see gravity area from gizmos
            private bool drawGravityForceGizmos = true;
            public bool DrawGravityForceGizmos
            {
                get { return drawGravityForceGizmos; }
                set { drawGravityForceGizmos = value; }
            }
        }

        //Draw gravity properties
        [System.Serializable]
        public class MemoryProperties
        {
            [SerializeField, Tooltip("Layer mask for all effected colliders.")]
            private LayerMask colliderLayerMask = -1;
            public LayerMask ColliderLayerMask
            {
                get { return colliderLayerMask; }
                set { colliderLayerMask = value; }
            }

            [SerializeField, Tooltip("Used for toggling memory properties.")]
            private bool toggleMemoryProperties;
            public bool ToggleMemoryProperties
            {
                get { return toggleMemoryProperties; }
                set { toggleMemoryProperties = value; }
            }

            [SerializeField, Tooltip("See affected colliders in gizmo.")]
            private bool seeColliders = false;
            public bool SeeColliders
            {
                get { return seeColliders; }
                set { seeColliders = value; }
            }

            [SerializeField, Tooltip("See affected raycasthits in gizmo.")]
            private bool seeRaycastHits = false;
            public bool SeeRaycastHits
            {
                get { return seeRaycastHits; }
                set { seeRaycastHits = value; }
            }

            [SerializeField, Tooltip("Use Non-Alloc physics.")]
            private bool nonAllocPhysics = false;
            public bool NonAllocPhysics
            {
                get { return nonAllocPhysics; }
                set { nonAllocPhysics = value; }
            }

            [SerializeField, Tooltip("Collider buffer size for the Non-Alloc 'Sphere' and 'Box' types.")]
            private int colliderBuffer = 100;
            public int ColliderBuffer
            {
                get { return colliderBuffer; }
                set { colliderBuffer = value; }
            }

            [SerializeField, Tooltip("RaycastHit buffer size for the Non-Alloc 'Capsule' and 'Raycast' shapes.")]
            private int raycastHitBuffer = 100;
            public int RaycastHitBuffer
            {
                get { return raycastHitBuffer; }
                set { raycastHitBuffer = value; }
            }
        }

        #endregion

        #region Properties/Constructor

        public CGF()
        {
            _transformProperties = new TransformProperties();
            _forcePositionProperties = new ForcePositionProperties();
            _forceTypeProperties = new ForceTypeProperties();
            _filterProperties = new FilterProperties();
            _eventProperties = new EventProperties();
            _drawGravityProperties = new DrawGravityProperties();
            _memoryProperties = new MemoryProperties();
        }

        //Used for when wanting to see the cgf line
        static private string CirularGravityLineName = "CirularGravityForce_LineDisplay";

#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5)
        //Warning message for Requiring Unity 5.6
        static public string WarningMessageClosestPoint_5_6 = "Force Position Properties Closest Collider Requires Upgrading Project to Unity 5.6 or Higher.";
#endif

#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
        //Warning message for Requiring Unity 5.3
        static public string WarningMessageNonAllocUnity_5_3 = "3D Non-Alloc Physics Requires Upgrading Project to Unity 5.3 or Higher.";
        static public string WarningMessageBoxUnity_5_3 = "3D Box Shape Physics Requires Upgrading Project to Unity 5.3 or Higher.";
#endif

        [SerializeField, Tooltip("Enable/Disable the Circular Gravity Force.")]
        private bool enable = true;
        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        [SerializeField, Tooltip("Shape of the Cirular Gravity Force.")]
        private Shape shape = CGF.Shape.Sphere;
        public Shape _shape
        {
            get { return shape; }
            set { shape = value; }
        }

        [SerializeField, Tooltip("The force type of the Cirular Gravity Force.")]
        private ForceType forceType = ForceType.ForceAtPosition;
        public ForceType _forceType
        {
            get { return forceType; }
            set { forceType = value; }
        }

        [SerializeField, Tooltip("Option for how to apply a force.")]
        private ForceMode forceMode = ForceMode.Force;
        public ForceMode _forceMode
        {
            get { return forceMode; }
            set { forceMode = value; }
        }

        [SerializeField, Tooltip("Projects the force to the forward.")]
        private bool projectForward = false;
        public bool _projectForward
        {
            get { return projectForward; }
            set { projectForward = value; }
        }

        [SerializeField, Tooltip("Radius of the force.")]
        private float size = 5f;
        public float Size
        {
            get { return size; }
            set { size = value; }
        }

        [SerializeField, Tooltip("Capsule Radius size of the fore.")]
        private float capsuleRadius = 2f;
        public float CapsuleRadius
        {
            get { return capsuleRadius; }
            set { capsuleRadius = value; }
        }

        [SerializeField, Tooltip("Box vector size of the fore.")]
        private Vector3 boxSize = new Vector3(5f, 5f, 5f);
        public Vector3 BoxSize
        {
            get { return boxSize; }
            set { boxSize = value; }
        }

        [SerializeField, Tooltip("Power for the force, can be negative or positive.")]
        private float forcePower = 10f;
        public float ForcePower
        {
            get { return forcePower; }
            set { forcePower = value; }
        }

        [SerializeField, Tooltip("Velocity damping on effected rigidbodys.")]
        private float velocityDamping = 0f;
        public float VelocityDamping
        {
            get { return velocityDamping; }
            set { velocityDamping = value; }
        }

        [SerializeField, Tooltip("Angular velocity damping on effected rigidbodys.")]
        private float angularVelocityDamping = 0f;
        public float AngularVelocityDamping
        {
            get { return angularVelocityDamping; }
            set { angularVelocityDamping = value; }
        }

        [SerializeField, Tooltip("Manages all transform properties.")]
        private TransformProperties transformProperties;
        public TransformProperties _transformProperties
        {
            get { return transformProperties; }
            set { transformProperties = value; }
        }

        [SerializeField, Tooltip("Options for where you want the force to start. Only available 'Force At Position', 'Explosion Force', and 'Gravitational Attraction' force types.")]
        private ForcePositionProperties forcePositionProperties;
        public ForcePositionProperties _forcePositionProperties
        {
            get { return forcePositionProperties; }
            set { forcePositionProperties = value; }
        }

        [SerializeField, Tooltip("Force type properties.")]
        private ForceTypeProperties forceTypeProperties;
        public ForceTypeProperties _forceTypeProperties
        {
            get { return forceTypeProperties; }
            set { forceTypeProperties = value; }
        }

        [SerializeField, Tooltip("Filter properties options.")]
        private FilterProperties filterProperties;
        public FilterProperties _filterProperties
        {
            get { return filterProperties; }
            set { filterProperties = value; }
        }

        [SerializeField, Tooltip("Event properties options.")]
        private EventProperties eventProperties;
        public EventProperties _eventProperties
        {
            get { return eventProperties; }
            set { eventProperties = value; }
        }

        [SerializeField, Tooltip("Draw gravity properties.")]
        private DrawGravityProperties drawGravityProperties;
        public DrawGravityProperties _drawGravityProperties
        {
            get { return drawGravityProperties; }
            set { drawGravityProperties = value; }
        }

        [SerializeField, Tooltip("Memory Properties.")]
        private MemoryProperties memoryProperties;
        public MemoryProperties _memoryProperties
        {
            get { return memoryProperties; }
            set { memoryProperties = value; }
        }

        [SerializeField]
        private int colliderListCount;
        public int ColliderListCount
        {
            get { return colliderListCount; }
        }
        [SerializeField]
        private int raycastHitListCount;
        public int RaycastHitListCount
        {
            get { return raycastHitListCount; }
        }

        //Selected Flag
        bool isSelected = false;

        //Line Object
        private GameObject cirularGravityLine;

        //Pre-Allocated Objects
        private Collider[] colliderList;
        private RaycastHit[] raycastHitList;

        Color DebugGravityLineColorA;
        Color DebugGravityLineColorB;

        #endregion

        #region Gizmos

        //Used for draying icons
        void OnDrawGizmos()
        {
            if (_drawGravityProperties.DrawGizmo)
            {
#if (UNITY_EDITOR)
                string icon = "CircularGravityForce Icons/";
                icon = SetupIcons(icon);
                SetupDebugColors();

                isSelected = CheckGameObjects();

                if (isSelected)
                {
                    if (!EditorApplication.isPlaying && (_shape != Shape.Box) && (_shape != Shape.Sphere))
                    {
                        _drawGravityProperties.DrawGravityForceGizmos = false;
                    }
                    else
                    {
                        _drawGravityProperties.DrawGravityForceGizmos = true;
                    }
                }
                else
                {
                    Gizmos.DrawIcon(this.transform.position, icon, true);

                    _drawGravityProperties.DrawGravityForceGizmos = true;
                }
#endif
                if (_drawGravityProperties.DrawGravityForceGizmos)
                {
                    DrawGravityForceGizmos();
                }

                DrawClosetColliderGizmos();
            }
        }

#if (UNITY_EDITOR)
        bool CheckGameObjects()
        {
            if (Selection.activeGameObject == this.gameObject)
                return true;

            foreach (var item in Selection.gameObjects)
            {
                if (item == this.gameObject)
                    return true;
            }

            return false;
        }

        string SetupIcons(string icon)
        {
            string cgfDir = string.Format("{0}/ResurgamStudios/CircularGravityForce Package/Gizmos/CircularGravityForce Icons/", Application.dataPath);
            string dir = string.Format("{0}/Gizmos/CircularGravityForce Icons/", Application.dataPath);

            if (!Directory.Exists(dir))
            {
                if (Directory.Exists(cgfDir))
                {
                    CopyIcons(cgfDir, dir);

                    AssetDatabase.Refresh();
                }
            }

            icon = icon + "cgf_icon";

            if (forcePower == 0 || enable == false)
            {
                icon = icon + "0.png";
            }
            else if (forcePower >= 0)
            {
                icon = icon + "1.png";
            }
            else if (forcePower < 0)
            {
                icon = icon + "2.png";
            }

            return icon;
        }

        //Copys all cgf icons
        void CopyIcons(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir).Where(s => s.EndsWith(".png")))
            {
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
            }
        }
#endif

        #endregion

        #region Unity Events

        void Awake()
        {
            if (_memoryProperties.NonAllocPhysics)
            {
                colliderList = new Collider[_memoryProperties.ColliderBuffer];
                raycastHitList = new RaycastHit[_memoryProperties.RaycastHitBuffer];
            }
        }

        //Update is called once per frame
        void Update()
        {
            if (Enable)
            {
                //Sets up the line that gets rendered showing the area of forces
                if (_drawGravityProperties.DrawGravityForce)
                {
                    if (cirularGravityLine == null)
                    {
                        //Creates line for showing the force
                        cirularGravityLine = new GameObject(CirularGravityLineName);
                        cirularGravityLine.transform.SetParent(this.gameObject.transform, false);
                        cirularGravityLine.AddComponent<LineRenderer>();
                    }
                }
                else
                {
                    if (cirularGravityLine != null)
                    {
                        //Destroys line when not using
                        Destroy(cirularGravityLine);
                    }
                }
            }
            else
            {
                if (cirularGravityLine != null)
                {
                    //Destroys line when not using
                    Destroy(cirularGravityLine);
                }
            }
        }

        //Used for when drawing the cgf line with no lag
        void LateUpdate()
        {
            if (Enable)
            {
                //Validates the this.transform rotation and position
                _transformProperties.ValidateTransform(this.transform);

                //Sets up the line that gets rendered showing the area of forces
                if (_drawGravityProperties.DrawGravityForce)
                {
                    if (cirularGravityLine != null)
                    {
                        DrawGravityForceLineRenderer();
                    }
                }
            }
        }

        //This function is called every fixed frame
        void FixedUpdate()
        {
            if (Enable && forcePower != 0)
            {
                CalculateAndEstimateForce();
            }
        }

        #endregion

        #region Functions

        //Applys the force function
        private void ApplyForce(Rigidbody rigid, Transform trans, Collider coll)
        {
            var transPos = trans.position;

            switch (_forcePositionProperties.ForcePosition)
            {
                case ForcePosition.ThisTransform:
                    break;
                case ForcePosition.ClosestCollider:
                    if (_forcePositionProperties.ClosestColliders != null)
                    {
                        if (_forcePositionProperties.ClosestColliders.Count > 0)
                        {
                            float heightOffset = _forcePositionProperties.HeightOffset;

                            if (!_forcePositionProperties.UseEffectedClosestPoint)
                            {
                                var point = FindClosestPoints(rigid.position, _forcePositionProperties.ClosestColliders);

                                transPos = GetVectorHeightOffset(point, rigid.position, heightOffset);
                            }
                            else
                            {
                                Vector3 pointA = FindClosestPoints(coll.transform.position, _forcePositionProperties.ClosestColliders);
                                Vector3 pointB = FindClosestPoints(pointA, coll);

                                float distanceThisA = Vector3.Distance(coll.transform.position, pointA);
                                float distanceAB = Vector3.Distance(pointA, pointB);

                                transPos = GetVectorHeightOffset(pointA, coll.transform.position, Mathf.Abs(distanceThisA - distanceAB) + heightOffset);
                            }

                        }
                    }
                    break;
            }

            switch (_forceType)
            {
                case ForceType.ForceAtPosition:
                    ApplyCGFForceAtPosition(rigid, transPos, trans);
                    break;
                case ForceType.Force:
                    ApplyCGFForce(rigid, transPos, trans);
                    break;
                case ForceType.Torque:
                    ApplyCGFTorque(rigid, transPos, trans);
                    break;
                case ForceType.ExplosionForce:
                    ApplyCGFExplosionForce(rigid, transPos, trans);
                    break;
                case ForceType.GravitationalAttraction:
                    ApplyCGFGravitationalAttraction(rigid, transPos, trans);
                    break;
            }
        }

        private void ApplyCGFForceAtPosition(Rigidbody rigid, Vector3 transPos, Transform trans)
        {
            Vector3 forceAtPoint = transPos;

            if (_projectForward || _shape == Shape.Raycast)
            {
                forceAtPoint = (trans.forward * ForcePower) - (VelocityDamping * rigid.velocity);
            }
            else
            {
                forceAtPoint = ((rigid.transform.position - transPos) * ForcePower) - (VelocityDamping * rigid.velocity);
            }

            rigid.AddForceAtPosition(forceAtPoint, transPos, _forceMode);
        }

        private void ApplyCGFForce(Rigidbody rigid, Vector3 transPos, Transform trans)
        {
            Vector3 force = transPos;

            if (_projectForward || _shape == Shape.Raycast)
            {
                force = (trans.forward * ForcePower) - (VelocityDamping * rigid.velocity);
            }
            else
            {
                force = ((rigid.transform.position - transPos) * ForcePower) - (VelocityDamping * rigid.velocity);
            }

            rigid.AddForce(force, _forceMode);
        }

        private void ApplyCGFTorque(Rigidbody rigid, Vector3 transPos, Transform trans)
        {
            Vector3 torque = transPos;

            torque = (trans.right * ForcePower) - (AngularVelocityDamping * rigid.angularVelocity);

            rigid.maxAngularVelocity = _forceTypeProperties.TorqueMaxAngularVelocity;
            rigid.AddTorque(torque, _forceMode);
        }

        private void ApplyCGFExplosionForce(Rigidbody rigid, Vector3 transPos, Transform trans)
        {
            rigid.AddExplosionForce(ForcePower, transPos, Size, _forceTypeProperties.ExplosionForceUpwardsModifier, _forceMode);
        }

        private void ApplyCGFGravitationalAttraction(Rigidbody rigid, Vector3 transPos, Transform trans)
        {
            Vector3 gravitationalAttraction = transPos;

            if (_projectForward || _shape == Shape.Raycast)
            {
                gravitationalAttraction = trans.forward * rigid.mass * ForcePower / (trans.forward).sqrMagnitude - (VelocityDamping * rigid.velocity);
            }
            else
            {
                gravitationalAttraction = ((rigid.position - transPos).normalized * rigid.mass * ForcePower / (rigid.position - transPos).sqrMagnitude) - (VelocityDamping * rigid.velocity);
            }

            if (float.IsNaN(gravitationalAttraction.x) && float.IsNaN(gravitationalAttraction.y) && float.IsNaN(gravitationalAttraction.z))
            {
                gravitationalAttraction = Vector3.zero;
            }

            rigid.AddForce(gravitationalAttraction, _forceMode);
        }

        //Calculate and Estimate the force
        private void CalculateAndEstimateForce()
        {
            if (_shape == Shape.Sphere || _shape == Shape.Box)
            {
                #region Sphere

                colliderListCount = 0;

                if (_shape == Shape.Sphere)
                {
                    if (_memoryProperties.NonAllocPhysics)
                    {
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
                        colliderListCount = Physics.OverlapSphereNonAlloc(this.transform.position, Size, colliderList, _memoryProperties.ColliderLayerMask, QueryTriggerInteraction.Ignore);
#else
                        Debug.LogWarning(WarningMessageNonAllocUnity_5_3);
#endif
                    }
                    else
                    {
                        colliderList = Physics.OverlapSphere(this.transform.position, Size, _memoryProperties.ColliderLayerMask, QueryTriggerInteraction.Ignore);
                        colliderListCount = colliderList.Length;
                    }
                }
                if (_shape == Shape.Box)
                {
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
                    if (_memoryProperties.NonAllocPhysics)
                    {
                        colliderListCount = Physics.OverlapBoxNonAlloc(this.transform.position, BoxSize, colliderList, this.transform.rotation, _memoryProperties.ColliderLayerMask, QueryTriggerInteraction.Ignore);
                    }
                    else
                    {
                        colliderList = Physics.OverlapBox(this.transform.position, BoxSize, this.transform.rotation, _memoryProperties.ColliderLayerMask, QueryTriggerInteraction.Ignore);
                        colliderListCount = colliderList.Length;
                    }
#else
                    Debug.LogWarning(WarningMessageBoxUnity_5_3);
#endif
                }

                for (int i = 0; i < colliderListCount; i++)
                {
                    if (colliderList[i] != null)
                    {
                        if (!colliderList[i].isTrigger)
                        {
                            var rigid = colliderList[i].attachedRigidbody;

                            if (rigid != null)
                            {
                                if (_filterProperties.ValidateFilters(rigid, colliderList[i]))
                                {
                                    ApplyForce(rigid, this.transform, colliderList[i]);

                                    //Enables send message & events
                                    if (eventProperties.EnableEvents)
                                    {
                                        if (OnApplyCGFEvent != null)
                                        {
                                            OnApplyCGFEvent.Invoke(this, rigid, colliderList[i]);
                                        }
                                    }
                                    if (eventProperties.EnableSendMessage)
                                    {
                                        rigid.SendMessage("OnApplyCGF", this, SendMessageOptions.DontRequireReceiver);
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region RayCast / Capsule

                //Circular Gravity Force Transform
                Transform cgfTran = this.transform;

                raycastHitListCount = 0;

                if (_shape == Shape.Raycast)
                {
                    if (_memoryProperties.NonAllocPhysics)
                    {
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
                        raycastHitListCount = Physics.RaycastNonAlloc(cgfTran.position, cgfTran.rotation * Vector3.forward, raycastHitList, Size, _memoryProperties.ColliderLayerMask, QueryTriggerInteraction.Ignore);
#else
                        Debug.LogWarning(WarningMessageNonAllocUnity_5_3);
#endif
                    }
                    else
                    {
                        raycastHitList = Physics.RaycastAll(cgfTran.position, cgfTran.rotation * Vector3.forward, Size, _memoryProperties.ColliderLayerMask, QueryTriggerInteraction.Ignore);
                        raycastHitListCount = raycastHitList.Length;
                    }
                }
                else if (_shape == Shape.Capsule)
                {
                    if (_memoryProperties.NonAllocPhysics)
                    {
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
                        raycastHitListCount = Physics.CapsuleCastNonAlloc(cgfTran.position, cgfTran.position + ((cgfTran.rotation * Vector3.back)), capsuleRadius, cgfTran.position - ((cgfTran.position + (cgfTran.rotation * (Vector3.back)))), raycastHitList, Size, _memoryProperties.ColliderLayerMask, QueryTriggerInteraction.Ignore);
#else
                        Debug.LogWarning(WarningMessageNonAllocUnity_5_3);
#endif
                    }
                    else
                    {
                        raycastHitList = Physics.CapsuleCastAll(cgfTran.position, cgfTran.position + ((cgfTran.rotation * Vector3.back)), capsuleRadius, cgfTran.position - ((cgfTran.position + (cgfTran.rotation * (Vector3.back)))), Size, _memoryProperties.ColliderLayerMask, QueryTriggerInteraction.Ignore);
                        raycastHitListCount = raycastHitList.Length;
                    }
                }

                for (int i = 0; i < raycastHitListCount; i++)
                {
                    if (raycastHitList[i].collider != null)
                    {
                        if (!raycastHitList[i].collider.isTrigger)
                        {
                            var rigid = raycastHitList[i].collider.attachedRigidbody;

                            if (rigid != null)
                            {
                                if (_filterProperties.ValidateFilters(rigid, raycastHitList[i].collider))
                                {
                                    ApplyForce(rigid, this.transform, raycastHitList[i].collider);

                                    //Enables send message & events
                                    if (eventProperties.EnableEvents)
                                    {
                                        if (OnApplyCGFEvent != null)
                                        {
                                            OnApplyCGFEvent.Invoke(this, rigid, raycastHitList[i].collider);
                                        }
                                    }
                                    if (eventProperties.EnableSendMessage)
                                    {
                                        rigid.SendMessage("OnApplyCGF", this, SendMessageOptions.DontRequireReceiver);
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
            }
        }

        public Vector3 FindClosestPoints(Vector3 point, List<Collider> closestColliders)
        {
            Vector3 closestPoint = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

            if (closestColliders.Count > 0)
            {
                for (int i = 0; i < closestColliders.Count; i++)
                {
                    if (closestColliders[i] != null)
                    {
                        if (closestColliders[i].gameObject.activeSelf && closestColliders[i].enabled)
                        {
                            var distance = Vector3.Distance(point, closestPoint);

                            var newPoint = FindClosestPoints(point, closestColliders[i]);

                            if (distance > Vector3.Distance(point, newPoint))
                            {
                                closestPoint = newPoint;
                            }
                        }
                    }
                }
            }

            return closestPoint;
        }

        public Vector3 FindClosestPoints(Vector3 point, Collider closestCollider)
        {
            Vector3 closestPoint = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

            if (closestCollider != null)
            {
                var distance = Vector3.Distance(point, closestPoint);

                var newPoint = point;

#if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5)
                // newPoint = Physics.ClosestPoint(point, closestCollider, closestCollider.transform.position, closestCollider.transform.rotation);
                newPoint = closestCollider.ClosestPointOnBounds(point);
#else
                Debug.LogWarning(WarningMessageClosestPoint_5_6);
#endif

                if (distance > Vector3.Distance(point, newPoint))
                {
                    closestPoint = newPoint;
                }
            }
            else
            {
                closestPoint = point;
            }

            return closestPoint;
        }

        public Vector3 GetVectorHeightOffset(Vector3 A, Vector3 B, float x)
        {
            Vector3 P = x * Vector3.Normalize(B - A) + A;
            return P;
        }

        #endregion

        #region Static Functions

        //Static function used to create CGFs
        public static GameObject CreateCGF()
        {
            //Creates empty gameobject.
            GameObject cgf = new GameObject();

            //Sets gameojbect Name
            cgf.name = "CGF";

            //Creates Circular Gravity Force component
            cgf.AddComponent<CGF>()._drawGravityProperties.GravityLineMaterial = new Material(Shader.Find("GUI/Text Shader"));

            return cgf;
        }

        #endregion

        #region Draw

        //Draws effected area by forces line renderer
        private void DrawGravityForceLineRenderer()
        {
            //Circular Gravity Force Transform
            Transform cgfTran = this.transform;

            Color DebugGravityLineColor;

            if (Enable)
            {
                if (forcePower == 0)
                    DebugGravityLineColor = Color.white;
                else if (forcePower > 0)
                    DebugGravityLineColor = Color.green;
                else
                    DebugGravityLineColor = Color.red;
            }
            else
            {
                DebugGravityLineColor = Color.white;
            }

            //Line setup
            LineRenderer lineRenderer = cirularGravityLine.GetComponent<LineRenderer>();
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4)
            lineRenderer.startWidth = _drawGravityProperties.Thickness;
            lineRenderer.endWidth = _drawGravityProperties.Thickness;
            lineRenderer.numCornerVertices = 4;
#else
            lineRenderer.SetWidth(_drawGravityProperties.Thickness, _drawGravityProperties.Thickness);
#endif
            lineRenderer.material = _drawGravityProperties.GravityLineMaterial;
            lineRenderer.material.color = DebugGravityLineColor;

            //Renders type outline
            switch (_shape)
            {
                case Shape.Sphere:

                    //Models line
// #if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4)
//                     lineRenderer.positionCount = 12;
// #elif (UNITY_5_5)
                    lineRenderer.numPositions = 12;
// #else
//                     lineRenderer.SetVertexCount(12);
// #endif

                    lineRenderer.SetPosition(0, cgfTran.position + ((cgfTran.rotation * Vector3.up) * Size));
                    lineRenderer.SetPosition(1, cgfTran.position);
                    lineRenderer.SetPosition(2, cgfTran.position + ((cgfTran.rotation * Vector3.down) * Size));
                    lineRenderer.SetPosition(3, cgfTran.position);
                    lineRenderer.SetPosition(4, cgfTran.position + ((cgfTran.rotation * Vector3.left) * Size));
                    lineRenderer.SetPosition(5, cgfTran.position);
                    lineRenderer.SetPosition(6, cgfTran.position + ((cgfTran.rotation * Vector3.right) * Size));
                    lineRenderer.SetPosition(7, cgfTran.position);
                    lineRenderer.SetPosition(8, cgfTran.position + ((cgfTran.rotation * Vector3.forward) * Size));
                    lineRenderer.SetPosition(9, cgfTran.position);
                    lineRenderer.SetPosition(10, cgfTran.position + ((cgfTran.rotation * Vector3.back) * Size));
                    lineRenderer.SetPosition(11, cgfTran.position);

                    break;

                case Shape.Capsule:

                    //Models line
// #if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4)
//                     lineRenderer.positionCount = 17;
// #elif (UNITY_5_5)
                    lineRenderer.numPositions = 17;
// #else
//                     lineRenderer.SetVertexCount(17);
// #endif

                    //Starting Point
                    lineRenderer.SetPosition(0, cgfTran.position + ((cgfTran.rotation * Vector3.up) * capsuleRadius));
                    lineRenderer.SetPosition(1, cgfTran.position);
                    lineRenderer.SetPosition(2, cgfTran.position + ((cgfTran.rotation * Vector3.down) * capsuleRadius));
                    lineRenderer.SetPosition(3, cgfTran.position);
                    lineRenderer.SetPosition(4, cgfTran.position + ((cgfTran.rotation * Vector3.left) * capsuleRadius));
                    lineRenderer.SetPosition(5, cgfTran.position);
                    lineRenderer.SetPosition(6, cgfTran.position + ((cgfTran.rotation * Vector3.right) * capsuleRadius));
                    lineRenderer.SetPosition(7, cgfTran.position);

                    //Middle Line
                    Vector3 endPointLoc = cgfTran.position + ((cgfTran.rotation * Vector3.forward) * Size);
                    lineRenderer.SetPosition(8, endPointLoc);

                    //End Point
                    lineRenderer.SetPosition(9, endPointLoc + ((cgfTran.rotation * Vector3.up) * capsuleRadius));
                    lineRenderer.SetPosition(10, endPointLoc);
                    lineRenderer.SetPosition(11, endPointLoc + ((cgfTran.rotation * Vector3.down) * capsuleRadius));
                    lineRenderer.SetPosition(12, endPointLoc);
                    lineRenderer.SetPosition(13, endPointLoc + ((cgfTran.rotation * Vector3.left) * capsuleRadius));
                    lineRenderer.SetPosition(14, endPointLoc);
                    lineRenderer.SetPosition(15, endPointLoc + ((cgfTran.rotation * Vector3.right) * capsuleRadius));
                    lineRenderer.SetPosition(16, endPointLoc);

                    break;

                case Shape.Raycast:

                    //Models line
// #if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4)
//                     lineRenderer.positionCount = 2;
// #elif (UNITY_5_5)
                    lineRenderer.numPositions = 2;
// #else
//                     lineRenderer.SetVertexCount(2);
// #endif

                    lineRenderer.SetPosition(0, cgfTran.position);
                    lineRenderer.SetPosition(1, cgfTran.position + ((cgfTran.rotation * Vector3.forward) * Size));

                    break;

                case Shape.Box:

                    //Models line
// #if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4)
//                     lineRenderer.positionCount = 12;
// #elif (UNITY_5_5)
                    lineRenderer.numPositions = 12;
// #else
//                     lineRenderer.SetVertexCount(12);
// #endif

                    lineRenderer.SetPosition(0, cgfTran.position + ((cgfTran.rotation * Vector3.up) * BoxSize.y));
                    lineRenderer.SetPosition(1, cgfTran.position);
                    lineRenderer.SetPosition(2, cgfTran.position + ((cgfTran.rotation * Vector3.down) * BoxSize.y));
                    lineRenderer.SetPosition(3, cgfTran.position);
                    lineRenderer.SetPosition(4, cgfTran.position + ((cgfTran.rotation * Vector3.left) * BoxSize.x));
                    lineRenderer.SetPosition(5, cgfTran.position);
                    lineRenderer.SetPosition(6, cgfTran.position + ((cgfTran.rotation * Vector3.right) * BoxSize.x));
                    lineRenderer.SetPosition(7, cgfTran.position);
                    lineRenderer.SetPosition(8, cgfTran.position + ((cgfTran.rotation * Vector3.forward) * BoxSize.z));
                    lineRenderer.SetPosition(9, cgfTran.position);
                    lineRenderer.SetPosition(10, cgfTran.position + ((cgfTran.rotation * Vector3.back) * BoxSize.z));
                    lineRenderer.SetPosition(11, cgfTran.position);

                    break;
            }
        }

        //Sets up the gizmo colors for CGF for the editor
        private void SetupDebugColors()
        {
            if (Enable)
            {
                if (forcePower == 0)
                {
                    DebugGravityLineColorA = Color.white;
                    DebugGravityLineColorB = Color.white;
                }
                else if (forcePower > 0)
                {
                    DebugGravityLineColorA = Color.green;
                    DebugGravityLineColorB = Color.green;
                }
                else
                {
                    DebugGravityLineColorA = Color.red;
                    DebugGravityLineColorB = Color.red;
                }
            }
            else
            {
                DebugGravityLineColorA = Color.white;
                DebugGravityLineColorB = Color.white;
            }

            DebugGravityLineColorA.a = .5f;
            DebugGravityLineColorB.a = .1f;
        }

        //Draws effected area by forces with debug draw line, so you can see it in Gizmos
        private void DrawGravityForceGizmos()
        {
            //Circular Gravity Force Transform
            Transform cgfTran = this.transform;

            //Renders type outline
            switch (_shape)
            {
                case CGF.Shape.Sphere:

                    Gizmos.color = DebugGravityLineColorB;
                    Gizmos.DrawSphere(cgfTran.position, Size);

                    if (!isSelected)
                    {
                        Gizmos.color = DebugGravityLineColorA;
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.up) * Size), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.down) * Size), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.left) * Size), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.right) * Size), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.forward) * Size), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.back) * Size), cgfTran.position);
                    }
                    else
                    {
                        Gizmos.color = DebugGravityLineColorB;
                        Gizmos.DrawSphere(cgfTran.position, Size);
                    }


                    break;

                case CGF.Shape.Capsule:

                    Gizmos.color = DebugGravityLineColorA;
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.up) * capsuleRadius), cgfTran.position);
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.down) * capsuleRadius), cgfTran.position);
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.left) * capsuleRadius), cgfTran.position);
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.right) * capsuleRadius), cgfTran.position);

                    Vector3 endPointLoc = cgfTran.position + ((cgfTran.rotation * Vector3.forward) * Size);

                    Gizmos.DrawLine(cgfTran.position, endPointLoc);

                    Gizmos.DrawLine(endPointLoc + ((cgfTran.rotation * Vector3.up) * capsuleRadius), endPointLoc);
                    Gizmos.DrawLine(endPointLoc + ((cgfTran.rotation * Vector3.down) * capsuleRadius), endPointLoc);
                    Gizmos.DrawLine(endPointLoc + ((cgfTran.rotation * Vector3.left) * capsuleRadius), endPointLoc);
                    Gizmos.DrawLine(endPointLoc + ((cgfTran.rotation * Vector3.right) * capsuleRadius), endPointLoc);

                    break;

                case CGF.Shape.Raycast:

                    Gizmos.color = DebugGravityLineColorA;
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.forward) * Size), cgfTran.position);

                    break;

                case CGF.Shape.Box:

                    Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one * 2);
                    Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

                    Gizmos.matrix *= cubeTransform;

                    Gizmos.color = DebugGravityLineColorA;
                    Gizmos.DrawWireCube(Vector3.zero, BoxSize);

                    if (isSelected)
                        DebugGravityLineColorA.a = 1f;

                    if (isSelected)
                    {
                        Gizmos.color = DebugGravityLineColorA;
                        Gizmos.DrawWireCube(Vector3.zero, BoxSize);
                    }

                    Gizmos.color = DebugGravityLineColorB;
                    Gizmos.DrawCube(Vector3.zero, BoxSize);

                    if (isSelected)
                        Gizmos.DrawCube(Vector3.zero, BoxSize);

                    Gizmos.matrix = oldGizmosMatrix;

                    if (!isSelected)
                    {
                        Gizmos.color = DebugGravityLineColorA;
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.up) * BoxSize.y), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.down) * BoxSize.y), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.left) * BoxSize.x), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.right) * BoxSize.x), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.forward) * BoxSize.z), cgfTran.position);
                        Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.back) * BoxSize.z), cgfTran.position);
                    }

                    break;
            }

            if (memoryProperties.SeeColliders)
            {
                if (_shape == Shape.Sphere || _shape == Shape.Box)
                {
                    if (ForcePower != 0)
                    {
                        for (int i = 0; i < ColliderListCount; i++)
                        {
                            if (isSelected)
                            {
                                Gizmos.color = DebugGravityLineColorA;
                                DebugGravityLineColorA.a = 1f;
                            }
                            Gizmos.DrawLine(this.transform.position, colliderList[i].gameObject.transform.position);
                        }
                    }
                }
            }

            if (memoryProperties.SeeRaycastHits)
            {
                if (_shape == Shape.Capsule || _shape == Shape.Raycast)
                {
                    if (ForcePower != 0)
                    {
                        for (int i = 0; i < RaycastHitListCount; i++)
                        {
                            if (isSelected)
                            {
                                Gizmos.color = DebugGravityLineColorA;
                                DebugGravityLineColorA.a = 1f;
                            }
                            Gizmos.DrawLine(this.transform.position, raycastHitList[i].collider.gameObject.transform.position);
                        }
                    }
                }
            }

            if (_forcePositionProperties.ForcePosition == ForcePosition.ClosestCollider)
            {
                switch (_forceType)
                {
                    case ForceType.ForceAtPosition:
                    case ForceType.Force:
                    case ForceType.ExplosionForce:
                    case ForceType.GravitationalAttraction:

                        Gizmos.color = DebugGravityLineColorA;
                        if (_shape == Shape.Sphere || _shape == Shape.Box)
                        {
                            for (int i = 0; i < ColliderListCount; i++)
                            {
                                if (colliderList != null)
                                {
                                    if (colliderList.Length > 0)
                                    {
                                        var rigidCheck = colliderList[i].attachedRigidbody;
                                        if (rigidCheck != null)
                                        {
                                            if (_filterProperties.ValidateFilters(rigidCheck, colliderList[i]))
                                            {
                                                Vector3 pointA = FindClosestPoints(colliderList[i].transform.position, _forcePositionProperties.ClosestColliders);
                                                Vector3 pointB = Vector3.zero;

                                                if (_forcePositionProperties.UseEffectedClosestPoint)
                                                {
                                                    pointB = FindClosestPoints(pointA, colliderList[i]);
                                                }
                                                else
                                                {
                                                    pointB = colliderList[i].transform.position;
                                                }

                                                Gizmos.DrawWireSphere(pointA, .01f);
                                                Gizmos.DrawLine(pointA, pointB);
                                                Gizmos.DrawWireSphere(pointB, .01f);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (_shape == Shape.Capsule || _shape == Shape.Raycast)
                        {
                            for (int i = 0; i < RaycastHitListCount; i++)
                            {
                                if (raycastHitList != null)
                                {
                                    if (raycastHitList.Length > 0)
                                    {
                                        var rigidCheck = raycastHitList[i].collider.attachedRigidbody;
                                        if (rigidCheck != null)
                                        {
                                            if (_filterProperties.ValidateFilters(rigidCheck, raycastHitList[i].collider))
                                            {
                                                Vector3 pointA = FindClosestPoints(raycastHitList[i].collider.transform.position, _forcePositionProperties.ClosestColliders);
                                                Vector3 pointB = Vector3.zero;

                                                if (_forcePositionProperties.UseEffectedClosestPoint)
                                                {
                                                    pointB = FindClosestPoints(pointA, raycastHitList[i].collider);
                                                }
                                                else
                                                {
                                                    pointB = raycastHitList[i].collider.transform.position;
                                                }

                                                Gizmos.DrawWireSphere(pointA, .01f);
                                                Gizmos.DrawLine(pointA, pointB);
                                                Gizmos.DrawWireSphere(pointB, .01f);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    case ForceType.Torque:
                        break;
                    default:
                        break;
                }
            }
        }

        //Draws CGF bounds for ClosestCollider
        private void DrawClosetColliderGizmos()
        {
            if (!isSelected)
                return;

            Gizmos.color = DebugGravityLineColorA;

            if (_forcePositionProperties.ForcePosition == CGF.ForcePosition.ClosestCollider)
            {
                for (int i = 0; i < _forcePositionProperties.ClosestColliders.Count; i++)
                {
                    if (_forcePositionProperties.ClosestColliders[i] != null)
                    {
                        var meshFilter = _forcePositionProperties.ClosestColliders[i].gameObject.GetComponent<MeshFilter>();
                        if (meshFilter != null)
                        {
                            var mesh = meshFilter.sharedMesh;

                            if (mesh != null)
                            {
                                Gizmos.DrawWireMesh(mesh, meshFilter.gameObject.transform.position, meshFilter.gameObject.transform.rotation, meshFilter.gameObject.transform.lossyScale);
                            }
                        }
                        else
                        {
                            var bound = _forcePositionProperties.ClosestColliders[i].bounds;
                            Gizmos.DrawWireCube(bound.center, bound.size);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
