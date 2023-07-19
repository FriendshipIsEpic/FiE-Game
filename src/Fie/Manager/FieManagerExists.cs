using System;

namespace Fie.Manager
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class FieManagerExists : Attribute
	{
		public readonly FieManagerExistSceneFlag existFlag;

		public FieManagerExists(FieManagerExistSceneFlag initFlags)
		{
			existFlag = initFlags;
		}
	}
}
