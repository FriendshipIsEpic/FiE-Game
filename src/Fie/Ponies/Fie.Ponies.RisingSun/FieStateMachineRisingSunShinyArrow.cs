using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	public class FieStateMachineRisingSunShinyArrow : FieStateMachineAbilityBase
	{
		private const string SHINY_ARROW_SIGNATURE = "shiny_arrow";

		private const float SHINY_ARROW_DELAY = 0.3f;

		private const float SHINY_ARROW_DEFAULT_COOLDOWN = 7f;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieRisingSun)
			{
				FieRisingSun fieRisingSun = gameCharacter as FieRisingSun;
				if (!FieRisingSun.ignoreAttackState.Contains(fieRisingSun.getStateMachine().nowStateType()))
				{
					Vector3 vector = (fieRisingSun.flipState != 0) ? Vector3.right : Vector3.left;
					if (fieRisingSun.getStateMachine().nowStateType() == typeof(FieStateMachineCommonIdle) || fieRisingSun.getStateMachine().nowStateType() == typeof(FieStateMachinePoniesIdle) || fieRisingSun.getStateMachine().nowStateType() == typeof(FieStateMachineRisingSunFireSmall))
					{
						if (fieRisingSun.groundState == FieObjectGroundState.Grounding)
						{
							fieRisingSun.getStateMachine().setState(typeof(FieStateMachineRisingSunFireSmall), isForceSet: true, isDupulicate: true);
						}
						fieRisingSun.physicalForce.SetPhysicalForce(vector * -1f + Vector3.up * FieRandom.Range(-0.2f, 0.2f), 5000f, 0.5f);
					}
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRisingSunShinyArrow>(fieRisingSun.hornTransform, vector, fieRisingSun.detector.getLockonEnemyTransform(isCenter: true), fieRisingSun);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRisingSunSpellEffect>(fieRisingSun.hornTransform, Vector3.zero, null);
					fieRisingSun.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_USED_ABILITY));
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_RISING_SUN_ABILITY_1), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_RISING_SUN_ABILITY_1));
					fieRisingSun.abilitiesContainer.SetCooldown<FieStateMachineRisingSunShinyArrow>(7f);
					_isEnd = true;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			defaultCoolDown = 7f;
		}

		public override float getDelay()
		{
			return 0.3f;
		}

		public override string getSignature()
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

		public override FieAbilityActivationType getActivationType()
		{
			return FieAbilityActivationType.COOLDOWN;
		}
	}
}
