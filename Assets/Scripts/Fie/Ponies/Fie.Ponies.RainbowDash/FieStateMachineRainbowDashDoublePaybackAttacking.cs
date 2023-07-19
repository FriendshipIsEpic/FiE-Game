using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	public class FieStateMachineRainbowDashDoublePaybackAttacking : FieStateMachineGameCharacterBase
	{
		private enum AttackState
		{
			DISAPPEAR,
			PREPARE_FOR_APPEAR,
			ATTACKING,
			APPEAR,
			ATTACK_FINISHED
		}

		private const float ATTACKING_DURATION = 0.1f;

		private const float DOUBLE_PAYBACK_APPEAR_DELAY = 0.2f;

		private const float DOUBLE_PAYBACK_ATTACK_DELAY = 0.5f;

		private const float DOUBLE_PAYBACK_MOVING_DISTANCE = 2.5f;

		private const float DOUBLE_PAYBACK_EFFECT_DESTROY_DELAY = 1f;

		private Type _nextState = typeof(FieStateMachineRainbowDashFlying);

		private bool _isTakeOff;

		private bool _isEnd;

		private bool _isFinished;

		private float _appearCounter;

		private float _attackDealyCounter;

		private float _initializedDrag;

		private AttackState _prepareState;

		private Vector3 _initInputVec = Vector3.zero;

		private Vector3 _movintTargetPos = Vector3.zero;

		private Vector3 _attackingTargetPos = Vector3.zero;

		private Vector3 _reverseDirection = Vector3.zero;

		private Tweener<TweenTypesOutSine> _movingTweener = new Tweener<TweenTypesOutSine>();

		private Tweener<TweenTypesOutSine> _attackingTweener = new Tweener<TweenTypesOutSine>();

		private FieEmitObjectRainbowDashDoublePaybackAttackingEffect _attackingEffect;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieRainbowDash)
			{
				FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
				switch (_prepareState)
				{
				case AttackState.DISAPPEAR:
					if (fieRainbowDash.detector.lockonTargetObject == null)
					{
						_nextState = typeof(FieStateMachineCommonIdle);
						_isEnd = true;
					}
					else
					{
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashDoublePaybackActivationEffect>(fieRainbowDash.centerTransform, Vector3.up);
						_attackingEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashDoublePaybackAttackingEffect>(fieRainbowDash.centerTransform, Vector3.up);
						fieRainbowDash.animationManager.SetAnimation(37, isLoop: true, isForceSet: true);
						Vector3 a = -fieRainbowDash.detector.lockonTargetObject.flipDirectionVector + Vector3.up;
						a.Normalize();
						UpdateRotation(fieRainbowDash);
						_movintTargetPos = fieRainbowDash.detector.lockonTargetObject.transform.position + a * 2.5f;
						_movingTweener.InitTweener(0.2f, fieRainbowDash.transform.position, _movintTargetPos);
						fieRainbowDash.requestSetAwesomeEffect(1);
						fieRainbowDash.submeshObject.enabled = false;
						_prepareState = AttackState.PREPARE_FOR_APPEAR;
					}
					break;
				case AttackState.PREPARE_FOR_APPEAR:
					if (!_movingTweener.IsEnd())
					{
						fieRainbowDash.position = _movingTweener.UpdateParameterVec3(Time.deltaTime);
					}
					else
					{
						fieRainbowDash.submeshObject.enabled = true;
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashDoublePaybackActivationEffect>(fieRainbowDash.centerTransform, Vector3.up);
						fieRainbowDash.isEnableHeadTracking = false;
						_prepareState = AttackState.APPEAR;
					}
					break;
				case AttackState.APPEAR:
					if (fieRainbowDash.detector.lockonTargetObject == null)
					{
						_nextState = typeof(FieStateMachineCommonIdle);
						_isEnd = true;
					}
					else
					{
						_attackDealyCounter += Time.deltaTime;
						_attackingTargetPos = fieRainbowDash.detector.lockonTargetObject.centerTransform.position;
						UpdateRotation(fieRainbowDash);
						if (_attackDealyCounter > 0.5f)
						{
							FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashDoublePaybackCollision>(fieRainbowDash.rootBone.transform, _attackingTargetPos - fieRainbowDash.transform.position, null, fieRainbowDash);
							_attackingTweener.InitTweener(0.1f, fieRainbowDash.transform.position, _attackingTargetPos);
							_reverseDirection = _attackingTargetPos - fieRainbowDash.transform.position;
							_reverseDirection = _reverseDirection.normalized * -1f;
							fieRainbowDash.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_USED_ABILITY));
							_prepareState = AttackState.ATTACKING;
						}
					}
					break;
				case AttackState.ATTACKING:
					UpdateRotation(fieRainbowDash);
					if (!_attackingTweener.IsEnd())
					{
						fieRainbowDash.transform.position = _attackingTweener.UpdateParameterVec3(Time.deltaTime);
					}
					else
					{
						TrackEntry trackEntry = fieRainbowDash.animationManager.SetAnimation(38, isLoop: false, isForceSet: true);
						_prepareState = AttackState.ATTACK_FINISHED;
						fieRainbowDash.rootBone.transform.localRotation = Quaternion.identity;
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
							{
								if (e.Data.Name == "finished")
								{
									_isFinished = true;
								}
							};
							trackEntry.Complete += delegate
							{
								_isEnd = true;
								_nextState = typeof(FieStateMachineRainbowDashFlying);
								if (_attackingEffect != null)
								{
									_attackingEffect.stopEffect(1f);
								}
							};
						}
						GDESkillTreeData skill = fieRainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_DOUBLE_PAYBACK_LV3_1);
						if (skill != null)
						{
							fieRainbowDash.damageSystem.AddDefenceMagni(skill.ID, 1f, skill.Value1);
						}
						GDESkillTreeData skill2 = fieRainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_DOUBLE_PAYBACK_LV3_2);
						if (skill2 != null)
						{
							fieRainbowDash.damageSystem.AddAttackMagni(skill2.ID, skill2.Value2, skill2.Value1);
						}
						GDESkillTreeData skill3 = fieRainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_DOUBLE_PAYBACK_LV4_MAXIMUM_AWESOMENESS);
						if (skill3 != null)
						{
							fieRainbowDash.requestSetAwesomeEffect(fieRainbowDash.maximumAwesomeCount);
						}
						GDESkillTreeData skill4 = fieRainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_DOUBLE_PAYBACK_LV4_AWESOME_COMBO);
						if (skill4 != null)
						{
							fieRainbowDash.abilitiesContainer.IncreaseOrReduceCooldownAll(-999f);
						}
						fieRainbowDash.setMoveForce(_reverseDirection * 15f, 0.5f, useRound: false);
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
				if (vector != Vector3.zero)
				{
					rainbowDash.rootBone.transform.localRotation = Quaternion.LookRotation(vector) * Quaternion.AngleAxis((rainbowDash.flipState != 0) ? 270f : (-90f), Vector3.up);
				}
			}
		}

		private void setCounterEvent(FieRainbowDash rainbowDash)
		{
			rainbowDash.damageSystem.damageCheckEvent += HealthSystem_damageCheckEvent;
			rainbowDash.SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_P_LOYALTY_ABILITY_DOUBLE_PAYBACK_1), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_P_LOYALTY_ABILITY_DOUBLE_PAYBACK_2));
		}

		private bool HealthSystem_damageCheckEvent(FieGameCharacter attacker, FieDamage damage)
		{
			_nextState = typeof(FieStateMachineCommonIdle);
			_isEnd = true;
			return false;
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
				fieRainbowDash.damageSystem.damageCheckEvent -= HealthSystem_damageCheckEvent;
				fieRainbowDash.isEnableHeadTracking = true;
				fieRainbowDash.isEnableGravity = true;
				fieRainbowDash.isEnableAutoFlip = true;
				fieRainbowDash.isEnableCollider = true;
				fieRainbowDash.resetMoveForce();
				fieRainbowDash.submeshObject.enabled = true;
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

		public override List<Type> getAllowedStateList()
		{
			if (_isFinished)
			{
				List<Type> list = new List<Type>();
				list.Add(typeof(FieStateMachineRainbowDashOmniSmashStart));
				list.Add(typeof(FieStateMachineRainbowDashEvasion));
				return list;
			}
			if (_isEnd)
			{
				List<Type> list = new List<Type>();
				list.Add(typeof(FieStateMachineRainbowDashFlying));
				list.Add(typeof(FieStateMachineRainbowDashEvasion));
				return list;
			}
			return new List<Type>();
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
