using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackCharge : FieStateMachineGameCharacterBase
	{
		private enum StepState
		{
			STEP_START,
			STEPPING
		}

		private StepState _stepState;

		private bool _isEnd;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieApplejack fieApplejack = gameCharacter as FieApplejack;
			if (!(fieApplejack == null))
			{
				fieApplejack.isEnableAutoFlip = false;
				fieApplejack.isEnableHeadTracking = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieApplejack fieApplejack = gameCharacter as FieApplejack;
			if (!(fieApplejack == null))
			{
				fieApplejack.isEnableCollider = true;
				fieApplejack.isEnableHeadTracking = true;
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieApplejack)
			{
				FieApplejack applejack = gameCharacter as FieApplejack;
				if (_stepState == StepState.STEP_START)
				{
					if (applejack.groundState == FieObjectGroundState.Grounding)
					{
						TrackEntry trackEntry = null;
						trackEntry = applejack.animationManager.SetAnimation(40, isLoop: false, isForceSet: true);
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
							{
								if (trackIndex.Data.Name == "move")
								{
									Vector3 vector = applejack.flipDirectionVector * trackIndex.Float;
									if (applejack.externalInputVector != Vector3.zero)
									{
										vector = applejack.externalInputVector;
										vector.z = vector.y;
										vector.y = 0f;
										vector.Normalize();
										vector *= trackIndex.Float;
										vector.z *= 0.7f;
									}
									applejack.setFlipByVector(vector);
									applejack.resetMoveForce();
									applejack.setMoveForce(vector, 0f, useRound: false);
									applejack.applySideEffectOfStep();
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackCharge>(applejack.centerTransform, applejack.flipDirectionVector, null, applejack);
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackChargeEffect>(applejack.leftFrontHoofTransform, applejack.flipDirectionVector);
								}
								if (trackIndex.Data.Name == "finished")
								{
									_isEnd = true;
								}
								if (trackIndex.Data.Name == "immunity")
								{
									applejack.isEnableCollider = (trackIndex.Int < 1);
								}
							};
							trackEntry.Complete += delegate
							{
								_isEnd = true;
							};
						}
						else
						{
							_isEnd = true;
						}
					}
					else
					{
						_isEnd = true;
					}
					_stepState = StepState.STEPPING;
				}
			}
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return typeof(FieStateMachineCommonIdle);
		}
	}
}
