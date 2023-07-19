using Fie.Manager;
using UnityEngine;

namespace Fie.UI
{
	public class FieLobbyCanvasGUICameraCapture : MonoBehaviour
	{
		private Canvas selfCanvas;

		private void Start()
		{
			selfCanvas = base.gameObject.GetComponent<Canvas>();
		}

		private void Update()
		{
			if (!(selfCanvas == null) && !(FieManagerBehaviour<FieGUIManager>.I.uiCamera == null))
			{
				selfCanvas.worldCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera.camera;
			}
		}
	}
}
