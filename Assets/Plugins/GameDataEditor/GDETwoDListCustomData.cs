// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDETwoDListCustomData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDETwoDListCustomData : IGDEData
	{
		private static string cust_string_listKey = "cust_string_list";

		public List<string> cust_string_list;

		public GDETwoDListCustomData()
		{
			_key = string.Empty;
		}

		public GDETwoDListCustomData(string key)
		{
			_key = key;
		}

		public void Set_cust_string_list()
		{
			GDEDataManager.SetStringList(_key + "_" + cust_string_listKey, cust_string_list);
		}

		public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
		{
			_key = dataKey;
			if (dict == null)
			{
				LoadFromSavedData(dataKey);
				return;
			}
			dict.TryGetStringList(cust_string_listKey, out cust_string_list);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			cust_string_list = GDEDataManager.GetStringList(_key + "_" + cust_string_listKey, cust_string_list);
		}

		public void Reset_cust_string_list()
		{
			GDEDataManager.ResetToDefault(_key, cust_string_listKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetStringList(cust_string_listKey, out cust_string_list);
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, cust_string_listKey);
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}