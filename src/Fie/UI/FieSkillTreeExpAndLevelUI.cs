using Fie.Manager;
using TMPro;
using UnityEngine;

namespace Fie.UI
{
	public class FieSkillTreeExpAndLevelUI : MonoBehaviour
	{
		[SerializeField]
		private FieUIConstant2DText _levelText;

		[SerializeField]
		private FieUGUIUtilityGauge _expGauge;

		[SerializeField]
		private TMP_Text _expText;

		public void InithWithLevelInfo(FieLevelInfo levelInfo)
		{
			_levelText.replaceMethod = delegate(ref string targetString)
			{
				targetString = targetString.Replace("___Value1___", levelInfo.level.ToString());
				targetString = targetString.Replace("___Value2___", levelInfo.levelCap.ToString());
			};
			_levelText.InitializeText();
			_expGauge.Initialize(0f, (float)levelInfo.requiredExpToNextLevel, (float)levelInfo.currentExpToNextLevel, 1.5f);
			if (levelInfo.level == levelInfo.levelCap)
			{
				_expText.text = "MAX";
			}
			else
			{
				_expText.text = levelInfo.currentExpToNextLevel.ToString() + " / " + levelInfo.requiredExpToNextLevel.ToString();
			}
		}
	}
}
