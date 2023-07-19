using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisPenetrateShotActivateEffect")]
	public class FieEmitObjectQueenChrysalisPenetrateShotActivateEffect : FieEmittableObjectBase
	{
		private const float DURATION = 3f;

		public override void awakeEmitObject()
		{
			base.transform.rotation = Quaternion.LookRotation(directionalVec);
			destoryEmitObject(3f);
		}
	}
}
