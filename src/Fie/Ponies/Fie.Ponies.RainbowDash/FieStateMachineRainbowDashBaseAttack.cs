using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using System;

namespace Fie.Ponies.RainbowDash
{
	public class FieStateMachineRainbowDashBaseAttack : FieStateMachineGameCharacterBase
	{
		public const float BASE_ATTACK_REGENERATION_DELAY = 1.5f;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieRainbowDash)
			{
				FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
				if (true)
				{
					Type type = fieRainbowDash.getStateMachine().nowStateType();
					if (fieRainbowDash.isCloackMode)
					{
						fieRainbowDash.getStateMachine().setState(typeof(FieStateMachineRainbowDashStabAttack), isForceSet: false);
					}
					else if (type == typeof(FieStateMachineRainbowDashBaseAttack2))
					{
						fieRainbowDash.getStateMachine().setState(typeof(FieStateMachineRainbowDashBaseAttack3), isForceSet: false);
					}
					else if (type == typeof(FieStateMachineRainbowDashBaseAttack1))
					{
						fieRainbowDash.getStateMachine().setState(typeof(FieStateMachineRainbowDashBaseAttack2), isForceSet: false);
					}
					else
					{
						fieRainbowDash.getStateMachine().setState(typeof(FieStateMachineRainbowDashBaseAttack1), isForceSet: false);
					}
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_LOYALTY_BASE_ATTACK), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_LOYALTY_BASE_ATTACK));
					_isEnd = true;
				}
			}
		}

		private void emitShot()
		{
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return typeof(FieStateMachinePoniesAttackIdle);
		}

		public override bool isNotNetworkSync()
		{
			return true;
		}
	}
}
