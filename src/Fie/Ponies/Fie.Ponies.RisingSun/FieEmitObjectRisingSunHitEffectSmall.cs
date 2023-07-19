using Fie.Object;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/Power/RisingSunHitEffectSmall")]
	public class FieEmitObjectRisingSunHitEffectSmall : FieEmittableObjectBase
	{
		private const float DURATION = 1f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(1f);
		}
	}
}
