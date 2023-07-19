// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEReadSceneCustomData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDEReadSceneCustomData : IGDEData
	{
		private static string descriptionKey = "description";

		private string _description;

		public string description
		{
			get
			{
				return _description;
			}
			set
			{
				if (_description != value)
				{
					_description = value;
					GDEDataManager.SetString(_key + "_" + descriptionKey, _description);
				}
			}
		}

		public GDEReadSceneCustomData()
		{
			_key = string.Empty;
		}

		public GDEReadSceneCustomData(string key)
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
			dict.TryGetString(descriptionKey, out _description);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_description = GDEDataManager.GetString(_key + "_" + descriptionKey, _description);
		}

		public void Reset_description()
		{
			GDEDataManager.ResetToDefault(_key, descriptionKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(descriptionKey, out _description);
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, descriptionKey);
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}