using Fie.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.UI
{
	public class FieUIGamePadSelector : MonoBehaviour
	{
		public enum AxisType
		{
			VIRTICAL,
			HORIZONTAL
		}

		[SerializeField]
		private AxisType axisType;

		[SerializeField]
		private List<FieUIMouseEvent> uiElements = new List<FieUIMouseEvent>();

		[SerializeField]
		private int defaultSelectIndex = -1;

		private int currentIndex;

		private void Start()
		{
			if (uiElements.Count > 0)
			{
				FieManagerBehaviour<FieInputManager>.I.uiInputEvent += I_uiInputEvent;
				currentIndex = Mathf.Clamp(defaultSelectIndex, 0, uiElements.Count - 1);
				uiElements[currentIndex].ExecuteOverEvent();
				FieManagerBehaviour<FieInputManager>.I.SetUIControlMode(FieInputManager.FieInputUIControlMode.GAME_PAD);
			}
		}

		private void I_uiInputEvent(FieInputManager.FieInputUIKeyType keyType)
		{
			int num = 0;
			if (axisType == AxisType.VIRTICAL)
			{
				if ((keyType & FieInputManager.FieInputUIKeyType.KEY_TYPE_UP) == FieInputManager.FieInputUIKeyType.KEY_TYPE_UP)
				{
					num = -1;
				}
				if ((keyType & FieInputManager.FieInputUIKeyType.KEY_TYPE_DOWN) == FieInputManager.FieInputUIKeyType.KEY_TYPE_DOWN)
				{
					num = 1;
				}
			}
			if (axisType == AxisType.HORIZONTAL)
			{
				if ((keyType & FieInputManager.FieInputUIKeyType.KEY_TYPE_LEFT) == FieInputManager.FieInputUIKeyType.KEY_TYPE_LEFT)
				{
					num = 1;
				}
				if ((keyType & FieInputManager.FieInputUIKeyType.KEY_TYPE_RIGHT) == FieInputManager.FieInputUIKeyType.KEY_TYPE_RIGHT)
				{
					num = -1;
				}
			}
			if (num != 0)
			{
				if (currentIndex >= 0)
				{
					uiElements[currentIndex].ExecuteExitEvent();
				}
				if (currentIndex < 0)
				{
					currentIndex = 0;
				}
				else
				{
					currentIndex += num;
				}
				if (currentIndex < 0)
				{
					currentIndex = uiElements.Count - 1;
				}
				if (currentIndex >= uiElements.Count)
				{
					currentIndex = 0;
				}
				uiElements[currentIndex].ExecuteOverEvent();
				FieManagerBehaviour<FieInputManager>.I.SetUIControlMode(FieInputManager.FieInputUIControlMode.GAME_PAD);
			}
			else
			{
				if ((keyType & FieInputManager.FieInputUIKeyType.KEY_TYPE_DECIDE) == FieInputManager.FieInputUIKeyType.KEY_TYPE_DECIDE)
				{
					uiElements[currentIndex].ExecuteClickEvent();
				}
				FieManagerBehaviour<FieInputManager>.I.SetUIControlMode(FieInputManager.FieInputUIControlMode.GAME_PAD);
			}
		}
	}
}
