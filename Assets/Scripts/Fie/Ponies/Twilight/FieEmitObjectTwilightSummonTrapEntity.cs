using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwiilghtSummonTrapEntity")]
	public class FieEmitObjectTwilightSummonTrapEntity : FieEmittableObjectBase
	{
		[SerializeField]
		private float TrapEntityDuration = 20f;

		[SerializeField]
		private float TrapEntityDestroyDuration = 1f;

		[SerializeField]
		private PKFxFX EntityEffect;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		public override void awakeEmitObject()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
			if (EntityEffect != null)
			{
				EntityEffect.StopEffect();
				EntityEffect.StartEffect();
			}
		}

		public void Update()
		{
			base.transform.localRotation = Quaternion.identity;
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= TrapEntityDuration - TrapEntityDestroyDuration)
				{
					if (EntityEffect != null)
					{
						EntityEffect.StopEffect();
					}
					destoryEmitObject(TrapEntityDestroyDuration);
					_isEndUpdate = true;
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > TrapEntityDuration - TrapEntityDestroyDuration) && collider.gameObject.tag == getHostileTagString())
			{
				Explode();
				if (EntityEffect != null)
				{
					EntityEffect.StopEffect();
				}
				destoryEmitObject(TrapEntityDestroyDuration);
				_isEndUpdate = true;
			}
		}

		private void Explode()
		{
			Transform targetTransform = null;
			FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightSummonTrap>(base.transform, Vector3.zero, targetTransform, base.ownerCharacter);
		}
	}
}
