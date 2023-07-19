using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisCrucibleBurst")]
	public class FieEmitObjectQueenChrysalisCrucibleBurst : FieEmittableObjectBase
	{
		[SerializeField]
		private float CRUCIBLE_BURST_DURATION = 3f;

		[SerializeField]
		private float CRUCIBLE_BURST_DAMAGE_DURATION = 0.75f;

		[SerializeField]
		private float CRUCIBLE_BURST_DAMAGE_START_DELAY = 0.5f;

		private float _lifeTimeCount;

		private bool _isEndTrail;

		private bool _isEndUpdate;

		public override void awakeEmitObject()
		{
			base.transform.rotation = Quaternion.LookRotation(directionalVec);
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= CRUCIBLE_BURST_DURATION)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount < CRUCIBLE_BURST_DAMAGE_START_DELAY) && !(_lifeTimeCount > CRUCIBLE_BURST_DAMAGE_DURATION + CRUCIBLE_BURST_DAMAGE_START_DELAY) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter x = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (x != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHitEffectSmall>(base.transform, Vector3.zero).transform.position = collider.ClosestPointOnBounds(base.transform.position);
				}
			}
		}
	}
}
