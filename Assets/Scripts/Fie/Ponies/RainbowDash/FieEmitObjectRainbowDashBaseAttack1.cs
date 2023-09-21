using Fie.Manager;
using Fie.Object;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashBaseAttack1")]
	public class FieEmitObjectRainbowDashBaseAttack1 : FieEmittableObjectBase
	{
		[SerializeField]
		private float DURATION = 1.5f;

		[SerializeField]
		private float TRAIL_DURATION = 0.4f;

		[SerializeField]
		private float DAMAGE_DURATION = 0.5f;

		[SerializeField]
		private SmoothTrail _airKickTrail;

		private float _lifeTimeCount;

		private bool _isEndTrail;

		private bool _isEndUpdate;

		public override void awakeEmitObject()
		{
			_airKickTrail.Emit = true;
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= TRAIL_DURATION)
				{
					_airKickTrail.Emit = false;
				}
				if (_lifeTimeCount >= DURATION)
				{
					destoryEmitObject();
				}
				if (initTransform != null)
				{
					base.transform.position = initTransform.position;
					base.transform.rotation = initTransform.rotation;
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
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
