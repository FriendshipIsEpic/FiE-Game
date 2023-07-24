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
    [RequireComponent(typeof(CGF))]
    public class CGF_AlignToForce : MonoBehaviour
    {
        #region Events

        public delegate void ApplyAlignToForceEvent(CGF cgf, Rigidbody rigid, Collider coll, Vector3 transPos);
        public static event ApplyAlignToForceEvent OnApplyAlignToForceEvent;

        #endregion

        public enum AlignDirection
        {
            Up,
            Down,
            Left,
            Right,
            Forward,
            Backward
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

        [SerializeField, Tooltip("The maximimum angular velocity of the rigidbody.")]
        private float maxAngularVelocity = 7f;
        public float MaxAngularVelocity
        {
            get { return maxAngularVelocity; }
            set { maxAngularVelocity = value; }
        }

        [SerializeField, Tooltip("Angular velocity damping.")]
        private float angularVelocityDamping = 1f;
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
        private CGF.FilterProperties filterProperties;
        public CGF.FilterProperties _filterProperties
        {
            get { return filterProperties; }
            set { filterProperties = value; }
        }

        private CGF cgf;

        void Awake()
        {
            CGF.OnApplyCGFEvent += CGF_OnApplyCGFEvent;

            cgf = this.GetComponent<CGF>();
        }

        void OnDestroy()
        {
            CGF.OnApplyCGFEvent -= CGF_OnApplyCGFEvent;
        }

        private void CGF_OnApplyCGFEvent(CGF cgf, Rigidbody rigid, Collider coll)
        {
            if (this.cgf == cgf)
            {
                ApplyAlignment(cgf, rigid, coll);
            }
        }

        //Applys the alignment
        private void ApplyAlignment(CGF cgf, Rigidbody rigid, Collider coll)
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
                                        var point = cgf.FindClosestPoints(rigid.position, cgf._forcePositionProperties.ClosestColliders);

                                        transPos = cgf.GetVectorHeightOffset(point, rigid.position, heightOffset);
                                    }
                                    else
                                    {
                                        Vector3 pointA = cgf.FindClosestPoints(coll.transform.position, cgf._forcePositionProperties.ClosestColliders);
                                        Vector3 pointB = cgf.FindClosestPoints(pointA, coll);

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
                            newLocal = -rigid.transform.up;
                            break;
                        case AlignDirection.Down:
                            newLocal = rigid.transform.up;
                            break;
                        case AlignDirection.Left:
                            newLocal = rigid.transform.right;
                            break;
                        case AlignDirection.Right:
                            newLocal = -rigid.transform.right;
                            break;
                        case AlignDirection.Forward:
                            newLocal = -rigid.transform.forward;
                            break;
                        case AlignDirection.Backward:
                            newLocal = rigid.transform.forward;
                            break;
                    }

                    Quaternion targetRotation = Quaternion.FromToRotation(newLocal, rigid.position - transPos) * rigid.rotation;

                    Quaternion deltaRotation = Quaternion.Inverse(rigid.rotation) * targetRotation;
                    Vector3 deltaAngles = GetRelativeAngles(deltaRotation.eulerAngles);
                    Vector3 worldDeltaAngles = rigid.transform.TransformDirection(deltaAngles);

                    rigid.maxAngularVelocity = MaxAngularVelocity;
                    rigid.AddTorque((RotateSpeed * worldDeltaAngles) - ((AngularVelocityDamping * rigid.angularVelocity)));

                    if (OnApplyAlignToForceEvent != null)
                    {
                        OnApplyAlignToForceEvent.Invoke(cgf, rigid, coll, transPos);
                    }
                }
            }
        }

        // Convert angles above 180 degrees into negative/relative angles
        Vector3 GetRelativeAngles(Vector3 angles)
        {
            Vector3 relativeAngles = angles;
            if (relativeAngles.x > 180f)
                relativeAngles.x -= 360f;
            if (relativeAngles.y > 180f)
                relativeAngles.y -= 360f;
            if (relativeAngles.z > 180f)
                relativeAngles.z -= 360f;

            return relativeAngles;
        }
    }
}