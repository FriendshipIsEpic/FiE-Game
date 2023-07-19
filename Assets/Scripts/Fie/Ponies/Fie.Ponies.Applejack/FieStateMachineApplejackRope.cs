using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using System;
using System.Collections.Generic;

namespace Fie.Ponies.Applejack
{
	[FieAbilityID(FieConstValues.FieAbility.ROPE)]
	public class FieStateMachineApplejackRope : FieStateMachineAbilityBase
	{
		private const string ROPE_ABILITY_SIGNATURE = "rope";

		public const float ROPE_DEFAULT_COOLDOWN = 3f;

		private Type _nextState = typeof(FieStateMachinePoniesAttackIdle);

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieApplejack)
			{
				FieApplejack fieApplejack = gameCharacter as FieApplejack;
				FieStateMachineInterface fieStateMachineInterface = fieApplejack.getStateMachine().setState(typeof(FieStateMachineApplejackFireRope), isForceSet: false);
				FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_HONESTY_ABILITY_1), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_HONESTY_ABILITY_1));
				_isEnd = true;
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			defaultCoolDown = 3f;
		}

		public override string getSignature()
		{
			return "rope";
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
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineApplejackRopeActionAir));
			list.Add(typeof(FieStateMachineApplejackRopeAction));
			return list;
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
