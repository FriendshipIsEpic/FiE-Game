using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisPenetrateShot")]
	public class FieEmitObjectQueenChrysalisPenetrateShot : FieEmittableObjectBase
	{
		[SerializeField]
		private float shotDuration = 1f;

		[SerializeField]
		private float shotVelocityMax = 4f;

		[SerializeField]
		private float shotAccelTime = 0.1f;

		[SerializeField]
		private float shoutDestroyDuration = 0.5f;

		[SerializeField]
		private float PenetrateShotPysicalForce = 5f;

		[SerializeField]
		private GameObject effectModel;

		[SerializeField]
		private Light shotLight;

		[SerializeField]
		private PKFxFX trail;

		private Tweener<TweenTypesInSine> _velocityTweener = new Tweener<TweenTypesInSine>();

		private Tweener<TweenTypesOutSine> _scaleTweener = new Tweener<TweenTypesOutSine>();

		private Vector3 _lastDirectionalVec = Vector3.zero;

		private Vector3 _initDirectionalVec = Vector3.zero;

		private float _lifeTimeCount;

		private float _minDistance = 3.40282347E+38f;

		private bool _isEndUpdate;

		private Vector3 _initEffectModelScale = Vector3.zero;

		public void Awake()
		{
			_initEffectModelScale = effectModel.transform.localScale;
		}

		public override void awakeEmitObject()
		{
			_velocityTweener.InitTweener(shotAccelTime, 0f, shotVelocityMax);
			_scaleTweener.InitTweener(shotAccelTime, Vector3.zero, Vector3.one);
			_initDirectionalVec = directionalVec;
			if (targetTransform != null && targetTransform.root != null)
			{
				Vector3 a = targetTransform.position;
				FieGameCharacter component = targetTransform.root.GetComponent<FieGameCharacter>();
				if (component != null && component.groundState == FieObjectGroundState.Grounding)
				{
					int layerMask = 512;
					if (Physics.Raycast(targetTransform.position + Vector3.up * 2f, Vector3.down, out RaycastHit hitInfo, 1024f, layerMask) && hitInfo.collider.tag == "Floor")
					{
						a = hitInfo.point;
					}
				}
				_initDirectionalVec = a - base.transform.position;
				_initDirectionalVec.Normalize();
				directionalVec = _initDirectionalVec;
			}
			if (trail != null)
			{
				trail.StopEffect();
				trail.StartEffect();
			}
			effectModel.transform.localScale = _initEffectModelScale;
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				Vector3 vector = directionalVec * _velocityTweener.UpdateParameterFloat(Time.deltaTime);
				base.transform.position += vector * Time.deltaTime;
				base.transform.localScale = _scaleTweener.UpdateParameterVec3(Time.deltaTime);
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= shotDuration)
				{
					trail.StopEffect();
					destoryEmitObject(shoutDestroyDuration);
					_isEndUpdate = true;
				}
				base.transform.rotation = Quaternion.LookRotation(vector);
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate)
			{
				if (collider.gameObject.tag == getHostileTagString())
				{
					FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, getDefaultDamageObject(), isPenetration: true);
					if (fieGameCharacter != null)
					{
						Vector3 normalized = directionalVec.normalized;
						normalized.y = 0.5f;
						fieGameCharacter.resetMoveForce();
						fieGameCharacter.setMoveForce(normalized * PenetrateShotPysicalForce, 0f, useRound: false);
					}
				}
				if (collider.gameObject.tag == "Floor")
				{
					FieEmitObjectQueenChrysalisPenetrateShotBurst fieEmitObjectQueenChrysalisPenetrateShotBurst = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisPenetrateShotBurst>(base.transform, Vector3.zero);
					if (fieEmitObjectQueenChrysalisPenetrateShotBurst != null)
					{
						fieEmitObjectQueenChrysalisPenetrateShotBurst.setAllyTag(getArrayTag());
						fieEmitObjectQueenChrysalisPenetrateShotBurst.setHostileTag(getHostileTag());
					}
					FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_BIG);
					if (trail != null)
					{
						trail.StopEffect();
					}
					shotLight.intensity = 0f;
					effectModel.transform.localScale = Vector3.zero;
					destoryEmitObject(shotDuration - _lifeTimeCount);
					_isEndUpdate = true;
				}
			}
		}

		private void OnDisable()
		{
			if (trail != null)
			{
				trail.StopEffect();
				effectModel.transform.localScale = Vector3.zero;
			}
		}
	}
}
