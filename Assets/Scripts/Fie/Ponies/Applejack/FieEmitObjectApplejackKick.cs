using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Power/ApplejackKick")]
	public class FieEmitObjectApplejackKick : FieEmittableObjectBase
	{
		[SerializeField]
		private float APPLEJACK_KICK_DURATION = 0.4f;

		[SerializeField]
		private float APPLEJACK_KICK_DAMAGE_DURATION = 0.3f;

		[SerializeField]
		private float APPLEJACK_KICK_PUSH_FORCE = 7f;

		private float _lifeTimeCount;

		private bool _isEndTrail;

		private bool _isEndUpdate;

		public void Update()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= APPLEJACK_KICK_DURATION)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > APPLEJACK_KICK_DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
			{
				FieDamage damageObject = getDefaultDamageObject();
				FieApplejack fieApplejack = base.ownerCharacter as FieApplejack;
				if (fieApplejack != null)
				{
					fieApplejack.ApplyKickDamageMagni(ref damageObject);
				}
				FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, damageObject);
				if (fieGameCharacter != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackHitEffectMiddle>(base.transform, collider.ClosestPointOnBounds(base.transform.position));
					FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_MIDDLE);
					fieGameCharacter.resetMoveForce();
					fieGameCharacter.setMoveForce(directionalVec * APPLEJACK_KICK_PUSH_FORCE, 0f, useRound: false);
				}
			}
		}
	}
}
