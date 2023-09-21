using Fie.Object;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightHitEffectMiddle")]
	public class FieEmitObjectTwilightHitEffectMiddle : FieEmittableObjectBase
	{
		private const float DURATION = 2f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(2f);
		}
	}
}
