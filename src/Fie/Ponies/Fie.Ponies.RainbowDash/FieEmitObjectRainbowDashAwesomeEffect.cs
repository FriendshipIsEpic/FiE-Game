using Fie.Object;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashAwesomeEffect")]
	public class FieEmitObjectRainbowDashAwesomeEffect : FieEmittableObjectBase
	{
		[SerializeField]
		private SmoothTrail awesomeTrail;

		[SerializeField]
		private GameObject lightObject;

		private bool isEndUpdate;

		public override void awakeEmitObject()
		{
			if (awesomeTrail != null)
			{
				awesomeTrail.ClearSystem(emitState: true);
			}
			if (lightObject != null)
			{
				lightObject.SetActive(value: true);
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
				if (awesomeTrail != null)
				{
					awesomeTrail.Emit = true;
				}
			}
		}

		public void stopEffect(float destoryDuration)
		{
			if (awesomeTrail != null)
			{
				awesomeTrail.Emit = false;
			}
			if (lightObject != null)
			{
				lightObject.SetActive(value: false);
			}
			destoryEmitObject(destoryDuration);
			isEndUpdate = true;
		}
	}
}
