using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;

namespace Fie.Enemies.HoovesRaces.Flightling
{
	public class FieStateMachineFlightlingShoot : FieStateMachineGameCharacterBase
	{
		private enum ShootState
		{
			STATE_PREPARE,
			STATE_SHOOT
		}

		private ShootState _shootState;

		private bool _isEnd;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = false;
				gameCharacter.isEnableGravity = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.isEnableGravity = true;
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			FieFlightling flightling = gameCharacter as FieFlightling;
			if (!(flightling == null))
			{
				if (flightling.detector.getLockonEnemyTransform() == null)
				{
					_isEnd = true;
				}
				else if (_shootState == ShootState.STATE_PREPARE)
				{
					TrackEntry trackEntry = flightling.animationManager.SetAnimationChain(13, 8, isLoop: true);
					if (trackEntry != null)
					{
						autoFlipToEnemy(flightling);
						trackEntry.Event += delegate(AnimationState state, int trackIndex, Event e)
						{
							if (e.Data.Name == "fire")
							{
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectFlightlingShot>(flightling.hornTransform, flightling.flipDirectionVector, flightling.detector.getLockonEnemyTransform(isCenter: true), flightling);
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
					_shootState = ShootState.STATE_SHOOT;
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
