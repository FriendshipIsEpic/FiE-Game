using System;
using UnityEngine;

namespace Fie.Object
{
	[Serializable]
	public class FieHealthStats
	{
		public const float DEFAULT_REGENERATE_DELAY = 3f;

		public const float DEFAULT_HITPOINT_REGENERATE_PER_SEC = 0.15f;

		public const float DEFAULT_SHIED_REGENERATE_PER_SEC = 0.2f;

		public const float DEFAULT_STAGGER_ATTENUATION_RATE_PER_SEC = 0.3f;

		public const float WEAK_ATTRIBUTE_DAMAGE_MAGNIFY = 0.3f;

		public const float STRONG_ATTRIBUTE_DAMAGE_MAGNIFY = -0.3f;

		public const float MAXIMUM_WEAK_ATTRIBUTE_DAMAGE_MAGNIFY = 10f;

		private FieAttribute _shieldType;

		private float _hitPoint;

		private float _shield;

		private float _stagger;

		[SerializeField]
		private float _maxHitPoint;

		[SerializeField]
		private float _maxShield;

		[SerializeField]
		private float _staggerResistance;

		[SerializeField]
		private float _regenerateDelay = 3f;

		[SerializeField]
		private float _hitPointRegeneratePerSec = 0.2f;

		[SerializeField]
		private float _shieldRegeneratePerSec = 0.2f;

		[SerializeField]
		private float _staggerAttenuationPerSec = 0.3f;

		[SerializeField]
		private float _weakAttributeDamageMagnify = 0.3f;

		[SerializeField]
		private float _strongAttributeDamageMagnify = -0.3f;

		public FieAttribute shieldType
		{
			get
			{
				return _shieldType;
			}
			set
			{
				_shieldType = value;
			}
		}

		public float hitPoint
		{
			get
			{
				return _hitPoint;
			}
			set
			{
				_hitPoint = Mathf.Max(value, 0f);
			}
		}

		public float shield
		{
			get
			{
				return _shield;
			}
			set
			{
				_shield = Mathf.Max(value, 0f);
			}
		}

		public float stagger
		{
			get
			{
				return _stagger;
			}
			set
			{
				_stagger = Mathf.Max(value, 0f);
			}
		}

		public float maxHitPoint
		{
			get
			{
				return _maxHitPoint;
			}
			set
			{
				_maxHitPoint = Mathf.Max(value, 1f);
			}
		}

		public float maxShield
		{
			get
			{
				return _maxShield;
			}
			set
			{
				_maxShield = Mathf.Max(value, 1f);
			}
		}

		public float staggerResistance
		{
			get
			{
				return _staggerResistance;
			}
			set
			{
				_staggerResistance = Mathf.Max(value, 1f);
			}
		}

		public float regenerateDelay
		{
			get
			{
				return _regenerateDelay;
			}
			set
			{
				_regenerateDelay = Mathf.Max(value, 0f);
			}
		}

		public float hitPointRegeneratePerSec
		{
			get
			{
				return _hitPointRegeneratePerSec;
			}
			set
			{
				_hitPointRegeneratePerSec = Mathf.Max(value, 0f);
			}
		}

		public float shieldRegeneratePerSec
		{
			get
			{
				return _shieldRegeneratePerSec;
			}
			set
			{
				_shieldRegeneratePerSec = Mathf.Max(value, 0f);
			}
		}

		public float staggerAttenuationPerSec
		{
			get
			{
				return _staggerAttenuationPerSec;
			}
			set
			{
				_staggerAttenuationPerSec = Mathf.Max(value, 0f);
			}
		}

		public float weakAttributeDamageMagnify
		{
			get
			{
				return _weakAttributeDamageMagnify;
			}
			set
			{
				_weakAttributeDamageMagnify = Mathf.Clamp(value, 0f, 10f);
			}
		}

		public float strongAttributeDamageMagnify
		{
			get
			{
				return _strongAttributeDamageMagnify;
			}
			set
			{
				_strongAttributeDamageMagnify = Mathf.Clamp(value, -1f, 0f);
			}
		}

		public float nowHelthAndShieldRatePerMax => (hitPoint + shield) / Mathf.Max(maxHitPoint + maxShield, 0.1f);
	}
}
