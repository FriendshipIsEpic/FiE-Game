using Fie.Utility;
using UnityEngine;

namespace Fie.LevelObject
{
	public class FieLevelObjectGlowEyes : MonoBehaviour
	{
		[SerializeField]
		private float _rotatingDuration = 0.5f;

		[SerializeField]
		private float _rotatebleAngleZ = 15f;

		[SerializeField]
		private float _rotatebleAngleY = 30f;

		[SerializeField]
		private Vector2 _changingAngleIntervalMinMax = new Vector2(0.75f, 5f);

		private Tweener<TweenTypesInOutSine> _tweener = new Tweener<TweenTypesInOutSine>();

		private Quaternion _currentTargetRotation;

		private Quaternion _oldRotation;

		private float _rotationCounter;

		private void Start()
		{
			SetNextParameter();
		}

		private void SetNextParameter()
		{
			_tweener.InitTweener(_rotatingDuration, 0f, 1f);
			_rotationCounter = Random.Range(_changingAngleIntervalMinMax.x, _changingAngleIntervalMinMax.y);
			_currentTargetRotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f - _rotatebleAngleY, _rotatebleAngleY), Random.Range(0f - _rotatebleAngleZ, _rotatebleAngleZ)));
			_oldRotation = base.transform.rotation;
		}

		private void Update()
		{
			if (!_tweener.IsEnd())
			{
				float t = _tweener.UpdateParameterFloat(Time.deltaTime);
				base.transform.rotation = Quaternion.Lerp(_oldRotation, _currentTargetRotation, t);
			}
			_rotationCounter -= Time.deltaTime;
			if (_rotationCounter <= 0f)
			{
				SetNextParameter();
			}
		}
	}
}
