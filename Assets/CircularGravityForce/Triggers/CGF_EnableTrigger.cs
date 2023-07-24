/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for cgf trigger, used for enabling or disabling based of if the raycast
*              is tripped.
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace CircularGravityForce
{
    public class CGF_EnableTrigger : MonoBehaviour
    {
        #region Properties

        [SerializeField, Tooltip("Circular gravity force object used for the enable trigger.")]
        private CGF cgf;
        public CGF Cgf
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

            if (Physics.Raycast(this.transform.position, fwd, out hitInfo, maxTripDistance, _layerMask))
            {
                if (hitInfo.distance > maxTripDistance)
                {
                    Gizmos.color = nonActiveColor;
                    Gizmos.DrawLine(this.transform.position, hitInfo.point);
                    Gizmos.DrawSphere(this.transform.position, gizmoSize);
                    Gizmos.DrawSphere(hitInfo.point, gizmoSize);
                    return;
                }

                Gizmos.color = activeColor;
                Gizmos.DrawLine(this.transform.position, hitInfo.point);
                Gizmos.DrawSphere(hitInfo.point, gizmoSize);
            }
            else
            {
                Gizmos.color = nonActiveColor;
                Gizmos.DrawLine(this.transform.position, this.transform.position + (fwd * MaxTripDistance));
            }

            Gizmos.DrawSphere(this.transform.position, gizmoSize);
        }

        #endregion

        #region Unity Functions

        void Start()
        {
            cgf.Enable = !TripValue;
        }

        void Update()
        {
            Vector3 fwd = this.transform.TransformDirection(Vector3.forward);

            RaycastHit hitInfo;

            if (Physics.Raycast(this.transform.position, fwd, out hitInfo, maxTripDistance, _layerMask))
            {
                if (hitInfo.distance > maxTripDistance)
                {
                    cgf.Enable = !TripValue;
					return;
                }

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
