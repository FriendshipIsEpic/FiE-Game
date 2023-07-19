using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackEvasion : FieStateMachineGameCharacterBase
	{
		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieApplejack)
			{
				FieApplejack fieApplejack = gameCharacter as FieApplejack;
				float num = Vector3.Dot(fieApplejack.externalInputVector.normalized, fieApplejack.flipDirectionVector);
				if (num > 0.75f)
				{
					_nextState = typeof(FieStateMachineApplejackCharge);
				}
				else
				{
					_nextState = typeof(FieStateMachineApplejackBackstep);
				}
				fieApplejack.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_EVADED), 25);
				FieStateMachineInterface fieStateMachineInterface = fieApplejack.getStateMachine().setState(_nextState, isForceSet: false);
				FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_HONESTY_EVADE), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_HONESTY_EVADE));
				_isEnd = true;
			}
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return _nextState;
		}

		public override bool isNotNetworkSync()
		{
			return true;
		}
	}
}
