// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDESetListData
using System.Collections.Generic;
using GameDataEditor;
using UnityEngine;

namespace GameDataEditor
{
	public class GDESetListData : IGDEData
	{
		private static string bool_list_fieldKey = "bool_list_field";

		public List<bool> bool_list_field;

		private static string int_list_fieldKey = "int_list_field";

		public List<int> int_list_field;

		private static string float_list_fieldKey = "float_list_field";

		public List<float> float_list_field;

		private static string string_list_fieldKey = "string_list_field";

		public List<string> string_list_field;

		private static string vector2_list_fieldKey = "vector2_list_field";

		public List<Vector2> vector2_list_field;

		private static string vector3_list_fieldKey = "vector3_list_field";

		public List<Vector3> vector3_list_field;

		private static string vector4_list_fieldKey = "vector4_list_field";

		public List<Vector4> vector4_list_field;

		private static string color_list_fieldKey = "color_list_field";

		public List<Color> color_list_field;

		private static string custom_list_fieldKey = "custom_list_field";

		public List<GDESetCustomData> custom_list_field;

		public GDESetListData()
		{
			_key = string.Empty;
		}

		public GDESetListData(string key)
		{
			_key = key;
		}

		public void Set_bool_list_field()
		{
			GDEDataManager.SetBoolList(_key + "_" + bool_list_fieldKey, bool_list_field);
		}

		public void Set_int_list_field()
		{
			GDEDataManager.SetIntList(_key + "_" + int_list_fieldKey, int_list_field);
		}

		public void Set_float_list_field()
		{
			GDEDataManager.SetFloatList(_key + "_" + float_list_fieldKey, float_list_field);
		}

		public void Set_string_list_field()
		{
			GDEDataManager.SetStringList(_key + "_" + string_list_fieldKey, string_list_field);
		}

		public void Set_vector2_list_field()
		{
			GDEDataManager.SetVector2List(_key + "_" + vector2_list_fieldKey, vector2_list_field);
		}

		public void Set_vector3_list_field()
		{
			GDEDataManager.SetVector3List(_key + "_" + vector3_list_fieldKey, vector3_list_field);
		}

		public void Set_vector4_list_field()
		{
			GDEDataManager.SetVector4List(_key + "_" + vector4_list_fieldKey, vector4_list_field);
		}

		public void Set_color_list_field()
		{
			GDEDataManager.SetColorList(_key + "_" + color_list_fieldKey, color_list_field);
		}

		public void Set_custom_list_field()
		{
			GDEDataManager.SetCustomList(_key + "_" + custom_list_fieldKey, custom_list_field);
		}

		public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
		{
			_key = dataKey;
			if (dict == null)
			{
				LoadFromSavedData(dataKey);
				return;
			}
			dict.TryGetBoolList(bool_list_fieldKey, out bool_list_field);
			dict.TryGetIntList(int_list_fieldKey, out int_list_field);
			dict.TryGetFloatList(float_list_fieldKey, out float_list_field);
			dict.TryGetStringList(string_list_fieldKey, out string_list_field);
			dict.TryGetVector2List(vector2_list_fieldKey, out vector2_list_field);
			dict.TryGetVector3List(vector3_list_fieldKey, out vector3_list_field);
			dict.TryGetVector4List(vector4_list_fieldKey, out vector4_list_field);
			dict.TryGetColorList(color_list_fieldKey, out color_list_field);
			dict.TryGetCustomList(custom_list_fieldKey, out custom_list_field);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			bool_list_field = GDEDataManager.GetBoolList(_key + "_" + bool_list_fieldKey, bool_list_field);
			int_list_field = GDEDataManager.GetIntList(_key + "_" + int_list_fieldKey, int_list_field);
			float_list_field = GDEDataManager.GetFloatList(_key + "_" + float_list_fieldKey, float_list_field);
			string_list_field = GDEDataManager.GetStringList(_key + "_" + string_list_fieldKey, string_list_field);
			vector2_list_field = GDEDataManager.GetVector2List(_key + "_" + vector2_list_fieldKey, vector2_list_field);
			vector3_list_field = GDEDataManager.GetVector3List(_key + "_" + vector3_list_fieldKey, vector3_list_field);
			vector4_list_field = GDEDataManager.GetVector4List(_key + "_" + vector4_list_fieldKey, vector4_list_field);
			color_list_field = GDEDataManager.GetColorList(_key + "_" + color_list_fieldKey, color_list_field);
			custom_list_field = GDEDataManager.GetCustomList(_key + "_" + custom_list_fieldKey, custom_list_field);
		}

		public void Reset_bool_list_field()
		{
			GDEDataManager.ResetToDefault(_key, bool_list_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetBoolList(bool_list_fieldKey, out bool_list_field);
		}

		public void Reset_int_list_field()
		{
			GDEDataManager.ResetToDefault(_key, int_list_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetIntList(int_list_fieldKey, out int_list_field);
		}

		public void Reset_float_list_field()
		{
			GDEDataManager.ResetToDefault(_key, float_list_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloatList(float_list_fieldKey, out float_list_field);
		}

		public void Reset_string_list_field()
		{
			GDEDataManager.ResetToDefault(_key, string_list_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetStringList(string_list_fieldKey, out string_list_field);
		}

		public void Reset_vector2_list_field()
		{
			GDEDataManager.ResetToDefault(_key, vector2_list_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetVector2List(vector2_list_fieldKey, out vector2_list_field);
		}

		public void Reset_vector3_list_field()
		{
			GDEDataManager.ResetToDefault(_key, vector3_list_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetVector3List(vector3_list_fieldKey, out vector3_list_field);
		}

		public void Reset_vector4_list_field()
		{
			GDEDataManager.ResetToDefault(_key, vector4_list_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetVector4List(vector4_list_fieldKey, out vector4_list_field);
		}

		public void Reset_color_list_field()
		{
			GDEDataManager.ResetToDefault(_key, color_list_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetColorList(color_list_fieldKey, out color_list_field);
		}

		public void Reset_custom_list_field()
		{
			GDEDataManager.ResetToDefault(_key, custom_list_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetCustomList(custom_list_fieldKey, out custom_list_field);
			custom_list_field = GDEDataManager.GetCustomList(_key + "_" + custom_list_fieldKey, custom_list_field);
			custom_list_field.ForEach(delegate(GDESetCustomData x)
			{
				x.ResetAll();
			});
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, int_list_fieldKey);
			GDEDataManager.ResetToDefault(_key, float_list_fieldKey);
			GDEDataManager.ResetToDefault(_key, string_list_fieldKey);
			GDEDataManager.ResetToDefault(_key, vector2_list_fieldKey);
			GDEDataManager.ResetToDefault(_key, vector3_list_fieldKey);
			GDEDataManager.ResetToDefault(_key, vector4_list_fieldKey);
			GDEDataManager.ResetToDefault(_key, color_list_fieldKey);
			GDEDataManager.ResetToDefault(_key, bool_list_fieldKey);
			GDEDataManager.ResetToDefault(_key, custom_list_fieldKey);
			Reset_custom_list_field();
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}