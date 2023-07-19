using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Collider))]
public class FieDetector : NetworkBehaviour
{
	public delegate void FieDetecterChangeDelegate(FieGameCharacter fromCharacter, FieGameCharacter toCharacter);

	public delegate void FieDetectorLocateDelegate(FieGameCharacter targetCharacter);

	private const float INPUT_CROSS_THLESHOLD = 0.75f;

	[SerializeField]
	private Transform _parentTransform;

	[SerializeField]
	public string enemyTag;

	[SerializeField]
	public string friendTag;

	private Dictionary<int, FieGameCharacter> _locatedObjectList = new Dictionary<int, FieGameCharacter>();

	private FieGameCharacter _lockonTargetObject;

	private bool _isAnythingDetected;

	public FieGameCharacter lockonTargetObject
	{
		get
		{
			return _lockonTargetObject;
		}
		private set
		{
			if (value == null)
			{
				_lockonTargetObject = null;
			}
			else if (_lockonTargetObject != null && value.GetInstanceID() == _lockonTargetObject.GetInstanceID())
			{
				return;
			}
			if (this.targetChangedEvent != null)
			{
				this.targetChangedEvent(_lockonTargetObject, value);
			}
			_lockonTargetObject = value;
			_isAnythingDetected = true;
		}
	}

	public event FieDetecterChangeDelegate targetChangedEvent;

	public event FieDetectorLocateDelegate missedEvent;

	public event FieDetectorLocateDelegate locatedEvent;

	public event FieDetectorLocateDelegate intoTheBattleEvent;

	public event FieDetectorLocateDelegate completelyMissedEvent;

	public void Initialize()
	{
		_locatedObjectList = new Dictionary<int, FieGameCharacter>();
		_lockonTargetObject = null;
	}

	public void Update()
	{
		if (_isAnythingDetected && (lockonTargetObject == null || lockonTargetObject.damageSystem.isDying))
		{
			lockonTargetObject = getNearestEnemyGameCharacter(-1);
			if (lockonTargetObject == null)
			{
				if (this.missedEvent != null)
				{
					this.missedEvent(null);
				}
				if (this.completelyMissedEvent != null)
				{
					this.completelyMissedEvent(null);
				}
				_isAnythingDetected = false;
			}
		}
	}

	public void setLockonTargetDynamic(FieGameCharacter targetCharacter)
	{
		_lockonTargetObject = targetCharacter;
	}

	public void OnTriggerEnter(Collider collider)
	{
		if (!(collider.tag != "Enemy") || !(collider.tag != "Player"))
		{
			FieCollider component = collider.GetComponent<FieCollider>();
			if (!(component == null) && component.isRoot)
			{
				FieGameCharacter parentGameCharacter = component.getParentGameCharacter();
				if (!(parentGameCharacter == null))
				{
					int instanceID = parentGameCharacter.GetInstanceID();
					if (!_locatedObjectList.ContainsKey(instanceID))
					{
						_locatedObjectList[parentGameCharacter.GetInstanceID()] = parentGameCharacter;
						if (this.locatedEvent != null)
						{
							this.locatedEvent(parentGameCharacter);
						}
					}
					if (lockonTargetObject == null && enemyTag == collider.tag)
					{
						lockonTargetObject = parentGameCharacter;
						if (this.intoTheBattleEvent != null)
						{
							this.intoTheBattleEvent(parentGameCharacter);
						}
					}
				}
			}
		}
	}

	public void OnTriggerExit(Collider collider)
	{
		if (!(collider.tag != "Enemy") || !(collider.tag != "Player"))
		{
			FieCollider component = collider.GetComponent<FieCollider>();
			if (!(component == null) && component.isRoot)
			{
				FieGameCharacter parentGameCharacter = component.getParentGameCharacter();
				if (!(parentGameCharacter == null))
				{
					int instanceID = parentGameCharacter.GetInstanceID();
					if (_locatedObjectList.ContainsKey(instanceID))
					{
						if (lockonTargetObject != null)
						{
							if (lockonTargetObject.GetInstanceID() == instanceID)
							{
								lockonTargetObject = getNearestEnemyGameCharacter(lockonTargetObject.GetInstanceID());
							}
						}
						else
						{
							lockonTargetObject = getNearestEnemyGameCharacter(-1);
						}
						if (lockonTargetObject == null)
						{
							lockonTargetObject = getNearestEnemyGameCharacter(instanceID);
						}
						if (this.missedEvent != null)
						{
							this.missedEvent(parentGameCharacter);
						}
						if (lockonTargetObject == null)
						{
							if (this.completelyMissedEvent != null)
							{
								this.completelyMissedEvent(null);
							}
							_isAnythingDetected = false;
						}
						_locatedObjectList.Remove(instanceID);
					}
				}
			}
		}
	}

	public Transform getNearestEnemyTransform(bool isCenter = false)
	{
		cleanupListsData();
		if (_locatedObjectList.Count <= 0)
		{
			return null;
		}
		FieGameCharacter fieGameCharacter = null;
		float num = 3.40282347E+38f;
		foreach (int key in _locatedObjectList.Keys)
		{
			if (_locatedObjectList[key].tag == enemyTag)
			{
				float num2 = Vector3.Distance(_parentTransform.position, _locatedObjectList[key].transform.position);
				if (num > num2)
				{
					fieGameCharacter = _locatedObjectList[key];
					num = num2;
				}
			}
		}
		if (fieGameCharacter != null && lockonTargetObject == null)
		{
			lockonTargetObject = fieGameCharacter.GetComponent<FieGameCharacter>();
		}
		return (!(fieGameCharacter != null)) ? null : ((!isCenter) ? fieGameCharacter.transform : fieGameCharacter.centerTransform);
	}

	public Transform getRandomEnemyTransform(bool isCenter = false, int seed = 0)
	{
		cleanupListsData();
		if (_locatedObjectList.Count <= 0)
		{
			return null;
		}
		List<int> list = new List<int>();
		foreach (int key2 in _locatedObjectList.Keys)
		{
			if (_locatedObjectList[key2].tag == enemyTag)
			{
				list.Add(key2);
			}
		}
		if (list.Count <= 0)
		{
			return null;
		}
		int key = list[0];
		if (list.Count > 1)
		{
			key = Random.Range(0, list.Count);
			key = list[key];
		}
		if (_locatedObjectList.ContainsKey(key))
		{
			if (isCenter)
			{
				return _locatedObjectList[key].centerTransform;
			}
			return _locatedObjectList[key].transform;
		}
		return null;
	}

	public FieGameCharacter getNearestEnemyGameCharacter(int ignoreID)
	{
		Transform nearestEnemyTransform = getNearestEnemyTransform();
		if (nearestEnemyTransform != null)
		{
			FieGameCharacter component = nearestEnemyTransform.GetComponent<FieGameCharacter>();
			if (component != null)
			{
				if (component.GetInstanceID() == ignoreID)
				{
					return null;
				}
				return component;
			}
		}
		return null;
	}

	public FieGameCharacter getNearestEnemyGameCharacter()
	{
		Transform nearestEnemyTransform = getNearestEnemyTransform();
		if (nearestEnemyTransform != null)
		{
			FieGameCharacter component = nearestEnemyTransform.GetComponent<FieGameCharacter>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public FieGameCharacter getRandomEnemyGameCharacter()
	{
		Transform randomEnemyTransform = getRandomEnemyTransform();
		if (randomEnemyTransform != null)
		{
			FieGameCharacter component = randomEnemyTransform.GetComponent<FieGameCharacter>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public Transform getLockonEnemyTransform(bool isCenter = false)
	{
		if (lockonTargetObject != null)
		{
			return (!isCenter) ? lockonTargetObject.transform : lockonTargetObject.centerTransform;
		}
		return getNearestEnemyTransform(isCenter);
	}

	public FieGameCharacter getLockonEnemyGameCharacter()
	{
		Transform lockonEnemyTransform = getLockonEnemyTransform();
		if (lockonEnemyTransform != null)
		{
			FieGameCharacter component = lockonEnemyTransform.GetComponent<FieGameCharacter>();
			if (component != null)
			{
				return component;
			}
		}
		return null;
	}

	public FieGameCharacter ChangeLockonTargetByInstanceID(int instanceId)
	{
		cleanupListsData();
		if (_locatedObjectList.ContainsKey(instanceId))
		{
			lockonTargetObject = _locatedObjectList[instanceId];
			return lockonTargetObject;
		}
		return null;
	}

	public bool isTargetting(FieGameCharacter gameCharacter)
	{
		if (gameCharacter == null)
		{
			return false;
		}
		return lockonTargetObject != null && lockonTargetObject.GetInstanceID() == gameCharacter.GetInstanceID();
	}

	public FieGameCharacter ChangeLockonTargetFromNearestWorldPosition(Vector3 worldPosition, float distanceThreadshold)
	{
		worldPosition.z = 0f;
		Dictionary<int, Vector3> dictionary = new Dictionary<int, Vector3>();
		Dictionary<int, float> dictionary2 = new Dictionary<int, float>();
		int num = -1;
		if (lockonTargetObject != null)
		{
			num = lockonTargetObject.GetInstanceID();
		}
		cleanupListsData();
		foreach (KeyValuePair<int, FieGameCharacter> locatedObject in _locatedObjectList)
		{
			if (!(locatedObject.Value == null) && !(locatedObject.Value.tag != enemyTag))
			{
				int instanceID = locatedObject.Value.GetInstanceID();
				if (instanceID != num)
				{
					Dictionary<int, Vector3> dictionary3 = dictionary;
					int key = instanceID;
					Vector3 position = locatedObject.Value.centerTransform.position;
					float x = position.x;
					Vector3 position2 = locatedObject.Value.centerTransform.position;
					dictionary3[key] = new Vector3(x, position2.y, 0f);
				}
			}
		}
		if (dictionary.Count <= 0)
		{
			return lockonTargetObject;
		}
		foreach (KeyValuePair<int, Vector3> item in dictionary)
		{
			float num2 = Vector3.Distance(item.Value, worldPosition);
			if (!(num2 > distanceThreadshold))
			{
				dictionary2[item.Key] = num2;
			}
		}
		if (dictionary2.Count <= 0)
		{
			return lockonTargetObject;
		}
		IOrderedEnumerable<KeyValuePair<int, float>> orderedEnumerable = from entry in dictionary2
		orderby entry.Value
		select entry;
		if (orderedEnumerable == null)
		{
			return lockonTargetObject;
		}
		if (orderedEnumerable.Count() <= 0)
		{
			return lockonTargetObject;
		}
		foreach (KeyValuePair<int, float> item2 in orderedEnumerable)
		{
			if (_locatedObjectList.ContainsKey(item2.Key))
			{
				lockonTargetObject = _locatedObjectList[item2.Key];
				break;
			}
		}
		return lockonTargetObject;
	}

	public FieGameCharacter ChangeLockonTargetByScreenDirection(Camera camera, Vector3 inputVector)
	{
		if (camera == null)
		{
			return null;
		}
		if (lockonTargetObject == null)
		{
			return null;
		}
		inputVector.Normalize();
		Vector3 vector = camera.WorldToScreenPoint(lockonTargetObject.centerTransform.position);
		Dictionary<int, Vector3> dictionary = new Dictionary<int, Vector3>();
		Dictionary<int, float> dictionary2 = new Dictionary<int, float>();
		int instanceID = lockonTargetObject.GetInstanceID();
		cleanupListsData();
		foreach (KeyValuePair<int, FieGameCharacter> locatedObject in _locatedObjectList)
		{
			if (!(locatedObject.Value == null) && !(locatedObject.Value.tag != enemyTag))
			{
				int instanceID2 = locatedObject.Value.GetInstanceID();
				if (instanceID2 != instanceID)
				{
					dictionary[instanceID2] = camera.WorldToScreenPoint(locatedObject.Value.centerTransform.position);
				}
			}
		}
		if (dictionary.Count <= 0)
		{
			return lockonTargetObject;
		}
		foreach (KeyValuePair<int, Vector3> item in dictionary)
		{
			Vector3 lhs = vector - item.Value;
			lhs.Normalize();
			float num = Vector3.Dot(lhs, inputVector);
			if (num <= 0.75f)
			{
				float num2 = 1f + num;
				num2 = Mathf.Abs(Vector3.Distance(item.Value, vector)) * num2;
				dictionary2[item.Key] = num2;
			}
		}
		if (dictionary2.Count <= 0)
		{
			return lockonTargetObject;
		}
		IOrderedEnumerable<KeyValuePair<int, float>> orderedEnumerable = from entry in dictionary2
		orderby entry.Value
		select entry;
		if (orderedEnumerable == null)
		{
			return lockonTargetObject;
		}
		if (orderedEnumerable.Count() <= 0)
		{
			return lockonTargetObject;
		}
		foreach (KeyValuePair<int, float> item2 in orderedEnumerable)
		{
			if (_locatedObjectList.ContainsKey(item2.Key))
			{
				lockonTargetObject = _locatedObjectList[item2.Key];
				break;
			}
		}
		return lockonTargetObject;
	}

	private void cleanupListsData()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, FieGameCharacter> locatedObject in _locatedObjectList)
		{
			if (locatedObject.Value == null)
			{
				list.Add(locatedObject.Key);
			}
		}
		foreach (int item in list)
		{
			_locatedObjectList.Remove(item);
		}
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result = default(bool);
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}
}
