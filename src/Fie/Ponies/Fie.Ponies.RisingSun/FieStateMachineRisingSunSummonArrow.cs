using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	public class FieStateMachineRisingSunSummonArrow : FieStateMachineAbilityBase
	{
		private const string SUMMON_ARROW_SIGNATURE = "summon_arrow";

		private const float SUMMON_ARROW_DELAY = 0.3f;

		private const float SUMMON_ARROW_DEFAULT_COOLDOWN = 6f;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieRisingSun)
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
						fieRisingSun.physicalForce.SetPhysicalForce(vector * -1f + Vector3.up * FieRandom.Range(-0.2f, 0.2f), 500f, 0.1f);
					}
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRisingSunSummonArrow>(fieRisingSun.hornTransform, vector, fieRisingSun.detector.getLockonEnemyTransform(isCenter: true), fieRisingSun);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRisingSunSpellEffect>(fieRisingSun.hornTransform, Vector3.zero, null);
					fieRisingSun.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_USED_ABILITY));
					fieRisingSun.abilitiesContainer.SetCooldown<FieStateMachineRisingSunSummonArrow>(6f);
					_isEnd = true;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			defaultCoolDown = 6f;
		}

		public override float getDelay()
		{
			return 0.3f;
		}

		public override string getSignature()
		{
			return "summon_arrow";
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
			return null;
		}

		public override FieAbilityActivationType getActivationType()
		{
			return FieAbilityActivationType.COOLDOWN;
		}
	}
}
