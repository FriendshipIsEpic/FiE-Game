using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Changeling/Power/ChangelingShot")]
	public class FieEmitObjectChangelingShot : FieEmittableObjectBase
	{
		[SerializeField]
		private float CHANGELING_SINGLE_SHOT_DURATION = 2f;

		[SerializeField]
		private float CHANGELING_SINGLE_SHOT_VELOCITY_MAX = 2f;

		[SerializeField]
		private float CHANGELING_SINGLE_SHOT_VELOCITY_ACCEL_TIME = 2f;

		[SerializeField]
		private float CHANGELING_SINGLE_SHOT_DESTORY_DURATION = 0.7f;

		[SerializeField]
		private GameObject cilinder;

		[SerializeField]
		private SmoothTrail trail;

		private const float HORMING_DISTANCE_MAX = 10f;

		private Tweener<TweenTypesInSine> _velocityTweener = new Tweener<TweenTypesInSine>();

		private Tweener<TweenTypesOutSine> _scaleTweener = new Tweener<TweenTypesOutSine>();

		private Vector3 _velocityVec = Vector3.zero;

		private Vector3 _scale = Vector3.zero;

		private Vector3 _additionalVelocity = Vector3.zero;

		private Vector3 _initDirectionalVec = Vector3.zero;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		private Vector3 _initEffectModelScale = Vector3.zero;

		public void Awake()
		{
			_initEffectModelScale = cilinder.transform.localScale;
		}

		public override void awakeEmitObject()
		{
			_velocityTweener.InitTweener(CHANGELING_SINGLE_SHOT_VELOCITY_ACCEL_TIME, 0f, CHANGELING_SINGLE_SHOT_VELOCITY_MAX);
			_scaleTweener.InitTweener(CHANGELING_SINGLE_SHOT_VELOCITY_ACCEL_TIME, Vector3.zero, Vector3.one);
			_initDirectionalVec = directionalVec;
			if (targetTransform != null)
			{
				_initDirectionalVec = (targetTransform.position - base.transform.position).normalized;
			}
			directionalVec = _initDirectionalVec;
			cilinder.transform.localScale = _initEffectModelScale;
			trail.ClearSystem(emitState: true);
		}

		public void setAdditionalVelocity(Vector3 additionalVelocity)
		{
			_additionalVelocity = additionalVelocity;
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				Vector3 vector = directionalVec * _velocityTweener.UpdateParameterFloat(Time.deltaTime);
				base.transform.position += vector;
				base.transform.localScale = _scaleTweener.UpdateParameterVec3(Time.deltaTime);
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= CHANGELING_SINGLE_SHOT_DURATION)
				{
					destoryEmitObject(CHANGELING_SINGLE_SHOT_DESTORY_DURATION);
					trail.Emit = false;
					trail.ClearSystem(emitState: false);
					cilinder.transform.localScale = Vector3.zero;
					_isEndUpdate = true;
				}
				base.transform.rotation = Quaternion.LookRotation(vector);
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && (collider.gameObject.tag == getHostileTagString() || collider.gameObject.tag == "Floor"))
			{
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingHitEffectSmall>(base.transform, Vector3.zero);
				addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				trail.Emit = false;
				destoryEmitObject(CHANGELING_SINGLE_SHOT_DESTORY_DURATION);
				cilinder.transform.localScale = Vector3.zero;
				_isEndUpdate = true;
			}
		}

		private void OnDisable()
		{
			if (trail != null)
			{
				trail.ClearSystem(emitState: false);
			}
		}
	}
}
