using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwiilghtSummonTrapExplosion")]
	public class FieEmitObjectTwilightSummonTrap : FieEmittableObjectBase
	{
		[SerializeField]
		private float ExplodeDuration = 0.5f;

		[SerializeField]
		private float ExplodeWarmup = 0.5f;

		[SerializeField]
		private float ExplodeDestroyDuration = 1f;

		[SerializeField]
		private float ExplodePysicalForce = 1f;

		private bool _isEndUpdate;

		private float _lifeCount;

		private void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeCount += Time.deltaTime;
				if (_lifeCount > ExplodeDuration)
				{
					destoryEmitObject(ExplodeDestroyDuration);
					_isEndUpdate = true;
				}
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeCount > ExplodeDuration + ExplodeWarmup) && !(_lifeCount < ExplodeWarmup) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (fieGameCharacter != null)
				{
					Vector3 a = collider.ClosestPointOnBounds(base.transform.position);
					Vector3 vector = a - base.transform.position;
					vector = new Vector3(vector.x, 0f, vector.z).normalized;
					fieGameCharacter.resetMoveForce();
					fieGameCharacter.setMoveForce(vector * ExplodePysicalForce, 0f, useRound: false);
				}
			}
		}
	}
}
