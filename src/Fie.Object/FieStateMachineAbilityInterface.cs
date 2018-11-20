namespace Fie.Object
{
	public interface FieStateMachineAbilityInterface
	{
		FieAbilityActivationType getActivationType();

		string getSignature();

		float getCooldown();
	}
}
