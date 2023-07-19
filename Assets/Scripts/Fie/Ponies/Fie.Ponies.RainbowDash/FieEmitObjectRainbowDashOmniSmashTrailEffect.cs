using Fie.Object;
using System.Collections;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashOmniSmashTrailEffect")]
	public class FieEmitObjectRainbowDashOmniSmashTrailEffect : FieEmittableObjectBase
	{
		[SerializeField]
		private PKFxFX trailFx;

		private const float TRAIL_EMIT_DURATION = 0.3f;

		private const float DURATION = 1f;

		private float _lifeCount;

		private IEnumerator StopEffectCoroutine()
		{
			yield return (object)new WaitForSeconds(0.3f);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public override void awakeEmitObject()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
			if (trailFx != null)
			{
				trailFx.StopEffect();
				trailFx.StartEffect();
			}
			if (directionalVec != Vector3.zero)
			{
				base.transform.rotation = Quaternion.LookRotation(directionalVec);
			}
			destoryEmitObject(1f);
			StartCoroutine(StopEffectCoroutine());
		}

		private void Update()
		{
			_lifeCount += Time.deltaTime;
		}

		private void LateUpdate()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
		}
	}
}
