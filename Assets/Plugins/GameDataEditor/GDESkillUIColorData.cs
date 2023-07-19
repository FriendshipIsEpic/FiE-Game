// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDESkillUIColorData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDESkillUIColorData : IGDEData
	{
		private static string RKey = "R";

		private float _R;

		private static string GKey = "G";

		private float _G;

		private static string BKey = "B";

		private float _B;

		private static string WKey = "W";

		private float _W;

		public float R
		{
			get
			{
				return _R;
			}
			set
			{
				if (_R != value)
				{
					_R = value;
					GDEDataManager.SetFloat(_key + "_" + RKey, _R);
				}
			}
		}

		public float G
		{
			get
			{
				return _G;
			}
			set
			{
				if (_G != value)
				{
					_G = value;
					GDEDataManager.SetFloat(_key + "_" + GKey, _G);
				}
			}
		}

		public float B
		{
			get
			{
				return _B;
			}
			set
			{
				if (_B != value)
				{
					_B = value;
					GDEDataManager.SetFloat(_key + "_" + BKey, _B);
				}
			}
		}

		public float W
		{
			get
			{
				return _W;
			}
			set
			{
				if (_W != value)
				{
					_W = value;
					GDEDataManager.SetFloat(_key + "_" + WKey, _W);
				}
			}
		}

		public GDESkillUIColorData()
		{
			_key = string.Empty;
		}

		public GDESkillUIColorData(string key)
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
			dict.TryGetFloat(RKey, out _R);
			dict.TryGetFloat(GKey, out _G);
			dict.TryGetFloat(BKey, out _B);
			dict.TryGetFloat(WKey, out _W);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_R = GDEDataManager.GetFloat(_key + "_" + RKey, _R);
			_G = GDEDataManager.GetFloat(_key + "_" + GKey, _G);
			_B = GDEDataManager.GetFloat(_key + "_" + BKey, _B);
			_W = GDEDataManager.GetFloat(_key + "_" + WKey, _W);
		}

		public void Reset_R()
		{
			GDEDataManager.ResetToDefault(_key, RKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(RKey, out _R);
		}

		public void Reset_G()
		{
			GDEDataManager.ResetToDefault(_key, GKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(GKey, out _G);
		}

		public void Reset_B()
		{
			GDEDataManager.ResetToDefault(_key, BKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(BKey, out _B);
		}

		public void Reset_W()
		{
			GDEDataManager.ResetToDefault(_key, WKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(WKey, out _W);
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, RKey);
			GDEDataManager.ResetToDefault(_key, GKey);
			GDEDataManager.ResetToDefault(_key, BKey);
			GDEDataManager.ResetToDefault(_key, WKey);
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}