using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.Flightling
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Flightling/Power/FlightlingShot")]
	public class FieEmitObjectFlightlingShot : FieEmittableObjectBase
	{
		[SerializeField]
		private float FlightlingShotDuration = 3f;

		[SerializeField]
		private float FlightlingShotVelocityMax = 1f;

		[SerializeField]
		private float FlightlingShotVelocityAccelTime = 2f;

		[SerializeField]
		private float FlightlingShotDestroyDuration = 0.7f;

		[SerializeField]
		private float FlightlingShotTiltDuration = 1f;

		[SerializeField]
		private GameObject effectModel;

		[SerializeField]
		private SmoothTrail trail;

		private const float RANDOM_TILT_DISTANCE = 0.2f;

		private const float HORMING_DISTANCE_MAX = 1f;

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

		public void Awake()
		{
			_initEffectModelScale = effectModel.transform.localScale;
		}

		public override void awakeEmitObject()
		{
			_velocityTweener.InitTweener(FlightlingShotVelocityAccelTime, 0f, FlightlingShotVelocityMax);
			_tiltForceTweener.InitTweener(FlightlingShotTiltDuration, 1f, 0f);
			effectModel.transform.localScale = _initEffectModelScale;
			trail.ClearSystem(emitState: true);
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
				Vector3 a3 = a * _velocityTweener.UpdateParameterFloat(Time.deltaTime);
				a3 *= Time.deltaTime;
				_totalMoveDistance += Vector3.Distance(base.transform.position, base.transform.position + a3);
				base.transform.position += a3;
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= FlightlingShotDuration)
				{
					destoryEmitObject(FlightlingShotDestroyDuration);
					FieEmitObjectFlightlingBurst fieEmitObjectFlightlingBurst = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectFlightlingBurst>(base.transform, Vector3.zero);
					if (fieEmitObjectFlightlingBurst != null)
					{
						fieEmitObjectFlightlingBurst.setAllyTag(getArrayTag());
						fieEmitObjectFlightlingBurst.setHostileTag(getHostileTag());
					}
					trail.Emit = false;
					trail.ClearSystem(emitState: false);
					effectModel.transform.localScale = Vector3.zero;
					_isEndUpdate = true;
				}
				if (a3 != Vector3.zero)
				{
					base.transform.rotation = Quaternion.LookRotation(a3);
				}
			}
		}

		private void initRandomTilt()
		{
			Vector3 a = base.transform.position + directionalVec * 0.2f;
			Vector3 a2 = Quaternion.AngleAxis(FieRandom.Range(0f, 360f), directionalVec) * Vector3.up;
			a2 *= 0.2f;
			a += a2 * 0.2f;
			_randomTiltVec = Quaternion.LookRotation(a - base.transform.position) * Vector3.forward;
			_randomTiltVec.Normalize();
			_isInitRandomTilt = true;
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && (collider.gameObject.tag == getHostileTagString() || collider.gameObject.tag == "Floor"))
			{
				FieEmitObjectFlightlingBurst fieEmitObjectFlightlingBurst = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectFlightlingBurst>(base.transform, Vector3.zero);
				if (fieEmitObjectFlightlingBurst != null)
				{
					fieEmitObjectFlightlingBurst.setAllyTag(getArrayTag());
					fieEmitObjectFlightlingBurst.setHostileTag(getHostileTag());
				}
				destoryEmitObject(FlightlingShotDestroyDuration);
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
