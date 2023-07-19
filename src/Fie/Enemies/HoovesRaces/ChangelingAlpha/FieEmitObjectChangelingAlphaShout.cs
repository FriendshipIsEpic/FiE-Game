using Fie.Object;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/ChangelingAlpha/Power/ChangelingAlphaShout")]
	public class FieEmitObjectChangelingAlphaShout : FieEmittableObjectBase
	{
		[SerializeField]
		private float shoutDuration = 1.5f;

		[SerializeField]
		private float ShoutEnableDuration = 0.8f;

		[SerializeField]
		private float ShoutPysicalForce = 5f;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		public void Update()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= shoutDuration)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > ShoutEnableDuration) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (fieGameCharacter != null)
				{
					fieGameCharacter.resetMoveForce();
					fieGameCharacter.setMoveForce((fieGameCharacter.centerTransform.position - base.transform.position).normalized * ShoutPysicalForce, 0f, useRound: false);
				}
			}
		}
	}
}
