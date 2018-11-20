using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	public class FieStateMachineChangelingShoot : FieStateMachineGameCharacterBase
	{
		private enum ShootState
		{
			STATE_PREPARE,
			STATE_SHOOT
		}

		private ShootState _shootState;

		private bool _isEnd;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			gameCharacter.isEnableAutoFlip = false;
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			FieChangeling changeling = gameCharacter as FieChangeling;
			if (!(changeling == null))
			{
				if (changeling.detector.getLockonEnemyTransform() == null)
				{
					_isEnd = true;
				}
				else if (_shootState == ShootState.STATE_PREPARE)
				{
					TrackEntry trackEntry = changeling.animationManager.SetAnimation(12, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						autoFlipToEnemy(changeling);
						trackEntry.Event += delegate(AnimationState state, int trackIndex, Event e)
						{
							if (e.Data.Name == "fire")
							{
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingShot>(changeling.hornTransform, changeling.flipDirectionVector, changeling.detector.getLockonEnemyTransform(isCenter: true), changeling);
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
					_shootState = ShootState.STATE_SHOOT;
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
			return new List<Type>();
		}
	}
}
