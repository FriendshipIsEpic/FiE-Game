using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashOmniSmashHitEffect")]
	public class FieEmitObjectRainbowDashOmniSmashHitEffect : FieEmittableObjectBase
	{
		private const float duration = 1.5f;

		public override void awakeEmitObject()
		{
			base.transform.rotation = Quaternion.LookRotation(directionalVec);
			destoryEmitObject(1.5f);
		}

		private void Update()
		{
		}
	}
}
