using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashOmniSmashExplosionEffect")]
	public class FieEmitObjectRainbowDashOmniSmashExplosionEffect : FieEmittableObjectBase
	{
		[SerializeField]
		private PKFxFX effectFx;

		public float scale = 1f;

		private const float duration = 2.5f;

		public override void awakeEmitObject()
		{
			destoryEmitObject(2.5f);
			scale = 1f;
			if (effectFx != null)
			{
				effectFx.StopEffect();
				effectFx.StartEffect();
			}
		}

		public void SetScale(float newScale)
		{
			scale = newScale;
			effectFx.SetAttribute(new PKFxManager.Attribute("Scale", scale));
		}

		private void Update()
		{
			base.transform.rotation = Quaternion.identity;
		}
	}
}
