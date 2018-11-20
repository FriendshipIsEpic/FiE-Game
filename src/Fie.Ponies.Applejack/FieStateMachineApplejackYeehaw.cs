using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using System;
using System.Collections.Generic;

namespace Fie.Ponies.Applejack
{
	[FieAbilityID(FieConstValues.FieAbility.YEEHAW)]
	public class FieStateMachineApplejackYeehaw : FieStateMachineAbilityBase
	{
		private const string YEEHAW_SIGNATRUE = "yeehaw";

		private const float YEEHAW_DEFAULT_COOLDOWN = 16f;

		public const float YEEHAW_REGEN_RATE = 0.2f;

		private Type _nextState = typeof(FieStateMachinePoniesAttackIdle);

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieApplejack)
			{
				FieApplejack fieApplejack = gameCharacter as FieApplejack;
				Type type = null;
				type = ((fieApplejack.groundState != 0) ? typeof(FieStateMachineApplejackYeehawActionMidAir) : typeof(FieStateMachineApplejackYeehawAction));
				FieStateMachineInterface fieStateMachineInterface = fieApplejack.getStateMachine().setState(type, isForceSet: false);
				if (fieStateMachineInterface != null)
				{
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_HONESTY_ABILITY_2), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_HONESTY_ABILITY_2));
					fieApplejack.abilitiesContainer.SetCooldown<FieStateMachineApplejackYeehaw>(16f);
				}
				_isEnd = true;
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			defaultCoolDown = 16f;
		}

		public override string getSignature()
		{
			return "yeehaw";
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
