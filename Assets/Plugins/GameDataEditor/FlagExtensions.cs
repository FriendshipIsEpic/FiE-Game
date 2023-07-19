// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.FlagExtensions
using System;

namespace GameDataEditor
{
	public static class FlagExtensions
	{
		public static bool IsSet(this Enum variable, Enum flag)
		{
			ulong num = Convert.ToUInt64(variable);
			ulong num2 = Convert.ToUInt64(flag);
			return (num & num2) == num2;
		}
	}
}