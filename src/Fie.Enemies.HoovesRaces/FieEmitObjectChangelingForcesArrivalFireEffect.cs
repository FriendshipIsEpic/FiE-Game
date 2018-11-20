using Fie.Object;

namespace Fie.Enemies.HoovesRaces
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Common/ChangelingForcesArrivalFire")]
	public class FieEmitObjectChangelingForcesArrivalFireEffect : FieEmittableObjectBase
	{
		private const float DURATION = 2f;

		private void Start()
		{
			destoryEmitObject(2f);
		}
	}
}
