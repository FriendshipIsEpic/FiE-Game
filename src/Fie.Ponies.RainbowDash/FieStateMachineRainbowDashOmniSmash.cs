using Fie.Object;
using System;

namespace Fie.Ponies.RainbowDash
{
	[FieAbilityID(FieConstValues.FieAbility.OMNISMASH)]
	public class FieStateMachineRainbowDashOmniSmash : FieStateMachineAbilityBase
	{
		private const string OMNI_SMASH_ABILITY_SIGNATURE = "omnismash";

		public const float OMNI_SMASH_DEFAULT_COOLDOWN = 0f;

		private Type _nextState = typeof(FieStateMachinePoniesAttackIdle);

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieRainbowDash)
			{
				FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
				fieRainbowDash.getStateMachine().setState(typeof(FieStateMachineRainbowDashOmniSmashStart), isForceSet: false);
				_isEnd = true;
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			defaultCoolDown = 0f;
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return _nextState;
		}

		public override string getSignature()
		{
			return "omnismash";
		}

		public override FieAbilityActivationType getActivationType()
		{
			return FieAbilityActivationType.SPECEFIC_METHOD;
		}

		public override bool isNotNetworkSync()
		{
			return true;
		}
	}
}
