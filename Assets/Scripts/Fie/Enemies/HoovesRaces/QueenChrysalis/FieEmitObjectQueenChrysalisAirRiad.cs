using Fie.Manager;
using Fie.Object;
using ParticlePlayground;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisAirRaidEffectMain")]
	public class FieEmitObjectQueenChrysalisAirRiad : FieEmittableObjectBase
	{
		[SerializeField]
		private float AirRaidBurstDuration = 0.5f;

		[SerializeField]
		private float AirRaidAttackBurstWarmup = 1.5f;

		[SerializeField]
		private float AirRaidBurstDestroyDuration = 1.5f;

		[SerializeField]
		private PlaygroundParticlesC _asteroidParticle;

		private bool _isEndUpdate;

		private float _lifeCount;

		private bool _isEmittedAsteroid;

		public override void awakeEmitObject()
		{
			base.transform.rotation = Quaternion.identity;
			_asteroidParticle.emit = false;
		}

		private void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeCount += Time.deltaTime;
				if (_lifeCount > AirRaidAttackBurstWarmup && !_isEmittedAsteroid)
				{
					FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(0.8f, 20, new Vector3(0.1f, 0.6f));
					_asteroidParticle.emit = true;
					_isEmittedAsteroid = true;
				}
				if (_lifeCount > AirRaidBurstDuration + AirRaidAttackBurstWarmup)
				{
					destoryEmitObject(AirRaidBurstDestroyDuration);
					_isEndUpdate = true;
					_asteroidParticle.emit = false;
				}
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeCount > AirRaidBurstDuration + AirRaidAttackBurstWarmup) && !(_lifeCount < AirRaidAttackBurstWarmup) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter x = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (x != null)
				{
					Vector3 position = collider.ClosestPointOnBounds(base.transform.position);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHitEffectSmall>(base.transform, Vector3.zero).transform.position = position;
				}
			}
		}
	}
}
