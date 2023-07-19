using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	public class FieStateMachineChangelingAlphaShoot : FieStateMachineGameCharacterBase
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
			FieChangelingAlpha changelingAlpha = gameCharacter as FieChangelingAlpha;
			if (!(changelingAlpha == null))
			{
				if (changelingAlpha.detector.getLockonEnemyTransform() == null)
				{
					_isEnd = true;
				}
				else if (_shootState == ShootState.STATE_PREPARE)
				{
					TrackEntry trackEntry = changelingAlpha.animationManager.SetAnimation(8, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						autoFlipToEnemy(changelingAlpha);
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "fire")
							{
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingAlphaShot>(changelingAlpha.hornTransform, changelingAlpha.flipDirectionVector, changelingAlpha.detector.getLockonEnemyTransform(isCenter: true), changelingAlpha);
							}
							if (e.Data.Name == "move")
							{
								Vector3 flipDirectionVector = changelingAlpha.flipDirectionVector;
								Vector3 moveForce = flipDirectionVector * e.Float;
								changelingAlpha.resetMoveForce();
								changelingAlpha.setMoveForce(moveForce, 0f, useRound: false);
							}
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
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineChangelingAlphaConcentration));
			list.Add(typeof(FieStateMachineChangelingAlphaShout));
			return list;
		}
	}
}
