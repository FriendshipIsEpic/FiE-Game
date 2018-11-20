using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashOmniSmashImpactEffect")]
	public class FieEmitObjectRainbowDashOmniSmashImpactEffect : FieEmittableObjectBase
	{
		[SerializeField]
		private PKFxFX effectFx;

		private const float duration = 1.5f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(1.5f);
			if (effectFx != null)
			{
				effectFx.StopEffect();
				effectFx.StartEffect();
			}
			FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(0.5f, 6, new Vector3(0.5f, 0.5f, 0.5f));
		}

		private void Update()
		{
			base.transform.rotation = Quaternion.identity;
		}
	}
}
