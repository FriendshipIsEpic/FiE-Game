using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/Power/RisingSunSummonArrowChild")]
	public class FieEmitObjectRisingSunSummonArrowChild : FieEmittableObjectBase
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
		private GameObject effectModel;

		[SerializeField]
		private SmoothTrail trail;

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

		private Vector3 _initEffectModelScale = Vector3.zero;

		public int childId;

		public void Awake()
		{
			_initEffectModelScale = effectModel.transform.localScale;
		}

		public override void awakeEmitObject()
		{
			trail.ClearSystem(emitState: true);
			_velocityTweener.InitTweener(SummonArrowVelocityAccelTime, 0f, SummonArrowVelocityMax);
			_tiltForceTweener.InitTweener(SummonArrowTiltDuration, 1f, 0f);
			effectModel.transform.localScale = _initEffectModelScale;
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				if (!_isInitRandomTilt)
				{
					initRandomTilt();
				}
				float num = _tiltForceTweener.UpdateParameterFloat(Time.deltaTime);
				Vector3 a = directionalVec * (1f - num);
				Vector3 b = _randomTiltVec * num;
				a += b;
				a.Normalize();
				if (targetTransform != null)
				{
					if (_initEnemyDistance <= 0f)
					{
						_initEnemyDistance = Vector3.Distance(base.transform.position, targetTransform.position);
					}
					if (_totalMoveDistance > _initEnemyDistance * 1.25f)
					{
						_isEndHorming = true;
						directionalVec = _lastDirectionalVec;
					}
					if (!_isEndHorming)
					{
						Vector3 a2 = Quaternion.LookRotation(targetTransform.position - base.transform.position) * Vector3.forward;
						a *= num;
						a += a2 * (1f - num);
						a.Normalize();
						_lastDirectionalVec = a;
					}
				}
				Vector3 vector = a * _velocityTweener.UpdateParameterFloat(Time.deltaTime) * Time.deltaTime;
				_totalMoveDistance += Vector3.Distance(base.transform.position, base.transform.position + vector);
				base.transform.position += vector;
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= SummonArrowChildDuration)
				{
					destoryEmitObject(SummonArrowDestroyDuration);
					trail.Emit = false;
					trail.ClearSystem(emitState: false);
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
			Vector3 a2 = Quaternion.AngleAxis((float)childId * 30f, directionalVec) * Vector3.up;
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
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRisingSunHitEffectSmall>(base.transform, Vector3.zero);
				addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				trail.Emit = false;
				destoryEmitObject(SummonArrowDestroyDuration);
				effectModel.transform.localScale = Vector3.zero;
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