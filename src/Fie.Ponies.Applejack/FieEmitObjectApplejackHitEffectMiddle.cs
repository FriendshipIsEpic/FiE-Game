using Fie.Object;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Power/ApplejackHitEffectMiddle")]
	public class FieEmitObjectApplejackHitEffectMiddle : FieEmittableObjectBase
	{
		private const float DURATION = 1f;

		public override void awakeEmitObject()
		{
			base.transform.position = directionalVec;
			destoryEmitObject(1f);
		}
	}
}
