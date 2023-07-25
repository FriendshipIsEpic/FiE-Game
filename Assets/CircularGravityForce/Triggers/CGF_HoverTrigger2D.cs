/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for cgf trigger, used creating a hover effect using the cgf object in 2D.
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace CircularGravityForce
{
    public class CGF_HoverTrigger2D : MonoBehaviour
    {
        #region Properties

        [SerializeField, Tooltip("Circular gravity force object used for the hover trigger.")]
        private CGF2D cgf;
        public CGF2D Cgf
        {
            get { return cgf; }
            set { cgf = value; }
        }

        [SerializeField, Tooltip("Hover force power.")]
        private float forcePower = 30f;
        public float ForcePower
        {
            get { return forcePower; }
            set { forcePower = value; }
        }

        [SerializeField, Tooltip("Max distace it can hover.")]
        private float hoverDistance = 3f;
        public float HoverDistance
        {
            get { return hoverDistance; }
            set { hoverDistance = value; }
        }

        [SerializeField, Tooltip("Max distace it can hover.")]
        private float maxDistance = 10f;
        public float MaxDistance
        {
            get { return maxDistance; }
            set { maxDistance = value; }
        }

        [SerializeField, Tooltip("Layer mask used from the ray cast.")]
        private LayerMask layerMask = -1;
        public LayerMask _layerMask
        {
            get { return layerMask; }
            set { layerMask = value; }
        }

        private float gizmoSize = .25f;

        #endregion

        #region Gizmos

        void OnDrawGizmos()
        {
            Vector3 fwd = this.transform.TransformDirection(Vector3.right);

			RaycastHit2D hitInfo = Physics2D.Raycast(this.transform.position, fwd, MaxDistance, _layerMask);

            if (cgf != null)
            {
                gizmoSize = (cgf.Size / 8f);
                if (gizmoSize > .25f)
                    gizmoSize = .25f;
                else if (gizmoSize < -.25f)
                    gizmoSize = -.25f;
            }

            Color activeColor = Color.cyan;
            Color nonActiveColor = Color.white;

            Gizmos.DrawSphere(this.transform.position, gizmoSize);

            if (hitInfo.transform == null)
            {
                Gizmos.color = nonActiveColor;
                Gizmos.DrawLine(this.transform.position, this.transform.position + (fwd * MaxDistance));
                return;
            }

            if (Vector2.Distance(this.transform.position, hitInfo.point) > maxDistance)
            {
                Gizmos.color = nonActiveColor;
                Gizmos.DrawLine(this.transform.position, hitInfo.point);

                Gizmos.DrawSphere(hitInfo.point, gizmoSize);
                return;
            }
            else if (hitInfo.transform != null)
            {
                Gizmos.color = activeColor;
                Gizmos.DrawLine(this.transform.position, hitInfo.point);
                Gizmos.DrawSphere(this.transform.position, gizmoSize);
                Gizmos.DrawSphere(hitInfo.point, gizmoSize);
            }
        }

        #endregion

        #region Unity Functions

        void Update()
        {
            Vector3 fwd = this.transform.TransformDirection(Vector3.right);

			RaycastHit2D hitInfo = Physics2D.Raycast(this.transform.position, fwd, MaxDistance, _layerMask);

            if (Vector2.Distance(this.transform.position, hitInfo.point) > maxDistance)
            {
                cgf.ForcePower = 0f;
            }

            if (hitInfo.transform != null)
            {
                float proportionalHeight = (HoverDistance - Vector2.Distance(this.transform.position, hitInfo.point)) / HoverDistance;
                cgf.ForcePower = proportionalHeight * ForcePower;
            }
            else
            {
                cgf.ForcePower = 0f;
            }
        }

        #endregion
    }
}