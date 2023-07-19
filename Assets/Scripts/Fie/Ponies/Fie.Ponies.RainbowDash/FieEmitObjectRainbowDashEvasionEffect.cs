using Fie.Manager;
using Fie.Object;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashEvasionEffect")]
	public class FieEmitObjectRainbowDashEvasionEffect : FieEmittableObjectBase
	{
		[SerializeField]
		private SmoothTrail evasionTrail;

		[SerializeField]
		private float DAMAGE_DURATION = 0.3f;

		private const float TRAIL_EMIT_DURATION = 0.15f;

		private const float DURATION = 0.6f;

		private float _lifeCount;

		public override void awakeEmitObject()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
			if (evasionTrail != null)
			{
				evasionTrail.ClearSystem(emitState: true);
				evasionTrail.Emit = true;
			}
			if (directionalVec != Vector3.zero)
			{
				base.transform.rotation = Quaternion.LookRotation(directionalVec);
			}
			destoryEmitObject(0.6f);
		}

		private void Update()
		{
			_lifeCount += Time.deltaTime;
			if (_lifeCount > 0.15f)
			{
				evasionTrail.Emit = false;
			}
		}

		private void LateUpdate()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!(_lifeCount > DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter x = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (x != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashHitEffectSmall>(base.transform, collider.ClosestPointOnBounds(base.transform.position));
				}
			}
		}
	}
}
