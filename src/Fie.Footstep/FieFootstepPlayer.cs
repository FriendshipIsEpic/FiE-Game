using ParticlePlayground;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fie.Footstep {
	/// <summary>
	/// An audio player for playing audio clips under a player.
	/// </summary>
	public class FieFootstepPlayer : MonoBehaviour {
		[SerializeField]
		private AudioSource _audioSource;

		[SerializeField]
		private PlaygroundParticlesC _particle;

		[SerializeField]
		private float _pitchOffset = 1f;

		private float _particleDuration;

		public AudioSource audioSource => _audioSource;

		public float pitchOffset => _pitchOffset;

		public void EmitFootstepParticle(float duration) {
			_particleDuration = duration;
		}

		public void SetMaterial(Material material) {
			_particle.particleSystemRenderer.material = material;
		}

		private void Update() {
			bool flag = _particleDuration > 0;
			_particle.emit = flag;
			if (flag) {
				_particleDuration -= Time.deltaTime;
			}
		}

		private void Awake() {
			SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		}

		/// <summary>
		/// Called when the scene is loaded.
		/// </summary>
		private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode) {
			if ((bool)_particle) {
				_particle.Start();
			}
		}
	}
}
