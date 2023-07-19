using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.NEVER_DESTROY)]
	public class FieGameCharacterManager : FieManagerBehaviour<FieGameCharacterManager>
	{
		public delegate void GameCharacterCreatedCallback<T>(T createdCharacter) where T : FieGameCharacter;

		public const float LOAD_RESOURCE_TIMEOUT = 3f;

		private Dictionary<Type, FiePrefabInfo> _prefabInfoCache = new Dictionary<Type, FiePrefabInfo>();

		private List<GameObject> _createdObjectList = new List<GameObject>();

		protected override void StartUpEntity()
		{
		}

		private void ResetAllCache()
		{
			_prefabInfoCache = new Dictionary<Type, FiePrefabInfo>();
		}

		public void DestroyAllCharacters()
		{
			foreach (GameObject createdObject in _createdObjectList)
			{
				if (!(createdObject == null))
				{
					PhotonView component = createdObject.GetComponent<PhotonView>();
					if (component != null)
					{
						if (component.isMine)
						{
							PhotonNetwork.Destroy(component);
						}
					}
					else
					{
						UnityEngine.Object.Destroy(createdObject);
					}
				}
			}
		}

		private IEnumerator AsyncCreateGameCharacter<T>(string path, GameCharacterCreatedCallback<T> callback, bool isSyncNetwork = true) where T : FieGameCharacter
		{
			ResourceRequest loadRequest = Resources.LoadAsync(path);
			float time = 0f;
			if (time < 3f && !loadRequest.isDone)
			{
				float num = time + Time.deltaTime;
				yield return (object)null;
				/*Error: Unable to find new state assignment for yield return*/;
			}
			GameObject loadObject = loadRequest.asset as GameObject;
			GameObject createdObject = (!isSyncNetwork || PhotonNetwork.offlineMode) ? UnityEngine.Object.Instantiate(loadObject, Vector3.zero, Quaternion.identity) : PhotonNetwork.Instantiate(path, Vector3.zero, Quaternion.identity, 0);
			createdObject.transform.SetParent(base.transform);
			createdObject.SetActive(value: true);
			T createdCharacter = createdObject.GetComponent<T>();
			if ((UnityEngine.Object)createdCharacter != (UnityEngine.Object)null)
			{
				callback?.Invoke(createdCharacter);
			}
			_createdObjectList.Add(createdObject);
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public void CreateGameCharacter<T>(GameCharacterCreatedCallback<T> callback, bool isSyncNetwork = true) where T : FieGameCharacter
		{
			string empty = string.Empty;
			if (!_prefabInfoCache.ContainsKey(typeof(T)))
			{
				FiePrefabInfo fiePrefabInfo = (FiePrefabInfo)Attribute.GetCustomAttribute(typeof(T), typeof(FiePrefabInfo));
				if (fiePrefabInfo == null)
				{
					return;
				}
				_prefabInfoCache[typeof(T)] = fiePrefabInfo;
				empty = fiePrefabInfo.path;
			}
			else
			{
				empty = _prefabInfoCache[typeof(T)].path;
			}
			if (empty == string.Empty)
			{
				Debug.LogError("The FiePrefabInfo dosen't exists! Please check the class : " + typeof(T).FullName);
			}
			else
			{
				FieManagerBehaviour<FieGameCharacterManager>.I.StartCoroutine(FieManagerBehaviour<FieGameCharacterManager>.I.AsyncCreateGameCharacter(empty, callback, isSyncNetwork));
			}
		}

		public FieGameCharacter CreateGameCharacter(Type gameCharacterType)
		{
			FiePrefabInfo fiePrefabInfo = (FiePrefabInfo)Attribute.GetCustomAttribute(gameCharacterType, typeof(FiePrefabInfo));
			if (fiePrefabInfo == null)
			{
				return null;
			}
			GameObject original = Resources.Load(fiePrefabInfo.path) as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity);
			if (gameObject == null)
			{
				return null;
			}
			return gameObject.GetComponent<FieGameCharacter>();
		}
	}
}
