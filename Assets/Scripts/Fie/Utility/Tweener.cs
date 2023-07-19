using UnityEngine;

namespace Fie.Utility
{
	public class Tweener<T> where T : TweenTypesBase, new()
	{
		private T _base;

		private float _nowTime = 0.1f;

		public Tweener()
		{
			_base = new T();
			_nowTime = 0.1f;
			_base.InitTweener(0.1f, Vector3.zero, Vector3.zero);
		}

		public void Finish()
		{
			_nowTime = _base.duration;
		}

		public void InitTweener(float total, Vector3 start, Vector3 end)
		{
			_nowTime = 0f;
			_base.InitTweener(total, start, end);
		}

		public void InitTweener(float total, Vector4 start, Vector4 end)
		{
			_nowTime = 0f;
			_base.InitTweener(total, start, end);
		}

		public void InitTweener(float total, float start, float end)
		{
			_nowTime = 0f;
			_base.InitTweener(total, new Vector3(start, 0f, 0f), new Vector3(end, 0f, 0f));
		}

		public Vector4 GetParameterVec4(float now)
		{
			return _base.GetParameter(now);
		}

		public Vector3 GetParameterVec3(float now)
		{
			return _base.GetParameter(now);
		}

		public float GetParameterFloat(float now)
		{
			Vector4 parameter = _base.GetParameter(_nowTime);
			return parameter.x;
		}

		public Vector4 GetNowParameterVec4()
		{
			return _base.GetParameter(_nowTime);
		}

		public Vector3 GetNowParameterVec3()
		{
			return _base.GetParameter(_nowTime);
		}

		public float GetNowParameterFloat()
		{
			Vector4 parameter = _base.GetParameter(_nowTime);
			return parameter.x;
		}

		public float GetNowProgress()
		{
			return _nowTime / getTotalTime();
		}

		public Vector4 UpdateParameterVec4(float addTime)
		{
			if (IsEnd())
			{
				return _base.endParam;
			}
			_nowTime = Mathf.Min(_nowTime + addTime, _base.duration);
			return _base.GetParameter(_nowTime);
		}

		public Vector3 UpdateParameterVec3(float addTime)
		{
			if (IsEnd())
			{
				return _base.endParam;
			}
			_nowTime = Mathf.Min(_nowTime + addTime, _base.duration);
			return _base.GetParameter(_nowTime);
		}

		public float UpdateParameterFloat(float addTime)
		{
			if (IsEnd())
			{
				Vector4 endParam = _base.endParam;
				return endParam.x;
			}
			_nowTime = Mathf.Min(_nowTime + addTime, _base.duration);
			Vector4 parameter = _base.GetParameter(_nowTime);
			return parameter.x;
		}

		public bool IsEnd()
		{
			return _nowTime >= _base.duration;
		}

		public float getTotalTime()
		{
			return _base.duration;
		}

		public Vector4 getStartParamVec4()
		{
			return _base.startParam;
		}

		public Vector4 getEndParamVec4()
		{
			return _base.endParam;
		}

		public Vector3 getStartParamVec3()
		{
			return _base.startParam;
		}

		public Vector3 getEndParamVec3()
		{
			return _base.endParam;
		}

		public float getStartParamFloat()
		{
			Vector4 startParam = _base.startParam;
			return startParam.x;
		}

		public float getEndParamFloat()
		{
			Vector4 endParam = _base.endParam;
			return endParam.x;
		}
	}
}
