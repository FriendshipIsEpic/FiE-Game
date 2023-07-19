using Fie.Object;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashHitEffectSmall")]
	public class FieEmitObjectRainbowDashHitEffectSmall : FieEmittableObjectBase
	{
		private const float DURATION = 1f;

		public override void awakeEmitObject()
		{
			base.transform.position = directionalVec;
			destoryEmitObject(1f);
		}
	}
}
