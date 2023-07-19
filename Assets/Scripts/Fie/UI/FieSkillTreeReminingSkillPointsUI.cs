using UnityEngine;

namespace Fie.UI
{
	public class FieSkillTreeReminingSkillPointsUI : MonoBehaviour
	{
		[SerializeField]
		private FieUIConstant2DText _reminingPointsText;

		public void InithWithSkillPoint(int skillPoint)
		{
			_reminingPointsText.replaceMethod = delegate(ref string targetString)
			{
				targetString = targetString.Replace("___Value1___", skillPoint.ToString());
			};
			_reminingPointsText.InitializeText();
		}
	}
}
