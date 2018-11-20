using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashBaseAttack2")]
	public class FieEmitObjectRainbowDashBaseAttack2 : FieEmittableObjectBase
	{
		[SerializeField]
		private float DURATION = 0.3f;

		[SerializeField]
		private float DAMAGE_DURATION = 0.2f;

		private float _lifeTimeCount;

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
