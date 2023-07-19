using System;

namespace Fie.Object
{
	[Serializable]
	public class FieStatusEffectsBuffAndDebuffToDeffenceEntity : FieStatusEffectEntityBase
	{
		public int skillID = -1;

		public float magni;

		public bool isEnableStack;
	}
}
