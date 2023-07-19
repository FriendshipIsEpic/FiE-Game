using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FieAbilityID(FieConstValues.FieAbility.SUMMON_ARROW)]
	public class FieStateMachineTwilightSummonArrow : FieStateMachineAbilityBase
	{
		private const string SUMMON_ARROW_SIGNATURE = "summon_arrow";

		private const float SUMMON_ARROW_DELAY = 0.3f;

		private const float SUMMON_ARROW_DEFAULT_COOLDOWN = 24f;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieTwilight)
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
						fieTwilight.physicalForce.SetPhysicalForce(vector * -1f + Vector3.up * FieRandom.Range(-0.2f, 0.2f), 500f, 0.1f);
					}
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightSummonArrow>(fieTwilight.hornTransform, vector, fieTwilight.detector.getLockonEnemyTransform(isCenter: true), fieTwilight);
					if (fieTwilight.GetSkill(FieConstValues.FieSkill.MAGIC_SUMMON_ARROW_LV4_SUMMON_TRAP) != null)
					{
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightSummonTrapEntity>(fieTwilight.hornTransform, vector, null, fieTwilight);
					}
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightSpellEffect>(fieTwilight.hornTransform, Vector3.zero, null);
					fieTwilight.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_USED_ABILITY));
					float num = 24f;
					GDESkillTreeData skill = fieTwilight.GetSkill(FieConstValues.FieSkill.MAGIC_SUMMON_ARROW_LV4_SUMMON_TRAP);
					if (skill != null)
					{
						num *= skill.Value1;
					}
					fieTwilight.abilitiesContainer.SetCooldown<FieStateMachineTwilightSummonArrow>(24f);
					_isEnd = true;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			defaultCoolDown = 24f;
		}

		public override float getDelay()
		{
			return 0.3f;
		}

		public override string getSignature()
		{
			return "summon_arrow";
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
