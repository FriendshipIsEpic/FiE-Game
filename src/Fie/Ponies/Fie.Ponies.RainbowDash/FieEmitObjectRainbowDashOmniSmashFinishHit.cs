using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashOmniSmashFinishHit")]
	public class FieEmitObjectRainbowDashOmniSmashFinishHit : FieEmittableObjectBase
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
			if (!_isEndUpdate && !(_lifeTimeCount > DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString() && !(base.ownerCharacter == null))
			{
				FieRainbowDash fieRainbowDash = base.ownerCharacter as FieRainbowDash;
				if (!(fieRainbowDash == null))
				{
					FieDamage defaultDamageObject = getDefaultDamageObject();
					defaultDamageObject.damage *= (float)fieRainbowDash.omniSmashAttackingCount * 2f;
					defaultDamageObject.stagger *= 1f + (float)fieRainbowDash.omniSmashAttackingCount * 0.5f;
					FieGameCharacter x = addDamageToCollisionCharacter(collider, defaultDamageObject);
					if (x != null)
					{
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashHitEffect>(base.transform, collider.ClosestPointOnBounds(base.transform.position));
						FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_SMALL);
						if (base.ownerCharacter != null && base.ownerCharacter.friendshipStats != null)
						{
							base.ownerCharacter.friendshipStats.safeAddFriendship(base.gainedFriendshipPoint * ((float)fieRainbowDash.omniSmashAttackingCount * 0.5f));
						}
					}
				}
			}
		}
	}
}
