using System;

namespace Fie.Object
{
	[Serializable]
	public class FieStatusEffectsBuffAndDebuffToAttackEntity : FieStatusEffectEntityBase
	{
		public int skillID = -1;

		public int abilityID = -1;

		public float magni;

		public bool isEnableStack;
	}
}
