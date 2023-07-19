using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	public class FieStateMachineChangelingBackstep : FieStateMachineGameCharacterBase
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
			FieChangeling fieChangeling = gameCharacter as FieChangeling;
			if (!(fieChangeling == null))
			{
				autoFlipToEnemy(fieChangeling);
				fieChangeling.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.isEnableCollider = true;
				gameCharacter.damageSystem.isEnableStaggerImmunity = false;
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieChangeling)
			{
				FieChangeling changeling = gameCharacter as FieChangeling;
				if (_stepState == StepState.STEP_START)
				{
					if (changeling.groundState == FieObjectGroundState.Grounding)
					{
						TrackEntry trackEntry = changeling.animationManager.SetAnimation(changeling.getBackStepAnimationID(), isLoop: false, isForceSet: true);
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
							{
								if (e.Data.Name == "move")
								{
									Vector3 moveForce = changeling.flipDirectionVector * e.Float;
									changeling.setMoveForce(moveForce, 0f, useRound: false);
								}
								if (e.Data.Name == "finished")
								{
									_isEnd = true;
								}
								if (e.Data.Name == "immunity")
								{
									changeling.isEnableCollider = (e.Int < 1);
									changeling.damageSystem.isEnableStaggerImmunity = (e.Int >= 1);
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

		public override List<Type> getAllowedStateList()
		{
			return new List<Type>();
		}
	}
}
