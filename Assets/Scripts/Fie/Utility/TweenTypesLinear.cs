using UnityEngine;

namespace Fie.Utility
{
	public class TweenTypesLinear : TweenTypesBase
	{
		public override Vector4 GetParameter(float now)
		{
			return Vector4.Lerp(base.startParam, base.endParam, now / base.duration);
		}
	}
}
