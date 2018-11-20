using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Power/ApplejackPunch")]
	public class FieEmitObjectApplejackPunch : FieEmittableObjectBase
	{
		[SerializeField]
		private float APPLEJACK_PUNCH_DURATION = 0.3f;

		[SerializeField]
		private float APPLEJACK_PUNCH_DAMAGE_DURATION = 0.2f;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= APPLEJACK_PUNCH_DURATION)
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

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > APPLEJACK_PUNCH_DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter x = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (x != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackHitEffectSmall>(base.transform, collider.ClosestPointOnBounds(base.transform.position));
				}
			}
		}
	}
}
