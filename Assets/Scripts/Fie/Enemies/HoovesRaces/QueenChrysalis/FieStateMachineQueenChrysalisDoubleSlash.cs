using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisDoubleSlash : FieStateMachineGameCharacterBase
	{
		private enum MeleeState
		{
			MELEE_START,
			MELEE
		}

		private const float MELEE_HORMING_DISTANCE = 1f;

		private const float MELEE_HORMING_DEFAULT_RATE = 0.5f;

		private MeleeState _meleeState;

		private bool _isEnd;

		private bool _isMidAirAttack;

		private int _attackCount;

		private FieEmitObjectQueenChrysalisHornEffect _hornEffect;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieQueenChrysalis fieQueenChrysalis = gameCharacter as FieQueenChrysalis;
			if (!(fieQueenChrysalis == null))
			{
				autoFlipToEnemy(fieQueenChrysalis);
				fieQueenChrysalis.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.isEnableGravity = true;
				if ((bool)_hornEffect)
				{
					_hornEffect.Kill();
				}
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieQueenChrysalis)
			{
				FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
				if (_meleeState == MeleeState.MELEE_START)
				{
					if (chrysalis.detector.lockonTargetObject != null)
					{
						Vector3 position = chrysalis.transform.position;
						float y = position.y;
						Vector3 position2 = chrysalis.detector.lockonTargetObject.position;
						float y2 = position2.y;
						_isMidAirAttack = (y2 > y + 1f);
						_isMidAirAttack |= (chrysalis.groundState == FieObjectGroundState.Flying);
					}
					TrackEntry trackEntry = null;
					if (!_isMidAirAttack)
					{
						trackEntry = chrysalis.animationManager.SetAnimation(17, isLoop: false, isForceSet: true);
					}
					else
					{
						trackEntry = chrysalis.animationManager.SetAnimation(18, isLoop: false, isForceSet: true);
						chrysalis.isEnableGravity = false;
					}
					if (trackEntry != null)
					{
						_hornEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHornEffect>(chrysalis.hornTransform, Vector3.zero, null);
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "move")
							{
								Vector3 a = chrysalis.flipDirectionVector;
								Transform lockonEnemyTransform = chrysalis.detector.getLockonEnemyTransform();
								float num = 0.5f;
								if (lockonEnemyTransform != null)
								{
									Vector3 vector = lockonEnemyTransform.position - chrysalis.transform.position;
									num = Mathf.Min(Mathf.Abs(vector.x) / 1f, 1f);
									vector.Normalize();
									a = vector;
									vector.y = 0f;
								}
								Vector3 vector2 = a * (e.Float * num);
								chrysalis.resetMoveForce();
								chrysalis.setMoveForce(vector2, 0f, useRound: false);
								chrysalis.setFlipByVector(vector2);
							}
							if (e.Data.Name == "fire")
							{
								if (_attackCount == 0)
								{
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisDoubleSlashFirst>(chrysalis.torsoTransform, chrysalis.flipDirectionVector, null, chrysalis);
								}
								else
								{
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisDoubleSlashSecond>(chrysalis.torsoTransform, chrysalis.flipDirectionVector, null, chrysalis);
								}
								_attackCount++;
							}
							if (e.Data.Name == "finished")
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
					_meleeState = MeleeState.MELEE;
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
