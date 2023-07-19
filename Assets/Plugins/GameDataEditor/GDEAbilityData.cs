// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEAbilityData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDEAbilityData : IGDEData
	{
		private static string IDKey = "ID";

		private int _ID;

		private static string GameCharacterTypeKey = "GameCharacterType";

		private GDEGameCharacterTypeData _GameCharacterType;

		private static string AbilityNameKey = "AbilityName";

		private GDEConstantTextListData _AbilityName;

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

		public GDEGameCharacterTypeData GameCharacterType
		{
			get
			{
				return _GameCharacterType;
			}
			set
			{
				if (_GameCharacterType != value)
				{
					_GameCharacterType = value;
					GDEDataManager.SetCustom(_key + "_" + GameCharacterTypeKey, _GameCharacterType);
				}
			}
		}

		public GDEConstantTextListData AbilityName
		{
			get
			{
				return _AbilityName;
			}
			set
			{
				if (_AbilityName != value)
				{
					_AbilityName = value;
					GDEDataManager.SetCustom(_key + "_" + AbilityNameKey, _AbilityName);
				}
			}
		}

		public GDEAbilityData()
		{
			_key = string.Empty;
		}

		public GDEAbilityData(string key)
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
			dict.TryGetString(GameCharacterTypeKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEGameCharacterTypeData>(value, out _GameCharacterType);
			dict.TryGetString(AbilityNameKey, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _AbilityName);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_ID = GDEDataManager.GetInt(_key + "_" + IDKey, _ID);
			_GameCharacterType = GDEDataManager.GetCustom(_key + "_" + GameCharacterTypeKey, _GameCharacterType);
			_AbilityName = GDEDataManager.GetCustom(_key + "_" + AbilityNameKey, _AbilityName);
		}

		public void Reset_ID()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(IDKey, out _ID);
		}

		public void Reset_GameCharacterType()
		{
			GDEDataManager.ResetToDefault(_key, GameCharacterTypeKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(GameCharacterTypeKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEGameCharacterTypeData>(value, out _GameCharacterType);
			GameCharacterType = GDEDataManager.GetCustom(_key + "_" + GameCharacterTypeKey, _GameCharacterType);
			GameCharacterType.ResetAll();
		}

		public void Reset_AbilityName()
		{
			GDEDataManager.ResetToDefault(_key, AbilityNameKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(AbilityNameKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _AbilityName);
			AbilityName = GDEDataManager.GetCustom(_key + "_" + AbilityNameKey, _AbilityName);
			AbilityName.ResetAll();
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.ResetToDefault(_key, GameCharacterTypeKey);
			GDEDataManager.ResetToDefault(_key, AbilityNameKey);
			Reset_GameCharacterType();
			Reset_AbilityName();
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}