namespace Fie.Object
{
	[FieAbilityID(FieConstValues.FieAbility.NONE)]
	public abstract class FieStateMachineAbilityBase : FieStateMachineGameCharacterBase, FieStateMachineAbilityInterface
	{
		public float defaultCoolDown;

		public abstract FieAbilityActivationType getActivationType();

		public abstract string getSignature();

		public virtual float getCooldown()
		{
			return defaultCoolDown;
		}
	}
}
