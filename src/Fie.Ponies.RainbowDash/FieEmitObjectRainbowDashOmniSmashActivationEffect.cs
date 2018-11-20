using Fie.Object;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashOmniSmashActivationEffect")]
	public class FieEmitObjectRainbowDashOmniSmashActivationEffect : FieEmittableObjectBase
	{
		private const float duration = 4f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(4f);
		}

		private void Update()
		{
		}
	}
}
