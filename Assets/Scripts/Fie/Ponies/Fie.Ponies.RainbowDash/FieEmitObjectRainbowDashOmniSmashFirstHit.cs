using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashOmniSmashFirstHit")]
	public class FieEmitObjectRainbowDashOmniSmashFirstHit : FieEmittableObjectBase
	{
		[SerializeField]
		private float DURATION = 0.4f;

		[SerializeField]
		private float DAMAGE_DURATION = 0.3f;

		private float _lifeTimeCount;

		private bool _isEndTrail;

		private bool _isEndUpdate;

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= DURATION)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
				if (initTransform != null)
				{
					base.transform.position = initTransform.position;
				}
				base.transform.rotation = Quaternion.LookRotation(directionalVec);
			}
		}

		protected void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter x = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (x != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashHitEffect>(base.transform, collider.ClosestPointOnBounds(base.transform.position));
					FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_SMALL);
				}
			}
		}
	}
}
