using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightStunningSummonArrowChild")]
	public class FieEmitObjectTwilightStunningSummonArrowChild : FieEmittableObjectBase
	{
		[SerializeField]
		private float SummonArrowChildDuration = 1.5f;

		[SerializeField]
		private float SummonArrowVelocityMax = 2f;

		[SerializeField]
		private float SummonArrowVelocityAccelTime = 0.3f;

		[SerializeField]
		private float SummonArrowDestroyDuration = 0.7f;

		[SerializeField]
		private float SummonArrowTiltDuration = 0.5f;

		[SerializeField]
		private float HormingDuration = 1.5f;

		[SerializeField]
		private GameObject effectModel;

		[SerializeField]
		private PKFxFX trail;

		private const float RANDOM_TILT_DISTANCE = 2f;

		private const float HORMING_DISTANCE_MAX = 10f;

		private Tweener<TweenTypesInSine> _velocityTweener = new Tweener<TweenTypesInSine>();

		private Tweener<TweenTypesInSine> _tiltForceTweener = new Tweener<TweenTypesInSine>();

		private Vector3 _velocityVec = Vector3.zero;

		private Vector3 _randomTiltVec = Vector3.zero;

		private Vector3 _lastDirectionalVec = Vector3.zero;

		private float _lifeTimeCount;

		private bool _isInitRandomTilt;

		private float _totalMoveDistance;

		private float _initEnemyDistance = -3.40282347E+38f;

		private bool _isEndUpdate;

		private bool _isEndHorming;

		private float _initHormingDuration;

		private Vector3 _initEffectModelScale = Vector3.zero;

		private Vector3 _latestDirectionalVec = Vector3.zero;

		public int childId;

		public void Awake()
		{
			_initEffectModelScale = effectModel.transform.localScale;
			trail.StopEffect();
		}

		public override void awakeEmitObject()
		{
			trail.StopEffect();
			trail.StartEffect();
			_velocityTweener.InitTweener(SummonArrowVelocityAccelTime, 0f, SummonArrowVelocityMax);
			_tiltForceTweener.InitTweener(SummonArrowTiltDuration, 1f, 0f);
			effectModel.transform.localScale = _initEffectModelScale;
			_latestDirectionalVec = (directionalVec = new Vector3((float)Random.Range(-1, 1), (float)Random.Range(-1, 1), (float)Random.Range(-1, 1)));
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				if (!_isInitRandomTilt)
				{
					initRandomTilt();
				}
				if (directionalVec != _latestDirectionalVec)
				{
					_lastDirectionalVec = (_latestDirectionalVec = directionalVec);
				}
				float num = _tiltForceTweener.UpdateParameterFloat(Time.deltaTime);
				Vector3 a = directionalVec * (1f - num);
				Vector3 b = _randomTiltVec * num;
				a += b;
				a.Normalize();
				if (base.ownerCharacter != null)
				{
					targetTransform = base.ownerCharacter.detector.getLockonEnemyTransform(isCenter: true);
				}
				if (targetTransform != null)
				{
					if (_initEnemyDistance <= 0f)
					{
						_initEnemyDistance = Vector3.Distance(base.transform.position, targetTransform.position);
					}
					Vector3 a2 = Quaternion.LookRotation(targetTransform.position - base.transform.position) * Vector3.forward;
					a *= num;
					a += a2 * (1f - num);
					a.Normalize();
					a = (_lastDirectionalVec = Vector3.Lerp(a, _lastDirectionalVec, Mathf.Clamp(_lifeTimeCount / HormingDuration, 0f, 1f)));
				}
				Vector3 vector = a * _velocityTweener.UpdateParameterFloat(Time.deltaTime) * Time.deltaTime;
				_totalMoveDistance += Vector3.Distance(base.transform.position, base.transform.position + vector);
				base.transform.position += vector;
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= SummonArrowChildDuration)
				{
					destoryEmitObject(SummonArrowDestroyDuration);
					trail.StopEffect();
					effectModel.transform.localScale = Vector3.zero;
					_isEndUpdate = true;
				}
				if (vector != Vector3.zero)
				{
					base.transform.rotation = Quaternion.LookRotation(vector);
				}
			}
		}

		private void initRandomTilt()
		{
			Vector3 a = base.transform.position + directionalVec * 2f;
			Vector3 a2 = Quaternion.AngleAxis(Random.Range(0f, 360f), directionalVec) * Vector3.up;
			a2 *= 2f;
			a += a2 * 2f;
			_randomTiltVec = Quaternion.LookRotation(a - base.transform.position) * Vector3.forward;
			_randomTiltVec.Normalize();
			_isInitRandomTilt = true;
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && (collider.gameObject.tag == getHostileTagString() || collider.gameObject.tag == "Floor"))
			{
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightHitEffectSmall>(base.transform, Vector3.zero);
				addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				trail.StopEffect();
				destoryEmitObject(SummonArrowDestroyDuration);
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
