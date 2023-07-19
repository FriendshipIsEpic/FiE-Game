using Fie.Scene;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fie.Manager
{
	public sealed class FieManagerFactory : MonoBehaviour
	{
		private FieSceneType _currentSceneType;

		private static FieManagerFactory instance;

		private Dictionary<Type, FieManagerBase> managerList = new Dictionary<Type, FieManagerBase>();

		public static FieManagerFactory I
		{
			get
			{
				if (instance == null)
				{
					instance = UnityEngine.Object.FindObjectOfType<FieManagerFactory>();
					if (instance == null)
					{
						GameObject gameObject = new GameObject();
						UnityEngine.Object.DontDestroyOnLoad(gameObject);
						instance = gameObject.AddComponent<FieManagerFactory>();
						gameObject.name = instance.GetType().Name;
					}
				}
				return instance;
			}
		}

		public FieSceneType currentSceneType
		{
			get
			{
				return _currentSceneType;
			}
			set
			{
				_currentSceneType = value;
			}
		}

		public void AddManager(FieManagerBase manager)
		{
			if (!managerList.ContainsKey(manager.GetType()))
			{
				managerList[manager.GetType()] = manager;
			}
			if (manager.transform != null)
			{
				manager.transform.parent = base.gameObject.transform;
			}
		}

		private void CleanupManager(FieSceneBase targertScene)
		{
			if (managerList.Count > 0)
			{
				if (targertScene != null)
				{
					FieSceneLink sceneLinkInfo = targertScene.GetSceneLinkInfo();
					_currentSceneType = sceneLinkInfo.definedSceneType;
				}
				List<Type> list = new List<Type>();
				foreach (KeyValuePair<Type, FieManagerBase> manager in managerList)
				{
					bool flag = false;
					if (manager.Value == null || manager.Value.transform == null)
					{
						flag = true;
					}
					else
					{
						FieManagerExists fieManagerExists = (FieManagerExists)Attribute.GetCustomAttribute(manager.Key, typeof(FieManagerExists));
						if (fieManagerExists != null)
						{
							if ((fieManagerExists.existFlag & FieManagerExistSceneFlag.NEVER_DESTROY) != 0)
							{
								continue;
							}
							if ((fieManagerExists.existFlag & FieManagerExistSceneFlag.ANYTIME_DESTROY) != 0)
							{
								flag = true;
							}
							if (_currentSceneType == FieSceneType.INGAME && (fieManagerExists.existFlag & FieManagerExistSceneFlag.INGAME) == (FieManagerExistSceneFlag)0)
							{
								flag = true;
							}
							if (_currentSceneType == FieSceneType.OUTGAME && (fieManagerExists.existFlag & FieManagerExistSceneFlag.OUTGAME) == (FieManagerExistSceneFlag)0)
							{
								flag = true;
							}
						}
					}
					if (flag)
					{
						list.Add(manager.Key);
					}
				}
				foreach (Type item in list)
				{
					managerList[item].Destroy();
					managerList.Remove(item);
				}
			}
		}

		public void StartUp()
		{
			FieManagerBehaviour<FieSceneManager>.I.StartUp();
			FieManagerBehaviour<FieSaveManager>.I.StartUp();
			FieManagerBehaviour<FieAudioManager>.I.StartUp();
			FieManagerBehaviour<FieFaderManager>.I.StartUp();
			FieManagerBehaviour<FieInputManager>.I.StartUp();
			FieManagerBehaviour<FieEnvironmentManager>.I.StartUp();
			FieManagerBehaviour<FieUserManager>.I.StartUp();
			FieManagerBehaviour<FieNetworkManager>.I.StartUp();
			FieManagerBehaviour<FieEmittableObjectManager>.I.StartUp();
			FieManagerBehaviour<FieSceneManager>.I.FiePreparedForLoadSceneEvent += ScenePreparedToLoadCallback;
			FieManagerBehaviour<FieSceneManager>.I.FieSceneWasLoadedEvent += SceneLoadedCallback;
		}

		private void ScenePreparedToLoadCallback(FieSceneBase targetScene)
		{
			CleanupManager(targetScene);
		}

		private void SceneLoadedCallback(FieSceneBase targetScene)
		{
			if (currentSceneType == FieSceneType.INGAME)
			{
				if (!FieManagerBehaviour<FieCurrentGameManager>.I.isBooted)
				{
					FieManagerBehaviour<FieCurrentGameManager>.I.StartUp();
				}
				FieManagerBehaviour<FieInGameStateManager>.I.StartUp();
			}
		}

		public void Restart()
		{
			I.KillPopcornFxAll();
			if (!FieManagerBehaviour<FieSceneManager>.I.isTitle())
			{
				foreach (KeyValuePair<Type, FieManagerBase> manager in managerList)
				{
					UnityEngine.Object.Destroy(manager.Value.gameObject);
				}
				UnityEngine.Object.Destroy(base.gameObject);
				SceneManager.LoadScene(0, LoadSceneMode.Single);
			}
		}

		public void KillPopcornFxAll()
		{
			PKFxFX[] array = UnityEngine.Object.FindObjectsOfType<PKFxFX>();
			if (array != null && array.Length > 0)
			{
				PKFxFX[] array2 = array;
				foreach (PKFxFX pKFxFX in array2)
				{
					pKFxFX.KillEffect();
				}
			}
		}
	}
}
