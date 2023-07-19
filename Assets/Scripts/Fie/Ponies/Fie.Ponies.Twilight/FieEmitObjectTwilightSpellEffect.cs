using Fie.Object;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightSpellEffect")]
	public class FieEmitObjectTwilightSpellEffect : FieEmittableObjectBase
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
