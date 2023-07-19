using Fie.Manager;
using Fie.Utility;
using GameDataEditor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieLobbySelectableUICharacterSelect : MonoBehaviour
	{
		public enum CharacterSelectMode
		{
			SELECTING_PLAYER_CHARACTER,
			CONFIRMING_REBIRTH
		}

		[SerializeField]
		private List<FieLobbySelectableUICharacterSelectButton> _buttons;

		[SerializeField]
		private FieLobbySelectableUIController _parent;

		[SerializeField]
		private FieUIConstant2DText _CharacterNameText;

		[SerializeField]
		private Image _CharacterNameBG;

		[SerializeField]
		private FieUIConstant2DText _CharacterLevelText;

		[SerializeField]
		private TextMeshProUGUI _CharacterDescText;

		[SerializeField]
		private FieLobbySelectableUICharacterSelectButton _firstSelectButton;

		[SerializeField]
		private RectTransform _characterWindowRectTransform;

		[SerializeField]
		private RectTransform _cursor;

		[SerializeField]
		private Image _characterWindowImageCenter;

		[SerializeField]
		private RectTransform rebirthingWindowTransform;

		[SerializeField]
		private List<Button> characterSelectButtons = new List<Button>();

		[SerializeField]
		private Button decideButton;

		[SerializeField]
		private Button rebirthButton;

		[SerializeField]
		private RectTransform rebirthingWindowConfirmButton;

		[SerializeField]
		private RectTransform rebirthingWindowCancelButton;

		[SerializeField]
		private RectTransform rebirthingWindowOkForNonExecutableButton;

		private Sprite _currentCharacterSprite;

		private Sprite _nextCharacterSprite;

		private GameObject _latestSelectedGameObject;

		private FieLobbySelectableUICharacterSelectButton _latestSelectedButton;

		private FieGameCharacter _decidedCharacter;

		private Queue<FieLobbySelectableUICharacterSelectButton> _transitionQueue = new Queue<FieLobbySelectableUICharacterSelectButton>();

		private bool _isNowCharacterWindowTransition;

		private Tweener<TweenTypesInSine> _characterWindowTweenerOut = new Tweener<TweenTypesInSine>();

		private Tweener<TweenTypesOutSine> _characterWindowTweenerIn = new Tweener<TweenTypesOutSine>();

		public FieGameCharacter decidedCharacter => _decidedCharacter;

		public FieGameCharacter GetLatestSelectedGameCharacter()
		{
			if (_latestSelectedButton == null)
			{
				return null;
			}
			return _latestSelectedButton.relatedCharacter;
		}

		private void Awake()
		{
		}

		public void Initialize()
		{
			if (_latestSelectedButton == null)
			{
				_latestSelectedButton = _firstSelectButton;
			}
			_characterWindowImageCenter.sprite = _latestSelectedButton.relatedSprite;
			EventSystem.current.SetSelectedGameObject(_latestSelectedButton.gameObject);
			ChangeCaracterWindowInformation(_latestSelectedButton, isEnableTransition: false);
		}

		private void Update()
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != _latestSelectedGameObject)
			{
				if (currentSelectedGameObject != null)
				{
					FieLobbySelectableUICharacterSelectButton component = currentSelectedGameObject.GetComponent<FieLobbySelectableUICharacterSelectButton>();
					if (component != null)
					{
						ChangeCaracterWindowInformation(component);
						_latestSelectedButton = component;
					}
				}
				_latestSelectedGameObject = currentSelectedGameObject;
			}
			if (_transitionQueue.Count >= 1 && !_isNowCharacterWindowTransition)
			{
				StartCoroutine(CharacterWindowTransitionTask(_transitionQueue.Dequeue()));
			}
			if (_latestSelectedButton != null)
			{
				if (!_cursor.gameObject.activeSelf)
				{
					_cursor.gameObject.SetActive(value: true);
				}
				_cursor.localPosition = Vector3.Lerp(_cursor.localPosition, _latestSelectedButton.transform.localPosition, Mathf.Min(Time.deltaTime * 10f, 1f));
			}
			else if (_cursor.gameObject.activeSelf)
			{
				_cursor.gameObject.SetActive(value: false);
			}
		}

		private IEnumerator ShowConfirmingToRebrithWindow()
		{
			if (!_isNowCharacterWindowTransition)
			{
				_isNowCharacterWindowTransition = true;
				foreach (Button characterSelectButton in characterSelectButtons)
				{
					characterSelectButton.interactable = false;
				}
				rebirthingWindowTransform.gameObject.SetActive(value: true);
				bool canRebirth = false;
				FieGameCharacter selectedCharacter = GetLatestSelectedGameCharacter();
				if (selectedCharacter != null)
				{
					int totalExp = FieManagerBehaviour<FieSaveManager>.I.onMemorySaveData.CharacterExp[(int)selectedCharacter.getGameCharacterID()];
					FieLevelInfo levelInfoByTotalExp = FieManagerBehaviour<FieSaveManager>.I.GetLevelInfoByTotalExp(totalExp);
					if (levelInfoByTotalExp.level == levelInfoByTotalExp.levelCap)
					{
						canRebirth = true;
					}
				}
				if (canRebirth)
				{
					rebirthingWindowCancelButton.gameObject.SetActive(value: true);
					rebirthingWindowConfirmButton.gameObject.SetActive(value: true);
					rebirthingWindowOkForNonExecutableButton.gameObject.SetActive(value: false);
					EventSystem.current.SetSelectedGameObject(rebirthingWindowCancelButton.gameObject);
				}
				else
				{
					rebirthingWindowCancelButton.gameObject.SetActive(value: false);
					rebirthingWindowConfirmButton.gameObject.SetActive(value: false);
					rebirthingWindowOkForNonExecutableButton.gameObject.SetActive(value: true);
					EventSystem.current.SetSelectedGameObject(rebirthingWindowOkForNonExecutableButton.gameObject);
				}
				_characterWindowTweenerIn.InitTweener(0.5f, 0f, 1f);
				if (!_characterWindowTweenerIn.IsEnd())
				{
					float rate = _characterWindowTweenerIn.UpdateParameterFloat(Time.deltaTime);
					rebirthingWindowTransform.localScale = new Vector3(1f, rate, 1f);
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				_isNowCharacterWindowTransition = false;
			}
		}

		private IEnumerator BackToCharacterSelection()
		{
			if (!_isNowCharacterWindowTransition)
			{
				_isNowCharacterWindowTransition = true;
				_characterWindowTweenerIn.InitTweener(0.5f, 1f, 0f);
				if (!_characterWindowTweenerIn.IsEnd())
				{
					float rate = _characterWindowTweenerIn.UpdateParameterFloat(Time.deltaTime);
					rebirthingWindowTransform.localScale = new Vector3(1f, rate, 1f);
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				foreach (Button characterSelectButton in characterSelectButtons)
				{
					characterSelectButton.interactable = true;
				}
				rebirthingWindowTransform.gameObject.SetActive(value: false);
				EventSystem.current.SetSelectedGameObject(rebirthButton.gameObject);
				_isNowCharacterWindowTransition = false;
			}
		}

		private IEnumerator CharacterWindowTransitionTask(FieLobbySelectableUICharacterSelectButton button)
		{
			if (!_isNowCharacterWindowTransition && !(button == null))
			{
				_isNowCharacterWindowTransition = true;
				_characterWindowTweenerOut.InitTweener(0.15f, 0f, 1f);
				if (!_characterWindowTweenerOut.IsEnd())
				{
					float rate = _characterWindowTweenerOut.UpdateParameterFloat(Time.deltaTime);
					_characterWindowRectTransform.localPosition = new Vector3(-1315f * rate, 0f, 0f);
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				_characterWindowImageCenter.sprite = button.relatedSprite;
				_characterWindowTweenerIn.InitTweener(0.15f, 1f, 0f);
				if (!_characterWindowTweenerIn.IsEnd())
				{
					float rate2 = _characterWindowTweenerIn.UpdateParameterFloat(Time.deltaTime);
					_characterWindowRectTransform.localPosition = new Vector3(1315f * rate2, 0f, 0f);
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				_isNowCharacterWindowTransition = false;
			}
		}

		private void ChangeCaracterWindowInformation(FieLobbySelectableUICharacterSelectButton button, bool isEnableTransition = true)
		{
			if (!(button == null))
			{
				GDEGameCharacterTypeData gDEGameCharacterTypeData = FieMasterData<GDEGameCharacterTypeData>.FindMasterData(delegate(GDEGameCharacterTypeData data)
				{
					if (data.ID == (int)button.gameCharacterType)
					{
						return true;
					}
					return false;
				});
				if (gDEGameCharacterTypeData != null)
				{
					FieLevelInfo characterLevelInfo = FieManagerBehaviour<FieSaveManager>.I.GetCharacterLevelInfo(gDEGameCharacterTypeData);
					_CharacterNameText.TmpTextObject.text = FieLocalizeUtility.GetConstantText(gDEGameCharacterTypeData.CharacterName.Key);
					_CharacterDescText.text = FieLocalizeUtility.GetConstantText(gDEGameCharacterTypeData.CharacterDescription.Key);
					string constantText = FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_SKILL_TREE_LEVEL_LABEL);
					constantText = constantText.Replace("___Value1___", characterLevelInfo.level.ToString());
					constantText = constantText.Replace("___Value2___", characterLevelInfo.levelCap.ToString());
					_CharacterLevelText.TmpTextObject.text = constantText;
					_CharacterNameBG.color = new Color(gDEGameCharacterTypeData.SymbolColor.R, gDEGameCharacterTypeData.SymbolColor.G, gDEGameCharacterTypeData.SymbolColor.B, 0.5f);
					if (_latestSelectedButton.relatedCharacter.getGameCharacterID() != button.relatedCharacter.getGameCharacterID() && isEnableTransition)
					{
						if (_transitionQueue.Count > 1)
						{
							_transitionQueue.Dequeue();
						}
						_transitionQueue.Enqueue(button);
					}
				}
			}
		}

		public void ClickedCharacterButtonCallback()
		{
			EventSystem.current.SetSelectedGameObject(decideButton.gameObject);
		}

		public void ClickedRebrithButtonCallback()
		{
			StartCoroutine(ShowConfirmingToRebrithWindow());
		}

		public void ClickedConfrimRebrithButtonCallback()
		{
			FieGameCharacter latestSelectedGameCharacter = GetLatestSelectedGameCharacter();
			FieManagerBehaviour<FieSaveManager>.I.PromoteGameCharacter(latestSelectedGameCharacter.getGameCharacterID());
			FieLevelInfo characterLevelInfo = FieManagerBehaviour<FieSaveManager>.I.GetCharacterLevelInfo(latestSelectedGameCharacter.getGameCharacterTypeData());
			string constantText = FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_SKILL_TREE_LEVEL_LABEL);
			constantText = constantText.Replace("___Value1___", characterLevelInfo.level.ToString());
			constantText = constantText.Replace("___Value2___", characterLevelInfo.levelCap.ToString());
			_CharacterLevelText.TmpTextObject.text = constantText;
			StartCoroutine(BackToCharacterSelection());
		}

		public void ClickedSkillTreeButtonCallback()
		{
			if (_latestSelectedButton != null)
			{
				_parent.ChangeState(FieLobbySelectableUIController.SelectableWindowState.SKILL_TREE);
			}
		}

		public void ClickedCancelRebirthButtonCallback()
		{
			StartCoroutine(BackToCharacterSelection());
		}

		public void ClickedDecideButtonCallback()
		{
			_decidedCharacter = GetLatestSelectedGameCharacter();
			_parent.ChangeState(FieLobbySelectableUIController.SelectableWindowState.GOTO_GAME);
		}
	}
}
