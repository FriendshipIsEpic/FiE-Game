using UnityEngine;

namespace Fie.Voice
{
	[RequireComponent(typeof(AudioSource))]
	public class FieVoiceSpeaker : MonoBehaviour
	{
		private AudioSource _speaker;

		public AudioSource audioSource => _speaker;

		private void Awake()
		{
			_speaker = base.gameObject.GetComponent<AudioSource>();
		}
	}
}
