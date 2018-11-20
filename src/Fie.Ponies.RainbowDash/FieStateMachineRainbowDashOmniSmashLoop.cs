using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	public class FieStateMachineRainbowDashOmniSmashLoop : FieStateMachineGameCharacterBase
	{
		private enum AttackState
		{
			ACTIVATE,
			ATTACKING,
			DISAPPEAR
		}

		private const float ATTACKING_DURATION = 0.25f;

		private const float OMNI_SMASH_MOVING_DISTANCE = 1f;

		private const float OMNI_SMASH_NEXT_STATE_DELAY = 0.3f;

		private const float OMNI_SMASH_ENTITY_RANGE = 3f;

		private Type _nextState = typeof(FieStateMachineRainbowDashFlying);

		private int _attackCount;

		private bool _isEnd;

		private bool _isFinished;

		private AttackState _prepareState;

		private float _attackDealyCounter;

		private float _attackInterval;

		private Vector3 _initInputVec = Vector3.zero;

		private Vector3 _movintTargetPos = Vector3.zero;

		private Vector3 _attackingTargetPos = Vector3.zero;

		private Vector3 _reverseDirection = Vector3.zero;

		private Tweener<TweenTypesInOutSine> _movingTweener = new Tweener<TweenTypesInOutSine>();

		private FieEmitObjectRainbowDashDoublePaybackAttackingEffect _attackingEffect;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieRainbowDash)
			{
				FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
				switch (_prepareState)
				{
				case AttackState.ACTIVATE:
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashActivationEffect>(fieRainbowDash.centerTransform, Vector3.up);
					fieRainbowDash.animationManager.SetAnimation(44, isLoop: true, isForceSet: true);
					_attackCount = Mathf.Max(0, fieRainbowDash.omniSmashAttackingCount - 2);
					fieRainbowDash.submeshObject.enabled = false;
					_prepareState = AttackState.ATTACKING;
					break;
				case AttackState.ATTACKING:
					if (fieRainbowDash.detector.lockonTargetObject != null)
					{
						fieRainbowDash.position = fieRainbowDash.detector.lockonTargetObject.centerTransform.position;
					}
					_attackInterval -= Time.deltaTime;
					if (_attackInterval <= 0f)
					{
						if (_attackCount <= 0)
						{
							_nextState = typeof(FieStateMachineRainbowDashOmniSmashFinish);
							fieRainbowDash.submeshObject.enabled = true;
							_isEnd = true;
						}
						else
						{
							EmitOmniSmashAttackEntity(fieRainbowDash);
							_attackCount--;
							_attackInterval = 0.25f;
						}
					}
					break;
				}
			}
		}

		private void EmitOmniSmashAttackEntity(FieRainbowDash rainbowDash)
		{
			Vector3 vector = new Vector3((float)FieRandom.Range(-100, 100), (float)UnityEngine.Random.Range(-30, 30), (float)UnityEngine.Random.Range(-100, 100)).normalized;
			if (vector == Vector3.zero)
			{
				vector = Vector3.back;
			}
			Vector3 vector2 = rainbowDash.centerTransform.position + vector * 3f;
			Vector3 normalized = (rainbowDash.centerTransform.position - vector2).normalized;
			FieEmitObjectRainbowDashOmniSmashSecondHit fieEmitObjectRainbowDashOmniSmashSecondHit = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashSecondHit>(rainbowDash.centerTransform, normalized, null, rainbowDash);
			if (fieEmitObjectRainbowDashOmniSmashSecondHit != null)
			{
				fieEmitObjectRainbowDashOmniSmashSecondHit.SetInitializePosition(vector2);
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				fieRainbowDash.isEnableCollider = false;
				fieRainbowDash.isEnableHeadTracking = false;
				fieRainbowDash.isEnableAutoFlip = false;
				fieRainbowDash.isEnableGravity = false;
				fieRainbowDash.resetMoveForce();
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
				fieRainbowDash.resetMoveForce();
			}
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineRainbowDashOmniSmashFinish));
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
