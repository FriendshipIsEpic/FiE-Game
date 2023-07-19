using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	public class FieStateMachineRisingSunBaseShot : FieStateMachineRisingSunInterface
	{
		private const float BASE_ATTACK_DEFAULT_SHILED_COST = 0.03f;

		private const float BASE_SHOT_DELAY = 0.2f;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieRisingSun)
			{
				FieRisingSun fieRisingSun = gameCharacter as FieRisingSun;
				if (!FieRisingSun.ignoreAttackState.Contains(fieRisingSun.getStateMachine().nowStateType()))
				{
					Vector3 flipDirectionVector = fieRisingSun.flipDirectionVector;
					FieEmitObjectRisingSunBaseShot fieEmitObjectRisingSunBaseShot = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRisingSunBaseShot>(fieRisingSun.hornTransform, flipDirectionVector, fieRisingSun.detector.getLockonEnemyTransform(isCenter: true), fieRisingSun);
					if (fieEmitObjectRisingSunBaseShot != null)
					{
						fieEmitObjectRisingSunBaseShot.setAdditionalVelocity(fieRisingSun.getNowMoveForce() * 0.05f);
					}
					Type type = fieRisingSun.getStateMachine().nowStateType();
					if ((type == typeof(FieStateMachineCommonIdle) || type == typeof(FieStateMachinePoniesIdle) || type == typeof(FieStateMachineRisingSunFireSmall) || type == typeof(FieStateMachineRisingSunTeleportationEndGround)) && fieRisingSun.groundState == FieObjectGroundState.Grounding)
					{
						fieRisingSun.getStateMachine().setState(typeof(FieStateMachineRisingSunFireSmall), isForceSet: true, isDupulicate: true);
						fieRisingSun.physicalForce.SetPhysicalForce(flipDirectionVector * -1f + Vector3.up * FieRandom.Range(-0.2f, 0.2f), 3000f, 0.1f);
					}
					fieRisingSun.damageSystem.calcShieldDirect((0f - fieRisingSun.healthStats.maxShield) * 0.03f);
					fieRisingSun.damageSystem.setRegenerateDelay(fieRisingSun.healthStats.regenerateDelay * 0.1f, roundToBigger: true);
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_RISING_SUN_BASE_ATTACK), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_RISING_SUN_BASE_ATTACK));
					_isEnd = true;
				}
			}
		}

		public override float getDelay()
		{
			return 0.2f;
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
