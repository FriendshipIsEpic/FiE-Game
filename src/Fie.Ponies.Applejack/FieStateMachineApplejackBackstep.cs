using Fie.Object;
using Spine;
using System;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackBackstep : FieStateMachineGameCharacterBase
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
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableCollider = true;
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
						trackEntry = applejack.animationManager.SetAnimation(36, isLoop: false, isForceSet: true);
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
							{
								if (e.Data.Name == "move")
								{
									Vector3 vector = applejack.flipDirectionVector * e.Float;
									if (applejack.externalInputVector != Vector3.zero)
									{
										vector = applejack.externalInputVector;
										vector.z = vector.y;
										vector.y = 0f;
										vector.Normalize();
										vector *= 0f - e.Float;
										vector.z *= 0.7f;
									}
									applejack.setFlipByVector(-vector);
									applejack.resetMoveForce();
									applejack.setMoveForce(vector, 0f, useRound: false);
									applejack.applySideEffectOfStep();
								}
								if (e.Data.Name == "finished")
								{
									_isEnd = true;
								}
								if (e.Data.Name == "immunity")
								{
									applejack.isEnableCollider = (e.Int < 1);
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
