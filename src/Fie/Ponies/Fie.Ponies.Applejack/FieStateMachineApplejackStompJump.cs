using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackStompJump : FieStateMachineGameCharacterBase
	{
		private enum JumpState
		{
			JUMP_TAKEOFF_START,
			JUMP_TAKEOFF_STANDBY,
			JUMP_TAKEOFF_READY,
			JUMP_TAKEOFF,
			JUMP_FLYING,
			JUMP_LANDING
		}

		private const float TAKEOFF_FORCE = 6f;

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

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieApplejack)
			{
				FieApplejack fieApplejack = gameCharacter as FieApplejack;
				Vector3 a = fieApplejack.externalInputVector;
				float externalInputForce = fieApplejack.externalInputForce;
				a.y = (a.z = 0f);
				switch (_jumpState)
				{
				case JumpState.JUMP_TAKEOFF_START:
				{
					if (fieApplejack.groundState == FieObjectGroundState.Flying)
					{
						_isEnd = true;
						return;
					}
					TrackEntry trackEntry = fieApplejack.animationManager.SetAnimation(19, isLoop: false, isForceSet: true);
					_jumpState = JumpState.JUMP_TAKEOFF_STANDBY;
					if (trackEntry == null)
					{
						_isEnd = true;
						return;
					}
					trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
					{
						if (e.Data.name == "takeOff")
						{
							_jumpState = JumpState.JUMP_TAKEOFF_READY;
						}
						if (e.Data.name == "finished")
						{
							_isEnd = true;
						}
					};
					trackEntry.Complete += delegate
					{
						_isEnd = true;
					};
					a *= 0.5f;
					break;
				}
				case JumpState.JUMP_TAKEOFF_READY:
				{
					Vector3 vector = fieApplejack.externalInputVector;
					vector += Vector3.up;
					vector.x *= 0.1f;
					vector.Normalize();
					vector *= 6f;
					fieApplejack.setMoveForce(vector, 0.5f);
					_jumpState = JumpState.JUMP_TAKEOFF;
					a *= 0.5f;
					break;
				}
				}
				gameCharacter.addMoveForce(a * (fieApplejack.getDefaultMoveSpeed() * fieApplejack.externalInputForce) * 0.5f, 0.4f);
				fieApplejack.physicalForce.SetPhysicalForce(gameCharacter.getNowMoveForce() * -1f, 500f);
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
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
			return typeof(FieStateMachineApplejackStompAction);
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
