using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/ChangelingAlpha/Power/ChangelingAlphaCharge")]
	public class FieEmitObjectChangelingAlphaCharge : FieEmittableObjectBase
	{
		[SerializeField]
		private float ChargeDuration = 0.4f;

		[SerializeField]
		private float ChargeDamageDuration = 0.3f;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= ChargeDuration)
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
			if (!_isEndUpdate && !(_lifeTimeCount > ChargeDamageDuration))
			{
				if (collider.gameObject.tag == getHostileTagString())
				{
					FieGameCharacter x = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
					if (x != null)
					{
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingAlphaHitEffect>(base.transform, Vector3.zero);
						destoryEmitObject(ChargeDuration - _lifeTimeCount);
					}
				}
				FieEmittableObjectBase component = collider.GetComponent<FieEmittableObjectBase>();
				if (component != null && reflectEmitObject(component))
				{
					Vector3 vector = collider.ClosestPointOnBounds(base.transform.position);
					Vector3 vector2 = vector - base.transform.position;
					FieEmitObjectChangelingAlphaReflectionEffect fieEmitObjectChangelingAlphaReflectionEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingAlphaReflectionEffect>(base.transform, vector2.normalized);
					if (fieEmitObjectChangelingAlphaReflectionEffect != null)
					{
						fieEmitObjectChangelingAlphaReflectionEffect.transform.position = vector;
					}
				}
			}
		}
	}
}
