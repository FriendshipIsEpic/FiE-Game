// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDESkillTypeData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDESkillTypeData : IGDEData
	{
		private static string IDKey = "ID";

		private int _ID;

		private static string DescriptionTextKey = "DescriptionText";

		private GDEConstantTextListData _DescriptionText;

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

		public GDEConstantTextListData DescriptionText
		{
			get
			{
				return _DescriptionText;
			}
			set
			{
				if (_DescriptionText != value)
				{
					_DescriptionText = value;
					GDEDataManager.SetCustom(_key + "_" + DescriptionTextKey, _DescriptionText);
				}
			}
		}

		public GDESkillTypeData()
		{
			_key = string.Empty;
		}

		public GDESkillTypeData(string key)
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
			dict.TryGetString(DescriptionTextKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _DescriptionText);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_ID = GDEDataManager.GetInt(_key + "_" + IDKey, _ID);
			_DescriptionText = GDEDataManager.GetCustom(_key + "_" + DescriptionTextKey, _DescriptionText);
		}

		public void Reset_ID()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(IDKey, out _ID);
		}

		public void Reset_DescriptionText()
		{
			GDEDataManager.ResetToDefault(_key, DescriptionTextKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(DescriptionTextKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _DescriptionText);
			DescriptionText = GDEDataManager.GetCustom(_key + "_" + DescriptionTextKey, _DescriptionText);
			DescriptionText.ResetAll();
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.ResetToDefault(_key, DescriptionTextKey);
			Reset_DescriptionText();
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}