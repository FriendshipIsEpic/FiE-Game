using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisIgniteBurst")]
	public class FieEmitObjectQueenChrysalisIgniteBurst : FieEmittableObjectBase
	{
		[SerializeField]
		private float IGNITE_BURST_DURATION = 3f;

		[SerializeField]
		private float IGNITE_DAMAGE_DURATION = 0.5f;

		private float _lifeCount;

		public override void awakeEmitObject()
		{
			destoryEmitObject(IGNITE_BURST_DURATION);
		}

		public void AddDamageForGameCharacter(FieGameCharacter targetCharacter)
		{
			if (!(targetCharacter == null))
			{
				FieGameCharacter x = addDamageToCharacter(targetCharacter, getDefaultDamageObject(), isPenetration: true);
				if (x != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHitEffectBurned>(targetCharacter.centerTransform, Vector3.zero);
				}
			}
		}

		private void Update()
		{
			_lifeCount += Time.deltaTime;
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!(_lifeCount > IGNITE_DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
			{
				FieDamage defaultDamageObject = getDefaultDamageObject();
				defaultDamageObject.damage *= 0.5f;
				FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, defaultDamageObject, isPenetration: true);
				if (fieGameCharacter != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHitEffectBurned>(fieGameCharacter.centerTransform, Vector3.zero);
				}
			}
		}
	}
}
