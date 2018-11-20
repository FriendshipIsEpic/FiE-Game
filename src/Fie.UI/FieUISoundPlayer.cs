using UnityEngine;
using UnityEngine.EventSystems;

namespace Fie.UI
{
	[RequireComponent(typeof(AudioSource))]
	public class FieUISoundPlayer : MonoBehaviour, ISubmitHandler, IDeselectHandler, ISelectHandler, IEventSystemHandler
	{
		[SerializeField]
		private AudioClip _exitSound;

		[SerializeField]
		private AudioClip _enterSound;

		[SerializeField]
		private AudioClip _clickedSound;

		private AudioSource _audioSource;

		public void Awake()
		{
			_audioSource = GetComponent<AudioSource>();
		}

		public void OnDeselect(BaseEventData eventData)
		{
			if (_exitSound != null)
			{
				_audioSource.PlayOneShot(_exitSound);
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			if (_enterSound != null)
			{
				_audioSource.PlayOneShot(_enterSound);
			}
		}

		public void OnSubmit(BaseEventData eventData)
		{
			if (_clickedSound != null)
			{
				_audioSource.PlayOneShot(_clickedSound);
			}
		}
	}
}
