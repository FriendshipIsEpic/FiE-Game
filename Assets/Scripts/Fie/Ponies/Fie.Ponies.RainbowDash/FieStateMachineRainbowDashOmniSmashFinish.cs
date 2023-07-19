using Fie.Camera;
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
	public class FieStateMachineRainbowDashOmniSmashFinish : FieStateMachineGameCharacterBase
	{
		private enum AttackState
		{
			ACTIVATE,
			PREPARE,
			ATTACKING,
			COOLDOWN,
			FINISHED
		}

		private const float PREPARE_DURATION = 0.3f;

		private const float PREPARE_FLYING_DISTANCE = 1.5f;

		private const float ATTAKING_DRATION = 0.2f;

		private const float MAXIMUM_DROP_DISTANCE = 20f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private bool _isEnd;

		private bool _isFinished;

		private AttackState _attackState;

		private float _attackDealyCounter;

		private Vector3 _initInputVec = Vector3.zero;

		private Vector3 _movintTargetPos = Vector3.zero;

		private Tweener<TweenTypesOutSine> _movingTweener = new Tweener<TweenTypesOutSine>();

		private Tweener<TweenTypesOutSine> _finishingTweener = new Tweener<TweenTypesOutSine>();

		private FieEmitObjectRainbowDashDoublePaybackAttackingEffect _attackingEffect;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieRainbowDash)
			{
				FieRainbowDash rainbowDash = gameCharacter as FieRainbowDash;
				switch (_attackState)
				{
				case AttackState.ACTIVATE:
				{
					rainbowDash.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_USED_FRIENDSHIP_ABILITY));
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashActivationEffect>(rainbowDash.centerTransform, Vector3.up);
					TrackEntry trackEntry = rainbowDash.animationManager.SetAnimation(40, isLoop: false, isForceSet: true);
					if (trackEntry == null)
					{
						_nextState = typeof(FieStateMachineCommonIdle);
						_isEnd = true;
					}
					else
					{
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "finished")
							{
								SetAttackState(rainbowDash);
								_attackState = AttackState.ATTACKING;
							}
						};
						if (FieManagerBehaviour<FieGameCameraManager>.I.gameCamera != null)
						{
							FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setOffsetTransition(rainbowDash, new FieCameraOffset(new FieCameraOffset.FieCameraOffsetParam(new Vector3(0f, 0f, 3f), new Vector3(0f, 0f, 0f), 40f), 1f, 0.5f, 1f));
						}
						GDESkillTreeData skill = rainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_OMNISMASH_LV4_LET_ME_SHOW_YOU_MY_TRUE_POWER);
						if (skill != null)
						{
							rainbowDash.damageSystem.calcShieldDirect(0f - rainbowDash.healthStats.maxShield);
							rainbowDash.damageSystem.calcHitPoitDirect((0f - rainbowDash.healthStats.maxHitPoint) * 0.99f);
							rainbowDash.damageSystem.setRegenerateDelay(rainbowDash.healthStats.regenerateDelay * (float)rainbowDash.omniSmashAttackingCount);
							rainbowDash.abilitiesContainer.IncreaseOrReduceCooldownAll(skill.Value2 * (float)rainbowDash.omniSmashAttackingCount);
						}
						rainbowDash.submeshObject.enabled = true;
						_movingTweener.InitTweener(0.3f, rainbowDash.transform.position, rainbowDash.transform.position + Vector3.up * 1.5f);
						_attackState = AttackState.PREPARE;
					}
					break;
				}
				case AttackState.PREPARE:
					if (!_movingTweener.IsEnd())
					{
						rainbowDash.position = _movingTweener.UpdateParameterVec3(Time.deltaTime);
					}
					break;
				case AttackState.ATTACKING:
					if (!_finishingTweener.IsEnd())
					{
						rainbowDash.position = _finishingTweener.UpdateParameterVec3(Time.deltaTime);
					}
					break;
				}
			}
		}

		private void SetAttackState(FieRainbowDash rainbowDash)
		{
			if (!(rainbowDash == null))
			{
				TrackEntry trackEntry = rainbowDash.animationManager.SetAnimation(41, isLoop: false, isForceSet: true);
				if (trackEntry == null)
				{
					_nextState = typeof(FieStateMachineCommonIdle);
					_isEnd = true;
				}
				else
				{
					trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
					{
						if (e.Data.Name == "fire")
						{
							FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashImpactEffect>(rainbowDash.transform, Vector3.forward);
						}
						if (e.Data.Name == "finished")
						{
							if (rainbowDash.omniSmashAttackingCount > 2)
							{
								float scale = (float)rainbowDash.omniSmashAttackingCount * 0.15f;
								FieEmitObjectRainbowDashOmniSmashExplosion fieEmitObjectRainbowDashOmniSmashExplosion = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashExplosion>(rainbowDash.transform, Vector3.forward, null, rainbowDash);
								if (fieEmitObjectRainbowDashOmniSmashExplosion != null)
								{
									fieEmitObjectRainbowDashOmniSmashExplosion.SetScale(scale);
								}
								FieEmitObjectRainbowDashOmniSmashExplosionEffect fieEmitObjectRainbowDashOmniSmashExplosionEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashExplosionEffect>(rainbowDash.transform, Vector3.forward, null, rainbowDash);
								if (fieEmitObjectRainbowDashOmniSmashExplosionEffect != null)
								{
									fieEmitObjectRainbowDashOmniSmashExplosionEffect.SetScale(scale);
								}
							}
							GDESkillTreeData skill2 = rainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_OMNISMASH_LV4_NOT_A_BIG_DEAL);
							if (skill2 != null)
							{
								_isFinished = true;
							}
							rainbowDash.isEnableGravity = true;
						}
					};
					trackEntry.Complete += delegate
					{
						rainbowDash.isEnableCollider = true;
						TrackEntry trackEntry2 = rainbowDash.animationManager.SetAnimation(42, isLoop: false, isForceSet: true);
						GDESkillTreeData skill = rainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_OMNISMASH_LV3_1);
						if (skill != null)
						{
							rainbowDash.damageSystem.AddAttackMagni(skill.ID, skill.Value2, skill.Value1);
						}
						trackEntry2.Complete += delegate
						{
							_isEnd = true;
						};
					};
					Vector3 position = rainbowDash.transform.position;
					Vector3 down = Vector3.down;
					if (rainbowDash.externalInputForce > 0.2f)
					{
						Vector3 externalInputVector = rainbowDash.externalInputVector;
						down.x = externalInputVector.x * 0.5f;
					}
					down.Normalize();
					position = rainbowDash.centerTransform.position - down * 20f;
					int layerMask = 512;
					if (Physics.Raycast(rainbowDash.centerTransform.position, down, out RaycastHit hitInfo, 20f, layerMask) && hitInfo.collider.tag == "Floor")
					{
						position = hitInfo.point;
					}
					if (down != Vector3.down)
					{
						rainbowDash.setFlipByVector(new Vector3(down.x, 0f, 0f).normalized);
					}
					_finishingTweener.InitTweener(0.2f, rainbowDash.transform.position, position);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashRainboomEffect>(rainbowDash.centerTransform, down);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashTrailEffect>(rainbowDash.centerTransform, down);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashFinishHit>(rainbowDash.centerTransform, down, null, rainbowDash);
					rainbowDash.emotionController.RestartAutoAnimation();
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				fieRainbowDash.emotionController.StopAutoAnimation();
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
				fieRainbowDash.submeshObject.enabled = true;
			}
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list;
			if (!_isFinished)
			{
				list = new List<Type>();
				list.Add(typeof(FieStateMachineRainbowDashOmniSmashLoop));
				return list;
			}
			list = new List<Type>();
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
