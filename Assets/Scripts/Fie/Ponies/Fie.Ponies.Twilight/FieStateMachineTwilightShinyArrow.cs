using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightShinyArrow : FieStateMachineGameCharacterBase
	{
		private const string SHINY_ARROW_SIGNATURE = "shiny_arrow";

		private const float SHINY_ARROW_DELAY = 0.3f;

		private const float SHINY_ARROW_DEFAULT_COOLDOWN = 8f;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieTwilight)
			{
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				if (!FieTwilight.ignoreAttackState.Contains(fieTwilight.getStateMachine().nowStateType()))
				{
					Vector3 vector = (fieTwilight.flipState != 0) ? Vector3.right : Vector3.left;
					if (fieTwilight.getStateMachine().nowStateType() == typeof(FieStateMachineCommonIdle) || fieTwilight.getStateMachine().nowStateType() == typeof(FieStateMachinePoniesIdle) || fieTwilight.getStateMachine().nowStateType() == typeof(FieStateMachineTwilightFireSmall))
					{
						if (fieTwilight.groundState == FieObjectGroundState.Grounding)
						{
							fieTwilight.getStateMachine().setState(typeof(FieStateMachineTwilightFireSmall), isForceSet: true, isDupulicate: true);
						}
						fieTwilight.physicalForce.SetPhysicalForce(vector * -1f + Vector3.up * FieRandom.Range(-0.2f, 0.2f), 5000f, 0.5f);
					}
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightShinyArrow>(fieTwilight.hornTransform, vector, fieTwilight.detector.getLockonEnemyTransform(isCenter: true), fieTwilight);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightSpellEffect>(fieTwilight.hornTransform, Vector3.zero, null);
					fieTwilight.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_USED_ABILITY));
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_MAGIC_ABILITY_1), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_MAGIC_ABILITY_1));
					_isEnd = true;
				}
			}
		}

		public override float getDelay()
		{
			return 0.3f;
		}

		public string getSignature()
		{
			return "shiny_arrow";
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
	}
}
