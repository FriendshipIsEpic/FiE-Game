// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEEnemyTableData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDEEnemyTableData : IGDEData
	{
		private static string IDKey = "ID";

		private int _ID;

		private static string CostKey = "Cost";

		private int _Cost;

		private static string ExpKey = "Exp";

		private int _Exp;

		private static string NameKey = "Name";

		private GDEConstantTextListData _Name;

		public int ID
		{
			get
			{
				return _ID;
			}
			set
			{
				if (_ID != value)
				{
					_ID = value;
					GDEDataManager.SetInt(_key + "_" + IDKey, _ID);
				}
			}
		}

		public int Cost
		{
			get
			{
				return _Cost;
			}
			set
			{
				if (_Cost != value)
				{
					_Cost = value;
					GDEDataManager.SetInt(_key + "_" + CostKey, _Cost);
				}
			}
		}

		public int Exp
		{
			get
			{
				return _Exp;
			}
			set
			{
				if (_Exp != value)
				{
					_Exp = value;
					GDEDataManager.SetInt(_key + "_" + ExpKey, _Exp);
				}
			}
		}

		public GDEConstantTextListData Name
		{
			get
			{
				return _Name;
			}
			set
			{
				if (_Name != value)
				{
					_Name = value;
					GDEDataManager.SetCustom(_key + "_" + NameKey, _Name);
				}
			}
		}

		public GDEEnemyTableData()
		{
			_key = string.Empty;
		}

		public GDEEnemyTableData(string key)
		{
			_key = key;
		}

		public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
		{
			_key = dataKey;
			if (dict == null)
			{
				LoadFromSavedData(dataKey);
				return;
			}
			dict.TryGetInt(IDKey, out _ID);
			dict.TryGetInt(CostKey, out _Cost);
			dict.TryGetInt(ExpKey, out _Exp);
			dict.TryGetString(NameKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _Name);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_ID = GDEDataManager.GetInt(_key + "_" + IDKey, _ID);
			_Cost = GDEDataManager.GetInt(_key + "_" + CostKey, _Cost);
			_Exp = GDEDataManager.GetInt(_key + "_" + ExpKey, _Exp);
			_Name = GDEDataManager.GetCustom(_key + "_" + NameKey, _Name);
		}

		public void Reset_ID()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(IDKey, out _ID);
		}

		public void Reset_Cost()
		{
			GDEDataManager.ResetToDefault(_key, CostKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(CostKey, out _Cost);
		}

		public void Reset_Exp()
		{
			GDEDataManager.ResetToDefault(_key, ExpKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(ExpKey, out _Exp);
		}

		public void Reset_Name()
		{
			GDEDataManager.ResetToDefault(_key, NameKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(NameKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _Name);
			Name = GDEDataManager.GetCustom(_key + "_" + NameKey, _Name);
			Name.ResetAll();
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.ResetToDefault(_key, CostKey);
			GDEDataManager.ResetToDefault(_key, ExpKey);
			GDEDataManager.ResetToDefault(_key, NameKey);
			Reset_Name();
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}