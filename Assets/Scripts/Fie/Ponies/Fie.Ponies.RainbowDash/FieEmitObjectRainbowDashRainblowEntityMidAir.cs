using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System.Collections;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashRainblowEntityMidAir")]
	public class FieEmitObjectRainbowDashRainblowEntityMidAir : FieEmittableObjectBase
	{
		[SerializeField]
		private float rainblowDuration = 7f;

		[SerializeField]
		private float rainblowDestroyDuration = 2f;

		[SerializeField]
		private PKFxFX _rainblowFx;

		[SerializeField]
		private AudioSource _vortexSound;

		[SerializeField]
		private AnimationCurve _damageRatePerLifeRatio;

		[SerializeField]
		private AnimationCurve _StaggerDamageRatePerLifeRatio;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		private Tweener<TweenTypesOutSine> _speedDamperTweener = new Tweener<TweenTypesOutSine>();

		private float _healingDelay;

		private float _healingRate;

		private float _healingRemSec;

		private FieStatusEffectsTimeScaler _timeScalerStatusEffectEntity;

		private IEnumerator stopAudio()
		{
			if (_vortexSound.volume > 0f)
			{
				yield return (object)new WaitForSeconds(0.05f);
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public override void awakeEmitObject()
		{
			if (!(_rainblowFx == null) && !(_vortexSound == null))
			{
				_rainblowFx.StopEffect();
				_rainblowFx.SetAttribute(new PKFxManager.Attribute("TornadeLife", rainblowDuration - rainblowDestroyDuration));
				_rainblowFx.StartEffect();
				_vortexSound.volume = 0.5f;
				if (base.ownerCharacter != null)
				{
					GDESkillTreeData skill = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.LOYALTY_RAINBLOW_LV4_WINGPOWER_EMITTER);
					if (skill != null)
					{
						_healingDelay = skill.Value1;
						_healingRate = skill.Value2;
					}
				}
				if (_timeScalerStatusEffectEntity != null)
				{
					_timeScalerStatusEffectEntity.isActive = false;
				}
				GDESkillTreeData skill2 = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.LOYALTY_RAINBLOW_LV4_STICKY_TORNADO);
				if (skill2 != null)
				{
					if (_timeScalerStatusEffectEntity == null)
					{
						_timeScalerStatusEffectEntity = base.gameObject.AddComponent<FieStatusEffectsTimeScaler>();
					}
					_timeScalerStatusEffectEntity.isActive = true;
					_timeScalerStatusEffectEntity.duration = skill2.Value1;
					_timeScalerStatusEffectEntity.targetTimeScale = skill2.Value2;
				}
			}
		}

		public void Update()
		{
			base.transform.rotation = Quaternion.identity;
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= rainblowDuration - rainblowDestroyDuration)
				{
					stopParticleEmitting();
					destoryEmitObject(rainblowDestroyDuration);
					_isEndUpdate = true;
				}
				if (_healingRemSec <= 0f)
				{
					if (base.ownerCharacter != null && _healingRate > 0f)
					{
						base.ownerCharacter.damageSystem.Regen((base.ownerCharacter.healthStats.maxHitPoint + base.ownerCharacter.healthStats.maxShield) * _healingRate);
					}
					_healingRemSec = _healingDelay;
				}
				else
				{
					_healingRemSec -= Time.deltaTime;
				}
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			if (!_isEndUpdate && collider.gameObject.tag == getHostileTagString())
			{
				FieDamage defaultDamageObject = getDefaultDamageObject();
				float num = _damageRatePerLifeRatio.Evaluate(Mathf.Clamp(_lifeTimeCount / (rainblowDuration - rainblowDestroyDuration), 0f, 1f));
				float num2 = _StaggerDamageRatePerLifeRatio.Evaluate(Mathf.Clamp(_lifeTimeCount / (rainblowDuration - rainblowDestroyDuration), 0f, 1f));
				defaultDamageObject.damage *= num;
				defaultDamageObject.stagger *= num2;
				FieGameCharacter x = addDamageToCollisionCharacter(collider, defaultDamageObject);
				if (x != null)
				{
					FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_SMALL);
				}
			}
		}

		private void stopParticleEmitting()
		{
			_rainblowFx.StopEffect();
			StartCoroutine(stopAudio());
		}
	}
}
