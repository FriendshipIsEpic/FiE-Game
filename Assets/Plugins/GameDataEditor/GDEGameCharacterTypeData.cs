// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEGameCharacterTypeData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDEGameCharacterTypeData : IGDEData
	{
		private static string IsPlayableKey = "IsPlayable";

		private bool _IsPlayable;

		private static string IDKey = "ID";

		private int _ID;

		private static string RequiredPlaylerLevelToUnlockKey = "RequiredPlaylerLevelToUnlock";

		private int _RequiredPlaylerLevelToUnlock;

		private static string ShieldTypeKey = "ShieldType";

		private string _ShieldType;

		private static string SignatureKey = "Signature";

		private string _Signature;

		private static string SymbolColorKey = "SymbolColor";

		private GDESkillUIColorData _SymbolColor;

		private static string CharacterNameKey = "CharacterName";

		private GDEConstantTextListData _CharacterName;

		private static string CharacterDescriptionKey = "CharacterDescription";

		private GDEConstantTextListData _CharacterDescription;

		private static string RequiredAchivementToUnlockKey = "RequiredAchivementToUnlock";

		public List<List<GDEAchievementData>> RequiredAchivementToUnlock;

		public bool IsPlayable
		{
			get
			{
				return _IsPlayable;
			}
			set
			{
				if (_IsPlayable != value)
				{
					_IsPlayable = value;
					GDEDataManager.SetBool(_key + "_" + IsPlayableKey, _IsPlayable);
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

		public int RequiredPlaylerLevelToUnlock
		{
			get
			{
				return _RequiredPlaylerLevelToUnlock;
			}
			set
			{
				if (_RequiredPlaylerLevelToUnlock != value)
				{
					_RequiredPlaylerLevelToUnlock = value;
					GDEDataManager.SetInt(_key + "_" + RequiredPlaylerLevelToUnlockKey, _RequiredPlaylerLevelToUnlock);
				}
			}
		}

		public string ShieldType
		{
			get
			{
				return _ShieldType;
			}
			set
			{
				if (_ShieldType != value)
				{
					_ShieldType = value;
					GDEDataManager.SetString(_key + "_" + ShieldTypeKey, _ShieldType);
				}
			}
		}

		public string Signature
		{
			get
			{
				return _Signature;
			}
			set
			{
				if (_Signature != value)
				{
					_Signature = value;
					GDEDataManager.SetString(_key + "_" + SignatureKey, _Signature);
				}
			}
		}

		public GDESkillUIColorData SymbolColor
		{
			get
			{
				return _SymbolColor;
			}
			set
			{
				if (_SymbolColor != value)
				{
					_SymbolColor = value;
					GDEDataManager.SetCustom(_key + "_" + SymbolColorKey, _SymbolColor);
				}
			}
		}

		public GDEConstantTextListData CharacterName
		{
			get
			{
				return _CharacterName;
			}
			set
			{
				if (_CharacterName != value)
				{
					_CharacterName = value;
					GDEDataManager.SetCustom(_key + "_" + CharacterNameKey, _CharacterName);
				}
			}
		}

		public GDEConstantTextListData CharacterDescription
		{
			get
			{
				return _CharacterDescription;
			}
			set
			{
				if (_CharacterDescription != value)
				{
					_CharacterDescription = value;
					GDEDataManager.SetCustom(_key + "_" + CharacterDescriptionKey, _CharacterDescription);
				}
			}
		}

		public GDEGameCharacterTypeData()
		{
			_key = string.Empty;
		}

		public GDEGameCharacterTypeData(string key)
		{
			_key = key;
		}

		public void Set_RequiredAchivementToUnlock()
		{
			GDEDataManager.SetCustomTwoDList(_key + "_" + RequiredAchivementToUnlockKey, RequiredAchivementToUnlock);
		}

		public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
		{
			_key = dataKey;
			if (dict == null)
			{
				LoadFromSavedData(dataKey);
				return;
			}
			dict.TryGetBool(IsPlayableKey, out _IsPlayable);
			dict.TryGetInt(IDKey, out _ID);
			dict.TryGetInt(RequiredPlaylerLevelToUnlockKey, out _RequiredPlaylerLevelToUnlock);
			dict.TryGetString(ShieldTypeKey, out _ShieldType);
			dict.TryGetString(SignatureKey, out _Signature);
			dict.TryGetString(SymbolColorKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDESkillUIColorData>(value, out _SymbolColor);
			dict.TryGetString(CharacterNameKey, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _CharacterName);
			dict.TryGetString(CharacterDescriptionKey, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _CharacterDescription);
			dict.TryGetCustomTwoDList(RequiredAchivementToUnlockKey, out RequiredAchivementToUnlock);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_IsPlayable = GDEDataManager.GetBool(_key + "_" + IsPlayableKey, _IsPlayable);
			_ID = GDEDataManager.GetInt(_key + "_" + IDKey, _ID);
			_RequiredPlaylerLevelToUnlock = GDEDataManager.GetInt(_key + "_" + RequiredPlaylerLevelToUnlockKey, _RequiredPlaylerLevelToUnlock);
			_ShieldType = GDEDataManager.GetString(_key + "_" + ShieldTypeKey, _ShieldType);
			_Signature = GDEDataManager.GetString(_key + "_" + SignatureKey, _Signature);
			_SymbolColor = GDEDataManager.GetCustom(_key + "_" + SymbolColorKey, _SymbolColor);
			_CharacterName = GDEDataManager.GetCustom(_key + "_" + CharacterNameKey, _CharacterName);
			_CharacterDescription = GDEDataManager.GetCustom(_key + "_" + CharacterDescriptionKey, _CharacterDescription);
			RequiredAchivementToUnlock = GDEDataManager.GetCustomTwoDList(_key + "_" + RequiredAchivementToUnlockKey, RequiredAchivementToUnlock);
		}

		public void Reset_IsPlayable()
		{
			GDEDataManager.ResetToDefault(_key, IsPlayableKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetBool(IsPlayableKey, out _IsPlayable);
		}

		public void Reset_ID()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(IDKey, out _ID);
		}

		public void Reset_RequiredPlaylerLevelToUnlock()
		{
			GDEDataManager.ResetToDefault(_key, RequiredPlaylerLevelToUnlockKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(RequiredPlaylerLevelToUnlockKey, out _RequiredPlaylerLevelToUnlock);
		}

		public void Reset_ShieldType()
		{
			GDEDataManager.ResetToDefault(_key, ShieldTypeKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(ShieldTypeKey, out _ShieldType);
		}

		public void Reset_Signature()
		{
			GDEDataManager.ResetToDefault(_key, SignatureKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(SignatureKey, out _Signature);
		}

		public void Reset_SymbolColor()
		{
			GDEDataManager.ResetToDefault(_key, SymbolColorKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(SymbolColorKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDESkillUIColorData>(value, out _SymbolColor);
			SymbolColor = GDEDataManager.GetCustom(_key + "_" + SymbolColorKey, _SymbolColor);
			SymbolColor.ResetAll();
		}

		public void Reset_CharacterName()
		{
			GDEDataManager.ResetToDefault(_key, CharacterNameKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(CharacterNameKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _CharacterName);
			CharacterName = GDEDataManager.GetCustom(_key + "_" + CharacterNameKey, _CharacterName);
			CharacterName.ResetAll();
		}

		public void Reset_CharacterDescription()
		{
			GDEDataManager.ResetToDefault(_key, CharacterDescriptionKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(CharacterDescriptionKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _CharacterDescription);
			CharacterDescription = GDEDataManager.GetCustom(_key + "_" + CharacterDescriptionKey, _CharacterDescription);
			CharacterDescription.ResetAll();
		}

		public void Reset_RequiredAchivementToUnlock()
		{
			GDEDataManager.ResetToDefault(_key, RequiredAchivementToUnlockKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetCustomTwoDList(RequiredAchivementToUnlockKey, out RequiredAchivementToUnlock);
			RequiredAchivementToUnlock = GDEDataManager.GetCustomTwoDList(_key + "_" + RequiredAchivementToUnlockKey, RequiredAchivementToUnlock);
			RequiredAchivementToUnlock.ForEach(delegate(List<GDEAchievementData> x)
			{
				x.ForEach(delegate(GDEAchievementData y)
				{
					y.ResetAll();
				});
			});
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.ResetToDefault(_key, SymbolColorKey);
			GDEDataManager.ResetToDefault(_key, IsPlayableKey);
			GDEDataManager.ResetToDefault(_key, ShieldTypeKey);
			GDEDataManager.ResetToDefault(_key, CharacterNameKey);
			GDEDataManager.ResetToDefault(_key, CharacterDescriptionKey);
			GDEDataManager.ResetToDefault(_key, RequiredPlaylerLevelToUnlockKey);
			GDEDataManager.ResetToDefault(_key, RequiredAchivementToUnlockKey);
			GDEDataManager.ResetToDefault(_key, SignatureKey);
			Reset_SymbolColor();
			Reset_CharacterName();
			Reset_CharacterDescription();
			Reset_RequiredAchivementToUnlock();
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}