using Fie.Object;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightChargedEffect")]
	public class FieEmitObjectTwilightChargedEffect : FieEmittableObjectBase
	{
		private const float DURATION = 2f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(2f);
		}
	}
}
