using Fie.Object;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisActivationEffect")]
	public class FieEmitObjectQueenChrysalisCommonActivationEffect : FieEmittableObjectBase
	{
		private const float DURATION = 2f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(2f);
		}
	}
}
