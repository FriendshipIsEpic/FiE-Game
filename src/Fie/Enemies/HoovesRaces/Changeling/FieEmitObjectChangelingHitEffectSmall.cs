using Fie.Object;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Changeling/Power/ChangelingHitEffectSmall")]
	public class FieEmitObjectChangelingHitEffectSmall : FieEmittableObjectBase
	{
		private const float DURATION = 1f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(1f);
		}
	}
}
