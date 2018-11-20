using ParticlePlayground;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fie.Footstep
{
	public class FieFootstepPlayer : MonoBehaviour
	{
		[SerializeField]
		private AudioSource _audioSource;

		[SerializeField]
		private PlaygroundParticlesC _particle;

		[SerializeField]
		private float _pitchOffset = 1f;

		private float _particleDuration;

		public AudioSource audioSource => _audioSource;

		public float pitchOffset => _pitchOffset;

		public void EmitFootstepParticle(float duration)
		{
			_particleDuration = duration;
		}

		public void SetMaterial(Material material)
		{
			_particle.particleSystemRenderer.material = material;
		}

		private void Update()
		{
			bool flag = _particleDuration > 0f;
			_particle.emit = flag;
			if (flag)
			{
				_particleDuration -= Time.deltaTime;
			}
		}

		private void Awake()
		{
			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		}

		private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
		{
			if ((bool)_particle)
			{
				_particle.Start();
			}
		}
	}
}
