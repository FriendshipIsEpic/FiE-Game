using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;

namespace Fie.Enemies.HoovesRaces
{
	public class FieStateMachineEnemiesHoovesRacesStaggerFall : FieStateMachineGameCharacterBase
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
			if (gameCharacter is FieEnemiesHoovesRaces)
			{
				FieEnemiesHoovesRaces fieEnemiesHoovesRaces = gameCharacter as FieEnemiesHoovesRaces;
				switch (_staggerState)
				{
				case StaggerState.STATE_PREPARE:
				{
					int animationId = 5;
					TrackEntry trackEntry2 = fieEnemiesHoovesRaces.animationManager.SetAnimation(animationId, isLoop: false, isForceSet: true);
					trackEntry2.mixDuration = 0f;
					_staggerState = StaggerState.STATE_STAGGER;
					break;
				}
				case StaggerState.STATE_STAGGER:
					if (fieEnemiesHoovesRaces.groundState == FieObjectGroundState.Grounding)
					{
						TrackEntry trackEntry = fieEnemiesHoovesRaces.animationManager.SetAnimation(6, isLoop: false, isForceSet: true);
						trackEntry.Event += delegate(AnimationState state, int trackIndex, Event e)
						{
							if (e.Data.Name == "finished")
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
					break;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieEnemiesHoovesRaces fieEnemiesHoovesRaces = gameCharacter as FieEnemiesHoovesRaces;
			if (!(fieEnemiesHoovesRaces == null))
			{
				fieEnemiesHoovesRaces.isEnableGravity = true;
				fieEnemiesHoovesRaces.setGravityRate(0.5f);
				fieEnemiesHoovesRaces.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieEnemiesHoovesRaces fieEnemiesHoovesRaces = gameCharacter as FieEnemiesHoovesRaces;
			if (!(fieEnemiesHoovesRaces == null))
			{
				fieEnemiesHoovesRaces.setGravityRate(1f);
				fieEnemiesHoovesRaces.isEnableAutoFlip = true;
				fieEnemiesHoovesRaces.isEnableGravity = true;
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
