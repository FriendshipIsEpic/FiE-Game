using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackBaseAttack : FieStateMachineGameCharacterBase
	{
		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieApplejack)
			{
				FieApplejack fieApplejack = gameCharacter as FieApplejack;
				if (true)
				{
					Vector3 vector = (fieApplejack.flipState != 0) ? Vector3.right : Vector3.left;
					Type type = fieApplejack.getStateMachine().nowStateType();
					if (fieApplejack.groundState == FieObjectGroundState.Grounding)
					{
						if (type == typeof(FieStateMachineApplejackRopeAction))
						{
							fieApplejack.getStateMachine().setState(typeof(FieStateMachineApplejackFireKickRift), isForceSet: false);
						}
						else if (type == typeof(FieStateMachineApplejackFirePunch))
						{
							fieApplejack.getStateMachine().setState(typeof(FieStateMachineApplejackFireKick), isForceSet: false);
						}
						else
						{
							fieApplejack.getStateMachine().setState(typeof(FieStateMachineApplejackFirePunch), isForceSet: false);
						}
						_isEnd = true;
					}
					else if (fieApplejack.groundState == FieObjectGroundState.Flying)
					{
						if (type == typeof(FieStateMachineApplejackRopeActionAir))
						{
							fieApplejack.getStateMachine().setState(typeof(FieStateMachineApplejackFireAirDouble), isForceSet: false);
						}
						else
						{
							fieApplejack.getStateMachine().setState(typeof(FieStateMachineApplejackFireAir), isForceSet: false);
						}
						_isEnd = true;
					}
					else
					{
						_isEnd = true;
					}
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_HONESTY_BASE_ATTACK), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_HONESTY_BASE_ATTACK));
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
