/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for cgf mod, sizes the cgf object based of the raycast hit point.
*******************************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CircularGravityForce
{
    [RequireComponent(typeof(CGF))]
    public class CGF_SizeByRaycast : MonoBehaviour
    {
        #region Properties

        [SerializeField, Tooltip("Offset the raycast.")]
        private float offsetRaycast = 1f;
        public float OffsetRaycast
        {
            get { return offsetRaycast; }
            set { offsetRaycast = value; }
        }

        [SerializeField, Tooltip("Max size that the circular gravity force can get.")]
        private float maxCgfSize = 10f;
        public float MaxCgfSize
        {
            get { return maxCgfSize; }
            set { maxCgfSize = value; }
        }

        [SerializeField, Tooltip("Doesnt size box size x.")]
        private bool dontSizeBoxSizeX = false;
        public bool DontSizeBoxSizeX
        {
            get { return dontSizeBoxSizeX; }
            set { dontSizeBoxSizeX = value; }
        }

        [SerializeField, Tooltip("Doesnt size box size y.")]
        private bool dontSizeBoxSizeY = false;
        public bool DontSizeBoxSizeY
        {
            get { return dontSizeBoxSizeY; }
            set { dontSizeBoxSizeY = value; }
        }

        [SerializeField, Tooltip("Doesnt size box size z.")]
        private bool dontSizeBoxSizeZ = false;
        public bool DontSizeBoxSizeZ
        {
            get { return dontSizeBoxSizeZ; }
            set { dontSizeBoxSizeZ = value; }
        }

        [SerializeField, Tooltip("Raycast hit point."), HideInInspector()]
		private Vector3 hitPoint;
		public Vector3 HitPoint
		{
			get { return hitPoint; }
			set { hitPoint = value; }
		}

        [SerializeField, Tooltip("Layer mask used with the ray cast.")]
        private LayerMask layerMask = -1;
        public LayerMask _layerMask
        {
            get { return layerMask; }
            set { layerMask = value; }
        }

        private CGF cgf;

        private float gizmoSize = .25f;

        #endregion

        #region Gizmos

        void OnDrawGizmos()
        {
            Vector3 fwd = this.transform.TransformDirection(Vector3.forward);

            RaycastHit hitInfo;

            if(this.GetComponent<CGF>() != null)
            {
                gizmoSize = (this.GetComponent<CGF>().Size / 8f);
                if (gizmoSize > .25f)
                    gizmoSize = .25f;
                else if (gizmoSize < -.25f)
                    gizmoSize = -.25f;
            }

            Color activeColor = Color.cyan;
            Color nonActiveColor = Color.white;

            if (Physics.Raycast(this.transform.position, fwd, out hitInfo, MaxCgfSize, _layerMask))
            {
                if (hitInfo.distance > maxCgfSize)
                {
                    Gizmos.color = nonActiveColor;
                    Gizmos.DrawLine(this.transform.position, hitInfo.point);
                    Gizmos.DrawSphere(hitInfo.point, gizmoSize);
                    return;
                }

                Gizmos.color = activeColor;
                Gizmos.DrawLine(this.transform.position, hitInfo.point);
                Gizmos.DrawSphere(hitInfo.point, gizmoSize);
                Gizmos.DrawSphere(hitInfo.point + (fwd * OffsetRaycast), gizmoSize);
            }
            else
            {
                Gizmos.color = nonActiveColor;
                Gizmos.DrawLine(this.transform.position, this.transform.position + (fwd * MaxCgfSize));
            }
        }

        #endregion

        #region Unity Functions

        // Use this for initialization
        void Start()
        {
            cgf = this.GetComponent<CGF>();

            cgf.Size = maxCgfSize;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 fwd = this.transform.TransformDirection(Vector3.forward);

            RaycastHit hitInfo;

            float setSize = cgf.Size;

            if (Physics.Raycast(this.transform.position, fwd, out hitInfo, MaxCgfSize, _layerMask))
            {
                if (hitInfo.distance > maxCgfSize)
                {
                    setSize = maxCgfSize + OffsetRaycast;
                    return;
                }

                setSize = hitInfo.distance + OffsetRaycast;

				hitPoint = hitInfo.point;
            }

			if(hitInfo.distance == 0)
			{
				hitPoint = Vector3.zero;
                setSize = maxCgfSize + OffsetRaycast;
			}

            if(cgf._shape != CGF.Shape.Box)
                cgf.Size = setSize;
            else
            {
                Vector3 setBoxSize = new Vector3(cgf.BoxSize.x, cgf.BoxSize.y, cgf.BoxSize.z);
                if (!DontSizeBoxSizeX)
                    setBoxSize = new Vector3(setSize, setBoxSize.y, setBoxSize.z);
                if (!DontSizeBoxSizeY)
                    setBoxSize = new Vector3(setBoxSize.x, setSize, setBoxSize.z);
                if (!DontSizeBoxSizeZ)
                    setBoxSize = new Vector3(setBoxSize.x, setBoxSize.y, setSize);

                cgf.BoxSize = setBoxSize;
            }
        }

        #endregion
    }
}