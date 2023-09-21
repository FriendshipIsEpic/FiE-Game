using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using System;

namespace Fie.Ponies.RainbowDash
{
	[FieAbilityID(FieConstValues.FieAbility.RAINBLOW)]
	public class FieStateMachineRainbowDashRainblow : FieStateMachineAbilityBase
	{
		private const string RAINBLOW_ABILITY_SIGNATURE = "rainblow";

		public const float RAINBLOW_COOLDOWN = 9f;

		private Type _nextState = typeof(FieStateMachinePoniesAttackIdle);

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieRainbowDash)
			{
				FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
				Type typeFromHandle = typeof(FieStateMachineRainbowDashRainblowCloack);
				FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_LOYALTY_ABILITY_2), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_LOYALTY_ABILITY_2));
				fieRainbowDash.getStateMachine().setState(typeFromHandle, isForceSet: false);
				_isEnd = true;
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			defaultCoolDown = 9f;
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
			return "rainblow";
		}

		public override FieAbilityActivationType getActivationType()
		{
			return FieAbilityActivationType.COOLDOWN;
		}

		public override bool isNotNetworkSync()
		{
			return true;
		}
	}
}
