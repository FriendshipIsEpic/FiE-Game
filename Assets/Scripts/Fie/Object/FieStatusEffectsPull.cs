namespace Fie.Object
{
	public sealed class FieStatusEffectsPull : FieStatusEffectsBase
	{
		public new FieStatusEffectsPullEntity _entity;

		public override FieStatusEffectEntityBase GetStatusEffectEntity()
		{
			return _entity;
		}
	}
}
