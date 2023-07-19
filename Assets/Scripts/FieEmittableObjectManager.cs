using Fie.Manager;
using Fie.Object;
using Fie.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FieManagerExists(FieManagerExistSceneFlag.NEVER_DESTROY)]
public class FieEmittableObjectManager : FieManagerBehaviour<FieEmittableObjectManager>
{
	public const float LOAD_RESOURCE_TIMEOUT = 3f;

	private Dictionary<Type, GameObject> _emittableObjectList = new Dictionary<Type, GameObject>();

	private Dictionary<Type, bool> _asyncLoadingFlag = new Dictionary<Type, bool>();

	private Dictionary<Type, Dictionary<int, GameObject>> _stackedObjectList = new Dictionary<Type, Dictionary<int, GameObject>>();

	private bool _isBooted;

	protected override void StartUpEntity()
	{
		if (!_isBooted)
		{
			FieManagerBehaviour<FieSceneManager>.I.FieSceneWasLoadedEvent += SceneLoadedCallback;
			_isBooted = true;
		}
	}

	public void SceneLoadedCallback(FieSceneBase scene)
	{
		RecalculateAllCaches();
	}

	public void RecalculateAllCaches()
	{
		if (_stackedObjectList.Count > 0)
		{
			foreach (KeyValuePair<Type, Dictionary<int, GameObject>> stackedObject in _stackedObjectList)
			{
				if (stackedObject.Value != null)
				{
					foreach (KeyValuePair<int, GameObject> item in stackedObject.Value)
					{
						if (!(item.Value == null))
						{
							UnityEngine.Object.Destroy(item.Value);
						}
					}
				}
			}
		}
		ResetAllCache();
		FieGameCharacter[] array = UnityEngine.Object.FindObjectsOfType<FieGameCharacter>();
		if (array.Length > 0)
		{
			FieGameCharacter[] array2 = array;
			foreach (FieGameCharacter fieGameCharacter in array2)
			{
				fieGameCharacter.ReloadPreloadedResouces();
			}
		}
	}

	private void ResetAllCache()
	{
		_stackedObjectList = new Dictionary<Type, Dictionary<int, GameObject>>();
		_asyncLoadingFlag = new Dictionary<Type, bool>();
		_emittableObjectList = new Dictionary<Type, GameObject>();
	}

	private IEnumerator AsyncInstanciateToBuffer<T>(GameObject instanceObject) where T : FieEmittableObjectBase
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(instanceObject, Vector3.zero, Quaternion.identity);
		gameObject.transform.SetParent(base.transform);
		gameObject.SetActive(value: false);
		if (!_stackedObjectList.ContainsKey(typeof(T)))
		{
			_stackedObjectList[typeof(T)] = new Dictionary<int, GameObject>();
		}
		_stackedObjectList[typeof(T)][gameObject.GetInstanceID()] = gameObject;
		T component = gameObject.GetComponent<T>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			string message = "the emitted object must be extended to FieEmitObjectBase";
			throw new ApplicationException(message);
		}
		component.InitializeInitializer<T>();
		yield break;
	}

	private IEnumerator AsyncLoadAndInstanciateToBuffer<T>(string path) where T : FieEmittableObjectBase
	{
		_asyncLoadingFlag[typeof(T)] = true;
		ResourceRequest loadRequest = Resources.LoadAsync(path);
		float time = 0f;
		if (time < 3f && !loadRequest.isDone)
		{
			float num = time + Time.deltaTime;
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		GameObject loadObject = loadRequest.asset as GameObject;
		_emittableObjectList[typeof(T)] = loadObject;
		GameObject emitObject = UnityEngine.Object.Instantiate(loadObject, Vector3.zero, Quaternion.identity);
		emitObject.transform.SetParent(base.transform);
		emitObject.SetActive(value: false);
		if (!_stackedObjectList.ContainsKey(typeof(T)))
		{
			_stackedObjectList[typeof(T)] = new Dictionary<int, GameObject>();
		}
		_stackedObjectList[typeof(T)][emitObject.GetInstanceID()] = emitObject;
		_asyncLoadingFlag[typeof(T)] = false;
		T emitedObject = emitObject.GetComponent<T>();
		if ((UnityEngine.Object)emitedObject == (UnityEngine.Object)null)
		{
			string message = "the emitted object must be extended to FieEmitObjectBase";
			throw new ApplicationException(message);
		}
		emitedObject.InitializeInitializer<T>();
	}

	public bool LoadAsync<T>() where T : FieEmittableObjectBase
	{
		if (_asyncLoadingFlag.ContainsKey(typeof(T)) && _asyncLoadingFlag[typeof(T)])
		{
			return false;
		}
		if (_emittableObjectList.ContainsKey(typeof(T)))
		{
			return true;
		}
		FiePrefabInfo fiePrefabInfo = (FiePrefabInfo)Attribute.GetCustomAttribute(typeof(T), typeof(FiePrefabInfo));
		if (fiePrefabInfo == null)
		{
			return true;
		}
		FieManagerBehaviour<FieEmittableObjectManager>.I.StartCoroutine(FieManagerBehaviour<FieEmittableObjectManager>.I.AsyncLoadAndInstanciateToBuffer<T>(fiePrefabInfo.path));
		return false;
	}

	public void Load<T>() where T : FieEmittableObjectBase
	{
		if (!_emittableObjectList.ContainsKey(typeof(T)))
		{
			FiePrefabInfo fiePrefabInfo = (FiePrefabInfo)Attribute.GetCustomAttribute(typeof(T), typeof(FiePrefabInfo));
			if (fiePrefabInfo != null)
			{
				GameObject gameObject = Resources.Load(fiePrefabInfo.path) as GameObject;
				_emittableObjectList[typeof(T)] = gameObject;
				FieManagerBehaviour<FieEmittableObjectManager>.I.StartCoroutine(FieManagerBehaviour<FieEmittableObjectManager>.I.AsyncInstanciateToBuffer<T>(gameObject));
			}
		}
	}

	public T EmitObject<T>(Transform initTransform, Vector3 directionalVec) where T : FieEmittableObjectBase
	{
		return EmitObject<T>(initTransform, directionalVec, null);
	}

	public T EmitObject<T>(Transform initTransform, Vector3 directionalVec, Transform targetTransform, FieGameCharacter owner = null) where T : FieEmittableObjectBase
	{
		GameObject gameObject = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObjectByBuffer<T>(initTransform.position, initTransform.rotation);
		if (gameObject == null)
		{
			return (T)null;
		}
		gameObject.transform.SetParent(base.transform);
		T component = gameObject.GetComponent<T>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			string message = "the emitted object must be extended to FieEmitObjectBase";
			throw new ApplicationException(message);
		}
		if (owner != null)
		{
			component.ownerCharacter = owner;
		}
		component.Initialize(initTransform, directionalVec, targetTransform);
		component.awakeEmitObject();
		return component;
	}

	private GameObject EmitObjectByBuffer<T>(Vector3 initPosition, Quaternion initRotation) where T : FieEmittableObjectBase
	{
		if (!_emittableObjectList.ContainsKey(typeof(T)))
		{
			Load<T>();
		}
		GameObject gameObject = null;
		int num = 0;
		int num2 = 0;
		if (_stackedObjectList.ContainsKey(typeof(T)) && _stackedObjectList[typeof(T)] != null)
		{
			num2 = _stackedObjectList[typeof(T)].Count;
			foreach (KeyValuePair<int, GameObject> item in _stackedObjectList[typeof(T)])
			{
				if (item.Value != null)
				{
					if (gameObject == null && !item.Value.activeSelf)
					{
						gameObject = item.Value;
					}
					else if (item.Value.activeSelf)
					{
						num++;
					}
				}
			}
		}
		if (gameObject != null)
		{
			gameObject.transform.position = initPosition;
			gameObject.transform.rotation = initRotation;
			gameObject.SetActive(value: true);
			if (num >= num2 - 1)
			{
				FieManagerBehaviour<FieEmittableObjectManager>.I.StartCoroutine(FieManagerBehaviour<FieEmittableObjectManager>.I.AsyncInstanciateToBuffer<T>(_emittableObjectList[typeof(T)]));
			}
			return gameObject;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(_emittableObjectList[typeof(T)], initPosition, initRotation);
		gameObject2.transform.SetParent(base.transform);
		if (!_stackedObjectList.ContainsKey(typeof(T)))
		{
			_stackedObjectList[typeof(T)] = new Dictionary<int, GameObject>();
		}
		_stackedObjectList[typeof(T)][gameObject2.GetInstanceID()] = gameObject2;
		T component = gameObject2.GetComponent<T>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			string message = "the emitted object must be extended to FieEmitObjectBase";
			throw new ApplicationException(message);
		}
		component.InitializeInitializer<T>();
		FieManagerBehaviour<FieEmittableObjectManager>.I.StartCoroutine(FieManagerBehaviour<FieEmittableObjectManager>.I.AsyncInstanciateToBuffer<T>(_emittableObjectList[typeof(T)]));
		return gameObject2;
	}
}
