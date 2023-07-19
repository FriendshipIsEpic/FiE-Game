using System;

namespace Fie.Object
{
	[Serializable]
	public class FieStatusEffectsRiftEntity : FieStatusEffectEntityBase
	{
		public float riftForceRate = 1f;

		public bool resetMoveForce;
	}
}
