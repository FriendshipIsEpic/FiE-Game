using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisIgniteFailed : FieStateMachineGameCharacterBase
	{
		private enum FailedState
		{
			FAILED_START,
			FAILED_END
		}

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FailedState _fireState;

		private bool _isEnd;

		private bool _isFinished;

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
			if (!(chrysalis == null))
			{
				switch (_fireState)
				{
				case FailedState.FAILED_START:
				{
					TrackEntry trackEntry = chrysalis.animationManager.SetAnimation(20, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Complete += delegate
						{
							chrysalis.animationManager.SetAnimation(0, isLoop: true);
							_nextState = typeof(FieStateMachineCommonIdle);
							_isEnd = true;
						};
						trackEntry.Event += delegate(AnimationState state, int trackIndex, Event e)
						{
							if (e.Data.Name == "finished")
							{
								_nextState = typeof(FieStateMachineCommonIdle);
								_isEnd = true;
							}
						};
					}
					else
					{
						_isEnd = true;
					}
					_fireState = FailedState.FAILED_END;
					break;
				}
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
			}
		}

		public override List<Type> getAllowedStateList()
		{
			return new List<Type>();
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
