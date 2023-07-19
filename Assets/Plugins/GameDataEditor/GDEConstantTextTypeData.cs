// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEConstantTextTypeData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDEConstantTextTypeData : IGDEData
	{
		public GDEConstantTextTypeData()
		{
			_key = string.Empty;
		}

		public GDEConstantTextTypeData(string key)
		{
			_key = key;
		}

		public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
		{
			_key = dataKey;
			if (dict == null)
			{
				LoadFromSavedData(dataKey);
			}
			else
			{
				LoadFromSavedData(dataKey);
			}
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
		}

		public void ResetAll()
		{
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}