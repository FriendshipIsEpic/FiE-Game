using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using System;
using System.Collections.Generic;

namespace Fie.Ponies.Applejack
{
	[FieAbilityID(FieConstValues.FieAbility.STOMP)]
	public class FieStateMachineApplejackStomp : FieStateMachineAbilityBase
	{
		public const string STOMP_SIGNATURE = "stomp";

		public const float STOMP_DEFAULT_COOLDOWN = 7f;

		private Type _nextState = typeof(FieStateMachinePoniesAttackIdle);

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieApplejack)
			{
				FieApplejack fieApplejack = gameCharacter as FieApplejack;
				Type typeFromHandle = typeof(FieStateMachineApplejackStompAction);
				if (fieApplejack.groundState == FieObjectGroundState.Grounding)
				{
					typeFromHandle = typeof(FieStateMachineApplejackStompJump);
				}
				FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_HONESTY_ABILITY_3), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_HONESTY_ABILITY_3));
				FieStateMachineInterface fieStateMachineInterface = fieApplejack.getStateMachine().setState(typeFromHandle, isForceSet: false);
				_isEnd = true;
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			defaultCoolDown = 7f;
		}

		public override string getSignature()
		{
			return "stomp";
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return _nextState;
		}

		public override List<Type> getAllowedStateList()
		{
			return new List<Type>();
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
