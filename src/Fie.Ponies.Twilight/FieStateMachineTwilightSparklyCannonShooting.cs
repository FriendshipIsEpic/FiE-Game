using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightSparklyCannonShooting : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			FIRE_START,
			FIRING
		}

		private const string SPARKLY_CANNON_SIGNATURE = "shiny_arrow";

		private const float SPARKLY_CANNON_DEFAULT_DURATION = 2f;

		private const float GROUND_FORCE_TIME = 1.5f;

		private const float GROUND_FORCE = 1.5f;

		private const float SHIELD_CONSUMING_INTERVAL = 0.5f;

		private const float SHIELD_CONSUME_PERCENT = 0.04f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 2f;

		private float _maxEmittionTime;

		private float _timeCount;

		private bool _isSetEndAnim;

		private float _shieldConsumingInterval = 0.5f;

		private float _damegeTakenRate = 1f;

		private float _damageReducionEffectDuration;

		private bool _isDeffensive;

		private bool _isUnstoppable;

		private FieEmitObjectTwilightLaser _laserObject;

		private FieEmitObjectTwilightLaserDust _dustObject;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieTwilight)
			{
				_timeCount += Time.deltaTime;
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				switch (_fireState)
				{
				case FireState.FIRE_START:
					fieTwilight.animationManager.SetAnimation(28, isLoop: false, isForceSet: true);
					_laserObject = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightLaser>(fieTwilight.hornTransform, fieTwilight.flipDirectionVector, fieTwilight.detector.getLockonEnemyTransform(isCenter: true), fieTwilight);
					if (_laserObject != null)
					{
						_maxEmittionTime = (_endTime = _laserObject.getLaserDuration());
					}
					else
					{
						_maxEmittionTime = (_endTime = 2f);
					}
					if (fieTwilight.groundState == FieObjectGroundState.Grounding)
					{
						fieTwilight.addMoveForce(fieTwilight.flipDirectionVector * -100f, 1f, useRound: false);
						fieTwilight.animationManager.SetAnimationChain(25, 26, isLoop: true, isForceSet: true);
						_dustObject = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightLaserDust>(fieTwilight.transform, Vector3.up, null, fieTwilight);
						if (_dustObject != null)
						{
							_dustObject.setDuration(_endTime);
						}
					}
					else
					{
						fieTwilight.animationManager.SetAnimation(20, isLoop: true, isForceSet: true);
						fieTwilight.addMoveForce(fieTwilight.flipDirectionVector * -50f, 1f, useRound: false);
						_nextState = typeof(FieStateMachineTwilightFlying);
						fieTwilight.isEnableGravity = false;
					}
					_fireState = FireState.FIRING;
					fieTwilight.resetMoveForce();
					break;
				case FireState.FIRING:
					if (!_isDeffensive)
					{
						_shieldConsumingInterval -= Time.deltaTime;
						if (_shieldConsumingInterval <= 0f)
						{
							fieTwilight.damageSystem.calcShieldDirect((0f - fieTwilight.healthStats.maxShield) * 0.04f);
							fieTwilight.damageSystem.setRegenerateDelay(fieTwilight.healthStats.regenerateDelay * 0.5f, roundToBigger: true);
							_shieldConsumingInterval = 0.5f;
						}
					}
					break;
				}
				if (_timeCount > _endTime)
				{
					if (!_isSetEndAnim)
					{
						if (fieTwilight.groundState == FieObjectGroundState.Grounding)
						{
							fieTwilight.animationManager.SetAnimation(27);
						}
						TrackEntry trackEntry = fieTwilight.animationManager.SetAnimation(29, isLoop: false, isForceSet: true);
						if (trackEntry != null)
						{
							_endTime += trackEntry.endTime;
						}
						_isSetEndAnim = true;
					}
					else
					{
						_isEnd = true;
					}
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.emotionController.StopAutoAnimation();
				gameCharacter.isEnableAutoFlip = false;
				if (gameCharacter != null)
				{
					GDESkillTreeData skill = gameCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_SPARKLY_CANNON_LV4__DEFFENSIVE_CANNON);
					if (skill != null)
					{
						_damegeTakenRate = skill.Value1;
						_damageReducionEffectDuration = skill.Value2;
						gameCharacter.damageSystem.beforeDamageEvent += this.HealthSystem_beforeDamageEvent;
						_isDeffensive = true;
					}
					else
					{
						GDESkillTreeData skill2 = gameCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_SPARKLY_CANNON_LV4_UNSTOPPABLE_CANNON);
						if (skill2 != null)
						{
							_isUnstoppable = true;
						}
					}
				}
			}
		}

		private void HealthSystem_beforeDamageEvent(FieGameCharacter attacker, ref FieDamage damage)
		{
			damage.damage *= _damegeTakenRate;
			damage.stagger *= _damegeTakenRate;
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.emotionController.RestartAutoAnimation();
				gameCharacter.setGravityRate(1f);
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.isEnableGravity = true;
				gameCharacter.isSpeakable = true;
				float num = Mathf.Min(3f, Mathf.Max(0.05f, _timeCount / Mathf.Max(0.1f, _maxEmittionTime)));
				if (_dustObject != null)
				{
					_dustObject.Stop();
				}
				if (_laserObject != null)
				{
					num = Mathf.Clamp(_timeCount / Mathf.Max(1f, _endTime), 0f, 1f);
					_laserObject.Stop();
				}
				gameCharacter.abilitiesContainer.SetCooldown<FieStateMachineTwilightSparklyCannon>(15f * num);
				if (_isDeffensive)
				{
					gameCharacter.damageSystem.AddDefenceMagni(7, Mathf.Max(0f, 1f - _damegeTakenRate), _damageReducionEffectDuration);
				}
				gameCharacter.damageSystem.beforeDamageEvent -= this.HealthSystem_beforeDamageEvent;
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
			if (!_isUnstoppable)
			{
				list.Add(typeof(FieStateMachinePoniesEvasion));
				list.Add(typeof(FieStateMachineTwilightTeleportation));
				list.Add(typeof(FieStateMachineTwilightJump));
			}
			return list;
		}
	}
}
