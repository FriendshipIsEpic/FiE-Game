// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDELevelTableData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDELevelTableData : IGDEData
	{
		private static string LevelKey = "Level";

		private int _Level;

		private static string SkillPointKey = "SkillPoint";

		private int _SkillPoint;

		private static string ExpRequirementKey = "ExpRequirement";

		private int _ExpRequirement;

		public int Level
		{
			get
			{
				return _Level;
			}
			set
			{
				if (_Level != value)
				{
					_Level = value;
					GDEDataManager.SetInt(_key + "_" + LevelKey, _Level);
				}
			}
		}

		public int SkillPoint
		{
			get
			{
				return _SkillPoint;
			}
			set
			{
				if (_SkillPoint != value)
				{
					_SkillPoint = value;
					GDEDataManager.SetInt(_key + "_" + SkillPointKey, _SkillPoint);
				}
			}
		}

		public int ExpRequirement
		{
			get
			{
				return _ExpRequirement;
			}
			set
			{
				if (_ExpRequirement != value)
				{
					_ExpRequirement = value;
					GDEDataManager.SetInt(_key + "_" + ExpRequirementKey, _ExpRequirement);
				}
			}
		}

		public GDELevelTableData()
		{
			_key = string.Empty;
		}

		public GDELevelTableData(string key)
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
			dict.TryGetInt(LevelKey, out _Level);
			dict.TryGetInt(SkillPointKey, out _SkillPoint);
			dict.TryGetInt(ExpRequirementKey, out _ExpRequirement);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_Level = GDEDataManager.GetInt(_key + "_" + LevelKey, _Level);
			_SkillPoint = GDEDataManager.GetInt(_key + "_" + SkillPointKey, _SkillPoint);
			_ExpRequirement = GDEDataManager.GetInt(_key + "_" + ExpRequirementKey, _ExpRequirement);
		}

		public void Reset_Level()
		{
			GDEDataManager.ResetToDefault(_key, LevelKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(LevelKey, out _Level);
		}

		public void Reset_SkillPoint()
		{
			GDEDataManager.ResetToDefault(_key, SkillPointKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(SkillPointKey, out _SkillPoint);
		}

		public void Reset_ExpRequirement()
		{
			GDEDataManager.ResetToDefault(_key, ExpRequirementKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(ExpRequirementKey, out _ExpRequirement);
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, LevelKey);
			GDEDataManager.ResetToDefault(_key, SkillPointKey);
			GDEDataManager.ResetToDefault(_key, ExpRequirementKey);
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}