using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackFireKick : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			KICK_START,
			KICKING
		}

		private const float KICK_DELAY = 0.2f;

		private const float KICK_HORMING_DISTANCE = 1f;

		private const float KICK_HORMING_DEFAULT_RATE = 0.5f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private float _timeCount;

		private bool _isSetEndAnim;

		private bool _isFinished;

		private bool _isCancellable;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieApplejack fieApplejack = gameCharacter as FieApplejack;
			if (!(fieApplejack == null))
			{
				autoFlipToEnemy(fieApplejack);
				fieApplejack.isEnableAutoFlip = false;
				fieApplejack.isEnableHeadTracking = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieApplejack fieApplejack = gameCharacter as FieApplejack;
			if (!(fieApplejack == null))
			{
				fieApplejack.isEnableAutoFlip = true;
				fieApplejack.isEnableHeadTracking = true;
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieApplejack)
			{
				_timeCount += Time.deltaTime;
				FieApplejack applejack = gameCharacter as FieApplejack;
				switch (_fireState)
				{
				case FireState.KICK_START:
					if (applejack.groundState == FieObjectGroundState.Grounding)
					{
						applejack.switchFlip();
						int animationId = 23;
						if (applejack.isHeavyKickMode)
						{
							animationId = 24;
						}
						TrackEntry trackEntry = applejack.animationManager.SetAnimation(animationId, isLoop: false, isForceSet: true);
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
							{
								if (e.Data.Name == "fire")
								{
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackKick>(applejack.leftBackHoofTransform, applejack.flipDirectionVector * -1f, null, applejack);
								}
								if (e.Data.Name == "move")
								{
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackKickEffect>(applejack.centerTransform, applejack.flipDirectionVector * -1f);
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackKickEffect2>(applejack.leftFrontHoofTransform, applejack.flipDirectionVector);
									Vector3 a = applejack.flipDirectionVector * -1f;
									Transform lockonEnemyTransform = applejack.detector.getLockonEnemyTransform();
									float num = 0.5f;
									if (lockonEnemyTransform != null)
									{
										Vector3 vector = lockonEnemyTransform.position - applejack.transform.position;
										num = Mathf.Min(Mathf.Abs(vector.x) / 1f, 1f);
										num *= 1f;
										vector.Normalize();
										a = vector;
										a.y = 0f;
									}
									Vector3 vector2 = a * (e.Float * num);
									applejack.resetMoveForce();
									applejack.setMoveForce(vector2, 0f, useRound: false);
									applejack.setFlipByVector(vector2 * -1f);
									applejack.CalcBatteCicleSkill();
								}
								if (e.Data.Name == "finished")
								{
									_isFinished = true;
								}
								if (e.Data.name == "cancellable")
								{
									_isCancellable = (e.Int >= 1);
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
			List<Type> list = new List<Type>();
			if (_isCancellable)
			{
				list.Add(typeof(FieStateMachineApplejackEvasion));
				list.Add(typeof(FieStateMachineApplejackFireRope));
				list.Add(typeof(FieStateMachineApplejackYeehaw));
			}
			if (_isFinished)
			{
				list.Add(typeof(FieStateMachineApplejackFirePunch));
			}
			return list;
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
