// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.BasicFieldType
using System;

namespace GameDataEditor
{
	[Flags]
	public enum BasicFieldType
	{
		Undefined = 0,
		Bool = 1,
		Int = 2,
		Float = 4,
		String = 8,
		Vector2 = 0x10,
		Vector3 = 0x20,
		Vector4 = 0x40,
		Color = 0x80,
		GameObject = 0x100,
		Texture2D = 0x200,
		Material = 0x400,
		AudioClip = 0x800
	}
}