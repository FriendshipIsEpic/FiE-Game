using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisRecoveringAttack")]
	public class FieEmitObjectQueenChrysalisStaggerRecoveringBurst : FieEmittableObjectBase
	{
		[SerializeField]
		private float RecoverAttackBurstDuration = 0.5f;

		[SerializeField]
		private float RecoverAttackBurstWarmup = 0.3f;

		[SerializeField]
		private float RecorverAttackBurstDestroyDuration = 1f;

		[SerializeField]
		private float RecorverAttackPysicalForce = 5f;

		private bool _isEndUpdate;

		private float _lifeCount;

		private void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeCount += Time.deltaTime;
				if (_lifeCount > RecoverAttackBurstDuration)
				{
					destoryEmitObject(RecorverAttackBurstDestroyDuration);
					_isEndUpdate = true;
				}
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeCount > RecoverAttackBurstDuration + RecoverAttackBurstWarmup) && !(_lifeCount < RecoverAttackBurstWarmup))
			{
				if (collider.gameObject.tag == getHostileTagString())
				{
					FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
					if (fieGameCharacter != null)
					{
						Vector3 a = collider.ClosestPointOnBounds(base.transform.position);
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHitEffectBurned>(fieGameCharacter.centerTransform, Vector3.zero);
						Vector3 vector = a - base.transform.position;
						vector = new Vector3(vector.x, 0f, vector.z).normalized;
						fieGameCharacter.resetMoveForce();
						fieGameCharacter.setMoveForce(vector * RecorverAttackPysicalForce, 0f, useRound: false);
					}
				}
				FieEmittableObjectBase component = collider.GetComponent<FieEmittableObjectBase>();
				if (component != null && reflectEmitObject(component))
				{
					Vector3 vector2 = collider.ClosestPointOnBounds(base.transform.position);
					Vector3 vector3 = vector2 - base.transform.position;
					FieEmitObjectQueenChrysalisReflectionEffect fieEmitObjectQueenChrysalisReflectionEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisReflectionEffect>(base.transform, vector3.normalized);
					if (fieEmitObjectQueenChrysalisReflectionEffect != null)
					{
						fieEmitObjectQueenChrysalisReflectionEffect.transform.position = vector2;
					}
				}
			}
		}
	}
}
