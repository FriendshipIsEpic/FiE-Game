using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Changeling/Power/ChangelingVortex")]
	public class FieEmitObjectChangelingVortex : FieEmittableObjectBase
	{
		[SerializeField]
		private float VortexDuration = 1f;

		[SerializeField]
		private float VortexDamageDuration = 0.75f;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= VortexDuration)
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
			if (!_isEndUpdate && !(_lifeTimeCount > VortexDamageDuration) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter x = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (x != null)
				{
					Vector3 position = collider.ClosestPointOnBounds(base.transform.position);
					FieEmitObjectChangelingBiteHitEffect fieEmitObjectChangelingBiteHitEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingBiteHitEffect>(base.transform, Vector3.zero);
					if (fieEmitObjectChangelingBiteHitEffect != null)
					{
						fieEmitObjectChangelingBiteHitEffect.transform.position = position;
					}
				}
			}
		}
	}
}
