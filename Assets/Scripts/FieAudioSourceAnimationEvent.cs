using UnityEngine;

public class FieAudioSourceAnimationEvent : MonoBehaviour
{
	[SerializeField]
	private AudioSource _audioSource;

	public void Play()
	{
		if (_audioSource != null)
		{
			_audioSource.Play();
		}
	}
}
