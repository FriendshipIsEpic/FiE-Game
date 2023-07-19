using Fie.Object;
using ParticlePlayground;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Common/ChangelingForcecArrivalParticle")]
	public class FieEmitObjectChangelingForcesArrivalParticleEffect : FieEmittableObjectBase
	{
		[SerializeField]
		private float arrivalParticleDuration = 1.5f;

		[SerializeField]
		private float arrivalDuration = 3f;

		[SerializeField]
		private List<PlaygroundParticlesC> particles;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		public void Awake()
		{
		}

		public void StopEffect()
		{
			if (!_isEndUpdate)
			{
				foreach (PlaygroundParticlesC particle in particles)
				{
					if (particle != null)
					{
						particle.emit = false;
					}
				}
				_isEndUpdate = true;
				destoryEmitObject(arrivalParticleDuration - arrivalDuration);
			}
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
				if (_lifeTimeCount >= arrivalDuration)
				{
					foreach (PlaygroundParticlesC particle in particles)
					{
						if (particle != null)
						{
							particle.emit = false;
						}
					}
					_isEndUpdate = true;
					destoryEmitObject(arrivalParticleDuration - arrivalDuration);
				}
			}
		}
	}
}
