using Fie.Utility;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Fie.Fader
{
	public class FieFader : MonoBehaviour
	{
		[SerializeField]
		private GameObject faderCameraRootObject;

		[SerializeField]
		private Image faderPlaneObject;

		[SerializeField]
		private SkeletonGraphic _loadingIcon;

		private Tweener<TweenTypesInOutSine> faderTweener = new Tweener<TweenTypesInOutSine>();

		private bool isDrawFader;

		public void Initialize()
		{
			faderCameraRootObject.transform.gameObject.SetActive(value: false);
		}

		public void HideFader()
		{
			isDrawFader = false;
			faderCameraRootObject.transform.gameObject.SetActive(value: false);
		}

		public void InitFader(Vector4 startColor, Vector4 endColor, float time)
		{
			isDrawFader = true;
			faderTweener.InitTweener(time, startColor, endColor);
			faderCameraRootObject.transform.gameObject.SetActive(value: true);
		}

		public bool IsEndUpdateFader()
		{
			return faderTweener == null || faderTweener.IsEnd();
		}

		private void Update()
		{
			if (isDrawFader)
			{
				Color color = faderTweener.UpdateParameterVec4(Time.deltaTime);
				faderPlaneObject.color = color;
				_loadingIcon.color = new Color(1f, 1f, 1f, color.a);
			}
		}

		public void ShowLoadScreen()
		{
		}

		public void HideLoadScreen()
		{
		}
	}
}
