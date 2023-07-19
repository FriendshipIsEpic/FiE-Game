// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.ColorExtensions
using System.Globalization;
using GameDataEditor;
using UnityEngine;

namespace GameDataEditor
{
	public static class ColorExtensions
	{
		public static string ToHexString(this Color32 color)
		{
			return string.Format("#{0}{1}{2}", color.r.ToString("x2"), color.g.ToString("x2"), color.b.ToString("x2"));
		}

		public static Color ToColor(this string hex)
		{
			return hex.ToColor32();
		}

		public static Color32 ToColor32(this string hex)
		{
			if (string.IsNullOrEmpty(hex))
			{
				return default(Color32);
			}
			hex = hex.Replace("#", string.Empty);
			byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
			return new Color32(r, g, b, 1);
		}

		public static bool NearlyEqual(this Color variable, Color other)
		{
			return variable.r.NearlyEqual(other.r) && variable.g.NearlyEqual(other.g) && variable.b.NearlyEqual(other.b);
		}

		public static bool NearlyEqual(this Color32 variable, Color32 other)
		{
			return FloatExtensions.NearlyEqual((int)variable.r, (int)other.r) && FloatExtensions.NearlyEqual((int)variable.g, (int)other.g) && FloatExtensions.NearlyEqual((int)variable.b, (int)other.b);
		}
	}
}