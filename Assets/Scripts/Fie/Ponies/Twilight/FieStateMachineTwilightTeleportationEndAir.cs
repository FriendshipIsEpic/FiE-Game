using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightTeleportationEndAir : FieStateMachineGameCharacterBase
	{
		private enum TeleportationState
		{
			TELEPORTATION_START,
			TELEPORTATION_END
		}

		private TeleportationState _teleportationState;

		private bool _isEnd;

		private bool _isFinished;

		private Type _nextState;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieTwilight)
			{
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				switch (_teleportationState)
				{
				case TeleportationState.TELEPORTATION_START:
				{
					TrackEntry trackEntry = fieTwilight.animationManager.SetAnimation(21, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
						{
							if (trackIndex.Data.Name == "finished")
							{
								_nextState = typeof(FieStateMachineTwilightFlying);
								_isFinished = true;
							}
						};
						trackEntry.End += delegate
						{
							_isEnd = true;
						};
					}
					else
					{
						_isEnd = true;
					}
					Vector3 externalInputVector = fieTwilight.externalInputVector;
					fieTwilight.setFlipByVector(externalInputVector);
					_teleportationState = TeleportationState.TELEPORTATION_END;
					break;
				}
				}
				if (_isEnd)
				{
					if (fieTwilight.groundState == FieObjectGroundState.Grounding)
					{
						_nextState = typeof(FieStateMachineCommonIdle);
					}
					else
					{
						_nextState = typeof(FieStateMachineTwilightFlying);
					}
				}
				else if (_isFinished && fieTwilight.externalInputForce > 0.5f && fieTwilight.groundState != 0)
				{
					_nextState = typeof(FieStateMachineTwilightFlying);
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			gameCharacter.isEnableGravity = false;
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			gameCharacter.isEnableGravity = true;
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
			if (_isFinished)
			{
				list.Add(typeof(FieStateMachineTwilightTeleportation));
				list.Add(typeof(FieStateMachineTwilightFlying));
			}
			return list;
		}
	}
}
