using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackFirePunch : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			PUNCH_START,
			PUNCHING
		}

		private const float PUNCH_DELAY = 0.5f;

		private const float PUNCH_HORMING_DISTANCE = 1f;

		private const float PUNCH_HORMING_DEFAULT_RATE = 0.5f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private float _timeCount;

		private bool _isSetEndAnim;

		private bool _isFinished;

		private bool _isCancellable;

		private int _punchCount;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			autoFlipToEnemy(gameCharacter);
			gameCharacter.isEnableAutoFlip = false;
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			gameCharacter.isEnableAutoFlip = true;
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieApplejack)
			{
				_timeCount += Time.deltaTime;
				FieApplejack applejack = gameCharacter as FieApplejack;
				switch (_fireState)
				{
				case FireState.PUNCH_START:
					if (applejack.groundState == FieObjectGroundState.Grounding)
					{
						TrackEntry trackEntry = applejack.animationManager.SetAnimation(22, isLoop: false, isForceSet: true);
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
							{
								if (trackIndex.Data.Name == "fire")
								{
									switch (_punchCount)
									{
									case 0:
										FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackPunch>(applejack.rightFrontHoofTransform, applejack.flipDirectionVector, null, applejack);
										break;
									case 1:
										FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackPunch>(applejack.leftFrontHoofTransform, applejack.flipDirectionVector, null, applejack);
										break;
									}
									_punchCount++;
								}
								if (trackIndex.Data.Name == "move")
								{
									Vector3 a = applejack.flipDirectionVector;
									Transform lockonEnemyTransform = applejack.detector.getLockonEnemyTransform();
									float num = 0.5f;
									if (lockonEnemyTransform != null)
									{
										Vector3 vector = lockonEnemyTransform.position - applejack.transform.position;
										num = Mathf.Min(Mathf.Abs(vector.x) / 1f, 1f);
										vector.Normalize();
										a = vector;
										a.y = 0f;
									}
									Vector3 vector2 = a * (trackIndex.Float * num);
									applejack.resetMoveForce();
									applejack.setMoveForce(vector2, 0f);
									applejack.setFlipByVector(vector2);
								}
								if (trackIndex.Data.Name == "finished")
								{
									_isFinished = true;
								}
								if (trackIndex.Data.name == "cancellable")
								{
									_isCancellable = true;
								}
							};
							_endTime = trackEntry.animationEnd;
						}
						else
						{
							_endTime = 0f;
						}
					}
					else
					{
						_isEnd = true;
					}
					_fireState = FireState.PUNCHING;
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
			if (_isFinished)
			{
				list.Add(typeof(FieStateMachineApplejackFireKick));
			}
			if (_isCancellable)
			{
				list.Add(typeof(FieStateMachineApplejackEvasion));
				list.Add(typeof(FieStateMachineApplejackFireRope));
				list.Add(typeof(FieStateMachineApplejackYeehawAction));
				list.Add(typeof(FieStateMachineApplejackStompJump));
			}
			return list;
		}

		public override float getDelay()
		{
			return 0.5f;
		}

		public override bool isFinished()
		{
			return _isFinished;
		}
	}
}
