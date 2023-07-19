using System.Collections.Generic;
using UnityEngine;

namespace Fie.Utility
{
	[RequireComponent(typeof(AudioSource))]
	public class FieUtilRandomAudioPlayer : MonoBehaviour
	{
		[SerializeField]
		private List<AudioClip> _audioClips = new List<AudioClip>();

		private Lottery<AudioClip> _lotter = new Lottery<AudioClip>();

		private AudioSource _audioSource;

		private void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
			if (_audioClips != null && _audioClips.Count > 0)
			{
				_lotter.InitializeFromListData(_audioClips);
			}
		}

		private void OnEnable()
		{
			if (_lotter.IsExecutable() && !(_audioSource == null))
			{
				_audioSource.PlayOneShot(_lotter.Lot());
			}
		}
	}
}
