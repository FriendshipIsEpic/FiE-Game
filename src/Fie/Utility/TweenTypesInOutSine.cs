using UnityEngine;

namespace Fie.Utility
{
	public class TweenTypesInOutSine : TweenTypesBase
	{
		public override Vector4 GetParameter(float now)
		{
			now /= base.duration / 2f;
			if (now < 1f)
			{
				return (base.endParam - base.startParam) / 2f * now * now + base.startParam;
			}
			now -= 1f;
			return -(base.endParam - base.startParam) / 2f * (now * (now - 2f) - 1f) + base.startParam;
		}
	}
}
