using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashDoublePaybackHitEffect")]
	public class FieEmitObjectRainbowDashDoublePaybackHitEffect : FieEmittableObjectBase
	{
		private const float duration = 3f;

		public override void awakeEmitObject()
		{
			base.transform.rotation = Quaternion.LookRotation(directionalVec);
			destoryEmitObject(3f);
		}

		private void Update()
		{
		}
	}
}
