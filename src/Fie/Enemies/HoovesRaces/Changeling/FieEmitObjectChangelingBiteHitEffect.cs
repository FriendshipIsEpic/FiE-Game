using Fie.Object;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Changeling/Power/ChangelingBiteHitEffect")]
	public class FieEmitObjectChangelingBiteHitEffect : FieEmittableObjectBase
	{
		private const float DURATION = 1f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(1f);
		}
	}
}
