// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.ArrayExtensions
using System;

namespace GameDataEditor
{
	public static class ArrayExtensions
	{
		public static bool IsValidIndex(this Array variable, int index)
		{
			return index > -1 && variable != null && index < variable.Length;
		}
	}
}