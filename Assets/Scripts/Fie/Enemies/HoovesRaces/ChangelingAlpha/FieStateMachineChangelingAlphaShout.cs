using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	public class FieStateMachineChangelingAlphaShout : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			SHOUT_START,
			SHOUT_END
		}

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private float _timeCount;

		private bool _isFinished;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieChangelingAlpha)
			{
				_timeCount += Time.deltaTime;
				FieChangelingAlpha changelingAlpha = gameCharacter as FieChangelingAlpha;
				switch (_fireState)
				{
				case FireState.SHOUT_START:
				{
					TrackEntry trackEntry = changelingAlpha.animationManager.SetAnimation(12, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Complete += delegate
						{
							changelingAlpha.animationManager.SetAnimation(0, isLoop: true);
							_nextState = typeof(FieStateMachineCommonIdle);
							_isEnd = true;
						};
						trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
						{
							if (trackIndex.Data.Name == "finished")
							{
								_nextState = typeof(FieStateMachineCommonIdle);
								_isEnd = true;
							}
						};
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingAlphaShout>(changelingAlpha.mouthTransform, Vector3.zero, null, changelingAlpha);
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingAlphaReflectionShout>(changelingAlpha.mouthTransform, Vector3.zero, null, changelingAlpha);
						FieManagerBehaviour<FieGameCameraManager>.I.setWiggler(1f, 10, Vector3.one * 0.2f);
					}
					else
					{
						_isEnd = true;
					}
					_fireState = FireState.SHOUT_END;
					break;
				}
				}
				if (_timeCount > _endTime)
				{
					_isEnd = true;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieChangelingAlpha fieChangelingAlpha = gameCharacter as FieChangelingAlpha;
			if (!(fieChangelingAlpha == null))
			{
				autoFlipToEnemy(fieChangelingAlpha);
				fieChangelingAlpha.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieChangelingAlpha fieChangelingAlpha = gameCharacter as FieChangelingAlpha;
			if (!(fieChangelingAlpha == null))
			{
				fieChangelingAlpha.isEnableAutoFlip = true;
			}
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineChangelingAlphaConcentration));
			list.Add(typeof(FieStateMachineChangelingAlphaCharge));
			list.Add(typeof(FieStateMachineChangelingAlphaZeroDistanceCharge));
			return list;
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return _nextState;
		}

		public override bool isFinished()
		{
			return _isFinished;
		}
	}
}
