using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightShinyArrow")]
	public class FieEmitObjectTwilightShinyArrow : FieEmittableObjectBase
	{
		[SerializeField]
		private float shinyArrowDuration = 1f;

		[SerializeField]
		private float ShinyArrowVelocityMax = 4f;

		[SerializeField]
		private float ShinyArrowAccelTime = 0.1f;

		[SerializeField]
		private float ShinyArrowPysicalForce = 5f;

		[SerializeField]
		private float ShinyArrowDestroyDuration = 0.5f;

		[SerializeField]
		private GameObject effectModel;

		[SerializeField]
		private FieOnetimeLight shotLight;

		[SerializeField]
		private PKFxFX trail;

		private const float HORMING_DISTANCE_MAX = 5f;

		private Tweener<TweenTypesInSine> _velocityTweener = new Tweener<TweenTypesInSine>();

		private Tweener<TweenTypesOutSine> _scaleTweener = new Tweener<TweenTypesOutSine>();

		private Vector3 _velocityVec = Vector3.zero;

		private Vector3 _lastDirectionalVec = Vector3.zero;

		private Vector3 _scale = Vector3.zero;

		private Vector3 _initDirectionalVec = Vector3.zero;

		private float _lifeTimeCount;

		private float _minDistance = 3.40282347E+38f;

		private bool _isEndUpdate;

		private bool _isInitializedHormingVector;

		private Vector3 _initEffectModelScale = Vector3.zero;

		public void Awake()
		{
			_initEffectModelScale = effectModel.transform.localScale;
			trail.StopEffect();
		}

		public override void awakeEmitObject()
		{
			_velocityTweener.InitTweener(ShinyArrowAccelTime, 0f, ShinyArrowVelocityMax);
			_scaleTweener.InitTweener(ShinyArrowAccelTime, Vector3.zero, Vector3.one);
			_initDirectionalVec = directionalVec;
			if (targetTransform != null)
			{
				_initDirectionalVec = (targetTransform.position - base.transform.position).normalized + directionalVec * 4f;
				_initDirectionalVec.Normalize();
			}
			effectModel.transform.localScale = _initEffectModelScale;
			trail.StopEffect();
			trail.StartEffect();
			base.emitObjectType = EmitObjectType.RANGED_ATTACK_REFLECTIVE;
			if (base.ownerCharacter != null && base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_ATTACK_PASSIVE_LV4_AGRESSIVE_CASTING) != null)
			{
				base.emitObjectType = EmitObjectType.RANGED_ATTACK;
			}
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				Vector3 initDirectionalVec = _initDirectionalVec;
				if (targetTransform != null)
				{
					float num = Vector3.Distance(targetTransform.position, base.transform.position);
					float d = 1f;
					Vector3 vector = Quaternion.LookRotation(targetTransform.position - base.transform.position) * Vector3.forward;
					float num2 = Vector3.Dot(initDirectionalVec, vector);
					if (num2 < 0.5f)
					{
						d = 0f;
					}
					initDirectionalVec *= 0.2f;
					initDirectionalVec += vector * d;
					initDirectionalVec.Normalize();
					if (!_isInitializedHormingVector)
					{
						directionalVec = initDirectionalVec;
						_isInitializedHormingVector = true;
					}
				}
				Vector3 vector2 = directionalVec * _velocityTweener.UpdateParameterFloat(Time.deltaTime);
				base.transform.position += vector2 * Time.deltaTime;
				base.transform.localScale = _scaleTweener.UpdateParameterVec3(Time.deltaTime);
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= shinyArrowDuration)
				{
					trail.StopEffect();
					destoryEmitObject(ShinyArrowDestroyDuration);
					_isEndUpdate = true;
				}
				base.transform.rotation = Quaternion.LookRotation(vector2);
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && (collider.gameObject.tag == getHostileTagString() || collider.gameObject.tag == "Floor"))
			{
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightHitEffectMiddle>(base.transform, Vector3.zero);
				if (collider.gameObject.tag == getHostileTagString())
				{
					FieManagerBehaviour<FieGameCameraManager>.I.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_MIDDLE);
				}
				FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (fieGameCharacter != null)
				{
					fieGameCharacter.resetMoveForce();
					fieGameCharacter.setMoveForce((directionalVec + Vector3.up).normalized * ShinyArrowPysicalForce, 0f, useRound: false);
				}
				trail.StopEffect();
				shotLight.Kill();
				effectModel.transform.localScale = Vector3.zero;
				destoryEmitObject(shinyArrowDuration - _lifeTimeCount);
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