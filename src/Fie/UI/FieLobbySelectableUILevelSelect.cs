using Fie.Manager;
using GameDataEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fie.UI
{
	internal class FieLobbySelectableUILevelSelect : MonoBehaviour
	{
		[SerializeField]
		private FieLobbySelectableUIController _parent;

		[SerializeField]
		private Button forcesSelectButton;

		[SerializeField]
		private Button levelSelectButton;

		[SerializeField]
		private FieUIConstant2DText difficultyButtonText;

		[SerializeField]
		private FieUIConstant2DText difficultyDescText;

		private Dictionary<int, string> _difficultyList;

		private int _currentDifficulty;

		public void Initialize()
		{
			_difficultyList = new Dictionary<int, string>();
			_currentDifficulty = (int)FieManagerBehaviour<FieEnvironmentManager>.I.currentDifficulty;
			for (int i = 0; i < 5; i++)
			{
				FieEnvironmentManager.Difficulty difficulty = (FieEnvironmentManager.Difficulty)i;
				GDEDifficultyListData difficultyData = FieManagerBehaviour<FieEnvironmentManager>.I.GetDifficultyData(difficulty);
				string constantText = FieLocalizeUtility.GetConstantText(difficultyData.difficultyName.Key);
				_difficultyList[i] = constantText;
			}
			EventSystem.current.SetSelectedGameObject(forcesSelectButton.gameObject);
			ApplyCurrentDifficultyInformationToText();
		}

		private void ApplyCurrentDifficultyInformationToText()
		{
			difficultyButtonText.TmpTextObject.text = _difficultyList[_currentDifficulty];
			difficultyButtonText.TmpTextObject.ForceMeshUpdate();
			GDEDifficultyListData gDEDifficultyListData = FieMasterData<GDEDifficultyListData>.FindMasterData((GDEDifficultyListData data) => (data.difficultyLevel == _currentDifficulty) ? true : false);
			if (gDEDifficultyListData != null)
			{
				string constantText = FieLocalizeUtility.GetConstantText(gDEDifficultyListData.description.Key);
				difficultyDescText.TmpTextObject.text = constantText.Replace("___Value1___", (gDEDifficultyListData.expMagnify * 100f).ToString());
				difficultyDescText.TmpTextObject.ForceMeshUpdate();
			}
		}

		public void OnClickDiffucltyButton()
		{
			_currentDifficulty++;
			if (_currentDifficulty > 4)
			{
				_currentDifficulty = 0;
			}
			ApplyCurrentDifficultyInformationToText();
		}

		public void OnClickDecide()
		{
			FieManagerBehaviour<FieEnvironmentManager>.I.SetDifficulty((FieEnvironmentManager.Difficulty)_currentDifficulty);
			FieManagerBehaviour<FieNetworkManager>.I.SetRoomDifficulty(_currentDifficulty);
			_parent.ChangeState(FieLobbySelectableUIController.SelectableWindowState.CHARACTER_SELECT);
		}
	}
}
