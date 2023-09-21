using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/Power/RisingSunEmission")]
	public class FieEmitObjectRisingSunEmission : FieEmittableObjectBase
	{
		[SerializeField]
		private float EmissionDuration = 1f;

		[SerializeField]
		private float EmissionDamageDuration = 0.75f;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= EmissionDuration)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
				if (initTransform != null)
				{
					base.transform.position = initTransform.position;
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > EmissionDamageDuration) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter x = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (x != null)
				{
					Vector3 position = collider.ClosestPointOnBounds(base.transform.position);
					FieEmitObjectRisingSunHitEffectSmall fieEmitObjectRisingSunHitEffectSmall = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRisingSunHitEffectSmall>(base.transform, Vector3.zero);
					if (fieEmitObjectRisingSunHitEffectSmall != null)
					{
						fieEmitObjectRisingSunHitEffectSmall.transform.position = position;
					}
				}
			}
		}
	}
}
