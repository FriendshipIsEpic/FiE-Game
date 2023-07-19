using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	public class FieStateMachineChangelingAlphaZeroDistanceCharge : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			KICK_START,
			KICKING
		}

		private const float KICK_DELAY = 0.2f;

		private const float KICK_HORMING_DISTANCE = 10f;

		private const float KICK_HORMING_DEFAULT_RATE = 1f;

		private const float GROUND_FORCE_TIME = 1.5f;

		private const float GROUND_FORCE = 15f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private float _timeCount;

		private bool _isSetEndAnim;

		private bool _isFinished;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieChangelingAlpha fieChangelingAlpha = gameCharacter as FieChangelingAlpha;
			if (!(fieChangelingAlpha == null))
			{
				autoFlipToEnemy(fieChangelingAlpha);
				fieChangelingAlpha.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieChangelingAlpha fieChangelingAlpha = gameCharacter as FieChangelingAlpha;
			if (!(fieChangelingAlpha == null))
			{
				fieChangelingAlpha.isEnableAutoFlip = false;
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieChangelingAlpha)
			{
				_timeCount += Time.deltaTime;
				FieChangelingAlpha changelingAlpha = gameCharacter as FieChangelingAlpha;
				switch (_fireState)
				{
				case FireState.KICK_START:
					if (changelingAlpha.groundState == FieObjectGroundState.Grounding)
					{
						TrackEntry trackEntry = changelingAlpha.animationManager.SetAnimation(15, isLoop: false, isForceSet: true);
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
							{
								if (e.Data.Name == "fire")
								{
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingAlphaChargeFinish>(changelingAlpha.centerTransform, changelingAlpha.flipDirectionVector, null, changelingAlpha);
								}
								if (e.Data.Name == "move")
								{
									Vector3 a = changelingAlpha.flipDirectionVector;
									Transform lockonEnemyTransform = changelingAlpha.detector.getLockonEnemyTransform();
									float num = 1f;
									if (lockonEnemyTransform != null)
									{
										Vector3 vector = lockonEnemyTransform.position - changelingAlpha.transform.position;
										vector.Normalize();
										a = vector;
										a.y = 0f;
									}
									Vector3 vector2 = a * (e.Float * num);
									changelingAlpha.resetMoveForce();
									changelingAlpha.setMoveForce(vector2, 0f, useRound: false);
									if (e.Float > 0f)
									{
										changelingAlpha.setFlipByVector(vector2);
									}
								}
								if (e.Data.Name == "finished")
								{
									_isEnd = true;
								}
							};
							_endTime = trackEntry.endTime;
						}
						else
						{
							_endTime = 0f;
						}
						_fireState = FireState.KICKING;
					}
					else
					{
						_isEnd = true;
					}
					break;
				}
				if (_timeCount > _endTime)
				{
					_isEnd = true;
				}
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
			return new List<Type>();
		}

		public override float getDelay()
		{
			return 0.2f;
		}

		public override bool isFinished()
		{
			return _isFinished;
		}
	}
}
