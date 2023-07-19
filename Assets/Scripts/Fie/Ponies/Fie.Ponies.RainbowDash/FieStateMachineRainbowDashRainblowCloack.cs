using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	public class FieStateMachineRainbowDashRainblowCloack : FieStateMachineGameCharacterBase
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
					TrackEntry trackEntry = rainbowDash.animationManager.SetAnimation(45, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "fire")
							{
								rainbowDash.Cloack();
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
					if (rainbowDash.groundState == FieObjectGroundState.Grounding)
					{
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashRainblowEntity>(rainbowDash.transform, rainbowDash.flipDirectionVector, null, rainbowDash);
					}
					else
					{
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashRainblowEntityMidAir>(rainbowDash.centerTransform, rainbowDash.flipDirectionVector, null, rainbowDash);
					}
					rainbowDash.abilitiesContainer.SetCooldown<FieStateMachineRainbowDashRainblow>(9f);
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
			list.Add(typeof(FieStateMachineRainbowDashEvasion));
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
