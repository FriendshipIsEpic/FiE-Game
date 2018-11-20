using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	public class FieStateMachineRainbowDashOmniSmashStart : FieStateMachineGameCharacterBase
	{
		private enum AttackState
		{
			ACTIVATE,
			ATTACKING,
			DISAPPEAR
		}

		private const float ATTACKING_DURATION = 0.3f;

		private const float OMNI_SMASH_MOVING_DISTANCE = 3f;

		private const float OMNI_SMASH_NEXT_STATE_DELAY = 0.3f;

		private Type _nextState = typeof(FieStateMachineRainbowDashFlying);

		private bool _isEnd;

		private bool _isFinished;

		private AttackState _prepareState;

		private float _attackDealyCounter;

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
					if (fieRainbowDash.detector.lockonTargetObject == null)
					{
						_nextState = typeof(FieStateMachineCommonIdle);
						_isEnd = true;
					}
					else
					{
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashActivationEffect>(fieRainbowDash.centerTransform, Vector3.up);
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashTrailEffect>(fieRainbowDash.centerTransform, Vector3.up);
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashFirstHit>(fieRainbowDash.centerTransform, fieRainbowDash.flipDirectionVector, null, fieRainbowDash);
						fieRainbowDash.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_USED_ABILITY));
						fieRainbowDash.animationManager.SetAnimation(39, isLoop: true, isForceSet: true);
						Vector3 vector = fieRainbowDash.detector.lockonTargetObject.centerTransform.position - fieRainbowDash.centerTransform.position;
						_movintTargetPos = fieRainbowDash.detector.lockonTargetObject.centerTransform.position + vector.normalized * 3f;
						float maxDistance = Vector3.Distance(fieRainbowDash.centerTransform.position, _movintTargetPos);
						int layerMask = 512;
						if (Physics.Raycast(fieRainbowDash.centerTransform.position, vector.normalized, out RaycastHit hitInfo, maxDistance, layerMask) && hitInfo.collider.tag == "Floor")
						{
							_movintTargetPos = hitInfo.point + Vector3.up * 0.5f;
						}
						_attackingTargetPos = _movintTargetPos;
						UpdateRotation(fieRainbowDash);
						_movingTweener.InitTweener(0.3f, fieRainbowDash.transform.position, _movintTargetPos);
						fieRainbowDash.CalcOmniSmashAttackCount();
						_prepareState = AttackState.ATTACKING;
					}
					break;
				case AttackState.ATTACKING:
					if (!_movingTweener.IsEnd())
					{
						fieRainbowDash.position = _movingTweener.UpdateParameterVec3(Time.deltaTime);
					}
					else
					{
						fieRainbowDash.animationManager.SetAnimation(24, isLoop: false, isForceSet: true);
						_prepareState = AttackState.DISAPPEAR;
					}
					break;
				case AttackState.DISAPPEAR:
					if (fieRainbowDash.detector.lockonTargetObject == null)
					{
						fieRainbowDash.position = _attackingTargetPos;
						_nextState = typeof(FieStateMachineRainbowDashOmniSmashFinish);
						_isEnd = true;
					}
					_attackDealyCounter += Time.deltaTime;
					if (_attackDealyCounter > 0.3f)
					{
						_nextState = typeof(FieStateMachineRainbowDashOmniSmashLoop);
						_isEnd = true;
					}
					break;
				}
			}
		}

		private void UpdateRotation(FieRainbowDash rainbowDash)
		{
			if (!(rainbowDash.rootBone == null))
			{
				rainbowDash.setFlipByVector(_attackingTargetPos - rainbowDash.transform.position);
				Vector3 vector = rainbowDash.transform.position - _attackingTargetPos;
				vector.z = 0f;
				vector.Normalize();
				rainbowDash.rootBone.mode = SkeletonUtilityBone.Mode.Override;
				rainbowDash.rootBone.overrideAlpha = 1f;
				if (vector != Vector3.zero)
				{
					rainbowDash.rootBone.transform.localRotation = Quaternion.LookRotation(vector) * Quaternion.AngleAxis((rainbowDash.flipState != 0) ? 270f : (-90f), Vector3.up);
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				fieRainbowDash.Decloack();
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
				fieRainbowDash.rootBone.transform.localRotation = Quaternion.identity;
				fieRainbowDash.resetMoveForce();
			}
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineRainbowDashOmniSmashLoop));
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
