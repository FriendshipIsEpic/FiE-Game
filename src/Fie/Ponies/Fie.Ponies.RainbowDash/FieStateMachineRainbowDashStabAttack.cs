using Fie.Camera;
using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	public class FieStateMachineRainbowDashStabAttack : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			KICK_START,
			KICKING
		}

		private const float KICK_DELAY = 0.2f;

		private const float KICK_HORMING_DISTANCE = 1f;

		private const float KICK_HORMING_DEFAULT_RATE = 0.5f;

		private const float ATTACKING_EFFECT_DESTROY_DELAY = 1f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private float _timeCount;

		private bool _isSetEndAnim;

		private bool _isFinished;

		private bool _isCancellable;

		private float _initializedDrag;

		private FieEmitObjectRainbowDashDoublePaybackAttackingEffect _attackingEffect;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				autoFlipToEnemy(fieRainbowDash);
				fieRainbowDash.Decloack();
				fieRainbowDash.isEnableHeadTracking = false;
				fieRainbowDash.isEnableAutoFlip = false;
				fieRainbowDash.isEnableGravity = false;
				fieRainbowDash.resetMoveForce();
				Rigidbody component = fieRainbowDash.GetComponent<Rigidbody>();
				if (component != null)
				{
					_initializedDrag = component.drag;
					if (fieRainbowDash.healthStats.shield >= 0f)
					{
						component.drag = 5f;
					}
				}
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				fieRainbowDash.isEnableHeadTracking = true;
				fieRainbowDash.isEnableGravity = true;
				fieRainbowDash.isEnableAutoFlip = true;
				fieRainbowDash.isEnableCollider = true;
				fieRainbowDash.isEnableAutoFlip = true;
				fieRainbowDash.isEnableHeadTracking = true;
				Rigidbody component = fieRainbowDash.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.drag = _initializedDrag;
				}
				if (_attackingEffect != null && !_attackingEffect.isDestroyed)
				{
					_attackingEffect.stopEffect(1f);
				}
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieRainbowDash)
			{
				_timeCount += Time.deltaTime;
				FieRainbowDash rainbowDash = gameCharacter as FieRainbowDash;
				_nextState = ((rainbowDash.groundState != 0) ? typeof(FieStateMachineRainbowDashFlying) : typeof(FieStateMachineCommonIdle));
				switch (_fireState)
				{
				case FireState.KICK_START:
				{
					TrackEntry trackEntry = rainbowDash.animationManager.SetAnimation(28, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "move")
							{
								Vector3 vector = rainbowDash.flipDirectionVector;
								if (rainbowDash.detector.lockonTargetObject != null)
								{
									vector = (rainbowDash.detector.lockonTargetObject.centerTransform.position - rainbowDash.centerTransform.position).normalized;
								}
								rainbowDash.resetMoveForce();
								rainbowDash.setMoveForce(vector * e.Float * 3f, 0f, useRound: false);
								rainbowDash.setFlipByVector(vector);
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashStabAttack>(rainbowDash.centerTransform, vector, null, rainbowDash);
								_attackingEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashDoublePaybackAttackingEffect>(rainbowDash.centerTransform, Vector3.up);
								if (FieManagerBehaviour<FieGameCameraManager>.I.gameCamera != null)
								{
									FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setOffsetTransition(rainbowDash, new FieCameraOffset(new FieCameraOffset.FieCameraOffsetParam(new Vector3(0f, 0f, 1.5f), new Vector3(0f, 0f, 0f), 10f), 0.1f, 0.3f, 1.5f));
								}
							}
							if (e.Data.Name == "finished")
							{
								if (_attackingEffect != null)
								{
									_attackingEffect.stopEffect(1f);
								}
								_isFinished = true;
							}
							if (e.Data.name == "cancellable")
							{
								_isCancellable = true;
							}
						};
						_endTime = trackEntry.endTime;
					}
					else
					{
						_endTime = 0f;
					}
					_fireState = FireState.KICKING;
					break;
				}
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
				list.Add(typeof(FieStateMachineRainbowDashBaseAttack1));
			}
			if (_isCancellable)
			{
				list.Add(typeof(FieStateMachineRainbowDashEvasion));
				list.Add(typeof(FieStateMachineRainbowDashOmniSmashStart));
				list.Add(typeof(FieStateMachineRainbowDashDoublePaybackPrepareMidAir));
				list.Add(typeof(FieStateMachineRainbowDashDoublePaybackPrepareOnGround));
				list.Add(typeof(FieStateMachineRainbowDashRainblowCloack));
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
