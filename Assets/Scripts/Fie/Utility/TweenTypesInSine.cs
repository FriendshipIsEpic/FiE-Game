using UnityEngine;

namespace Fie.Utility
{
	public class TweenTypesInSine : TweenTypesBase
	{
		public override Vector4 GetParameter(float now)
		{
			now /= base.duration;
			return (base.endParam - base.startParam) * now * now + base.startParam;
		}
	}
}
