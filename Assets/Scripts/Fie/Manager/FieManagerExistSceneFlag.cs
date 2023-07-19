using System;

namespace Fie.Manager
{
	[Flags]
	public enum FieManagerExistSceneFlag
	{
		ANYTIME_DESTROY = 0x1,
		OUTGAME = 0x2,
		INGAME = 0x4,
		NEVER_DESTROY = 0x8
	}
}
