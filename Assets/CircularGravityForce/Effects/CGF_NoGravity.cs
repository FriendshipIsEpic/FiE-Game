/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for cgf effects, enables no gravity effect.
*******************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CircularGravityForce
{
    [RequireComponent(typeof(CGF))]
    public class CGF_NoGravity : MonoBehaviour
    {
        public class OnGravity_CGFCollection : CGFCollection
        {
            public override void UpdateEffectedObject(Rigidbody rigid)
            {
                base.UpdateEffectedObject(rigid);

                rigid.useGravity = false;
            }

            public override void ResetEffectedObject(EffectedObject effectedObject)
            {
                base.ResetEffectedObject(effectedObject);

                effectedObject._rigidbody.useGravity = true;
            }
        }

        [SerializeField, Tooltip("Enables no gravity.")]
        private bool enable = true;
        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        [SerializeField, Tooltip("")]
        private float timeEffected = 3f;
        public float TimeEffected
        {
            get { return timeEffected; }
            set { timeEffected = value; }
        }

        [SerializeField, Tooltip("Filter properties options.")]
        private CGF.FilterProperties filterProperties;
        public CGF.FilterProperties _filterProperties
        {
            get { return filterProperties; }
            set { filterProperties = value; }
        }

        private OnGravity_CGFCollection cgfCollection;
        public OnGravity_CGFCollection _cgfCollection
        {
            get { return cgfCollection; }
            set { cgfCollection = value; }
        }

        private CGF cgf;

        public CGF_NoGravity()
        {
            _cgfCollection = new OnGravity_CGFCollection();
        }

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
            if (_filterProperties == null)
                return;

            if (Enable)
            {
                if (this.cgf == cgf)
                {
                    if (_filterProperties != null)
                    {
                        if (_filterProperties.ValidateFilters(rigid, coll))
                        {
                            _cgfCollection.Add(rigid, Time.time, TimeEffected);
                        }
                    }
                }
            }
        }

        void Update()
        {
            _cgfCollection.Sync();
        }
    }
}