using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisHormingShot")]
	public class FieEmitObjectQueenChrysalisHormingShot : FieEmittableObjectBase
	{
		[SerializeField]
		private float HormingShotDuration = 2f;

		[SerializeField]
		private float HormingShotVelocityMax = 2f;

		[SerializeField]
		private float HormingShotVelocityAccelTime = 0.3f;

		[SerializeField]
		private float HormingShotDestroyDuration = 1f;

		[SerializeField]
		private float HormingShotTiltDuration = 1f;

		[SerializeField]
		private float HormingDuration = 1.5f;

		[SerializeField]
		private float TiltRotPerSecDegree = 120f;

		[SerializeField]
		private GameObject effectModel;

		[SerializeField]
		private PKFxFX trail;

		private const float RANDOM_TILT_DISTANCE = 2f;

		private const float HORMING_DISTANCE_MAX = 10f;

		private Tweener<TweenTypesInSine> _velocityTweener = new Tweener<TweenTypesInSine>();

		private Tweener<TweenTypesInSine> _tiltForceTweener = new Tweener<TweenTypesInSine>();

		private Vector3 _velocityVec = Vector3.zero;

		private Vector3 _lastDirectionalVec = Vector3.zero;

		private Quaternion _randomTiltRot = Quaternion.identity;

		private float _lifeTimeCount;

		private bool _isInitRandomTilt;

		private float _totalMoveDistance;

		private float _initEnemyDistance = -3.40282347E+38f;

		private bool _isEndUpdate;

		private bool _isEndHorming;

		private float _initHormingDuration;

		private float _tiltDegRot;

		private float _sinWave;

		private Vector3 _initEffectModelScale = Vector3.zero;

		private Vector3 _latestDirectionalVec = Vector3.zero;

		public int childId;

		public void Awake()
		{
			_initEffectModelScale = effectModel.transform.localScale;
			base.reflectEvent += delegate
			{
				trail.StopEffect();
				destoryEmitObject(HormingShotDestroyDuration);
				effectModel.transform.localScale = Vector3.zero;
				_isEndUpdate = true;
			};
		}

		public override void awakeEmitObject()
		{
			trail.StopEffect();
			trail.StartEffect();
			_velocityTweener.InitTweener(HormingShotVelocityAccelTime, 0f, HormingShotVelocityMax);
			_tiltForceTweener.InitTweener(HormingShotTiltDuration, 1f, 0f);
			effectModel.transform.localScale = _initEffectModelScale;
			if (targetTransform != null)
			{
				directionalVec = (targetTransform.position - base.transform.position).normalized;
			}
			_latestDirectionalVec = directionalVec;
			base.transform.rotation = Quaternion.LookRotation(directionalVec);
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
				_tiltDegRot = Mathf.Repeat(_tiltDegRot + TiltRotPerSecDegree * Time.deltaTime, 360f);
				Vector3 vector = base.transform.rotation * Vector3.forward;
				Vector3 a = base.transform.position + vector * 2f;
				Vector3 a2 = Quaternion.AngleAxis(_tiltDegRot, vector) * Vector3.up;
				a2 *= 2f;
				a += a2 * 2f;
				_randomTiltRot = Quaternion.LookRotation(a - base.transform.position);
				Vector3 forward = vector;
				if (targetTransform != null)
				{
					forward = (directionalVec = Vector3.Lerp(vector, (targetTransform.position - base.transform.position).normalized, 3f * Time.deltaTime));
				}
				_sinWave += 120f * Time.deltaTime;
				Quaternion a3 = Quaternion.LookRotation(forward);
				float num = _tiltForceTweener.UpdateParameterFloat(Time.deltaTime);
				base.transform.rotation = Quaternion.Lerp(a3, _randomTiltRot, Mathf.Min(Mathf.Abs(Mathf.Sin(_sinWave)) * 0.2f, 1f) * num);
				Vector3 vector2 = base.transform.rotation * Vector3.forward * _velocityTweener.UpdateParameterFloat(Time.deltaTime) * Time.deltaTime;
				base.transform.position += vector2;
				_totalMoveDistance += Vector3.Distance(base.transform.position, base.transform.position + vector2);
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= HormingShotDuration)
				{
					destoryEmitObject(HormingShotDestroyDuration);
					trail.StopEffect();
					effectModel.transform.localScale = Vector3.zero;
					_isEndUpdate = true;
				}
			}
		}

		private void initRandomTilt()
		{
			_tiltDegRot = Random.Range(0f, 360f);
			_isInitRandomTilt = true;
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && (collider.gameObject.tag == getHostileTagString() || collider.gameObject.tag == "Floor"))
			{
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHitEffectSmall>(base.transform, Vector3.zero);
				addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				trail.StopEffect();
				destoryEmitObject(HormingShotDestroyDuration);
				effectModel.transform.localScale = Vector3.zero;
				_isEndUpdate = true;
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
