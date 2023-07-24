/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Core logic for Circular Gravity Force for 2D.
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
    [AddComponentMenu("Physics 2D/Circular Gravity Force 2D", -1)]
    public class CGF2D : MonoBehaviour
    {
        #region Events

        public delegate void ApplyCGFEvent(CGF2D cgf, Rigidbody2D rigid, Collider2D coll);
        public static event ApplyCGFEvent OnApplyCGFEvent;

        #endregion

        #region Enums

        //Force Types
        public enum ForceType2D
        {
            ForceAtPosition,
            Force,
            Torque,
            GravitationalAttraction
        }

        //Force Types
        public enum Shape2D
        {
            Sphere,
            Raycast,
            Box
        }

        #endregion

        #region Classes

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
            private CGF.ForcePosition forcePosition = CGF.ForcePosition.ThisTransform;
            public CGF.ForcePosition ForcePosition
            {
                get { return forcePosition; }
                set { forcePosition = value; }
            }

            [SerializeField, Tooltip("Colliders for when using 'Closest Collider' for 'Force Position'")]
            private List<Collider2D> closestColliders;
            public List<Collider2D> ClosestColliders
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
                ClosestColliders = new List<Collider2D>();
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
                private Collider2D collider;
                public Collider2D _collider
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
                private PhysicsMaterial2D physicMaterial;
                public PhysicsMaterial2D _physicMaterial
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

            [SerializeField, Tooltip("Used to filter out collider bounds, has authority over physicMaterial, and layermasks.")]
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
            public bool ValidateFilters(Rigidbody2D rigid, Collider2D coll)
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

        //Trigger Area Filter
        [System.Serializable]
        public class TriggerAreaFilter2D
        {
            //Trigger Options
            public enum TriggerAreaFilterOptions
            {
                Disabled,
                OnlyAffectWithinTigger,
                DontAffectWithinTigger,
            }

            [SerializeField, Tooltip("Trigger area filter options.")]
            private TriggerAreaFilterOptions triggerAreaFilterOptions = TriggerAreaFilterOptions.Disabled;
            public TriggerAreaFilterOptions _triggerAreaFilterOptions
            {
                get { return triggerAreaFilterOptions; }
                set { triggerAreaFilterOptions = value; }
            }

            [SerializeField, Tooltip("Listed triggers used for the filter.")]
            private List<Collider2D> triggerAreas;
            public List<Collider2D> TriggerAreas
            {
                get { return triggerAreas; }
                set { triggerAreas = value; }
            }
        }

        #endregion

        #region Properties/Constructor

        public CGF2D()
        {
            _transformProperties = new CGF.TransformProperties();
            _forcePositionProperties = new ForcePositionProperties();
            _filterProperties = new FilterProperties();
            _eventProperties = new CGF.EventProperties();
            _drawGravityProperties = new CGF.DrawGravityProperties();
            _memoryProperties = new CGF.MemoryProperties();
        }

        //Used for when wanting to see the cgf line
        static private string CirularGravityLineName = "CirularGravityForce_LineDisplay";

#if (UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
        //Warning message for Requiring Unity 5.3
        static private string WarningMessageBoxUnity_5_3 = "2D Box Shape Physics Requires Upgrading Project to Unity 5.3 or Higher.";
#endif

        [SerializeField, Tooltip("Enable/Disable the Circular Gravity Force.")]
        private bool enable = true;
        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        [SerializeField, Tooltip("Shape of the Cirular Gravity Force.")]
        private Shape2D shape2D = Shape2D.Sphere;
        public Shape2D _shape2D
        {
            get { return shape2D; }
            set { shape2D = value; }
        }

        [SerializeField, Tooltip("The force type of the Cirular Gravity Force.")]
        private ForceType2D forceType2D = ForceType2D.ForceAtPosition;
        public ForceType2D _forceType2D
        {
            get { return forceType2D; }
            set { forceType2D = value; }
        }

        [SerializeField, Tooltip("Option for how to apply a force.")]
        private ForceMode2D forceMode2D = ForceMode2D.Force;
        public ForceMode2D _forceMode2D
        {
            get { return forceMode2D; }
            set { forceMode2D = value; }
        }

        [SerializeField, Tooltip("Projects the force to the right.")]
        private bool projectRight = false;
        public bool _projectRight
        {
            get { return projectRight; }
            set { projectRight = value; }
        }

        [SerializeField, Tooltip("Radius of the force.")]
        private float size = 5f;
        public float Size
        {
            get { return size; }
            set { size = value; }
        }

        [SerializeField, Tooltip("Radius of the force.")]
        private Vector2 boxSize = Vector2.one * 5f;
        public Vector2 BoxSize
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
        private CGF.TransformProperties transformProperties;
        public CGF.TransformProperties _transformProperties
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

        [SerializeField, Tooltip("Filter properties options.")]
        private FilterProperties filterProperties;
        public FilterProperties _filterProperties
        {
            get { return filterProperties; }
            set { filterProperties = value; }
        }

        [SerializeField, Tooltip("Event properties options.")]
        private CGF.EventProperties eventProperties;
        public CGF.EventProperties _eventProperties
        {
            get { return eventProperties; }
            set { eventProperties = value; }
        }

        [SerializeField, Tooltip("Draw gravity properties.")]
        private CGF.DrawGravityProperties drawGravityProperties;
        public CGF.DrawGravityProperties _drawGravityProperties
        {
            get { return drawGravityProperties; }
            set { drawGravityProperties = value; }
        }

        [SerializeField, Tooltip("Memory Properties.")]
        private CGF.MemoryProperties memoryProperties;
        public CGF.MemoryProperties _memoryProperties
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

        //Effected Rigidbodys
        Collider2D[] colliderList;
        RaycastHit2D[] raycastHitList;

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
                    if (!EditorApplication.isPlaying && (_shape2D != Shape2D.Box))
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
            else if (ForcePower < 0)
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
                colliderList = new Collider2D[_memoryProperties.ColliderBuffer];
                raycastHitList = new RaycastHit2D[_memoryProperties.RaycastHitBuffer];
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
            if (Enable && ForcePower != 0)
            {
                CalculateAndEstimateForce();
            }
        }

        #endregion

        #region Functions

        //Applys the force function
        private void ApplyForce(Rigidbody2D rigid, Transform trans, Collider2D coll)
        {
            var transPos = trans.position;

            switch (_forcePositionProperties.ForcePosition)
            {
                case CGF.ForcePosition.ThisTransform:
                    break;
                case CGF.ForcePosition.ClosestCollider:
                    if (_forcePositionProperties.ClosestColliders != null)
                    {
                        if (_forcePositionProperties.ClosestColliders.Count > 0)
                        {
                            float heightOffset = _forcePositionProperties.HeightOffset;

                            if (!_forcePositionProperties.UseEffectedClosestPoint)
                            {
                                var point = FindClosestPoints(coll, _forcePositionProperties.ClosestColliders, false);

                                transPos = GetVectorHeightOffset(point, coll.transform.position, heightOffset);
                            }
                            else
                            {
                                Vector3 pointA = FindClosestPoints(coll, _forcePositionProperties.ClosestColliders, false);
                                Vector3 pointB = FindClosestPoints(coll, _forcePositionProperties.ClosestColliders, true);

                                float distanceThisA = Vector3.Distance(coll.transform.position, pointA);
                                float distanceAB = Vector3.Distance(pointA, pointB);

                                transPos = GetVectorHeightOffset(pointA, coll.transform.position, Mathf.Abs(distanceThisA - distanceAB) + heightOffset);
                            }

                        }
                    }
                    break;
            }

            switch (_forceType2D)
            {
                //TODO: Add infinity check
                case ForceType2D.ForceAtPosition:
                    ApplyCGFForceAtPosition(rigid, transPos, trans);
                    break;
                case ForceType2D.Force:
                    ApplyCGFForce(rigid, transPos, trans);
                    break;
                case ForceType2D.Torque:
                    ApplyCGFTorque(rigid, transPos, trans);
                    break;
                case ForceType2D.GravitationalAttraction:
                    ApplyCGFGravitationalAttraction(rigid, transPos, trans);
                    break;
            }
        }

        private void ApplyCGFForceAtPosition(Rigidbody2D rigid, Vector3 transPos, Transform trans)
        {
            Vector3 velocity = new Vector3(rigid.velocity.x, rigid.velocity.y, 0f);

            Vector3 forceAtPoint = transPos;

            if (_projectRight || _shape2D == Shape2D.Raycast)
            {
                forceAtPoint = ((trans.right) * ForcePower) - (VelocityDamping * velocity);
            }
            else
            {
                forceAtPoint = ((rigid.gameObject.transform.position - transPos) * ForcePower) - (VelocityDamping * velocity);
            }

            rigid.AddForceAtPosition(forceAtPoint, transPos, _forceMode2D);
        }

        private void ApplyCGFForce(Rigidbody2D rigid, Vector3 transPos, Transform trans)
        {
            Vector3 velocity = new Vector3(rigid.velocity.x, rigid.velocity.y, 0f);

            if (_projectRight || _shape2D == Shape2D.Raycast)
            {
                rigid.AddForce(((trans.right) * ForcePower) - (VelocityDamping * velocity), _forceMode2D);
            }
            else
            {
                rigid.AddForce(((rigid.transform.position - transPos) * ForcePower) - (VelocityDamping * velocity), _forceMode2D);
            }
        }

        private void ApplyCGFTorque(Rigidbody2D rigid, Vector3 transPos, Transform trans)
        {
            rigid.AddTorque(-(ForcePower + ((rigid.angularVelocity * AngularVelocityDamping) * Time.deltaTime)), _forceMode2D);
        }

        private void ApplyCGFGravitationalAttraction(Rigidbody2D rigid, Vector3 transPos, Transform trans)
        {
            Vector3 velocity = new Vector3(rigid.velocity.x, rigid.velocity.y, 0f);

            if (_projectRight || _shape2D == Shape2D.Raycast)
            {
                Vector3 gravitationalAttraction = (trans.right).normalized * rigid.mass * ForcePower / (trans.right).sqrMagnitude;

                if (float.IsNaN(gravitationalAttraction.x) && float.IsNaN(gravitationalAttraction.y) && float.IsNaN(gravitationalAttraction.z))
                {
                    gravitationalAttraction = Vector3.zero;
                }

                rigid.AddForce(gravitationalAttraction - (VelocityDamping * velocity), _forceMode2D);
            }
            else
            {
                Vector3 gravitationalAttraction = (rigid.gameObject.transform.position - transPos).normalized * rigid.mass * ForcePower / (rigid.gameObject.transform.position - transPos).sqrMagnitude;

                if (float.IsNaN(gravitationalAttraction.x) && float.IsNaN(gravitationalAttraction.y) && float.IsNaN(gravitationalAttraction.z))
                {
                    gravitationalAttraction = Vector3.zero;
                }

                rigid.AddForce(gravitationalAttraction - (VelocityDamping * velocity), _forceMode2D);
            }
        }

        //Calculate and Estimate the force
        private void CalculateAndEstimateForce()
        {
            if (_shape2D == Shape2D.Sphere)
            {
                #region Sphere

                colliderListCount = 0;

                if (_shape2D == Shape2D.Sphere)
                {
                    if (_memoryProperties.NonAllocPhysics)
                    {
                        colliderListCount = Physics2D.OverlapCircleNonAlloc(this.transform.position, Size, colliderList, _memoryProperties.ColliderLayerMask);
                    }
                    else
                    {
                        colliderList = Physics2D.OverlapCircleAll(this.transform.position, Size, _memoryProperties.ColliderLayerMask);
                        colliderListCount = colliderList.Length;
                    }
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
                #region RayCast

                //Circular Gravity Force Transform
                Transform cgfTran = this.transform;

                raycastHitListCount = 0;

                if (_shape2D == Shape2D.Raycast)
                {
                    if (_memoryProperties.NonAllocPhysics)
                    {
                        raycastHitListCount = Physics2D.RaycastNonAlloc(cgfTran.position, cgfTran.rotation * Vector3.right, raycastHitList, Size, _memoryProperties.ColliderLayerMask);
                    }
                    else
                    {
                        raycastHitList = Physics2D.RaycastAll(cgfTran.position, cgfTran.rotation * Vector3.right, Size, _memoryProperties.ColliderLayerMask);
                        raycastHitListCount = raycastHitList.Length;
                    }
                }

                if (_shape2D == Shape2D.Box)
                {
#if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
                    if (_memoryProperties.NonAllocPhysics)
                    {
                        raycastHitListCount = Physics2D.BoxCastNonAlloc(this.transform.position, new Vector2(boxSize.x * 2, boxSize.y * 2), this.transform.eulerAngles.z, Vector3.zero, raycastHitList, _memoryProperties.ColliderLayerMask);
                    }
                    else
                    {
                        raycastHitList = Physics2D.BoxCastAll(this.transform.position, new Vector2(boxSize.x * 2, boxSize.y * 2), this.transform.eulerAngles.z, Vector3.zero, _memoryProperties.ColliderLayerMask);
                        raycastHitListCount = raycastHitList.Length;
                    }
#else
                    Debug.LogWarning(WarningMessageBoxUnity_5_3);
#endif
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

        public Vector3 FindClosestPoints(Collider2D point, List<Collider2D> closestColliders, bool findClosestPoint)
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
                            var distance = Vector3.Distance(point.transform.position, closestPoint);

                            var newPoint = FindClosestPoints(point, closestColliders[i], findClosestPoint);

                            if (distance > Vector3.Distance(point.transform.position, newPoint))
                            {
                                closestPoint = newPoint;
                            }
                        }
                    }
                }
            }

            return closestPoint;
        }

        public Vector3 FindClosestPoints(Collider2D point, Collider2D closestCollider, bool findClosestPoint)
        {
            Vector3 closestPoint = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

            if (closestCollider != null)
            {
                var distance = Vector3.Distance(point.transform.position, closestPoint);

                var newPoint = point.transform.position;

#if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5)
                // if (!findClosestPoint)
                //     newPoint = point.Distance(closestCollider).pointB;
                // else
                //     newPoint = point.Distance(closestCollider).pointA;
#else
                Debug.LogWarning(CGF.WarningMessageClosestPoint_5_6);
#endif

                if (distance > Vector3.Distance(point.transform.position, newPoint))
                {
                    closestPoint = newPoint;
                }
            }
            else
            {
                closestPoint = point.transform.position;
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
            cgf.name = "CGF 2D";

            //Creates Circular Gravity Force component
            cgf.AddComponent<CGF2D>()._drawGravityProperties.GravityLineMaterial = new Material(Shader.Find("GUI/Text Shader"));

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
                if (ForcePower == 0)
                    DebugGravityLineColor = Color.white;
                else if (ForcePower > 0)
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
            switch (_shape2D)
            {
                case Shape2D.Sphere:

                    //Models line
// #if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4)
//                     lineRenderer.positionCount = 8;
// #elif (UNITY_5_5)
                    lineRenderer.numPositions = 8;
// #else
//                     lineRenderer.SetVertexCount(8);
// #endif

                    lineRenderer.SetPosition(0, cgfTran.position + ((cgfTran.rotation * Vector3.up) * Size));
                    lineRenderer.SetPosition(1, cgfTran.position);
                    lineRenderer.SetPosition(2, cgfTran.position + ((cgfTran.rotation * Vector3.down) * Size));
                    lineRenderer.SetPosition(3, cgfTran.position);
                    lineRenderer.SetPosition(4, cgfTran.position + ((cgfTran.rotation * Vector3.left) * Size));
                    lineRenderer.SetPosition(5, cgfTran.position);
                    lineRenderer.SetPosition(6, cgfTran.position + ((cgfTran.rotation * Vector3.right) * Size));
                    lineRenderer.SetPosition(7, cgfTran.position);

                    break;

                case Shape2D.Raycast:

                    //Models line
// #if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4)
//                     lineRenderer.positionCount = 2;
// #elif (UNITY_5_5)
                    lineRenderer.numPositions = 2;
// #else
//                     lineRenderer.SetVertexCount(2);
// #endif

                    lineRenderer.SetPosition(0, cgfTran.position);
                    lineRenderer.SetPosition(1, cgfTran.position + ((cgfTran.rotation * Vector3.right) * Size));

                    break;


                case Shape2D.Box:

                    //Models line
// #if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4)
//                     lineRenderer.positionCount = 8;
// #elif (UNITY_5_5)
                    lineRenderer.numPositions = 8;
// #else
//                     lineRenderer.SetVertexCount(8);
// #endif

                    lineRenderer.SetPosition(0, cgfTran.position + ((cgfTran.rotation * Vector3.up) * BoxSize.y));
                    lineRenderer.SetPosition(1, cgfTran.position);
                    lineRenderer.SetPosition(2, cgfTran.position + ((cgfTran.rotation * Vector3.down) * BoxSize.y));
                    lineRenderer.SetPosition(3, cgfTran.position);
                    lineRenderer.SetPosition(4, cgfTran.position + ((cgfTran.rotation * Vector3.left) * BoxSize.x));
                    lineRenderer.SetPosition(5, cgfTran.position);
                    lineRenderer.SetPosition(6, cgfTran.position + ((cgfTran.rotation * Vector3.right) * BoxSize.x));
                    lineRenderer.SetPosition(7, cgfTran.position);

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
            switch (_shape2D)
            {
                case Shape2D.Sphere:

                    Gizmos.color = DebugGravityLineColorA;
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.up) * Size), cgfTran.position);
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.down) * Size), cgfTran.position);
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.left) * Size), cgfTran.position);
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.right) * Size), cgfTran.position);

                    break;

                case Shape2D.Raycast:

                    Gizmos.color = DebugGravityLineColorA;
                    Gizmos.DrawLine(cgfTran.position + ((cgfTran.rotation * Vector3.right) * Size), cgfTran.position);

                    break;

                case Shape2D.Box:

                    Vector3 BoxSize = new Vector3(boxSize.x, boxSize.y, 0f);

                    Gizmos.color = DebugGravityLineColorA;
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
                    }

                    break;
            }

            if (memoryProperties.SeeColliders)
            {
                if (_shape2D == Shape2D.Sphere)
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
                if (shape2D == Shape2D.Raycast || _shape2D == Shape2D.Box)
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

            if (_forcePositionProperties.ForcePosition == CGF.ForcePosition.ClosestCollider)
            {
                switch (_forceType2D)
                {
                    case ForceType2D.ForceAtPosition:
                    case ForceType2D.Force:
                    case ForceType2D.GravitationalAttraction:

                        Gizmos.color = DebugGravityLineColorA;
                        if (_shape2D == Shape2D.Sphere)
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
                                                Vector3 pointA = FindClosestPoints(colliderList[i], _forcePositionProperties.ClosestColliders, false);
                                                Vector3 pointB = Vector3.zero;

                                                if (_forcePositionProperties.UseEffectedClosestPoint)
                                                {
                                                    pointB = FindClosestPoints(colliderList[i], _forcePositionProperties.ClosestColliders, true);
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

                        if (_shape2D == Shape2D.Raycast || _shape2D == Shape2D.Box)
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
                                                Vector3 pointA = FindClosestPoints(raycastHitList[i].collider, _forcePositionProperties.ClosestColliders, false);
                                                Vector3 pointB = Vector3.zero;

                                                if (_forcePositionProperties.UseEffectedClosestPoint)
                                                {
                                                    pointB = FindClosestPoints(raycastHitList[i].collider, _forcePositionProperties.ClosestColliders, true);
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
                    case ForceType2D.Torque:
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
                        var bound = _forcePositionProperties.ClosestColliders[i].bounds;
                        Gizmos.DrawWireCube(bound.center, bound.size);
                    }
                }
            }
        }

        #endregion
    }
}
