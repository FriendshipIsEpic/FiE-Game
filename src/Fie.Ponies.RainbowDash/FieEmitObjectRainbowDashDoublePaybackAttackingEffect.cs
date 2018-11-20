using Fie.Object;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashDoublePaybackAttackingEffect")]
	public class FieEmitObjectRainbowDashDoublePaybackAttackingEffect : FieEmittableObjectBase
	{
		[SerializeField]
		private PKFxFX lightningEffect;

		[SerializeField]
		private SmoothTrail attackingTrail;

		private bool isEndUpdate;

		public override void awakeEmitObject()
		{
			if (lightningEffect != null)
			{
				lightningEffect.StopEffect();
				lightningEffect.StartEffect();
			}
			if (attackingTrail != null)
			{
				attackingTrail.ClearSystem(emitState: true);
			}
			isEndUpdate = false;
		}

		public void Update()
		{
			if (!isEndUpdate)
			{
				if (initTransform != null)
				{
					base.transform.position = initTransform.position;
				}
				if (attackingTrail != null)
				{
					attackingTrail.Emit = true;
				}
			}
		}

		public void stopEffect(float destoryDuration)
		{
			if (lightningEffect != null)
			{
				lightningEffect.StopEffect();
			}
			if (attackingTrail != null)
			{
				attackingTrail.Emit = false;
			}
			destoryEmitObject(destoryDuration);
			isEndUpdate = true;
		}
	}
}
