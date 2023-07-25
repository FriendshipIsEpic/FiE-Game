/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for cgf mod, sizes the cgf object based of the raycast hit point in 2D.
*******************************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CircularGravityForce
{
    [RequireComponent(typeof(CGF2D))]
    public class CGF_SizeByRaycast2D : MonoBehaviour
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

        [SerializeField, Tooltip("Raycast hit point.")]
		private Vector2 hitPoint;
		public Vector2 HitPoint
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

        private CGF2D cgf;

        private float gizmoSize = .25f;

        #endregion

        #region Gizmos

        void OnDrawGizmos()
        {
            Vector3 fwd = this.transform.TransformDirection(Vector3.right);

            RaycastHit2D hitInfo = Physics2D.Raycast(this.transform.position, fwd, MaxCgfSize, _layerMask);

            if (this.GetComponent<CGF>() != null)
            {
                gizmoSize = (this.GetComponent<CGF>().Size / 8f);
                if (gizmoSize > .25f)
                    gizmoSize = .25f;
                else if (gizmoSize < -.25f)
                    gizmoSize = -.25f;
            }

            Color activeColor = Color.cyan;
            Color nonActiveColor = Color.white;

            if (hitInfo.transform == null)
            {
                Gizmos.color = nonActiveColor;
                Gizmos.DrawLine(new Vector3(this.transform.position.x, this.transform.position.y, 0f), new Vector3(this.transform.position.x, this.transform.position.y, 0f) + (fwd * MaxCgfSize));
                return;
            }

            if (Vector2.Distance(this.transform.position, hitInfo.point) > maxCgfSize)
            {
                Gizmos.color = nonActiveColor;
                Gizmos.DrawLine(new Vector3(this.transform.position.x, this.transform.position.y, 0f), hitInfo.point);
                Gizmos.DrawSphere(hitInfo.point, gizmoSize);
                return;
            }
            else if(hitInfo.transform != null)
            {
                Gizmos.color = activeColor;
                Gizmos.DrawLine(new Vector3(this.transform.position.x, this.transform.position.y, 0f), hitInfo.point);
                Gizmos.DrawSphere(hitInfo.point, gizmoSize);
                Gizmos.DrawSphere((Vector3)hitInfo.point + (fwd * OffsetRaycast), gizmoSize);
            }
        }

        #endregion

        #region Unity Functions

        // Use this for initialization
        void Start()
        {
            cgf = this.GetComponent<CGF2D>();

            cgf.Size = maxCgfSize;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 fwd = this.transform.TransformDirection(Vector3.right);

            RaycastHit2D hitInfo = Physics2D.Raycast(this.transform.position, fwd, MaxCgfSize, _layerMask);

            float setSize = cgf.Size;

            if (Vector2.Distance(this.transform.position, hitInfo.point) > maxCgfSize)
            {
                setSize = maxCgfSize + OffsetRaycast;
                hitPoint = hitInfo.point;
                return;
            }

            if (Vector2.Distance(this.transform.position, hitInfo.point) == 0)
            {
                setSize = maxCgfSize + OffsetRaycast;
                hitPoint = Vector2.zero;
            }
            else
            {
                setSize = Vector2.Distance(this.transform.position, hitInfo.point) + OffsetRaycast;
                hitPoint = hitInfo.point;
            }

            if (cgf._shape2D != CGF2D.Shape2D.Box)
                cgf.Size = setSize;
            else
            {
                Vector2 setBoxSize = new Vector3(cgf.BoxSize.x, cgf.BoxSize.y);
                if (!DontSizeBoxSizeX)
                    setBoxSize = new Vector3(setSize, setBoxSize.y);
                if (!DontSizeBoxSizeY)
                    setBoxSize = new Vector3(setBoxSize.x, setSize);

                cgf.BoxSize = setBoxSize;
            }
        }
        #endregion
    }
}