using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	public class FieStateMachineRisingSunFireSmall : FieStateMachineRisingSunInterface
	{
		private enum FireState
		{
			FIRE_START,
			FIRING
		}

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private float _timeCount;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieRisingSun)
			{
				_timeCount += Time.deltaTime;
				FieRisingSun fieRisingSun = gameCharacter as FieRisingSun;
				switch (_fireState)
				{
				case FireState.FIRE_START:
					if (fieRisingSun.groundState == FieObjectGroundState.Grounding)
					{
						int animationId = 23;
						TrackEntry trackEntry = fieRisingSun.animationManager.SetAnimation(animationId, isLoop: false, isForceSet: true);
						if (trackEntry != null)
						{
							_endTime = trackEntry.endTime;
						}
					}
					_fireState = FireState.FIRING;
					break;
				}
				if (_timeCount > _endTime)
				{
					_isEnd = true;
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
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineRisingSunFlying));
			list.Add(typeof(FieStateMachinePoniesMove));
			list.Add(typeof(FieStateMachineRisingSunTeleportation));
			return list;
		}
	}
}
