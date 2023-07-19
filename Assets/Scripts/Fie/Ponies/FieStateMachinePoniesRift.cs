using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesRift : FieStateMachineGameCharacterBase
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

		private float _riftForceRate = 1f;

		private bool _resetMoveForce;

		public void SetRiftForceRate(float forceRate)
		{
			_riftForceRate = forceRate;
		}

		public void ResetMoveForce(bool resetMoveForce)
		{
			_resetMoveForce = resetMoveForce;
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FiePonies)
			{
				FiePonies fiePonies = gameCharacter as FiePonies;
				switch (_staggerState)
				{
				case StaggerState.STATE_PREPARE:
					_staggerState = StaggerState.STATE_STAGGER;
					break;
				case StaggerState.STATE_STAGGER:
				{
					int num = 6;
					_nextState = typeof(FieStateMachinePoniesStaggerFall);
					fiePonies.isEnableGravity = false;
					fiePonies.isEnableGravity = true;
					gameCharacter.setGravityRate(0.2f);
					if (_resetMoveForce)
					{
						fiePonies.resetMoveForce();
					}
					fiePonies.setMoveForce(Vector3.up * 8f * _riftForceRate, 1f, useRound: false);
					num = 7;
					TrackEntry trackEntry = fiePonies.animationManager.SetAnimation(num, isLoop: false, isForceSet: true);
					trackEntry.mixDuration = 0f;
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
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
					}
					else
					{
						_isEnd = true;
					}
					_staggerState = StaggerState.STATE_STAGGER_END;
					break;
				}
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
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
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachinePoniesStagger));
			return list;
		}
	}
}