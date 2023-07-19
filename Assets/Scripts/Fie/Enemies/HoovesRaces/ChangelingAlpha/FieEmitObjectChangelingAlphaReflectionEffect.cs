using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/ChangelingAlpha/Power/ChangelingAlphaReflectEffect")]
	public class FieEmitObjectChangelingAlphaReflectionEffect : FieEmittableObjectBase
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
