using Fie.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Footstep {
	/// <summary>
	/// Plays the footstep animations created as players or enemies move around.
	/// </summary>
	public class FieFootstepMaterial : MonoBehaviour {
		[SerializeField]
		private float _audioVolume = 0.5f;

		[SerializeField]
		private List<AudioClip> _audioClips = new List<AudioClip>();

		/// <summary>
		/// The particle material to spawn under an entity's hooves as they walk.
		/// </summary>
		[SerializeField]
		private Material _footStepParticleMaterial;

		/// <summary>
		/// A lotto of audio clips to play. Provides a random one when queried.
		/// </summary>
		private Lottery<AudioClip> _lottery = new Lottery<AudioClip>();

		/// <summary>
		/// The pool of sounds that can be played by this material.
		/// </summary>
		public List<AudioClip> audioClips => _audioClips;

		private void Awake() {
			foreach (AudioClip audioClip in _audioClips) {
				_lottery.AddItem(audioClip); // add each audio clip to the lottery
			}
		}

		/// <summary>
		/// Attempts to play an audio clip for the given footstep player.
		/// Accepts nulls.
		/// </summary>
		public void playFootstepAudio(FieFootstepPlayer player) {
			if (!(player == null) && _lottery.IsExecutable()) {
				AudioClip audioClip = _lottery.Lot();

				if (!(audioClip == null)) {
					player.SetMaterial(_footStepParticleMaterial);

					player.audioSource.pitch = player.pitchOffset;
					player.audioSource.PlayOneShot(audioClip, _audioVolume);

					player.EmitFootstepParticle(audioClip.length);
				}
			}
		}
	}
}
