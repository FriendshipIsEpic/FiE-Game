// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEConstantTextListData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDEConstantTextListData : IGDEData
	{
		private static string ForceEnableToUseEnglishFontKey = "ForceEnableToUseEnglishFont";

		private bool _ForceEnableToUseEnglishFont;

		private static string EnglishKey = "English";

		private string _English;

		private static string JapaneseKey = "Japanese";

		private string _Japanese;

		private static string FrenchKey = "French";

		private string _French;

		private static string TypeKey = "Type";

		private GDEConstantTextTypeData _Type;

		public bool ForceEnableToUseEnglishFont
		{
			get
			{
				return _ForceEnableToUseEnglishFont;
			}
			set
			{
				if (_ForceEnableToUseEnglishFont != value)
				{
					_ForceEnableToUseEnglishFont = value;
					GDEDataManager.SetBool(_key + "_" + ForceEnableToUseEnglishFontKey, _ForceEnableToUseEnglishFont);
				}
			}
		}

		public string English
		{
			get
			{
				return _English;
			}
			set
			{
				if (_English != value)
				{
					_English = value;
					GDEDataManager.SetString(_key + "_" + EnglishKey, _English);
				}
			}
		}

		public string Japanese
		{
			get
			{
				return _Japanese;
			}
			set
			{
				if (_Japanese != value)
				{
					_Japanese = value;
					GDEDataManager.SetString(_key + "_" + JapaneseKey, _Japanese);
				}
			}
		}

		public string French
		{
			get
			{
				return _French;
			}
			set
			{
				if (_French != value)
				{
					_French = value;
					GDEDataManager.SetString(_key + "_" + FrenchKey, _French);
				}
			}
		}

		public GDEConstantTextTypeData Type
		{
			get
			{
				return _Type;
			}
			set
			{
				if (_Type != value)
				{
					_Type = value;
					GDEDataManager.SetCustom(_key + "_" + TypeKey, _Type);
				}
			}
		}

		public GDEConstantTextListData()
		{
			_key = string.Empty;
		}

		public GDEConstantTextListData(string key)
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
			dict.TryGetBool(ForceEnableToUseEnglishFontKey, out _ForceEnableToUseEnglishFont);
			dict.TryGetString(EnglishKey, out _English);
			dict.TryGetString(JapaneseKey, out _Japanese);
			dict.TryGetString(FrenchKey, out _French);
			dict.TryGetString(TypeKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextTypeData>(value, out _Type);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_ForceEnableToUseEnglishFont = GDEDataManager.GetBool(_key + "_" + ForceEnableToUseEnglishFontKey, _ForceEnableToUseEnglishFont);
			_English = GDEDataManager.GetString(_key + "_" + EnglishKey, _English);
			_Japanese = GDEDataManager.GetString(_key + "_" + JapaneseKey, _Japanese);
			_French = GDEDataManager.GetString(_key + "_" + FrenchKey, _French);
			_Type = GDEDataManager.GetCustom(_key + "_" + TypeKey, _Type);
		}

		public void Reset_ForceEnableToUseEnglishFont()
		{
			GDEDataManager.ResetToDefault(_key, ForceEnableToUseEnglishFontKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetBool(ForceEnableToUseEnglishFontKey, out _ForceEnableToUseEnglishFont);
		}

		public void Reset_English()
		{
			GDEDataManager.ResetToDefault(_key, EnglishKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(EnglishKey, out _English);
		}

		public void Reset_Japanese()
		{
			GDEDataManager.ResetToDefault(_key, JapaneseKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(JapaneseKey, out _Japanese);
		}

		public void Reset_French()
		{
			GDEDataManager.ResetToDefault(_key, FrenchKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(FrenchKey, out _French);
		}

		public void Reset_Type()
		{
			GDEDataManager.ResetToDefault(_key, TypeKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(TypeKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextTypeData>(value, out _Type);
			Type = GDEDataManager.GetCustom(_key + "_" + TypeKey, _Type);
			Type.ResetAll();
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, TypeKey);
			GDEDataManager.ResetToDefault(_key, EnglishKey);
			GDEDataManager.ResetToDefault(_key, JapaneseKey);
			GDEDataManager.ResetToDefault(_key, FrenchKey);
			GDEDataManager.ResetToDefault(_key, ForceEnableToUseEnglishFontKey);
			Reset_Type();
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}