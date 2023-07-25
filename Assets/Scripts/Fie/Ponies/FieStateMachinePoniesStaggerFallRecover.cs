using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesStaggerFallRecover : FieStateMachineGameCharacterBase
	{
		private enum StaggerState
		{
			STATE_PREPARE,
			STATE_STAGGER,
			STATE_STAGGER_END
		}

		private StaggerState _staggerState;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private List<Type> _allowedStateList = new List<Type>();

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FiePonies)
			{
				FiePonies fiePonies = gameCharacter as FiePonies;
				if (_staggerState == StaggerState.STATE_PREPARE)
				{
					fiePonies.emotionController.RestoreEmotionFromDefaultData();
					TrackEntry trackEntry = fiePonies.animationManager.SetAnimation(9, isLoop: false, isForceSet: true);
					trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
					{
						if (trackIndex.Data.Name == "finished")
						{
							_isEnd = true;
						}
					};
					trackEntry.Complete += delegate
					{
						_isEnd = true;
					};
					_staggerState = StaggerState.STATE_STAGGER_END;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FiePonies x = gameCharacter as FiePonies;
			if (!(x == null))
			{
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.emotionController.RestoreEmotionFromDefaultData();
				fiePonies.isEnableHeadTracking = true;
				fiePonies.setGravityRate(1f);
				fiePonies.isEnableAutoFlip = true;
				fiePonies.isEnableGravity = true;
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
			return _allowedStateList;
		}
	}
}
