using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/ChangelingAlpha/Power/ChangelingAlphaBurst")]
	public class FieEmitObjectChangelingAlphaBurst : FieEmittableObjectBase
	{
		[SerializeField]
		private float ChangelingAlphaBurstDuration = 0.7f;

		[SerializeField]
		private float ChangelingAlphaBurstDestroyDuration = 0.5f;

		private bool _isEndUpdate;

		private float _lifeCount;

		private void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeCount += Time.deltaTime;
				if (_lifeCount > ChangelingAlphaBurstDuration)
				{
					destoryEmitObject(ChangelingAlphaBurstDestroyDuration);
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
