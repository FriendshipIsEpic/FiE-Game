using Fie.Object;
using ParticlePlayground;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/Power/RisingSunLaserDust")]
	public class FieEmitObjectRisingSunLaserDust : FieEmittableObjectBase
	{
		[SerializeField]
		private float DustDuration = 3f;

		[SerializeField]
		private float DustEmitDuration = 1.8f;

		[SerializeField]
		private List<PlaygroundParticlesC> _childParticles = new List<PlaygroundParticlesC>();

		private float _lifeTime;

		private bool _isEndUpdate;

		private void Update()
		{
			if (!_isEndUpdate)
			{
				base.transform.position = initTransform.position;
				_lifeTime += Time.deltaTime;
				if (_lifeTime > DustEmitDuration)
				{
					_childParticles.ForEach(delegate(PlaygroundParticlesC particle)
					{
						particle.Emit(setEmission: false);
					});
					_isEndUpdate = true;
					destoryEmitObject(DustDuration - _lifeTime);
				}
			}
		}
	}
}
