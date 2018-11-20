using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	public class FieStateMachineRisingSunEmission : FieStateMachineAbilityBase
	{
		private const string FORCE_FIELD_SIGNATURE = "magic_bubble";

		private const float FORCE_FIELD_DELAY = 0.3f;

		private const float FORCE_FIELD_DEFAULT_COOLDOWN = 3f;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieRisingSun)
			{
				FieRisingSun fieRisingSun = gameCharacter as FieRisingSun;
				if (!FieRisingSun.ignoreAttackState.Contains(fieRisingSun.getStateMachine().nowStateType()))
				{
					Vector3 a = (fieRisingSun.flipState != 0) ? Vector3.right : Vector3.left;
					if (fieRisingSun.getStateMachine().nowStateType() == typeof(FieStateMachineCommonIdle) || fieRisingSun.getStateMachine().nowStateType() == typeof(FieStateMachinePoniesIdle) || fieRisingSun.getStateMachine().nowStateType() == typeof(FieStateMachineRisingSunFireSmall))
					{
						if (fieRisingSun.groundState == FieObjectGroundState.Grounding)
						{
							fieRisingSun.getStateMachine().setState(typeof(FieStateMachineRisingSunFireSmall), isForceSet: true, isDupulicate: true);
						}
						fieRisingSun.physicalForce.SetPhysicalForce(a * -1f + Vector3.up * FieRandom.Range(-0.2f, 0.2f), 5000f, 0.1f);
					}
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRisingSunEmission>(fieRisingSun.hornTransform, Vector3.up, null, fieRisingSun);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRisingSunSpellEffect>(fieRisingSun.hornTransform, Vector3.zero, null);
					fieRisingSun.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_USED_ABILITY));
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_RISING_SUN_ABILITY_3), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_RISING_SUN_ABILITY_3));
					fieRisingSun.abilitiesContainer.SetCooldown<FieStateMachineRisingSunEmission>(3f);
					_isEnd = true;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			defaultCoolDown = 3f;
		}

		public override float getDelay()
		{
			return 0.3f;
		}

		public override string getSignature()
		{
			return "magic_bubble";
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
