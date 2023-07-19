using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	public class FieStateMachineRainbowDashEvasionBack : FieStateMachineGameCharacterBase
	{
		private enum EvasionState
		{
			PREPARE,
			EVASION
		}

		private const float EVASION_DURATION = 0.2f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private bool _isTakeOff;

		private bool _isEnd;

		private bool _isFinished;

		private EvasionState _evasionState;

		private Vector3 _initInputVec = Vector3.zero;

		private Vector3 _evasionTargetPos = Vector3.zero;

		private Tweener<TweenTypesOutSine> _evasionTweener = new Tweener<TweenTypesOutSine>();

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieRainbowDash)
			{
				FieRainbowDash rainbowDash = gameCharacter as FieRainbowDash;
				switch (_evasionState)
				{
				case EvasionState.PREPARE:
				{
					_initInputVec = rainbowDash.externalInputVector;
					TrackEntry trackEntry = rainbowDash.animationManager.SetAnimation(23, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "move")
							{
								setEvasion(rainbowDash, e);
							}
							if (e.Data.Name == "finished")
							{
								_isFinished = true;
							}
							if (e.Data.Name == "takeOff")
							{
								_isTakeOff = true;
							}
							if (e.Data.Name == "immunity")
							{
								rainbowDash.isEnableCollider = (e.Int < 1);
							}
						};
						trackEntry.Complete += delegate
						{
							setEnd(rainbowDash as T);
						};
					}
					else
					{
						setEnd(rainbowDash as T);
					}
					_evasionState = EvasionState.EVASION;
					break;
				}
				case EvasionState.EVASION:
					if (!_evasionTweener.IsEnd())
					{
						rainbowDash.position = _evasionTweener.UpdateParameterVec3(Time.deltaTime);
						if (rainbowDash.groundState == FieObjectGroundState.Grounding && _isTakeOff)
						{
							setEnd(rainbowDash as T);
						}
					}
					break;
				}
			}
		}

		private void setEvasion(FieRainbowDash rainbowDash, Spine.Event e)
		{
			Vector3 vector = _initInputVec;
			if (rainbowDash.externalInputForce <= 0.25f || vector == Vector3.zero)
			{
				vector = -rainbowDash.flipDirectionVector;
			}
			if (rainbowDash.groundState == FieObjectGroundState.Grounding)
			{
				if (Mathf.Abs(vector.x) < 0.5f)
				{
					Vector3 flipDirectionVector = rainbowDash.flipDirectionVector;
					vector.x = 0f - flipDirectionVector.x;
				}
				vector.y = Mathf.Clamp(vector.y, 0.25f, 0.5f);
			}
			vector.Normalize();
			_evasionTargetPos = rainbowDash.position + vector * e.Float;
			int layerMask = 262656;
			if (Physics.Raycast(rainbowDash.position, vector, out RaycastHit hitInfo, e.Float, layerMask))
			{
				_evasionTargetPos = hitInfo.point;
			}
			FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashEvasionEffect>(rainbowDash.centerTransform, _evasionTargetPos - rainbowDash.position, null, rainbowDash);
			_evasionTweener.InitTweener(0.2f, rainbowDash.position, _evasionTargetPos);
			rainbowDash.resetMoveForce();
		}

		private void setEnd<T>(T gameCharacter) where T : FieGameCharacter
		{
			if ((UnityEngine.Object)gameCharacter != (UnityEngine.Object)null)
			{
				if (gameCharacter.groundState == FieObjectGroundState.Flying)
				{
					_nextState = typeof(FieStateMachineRainbowDashFlying);
				}
				else
				{
					_nextState = typeof(FieStateMachinePoniesLanding);
				}
			}
			_isEnd = true;
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				fieRainbowDash.isEnableHeadTracking = false;
				fieRainbowDash.isEnableAutoFlip = false;
				fieRainbowDash.isEnableGravity = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				fieRainbowDash.isEnableHeadTracking = true;
				fieRainbowDash.setGravityRate(1f);
				fieRainbowDash.isEnableGravity = true;
				fieRainbowDash.isEnableAutoFlip = true;
				fieRainbowDash.isEnableCollider = true;
			}
		}

		public override List<Type> getAllowedStateList()
		{
			if (!_isFinished)
			{
				return new List<Type>();
			}
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineRainbowDashBaseAttack1));
			list.Add(typeof(FieStateMachineRainbowDashBaseAttack3));
			list.Add(typeof(FieStateMachineRainbowDashStabAttack));
			list.Add(typeof(FieStateMachineRainbowDashEvasion));
			list.Add(typeof(FieStateMachineRainbowDashRainblow));
			list.Add(typeof(FieStateMachineRainbowDashOmniSmashStart));
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
	}
}
