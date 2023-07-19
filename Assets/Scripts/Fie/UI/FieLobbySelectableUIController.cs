using Fie.Camera;
using Fie.Manager;
using Fie.Ponies.Applejack;
using Fie.Ponies.RainbowDash;
using Fie.Ponies.Twilight;
using Fie.User;
using Fie.Utility;
using GameDataEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.UI
{
	public class FieLobbySelectableUIController : MonoBehaviour
	{
		public enum SelectableWindowState
		{
			DISABLED,
			CHARACTER_SELECT,
			LEVEL_SELECT,
			SKILL_TREE,
			CONFIRMING_REBIRTH,
			GOTO_GAME
		}

		private delegate void ResouceLoadedCallback<T>(T gameCharacter) where T : FieGameCharacter;

		[SerializeField]
		private FieLobbySelectableUIHeader header;

		[SerializeField]
		private FieLobbySelectableUICharacterSelect characterSelect;

		[SerializeField]
		private FieLobbySelectableUILevelSelect levelSelect;

		[SerializeField]
		private CanvasGroup _headerCanvas;

		[SerializeField]
		private List<CanvasGroup> _characterSelectCanvases = new List<CanvasGroup>();

		[SerializeField]
		private List<CanvasGroup> _levelSelectCanvases = new List<CanvasGroup>();

		[SerializeField]
		private FieSkillTree skillTree;

		[SerializeField]
		private GameObject _lobbyDefaultBgmObject;

		[SerializeField]
		private GameObject _lobbySkillTreeBgmObject;

		private Tweener<TweenTypesLinear> _transitionTweener = new Tweener<TweenTypesLinear>();

		private Tweener<TweenTypesInOutSine> _transitionInOutTweener = new Tweener<TweenTypesInOutSine>();

		private SelectableWindowState _currentState;

		private bool _isNowTransition;

		private Coroutine _transitionCoroutine;

		public SelectableWindowState currentState => _currentState;

		private IEnumerator AsyncLoadCharacterResouce<T>(ResouceLoadedCallback<T> callback) where T : FieGameCharacter
		{
			FiePrefabInfo existsAttribute = (FiePrefabInfo)Attribute.GetCustomAttribute(typeof(T), typeof(FiePrefabInfo));
			if (existsAttribute == null)
			{
				callback((T)null);
			}
			else
			{
				ResourceRequest loadRequest = Resources.LoadAsync(existsAttribute.path);
				float time = 0f;
				if (time < 3f && !loadRequest.isDone)
				{
					float num = time + Time.deltaTime;
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				GameObject loadObject = loadRequest.asset as GameObject;
				if (loadObject == null)
				{
					callback((T)null);
				}
				else
				{
					T gameCharacter = loadObject.GetComponent<T>();
					if ((UnityEngine.Object)gameCharacter == (UnityEngine.Object)null)
					{
						callback((T)null);
					}
					else
					{
						callback(gameCharacter);
					}
				}
			}
		}

		public void Start()
		{
			_headerCanvas.alpha = 0f;
			for (int i = 0; i < _characterSelectCanvases.Count; i++)
			{
				_characterSelectCanvases[i].alpha = 0f;
			}
			for (int j = 0; j < _levelSelectCanvases.Count; j++)
			{
				_levelSelectCanvases[j].alpha = 0f;
			}
			header.gameObject.SetActive(value: false);
			characterSelect.gameObject.SetActive(value: false);
			levelSelect.gameObject.SetActive(value: false);
			skillTree.gameObject.SetActive(value: false);
			_lobbyDefaultBgmObject.SetActive(value: true);
			_lobbySkillTreeBgmObject.SetActive(value: false);
		}

		private void Update()
		{
			SelectableWindowState currentState = _currentState;
			if (currentState == SelectableWindowState.SKILL_TREE)
			{
				UpdateSkillTreeState();
			}
		}

		private void UpdateSkillTreeState()
		{
			if (skillTree.isEnd)
			{
				ChangeState(SelectableWindowState.CHARACTER_SELECT);
			}
		}

		public void ChangeState(SelectableWindowState targetState)
		{
			if (!_isNowTransition)
			{
				if (_transitionCoroutine != null)
				{
					StopCoroutine(_transitionCoroutine);
				}
				switch (targetState)
				{
				case SelectableWindowState.CONFIRMING_REBIRTH:
					break;
				case SelectableWindowState.CHARACTER_SELECT:
					if (_currentState == SelectableWindowState.SKILL_TREE)
					{
						_transitionCoroutine = StartCoroutine(BackToCharacterSelectFromSkillTree());
					}
					else
					{
						_transitionCoroutine = StartCoroutine(ShowCharacterSelectUI());
					}
					break;
				case SelectableWindowState.LEVEL_SELECT:
					if (PhotonNetwork.isMasterClient || PhotonNetwork.offlineMode)
					{
						_transitionCoroutine = StartCoroutine(ShowLevelSelectUI());
					}
					else
					{
						_currentState = SelectableWindowState.LEVEL_SELECT;
						_transitionCoroutine = StartCoroutine(ShowCharacterSelectUI());
					}
					break;
				case SelectableWindowState.DISABLED:
					_transitionCoroutine = StartCoroutine(HideSelectUI());
					break;
				case SelectableWindowState.SKILL_TREE:
					_transitionCoroutine = StartCoroutine(ShowSkillTree());
					break;
				case SelectableWindowState.GOTO_GAME:
					_transitionCoroutine = StartCoroutine(StartGameCoroutine());
					break;
				}
			}
		}

		private IEnumerator StartGameCoroutine()
		{
			if (!_isNowTransition)
			{
				_isNowTransition = true;
				if (_currentState == SelectableWindowState.CHARACTER_SELECT)
				{
					_transitionTweener.InitTweener(0.2f, 1f, 0f);
					if (!_transitionTweener.IsEnd())
					{
						float rate3 = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
						for (int i = 0; i < _characterSelectCanvases.Count; i++)
						{
							_characterSelectCanvases[i].alpha = 1f * rate3;
						}
						yield return (object)null;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
				else if (_currentState != SelectableWindowState.LEVEL_SELECT)
				{
					_transitionTweener.InitTweener(0.5f, 1f, 0f);
					if (!_transitionTweener.IsEnd())
					{
						float rate2 = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
						for (int j = 0; j < _levelSelectCanvases.Count; j++)
						{
							_levelSelectCanvases[j].alpha = 1f * rate2;
						}
						yield return (object)null;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
				_transitionTweener.InitTweener(0.2f, 0f, 1f);
				if (!_transitionTweener.IsEnd())
				{
					float rate = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
					_headerCanvas.alpha = 1f * rate;
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				Decide();
				header.gameObject.SetActive(value: false);
				characterSelect.gameObject.SetActive(value: false);
				levelSelect.gameObject.SetActive(value: false);
				skillTree.gameObject.SetActive(value: false);
				_currentState = SelectableWindowState.GOTO_GAME;
				_isNowTransition = false;
			}
		}

		private IEnumerator BackToCharacterSelectFromSkillTree()
		{
			if (!_isNowTransition)
			{
				_isNowTransition = true;
				FieManagerBehaviour<FieFaderManager>.I.LaunchFader(FieFaderManager.FadeType.OUT_TO_WHITE, 0.3f);
				yield return (object)new WaitForSeconds(0.3f);
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		private IEnumerator ShowSkillTree()
		{
			if (!_isNowTransition)
			{
				_isNowTransition = true;
				if (_currentState == SelectableWindowState.CHARACTER_SELECT)
				{
					_transitionTweener.InitTweener(0.2f, 1f, 0f);
					if (!_transitionTweener.IsEnd())
					{
						float rate2 = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
						for (int i = 0; i < _characterSelectCanvases.Count; i++)
						{
							_characterSelectCanvases[i].alpha = 1f * rate2;
						}
						yield return (object)null;
						/*Error: Unable to find new state assignment for yield return*/;
					}
					_transitionTweener.InitTweener(0.2f, 1f, 0f);
					if (!_transitionTweener.IsEnd())
					{
						float rate3 = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
						_headerCanvas.alpha = 1f * rate3;
						yield return (object)null;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
				_lobbyDefaultBgmObject.SetActive(value: false);
				_lobbySkillTreeBgmObject.SetActive(value: true);
				FieManagerBehaviour<FieFaderManager>.I.LaunchFader(FieFaderManager.FadeType.OUT_TO_WHITE, 0.3f);
				_transitionInOutTweener.InitTweener(0.3f, 1f, 0.05f);
				if (!_transitionInOutTweener.IsEnd())
				{
					float rate = _transitionInOutTweener.UpdateParameterFloat(Time.deltaTime);
					FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.camera.fieldOfView = 45f * rate;
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				FieGameCharacter gameCharacter = characterSelect.GetLatestSelectedGameCharacter();
				GDEGameCharacterTypeData gameCharacterTypeData = gameCharacter.getCharacterTypeData();
				FieManagerBehaviour<FieSaveManager>.I.GetGameCharacterBuildData(gameCharacterTypeData);
				FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.gameObject.SetActive(value: false);
				FieManagerBehaviour<FieGUIManager>.I.uiCamera.gameObject.SetActive(value: false);
				header.gameObject.SetActive(value: false);
				characterSelect.gameObject.SetActive(value: false);
				levelSelect.gameObject.SetActive(value: false);
				skillTree.gameObject.SetActive(value: true);
				yield return (object)new WaitForSeconds(0.1f);
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		private IEnumerator HideSelectUI()
		{
			if (!_isNowTransition)
			{
				_isNowTransition = true;
				if (_currentState != 0)
				{
					_transitionTweener.InitTweener(0.5f, 1f, 0f);
					if (!_transitionTweener.IsEnd())
					{
						float rate = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
						_headerCanvas.alpha = 1f * rate;
						yield return (object)null;
						/*Error: Unable to find new state assignment for yield return*/;
					}
					if (_currentState != SelectableWindowState.CHARACTER_SELECT)
					{
						_transitionTweener.InitTweener(0.5f, 1f, 0f);
						if (!_transitionTweener.IsEnd())
						{
							float rate3 = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
							for (int i = 0; i < _characterSelectCanvases.Count; i++)
							{
								_characterSelectCanvases[i].alpha = 1f * rate3;
							}
							yield return (object)null;
							/*Error: Unable to find new state assignment for yield return*/;
						}
					}
					else if (_currentState != SelectableWindowState.LEVEL_SELECT)
					{
						_transitionTweener.InitTweener(0.5f, 1f, 0f);
						if (!_transitionTweener.IsEnd())
						{
							float rate2 = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
							for (int j = 0; j < _levelSelectCanvases.Count; j++)
							{
								_levelSelectCanvases[j].alpha = 1f * rate2;
							}
							yield return (object)null;
							/*Error: Unable to find new state assignment for yield return*/;
						}
					}
				}
				_isNowTransition = false;
				_currentState = SelectableWindowState.DISABLED;
				header.gameObject.SetActive(value: false);
				characterSelect.gameObject.SetActive(value: false);
				levelSelect.gameObject.SetActive(value: false);
			}
		}

		private IEnumerator ShowLevelSelectUI()
		{
			if (!_isNowTransition)
			{
				_isNowTransition = true;
				header.gameObject.SetActive(value: false);
				characterSelect.gameObject.SetActive(value: false);
				levelSelect.gameObject.SetActive(value: true);
				yield return (object)null;
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		private IEnumerator ShowCharacterSelectUI()
		{
			if (!_isNowTransition)
			{
				_isNowTransition = true;
				header.gameObject.SetActive(value: true);
				characterSelect.gameObject.SetActive(value: true);
				levelSelect.gameObject.SetActive(value: true);
				yield return (object)null;
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public void Decide()
		{
			FieLobbyGameCharacterGenerateManager.LobbyGameCharacterCreatedCallback lobbyCallback = delegate
			{
				FieManagerBehaviour<FieGameCameraManager>.I.setDefaultCameraOffset(new Vector3(0f, 0.75f, -5f));
				FieManagerBehaviour<FieGameCameraManager>.I.setDefaultCameraRotation(new Vector3(354f, 0f, 0f));
				FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.SetCameraTask<FieGameCameraTaskStop>(1f);
			};
			int userNumberByHash = FieManagerBehaviour<FieUserManager>.I.getUserNumberByHash(FieManagerBehaviour<FieUserManager>.I.myHash);
			FieUser userData = FieManagerBehaviour<FieUserManager>.I.GetUserData(userNumberByHash);
			int totalExp = FieManagerBehaviour<FieSaveManager>.I.onMemorySaveData.CharacterExp[(int)characterSelect.decidedCharacter.getGameCharacterID()];
			FieLevelInfo levelInfoByTotalExp = FieManagerBehaviour<FieSaveManager>.I.GetLevelInfoByTotalExp(totalExp);
			FieManagerBehaviour<FieNetworkManager>.I.SetMyCharacterLevel(levelInfoByTotalExp.level);
			FieManagerBehaviour<FieActivityManager>.I.RequestToHideActivity();
			switch (characterSelect.decidedCharacter.getGameCharacterID())
			{
			case FieConstValues.FieGameCharacter.MAGIC:
			{
				ResouceLoadedCallback<FieTwilight> callback3 = delegate
				{
					FieManagerBehaviour<FieLobbyGameCharacterGenerateManager>.I.RequestToCreateGameCharacter<FieTwilight>(userData, lobbyCallback);
				};
				StartCoroutine(AsyncLoadCharacterResouce(callback3));
				break;
			}
			case FieConstValues.FieGameCharacter.LOYALTY:
			{
				ResouceLoadedCallback<FieRainbowDash> callback2 = delegate
				{
					FieManagerBehaviour<FieLobbyGameCharacterGenerateManager>.I.RequestToCreateGameCharacter<FieRainbowDash>(userData, lobbyCallback);
				};
				StartCoroutine(AsyncLoadCharacterResouce(callback2));
				break;
			}
			case FieConstValues.FieGameCharacter.HONESTY:
			{
				ResouceLoadedCallback<FieApplejack> callback = delegate
				{
					FieManagerBehaviour<FieLobbyGameCharacterGenerateManager>.I.RequestToCreateGameCharacter<FieApplejack>(userData, lobbyCallback);
				};
				StartCoroutine(AsyncLoadCharacterResouce(callback));
				break;
			}
			}
		}
	}
}
