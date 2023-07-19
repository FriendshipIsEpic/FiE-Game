// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.FloatExtensions
using System;

namespace GameDataEditor
{
	public static class FloatExtensions
	{
		public const float TOLERANCE = 0.1f;

		public static bool NearlyEqual(this float a, float b)
		{
			return Math.Abs(a - b) < 0.1f;
		}
	}
}