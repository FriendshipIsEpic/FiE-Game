using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/Power/RisingSunHitEffectMiddle")]
	public class FieEmitObjectRisingSunHitEffectMiddle : FieEmittableObjectBase
	{
		[SerializeField]
		private float RisingSunBurstDuration = 0.7f;

		[SerializeField]
		private float RisingSunBurstDestroyDuration = 0.5f;

		private bool _isEndUpdate;

		private float _lifeCount;

		private void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeCount += Time.deltaTime;
				if (_lifeCount > RisingSunBurstDuration)
				{
					destoryEmitObject(RisingSunBurstDestroyDuration);
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
