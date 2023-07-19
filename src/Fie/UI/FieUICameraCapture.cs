using Fie.Manager;
using UnityEngine;

namespace Fie.UI
{
	public class FieUICameraCapture : MonoBehaviour
	{
		private Canvas selfCanvas;

		private bool isInitialized;

		private void Start()
		{
			selfCanvas = base.gameObject.GetComponent<Canvas>();
		}

		private void Update()
		{
			if (!isInitialized && !(selfCanvas == null) && !(FieManagerBehaviour<FieGUIManager>.I.uiCamera == null))
			{
				selfCanvas.worldCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera.camera;
			}
		}
	}
}
