// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.VectorExtensions
using GameDataEditor;
using UnityEngine;

namespace GameDataEditor
{
	public static class VectorExtensions
	{
		public static bool NearlyEqual(this Vector3 variable, Vector3 other)
		{
			return variable.x.NearlyEqual(other.x) && variable.y.NearlyEqual(other.y) && variable.z.NearlyEqual(other.z);
		}
	}
}