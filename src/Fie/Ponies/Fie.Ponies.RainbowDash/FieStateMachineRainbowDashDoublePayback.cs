using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using System;

namespace Fie.Ponies.RainbowDash
{
	[FieAbilityID(FieConstValues.FieAbility.DOUBLE_PAYBACK)]
	public class FieStateMachineRainbowDashDoublePayback : FieStateMachineAbilityBase
	{
		private const string DOUBLE_PAYBACK_ABILITY_SIGNATURE = "double_payback";

		public const float DOUBLE_PAYBACK_DEFAULT_COOLDOWN = 12f;

		public const int DOUBLE_PAYBACK_GETTING_AWESOME_COUNT = 1;

		private Type _nextState = typeof(FieStateMachinePoniesAttackIdle);

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieRainbowDash)
			{
				FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
				Type type = null;
				type = ((fieRainbowDash.groundState != 0) ? typeof(FieStateMachineRainbowDashDoublePaybackPrepareMidAir) : typeof(FieStateMachineRainbowDashDoublePaybackPrepareOnGround));
				FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_LOYALTY_ABILITY_1), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_LOYALTY_ABILITY_1));
				fieRainbowDash.getStateMachine().setState(type, isForceSet: false);
				_isEnd = true;
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			defaultCoolDown = 12f;
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
			return "double_payback";
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
