using Fie.Object;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/Power/RisingSunSpellEffect")]
	public class FieEmitObjectRisingSunSpellEffect : FieEmittableObjectBase
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
