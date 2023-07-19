using Fie.Manager;
using UnityEngine;

namespace Fie.UI
{
	public class FieUIGamePadEventSender : MonoBehaviour
	{
		[SerializeField]
		private FieInputManager.FieInputUIKeyType keyType;

		[SerializeField]
		private FieUIMouseEvent uiElement;

		private void Start()
		{
			FieManagerBehaviour<FieInputManager>.I.uiInputEvent += I_uiInputEvent;
		}

		private void I_uiInputEvent(FieInputManager.FieInputUIKeyType keyType)
		{
			if ((keyType & this.keyType) == this.keyType)
			{
				uiElement.ExecuteGamepadDecide();
			}
		}
	}
}
