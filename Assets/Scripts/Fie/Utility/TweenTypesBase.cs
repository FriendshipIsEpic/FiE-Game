using UnityEngine;

namespace Fie.Utility
{
	public abstract class TweenTypesBase
	{
		private float _duration = 0.1f;

		private Vector4 _startParam = Vector4.zero;

		private Vector4 _endParam = Vector4.zero;

		public float duration => _duration;

		public Vector4 startParam => _startParam;

		public Vector4 endParam => _endParam;

		public void InitTweener(float duration, Vector4 start, Vector4 end)
		{
			_duration = duration;
			_startParam = start;
			_endParam = end;
		}

		public abstract Vector4 GetParameter(float now);
	}
}
