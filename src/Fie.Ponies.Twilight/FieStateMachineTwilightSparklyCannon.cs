using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using System;
using System.Collections.Generic;

namespace Fie.Ponies.Twilight
{
	[FieAbilityID(FieConstValues.FieAbility.SPARKLY_CANNON)]
	public class FieStateMachineTwilightSparklyCannon : FieStateMachineAbilityBase
	{
		private const string SPARKLY_CANNON_SIGNATURE = "sparkly_cannon";

		public const float SPARKLY_CANNON_DEFAULT_COOLDOWN = 15f;

		private Type _nextState = typeof(FieStateMachinePoniesAttackIdle);

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieTwilight)
			{
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_LOYALTY_ABILITY_1), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_LOYALTY_ABILITY_1));
				fieTwilight.getStateMachine().setState(typeof(FieStateMachineTwilightSparklyCannonShooting), isForceSet: false);
				_isEnd = true;
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			defaultCoolDown = 15f;
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
			return "sparkly_cannon";
		}

		public override FieAbilityActivationType getActivationType()
		{
			return FieAbilityActivationType.COOLDOWN;
		}

		public override bool isNotNetworkSync()
		{
			return true;
		}

		public override List<Type> getAllowedStateList()
		{
			return new List<Type>();
		}
	}
}
