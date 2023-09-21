using Fie.Object;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashHitEffectStab")]
	public class FieEmitObjectRainbowDashHitEffectStab : FieEmittableObjectBase
	{
		private const float DURATION = 1.5f;

		public override void awakeEmitObject()
		{
			base.transform.position = directionalVec;
			destoryEmitObject(1.5f);
		}
	}
}
