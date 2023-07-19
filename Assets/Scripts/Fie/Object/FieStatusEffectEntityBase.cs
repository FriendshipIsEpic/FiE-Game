using System;

namespace Fie.Object
{
	[Serializable]
	public abstract class FieStatusEffectEntityBase
	{
		public bool isActive = true;

		public bool isOnlyStatusEffect;

		public float duration;

		public bool ApplyStatusEffect(FieDamageSystem healthSystem, FieGameCharacter attacker, FieDamage damage)
		{
			healthSystem.applyStatusEffectCallback(this, attacker, damage);
			return isOnlyStatusEffect;
		}
	}
}
