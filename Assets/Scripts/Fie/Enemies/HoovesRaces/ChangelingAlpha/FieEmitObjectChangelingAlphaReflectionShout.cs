using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/ChangelingAlpha/Power/ChangelingAlphaReflectionShout")]
	public class FieEmitObjectChangelingAlphaReflectionShout : FieEmittableObjectBase
	{
		[SerializeField]
		private float shoutDuration = 1.5f;

		[SerializeField]
		private float ShoutEnableDuration = 0.8f;

		private float _lifeTimeCount;

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
				if (_lifeTimeCount >= shoutDuration)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > ShoutEnableDuration))
			{
				if (collider.gameObject.tag == getHostileTagString())
				{
					addDamageToCollisionCharacter(collider, getDefaultDamageObject());
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
