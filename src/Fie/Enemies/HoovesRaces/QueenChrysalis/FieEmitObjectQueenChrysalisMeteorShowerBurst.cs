using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisMeteorBurst")]
	public class FieEmitObjectQueenChrysalisMeteorShowerBurst : FieEmittableObjectBase
	{
		[SerializeField]
		private float MeteorShowerBurstDuration = 0.7f;

		[SerializeField]
		private float MeteorShowerBurstDestroyDuration = 0.7f;

		private bool _isEndUpdate;

		private float _lifeCount;

		public override void awakeEmitObject()
		{
			base.transform.rotation = Quaternion.LookRotation(directionalVec);
		}

		private void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeCount += Time.deltaTime;
				if (_lifeCount > MeteorShowerBurstDuration)
				{
					destoryEmitObject(MeteorShowerBurstDestroyDuration);
					_isEndUpdate = true;
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && collider.gameObject.tag == getHostileTagString())
			{
				addDamageToCollisionCharacter(collider, getDefaultDamageObject());
			}
		}
	}
}
