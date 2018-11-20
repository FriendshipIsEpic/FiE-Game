using Fie.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Footstep
{
	public class FieFootstepMaterial : MonoBehaviour
	{
		[SerializeField]
		private float _audioVolume = 0.5f;

		[SerializeField]
		private List<AudioClip> _audioClips = new List<AudioClip>();

		[SerializeField]
		private Material _footStepParticleMaterial;

		private Lottery<AudioClip> _lottery = new Lottery<AudioClip>();

		public List<AudioClip> audioClips => _audioClips;

		private void Awake()
		{
			foreach (AudioClip audioClip in _audioClips)
			{
				_lottery.AddItem(audioClip);
			}
		}

		public void playFootstepAudio(FieFootstepPlayer player)
		{
			if (!(player == null) && _lottery.IsExecutable())
			{
				AudioClip audioClip = _lottery.Lot();
				if (!(audioClip == null))
				{
					player.SetMaterial(_footStepParticleMaterial);
					player.audioSource.pitch = player.pitchOffset;
					player.audioSource.PlayOneShot(audioClip, _audioVolume);
					player.EmitFootstepParticle(audioClip.length);
				}
			}
		}
	}
}
