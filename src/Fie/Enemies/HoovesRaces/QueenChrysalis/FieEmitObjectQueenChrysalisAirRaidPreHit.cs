using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisAirRaidPreEffect")]
	public class FieEmitObjectQueenChrysalisAirRaidPreHit : FieEmittableObjectBase
	{
		private const float DURATION = 2f;

		[SerializeField]
		private float _damageDuration = 0.3f;

		private float _lifeCount;

		public override void awakeEmitObject()
		{
			base.transform.rotation = Quaternion.identity;
			destoryEmitObject(2f);
		}

		private void Update()
		{
			_lifeCount += Time.deltaTime;
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!(_lifeCount > _damageDuration) && collider.gameObject.tag == getHostileTagString())
			{
				addDamageToCollisionCharacter(collider, getDefaultDamageObject());
			}
		}
	}
}
