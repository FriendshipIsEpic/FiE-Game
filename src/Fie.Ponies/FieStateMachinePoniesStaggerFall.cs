using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesStaggerFall : FieStateMachineGameCharacterBase
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
				switch (_staggerState)
				{
				case StaggerState.STATE_PREPARE:
				{
					_allowedStateList = fiePonies.getStaggerCancelableStateList();
					fiePonies.emotionController.SetEmoteAnimation(17, isForceSet: true);
					int animationId = 8;
					TrackEntry trackEntry = fiePonies.animationManager.SetAnimation(animationId, isLoop: false, isForceSet: true);
					trackEntry.mixDuration = 0f;
					_staggerState = StaggerState.STATE_STAGGER;
					break;
				}
				case StaggerState.STATE_STAGGER:
					if (fiePonies.groundState == FieObjectGroundState.Grounding)
					{
						if (fiePonies.damageSystem.isDying)
						{
							_nextState = typeof(FieStateMachinePoniesDead);
							_isEnd = true;
						}
						else
						{
							_nextState = typeof(FieStateMachinePoniesStaggerFallRecover);
							_isEnd = true;
							fiePonies.emotionController.RestoreEmotionFromDefaultData();
							_staggerState = StaggerState.STATE_STAGGER_END;
						}
					}
					break;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.isEnableHeadTracking = false;
				fiePonies.isEnableGravity = true;
				fiePonies.setGravityRate(0.5f);
				fiePonies.isEnableAutoFlip = false;
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
