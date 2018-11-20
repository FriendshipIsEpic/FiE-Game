using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisReflectEffect")]
	public class FieEmitObjectQueenChrysalisReflectionEffect : FieEmittableObjectBase
	{
		[SerializeField]
		private float reflectEffectDuration = 0.6f;

		public override void awakeEmitObject()
		{
			base.transform.rotation = Quaternion.LookRotation(directionalVec);
			destoryEmitObject(reflectEffectDuration);
		}
	}
}
