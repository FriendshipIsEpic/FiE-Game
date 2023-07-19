// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEWordScriptTriggerTypeData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDEWordScriptTriggerTypeData : IGDEData
	{
		private static string enableStakingKey = "enableStaking";

		private bool _enableStaking;

		private static string priorityKey = "priority";

		private int _priority;

		public bool enableStaking
		{
			get
			{
				return _enableStaking;
			}
			set
			{
				if (_enableStaking != value)
				{
					_enableStaking = value;
					GDEDataManager.SetBool(_key + "_" + enableStakingKey, _enableStaking);
				}
			}
		}

		public int priority
		{
			get
			{
				return _priority;
			}
			set
			{
				if (_priority != value)
				{
					_priority = value;
					GDEDataManager.SetInt(_key + "_" + priorityKey, _priority);
				}
			}
		}

		public GDEWordScriptTriggerTypeData()
		{
			_key = string.Empty;
		}

		public GDEWordScriptTriggerTypeData(string key)
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
			dict.TryGetBool(enableStakingKey, out _enableStaking);
			dict.TryGetInt(priorityKey, out _priority);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_enableStaking = GDEDataManager.GetBool(_key + "_" + enableStakingKey, _enableStaking);
			_priority = GDEDataManager.GetInt(_key + "_" + priorityKey, _priority);
		}

		public void Reset_enableStaking()
		{
			GDEDataManager.ResetToDefault(_key, enableStakingKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetBool(enableStakingKey, out _enableStaking);
		}

		public void Reset_priority()
		{
			GDEDataManager.ResetToDefault(_key, priorityKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(priorityKey, out _priority);
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, priorityKey);
			GDEDataManager.ResetToDefault(_key, enableStakingKey);
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}