using Fie.Utility;
using UnityEngine;

namespace Fie.Title
{
	public class FieTitleElementsShooter : MonoBehaviour
	{
		[SerializeField]
		private float SummonArrowChildDuration = 1.5f;

		[SerializeField]
		private float SummonArrowVelocityMax = 4f;

		[SerializeField]
		private float SummonArrowVelocityAccelTime = 0.3f;

		[SerializeField]
		private float SummonArrowTiltDuration = 0.5f;

		private const float RANDOM_TILT_DISTANCE = 15f;

		private const float HORMING_DISTANCE_MAX = 10f;

		public float transitionTime = 0.5f;

		public float shootTime = 4f;

		public Transform targetTransform;

		private Tweener<TweenTypesOutSine> _velocityTweener = new Tweener<TweenTypesOutSine>();

		private Tweener<TweenTypesInSine> _tiltForceTweener = new Tweener<TweenTypesInSine>();

		private float shootCounter;

		private bool isShoot;

		private Vector3 directionalVec = Vector3.zero;

		private Vector3 _randomTiltVec = Vector3.zero;

		private Vector3 fromPosition = Vector3.zero;

		private void Start()
		{
			_velocityTweener.InitTweener(SummonArrowVelocityAccelTime, 0f, SummonArrowVelocityMax);
		}

		private void initRandomTilt()
		{
			Vector3 a = base.transform.position + directionalVec * 15f;
			Vector3 a2 = Quaternion.AngleAxis(Random.Range(0f, 360f), directionalVec) * Vector3.up;
			a2 *= 15f;
			a += a2 * 15f;
			_randomTiltVec = Quaternion.LookRotation(a - base.transform.position) * Vector3.forward;
			_randomTiltVec.Normalize();
		}

		private void Update()
		{
			shootCounter += Time.deltaTime;
			if (shootCounter >= shootTime)
			{
				if (!isShoot)
				{
					base.transform.parent = null;
					isShoot = true;
					directionalVec = targetTransform.position - base.transform.position;
					directionalVec.Normalize();
					_velocityTweener.InitTweener(SummonArrowVelocityAccelTime, base.transform.position, targetTransform.position);
					_tiltForceTweener.InitTweener(SummonArrowTiltDuration, 0f, 1f);
					fromPosition = base.transform.position;
				}
				else
				{
					float t = _tiltForceTweener.UpdateParameterFloat(Time.deltaTime);
					base.transform.position = Vector3.Slerp(fromPosition, targetTransform.position, t);
				}
			}
		}
	}
}
