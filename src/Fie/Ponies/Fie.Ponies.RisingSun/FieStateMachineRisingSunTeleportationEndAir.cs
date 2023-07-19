using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	public class FieStateMachineRisingSunTeleportationEndAir : FieStateMachineRisingSunInterface
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
			if (gameCharacter is FieRisingSun)
			{
				FieRisingSun fieRisingSun = gameCharacter as FieRisingSun;
				switch (_teleportationState)
				{
				case TeleportationState.TELEPORTATION_START:
				{
					TrackEntry trackEntry = fieRisingSun.animationManager.SetAnimation(21, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "finished")
							{
								_nextState = typeof(FieStateMachineRisingSunFlying);
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
					Vector3 externalInputVector = fieRisingSun.externalInputVector;
					fieRisingSun.setFlipByVector(externalInputVector);
					_teleportationState = TeleportationState.TELEPORTATION_END;
					break;
				}
				}
				if (_isEnd)
				{
					if (fieRisingSun.groundState == FieObjectGroundState.Grounding)
					{
						_nextState = typeof(FieStateMachineCommonIdle);
					}
					else
					{
						_nextState = typeof(FieStateMachineRisingSunFlying);
					}
				}
				else if (_isFinished && fieRisingSun.externalInputForce > 0.5f && fieRisingSun.groundState != 0)
				{
					_nextState = typeof(FieStateMachineRisingSunFlying);
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
				list.Add(typeof(FieStateMachineRisingSunTeleportation));
				list.Add(typeof(FieStateMachineRisingSunFlying));
			}
			return list;
		}
	}
}
