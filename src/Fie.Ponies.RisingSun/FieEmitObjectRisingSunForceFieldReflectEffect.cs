using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/Power/RisingSunForceFieldReflectEffect")]
	public class FieEmitObjectRisingSunForceFieldReflectEffect : FieEmittableObjectBase
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
