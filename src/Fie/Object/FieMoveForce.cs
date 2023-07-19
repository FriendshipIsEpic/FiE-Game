using UnityEngine;

namespace Fie.Object
{
	public class FieMoveForce
	{
		private Vector3 _moveForce;

		private float _duration;

		private float _restDuration;

		public bool isEnable => _restDuration > 0f;

		public Vector3 moveForce => (!isEnable) ? Vector3.zero : (_moveForce * restForceRate);

		public float restForceRate => 1f - Mathf.Abs(_restDuration - _duration) / Mathf.Max(0.01f, _duration);

		public FieMoveForce(Vector3 moveForce, float duration = 0f)
		{
			_moveForce = moveForce;
			_duration = (_restDuration = duration);
		}

		public void Update(float fixedTime)
		{
			_restDuration -= fixedTime;
		}
	}
}
