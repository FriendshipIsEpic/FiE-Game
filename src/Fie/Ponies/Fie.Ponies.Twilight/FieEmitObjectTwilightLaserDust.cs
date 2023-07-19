using Fie.Object;
using ParticlePlayground;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightLaserDust")]
	public class FieEmitObjectTwilightLaserDust : FieEmittableObjectBase
	{
		[SerializeField]
		private float DustDuration = 3f;

		[SerializeField]
		private float DustEmitDuration = 1.8f;

		[SerializeField]
		private List<PlaygroundParticlesC> _childParticles = new List<PlaygroundParticlesC>();

		private float _lifeTime;

		private bool _isEndUpdate;

		public void setDuration(float duration)
		{
			DustEmitDuration = duration;
		}

		public override void awakeEmitObject()
		{
			foreach (PlaygroundParticlesC childParticle in _childParticles)
			{
				childParticle.Emit(setEmission: true);
			}
		}

		private void Update()
		{
			if (!_isEndUpdate)
			{
				base.transform.position = initTransform.position;
				_lifeTime += Time.deltaTime;
				if (_lifeTime > DustEmitDuration)
				{
					foreach (PlaygroundParticlesC childParticle in _childParticles)
					{
						childParticle.Emit(setEmission: false);
					}
					_isEndUpdate = true;
					destoryEmitObject(DustDuration - _lifeTime);
				}
			}
		}

		internal void Stop()
		{
			_lifeTime = DustEmitDuration;
		}
	}
}
