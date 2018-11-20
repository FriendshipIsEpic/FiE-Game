using Fie.Object;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightHitEffectSemiMiddle")]
	public class FieEmitObjectTwilightHitEffectSemiMiddle : FieEmittableObjectBase
	{
		private const float DURATION = 2f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(2f);
		}
	}
}
