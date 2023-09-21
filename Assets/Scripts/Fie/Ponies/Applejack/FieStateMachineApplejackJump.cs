using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackJump : FieStateMachineGameCharacterBase
	{
		private enum JumpState
		{
			JUMP_TAKEOFF_START,
			JUMP_TAKEOFF_STANDBY,
			JUMP_TAKEOFF_READY,
			JUMP_TAKEOFF
		}

		private const float TAKEOFF_FORCE = 8f;

		private const float TAKEOFF_PAST_TIME = 0.5f;

		private const float FLYING_MOVE_FORCE_ROUND = 0.5f;

		private const float MAX_FLYING_SPEED = 1f;

		private const float FLYING_UPPER_FORCE_DURATION = 3f;

		private const float LANDING_DELAY = 0.05f;

		private const float JUMP_DELAY = 0.1f;

		private JumpState _jumpState;

		private float _takeoffTime;

		private float _flyingTime;

		private float _landingCount;

		private bool _isSetLandingAnimation;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieApplejack)
			{
				FieApplejack applejack = gameCharacter as FieApplejack;
				Vector3 a = applejack.externalInputVector;
				float externalInputForce = applejack.externalInputForce;
				a.y = (a.z = 0f);
				if (_jumpState == JumpState.JUMP_TAKEOFF_START)
				{
					if (applejack.groundState == FieObjectGroundState.Flying)
					{
						_nextState = typeof(FieStateMachineApplejackFlying);
						_isEnd = true;
					}
					else
					{
						TrackEntry trackEntry = applejack.animationManager.SetAnimation(19);
						_jumpState = JumpState.JUMP_TAKEOFF_STANDBY;
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
							{
								if (trackIndex.Data.name == "takeOff")
								{
									Vector3 vector = applejack.externalInputVector;
									vector += Vector3.up;
									vector.x *= 0.1f;
									vector.Normalize();
									vector *= 8f;
									applejack.setMoveForce(vector, 0.5f);
									_jumpState = JumpState.JUMP_TAKEOFF;
								}
								else if (trackIndex.Data.name == "finished")
								{
									_nextState = typeof(FieStateMachineApplejackFlying);
									_isEnd = true;
								}
							};
							trackEntry.Complete += delegate
							{
								_nextState = typeof(FieStateMachineApplejackFlying);
								_isEnd = true;
							};
						}
						a *= 0.5f;
					}
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = true;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.setGravityRate(1f);
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = true;
			}
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return _nextState;
		}

		public override List<Type> getAllowedStateList()
		{
			return new List<Type>();
		}

		public override float getDelay()
		{
			return 0.1f;
		}
	}
}
