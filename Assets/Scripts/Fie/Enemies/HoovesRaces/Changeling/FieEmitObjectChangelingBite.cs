using Fie.Manager;
using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Changeling/Power/ChangelingBite")]
	public class FieEmitObjectChangelingBite : FieEmittableObjectBase
	{
		[SerializeField]
		private float CHANGELING_BITE_DURATION = 0.4f;

		[SerializeField]
		private float CHANGELING_BITE_DAMAGE_DURATION = 0.3f;

		private float _lifeTimeCount;

		private bool _isEndTrail;

		private bool _isEndUpdate;

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= CHANGELING_BITE_DURATION)
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
			if (!_isEndUpdate && !(_lifeTimeCount > CHANGELING_BITE_DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter x = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (x != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingBiteHitEffect>(base.transform, Vector3.zero);
				}
			}
		}
	}
}
