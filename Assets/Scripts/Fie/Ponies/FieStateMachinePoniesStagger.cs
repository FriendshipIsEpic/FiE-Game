using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesStagger : FieStateMachineGameCharacterBase
	{
		private enum StaggerState
		{
			STATE_PREPARE,
			STATE_STAGGER
		}

		private const float STAGGER_MOVE_FORCE = 6f;

		private StaggerState _staggerState;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FiePonies)
			{
				FiePonies fiePonies = gameCharacter as FiePonies;
				if (_staggerState == StaggerState.STATE_PREPARE)
				{
					fiePonies.emotionController.SetEmoteAnimation(17, isForceSet: true);
					int animationId = 6;
					if (fiePonies.groundState == FieObjectGroundState.Flying)
					{
						_nextState = typeof(FieStateMachinePoniesStaggerFall);
						fiePonies.isEnableGravity = false;
						fiePonies.isEnableGravity = true;
						gameCharacter.setGravityRate(0.2f);
						Vector3 a = Vector3.Normalize(fiePonies.centerTransform.position - fiePonies.latestDamageWorldPoint);
						a.y *= 0.5f;
						a.z = 0f;
						a.Normalize();
						fiePonies.setFlipByVector(a * -1f);
						fiePonies.setMoveForce((Vector3.up + a * 0.5f).normalized * 6f, 1f, useRound: false);
						animationId = 7;
					}
					TrackEntry trackEntry = fiePonies.animationManager.SetAnimation(animationId, isLoop: false, isForceSet: true);
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
					_staggerState = StaggerState.STATE_STAGGER;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.isEnableAutoFlip = false;
				fiePonies.isEnableHeadTracking = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.emotionController.RestoreEmotionFromDefaultData();
				fiePonies.setGravityRate(1f);
				fiePonies.isEnableHeadTracking = true;
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
