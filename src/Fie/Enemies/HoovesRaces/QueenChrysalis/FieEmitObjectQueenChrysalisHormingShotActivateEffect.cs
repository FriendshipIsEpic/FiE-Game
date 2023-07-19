using Fie.Object;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisHormingShotActivateEffect")]
	public class FieEmitObjectQueenChrysalisHormingShotActivateEffect : FieEmittableObjectBase
	{
		private const float DURATION = 1.5f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(1.5f);
		}
	}
}
