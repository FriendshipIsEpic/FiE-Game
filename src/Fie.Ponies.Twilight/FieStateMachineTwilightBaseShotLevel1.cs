using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightBaseShotLevel1 : FieStateMachineGameCharacterBase
	{
		private const float BASE_ATTACK_DEFAULT_SHILED_COST = 0.015f;

		private const float BASE_SHOT_DELAY = 0.3f;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd)
			{
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				if (!(fieTwilight == null))
				{
					FieEmitObjectTwilightBaseShot fieEmitObjectTwilightBaseShot = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightBaseShot>(fieTwilight.hornTransform, fieTwilight.flipDirectionVector, fieTwilight.detector.getLockonEnemyTransform(isCenter: true), fieTwilight);
					if (fieEmitObjectTwilightBaseShot != null)
					{
						fieEmitObjectTwilightBaseShot.setAdditionalVelocity(fieTwilight.getNowMoveForce() * 0.05f);
					}
					Type type = fieTwilight.getStateMachine().nowStateType();
					if ((type == typeof(FieStateMachineCommonIdle) || type == typeof(FieStateMachinePoniesIdle) || type == typeof(FieStateMachineTwilightFireSmall) || type == typeof(FieStateMachineTwilightTeleportationEndGround)) && fieTwilight.groundState == FieObjectGroundState.Grounding)
					{
						fieTwilight.getStateMachine().setState(typeof(FieStateMachineTwilightFireSmall), isForceSet: true, isDupulicate: true);
						fieTwilight.physicalForce.SetPhysicalForce(fieTwilight.flipDirectionVector * -1f + Vector3.up * FieRandom.Range(-0.2f, 0.2f), 3000f, 0.1f);
					}
					fieTwilight.damageSystem.calcShieldDirect((0f - fieTwilight.healthStats.maxShield) * (0.015f * fieTwilight.baseAttackConsumeShieldRate));
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_MAGIC_BASE_ATTACK), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_MAGIC_BASE_ATTACK));
					_isEnd = true;
				}
			}
		}

		public override float getDelay()
		{
			return 0.3f;
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
