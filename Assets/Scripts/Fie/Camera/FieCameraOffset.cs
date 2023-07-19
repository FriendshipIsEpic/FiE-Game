using Fie.Utility;
using UnityEngine;

namespace Fie.Camera
{
	public class FieCameraOffset
	{
		public struct FieCameraOffsetParam
		{
			public Vector3 position;

			public Vector3 angle;

			public float fov;

			public FieCameraOffsetParam(Vector3 position, Vector3 angle, float v)
			{
				this.position = position;
				this.angle = angle;
				fov = v;
			}

			public static FieCameraOffsetParam operator *(FieCameraOffsetParam a, float b)
			{
				return new FieCameraOffsetParam(a.position * b, a.angle * b, a.fov * b);
			}
		}

		private Tweener<TweenTypesInOutSine> _startTweener = new Tweener<TweenTypesInOutSine>();

		private Tweener<TweenTypesInOutSine> _endTweener = new Tweener<TweenTypesInOutSine>();

		private FieCameraOffsetParam _offset;

		private float stayDuration;

		public FieCameraOffset(FieCameraOffsetParam offset, float startDuration, float stayDuration, float endDuration)
		{
			_offset = offset;
			_startTweener.InitTweener(startDuration, 0f, 1f);
			_endTweener.InitTweener(endDuration, 1f, 0f);
			this.stayDuration = stayDuration;
		}

		public bool isEnd()
		{
			if (_startTweener.IsEnd() && _endTweener.IsEnd())
			{
				return true;
			}
			return false;
		}

		public FieCameraOffsetParam updateParams(float deltaTime)
		{
			if (!_startTweener.IsEnd())
			{
				return _offset * _startTweener.UpdateParameterFloat(deltaTime);
			}
			if (stayDuration > 0f)
			{
				stayDuration -= deltaTime;
				return _offset * 1f;
			}
			if (!_endTweener.IsEnd())
			{
				return _offset * _endTweener.UpdateParameterFloat(deltaTime);
			}
			return _offset * 0f;
		}
	}
}
