using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using System;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightBaseShotLevel2 : FieStateMachineGameCharacterBase
	{
		private const float BASE_ATTACK_DEFAULT_SHILED_COST = 0.05f;

		private const float BASE_SHOT_DELAY = 0.2f;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd)
			{
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				if (!(fieTwilight == null))
				{
					FieEmitObjectTwilightTwinkleArrow fieEmitObjectTwilightTwinkleArrow = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightTwinkleArrow>(fieTwilight.hornTransform, fieTwilight.flipDirectionVector, fieTwilight.detector.getLockonEnemyTransform(isCenter: true), fieTwilight);
					if (fieEmitObjectTwilightTwinkleArrow != null)
					{
						fieEmitObjectTwilightTwinkleArrow.setAdditionalVelocity(fieTwilight.getNowMoveForce() * 0.05f);
					}
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightSpellEffect>(fieTwilight.hornTransform, Vector3.zero, null);
					Type type = fieTwilight.getStateMachine().nowStateType();
					if ((type == typeof(FieStateMachineCommonIdle) || type == typeof(FieStateMachinePoniesIdle) || type == typeof(FieStateMachineTwilightFireSmall) || type == typeof(FieStateMachineTwilightTeleportationEndGround)) && fieTwilight.groundState == FieObjectGroundState.Grounding)
					{
						fieTwilight.getStateMachine().setState(typeof(FieStateMachineTwilightFireSmall), isForceSet: true, isDupulicate: true);
						fieTwilight.physicalForce.SetPhysicalForce(fieTwilight.flipDirectionVector * -1f + Vector3.up * FieRandom.Range(-0.2f, 0.2f), 3000f, 0.1f);
					}
					_isEnd = true;
				}
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
