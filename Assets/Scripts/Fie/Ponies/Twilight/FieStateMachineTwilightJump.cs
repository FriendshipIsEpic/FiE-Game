using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightJump : FieStateMachineGameCharacterBase
	{
		private enum JumpState
		{
			JUMP_TAKEOFF_START,
			JUMP_TAKEOFF_STANDBY,
			JUMP_TAKEOFF
		}

		private const float TAKEOFF_FORCE = 6f;

		private const float TAKEOFF_PAST_TIME = 0.5f;

		private JumpState _jumpState;

		private float _takeoffTime;

		private float _flyingTime;

		private float _landingCount;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieTwilight)
			{
				FieTwilight twilight = gameCharacter as FieTwilight;
				if (_jumpState == JumpState.JUMP_TAKEOFF_START)
				{
					if (twilight.groundState == FieObjectGroundState.Flying)
					{
						_nextState = typeof(FieStateMachineTwilightFlying);
						_isEnd = true;
						return;
					}
					TrackEntry trackEntry = twilight.animationManager.SetAnimation(19);
					_jumpState = JumpState.JUMP_TAKEOFF_STANDBY;
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
						{
							if (trackIndex.Data.name == "takeOff")
							{
								Vector3 vector = twilight.externalInputVector;
								vector += Vector3.up;
								vector.x *= 0.1f;
								vector.Normalize();
								vector *= 6f;
								twilight.setMoveForce(vector, 0.5f);
								_jumpState = JumpState.JUMP_TAKEOFF;
							}
							else if (trackIndex.Data.name == "finished")
							{
								_nextState = typeof(FieStateMachineTwilightFlying);
								_isEnd = true;
							}
						};
						trackEntry.Complete += delegate
						{
							_nextState = typeof(FieStateMachineTwilightFlying);
							_isEnd = true;
						};
					}
				}
				twilight.physicalForce.SetPhysicalForce(gameCharacter.getNowMoveForce() * -1f, 1000f);
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.setGravityRate(0.5f);
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
				gameCharacter.setGravityRate(1f);
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
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachinePoniesEvasion));
			list.Add(typeof(FieStateMachineTwilightTeleportation));
			return list;
		}
	}
}