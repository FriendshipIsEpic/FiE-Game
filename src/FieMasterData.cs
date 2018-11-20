using GameDataEditor;
using System.Collections.Generic;
using UnityEngine;

public class FieMasterData<T> where T : IGDEData, new()
{
	public delegate bool FieMasterDataFilterDelegate(T data);

	private Dictionary<string, T> _allDataList = new Dictionary<string, T>();

	private static FieMasterData<T> _instance;

	public static FieMasterData<T> I
	{
		get
		{
			if (_instance == null)
			{
				_instance = new FieMasterData<T>();
			}
			return _instance;
		}
	}

	public FieMasterData()
	{
		GDEDataManager.Init("MasterData/fie_master_data");
		InitMasterData();
	}

	public void InitMasterData()
	{
		Dictionary<string, object> data = null;
		GDEDataManager.GetAllDataBySchema(typeof(T).Name.Replace("GDE", string.Empty).Replace("Data", string.Empty), out data);
		if (data == null)
		{
			Debug.LogError("GDE faild to load spreadsheet. Schema Name : " + typeof(T).ToString());
		}
		foreach (KeyValuePair<string, object> item in data)
		{
			T value = new T();
			GDEDataManager.DataDictionary.TryGetCustom(item.Key, out value);
			_allDataList[item.Key] = value;
		}
	}

	public Dictionary<string, T> GetAllMasterData()
	{
		return _allDataList;
	}

	public T GetMasterData(string key)
	{
		if (!I._allDataList.ContainsKey(key))
		{
			return (T)null;
		}
		return I._allDataList[key];
	}

	public static List<T> FindMasterDataList(FieMasterDataFilterDelegate filterDelegate = null)
	{
		if (I._allDataList.Count <= 0)
		{
			return null;
		}
		List<T> list = new List<T>();
		foreach (KeyValuePair<string, T> allData in I._allDataList)
		{
			if (filterDelegate(allData.Value))
			{
				list.Add(allData.Value);
			}
		}
		return list;
	}

	public static T FindMasterData(FieMasterDataFilterDelegate filterDelegate = null)
	{
		if (I._allDataList.Count <= 0)
		{
			return (T)null;
		}
		foreach (KeyValuePair<string, T> allData in I._allDataList)
		{
			if (filterDelegate(allData.Value))
			{
				return allData.Value;
			}
		}
		return (T)null;
	}

	public Dictionary<string, T> FindMasterDataDictionary(FieMasterDataFilterDelegate filterDelegate = null)
	{
		if (I._allDataList.Count <= 0)
		{
			return null;
		}
		if (filterDelegate != null)
		{
			Dictionary<string, T> dictionary = new Dictionary<string, T>();
			{
				foreach (KeyValuePair<string, T> allData in I._allDataList)
				{
					if (filterDelegate(allData.Value))
					{
						dictionary[allData.Key] = allData.Value;
					}
				}
				return dictionary;
			}
		}
		return I._allDataList;
	}
}
