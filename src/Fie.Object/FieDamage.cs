using System;
using System.Collections.Generic;

namespace Fie.Object
{
	[Serializable]
	public class FieDamage
	{
		public enum FieAttributeDamageState
		{
			DEFAULT,
			EFFECTIVE,
			NONEFFECTIVE
		}

		public const float DEFAULT_DAMAGE_FLUCTUATING_RATE = 0.05f;

		public FieAttribute attribute;

		public FieAttributeDamageState attributeDamageState;

		public float damage;

		public float finallyDamage;

		public float stagger;

		public float hate;

		public float fluctuatingRate;

		public List<FieStatusEffectEntityBase> statusEffects = new List<FieStatusEffectEntityBase>();

		public FieDamage()
		{
			attributeDamageState = FieAttributeDamageState.DEFAULT;
			attribute = FieAttribute.NONE;
			damage = (stagger = (hate = 0f));
			fluctuatingRate = 0.05f;
			finallyDamage = 0f;
		}

		public FieDamage(FieAttribute initAttribute, float initDamage, float initStagger, float initHate, float initFluctuatingRate, List<FieStatusEffectEntityBase> statusEffects)
		{
			attributeDamageState = FieAttributeDamageState.DEFAULT;
			attribute = initAttribute;
			damage = initDamage;
			stagger = initStagger;
			hate = initHate;
			fluctuatingRate = initFluctuatingRate;
			finallyDamage = 0f;
			this.statusEffects = statusEffects;
		}

		public FieDamage(float initStagger, float initHate)
		{
			attributeDamageState = FieAttributeDamageState.DEFAULT;
			attribute = FieAttribute.NONE;
			stagger = initStagger;
			hate = initHate;
			damage = 0f;
			fluctuatingRate = 0.05f;
			finallyDamage = 0f;
		}
	}
}
