using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackRopeAction : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			ROPE_ACTION_START,
			ROPE_ACTION_PULL,
			ROPE_ACTION_END
		}

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
				FieApplejack applejack = gameCharacter as FieApplejack;
				switch (_fireState)
				{
				case FireState.ROPE_ACTION_START:
				{
					TrackEntry trackEntry = applejack.animationManager.SetAnimation(35, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "move")
							{
								Vector3 flipDirectionVector = applejack.flipDirectionVector;
								Vector3 moveForce = flipDirectionVector * e.Float;
								applejack.setMoveForce(moveForce, 0f);
							}
							if (e.Data.Name == "finished")
							{
								_isFinished = true;
							}
						};
						trackEntry.Complete += delegate
						{
							applejack.animationManager.SetAnimation(0, isLoop: true);
							_nextState = typeof(FieStateMachineCommonIdle);
							_isEnd = true;
						};
					}
					else
					{
						_isEnd = true;
					}
					_fireState = FireState.ROPE_ACTION_PULL;
					break;
				}
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
				fieApplejack.emotionController.RestoreEmotionFromDefaultData();
				fieApplejack.abilitiesContainer.SetCooldown<FieStateMachineApplejackRope>(3f);
				fieApplejack.isEnableAutoFlip = true;
				fieApplejack.isEnableHeadTracking = true;
			}
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list = new List<Type>();
			if (_isFinished)
			{
				list.Add(typeof(FieStateMachineApplejackFireKickRift));
			}
			return list;
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return _nextState;
		}

		public override bool isFinished()
		{
			return _isFinished;
		}
	}
}
