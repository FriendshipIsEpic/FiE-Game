using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackRopeActionAir : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			ROPE_ACTION_START,
			ROPE_ACTION_FLYING,
			ROPE_ACTION_END
		}

		private const float ROPE_ACTION_FLYING_VELOCITY = 40f;

		private const float ROPE_ACTION_END_DISTANCE = 1f;

		private const float ROPE_ACTION_FINISH_FLYING_FORCE = 5f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private float _timeCount;

		private bool _isFinished;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieApplejack)
			{
				_timeCount += Time.deltaTime;
				FieApplejack fieApplejack = gameCharacter as FieApplejack;
				switch (_fireState)
				{
				case FireState.ROPE_ACTION_START:
					fieApplejack.isEnableGravity = false;
					fieApplejack.animationManager.SetAnimation(33, isLoop: false, isForceSet: true);
					gameCharacter.isEnableAutoFlip = false;
					fieApplejack.isEnableCollider = false;
					_fireState = FireState.ROPE_ACTION_FLYING;
					break;
				case FireState.ROPE_ACTION_FLYING:
					if (targetGameCharacterList.Count > 0)
					{
						if (targetGameCharacterList[0] == null)
						{
							_isEnd = true;
						}
						else
						{
							Transform transform = targetGameCharacterList[0].transform;
							if (transform != null)
							{
								Vector3 a = transform.position - fieApplejack.transform.position;
								float num = Vector3.Distance(transform.position, fieApplejack.transform.position);
								a.Normalize();
								fieApplejack.setFlipByVector(transform.position - fieApplejack.transform.position);
								fieApplejack.addMoveForce(a * 40f, 0f, useRound: false);
								fieApplejack.currentMovingVec += a * 40f;
								if (num < 1f)
								{
									TrackEntry trackEntry = fieApplejack.animationManager.SetAnimation(34, isLoop: false, isForceSet: true);
									if (trackEntry != null)
									{
										trackEntry.Complete += delegate
										{
											_nextState = typeof(FieStateMachinePoniesJump);
											_isEnd = true;
										};
									}
									fieApplejack.isEnableGravity = true;
									Vector3 up = Vector3.up;
									up += fieApplejack.flipDirectionVector * -0.1f;
									up.Normalize();
									up *= 5f;
									fieApplejack.setGravityRate(1f);
									fieApplejack.resetMoveForce();
									fieApplejack.setMoveForce(up, 0f);
									fieApplejack.currentMovingVec += up * 40f;
									fieApplejack.isEnableCollider = true;
									_isFinished = true;
									_fireState = FireState.ROPE_ACTION_END;
								}
							}
							else
							{
								_isEnd = true;
								_fireState = FireState.ROPE_ACTION_END;
							}
						}
					}
					else
					{
						_isEnd = true;
						_fireState = FireState.ROPE_ACTION_END;
					}
					break;
				}
				if (_timeCount > _endTime)
				{
					_isEnd = true;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieApplejack fieApplejack = gameCharacter as FieApplejack;
			if (!(fieApplejack == null))
			{
				fieApplejack.isEnableAutoFlip = false;
				fieApplejack.isEnableHeadTracking = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieApplejack fieApplejack = gameCharacter as FieApplejack;
			if (!(fieApplejack == null))
			{
				fieApplejack.abilitiesContainer.SetCooldown<FieStateMachineApplejackRope>(3f);
				fieApplejack.isEnableAutoFlip = true;
				fieApplejack.emotionController.RestoreEmotionFromDefaultData();
				fieApplejack.isEnableCollider = true;
				fieApplejack.setGravityRate(1f);
				fieApplejack.isEnableGravity = true;
				fieApplejack.isEnableHeadTracking = true;
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
			list.Add(typeof(FieStateMachineApplejackFlying));
			list.Add(typeof(FieStateMachineApplejackFireAir));
			list.Add(typeof(FieStateMachineApplejackFireAirDouble));
			list.Add(typeof(FieStateMachineApplejackYeehawActionMidAir));
			list.Add(typeof(FieStateMachineApplejackStompAction));
			return list;
		}

		public override bool isFinished()
		{
			return _isFinished;
		}
	}
}
