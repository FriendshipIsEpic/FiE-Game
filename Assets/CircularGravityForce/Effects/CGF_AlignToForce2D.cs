/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for cgf effects, enables align to force effect.
*******************************************************************************************/
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircularGravityForce
{
    [RequireComponent(typeof(CGF2D))]
    public class CGF_AlignToForce2D : MonoBehaviour
    {
        #region Events

        public delegate void ApplyAlignToForceEvent(CGF2D cgf, Rigidbody2D rigid, Collider2D coll, Vector3 transPos);
        public static event ApplyAlignToForceEvent OnApplyAlignToForceEvent;

        #endregion

        public enum AlignDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        [SerializeField, Tooltip("Enables align to force.")]
        private bool enable = true;
        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        [SerializeField, Tooltip("If alignToForce is enabled, lets you pick the align direction of the GameObjects.")]
        private AlignDirection alignDirection = AlignDirection.Down;
        public AlignDirection _alignDirection
        {
            get { return alignDirection; }
            set { alignDirection = value; }
        }

        [SerializeField, Tooltip("Rotation speed.")]
        private float rotateSpeed = 1f;
        public float RotateSpeed
        {
            get { return rotateSpeed; }
            set { rotateSpeed = value; }
        }

        [SerializeField, Tooltip("Angular velocity damping.")]
        private float angularVelocityDamping = 0.1f;
        public float AngularVelocityDamping
        {
            get { return angularVelocityDamping; }
            set { angularVelocityDamping = value; }
        }

        [SerializeField, Tooltip("Use closest collider height offset.")]
        private bool useClosestColliderHeightOffset = false;
        public bool UseClosestColliderHeightOffset
        {
            get { return useClosestColliderHeightOffset; }
            set { useClosestColliderHeightOffset = value; }
        }

        [SerializeField, Tooltip("Filter properties options.")]
        private CGF2D.FilterProperties filterProperties;
        public CGF2D.FilterProperties _filterProperties
        {
            get { return filterProperties; }
            set { filterProperties = value; }
        }

        private CGF2D cgf;

        void Awake()
        {
            CGF2D.OnApplyCGFEvent += CGF_OnApplyCGFEvent;

            cgf = this.GetComponent<CGF2D>();
        }

        void OnDestroy()
        {
            CGF2D.OnApplyCGFEvent -= CGF_OnApplyCGFEvent;
        }

        private void CGF_OnApplyCGFEvent(CGF2D cgf, Rigidbody2D rigid, Collider2D coll)
        {
            if (this.cgf == cgf)
            {
                ApplyAlignment(cgf, rigid, coll);
            }
        }

        //Applys the alignment
        private void ApplyAlignment(CGF2D cgf, Rigidbody2D rigid, Collider2D coll)
        {
            if (_filterProperties == null)
                return;

            if (Enable)
            {
                if (_filterProperties.ValidateFilters(rigid, coll))
                {
                    var transPos = this.transform.position;

                    switch (cgf._forcePositionProperties.ForcePosition)
                    {
                        case CGF.ForcePosition.ThisTransform:
                            break;
                        case CGF.ForcePosition.ClosestCollider:
                            if (cgf._forcePositionProperties.ClosestColliders != null)
                            {
                                if (cgf._forcePositionProperties.ClosestColliders.Count > 0)
                                {
                                    float heightOffset = 0f;

                                    if (UseClosestColliderHeightOffset)
                                        heightOffset = cgf._forcePositionProperties.HeightOffset;

                                    if (!cgf._forcePositionProperties.UseEffectedClosestPoint)
                                    {
                                        var point = cgf.FindClosestPoints(coll, cgf._forcePositionProperties.ClosestColliders, false);
                                        transPos = cgf.GetVectorHeightOffset(point, coll.transform.position, heightOffset);
                                    }
                                    else
                                    {
                                        Vector3 pointA = cgf.FindClosestPoints(coll, cgf._forcePositionProperties.ClosestColliders, false);
                                        Vector3 pointB = cgf.FindClosestPoints(coll, cgf._forcePositionProperties.ClosestColliders, true);

                                        float distanceThisA = Vector3.Distance(coll.transform.position, pointA);
                                        float distanceAB = Vector3.Distance(pointA, pointB);

                                        transPos = cgf.GetVectorHeightOffset(pointA, coll.transform.position, Mathf.Abs(distanceThisA - distanceAB) + heightOffset);
                                    }
                                }
                            }
                            break;
                    }

                    Vector3 newLocal = Vector3.zero;

                    switch (_alignDirection)
                    {
                        case AlignDirection.Up:
                            newLocal = rigid.transform.up;
                            break;
                        case AlignDirection.Down:
                            newLocal = -rigid.transform.up;
                            break;
                        case AlignDirection.Left:
                            newLocal = -rigid.transform.right;
                            break;
                        case AlignDirection.Right:
                            newLocal = rigid.transform.right;
                            break;
                    }

                    float angle = LookAtAngle(rigid, newLocal, transPos);

                    rigid.AddTorque((RotateSpeed * angle) - ((rigid.angularVelocity * AngularVelocityDamping) * Time.deltaTime));

                    if (OnApplyAlignToForceEvent != null)
                    {
                        OnApplyAlignToForceEvent.Invoke(cgf, rigid, coll, transPos);
                    }
                }
            }
        }

        public float LookAtAngle(Rigidbody2D rigid2D, Vector3 alignDirection, Vector3 target)
        {
            int turnTo = 0;

            Vector3 angleRelativeToMe = target - rigid2D.transform.position;

            //float angleDiff = Vector3.Angle(alignDirection, angleRelativeToMe);

            Vector3 cross = Vector3.Cross(alignDirection, angleRelativeToMe);

            if (cross.z < 0)
            {
                turnTo = -1;
            }
            else
            {
                turnTo = 1;
            }

            return turnTo;
        }
    }
}