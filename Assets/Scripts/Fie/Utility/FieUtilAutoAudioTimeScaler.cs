using UnityEngine;

namespace Fie.Utility
{
	public class FieUtilAutoAudioTimeScaler : MonoBehaviour
	{
		private float _defaultPitch = 1f;

		private AudioSource _audioSource;

		private void Awake()
		{
			_audioSource = base.gameObject.GetComponent<AudioSource>();
			if (_audioSource != null)
			{
				_defaultPitch = _audioSource.pitch;
			}
		}

		private void Update()
		{
			if (!(_audioSource == null))
			{
				_audioSource.pitch = _defaultPitch * Time.timeScale;
			}
		}
	}
}
