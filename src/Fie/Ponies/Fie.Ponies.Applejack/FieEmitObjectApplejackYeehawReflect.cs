using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Power/ApplejackYeehawReflect")]
	public class FieEmitObjectApplejackYeehawReflect : FieEmittableObjectBase
	{
		[SerializeField]
		private float yeehawEnableDuration = 0.3f;

		[SerializeField]
		private float yeehawPysicalForce = 1.5f;

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
				if (_lifeTimeCount >= yeehawEnableDuration)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate)
			{
				if (collider.gameObject.tag == getHostileTagString())
				{
					FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
					if (fieGameCharacter != null)
					{
						Vector3 a = collider.ClosestPointOnBounds(base.transform.position);
						Vector3 vector = a - base.transform.position;
						vector = new Vector3(vector.x, 0f, vector.z).normalized;
						fieGameCharacter.resetMoveForce();
						fieGameCharacter.setMoveForce(vector * yeehawPysicalForce, 0f, useRound: false);
					}
				}
				FieEmittableObjectBase component = collider.GetComponent<FieEmittableObjectBase>();
				if (component != null)
				{
					reflectEmitObject(component);
				}
			}
		}
	}
}
