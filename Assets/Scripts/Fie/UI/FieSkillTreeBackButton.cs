using UnityEngine;

namespace Fie.UI
{
	public class FieSkillTreeBackButton : MonoBehaviour
	{
		public delegate void FieSkillTreeBackButtonCallback(FieSkillTreeBackButton clickedButton);

		public event FieSkillTreeBackButtonCallback clickedEvent;

		public void OnClickedCallback()
		{
			if (this.clickedEvent != null)
			{
				this.clickedEvent(this);
			}
		}
	}
}
