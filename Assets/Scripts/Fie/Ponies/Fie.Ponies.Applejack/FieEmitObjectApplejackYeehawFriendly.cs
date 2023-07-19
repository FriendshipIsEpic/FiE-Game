using Fie.Object;
using ParticlePlayground;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Power/ApplejackYeehawFriendly")]
	public class FieEmitObjectApplejackYeehawFriendly : FieEmittableObjectBase
	{
		[SerializeField]
		private float yeehawRegenDuration = 13f;

		[SerializeField]
		private float yeehawRegenEnableDuration = 10f;

		[SerializeField]
		private PlaygroundParticlesC particle;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		public override void awakeEmitObject()
		{
			particle.emit = true;
			base.ownerCharacter.damageSystem.AddDefenceMagni(-1, 0.25f, yeehawRegenEnableDuration);
		}

		public void Update()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= yeehawRegenEnableDuration)
				{
					_isEndUpdate = true;
					destoryEmitObject(yeehawRegenDuration - yeehawRegenEnableDuration);
				}
			}
		}
	}
}
