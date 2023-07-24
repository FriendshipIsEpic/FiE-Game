/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for cgf trigger, used creating a hover effect using the cgf object.
*******************************************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CircularGravityForce
{
	public class CGF_HoverTrigger : MonoBehaviour
    {
        #region Properties

        [SerializeField, Tooltip("Circular gravity force object used for the hover trigger.")]
        private CGF cgf;
        public CGF Cgf
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

		[SerializeField, Tooltip("Hover distance.")]
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
			Vector3 fwd = this.transform.TransformDirection(Vector3.forward);
			
			RaycastHit hitInfo;

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

			if (Physics.Raycast(this.transform.position, fwd, out hitInfo, maxDistance, _layerMask))
			{
                if (hitInfo.distance < maxDistance)
                {
                    Gizmos.color = activeColor;
                }
                else
                {
                    Gizmos.color = nonActiveColor;
                }

                Gizmos.DrawLine(this.transform.position, hitInfo.point);
                Gizmos.DrawSphere(hitInfo.point, gizmoSize);
			}
			else
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(this.transform.position, this.transform.position + (fwd * MaxDistance));
			}

            Gizmos.DrawSphere(this.transform.position, gizmoSize);
		}
		
        #endregion

        #region Unity Functions

        void Update()
		{
			Vector3 fwd = this.transform.TransformDirection(Vector3.forward);
			
			RaycastHit hitInfo;
			
			if (Physics.Raycast(this.transform.position, fwd, out hitInfo, maxDistance, _layerMask))
			{
                if (hitInfo.distance < maxDistance)
                {
                    float proportionalHeight = (HoverDistance - hitInfo.distance) / HoverDistance;
                    cgf.ForcePower = proportionalHeight * ForcePower;
                }
                else
                {
                    cgf.ForcePower = 0f;
                }
			}
			else
			{
				cgf.ForcePower = 0f;
			}
        }

        #endregion
    }
}
