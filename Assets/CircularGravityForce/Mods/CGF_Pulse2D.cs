/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for cgf mod, creates a pulse effect using the cgf in 2D.
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace CircularGravityForce
{
    [RequireComponent(typeof(CGF2D))]
    public class CGF_Pulse2D : MonoBehaviour
    {
        #region Classis

        //Pulse properties
        [System.Serializable]
        public class PulseProperties
        {
            [SerializeField, Tooltip("Enable/Disable's the pulse.")]
            private bool pulse = true;
            public bool Pulse
            {
                get { return pulse; }
                set { pulse = value; }
            }
            [SerializeField, Tooltip("Speed of the pulse.")]
            private float speed = 10f;
            public float Speed
            {
                get { return speed; }
                set { speed = value; }
            }
            [SerializeField, Tooltip("Min pulse size.")]
            private float minSize = 1f;
            public float MinSize
            {
                get { return minSize; }
                set { MinSize = value; }
            }
            [SerializeField, Tooltip("Max pulse size.")]
            private float maxSize = 5f;
            public float MaxSize
            {
                get { return maxSize; }
                set { maxSize = value; }
            }

            [SerializeField, Tooltip("Min pulse box size.")]
            private Vector2 minBoxSize = Vector2.one;
            public Vector2 MinBoxSize
            {
                get { return minBoxSize; }
                set { minBoxSize = value; }
            }

            [SerializeField, Tooltip("Max pulse box size.")]
            private Vector2 maxBoxSize = Vector2.one * 5f;
            public Vector2 MaxBoxSize
            {
                get { return maxBoxSize; }
                set { maxBoxSize = value; }
            }
        }

        #endregion

        #region Properties/Constructor

        [SerializeField, Tooltip("Pulse properties.")]
        private PulseProperties pulseProperties;
        public PulseProperties _pulseProperties
        {
            get { return pulseProperties; }
            set { pulseProperties = value; }
        }

        private CGF2D cgf;

        //Used to tell whether to add or subtract to pulse
        private bool pulse_Positive;
        private bool pulse_PositiveBoxX;
        private bool pulse_PositiveBoxY;

        public CGF_Pulse2D()
        {
            _pulseProperties = new PulseProperties();
        }

        #endregion

        #region Unity Functions

        void Start()
        {
            cgf = this.GetComponent<CGF2D>();

            if (_pulseProperties.Pulse)
            {
                cgf.Size = _pulseProperties.MinSize;
                pulse_Positive = true;
                pulse_PositiveBoxX = true;
                pulse_PositiveBoxY = true;
            }
        }

        void Update()
        {
            if (cgf.Enable)
            {
                if (_pulseProperties.Pulse)
                {
                    CalculatePulse();
                }
            }
        }

        #endregion

        #region Functions

        //Calculatie the given pulse
        void CalculatePulse()
        {
            if (_pulseProperties.Pulse)
            {
                if (cgf._shape2D != CGF2D.Shape2D.Box)
                    CalculatePulseBySize();
                else
                    CalculatePulseByBoxSize();
            }
        }

        void CalculatePulseBySize()
        {
            if (pulse_Positive)
            {
                if (cgf.Size <= _pulseProperties.MaxSize)
                    cgf.Size = cgf.Size + (_pulseProperties.Speed * Time.deltaTime);
                else
                    pulse_Positive = false;
            }
            else
            {
                if (cgf.Size >= _pulseProperties.MinSize)
                    cgf.Size = cgf.Size - (_pulseProperties.Speed * Time.deltaTime);
                else
                    pulse_Positive = true;
            }
        }

        void CalculatePulseByBoxSize()
        {
            if (pulse_PositiveBoxX)
            {
                if (cgf.BoxSize.x <= _pulseProperties.MaxBoxSize.x)
                    cgf.BoxSize = new Vector2(cgf.BoxSize.x + (_pulseProperties.Speed * Time.deltaTime), cgf.BoxSize.y);
                else
                    pulse_PositiveBoxX = false;
            }
            else
            {
                if (cgf.BoxSize.x >= _pulseProperties.MinBoxSize.x)
                    cgf.BoxSize = new Vector2(cgf.BoxSize.x - (_pulseProperties.Speed * Time.deltaTime), cgf.BoxSize.y);
                else
                    pulse_PositiveBoxX = true;
            }

            if (pulse_PositiveBoxY)
            {
                if (cgf.BoxSize.y <= _pulseProperties.MaxBoxSize.y)
                    cgf.BoxSize = new Vector2(cgf.BoxSize.x, cgf.BoxSize.y + (_pulseProperties.Speed * Time.deltaTime));
                else
                    pulse_PositiveBoxY = false;
            }
            else
            {
                if (cgf.BoxSize.y >= _pulseProperties.MinBoxSize.y)
                    cgf.BoxSize = new Vector2(cgf.BoxSize.x, cgf.BoxSize.y - (_pulseProperties.Speed * Time.deltaTime));
                else
                    pulse_PositiveBoxY = true;
            }
        }

        #endregion
    }
}
