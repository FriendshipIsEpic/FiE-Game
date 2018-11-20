using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisPenetrateShotBurst")]
	public class FieEmitObjectQueenChrysalisPenetrateShotBurst : FieEmittableObjectBase
	{
		[SerializeField]
		private float PenetrateShotBurstDuration = 0.7f;

		[SerializeField]
		private float PenetrateShotBurstDestroyDuration = 0.5f;

		private bool _isEndUpdate;

		private float _lifeCount;

		private void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeCount += Time.deltaTime;
				if (_lifeCount > PenetrateShotBurstDuration)
				{
					destoryEmitObject(PenetrateShotBurstDestroyDuration);
					_isEndUpdate = true;
				}
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			if (!_isEndUpdate && collider.gameObject.tag == getHostileTagString())
			{
				addDamageToCollisionCharacter(collider, getDefaultDamageObject());
			}
		}
	}
}
