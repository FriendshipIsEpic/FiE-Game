// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEWordScriptsListData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDEWordScriptsListData : IGDEData
	{
		private static string VoiceAssetPathKey = "VoiceAssetPath";

		private string _VoiceAssetPath;

		private static string ThaiCaptionKey = "ThaiCaption";

		private string _ThaiCaption;

		private static string TraditionalChineseCaptionKey = "TraditionalChineseCaption";

		private string _TraditionalChineseCaption;

		private static string FrenchCaptionKey = "FrenchCaption";

		private string _FrenchCaption;

		private static string EnglishCaptionKey = "EnglishCaption";

		private string _EnglishCaption;

		private static string JapaneseCaptionKey = "JapaneseCaption";

		private string _JapaneseCaption;

		private static string TriggerKey = "Trigger";

		private GDEWordScriptTriggerTypeData _Trigger;

		private static string ActorKey = "Actor";

		private GDEGameCharacterTypeData _Actor;

		private static string Together_1Key = "Together_1";

		private GDEGameCharacterTypeData _Together_1;

		private static string Together_2Key = "Together_2";

		private GDEGameCharacterTypeData _Together_2;

		private static string NextKey = "Next";

		private GDEWordScriptsListData _Next;

		public string VoiceAssetPath
		{
			get
			{
				return _VoiceAssetPath;
			}
			set
			{
				if (_VoiceAssetPath != value)
				{
					_VoiceAssetPath = value;
					GDEDataManager.SetString(_key + "_" + VoiceAssetPathKey, _VoiceAssetPath);
				}
			}
		}

		public string ThaiCaption
		{
			get
			{
				return _ThaiCaption;
			}
			set
			{
				if (_ThaiCaption != value)
				{
					_ThaiCaption = value;
					GDEDataManager.SetString(_key + "_" + ThaiCaptionKey, _ThaiCaption);
				}
			}
		}

		public string TraditionalChineseCaption
		{
			get
			{
				return _TraditionalChineseCaption;
			}
			set
			{
				if (_TraditionalChineseCaption != value)
				{
					_TraditionalChineseCaption = value;
					GDEDataManager.SetString(_key + "_" + TraditionalChineseCaptionKey, _TraditionalChineseCaption);
				}
			}
		}

		public string FrenchCaption
		{
			get
			{
				return _FrenchCaption;
			}
			set
			{
				if (_FrenchCaption != value)
				{
					_FrenchCaption = value;
					GDEDataManager.SetString(_key + "_" + FrenchCaptionKey, _FrenchCaption);
				}
			}
		}

		public string EnglishCaption
		{
			get
			{
				return _EnglishCaption;
			}
			set
			{
				if (_EnglishCaption != value)
				{
					_EnglishCaption = value;
					GDEDataManager.SetString(_key + "_" + EnglishCaptionKey, _EnglishCaption);
				}
			}
		}

		public string JapaneseCaption
		{
			get
			{
				return _JapaneseCaption;
			}
			set
			{
				if (_JapaneseCaption != value)
				{
					_JapaneseCaption = value;
					GDEDataManager.SetString(_key + "_" + JapaneseCaptionKey, _JapaneseCaption);
				}
			}
		}

		public GDEWordScriptTriggerTypeData Trigger
		{
			get
			{
				return _Trigger;
			}
			set
			{
				if (_Trigger != value)
				{
					_Trigger = value;
					GDEDataManager.SetCustom(_key + "_" + TriggerKey, _Trigger);
				}
			}
		}

		public GDEGameCharacterTypeData Actor
		{
			get
			{
				return _Actor;
			}
			set
			{
				if (_Actor != value)
				{
					_Actor = value;
					GDEDataManager.SetCustom(_key + "_" + ActorKey, _Actor);
				}
			}
		}

		public GDEGameCharacterTypeData Together_1
		{
			get
			{
				return _Together_1;
			}
			set
			{
				if (_Together_1 != value)
				{
					_Together_1 = value;
					GDEDataManager.SetCustom(_key + "_" + Together_1Key, _Together_1);
				}
			}
		}

		public GDEGameCharacterTypeData Together_2
		{
			get
			{
				return _Together_2;
			}
			set
			{
				if (_Together_2 != value)
				{
					_Together_2 = value;
					GDEDataManager.SetCustom(_key + "_" + Together_2Key, _Together_2);
				}
			}
		}

		public GDEWordScriptsListData Next
		{
			get
			{
				return _Next;
			}
			set
			{
				if (_Next != value)
				{
					_Next = value;
					GDEDataManager.SetCustom(_key + "_" + NextKey, _Next);
				}
			}
		}

		public GDEWordScriptsListData()
		{
			_key = string.Empty;
		}

		public GDEWordScriptsListData(string key)
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
			dict.TryGetString(VoiceAssetPathKey, out _VoiceAssetPath);
			dict.TryGetString(ThaiCaptionKey, out _ThaiCaption);
			dict.TryGetString(TraditionalChineseCaptionKey, out _TraditionalChineseCaption);
			dict.TryGetString(FrenchCaptionKey, out _FrenchCaption);
			dict.TryGetString(EnglishCaptionKey, out _EnglishCaption);
			dict.TryGetString(JapaneseCaptionKey, out _JapaneseCaption);
			dict.TryGetString(TriggerKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEWordScriptTriggerTypeData>(value, out _Trigger);
			dict.TryGetString(ActorKey, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEGameCharacterTypeData>(value, out _Actor);
			dict.TryGetString(Together_1Key, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEGameCharacterTypeData>(value, out _Together_1);
			dict.TryGetString(Together_2Key, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEGameCharacterTypeData>(value, out _Together_2);
			dict.TryGetString(NextKey, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEWordScriptsListData>(value, out _Next);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_VoiceAssetPath = GDEDataManager.GetString(_key + "_" + VoiceAssetPathKey, _VoiceAssetPath);
			_ThaiCaption = GDEDataManager.GetString(_key + "_" + ThaiCaptionKey, _ThaiCaption);
			_TraditionalChineseCaption = GDEDataManager.GetString(_key + "_" + TraditionalChineseCaptionKey, _TraditionalChineseCaption);
			_FrenchCaption = GDEDataManager.GetString(_key + "_" + FrenchCaptionKey, _FrenchCaption);
			_EnglishCaption = GDEDataManager.GetString(_key + "_" + EnglishCaptionKey, _EnglishCaption);
			_JapaneseCaption = GDEDataManager.GetString(_key + "_" + JapaneseCaptionKey, _JapaneseCaption);
			_Trigger = GDEDataManager.GetCustom(_key + "_" + TriggerKey, _Trigger);
			_Actor = GDEDataManager.GetCustom(_key + "_" + ActorKey, _Actor);
			_Together_1 = GDEDataManager.GetCustom(_key + "_" + Together_1Key, _Together_1);
			_Together_2 = GDEDataManager.GetCustom(_key + "_" + Together_2Key, _Together_2);
			_Next = GDEDataManager.GetCustom(_key + "_" + NextKey, _Next);
		}

		public void Reset_VoiceAssetPath()
		{
			GDEDataManager.ResetToDefault(_key, VoiceAssetPathKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(VoiceAssetPathKey, out _VoiceAssetPath);
		}

		public void Reset_ThaiCaption()
		{
			GDEDataManager.ResetToDefault(_key, ThaiCaptionKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(ThaiCaptionKey, out _ThaiCaption);
		}

		public void Reset_TraditionalChineseCaption()
		{
			GDEDataManager.ResetToDefault(_key, TraditionalChineseCaptionKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(TraditionalChineseCaptionKey, out _TraditionalChineseCaption);
		}

		public void Reset_FrenchCaption()
		{
			GDEDataManager.ResetToDefault(_key, FrenchCaptionKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(FrenchCaptionKey, out _FrenchCaption);
		}

		public void Reset_EnglishCaption()
		{
			GDEDataManager.ResetToDefault(_key, EnglishCaptionKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(EnglishCaptionKey, out _EnglishCaption);
		}

		public void Reset_JapaneseCaption()
		{
			GDEDataManager.ResetToDefault(_key, JapaneseCaptionKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(JapaneseCaptionKey, out _JapaneseCaption);
		}

		public void Reset_Trigger()
		{
			GDEDataManager.ResetToDefault(_key, TriggerKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(TriggerKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEWordScriptTriggerTypeData>(value, out _Trigger);
			Trigger = GDEDataManager.GetCustom(_key + "_" + TriggerKey, _Trigger);
			Trigger.ResetAll();
		}

		public void Reset_Actor()
		{
			GDEDataManager.ResetToDefault(_key, ActorKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(ActorKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEGameCharacterTypeData>(value, out _Actor);
			Actor = GDEDataManager.GetCustom(_key + "_" + ActorKey, _Actor);
			Actor.ResetAll();
		}

		public void Reset_Together_1()
		{
			GDEDataManager.ResetToDefault(_key, Together_1Key);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(Together_1Key, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEGameCharacterTypeData>(value, out _Together_1);
			Together_1 = GDEDataManager.GetCustom(_key + "_" + Together_1Key, _Together_1);
			Together_1.ResetAll();
		}

		public void Reset_Together_2()
		{
			GDEDataManager.ResetToDefault(_key, Together_2Key);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(Together_2Key, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEGameCharacterTypeData>(value, out _Together_2);
			Together_2 = GDEDataManager.GetCustom(_key + "_" + Together_2Key, _Together_2);
			Together_2.ResetAll();
		}

		public void Reset_Next()
		{
			GDEDataManager.ResetToDefault(_key, NextKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(NextKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEWordScriptsListData>(value, out _Next);
			Next = GDEDataManager.GetCustom(_key + "_" + NextKey, _Next);
			Next.ResetAll();
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, TriggerKey);
			GDEDataManager.ResetToDefault(_key, ActorKey);
			GDEDataManager.ResetToDefault(_key, Together_1Key);
			GDEDataManager.ResetToDefault(_key, Together_2Key);
			GDEDataManager.ResetToDefault(_key, VoiceAssetPathKey);
			GDEDataManager.ResetToDefault(_key, NextKey);
			GDEDataManager.ResetToDefault(_key, ThaiCaptionKey);
			GDEDataManager.ResetToDefault(_key, TraditionalChineseCaptionKey);
			GDEDataManager.ResetToDefault(_key, FrenchCaptionKey);
			GDEDataManager.ResetToDefault(_key, EnglishCaptionKey);
			GDEDataManager.ResetToDefault(_key, JapaneseCaptionKey);
			Reset_Trigger();
			Reset_Actor();
			Reset_Together_1();
			Reset_Together_2();
			Reset_Next();
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}