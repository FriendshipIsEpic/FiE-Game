using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisRift : FieStateMachineGameCharacterBase
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

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieEnemiesHoovesRaces)
			{
				FieEnemiesHoovesRaces fieEnemiesHoovesRaces = gameCharacter as FieEnemiesHoovesRaces;
				switch (_staggerState)
				{
				case StaggerState.STATE_PREPARE:
					_staggerState = StaggerState.STATE_STAGGER;
					break;
				case StaggerState.STATE_STAGGER:
				{
					int num = 3;
					_nextState = typeof(FieStateMachineQueenChrysalisStaggerFall);
					fieEnemiesHoovesRaces.isEnableGravity = false;
					fieEnemiesHoovesRaces.isEnableGravity = true;
					gameCharacter.setGravityRate(0.15f);
					if (_resetMoveForce)
					{
						fieEnemiesHoovesRaces.resetMoveForce();
					}
					fieEnemiesHoovesRaces.setMoveForce(Vector3.up * 8f * _riftForceRate, 1f, useRound: false);
					num = 4;
					TrackEntry trackEntry = fieEnemiesHoovesRaces.animationManager.SetAnimation(num, isLoop: false, isForceSet: true);
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
			FieEnemiesHoovesRaces fieEnemiesHoovesRaces = gameCharacter as FieEnemiesHoovesRaces;
			if (!(fieEnemiesHoovesRaces == null))
			{
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

		public void SetRiftForceRate(float forceRate)
		{
			_riftForceRate = forceRate;
		}

		public void ResetMoveForce(bool resetMoveForce)
		{
			_resetMoveForce = resetMoveForce;
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
			list.Add(typeof(FieStateMachineEnemiesQueenChrysalisStagger));
			return list;
		}
	}
}
