using UnityEngine;

namespace Fie.Object
{
	public sealed class FieStatusEffectsRift : FieStatusEffectsBase
	{
		[SerializeField]
		private float _riftForceRate = 1f;

		[SerializeField]
		private bool _resetMoveForce;

		public override FieStatusEffectEntityBase GetStatusEffectEntity()
		{
			FieStatusEffectsRiftEntity fieStatusEffectsRiftEntity = new FieStatusEffectsRiftEntity();
			fieStatusEffectsRiftEntity.riftForceRate = _riftForceRate;
			fieStatusEffectsRiftEntity.resetMoveForce = _resetMoveForce;
			return fieStatusEffectsRiftEntity;
		}
	}
}
