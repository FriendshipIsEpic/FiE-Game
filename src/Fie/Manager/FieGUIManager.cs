using Fie.Camera;
using Fie.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Manager
{
	public class FieGUIManager : FieManagerBehaviour<FieGUIManager>
	{
		public enum FieUILayer
		{
			FORWARD,
			DEFAULT,
			BACKWARD,
			BACKWORD_SECOND,
			BACKWORD_THIRD
		}

		public enum FieUIPositionTag
		{
			HEADER_ROOT,
			FOOTER_ROOT,
			ABILITY_ICON_1,
			ABILITY_ICON_2,
			ABILITY_ICON_3
		}

		private FieUICamera _uiCamera;

		private Dictionary<Type, GameObject> _asyncLoadObjectList = new Dictionary<Type, GameObject>();

		private Dictionary<Type, GameObject> _gameUIResouceList = new Dictionary<Type, GameObject>();

		private Dictionary<Type, List<FieGameUIBase>> _gameUIList = new Dictionary<Type, List<FieGameUIBase>>();

		private FieGameUIHeaderFooterManager _headerFooterManager;

		private FieGameUIDialogCaptionManager _dialogCaptionManager;

		private FieGameUIActivityWindowManager _activityWindowManager;

		private FieGameUIPlayerWindowManager _playerWindowManager;

		private FieGameUIGameOverWindowManager _gameOverWindowManager;

		public Dictionary<FieUIPositionTag, Transform> uiPositionList = new Dictionary<FieUIPositionTag, Transform>();

		private bool _isBooted;

		public FieUICamera uiCamera => _uiCamera;

		protected override void StartUpEntity()
		{
			if (!_isBooted)
			{
				if (_uiCamera == null)
				{
					_uiCamera = UnityEngine.Object.FindObjectOfType<FieUICamera>();
					if (_uiCamera == null)
					{
						GameObject gameObject = Resources.Load("Prefabs/Manager/GUICamera") as GameObject;
						if (gameObject == null)
						{
							throw new Exception("Missing the GUI camera prefab.");
						}
						GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
						if (gameObject2 == null)
						{
							throw new Exception("Fiald to instantiate the GUI camera object.");
						}
						gameObject2.transform.parent = base.transform;
						_uiCamera = gameObject2.GetComponent<FieUICamera>();
						if (_uiCamera == null)
						{
							throw new Exception("GUI Camera component dosen't exists in GUI camera prefab.");
						}
					}
				}
				List<Type> uIComponentManagerList = getUIComponentManagerList();
				if (uIComponentManagerList != null && uIComponentManagerList.Count > 0)
				{
					for (int i = 0; i < uIComponentManagerList.Count; i++)
					{
						base.gameObject.AddComponent(uIComponentManagerList[i]);
					}
					PreloadInGameUIResouses();
					ReloadUIOwner();
					_headerFooterManager = GetComponent<FieGameUIHeaderFooterManager>();
					_dialogCaptionManager = GetComponent<FieGameUIDialogCaptionManager>();
					_activityWindowManager = GetComponent<FieGameUIActivityWindowManager>();
					_playerWindowManager = GetComponent<FieGameUIPlayerWindowManager>();
					_gameOverWindowManager = GetComponent<FieGameUIGameOverWindowManager>();
					_activityWindowManager.StartUp();
					_isBooted = true;
				}
			}
		}

		public void ReloadUIOwner()
		{
			setUIOwner(FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter);
			uiCamera.setGameCamera(FieManagerBehaviour<FieGameCameraManager>.I.gameCamera);
		}

		public void ReloadPlayerWindow()
		{
			if (!(_playerWindowManager == null))
			{
				_playerWindowManager.ReloadPlayerWindows();
			}
		}

		public void setUIOwner(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				FieGameUIComponentManagerBase[] components = GetComponents<FieGameUIComponentManagerBase>();
				if (components != null && components.Length > 0)
				{
					for (int i = 0; i < components.Length; i++)
					{
						components[i].setComponentManagerOwner(gameCharacter);
						components[i].StartUp();
					}
				}
			}
		}

		public void ShowHeaderFooter()
		{
			if (!(_headerFooterManager == null))
			{
				_headerFooterManager.Show();
			}
		}

		public void HideHeaderFooter()
		{
			if (!(_headerFooterManager == null))
			{
				_headerFooterManager.Hide();
			}
		}

		public FieGameUIActivityWindow.ActivityWindowState GetActivityWindowState()
		{
			if (_activityWindowManager == null)
			{
				return FieGameUIActivityWindow.ActivityWindowState.BUSY;
			}
			return _activityWindowManager.GetActivityWindowState();
		}

		private void PreloadInGameUIResouses()
		{
			Dictionary<Type, string> gameUIPrefabList = getGameUIPrefabList();
			foreach (KeyValuePair<Type, string> item in gameUIPrefabList)
			{
				if (item.Key.IsSubclassOf(typeof(FieGameUIBase)))
				{
					GameObject gameObject = Resources.Load(item.Value) as GameObject;
					if (gameObject == null)
					{
						Debug.LogError("faild to load resource of " + item.Key.ToString());
					}
					else
					{
						_gameUIResouceList[item.Key] = gameObject;
						FieManagerBehaviour<FieGUIManager>.I.StartCoroutine(FieManagerBehaviour<FieGUIManager>.I.AsyncInstantiateToBuffer(item.Key, gameObject));
					}
				}
			}
		}

		public void AssignGameUIObject<T>(string prefabPath) where T : FieGameUIBase
		{
			if (!_gameUIResouceList.ContainsKey(typeof(T)))
			{
				GameObject gameObject = Resources.Load(prefabPath) as GameObject;
				if (!(gameObject == null))
				{
					_gameUIResouceList[typeof(T)] = gameObject;
					FieManagerBehaviour<FieGUIManager>.I.StartCoroutine(FieManagerBehaviour<FieGUIManager>.I.AsyncInstantiateToBuffer(typeof(T), gameObject));
				}
			}
		}

		public T CreateGui<T>(FieGameCharacter ownerCharacter) where T : FieGameUIBase
		{
			if (!typeof(T).IsSubclassOf(typeof(FieGameUIBase)))
			{
				return (T)null;
			}
			if (!_gameUIList.ContainsKey(typeof(T)))
			{
				Debug.LogError(typeof(T).ToString() + " has not been assigned. You must assign this object before instantiate.");
				return (T)null;
			}
			int num = -1;
			for (int i = 0; i < _gameUIList[typeof(T)].Count; i++)
			{
				if (!_gameUIList[typeof(T)][i].gameObject.activeSelf)
				{
					_gameUIList[typeof(T)][i].ownerCharacter = ownerCharacter;
					_gameUIList[typeof(T)][i].uiActive = true;
					num = i;
					break;
				}
			}
			int num2 = 0;
			for (int j = 0; j < _gameUIList[typeof(T)].Count; j++)
			{
				if (_gameUIList[typeof(T)][j].gameObject.activeSelf)
				{
					num2++;
				}
			}
			if (num2 >= _gameUIList[typeof(T)].Count)
			{
				FieManagerBehaviour<FieGUIManager>.I.StartCoroutine(FieManagerBehaviour<FieGUIManager>.I.AsyncInstantiateToBuffer(typeof(T), _gameUIResouceList[typeof(T)]));
			}
			if (num >= 0)
			{
				return _gameUIList[typeof(T)][num] as T;
			}
			GameObject gameObject = CreateUIByBuffer(typeof(T), Vector3.zero, Quaternion.identity, useBuffer: false);
			T component = gameObject.GetComponent<T>();
			if ((UnityEngine.Object)component == (UnityEngine.Object)null)
			{
				return (T)null;
			}
			component.ownerCharacter = ownerCharacter;
			component.uiActive = true;
			return component;
		}

		private IEnumerator AsyncInstantiateToBuffer(Type uiObjectType, GameObject instantTargetObject)
		{
			_asyncLoadObjectList[uiObjectType] = null;
			GameObject uiObject = UnityEngine.Object.Instantiate(instantTargetObject, Vector3.zero, Quaternion.identity);
			FieGameUIBase uiBase = uiObject.GetComponent<FieGameUIBase>();
			if (uiBase != null)
			{
				uiBase.uiActive = false;
				uiObject.transform.SetParent(base.transform);
				_asyncLoadObjectList[uiObjectType] = uiObject;
				if (!_gameUIList.ContainsKey(uiObjectType))
				{
					_gameUIList[uiObjectType] = new List<FieGameUIBase>();
				}
				_gameUIList[uiObjectType].Add(uiBase);
			}
			else if (uiObject != null)
			{
				Debug.Log("unknown ui object was instantiated. this object does'nt extend FieGameUIBase.Type:" + uiObject.ToString());
				UnityEngine.Object.Destroy(uiObject);
			}
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}

		private GameObject CreateUIByBuffer(Type uiObjectType, Vector3 initPosition, Quaternion initRotation, bool useBuffer = true)
		{
			if (useBuffer && _asyncLoadObjectList[uiObjectType] != null)
			{
				GameObject gameObject = _asyncLoadObjectList[uiObjectType];
				gameObject.transform.position = initPosition;
				gameObject.transform.rotation = initRotation;
				gameObject.SetActive(value: true);
				FieManagerBehaviour<FieGUIManager>.I.StartCoroutine(FieManagerBehaviour<FieGUIManager>.I.AsyncInstantiateToBuffer(uiObjectType, _gameUIResouceList[uiObjectType]));
				return gameObject;
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(_gameUIResouceList[uiObjectType], initPosition, initRotation);
			if (gameObject2 == null)
			{
				return null;
			}
			FieGameUIBase component = gameObject2.GetComponent<FieGameUIBase>();
			if (component == null)
			{
				return null;
			}
			if (!_gameUIList.ContainsKey(uiObjectType))
			{
				_gameUIList[uiObjectType] = new List<FieGameUIBase>();
			}
			_gameUIList[uiObjectType].Add(component);
			return gameObject2;
		}

		private Dictionary<Type, string> getGameUIPrefabList()
		{
			Dictionary<Type, string> dictionary = new Dictionary<Type, string>();
			dictionary.Add(typeof(FieGameUIHeaderFooter), "Prefabs/UI/HeaderFooter/HeaderFooter");
			dictionary.Add(typeof(FieGameUIEnemyLifeGauge), "Prefabs/UI/LifeGauge/EnemyLifeGauge");
			dictionary.Add(typeof(FieGameUIPlayerWindow), "Prefabs/UI/PlayerWindow/PlayerWindow");
			dictionary.Add(typeof(FieGameUISplitLine), "Prefabs/UI/SplitLine/SplitLine");
			dictionary.Add(typeof(FieGameUIPlayerLifeGauge), "Prefabs/UI/LifeGauge/PlayerLifeGauge");
			dictionary.Add(typeof(FieGameUIFriendshipGauge), "Prefabs/UI/FriendshipGauge/PlayerFriendshipGauge");
			dictionary.Add(typeof(FieGameUITargetIcon), "Prefabs/UI/TargetIcon/TargetIcon");
			dictionary.Add(typeof(FieGameUIDamageCounter), "Prefabs/UI/DamageCounter/FieUIDamageCounter");
			dictionary.Add(typeof(FieGameUIDialogCaption), "Prefabs/UI/Dialog/FieDialog");
			dictionary.Add(typeof(FieGameUIActivityWindow), "Prefabs/UI/ActivityWindow/ActivityWindow");
			dictionary.Add(typeof(FieGameUIIndicator), "Prefabs/UI/Indicator/Indicator");
			dictionary.Add(typeof(FieGameUIGameOverWindow), "Prefabs/UI/GameOverWindow/GameOverWindow");
			return dictionary;
		}

		private List<Type> getUIComponentManagerList()
		{
			List<Type> list = new List<Type>();
			list.Add(typeof(FieGameUIHeaderFooterManager));
			list.Add(typeof(FieGameUIPlayerWindowManager));
			list.Add(typeof(FieGameUIEnemyLifeGaugeManager));
			list.Add(typeof(FieGameUIDamageCounterManager));
			list.Add(typeof(FieGameUIAbilitiesIconManager));
			list.Add(typeof(FieGameUIDialogCaptionManager));
			list.Add(typeof(FieGameUIActivityWindowManager));
			list.Add(typeof(FieGameUIIndicatorManager));
			list.Add(typeof(FieGameUIGameOverWindowManager));
			return list;
		}
	}
}
