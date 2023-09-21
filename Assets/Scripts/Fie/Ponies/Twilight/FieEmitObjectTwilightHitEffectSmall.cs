using Fie.Object;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightHitEffectSmall")]
	public class FieEmitObjectTwilightHitEffectSmall : FieEmittableObjectBase
	{
		private const float DURATION = 1f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(1f);
		}
	}
}
