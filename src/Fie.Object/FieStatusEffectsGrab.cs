namespace Fie.Object
{
	public sealed class FieStatusEffectsGrab : FieStatusEffectsBase
	{
		public new FieStatusEffectsGrabEntity _entity;

		public override FieStatusEffectEntityBase GetStatusEffectEntity()
		{
			return _entity;
		}
	}
}
