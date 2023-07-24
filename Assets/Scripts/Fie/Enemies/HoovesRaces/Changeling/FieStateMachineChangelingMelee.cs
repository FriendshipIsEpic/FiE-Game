using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	public class FieStateMachineChangelingMelee : FieStateMachineGameCharacterBase
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

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieChangeling fieChangeling = gameCharacter as FieChangeling;
			if (!(fieChangeling == null))
			{
				autoFlipToEnemy(fieChangeling);
				fieChangeling.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieChangeling)
			{
				FieChangeling changeling = gameCharacter as FieChangeling;
				if (_meleeState == MeleeState.MELEE_START)
				{
					if (changeling.groundState == FieObjectGroundState.Grounding)
					{
						TrackEntry trackEntry = changeling.animationManager.SetAnimation(11, isLoop: false, isForceSet: true);
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
							{
								if (trackIndex.Data.Name == "move")
								{
									Vector3 a = changeling.flipDirectionVector;
									Transform lockonEnemyTransform = changeling.detector.getLockonEnemyTransform();
									float num = 0.5f;
									if (lockonEnemyTransform != null)
									{
										Vector3 vector = lockonEnemyTransform.position - changeling.transform.position;
										num = Mathf.Min(Mathf.Abs(vector.x) / 1f, 1f);
										vector.Normalize();
										a = vector;
										vector.y = 0f;
									}
									Vector3 vector2 = a * (trackIndex.Float * num);
									changeling.resetMoveForce();
									changeling.setMoveForce(vector2, 0f, useRound: false);
									changeling.setFlipByVector(vector2);
								}
								if (trackIndex.Data.Name == "fire")
								{
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingBite>(changeling.mouthTransform, changeling.flipDirectionVector, null, changeling);
								}
								if (trackIndex.Data.Name == "finished")
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
					}
					else
					{
						_isEnd = true;
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
