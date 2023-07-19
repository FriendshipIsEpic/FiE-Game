// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDESkillTreeData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDESkillTreeData : IGDEData
	{
		private static string IDKey = "ID";

		private int _ID;

		private static string SkillLevelKey = "SkillLevel";

		private int _SkillLevel;

		private static string OptionIDKey = "OptionID";

		private int _OptionID;

		private static string CostKey = "Cost";

		private int _Cost;

		private static string Value1Key = "Value1";

		private float _Value1;

		private static string Value2Key = "Value2";

		private float _Value2;

		private static string Value3Key = "Value3";

		private float _Value3;

		private static string Value4Key = "Value4";

		private float _Value4;

		private static string Value5Key = "Value5";

		private float _Value5;

		private static string SkillGroupKey = "SkillGroup";

		private GDESkillGroupData _SkillGroup;

		private static string GameCharacterTypeKey = "GameCharacterType";

		private GDEGameCharacterTypeData _GameCharacterType;

		private static string AbilityKey = "Ability";

		private GDEAbilityData _Ability;

		private static string SkillTypeKey = "SkillType";

		private GDESkillTypeData _SkillType;

		private static string RelatedSkillKey = "RelatedSkill";

		private GDESkillTreeData _RelatedSkill;

		private static string UIColorKey = "UIColor";

		private GDESkillUIColorData _UIColor;

		private static string SpecificDescriptionTextKey = "SpecificDescriptionText";

		private GDEConstantTextListData _SpecificDescriptionText;

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

		public int SkillLevel
		{
			get
			{
				return _SkillLevel;
			}
			set
			{
				if (_SkillLevel != value)
				{
					_SkillLevel = value;
					GDEDataManager.SetInt(_key + "_" + SkillLevelKey, _SkillLevel);
				}
			}
		}

		public int OptionID
		{
			get
			{
				return _OptionID;
			}
			set
			{
				if (_OptionID != value)
				{
					_OptionID = value;
					GDEDataManager.SetInt(_key + "_" + OptionIDKey, _OptionID);
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

		public float Value1
		{
			get
			{
				return _Value1;
			}
			set
			{
				if (_Value1 != value)
				{
					_Value1 = value;
					GDEDataManager.SetFloat(_key + "_" + Value1Key, _Value1);
				}
			}
		}

		public float Value2
		{
			get
			{
				return _Value2;
			}
			set
			{
				if (_Value2 != value)
				{
					_Value2 = value;
					GDEDataManager.SetFloat(_key + "_" + Value2Key, _Value2);
				}
			}
		}

		public float Value3
		{
			get
			{
				return _Value3;
			}
			set
			{
				if (_Value3 != value)
				{
					_Value3 = value;
					GDEDataManager.SetFloat(_key + "_" + Value3Key, _Value3);
				}
			}
		}

		public float Value4
		{
			get
			{
				return _Value4;
			}
			set
			{
				if (_Value4 != value)
				{
					_Value4 = value;
					GDEDataManager.SetFloat(_key + "_" + Value4Key, _Value4);
				}
			}
		}

		public float Value5
		{
			get
			{
				return _Value5;
			}
			set
			{
				if (_Value5 != value)
				{
					_Value5 = value;
					GDEDataManager.SetFloat(_key + "_" + Value5Key, _Value5);
				}
			}
		}

		public GDESkillGroupData SkillGroup
		{
			get
			{
				return _SkillGroup;
			}
			set
			{
				if (_SkillGroup != value)
				{
					_SkillGroup = value;
					GDEDataManager.SetCustom(_key + "_" + SkillGroupKey, _SkillGroup);
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

		public GDEAbilityData Ability
		{
			get
			{
				return _Ability;
			}
			set
			{
				if (_Ability != value)
				{
					_Ability = value;
					GDEDataManager.SetCustom(_key + "_" + AbilityKey, _Ability);
				}
			}
		}

		public GDESkillTypeData SkillType
		{
			get
			{
				return _SkillType;
			}
			set
			{
				if (_SkillType != value)
				{
					_SkillType = value;
					GDEDataManager.SetCustom(_key + "_" + SkillTypeKey, _SkillType);
				}
			}
		}

		public GDESkillTreeData RelatedSkill
		{
			get
			{
				return _RelatedSkill;
			}
			set
			{
				if (_RelatedSkill != value)
				{
					_RelatedSkill = value;
					GDEDataManager.SetCustom(_key + "_" + RelatedSkillKey, _RelatedSkill);
				}
			}
		}

		public GDESkillUIColorData UIColor
		{
			get
			{
				return _UIColor;
			}
			set
			{
				if (_UIColor != value)
				{
					_UIColor = value;
					GDEDataManager.SetCustom(_key + "_" + UIColorKey, _UIColor);
				}
			}
		}

		public GDEConstantTextListData SpecificDescriptionText
		{
			get
			{
				return _SpecificDescriptionText;
			}
			set
			{
				if (_SpecificDescriptionText != value)
				{
					_SpecificDescriptionText = value;
					GDEDataManager.SetCustom(_key + "_" + SpecificDescriptionTextKey, _SpecificDescriptionText);
				}
			}
		}

		public GDESkillTreeData()
		{
			_key = string.Empty;
		}

		public GDESkillTreeData(string key)
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
			dict.TryGetInt(SkillLevelKey, out _SkillLevel);
			dict.TryGetInt(OptionIDKey, out _OptionID);
			dict.TryGetInt(CostKey, out _Cost);
			dict.TryGetFloat(Value1Key, out _Value1);
			dict.TryGetFloat(Value2Key, out _Value2);
			dict.TryGetFloat(Value3Key, out _Value3);
			dict.TryGetFloat(Value4Key, out _Value4);
			dict.TryGetFloat(Value5Key, out _Value5);
			dict.TryGetString(SkillGroupKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDESkillGroupData>(value, out _SkillGroup);
			dict.TryGetString(GameCharacterTypeKey, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEGameCharacterTypeData>(value, out _GameCharacterType);
			dict.TryGetString(AbilityKey, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEAbilityData>(value, out _Ability);
			dict.TryGetString(SkillTypeKey, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDESkillTypeData>(value, out _SkillType);
			dict.TryGetString(RelatedSkillKey, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDESkillTreeData>(value, out _RelatedSkill);
			dict.TryGetString(UIColorKey, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDESkillUIColorData>(value, out _UIColor);
			dict.TryGetString(SpecificDescriptionTextKey, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _SpecificDescriptionText);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_ID = GDEDataManager.GetInt(_key + "_" + IDKey, _ID);
			_SkillLevel = GDEDataManager.GetInt(_key + "_" + SkillLevelKey, _SkillLevel);
			_OptionID = GDEDataManager.GetInt(_key + "_" + OptionIDKey, _OptionID);
			_Cost = GDEDataManager.GetInt(_key + "_" + CostKey, _Cost);
			_Value1 = GDEDataManager.GetFloat(_key + "_" + Value1Key, _Value1);
			_Value2 = GDEDataManager.GetFloat(_key + "_" + Value2Key, _Value2);
			_Value3 = GDEDataManager.GetFloat(_key + "_" + Value3Key, _Value3);
			_Value4 = GDEDataManager.GetFloat(_key + "_" + Value4Key, _Value4);
			_Value5 = GDEDataManager.GetFloat(_key + "_" + Value5Key, _Value5);
			_SkillGroup = GDEDataManager.GetCustom(_key + "_" + SkillGroupKey, _SkillGroup);
			_GameCharacterType = GDEDataManager.GetCustom(_key + "_" + GameCharacterTypeKey, _GameCharacterType);
			_Ability = GDEDataManager.GetCustom(_key + "_" + AbilityKey, _Ability);
			_SkillType = GDEDataManager.GetCustom(_key + "_" + SkillTypeKey, _SkillType);
			_RelatedSkill = GDEDataManager.GetCustom(_key + "_" + RelatedSkillKey, _RelatedSkill);
			_UIColor = GDEDataManager.GetCustom(_key + "_" + UIColorKey, _UIColor);
			_SpecificDescriptionText = GDEDataManager.GetCustom(_key + "_" + SpecificDescriptionTextKey, _SpecificDescriptionText);
		}

		public void Reset_ID()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(IDKey, out _ID);
		}

		public void Reset_SkillLevel()
		{
			GDEDataManager.ResetToDefault(_key, SkillLevelKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(SkillLevelKey, out _SkillLevel);
		}

		public void Reset_OptionID()
		{
			GDEDataManager.ResetToDefault(_key, OptionIDKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(OptionIDKey, out _OptionID);
		}

		public void Reset_Cost()
		{
			GDEDataManager.ResetToDefault(_key, CostKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(CostKey, out _Cost);
		}

		public void Reset_Value1()
		{
			GDEDataManager.ResetToDefault(_key, Value1Key);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(Value1Key, out _Value1);
		}

		public void Reset_Value2()
		{
			GDEDataManager.ResetToDefault(_key, Value2Key);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(Value2Key, out _Value2);
		}

		public void Reset_Value3()
		{
			GDEDataManager.ResetToDefault(_key, Value3Key);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(Value3Key, out _Value3);
		}

		public void Reset_Value4()
		{
			GDEDataManager.ResetToDefault(_key, Value4Key);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(Value4Key, out _Value4);
		}

		public void Reset_Value5()
		{
			GDEDataManager.ResetToDefault(_key, Value5Key);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(Value5Key, out _Value5);
		}

		public void Reset_SkillGroup()
		{
			GDEDataManager.ResetToDefault(_key, SkillGroupKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(SkillGroupKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDESkillGroupData>(value, out _SkillGroup);
			SkillGroup = GDEDataManager.GetCustom(_key + "_" + SkillGroupKey, _SkillGroup);
			SkillGroup.ResetAll();
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

		public void Reset_Ability()
		{
			GDEDataManager.ResetToDefault(_key, AbilityKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(AbilityKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEAbilityData>(value, out _Ability);
			Ability = GDEDataManager.GetCustom(_key + "_" + AbilityKey, _Ability);
			Ability.ResetAll();
		}

		public void Reset_SkillType()
		{
			GDEDataManager.ResetToDefault(_key, SkillTypeKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(SkillTypeKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDESkillTypeData>(value, out _SkillType);
			SkillType = GDEDataManager.GetCustom(_key + "_" + SkillTypeKey, _SkillType);
			SkillType.ResetAll();
		}

		public void Reset_RelatedSkill()
		{
			GDEDataManager.ResetToDefault(_key, RelatedSkillKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(RelatedSkillKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDESkillTreeData>(value, out _RelatedSkill);
			RelatedSkill = GDEDataManager.GetCustom(_key + "_" + RelatedSkillKey, _RelatedSkill);
			RelatedSkill.ResetAll();
		}

		public void Reset_UIColor()
		{
			GDEDataManager.ResetToDefault(_key, UIColorKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(UIColorKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDESkillUIColorData>(value, out _UIColor);
			UIColor = GDEDataManager.GetCustom(_key + "_" + UIColorKey, _UIColor);
			UIColor.ResetAll();
		}

		public void Reset_SpecificDescriptionText()
		{
			GDEDataManager.ResetToDefault(_key, SpecificDescriptionTextKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(SpecificDescriptionTextKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _SpecificDescriptionText);
			SpecificDescriptionText = GDEDataManager.GetCustom(_key + "_" + SpecificDescriptionTextKey, _SpecificDescriptionText);
			SpecificDescriptionText.ResetAll();
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, IDKey);
			GDEDataManager.ResetToDefault(_key, SkillGroupKey);
			GDEDataManager.ResetToDefault(_key, SkillLevelKey);
			GDEDataManager.ResetToDefault(_key, OptionIDKey);
			GDEDataManager.ResetToDefault(_key, CostKey);
			GDEDataManager.ResetToDefault(_key, GameCharacterTypeKey);
			GDEDataManager.ResetToDefault(_key, AbilityKey);
			GDEDataManager.ResetToDefault(_key, SkillTypeKey);
			GDEDataManager.ResetToDefault(_key, Value1Key);
			GDEDataManager.ResetToDefault(_key, Value2Key);
			GDEDataManager.ResetToDefault(_key, Value3Key);
			GDEDataManager.ResetToDefault(_key, Value4Key);
			GDEDataManager.ResetToDefault(_key, Value5Key);
			GDEDataManager.ResetToDefault(_key, RelatedSkillKey);
			GDEDataManager.ResetToDefault(_key, UIColorKey);
			GDEDataManager.ResetToDefault(_key, SpecificDescriptionTextKey);
			Reset_SkillGroup();
			Reset_GameCharacterType();
			Reset_Ability();
			Reset_SkillType();
			Reset_RelatedSkill();
			Reset_UIColor();
			Reset_SpecificDescriptionText();
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}