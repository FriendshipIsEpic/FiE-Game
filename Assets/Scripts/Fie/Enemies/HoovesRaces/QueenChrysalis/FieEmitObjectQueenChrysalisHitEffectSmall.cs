using Fie.Object;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisHitEffectSmall")]
	public class FieEmitObjectQueenChrysalisHitEffectSmall : FieEmittableObjectBase
	{
		private const float DURATION = 1f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(1f);
		}
	}
}
