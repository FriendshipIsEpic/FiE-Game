using Fie.Manager;
using Fie.Scene;
using UnityEngine;

namespace Fie.Utility
{
	public class FieUtilBGM : MonoBehaviour
	{
		public bool _autoStart = true;

		private AudioSource _audioSource;

		private void Awake()
		{
			_audioSource = base.gameObject.GetComponent<AudioSource>();
		}

		private void Start()
		{
			if (FieManagerFactory.I.currentSceneType == FieSceneType.INGAME)
			{
				FieManagerBehaviour<FieInGameStateManager>.I.RetryEvent += RetryEvent;
			}
		}

		private void RetryEvent()
		{
			if (!_autoStart)
			{
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
