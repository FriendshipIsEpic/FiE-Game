using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightTeleportationEndGround : FieStateMachineGameCharacterBase
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
					TrackEntry trackEntry = fieTwilight.animationManager.SetAnimation(27, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "finished")
							{
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
					_teleportationState = TeleportationState.TELEPORTATION_END;
					Vector3 externalInputVector = fieTwilight.externalInputVector;
					fieTwilight.setFlipByVector(externalInputVector);
					if (fieTwilight.externalInputForce > 0.1f)
					{
						fieTwilight.addMoveForce(fieTwilight.flipDirectionVector.normalized * 10000f, 1f);
					}
					break;
				}
				}
				if (_isEnd)
				{
					_nextState = typeof(FieStateMachineCommonIdle);
				}
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
			if (_isFinished)
			{
				list.Add(typeof(FieStateMachineTwilightTeleportation));
				list.Add(typeof(FieStateMachinePoniesMove));
				list.Add(typeof(FieStateMachineTwilightFlying));
			}
			return list;
		}
	}
}
