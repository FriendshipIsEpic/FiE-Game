using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisPenetrateShot : FieStateMachineGameCharacterBase
	{
		private enum ShootState
		{
			STATE_PREPARE,
			STATE_SHOOT
		}

		private ShootState _shootState;

		private bool _isEnd;

		private FieEmitObjectQueenChrysalisHornEffect _hornEffect;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			gameCharacter.isEnableAutoFlip = false;
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if ((bool)_hornEffect)
			{
				_hornEffect.Kill();
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
			if (!(chrysalis == null))
			{
				if (chrysalis.detector.getLockonEnemyTransform() == null)
				{
					_isEnd = true;
				}
				else if (_shootState == ShootState.STATE_PREPARE)
				{
					TrackEntry trackEntry = chrysalis.animationManager.SetAnimation(29, isLoop: false, isForceSet: true);
					_hornEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHornEffect>(chrysalis.hornTransform, Vector3.zero, null);
					if (trackEntry != null)
					{
						autoFlipToEnemy(chrysalis);
						trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
						{
							if (trackIndex.Data.Name == "fire")
							{
								Transform lockonEnemyTransform = chrysalis.detector.getLockonEnemyTransform(isCenter: true);
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisPenetrateShot>(chrysalis.hornTransform, chrysalis.flipDirectionVector, lockonEnemyTransform, chrysalis);
								Vector3 vector = chrysalis.flipDirectionVector;
								if (lockonEnemyTransform != null)
								{
									vector = lockonEnemyTransform.position - chrysalis.hornTransform.position;
								}
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisPenetrateShotActivateEffect>(chrysalis.hornTransform, vector.normalized, null);
							}
							if (trackIndex.Data.Name == "move")
							{
								Vector3 flipDirectionVector = chrysalis.flipDirectionVector;
								Vector3 moveForce = flipDirectionVector * trackIndex.Float;
								chrysalis.resetMoveForce();
								chrysalis.setMoveForce(moveForce, 0f, useRound: false);
							}
							if (trackIndex.Data.Name == "finished")
							{
								if ((bool)_hornEffect)
								{
									_hornEffect.Kill();
								}
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
			list.Add(typeof(FieStateMachineQueenChrysalisPenetrateShot));
			return list;
		}
	}
}
