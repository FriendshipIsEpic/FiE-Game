using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies.RisingSun
{
	public class FieStateMachineRisingSunTeleportationEndGround : FieStateMachineRisingSunInterface
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
					TrackEntry trackEntry = fieRisingSun.animationManager.SetAnimation(27, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
						{
							if (trackIndex.Data.Name == "finished")
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
					Vector3 externalInputVector = fieRisingSun.externalInputVector;
					fieRisingSun.setFlipByVector(externalInputVector);
					if (fieRisingSun.externalInputForce > 0.1f)
					{
						fieRisingSun.addMoveForce(fieRisingSun.flipDirectionVector.normalized * 10000f, 1f);
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
				list.Add(typeof(FieStateMachineRisingSunTeleportation));
				list.Add(typeof(FieStateMachinePoniesMove));
				list.Add(typeof(FieStateMachineRisingSunFlying));
			}
			return list;
		}
	}
}
