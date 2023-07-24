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
    [RequireComponent(typeof(CGF2D))]
    public class CGF_NoGravity2D : MonoBehaviour
    {
        public class OnGravity_CGFCollection : CGFCollection2D
        {
            public override void UpdateEffectedObject(Rigidbody2D rigid)
            {
                base.UpdateEffectedObject(rigid);

                rigid.gravityScale = 0f;
            }

            public override void ResetEffectedObject(EffectedObject effectedObject)
            {
                base.ResetEffectedObject(effectedObject);

                effectedObject._rigidbody.gravityScale = 1f;
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
        private CGF2D.FilterProperties filterProperties;
        public CGF2D.FilterProperties _filterProperties
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

        private CGF2D cgf;

        public CGF_NoGravity2D()
        {
            _cgfCollection = new OnGravity_CGFCollection();
        }

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