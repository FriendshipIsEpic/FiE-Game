using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashOmniSmashRainboomEffect")]
	public class FieEmitObjectRainbowDashOmniSmashRainboomEffect : FieEmittableObjectBase
	{
		private const float duration = 2f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(2f);
		}

		private void Update()
		{
			base.transform.rotation = Quaternion.LookRotation(directionalVec);
		}
	}
}
