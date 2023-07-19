using Fie.Object;

namespace Fie.Enemies.HoovesRaces
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Common/ChangelingForcesDeadEffect")]
	public class FieEmitObjectChangelingForcesDeadEffect : FieEmittableObjectBase
	{
		private const float DURATION = 2.5f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(2.5f);
		}
	}
}
