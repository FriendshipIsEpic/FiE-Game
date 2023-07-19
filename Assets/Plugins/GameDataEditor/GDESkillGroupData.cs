// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDESkillGroupData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDESkillGroupData : IGDEData
	{
		private static string IDKey = "ID";

		private int _ID;

		private static string SkillGroupNameKey = "SkillGroupName";

		private GDEConstantTextListData _SkillGroupName;

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

		public GDEConstantTextListData SkillGroupName
		{
			get
			{
				return _SkillGroupName;
			}
			set
			{
				if (_SkillGroupName != value)
				{
					_SkillGroupName = value;
					GDEDataManager.SetCustom(_key + "_" + SkillGroupNameKey, _SkillGroupName);
				}
			}
		}

		public GDESkillGroupData()
		{
			_key = string.Empty;
		}

		public GDESkillGroupData(string key)
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
			dict.TryGetString(SkillGroupNameKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _SkillGroupName);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_ID = GDEDataManager.GetInt(_key + "_" + IDKey, _ID);
			_SkillGroupName = GDEDataManager.GetCustom(_key + "_" + SkillGroupNameKey, _SkillGroupName);
		}

		public void Reset_ID()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(IDKey, out _ID);
		}

		public void Reset_SkillGroupName()
		{
			GDEDataManager.ResetToDefault(_key, SkillGroupNameKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(SkillGroupNameKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _SkillGroupName);
			SkillGroupName = GDEDataManager.GetCustom(_key + "_" + SkillGroupNameKey, _SkillGroupName);
			SkillGroupName.ResetAll();
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.ResetToDefault(_key, SkillGroupNameKey);
			Reset_SkillGroupName();
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}