// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEReadSceneUnityTypesData
using System.Collections.Generic;
using GameDataEditor;
using UnityEngine;

namespace GameDataEditor
{
	public class GDEReadSceneUnityTypesData : IGDEData
	{
		private static string go_fieldKey = "go_field";

		private GameObject _go_field;

		private static string tex_fieldKey = "tex_field";

		private Texture2D _tex_field;

		private static string mat_fieldKey = "mat_field";

		private Material _mat_field;

		private static string aud_fieldKey = "aud_field";

		private AudioClip _aud_field;

		private static string go_list_fieldKey = "go_list_field";

		public List<GameObject> go_list_field;

		public GameObject go_field
		{
			get
			{
				return _go_field;
			}
			set
			{
				if (_go_field != value)
				{
					_go_field = value;
					GDEDataManager.SetGameObject(_key + "_" + go_fieldKey, _go_field);
				}
			}
		}

		public Texture2D tex_field
		{
			get
			{
				return _tex_field;
			}
			set
			{
				if (_tex_field != value)
				{
					_tex_field = value;
					GDEDataManager.SetTexture2D(_key + "_" + tex_fieldKey, _tex_field);
				}
			}
		}

		public Material mat_field
		{
			get
			{
				return _mat_field;
			}
			set
			{
				if (_mat_field != value)
				{
					_mat_field = value;
					GDEDataManager.SetMaterial(_key + "_" + mat_fieldKey, _mat_field);
				}
			}
		}

		public AudioClip aud_field
		{
			get
			{
				return _aud_field;
			}
			set
			{
				if (_aud_field != value)
				{
					_aud_field = value;
					GDEDataManager.SetAudioClip(_key + "_" + aud_fieldKey, _aud_field);
				}
			}
		}

		public GDEReadSceneUnityTypesData()
		{
			_key = string.Empty;
		}

		public GDEReadSceneUnityTypesData(string key)
		{
			_key = key;
		}

		public void Set_go_list_field()
		{
			GDEDataManager.SetGameObjectList(_key + "_" + go_list_fieldKey, go_list_field);
		}

		public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
		{
			_key = dataKey;
			if (dict == null)
			{
				LoadFromSavedData(dataKey);
				return;
			}
			dict.TryGetGameObject(go_fieldKey, out _go_field);
			dict.TryGetTexture2D(tex_fieldKey, out _tex_field);
			dict.TryGetMaterial(mat_fieldKey, out _mat_field);
			dict.TryGetAudioClip(aud_fieldKey, out _aud_field);
			dict.TryGetGameObjectList(go_list_fieldKey, out go_list_field);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_go_field = GDEDataManager.GetGameObject(_key + "_" + go_fieldKey, _go_field);
			_tex_field = GDEDataManager.GetTexture2D(_key + "_" + tex_fieldKey, _tex_field);
			_mat_field = GDEDataManager.GetMaterial(_key + "_" + mat_fieldKey, _mat_field);
			_aud_field = GDEDataManager.GetAudioClip(_key + "_" + aud_fieldKey, _aud_field);
			go_list_field = GDEDataManager.GetGameObjectList(_key + "_" + go_list_fieldKey, go_list_field);
		}

		public void Reset_go_field()
		{
			GDEDataManager.ResetToDefault(_key, go_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetGameObject(go_fieldKey, out _go_field);
		}

		public void Reset_tex_field()
		{
			GDEDataManager.ResetToDefault(_key, tex_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetTexture2D(tex_fieldKey, out _tex_field);
		}

		public void Reset_mat_field()
		{
			GDEDataManager.ResetToDefault(_key, mat_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetMaterial(mat_fieldKey, out _mat_field);
		}

		public void Reset_aud_field()
		{
			GDEDataManager.ResetToDefault(_key, aud_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetAudioClip(aud_fieldKey, out _aud_field);
		}

		public void Reset_go_list_field()
		{
			GDEDataManager.ResetToDefault(_key, go_list_fieldKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetGameObjectList(go_list_fieldKey, out go_list_field);
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, tex_fieldKey);
			GDEDataManager.ResetToDefault(_key, go_fieldKey);
			GDEDataManager.ResetToDefault(_key, mat_fieldKey);
			GDEDataManager.ResetToDefault(_key, go_list_fieldKey);
			GDEDataManager.ResetToDefault(_key, aud_fieldKey);
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}