using Fie.Utility;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Fie.Fader
{
	/// <summary>
	/// Fader used to white out the screen whilst the game is loading.
	/// </summary>
	public class FieFader : MonoBehaviour
	{
		[SerializeField]
		private GameObject faderCameraRootObject;

		[SerializeField]
		private Image faderPlaneObject;

		/// <summary>
		/// The loading icon that appears in the lower right corner.
		/// </summary>
		[SerializeField]
		private SkeletonGraphic _loadingIcon;

		private Tweener<TweenTypesInOutSine> faderTweener = new Tweener<TweenTypesInOutSine>();

		/// <summary>
		/// True if the loading screen is currently visible.
		/// </summary>
		private bool isDrawFader;

		public void Initialize() {
			faderCameraRootObject.transform.gameObject.SetActive(value: false);
		}

		public void HideFader() {
			isDrawFader = false;
			faderCameraRootObject.transform.gameObject.SetActive(value: false);
		}

		/// <summary>
		/// Creates this view, sets it to visible, and starts showing the loading screen.
		/// </summary>
		public void InitFader(Vector4 startColor, Vector4 endColor, float time) {
			isDrawFader = true;
			faderTweener.InitTweener(time, startColor, endColor);
			faderCameraRootObject.transform.gameObject.SetActive(value: true);
		}

		/// <summary>
		/// Checks if the loading screen is still fading in/our and returns false otherwise.
		/// </summary>
		public bool IsEndUpdateFader() {
			return faderTweener == null || faderTweener.IsEnd();
		}

		/// <summary>
		/// Draws the loading screen. Currently just a plain white background.
		/// </summary>
		private void Update() {
			if (isDrawFader) {
				Color color = faderTweener.UpdateParameterVec4(Time.deltaTime);
				faderPlaneObject.color = color;
				_loadingIcon.color = new Color(1f, 1f, 1f, color.a);
			}
		}

		public void ShowLoadScreen() {
			// TODO: Implement this
		}

		public void HideLoadScreen() {
			// TODO: Implement this
		}
	}
}
