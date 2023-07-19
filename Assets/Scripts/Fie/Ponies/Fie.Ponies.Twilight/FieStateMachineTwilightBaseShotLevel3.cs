using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightBaseShotLevel3 : FieStateMachineGameCharacterBase
	{
		private const float BASE_ATTACK_DEFAULT_SHILED_COST = 0.05f;

		private const float BASE_SHOT_DELAY = 0.2f;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieTwilight)
			{
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				if (fieTwilight.getStateMachine().nowStateType() == typeof(FieStateMachineCommonIdle) || fieTwilight.getStateMachine().nowStateType() == typeof(FieStateMachinePoniesIdle) || fieTwilight.getStateMachine().nowStateType() == typeof(FieStateMachineTwilightFireSmall))
				{
					if (fieTwilight.groundState == FieObjectGroundState.Grounding)
					{
						fieTwilight.getStateMachine().setState(typeof(FieStateMachineTwilightFireSmall), isForceSet: true, isDupulicate: true);
					}
					fieTwilight.physicalForce.SetPhysicalForce(fieTwilight.flipDirectionVector * -1f + Vector3.up * FieRandom.Range(-0.2f, 0.2f), 5000f, 0.5f);
				}
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightShinyArrow>(fieTwilight.hornTransform, fieTwilight.flipDirectionVector, fieTwilight.detector.getLockonEnemyTransform(isCenter: true), fieTwilight);
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightSpellEffect>(fieTwilight.hornTransform, Vector3.zero, null);
				fieTwilight.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_USED_ABILITY));
				FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_MAGIC_ABILITY_1), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_MAGIC_ABILITY_1));
				_isEnd = true;
			}
		}

		public override float getDelay()
		{
			return 0.2f;
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return typeof(FieStateMachineTwilightAttackIdle);
		}
	}
}
