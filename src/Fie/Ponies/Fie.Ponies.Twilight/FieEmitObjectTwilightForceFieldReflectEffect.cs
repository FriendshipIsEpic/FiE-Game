using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightForceFieldReflectEffect")]
	public class FieEmitObjectTwilightForceFieldReflectEffect : FieEmittableObjectBase
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
