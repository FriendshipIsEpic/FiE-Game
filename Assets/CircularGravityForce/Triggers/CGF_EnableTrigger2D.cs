/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for cgf trigger, used for enabling or disabling based of if the raycast
*              is tripped in 2D.
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace CircularGravityForce
{
    public class CGF_EnableTrigger2D : MonoBehaviour
    {
        #region Properties

        [SerializeField, Tooltip("Circular gravity force object used for the enable trigger.")]
        private CGF2D cgf;
        public CGF2D Cgf
        {
            get { return cgf; }
            set { cgf = value; }
        }

        [SerializeField, Tooltip("Value when tripped.")]
        private bool tripValue = true;
        public bool TripValue
        {
            get { return tripValue; }
            set { tripValue = value; }
        }

        [SerializeField, Tooltip("Max trip wire distance.")]
        private float maxTripDistance = 10f;
        public float MaxTripDistance
        {
            get { return maxTripDistance; }
            set { maxTripDistance = value; }
        }

        //Used for if you want to ignore a layer
        [SerializeField, Tooltip("Layer mask used for the ray cast.")]
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

            RaycastHit2D hitInfo = Physics2D.Raycast(this.transform.position, fwd, maxTripDistance, _layerMask);

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
                Gizmos.DrawLine(this.transform.position, this.transform.position + (fwd * maxTripDistance));
                return;
            }

            if (Vector2.Distance(this.transform.position, hitInfo.point) > maxTripDistance) 
			{
                Gizmos.color = nonActiveColor;
				Gizmos.DrawLine (this.transform.position, hitInfo.point);
                Gizmos.DrawSphere(hitInfo.point, gizmoSize);
				return;
			} 
			else if (hitInfo.transform != null) 
			{
                Gizmos.color = activeColor;
				Gizmos.DrawLine (this.transform.position, hitInfo.point);
                Gizmos.DrawSphere(this.transform.position, gizmoSize);
                Gizmos.DrawSphere(hitInfo.point, gizmoSize);
			}
		}

        #endregion

        #region Unity Functions

        void Start()
        {
            cgf.Enable = !TripValue;
        }

        void Update()
        {
            Vector3 fwd = this.transform.TransformDirection(Vector3.right);

            RaycastHit2D hitInfo = Physics2D.Raycast(this.transform.position, fwd, maxTripDistance, _layerMask);

            if (Vector2.Distance(this.transform.position, hitInfo.point) > maxTripDistance)
            {
                cgf.Enable = !TripValue;
                return;
            }

			if (hitInfo.transform != null) 
			{
                cgf.Enable = TripValue;
			}
            else
            {
                cgf.Enable = !TripValue;
            }
        }

        #endregion
    }
}
