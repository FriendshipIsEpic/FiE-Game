using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackYeehawActionMidAir : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			YEEHAW_ACTION_START,
			YEEHAW_ACTION_END
		}

		private const float YEEHAW_MID_AIR_TAKEOFF_PAST_TIME = 0.3f;

		private const float YEEHAW_MID_AIR_TAKEOFF_FORCE = 1f;

		private Type _nextState = typeof(FieStateMachineApplejackFlying);

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
				case FireState.YEEHAW_ACTION_START:
				{
					TrackEntry trackEntry = applejack.animationManager.SetAnimation(38, isLoop: false, isForceSet: true);
					applejack.emotionController.SetEmoteAnimation(43, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
						{
							if (trackIndex.Data.Name == "takeOff")
							{
								Vector3 externalInputVector = applejack.externalInputVector;
								externalInputVector += Vector3.up + applejack.flipDirectionVector * 0.05f;
								externalInputVector.Normalize();
								externalInputVector *= 1f;
								applejack.resetMoveForce();
								applejack.setMoveForce(externalInputVector, 0.3f);
							}
							if (trackIndex.Data.Name == "finished")
							{
								applejack.isEnableGravity = true;
								_isFinished = true;
								applejack.emotionController.RestoreEmotionFromDefaultData();
							}
						};
						trackEntry.Complete += delegate
						{
							_isEnd = true;
						};
						GDESkillTreeData skill = applejack.GetSkill(FieConstValues.FieSkill.HONESTY_YEEHAW_LV3_2);
						if (skill != null)
						{
							applejack.isEnableCollider = false;
						}
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackYeehaw>(applejack.mouthTransform, Vector3.zero, null, applejack);
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackYeehawReflect>(applejack.centerTransform, Vector3.zero, null, applejack);
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackYeehawRegen>(applejack.centerTransform, Vector3.zero, null, applejack);
						FieManagerBehaviour<FieGameCameraManager>.I.setWiggler(1f, 10, Vector3.one * 0.2f);
					}
					else
					{
						_isEnd = true;
					}
					float num = 0.2f;
					GDESkillTreeData skill2 = applejack.GetSkill(FieConstValues.FieSkill.HONESTY_YEEHAW_LV2_2);
					if (skill2 != null)
					{
						num += skill2.Value1;
					}
					applejack.damageSystem.Regen((applejack.healthStats.maxHitPoint + applejack.healthStats.maxShield) * num);
					_fireState = FireState.YEEHAW_ACTION_END;
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
				fieApplejack.isEnableGravity = false;
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
				fieApplejack.isEnableGravity = true;
				fieApplejack.isEnableAutoFlip = true;
				fieApplejack.isEnableHeadTracking = true;
				fieApplejack.isEnableCollider = true;
			}
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list = new List<Type>();
			if (_isFinished)
			{
				list.Add(typeof(FieStateMachineApplejackFireAir));
				list.Add(typeof(FieStateMachineApplejackFireRope));
				list.Add(typeof(FieStateMachineApplejackStompAction));
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
