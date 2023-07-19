using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisDoubleSlashFirst")]
	public class FieEmitObjectQueenChrysalisDoubleSlashFirst : FieEmittableObjectBase
	{
		[SerializeField]
		private float DOUBLE_SLASH_DURATION = 1.5f;

		[SerializeField]
		private float DOUBLE_SLASH_DAMAGE_DURATION = 0.3f;

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
				if (_lifeTimeCount >= DOUBLE_SLASH_DURATION)
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
			if (!_isEndUpdate && !(_lifeTimeCount > DOUBLE_SLASH_DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
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
