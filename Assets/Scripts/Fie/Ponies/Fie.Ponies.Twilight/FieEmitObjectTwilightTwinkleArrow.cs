using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightTwinkleArrow")]
	public class FieEmitObjectTwilightTwinkleArrow : FieEmittableObjectBase
	{
		[SerializeField]
		private float BaseShotDuration = 1.5f;

		[SerializeField]
		private float BaseShotVelocityMax = 2.5f;

		[SerializeField]
		private float BaseShotVelocityAccelTime = 0.1f;

		[SerializeField]
		private float BaseShotDestroyDuration = 0.7f;

		[SerializeField]
		private Light light;

		[SerializeField]
		private GameObject effectModel;

		[SerializeField]
		private PKFxFX trail;

		private const float HORMING_DISTANCE_MAX = 1f;

		private Tweener<TweenTypesInSine> _velocityTweener = new Tweener<TweenTypesInSine>();

		private Tweener<TweenTypesOutSine> _scaleTweener = new Tweener<TweenTypesOutSine>();

		private Vector3 _velocityVec = Vector3.zero;

		private Vector3 _scale = Vector3.zero;

		private Vector3 _additionalVelocity = Vector3.zero;

		private Vector3 _initDirectionalVec = Vector3.zero;

		private float _lifeTimeCount;

		private float _defualtLightIntensity;

		private bool _isEndUpdate;

		private bool _isInitializedHormingVector;

		private Vector3 _initEffectModelScale = Vector3.zero;

		public void Awake()
		{
			_initEffectModelScale = effectModel.transform.localScale;
			effectModel.transform.localScale = _initEffectModelScale;
			if (light != null)
			{
				_defualtLightIntensity = light.intensity;
			}
			trail.StopEffect();
		}

		public override void awakeEmitObject()
		{
			_velocityTweener.InitTweener(BaseShotVelocityAccelTime, 0f, BaseShotVelocityMax);
			_scaleTweener.InitTweener(BaseShotVelocityAccelTime, Vector3.zero, Vector3.one);
			trail.StopEffect();
			trail.StartEffect();
			_initDirectionalVec = directionalVec;
			if (targetTransform != null)
			{
				_initDirectionalVec = (targetTransform.position - base.transform.position).normalized + directionalVec * 4f;
				_initDirectionalVec.Normalize();
			}
			if (light != null)
			{
				light.intensity = _defualtLightIntensity;
			}
			if (effectModel != null)
			{
				effectModel.transform.localScale = _initEffectModelScale;
			}
			base.emitObjectType = EmitObjectType.RANGED_ATTACK_REFLECTIVE;
			if (base.ownerCharacter != null && base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_ATTACK_PASSIVE_LV4_AGRESSIVE_CASTING) != null)
			{
				base.emitObjectType = EmitObjectType.RANGED_ATTACK;
			}
		}

		public void setAdditionalVelocity(Vector3 additionalVelocity)
		{
			_additionalVelocity = additionalVelocity;
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				Vector3 directionalVec = base.directionalVec;
				if (targetTransform != null)
				{
					float num = Vector3.Distance(targetTransform.position, base.transform.position);
					float num2 = num / 1f;
					Vector3 vector = Quaternion.LookRotation(targetTransform.position - base.transform.position) * Vector3.forward;
					float num3 = Mathf.Max(Vector3.Dot(directionalVec, vector), 0f);
					num2 *= num3 * 0.5f;
					vector.z *= 1.2f;
					directionalVec += vector * num2;
					directionalVec.Normalize();
					if (!_isInitializedHormingVector)
					{
						base.directionalVec = directionalVec;
						_isInitializedHormingVector = true;
					}
				}
				Vector3 vector2 = base.directionalVec * _velocityTweener.UpdateParameterFloat(Time.deltaTime);
				base.transform.position += vector2 * Time.deltaTime;
				base.transform.localScale = _scaleTweener.UpdateParameterVec3(Time.deltaTime);
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= BaseShotDuration)
				{
					destoryEmitObject(BaseShotDestroyDuration);
					trail.StopEffect();
					_isEndUpdate = true;
				}
				base.transform.rotation = Quaternion.LookRotation(vector2);
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && (collider.gameObject.tag == getHostileTagString() || collider.gameObject.tag == "Floor"))
			{
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightHitEffectSemiMiddle>(base.transform, Vector3.zero);
				if (collider.gameObject.tag == getHostileTagString())
				{
					FieManagerBehaviour<FieGameCameraManager>.I.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_SMALL);
				}
				addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				destoryEmitObject(BaseShotDestroyDuration);
				light.intensity = 0f;
				trail.StopEffect();
				effectModel.transform.localScale = Vector3.zero;
				_isEndUpdate = true;
			}
		}

		private void OnDisable()
		{
			if (trail != null)
			{
				trail.StopEffect();
			}
		}
	}
}
