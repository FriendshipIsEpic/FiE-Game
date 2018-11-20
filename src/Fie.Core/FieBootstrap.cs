using Fie.Manager;
using Fie.Scene;
using UnityEngine;

namespace Fie.Core
{
	public class FieBootstrap : MonoBehaviour
	{
		private static bool _isBootedFromBootStrap;

		public static bool isBootedFromBootStrap => _isBootedFromBootStrap;

		private void Start()
		{
			FieManagerFactory.I.StartUp();
			FieManagerBehaviour<FieSceneManager>.I.LoadScene(new FieSceneTitle(), allowSceneActivation: true, FieFaderManager.FadeType.OUT_TO_WHITE, FieFaderManager.FadeType.IN_FROM_WHITE, 1.5f);
			_isBootedFromBootStrap = true;
			FieManagerBehaviour<FieAudioManager>.I.ChangeMixerVolume(0f, 0.5f, FieAudioManager.FieAudioMixerType.BGM, FieAudioManager.FieAudioMixerType.Voice, FieAudioManager.FieAudioMixerType.SE);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
