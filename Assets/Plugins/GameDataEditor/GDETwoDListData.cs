// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDETwoDListData
using System.Collections.Generic;
using GameDataEditor;
using UnityEngine;

namespace GameDataEditor
{
	public class GDETwoDListData : IGDEData
	{
		private static string bKey = "b";

		public List<List<bool>> b;

		private static string iKey = "i";

		public List<List<int>> i;

		private static string fKey = "f";

		public List<List<float>> f;

		private static string sKey = "s";

		public List<List<string>> s;

		private static string v2Key = "v2";

		public List<List<Vector2>> v2;

		private static string v3Key = "v3";

		public List<List<Vector3>> v3;

		private static string v4Key = "v4";

		public List<List<Vector4>> v4;

		private static string cKey = "c";

		public List<List<Color>> c;

		private static string goKey = "go";

		public List<List<GameObject>> go;

		private static string texKey = "tex";

		public List<List<Texture2D>> tex;

		private static string matKey = "mat";

		public List<List<Material>> mat;

		private static string audKey = "aud";

		public List<List<AudioClip>> aud;

		private static string cusKey = "cus";

		public List<List<GDETwoDListCustomData>> cus;

		public GDETwoDListData()
		{
			_key = string.Empty;
		}

		public GDETwoDListData(string key)
		{
			_key = key;
		}

		public void Set_b()
		{
			GDEDataManager.SetBoolTwoDList(_key + "_" + bKey, b);
		}

		public void Set_i()
		{
			GDEDataManager.SetIntTwoDList(_key + "_" + iKey, i);
		}

		public void Set_f()
		{
			GDEDataManager.SetFloatTwoDList(_key + "_" + fKey, f);
		}

		public void Set_s()
		{
			GDEDataManager.SetStringTwoDList(_key + "_" + sKey, s);
		}

		public void Set_v2()
		{
			GDEDataManager.SetVector2TwoDList(_key + "_" + v2Key, v2);
		}

		public void Set_v3()
		{
			GDEDataManager.SetVector3TwoDList(_key + "_" + v3Key, v3);
		}

		public void Set_v4()
		{
			GDEDataManager.SetVector4TwoDList(_key + "_" + v4Key, v4);
		}

		public void Set_c()
		{
			GDEDataManager.SetColorTwoDList(_key + "_" + cKey, c);
		}

		public void Set_go()
		{
			GDEDataManager.SetGameObjectTwoDList(_key + "_" + goKey, go);
		}

		public void Set_tex()
		{
			GDEDataManager.SetTexture2DTwoDList(_key + "_" + texKey, tex);
		}

		public void Set_mat()
		{
			GDEDataManager.SetMaterialTwoDList(_key + "_" + matKey, mat);
		}

		public void Set_aud()
		{
			GDEDataManager.SetAudioClipTwoDList(_key + "_" + audKey, aud);
		}

		public void Set_cus()
		{
			GDEDataManager.SetCustomTwoDList(_key + "_" + cusKey, cus);
		}

		public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
		{
			_key = dataKey;
			if (dict == null)
			{
				LoadFromSavedData(dataKey);
				return;
			}
			dict.TryGetBoolTwoDList(bKey, out b);
			dict.TryGetIntTwoDList(iKey, out i);
			dict.TryGetFloatTwoDList(fKey, out f);
			dict.TryGetStringTwoDList(sKey, out s);
			dict.TryGetVector2TwoDList(v2Key, out v2);
			dict.TryGetVector3TwoDList(v3Key, out v3);
			dict.TryGetVector4TwoDList(v4Key, out v4);
			dict.TryGetColorTwoDList(cKey, out c);
			dict.TryGetGameObjectTwoDList(goKey, out go);
			dict.TryGetTexture2DTwoDList(texKey, out tex);
			dict.TryGetMaterialTwoDList(matKey, out mat);
			dict.TryGetAudioClipTwoDList(audKey, out aud);
			dict.TryGetCustomTwoDList(cusKey, out cus);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			b = GDEDataManager.GetBoolTwoDList(_key + "_" + bKey, b);
			i = GDEDataManager.GetIntTwoDList(_key + "_" + iKey, i);
			f = GDEDataManager.GetFloatTwoDList(_key + "_" + fKey, f);
			s = GDEDataManager.GetStringTwoDList(_key + "_" + sKey, s);
			v2 = GDEDataManager.GetVector2TwoDList(_key + "_" + v2Key, v2);
			v3 = GDEDataManager.GetVector3TwoDList(_key + "_" + v3Key, v3);
			v4 = GDEDataManager.GetVector4TwoDList(_key + "_" + v4Key, v4);
			c = GDEDataManager.GetColorTwoDList(_key + "_" + cKey, c);
			go = GDEDataManager.GetGameObjectTwoDList(_key + "_" + goKey, go);
			tex = GDEDataManager.GetTexture2DTwoDList(_key + "_" + texKey, tex);
			mat = GDEDataManager.GetMaterialTwoDList(_key + "_" + matKey, mat);
			aud = GDEDataManager.GetAudioClipTwoDList(_key + "_" + audKey, aud);
			cus = GDEDataManager.GetCustomTwoDList(_key + "_" + cusKey, cus);
		}

		public void Reset_b()
		{
			GDEDataManager.ResetToDefault(_key, bKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetBoolTwoDList(bKey, out b);
		}

		public void Reset_i()
		{
			GDEDataManager.ResetToDefault(_key, iKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetIntTwoDList(iKey, out i);
		}

		public void Reset_f()
		{
			GDEDataManager.ResetToDefault(_key, fKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloatTwoDList(fKey, out f);
		}

		public void Reset_s()
		{
			GDEDataManager.ResetToDefault(_key, sKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetStringTwoDList(sKey, out s);
		}

		public void Reset_v2()
		{
			GDEDataManager.ResetToDefault(_key, v2Key);
			GDEDataManager.Get(_key, out var data);
			data.TryGetVector2TwoDList(v2Key, out v2);
		}

		public void Reset_v3()
		{
			GDEDataManager.ResetToDefault(_key, v3Key);
			GDEDataManager.Get(_key, out var data);
			data.TryGetVector3TwoDList(v3Key, out v3);
		}

		public void Reset_v4()
		{
			GDEDataManager.ResetToDefault(_key, v4Key);
			GDEDataManager.Get(_key, out var data);
			data.TryGetVector4TwoDList(v4Key, out v4);
		}

		public void Reset_c()
		{
			GDEDataManager.ResetToDefault(_key, cKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetColorTwoDList(cKey, out c);
		}

		public void Reset_go()
		{
			GDEDataManager.ResetToDefault(_key, goKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetGameObjectTwoDList(goKey, out go);
		}

		public void Reset_tex()
		{
			GDEDataManager.ResetToDefault(_key, texKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetTexture2DTwoDList(texKey, out tex);
		}

		public void Reset_mat()
		{
			GDEDataManager.ResetToDefault(_key, matKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetMaterialTwoDList(matKey, out mat);
		}

		public void Reset_aud()
		{
			GDEDataManager.ResetToDefault(_key, audKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetAudioClipTwoDList(audKey, out aud);
		}

		public void Reset_cus()
		{
			GDEDataManager.ResetToDefault(_key, cusKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetCustomTwoDList(cusKey, out cus);
			cus = GDEDataManager.GetCustomTwoDList(_key + "_" + cusKey, cus);
			cus.ForEach(delegate(List<GDETwoDListCustomData> x)
			{
				x.ForEach(delegate(GDETwoDListCustomData y)
				{
					y.ResetAll();
				});
			});
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, bKey);
			GDEDataManager.ResetToDefault(_key, iKey);
			GDEDataManager.ResetToDefault(_key, fKey);
			GDEDataManager.ResetToDefault(_key, sKey);
			GDEDataManager.ResetToDefault(_key, v2Key);
			GDEDataManager.ResetToDefault(_key, v3Key);
			GDEDataManager.ResetToDefault(_key, v4Key);
			GDEDataManager.ResetToDefault(_key, cKey);
			GDEDataManager.ResetToDefault(_key, cusKey);
			GDEDataManager.ResetToDefault(_key, goKey);
			GDEDataManager.ResetToDefault(_key, texKey);
			GDEDataManager.ResetToDefault(_key, matKey);
			GDEDataManager.ResetToDefault(_key, audKey);
			Reset_cus();
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}