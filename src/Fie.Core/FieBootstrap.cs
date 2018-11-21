using Fie.Manager;
using Fie.Scene;
using UnityEngine;

namespace Fie.Core {
	/// <summary>
	/// Entry-point for Fie. Everything starts here.
	/// </summary>
	public class FieBootstrap : MonoBehaviour {
		public static bool isBootedFromBootStrap { get; private set; };

		/// <summary>
		/// Starts the game. Loads the title screen.
		/// </summary>
		private void Start() {
			FieManagerFactory.I.StartUp();
			FieManagerBehaviour<FieSceneManager>.I.LoadScene(new FieSceneTitle(), allowSceneActivation: true,
				FieFaderManager.FadeType.OUT_TO_WHITE,
				FieFaderManager.FadeType.IN_FROM_WHITE,
				1.5f
			);

			isBootedFromBootStrap = true;

			FieManagerBehaviour<FieAudioManager>.I.ChangeMixerVolume(0f, 0.5f,
				FieAudioManager.FieAudioMixerType.BGM,
				FieAudioManager.FieAudioMixerType.Voice,
				FieAudioManager.FieAudioMixerType.SE
			);

			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
