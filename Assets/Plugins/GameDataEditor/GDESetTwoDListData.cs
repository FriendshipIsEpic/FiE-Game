// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDESetTwoDListData
using System.Collections.Generic;
using GameDataEditor;
using UnityEngine;

namespace GameDataEditor
{
	public class GDESetTwoDListData : IGDEData
	{
		private static string bool_2dlistKey = "bool_2dlist";

		public List<List<bool>> bool_2dlist;

		private static string int_2dlistKey = "int_2dlist";

		public List<List<int>> int_2dlist;

		private static string float_2dlistKey = "float_2dlist";

		public List<List<float>> float_2dlist;

		private static string string_2dlistKey = "string_2dlist";

		public List<List<string>> string_2dlist;

		private static string vector2_2dlistKey = "vector2_2dlist";

		public List<List<Vector2>> vector2_2dlist;

		private static string vector3_2dlistKey = "vector3_2dlist";

		public List<List<Vector3>> vector3_2dlist;

		private static string vector4_2dlistKey = "vector4_2dlist";

		public List<List<Vector4>> vector4_2dlist;

		private static string color_2dlistKey = "color_2dlist";

		public List<List<Color>> color_2dlist;

		private static string custom_2dlistKey = "custom_2dlist";

		public List<List<GDESetCustomData>> custom_2dlist;

		public GDESetTwoDListData()
		{
			_key = string.Empty;
		}

		public GDESetTwoDListData(string key)
		{
			_key = key;
		}

		public void Set_bool_2dlist()
		{
			GDEDataManager.SetBoolTwoDList(_key + "_" + bool_2dlistKey, bool_2dlist);
		}

		public void Set_int_2dlist()
		{
			GDEDataManager.SetIntTwoDList(_key + "_" + int_2dlistKey, int_2dlist);
		}

		public void Set_float_2dlist()
		{
			GDEDataManager.SetFloatTwoDList(_key + "_" + float_2dlistKey, float_2dlist);
		}

		public void Set_string_2dlist()
		{
			GDEDataManager.SetStringTwoDList(_key + "_" + string_2dlistKey, string_2dlist);
		}

		public void Set_vector2_2dlist()
		{
			GDEDataManager.SetVector2TwoDList(_key + "_" + vector2_2dlistKey, vector2_2dlist);
		}

		public void Set_vector3_2dlist()
		{
			GDEDataManager.SetVector3TwoDList(_key + "_" + vector3_2dlistKey, vector3_2dlist);
		}

		public void Set_vector4_2dlist()
		{
			GDEDataManager.SetVector4TwoDList(_key + "_" + vector4_2dlistKey, vector4_2dlist);
		}

		public void Set_color_2dlist()
		{
			GDEDataManager.SetColorTwoDList(_key + "_" + color_2dlistKey, color_2dlist);
		}

		public void Set_custom_2dlist()
		{
			GDEDataManager.SetCustomTwoDList(_key + "_" + custom_2dlistKey, custom_2dlist);
		}

		public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
		{
			_key = dataKey;
			if (dict == null)
			{
				LoadFromSavedData(dataKey);
				return;
			}
			dict.TryGetBoolTwoDList(bool_2dlistKey, out bool_2dlist);
			dict.TryGetIntTwoDList(int_2dlistKey, out int_2dlist);
			dict.TryGetFloatTwoDList(float_2dlistKey, out float_2dlist);
			dict.TryGetStringTwoDList(string_2dlistKey, out string_2dlist);
			dict.TryGetVector2TwoDList(vector2_2dlistKey, out vector2_2dlist);
			dict.TryGetVector3TwoDList(vector3_2dlistKey, out vector3_2dlist);
			dict.TryGetVector4TwoDList(vector4_2dlistKey, out vector4_2dlist);
			dict.TryGetColorTwoDList(color_2dlistKey, out color_2dlist);
			dict.TryGetCustomTwoDList(custom_2dlistKey, out custom_2dlist);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			bool_2dlist = GDEDataManager.GetBoolTwoDList(_key + "_" + bool_2dlistKey, bool_2dlist);
			int_2dlist = GDEDataManager.GetIntTwoDList(_key + "_" + int_2dlistKey, int_2dlist);
			float_2dlist = GDEDataManager.GetFloatTwoDList(_key + "_" + float_2dlistKey, float_2dlist);
			string_2dlist = GDEDataManager.GetStringTwoDList(_key + "_" + string_2dlistKey, string_2dlist);
			vector2_2dlist = GDEDataManager.GetVector2TwoDList(_key + "_" + vector2_2dlistKey, vector2_2dlist);
			vector3_2dlist = GDEDataManager.GetVector3TwoDList(_key + "_" + vector3_2dlistKey, vector3_2dlist);
			vector4_2dlist = GDEDataManager.GetVector4TwoDList(_key + "_" + vector4_2dlistKey, vector4_2dlist);
			color_2dlist = GDEDataManager.GetColorTwoDList(_key + "_" + color_2dlistKey, color_2dlist);
			custom_2dlist = GDEDataManager.GetCustomTwoDList(_key + "_" + custom_2dlistKey, custom_2dlist);
		}

		public void Reset_bool_2dlist()
		{
			GDEDataManager.ResetToDefault(_key, bool_2dlistKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetBoolTwoDList(bool_2dlistKey, out bool_2dlist);
		}

		public void Reset_int_2dlist()
		{
			GDEDataManager.ResetToDefault(_key, int_2dlistKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetIntTwoDList(int_2dlistKey, out int_2dlist);
		}

		public void Reset_float_2dlist()
		{
			GDEDataManager.ResetToDefault(_key, float_2dlistKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloatTwoDList(float_2dlistKey, out float_2dlist);
		}

		public void Reset_string_2dlist()
		{
			GDEDataManager.ResetToDefault(_key, string_2dlistKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetStringTwoDList(string_2dlistKey, out string_2dlist);
		}

		public void Reset_vector2_2dlist()
		{
			GDEDataManager.ResetToDefault(_key, vector2_2dlistKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetVector2TwoDList(vector2_2dlistKey, out vector2_2dlist);
		}

		public void Reset_vector3_2dlist()
		{
			GDEDataManager.ResetToDefault(_key, vector3_2dlistKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetVector3TwoDList(vector3_2dlistKey, out vector3_2dlist);
		}

		public void Reset_vector4_2dlist()
		{
			GDEDataManager.ResetToDefault(_key, vector4_2dlistKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetVector4TwoDList(vector4_2dlistKey, out vector4_2dlist);
		}

		public void Reset_color_2dlist()
		{
			GDEDataManager.ResetToDefault(_key, color_2dlistKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetColorTwoDList(color_2dlistKey, out color_2dlist);
		}

		public void Reset_custom_2dlist()
		{
			GDEDataManager.ResetToDefault(_key, custom_2dlistKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetCustomTwoDList(custom_2dlistKey, out custom_2dlist);
			custom_2dlist = GDEDataManager.GetCustomTwoDList(_key + "_" + custom_2dlistKey, custom_2dlist);
			custom_2dlist.ForEach(delegate(List<GDESetCustomData> x)
			{
				x.ForEach(delegate(GDESetCustomData y)
				{
					y.ResetAll();
				});
			});
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, custom_2dlistKey);
			GDEDataManager.ResetToDefault(_key, color_2dlistKey);
			GDEDataManager.ResetToDefault(_key, vector4_2dlistKey);
			GDEDataManager.ResetToDefault(_key, vector3_2dlistKey);
			GDEDataManager.ResetToDefault(_key, vector2_2dlistKey);
			GDEDataManager.ResetToDefault(_key, string_2dlistKey);
			GDEDataManager.ResetToDefault(_key, float_2dlistKey);
			GDEDataManager.ResetToDefault(_key, int_2dlistKey);
			GDEDataManager.ResetToDefault(_key, bool_2dlistKey);
			Reset_custom_2dlist();
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}