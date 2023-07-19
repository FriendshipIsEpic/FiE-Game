using Fie.Manager;
using Fie.Object;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Power/ApplejackAirKick")]
	public class FieEmitObjectApplejackAirKick : FieEmittableObjectBase
	{
		[SerializeField]
		private float APPLEJACK_AIR_KICK_DURATION = 1.5f;

		[SerializeField]
		private float APPLEJACK_AIR_KICK_TRAIL_DURATION = 0.4f;

		[SerializeField]
		private float APPLEJACK_AIR_KICK_DAMAGE_DURATION = 0.5f;

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
				if (_lifeTimeCount >= APPLEJACK_AIR_KICK_TRAIL_DURATION)
				{
					_airKickTrail.Emit = false;
				}
				if (_lifeTimeCount >= APPLEJACK_AIR_KICK_DURATION)
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
			if (!_isEndUpdate && !(_lifeTimeCount > APPLEJACK_AIR_KICK_DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
			{
				FieDamage damageObject = getDefaultDamageObject();
				FieApplejack fieApplejack = base.ownerCharacter as FieApplejack;
				if (fieApplejack != null)
				{
					fieApplejack.ApplyKickDamageMagni(ref damageObject);
				}
				FieGameCharacter x = addDamageToCollisionCharacter(collider, damageObject);
				if (x != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackHitEffectMiddle>(base.transform, collider.ClosestPointOnBounds(base.transform.position));
				}
			}
		}
	}
}
