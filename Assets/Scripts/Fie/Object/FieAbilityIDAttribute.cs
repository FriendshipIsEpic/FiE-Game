using System;

namespace Fie.Object
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class FieAbilityIDAttribute : Attribute
	{
		public readonly FieConstValues.FieAbility abilityID;

		public FieAbilityIDAttribute(FieConstValues.FieAbility abilityID)
		{
			this.abilityID = abilityID;
		}
	}
}
