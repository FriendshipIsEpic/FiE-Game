using Fie.Manager;
using Fie.Ponies;
using Fie.User;
using GameDataEditor;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.UI
{
	public class FieGameUIPlayerWindowManager : FieGameUIComponentManagerBase
	{
		private const float SCREEN_SPLIT_NUM = 3f;

		private Dictionary<int, FieGameUIPlayerWindow> _playerWindowList = new Dictionary<int, FieGameUIPlayerWindow>();

		private Dictionary<int, FieGameUISplitLine> _splitLineList = new Dictionary<int, FieGameUISplitLine>();

		private Dictionary<int, FieGameUIPlayerLifeGauge> _playerLifeGaugeList = new Dictionary<int, FieGameUIPlayerLifeGauge>();

		private Dictionary<int, FieGameUIFriendshipGauge> _playerFriendshipGaugeList = new Dictionary<int, FieGameUIFriendshipGauge>();

		private FieGameUITargetIcon _targetIcon;

		private int gameCharacterCount;

		public override void StartUp()
		{
			ReloadPlayerWindows();
			Relocate();
		}

		public void ReloadPlayerWindows()
		{
			foreach (KeyValuePair<int, FieGameUIPlayerWindow> playerWindow in _playerWindowList)
			{
				if (playerWindow.Value != null)
				{
					playerWindow.Value.uiActive = false;
				}
			}
			foreach (KeyValuePair<int, FieGameUIPlayerLifeGauge> playerLifeGauge in _playerLifeGaugeList)
			{
				if (playerLifeGauge.Value != null)
				{
					playerLifeGauge.Value.uiActive = false;
				}
			}
			foreach (KeyValuePair<int, FieGameUIFriendshipGauge> playerFriendshipGauge in _playerFriendshipGaugeList)
			{
				if (playerFriendshipGauge.Value != null)
				{
					playerFriendshipGauge.Value.uiActive = false;
				}
			}
			foreach (KeyValuePair<int, FieGameUISplitLine> splitLine in _splitLineList)
			{
				if (splitLine.Value != null)
				{
					splitLine.Value.uiActive = false;
				}
			}
			List<FiePonies> allPlayerCharacters = FieManagerBehaviour<FieUserManager>.I.GetAllPlayerCharacters<FiePonies>();
			if (allPlayerCharacters.Count > 0)
			{
				for (int i = 0; i < allPlayerCharacters.Count; i++)
				{
					FieGameUIPlayerWindow fieGameUIPlayerWindow = FieManagerBehaviour<FieGUIManager>.I.CreateGui<FieGameUIPlayerWindow>(allPlayerCharacters[i]);
					FieGameUIPlayerLifeGauge fieGameUIPlayerLifeGauge = FieManagerBehaviour<FieGUIManager>.I.CreateGui<FieGameUIPlayerLifeGauge>(allPlayerCharacters[i]);
					FieGameUIFriendshipGauge fieGameUIFriendshipGauge = FieManagerBehaviour<FieGUIManager>.I.CreateGui<FieGameUIFriendshipGauge>(allPlayerCharacters[i]);
					FieGameUISplitLine fieGameUISplitLine = FieManagerBehaviour<FieGUIManager>.I.CreateGui<FieGameUISplitLine>(allPlayerCharacters[i]);
					fieGameUIPlayerLifeGauge.isTrackingCharacterPosition = false;
					fieGameUIPlayerWindow.uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
					fieGameUIPlayerLifeGauge.uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
					fieGameUIFriendshipGauge.uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
					fieGameUISplitLine.uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
					fieGameUIPlayerWindow.transform.position = new Vector3(0f, 512f, 0f);
					fieGameUIPlayerLifeGauge.transform.position = new Vector3(0f, 512f, 0f);
					fieGameUIFriendshipGauge.transform.position = new Vector3(0f, 512f, 0f);
					fieGameUISplitLine.transform.position = new Vector3(0f, 512f, 0f);
					fieGameUIPlayerWindow.nameTextMesh.text = FieManagerBehaviour<FieUserManager>.I.GetUserName(i);
					FieUser data = FieManagerBehaviour<FieUserManager>.I.GetUserData(i);
					fieGameUIPlayerWindow.levelText.replaceMethod = delegate(ref string targetString)
					{
						string constantText = FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_PLAYER_WINDOW_LV_TEXT);
						if (data == null || data.playerInfo == null || data.playerInfo.IsLocal)
						{
							FieLevelInfo levelInfoByTotalExp = FieManagerBehaviour<FieSaveManager>.I.GetLevelInfoByTotalExp(data.usersCharacter.totalExp);
							constantText = constantText.Replace("___Value1___", FieManagerBehaviour<FieSaveManager>.I.onMemorySaveData.PlayerLevel.ToString());
							constantText = constantText.Replace("___Value2___", levelInfoByTotalExp.level.ToString());
						}
						else
						{
							constantText = constantText.Replace("___Value1___", ((int)data.playerInfo.CustomProperties["player_level"]).ToString());
							constantText = constantText.Replace("___Value2___", ((int)data.playerInfo.CustomProperties["character_level"]).ToString());
						}
						targetString = constantText;
					};
					_playerWindowList[i] = fieGameUIPlayerWindow;
					_playerLifeGaugeList[i] = fieGameUIPlayerLifeGauge;
					_playerFriendshipGaugeList[i] = fieGameUIFriendshipGauge;
					_splitLineList[i] = fieGameUISplitLine;
					gameCharacterCount++;
				}
			}
			_targetIcon = FieManagerBehaviour<FieGUIManager>.I.CreateGui<FieGameUITargetIcon>(base.componentManagerOwner);
			_targetIcon.uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
			Relocate();
		}

		public void Relocate()
		{
			float num = (float)Screen.width;
			float num2 = num / 3f;
			float num3 = num / 6f;
			float num4 = num2 * 0.5f;
			float num5 = num3 * 2f;
			Dictionary<int, Transform> dictionary = new Dictionary<int, Transform>();
			Dictionary<int, Transform> dictionary2 = new Dictionary<int, Transform>();
			foreach (KeyValuePair<int, FieGameUIPlayerWindow> playerWindow in _playerWindowList)
			{
				if (FieManagerBehaviour<FieGUIManager>.I.uiPositionList[FieGUIManager.FieUIPositionTag.HEADER_ROOT] != null)
				{
					playerWindow.Value.transform.parent = FieManagerBehaviour<FieGUIManager>.I.uiPositionList[FieGUIManager.FieUIPositionTag.HEADER_ROOT];
				}
				playerWindow.Value.transform.position = playerWindow.Value.uiCamera.camera.ScreenToWorldPoint(new Vector3(num4, (float)Screen.height, 0f));
				num4 += num2;
				dictionary[playerWindow.Key] = playerWindow.Value.lifeGaugePositionTransform;
				dictionary2[playerWindow.Key] = playerWindow.Value.friendshipGaugePositionTransform;
			}
			foreach (KeyValuePair<int, FieGameUISplitLine> splitLine in _splitLineList)
			{
				if (FieManagerBehaviour<FieGUIManager>.I.uiPositionList[FieGUIManager.FieUIPositionTag.HEADER_ROOT] != null)
				{
					splitLine.Value.transform.parent = FieManagerBehaviour<FieGUIManager>.I.uiPositionList[FieGUIManager.FieUIPositionTag.HEADER_ROOT];
				}
				if (splitLine.Key >= 2)
				{
					splitLine.Value.gameObject.SetActive(value: false);
				}
				else
				{
					splitLine.Value.transform.position = splitLine.Value.uiCamera.camera.ScreenToWorldPoint(new Vector3(num5, (float)Screen.height, 0f));
					num5 += num3 * 2f;
				}
			}
			foreach (KeyValuePair<int, FieGameUIPlayerLifeGauge> playerLifeGauge in _playerLifeGaugeList)
			{
				if (dictionary.ContainsKey(playerLifeGauge.Key))
				{
					playerLifeGauge.Value.transform.position = dictionary[playerLifeGauge.Key].position;
					playerLifeGauge.Value.gameObject.transform.SetParent(dictionary[playerLifeGauge.Key]);
				}
			}
			foreach (KeyValuePair<int, FieGameUIFriendshipGauge> playerFriendshipGauge in _playerFriendshipGaugeList)
			{
				if (dictionary2.ContainsKey(playerFriendshipGauge.Key))
				{
					playerFriendshipGauge.Value.transform.position = dictionary2[playerFriendshipGauge.Key].position;
					playerFriendshipGauge.Value.gameObject.transform.SetParent(dictionary2[playerFriendshipGauge.Key]);
				}
			}
		}

		public override void setComponentManagerOwner(FieGameCharacter owner)
		{
			base.setComponentManagerOwner(owner);
		}
	}
}
