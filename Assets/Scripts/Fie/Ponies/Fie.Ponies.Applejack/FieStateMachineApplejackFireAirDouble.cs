using Fie.Camera;
using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackFireAirDouble : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			AIR_KICK_START,
			AIR_KICKING
		}

		private const float KICK_DELAY = 0.75f;

		private const float GROUND_FORCE_TIME = 1.5f;

		private const float GROUND_FORCE = 15f;

		private const float AIR_ATTACK_PAST_TIME = 0.3f;

		private const float AIR_ATTACK_FLYING_FORCE = 3f;

		private const float AIR_ATTACK_HORMING_DISTANCE = 15f;

		private Type _nextState = typeof(FieStateMachineApplejackFlying);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private float _timeCount;

		private bool _isSetEndAnim;

		private bool _isFinished;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieApplejack)
			{
				_timeCount += Time.deltaTime;
				FieApplejack applejack = gameCharacter as FieApplejack;
				switch (_fireState)
				{
				case FireState.AIR_KICK_START:
					if (applejack.groundState == FieObjectGroundState.Flying)
					{
						applejack.isEnableGravity = false;
						int animationId = 29;
						if (applejack.isHeavyKickMode)
						{
							animationId = 30;
						}
						TrackEntry trackEntry = applejack.animationManager.SetAnimation(animationId, isLoop: false, isForceSet: true);
						if (FieManagerBehaviour<FieGameCameraManager>.I.gameCamera != null)
						{
							FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setOffsetTransition(applejack, new FieCameraOffset(new FieCameraOffset.FieCameraOffsetParam(new Vector3(0f, 0f, 2f), new Vector3(0f, 0f, 0f), 10f), 0.5f, 0.5f, 1f));
						}
						if (trackEntry != null)
						{
							applejack.CalcBatteCicleSkill();
							trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
							{
								if (trackIndex.Data.Name == "fire")
								{
									applejack.isEnableGravity = true;
									Vector3 externalInputVector2 = applejack.externalInputVector;
									externalInputVector2 += Vector3.up + applejack.flipDirectionVector * 0.2f;
									externalInputVector2.Normalize();
									externalInputVector2 *= 3f;
									applejack.adjustMoveForce(0.2f);
									applejack.setMoveForce(externalInputVector2, 0.3f);
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackAirKick>(applejack.leftBackHoofTransform, Vector3.zero, null, applejack);
									applejack.isEnableCollider = false;
								}
								else if (trackIndex.Data.Name == "finished")
								{
									applejack.isEnableGravity = true;
									applejack.isEnableCollider = true;
									_isEnd = true;
								}
							};
							_endTime = trackEntry.animationEnd;
						}
						else
						{
							_endTime = 0f;
						}
						_fireState = FireState.AIR_KICKING;
					}
					else
					{
						_isEnd = true;
					}
					break;
				}
				Vector3 externalInputVector = applejack.externalInputVector;
				float externalInputForce = applejack.externalInputForce;
				externalInputVector.y = (externalInputVector.z = 0f);
				if (_timeCount > _endTime)
				{
					applejack.isEnableGravity = true;
					_isEnd = true;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieApplejack fieApplejack = gameCharacter as FieApplejack;
			if (!(fieApplejack == null))
			{
				fieApplejack.adjustMoveForce(0.5f);
				fieApplejack.setGravityRate(0.5f);
				fieApplejack.isEnableHeadTracking = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieApplejack fieApplejack = gameCharacter as FieApplejack;
			if (!(fieApplejack == null))
			{
				fieApplejack.setGravityRate(1f);
				fieApplejack.isEnableGravity = true;
				fieApplejack.isEnableHeadTracking = true;
				fieApplejack.isEnableCollider = true;
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

		public override float getDelay()
		{
			return 0.75f;
		}

		public override bool isFinished()
		{
			return _isFinished;
		}
	}
}
