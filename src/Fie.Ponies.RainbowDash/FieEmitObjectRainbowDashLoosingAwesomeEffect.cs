using Fie.Object;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashLoosingAwesomeEffect")]
	public class FieEmitObjectRainbowDashLoosingAwesomeEffect : FieEmittableObjectBase
	{
		private const float duration = 2f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(2f);
		}

		private void Update()
		{
		}
	}
}
