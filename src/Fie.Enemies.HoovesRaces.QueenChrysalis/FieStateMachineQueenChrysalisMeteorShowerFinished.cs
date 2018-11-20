using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisMeteorShowerFinished : FieStateMachineGameCharacterBase
	{
		private enum FinishedState
		{
			METEOR_SHOWER_FINISHED_START,
			METEOR_SHOWER_FINISHED_END
		}

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FinishedState _fireState;

		private bool _isEnd;

		private bool _isFinished;

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis fieQueenChrysalis = gameCharacter as FieQueenChrysalis;
			if (!(fieQueenChrysalis == null))
			{
				switch (_fireState)
				{
				case FinishedState.METEOR_SHOWER_FINISHED_START:
				{
					TrackEntry trackEntry = fieQueenChrysalis.animationManager.SetAnimation(27, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Complete += delegate
						{
							_isEnd = true;
						};
					}
					else
					{
						_isEnd = true;
					}
					_fireState = FinishedState.METEOR_SHOWER_FINISHED_END;
					break;
				}
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.isEnableGravity = true;
				gameCharacter.resetMoveForce();
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.resetMoveForce();
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
