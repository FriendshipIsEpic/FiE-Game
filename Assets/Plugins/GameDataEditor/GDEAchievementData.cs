// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEAchievementData
using System.Collections.Generic;
using GameDataEditor;


namespace GameDataEditor
{
	public class GDEAchievementData : IGDEData
	{
		private static string EnabledKey = "Enabled";

		private bool _Enabled;

		private static string IDKey = "ID";

		private int _ID;

		private static string MaxValueKey = "MaxValue";

		private int _MaxValue;

		private static string AdditionalPlayerLevelKey = "AdditionalPlayerLevel";

		private int _AdditionalPlayerLevel;

		public bool Enabled
		{
			get
			{
				return _Enabled;
			}
			set
			{
				if (_Enabled != value)
				{
					_Enabled = value;
					GDEDataManager.SetBool(_key + "_" + EnabledKey, _Enabled);
				}
			}
		}

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

		public int MaxValue
		{
			get
			{
				return _MaxValue;
			}
			set
			{
				if (_MaxValue != value)
				{
					_MaxValue = value;
					GDEDataManager.SetInt(_key + "_" + MaxValueKey, _MaxValue);
				}
			}
		}

		public int AdditionalPlayerLevel
		{
			get
			{
				return _AdditionalPlayerLevel;
			}
			set
			{
				if (_AdditionalPlayerLevel != value)
				{
					_AdditionalPlayerLevel = value;
					GDEDataManager.SetInt(_key + "_" + AdditionalPlayerLevelKey, _AdditionalPlayerLevel);
				}
			}
		}

		public GDEAchievementData()
		{
			_key = string.Empty;
		}

		public GDEAchievementData(string key)
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
			dict.TryGetBool(EnabledKey, out _Enabled);
			dict.TryGetInt(IDKey, out _ID);
			dict.TryGetInt(MaxValueKey, out _MaxValue);
			dict.TryGetInt(AdditionalPlayerLevelKey, out _AdditionalPlayerLevel);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_Enabled = GDEDataManager.GetBool(_key + "_" + EnabledKey, _Enabled);
			_ID = GDEDataManager.GetInt(_key + "_" + IDKey, _ID);
			_MaxValue = GDEDataManager.GetInt(_key + "_" + MaxValueKey, _MaxValue);
			_AdditionalPlayerLevel = GDEDataManager.GetInt(_key + "_" + AdditionalPlayerLevelKey, _AdditionalPlayerLevel);
		}

		public void Reset_Enabled()
		{
			GDEDataManager.ResetToDefault(_key, EnabledKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetBool(EnabledKey, out _Enabled);
		}

		public void Reset_ID()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(IDKey, out _ID);
		}

		public void Reset_MaxValue()
		{
			GDEDataManager.ResetToDefault(_key, MaxValueKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(MaxValueKey, out _MaxValue);
		}

		public void Reset_AdditionalPlayerLevel()
		{
			GDEDataManager.ResetToDefault(_key, AdditionalPlayerLevelKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(AdditionalPlayerLevelKey, out _AdditionalPlayerLevel);
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.ResetToDefault(_key, EnabledKey);
			GDEDataManager.ResetToDefault(_key, MaxValueKey);
			GDEDataManager.ResetToDefault(_key, AdditionalPlayerLevelKey);
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}