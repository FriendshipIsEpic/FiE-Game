// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEDataManager
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GameDataEditor;
using UnityEngine;

namespace GameDataEditor
{
	public class GDEDataManager
	{
		private static bool isInitialized;

		private static Dictionary<string, object> dataDictionary;

		private static Dictionary<string, List<string>> dataKeysBySchema;

		private static string _dataFilePath;

		public static Dictionary<string, object> DataDictionary => dataDictionary;

		public static string DataFilePath
		{
			get
			{
				return _dataFilePath;
			}
			private set
			{
				_dataFilePath = value;
			}
		}

		public static bool Init(string filePath, bool encrypted = false)
		{
			bool result = true;
			if (isInitialized)
			{
				return result;
			}
			try
			{
				DataFilePath = filePath;
				TextAsset dataAsset = Resources.Load(DataFilePath) as TextAsset;
				Init(dataAsset, encrypted);
				isInitialized = true;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				result = false;
			}
			return result;
		}

		public static bool Init(TextAsset dataAsset, bool encrypted = false)
		{
			bool result = true;
			if (isInitialized)
			{
				return result;
			}
			if (dataAsset == null)
			{
				Debug.LogError("GDEInit: TextAsset is null!");
				return false;
			}
			try
			{
				string empty = string.Empty;
				empty = ((!encrypted) ? dataAsset.text : DecryptGDEData(dataAsset.bytes));
				InitFromText(empty);
				isInitialized = true;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				result = false;
			}
			return result;
		}

		public static bool InitFromText(string dataString)
		{
			bool result = true;
			if (isInitialized)
			{
				return result;
			}
			try
			{
				dataDictionary = Json.Deserialize(dataString) as Dictionary<string, object>;
				BuildDataKeysBySchemaList();
				isInitialized = true;
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				result = false;
			}
			return result;
		}

		public static string DecryptGDEData(byte[] encryptedContent)
		{
			GDECrypto gDECrypto = null;
			TextAsset textAsset = (TextAsset)Resources.Load("gde_meta_data", typeof(TextAsset));
			byte[] buffer = Convert.FromBase64String(textAsset.text);
			Resources.UnloadAsset(textAsset);
			using (MemoryStream serializationStream = new MemoryStream(buffer))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				gDECrypto = (GDECrypto)binaryFormatter.Deserialize(serializationStream);
			}
			string result = string.Empty;
			if (gDECrypto != null)
			{
				result = gDECrypto.Decrypt(encryptedContent);
			}
			return result;
		}

		private static void BuildDataKeysBySchemaList()
		{
			dataKeysBySchema = new Dictionary<string, List<string>>();
			foreach (KeyValuePair<string, object> item in dataDictionary)
			{
				if (!item.Key.StartsWith("_gdeSchema_"))
				{
					Dictionary<string, object> variable = item.Value as Dictionary<string, object>;
					variable.TryGetString("_gdeSchema", out var value);
					if (dataKeysBySchema.TryGetValue(value, out var value2))
					{
						value2.Add(item.Key);
						continue;
					}
					value2 = new List<string>();
					value2.Add(item.Key);
					dataKeysBySchema.Add(value, value2);
				}
			}
		}

		public static bool Get(string key, out Dictionary<string, object> data)
		{
			if (dataDictionary == null)
			{
				data = null;
				return false;
			}
			bool flag = true;
			flag = dataDictionary.TryGetValue(key, out var value);
			data = value as Dictionary<string, object>;
			return flag;
		}

		public static bool GetAllDataBySchema(string schema, out Dictionary<string, object> data)
		{
			if (dataDictionary == null)
			{
				data = null;
				return false;
			}
			bool result = true;
			data = new Dictionary<string, object>();
			if (dataKeysBySchema.TryGetValue(schema, out var value))
			{
				foreach (string item in value)
				{
					if (Get(item, out var data2))
					{
						data.Add(item.Clone().ToString(), data2.DeepCopy());
					}
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static bool GetAllDataKeysBySchema(string schema, out List<string> dataKeys)
		{
			if (dataDictionary == null)
			{
				dataKeys = null;
				return false;
			}
			return dataKeysBySchema.TryGetValue(schema, out dataKeys);
		}

		public static void ResetToDefault(string itemName, string fieldName)
		{
			PlayerPrefs.DeleteKey(itemName + "_" + fieldName);
		}

		public static string GetString(string key, string defaultVal)
		{
			string text = defaultVal;
			try
			{
				text = PlayerPrefs.GetString(key, text);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return text;
		}

		public static List<string> GetStringList(string key, List<string> defaultVal)
		{
			List<string> result = defaultVal;
			try
			{
				result = GDEPPX.GetStringList(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static List<List<string>> GetStringTwoDList(string key, List<List<string>> defaultVal)
		{
			List<List<string>> result = defaultVal;
			try
			{
				result = GDEPPX.Get2DStringList(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static int GetInt(string key, int defaultVal)
		{
			int num = defaultVal;
			try
			{
				num = PlayerPrefs.GetInt(key, num);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return num;
		}

		public static List<int> GetIntList(string key, List<int> defaultVal)
		{
			List<int> result = defaultVal;
			try
			{
				result = GDEPPX.GetIntList(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static List<List<int>> GetIntTwoDList(string key, List<List<int>> defaultVal)
		{
			List<List<int>> result = defaultVal;
			try
			{
				result = GDEPPX.Get2DIntList(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static float GetFloat(string key, float defaultVal)
		{
			float num = defaultVal;
			try
			{
				num = PlayerPrefs.GetFloat(key, num);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return num;
		}

		public static List<float> GetFloatList(string key, List<float> defaultVal)
		{
			List<float> list = defaultVal;
			try
			{
				list = GDEPPX.GetFloatList(key, list);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return list;
		}

		public static List<List<float>> GetFloatTwoDList(string key, List<List<float>> defaultVal)
		{
			List<List<float>> result = defaultVal;
			try
			{
				result = GDEPPX.Get2DFloatList(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static bool GetBool(string key, bool defaultVal)
		{
			bool result = defaultVal;
			try
			{
				result = GDEPPX.GetBool(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static List<bool> GetBoolList(string key, List<bool> defaultVal)
		{
			List<bool> result = defaultVal;
			try
			{
				result = GDEPPX.GetBoolList(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static List<List<bool>> GetBoolTwoDList(string key, List<List<bool>> defaultVal)
		{
			List<List<bool>> result = defaultVal;
			try
			{
				result = GDEPPX.Get2DBoolList(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static Color32 GetColor(string key, Color32 defaultVal)
		{
			Color32 result = defaultVal;
			try
			{
				result = GDEPPX.GetColor(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static List<Color> GetColorList(string key, List<Color> defaultVal)
		{
			List<Color> result = defaultVal;
			try
			{
				result = GDEPPX.GetColorList(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static List<List<Color>> GetColorTwoDList(string key, List<List<Color>> defaultVal)
		{
			List<List<Color>> result = defaultVal;
			try
			{
				result = GDEPPX.Get2DColorList(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static Vector2 GetVector2(string key, Vector2 defaultVal)
		{
			Vector2 result = defaultVal;
			try
			{
				result = GDEPPX.GetVector2(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static List<Vector2> GetVector2List(string key, List<Vector2> defaultVal)
		{
			List<Vector2> result = defaultVal;
			try
			{
				result = GDEPPX.GetVector2List(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static List<List<Vector2>> GetVector2TwoDList(string key, List<List<Vector2>> defaultVal)
		{
			List<List<Vector2>> result = defaultVal;
			try
			{
				result = GDEPPX.Get2DVector2List(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static Vector3 GetVector3(string key, Vector3 defaultVal)
		{
			Vector3 result = defaultVal;
			try
			{
				result = GDEPPX.GetVector3(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static List<Vector3> GetVector3List(string key, List<Vector3> defaultVal)
		{
			List<Vector3> result = defaultVal;
			try
			{
				result = GDEPPX.GetVector3List(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static List<List<Vector3>> GetVector3TwoDList(string key, List<List<Vector3>> defaultVal)
		{
			List<List<Vector3>> result = defaultVal;
			try
			{
				result = GDEPPX.Get2DVector3List(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static Vector4 GetVector4(string key, Vector4 defaultVal)
		{
			Vector4 result = defaultVal;
			try
			{
				result = GDEPPX.GetVector4(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static List<Vector4> GetVector4List(string key, List<Vector4> defaultVal)
		{
			List<Vector4> result = defaultVal;
			try
			{
				result = GDEPPX.GetVector4List(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static List<List<Vector4>> GetVector4TwoDList(string key, List<List<Vector4>> defaultVal)
		{
			List<List<Vector4>> result = defaultVal;
			try
			{
				result = GDEPPX.Get2DVector4List(key, defaultVal);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static GameObject GetGameObject(string key, GameObject defaultVal)
		{
			return defaultVal;
		}

		public static List<GameObject> GetGameObjectList(string key, List<GameObject> defaultVal)
		{
			return defaultVal;
		}

		public static List<List<GameObject>> GetGameObjectTwoDList(string key, List<List<GameObject>> defaultVal)
		{
			return defaultVal;
		}

		public static Texture2D GetTexture2D(string key, Texture2D defaultVal)
		{
			return defaultVal;
		}

		public static List<Texture2D> GetTexture2DList(string key, List<Texture2D> defaultVal)
		{
			return defaultVal;
		}

		public static List<List<Texture2D>> GetTexture2DTwoDList(string key, List<List<Texture2D>> defaultVal)
		{
			return defaultVal;
		}

		public static Material GetMaterial(string key, Material defaultVal)
		{
			return defaultVal;
		}

		public static List<Material> GetMaterialList(string key, List<Material> defaultVal)
		{
			return defaultVal;
		}

		public static List<List<Material>> GetMaterialTwoDList(string key, List<List<Material>> defaultVal)
		{
			return defaultVal;
		}

		public static AudioClip GetAudioClip(string key, AudioClip defaultVal)
		{
			return defaultVal;
		}

		public static List<AudioClip> GetAudioClipList(string key, List<AudioClip> defaultVal)
		{
			return defaultVal;
		}

		public static List<List<AudioClip>> GetAudioClipTwoDList(string key, List<List<AudioClip>> defaultVal)
		{
			return defaultVal;
		}

		public static T GetCustom<T>(string key, T defaultVal) where T : IGDEData, new()
		{
			T value = defaultVal;
			try
			{
				string text = ((defaultVal == null) ? string.Empty : defaultVal.Key);
				string @string = GetString(key, text);
				if (@string != text)
				{
					DataDictionary.TryGetCustom<string, object, T>(@string, out value);
					value.LoadFromSavedData(@string);
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return value;
		}

		public static List<T> GetCustomList<T>(string key, List<T> defaultVal) where T : IGDEData, new()
		{
			List<T> list = defaultVal;
			try
			{
				if (PlayerPrefs.HasKey(key))
				{
					list = new List<T>();
					List<string> stringList = GetStringList(key, null);
					if (stringList != null)
					{
						foreach (string item in stringList)
						{
							if (DataDictionary.TryGetCustom<string, object, T>(item, out var value))
							{
								list.Add(value);
							}
						}
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return list;
		}

		public static List<List<T>> GetCustomTwoDList<T>(string key, List<List<T>> defaultVal) where T : IGDEData, new()
		{
			List<List<T>> list = defaultVal;
			try
			{
				if (PlayerPrefs.HasKey(key))
				{
					list = new List<List<T>>();
					List<List<string>> stringTwoDList = GetStringTwoDList(key, null);
					if (stringTwoDList != null)
					{
						foreach (List<string> item in stringTwoDList)
						{
							List<T> list2 = new List<T>();
							foreach (string item2 in item)
							{
								if (DataDictionary.TryGetCustom<string, object, T>(item2, out var value))
								{
									list2.Add(value);
								}
							}
							list.Add(list2);
						}
					}
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return list;
		}

		public static void SetString(string key, string val)
		{
			try
			{
				PlayerPrefs.SetString(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetStringList(string key, List<string> val)
		{
			try
			{
				GDEPPX.SetStringList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetStringTwoDList(string key, List<List<string>> val)
		{
			try
			{
				GDEPPX.Set2DStringList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetInt(string key, int val)
		{
			try
			{
				PlayerPrefs.SetInt(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetIntList(string key, List<int> val)
		{
			try
			{
				GDEPPX.SetIntList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetIntTwoDList(string key, List<List<int>> val)
		{
			try
			{
				GDEPPX.Set2DIntList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetFloat(string key, float val)
		{
			try
			{
				PlayerPrefs.SetFloat(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetFloatList(string key, List<float> val)
		{
			try
			{
				GDEPPX.SetFloatList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetFloatTwoDList(string key, List<List<float>> val)
		{
			try
			{
				GDEPPX.Set2DFloatList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetBool(string key, bool val)
		{
			try
			{
				GDEPPX.SetBool(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetBoolList(string key, List<bool> val)
		{
			try
			{
				GDEPPX.SetBoolList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetBoolTwoDList(string key, List<List<bool>> val)
		{
			try
			{
				GDEPPX.Set2DBoolList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetColor(string key, Color32 val)
		{
			try
			{
				GDEPPX.SetColor(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetColorList(string key, List<Color> val)
		{
			try
			{
				GDEPPX.SetColorList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetColorTwoDList(string key, List<List<Color>> val)
		{
			try
			{
				GDEPPX.Set2DColorList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetVector2(string key, Vector2 val)
		{
			try
			{
				GDEPPX.SetVector2(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetVector2List(string key, List<Vector2> val)
		{
			try
			{
				GDEPPX.SetVector2List(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetVector2TwoDList(string key, List<List<Vector2>> val)
		{
			try
			{
				GDEPPX.Set2DVector2List(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetVector3(string key, Vector3 val)
		{
			try
			{
				GDEPPX.SetVector3(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetVector3List(string key, List<Vector3> val)
		{
			try
			{
				GDEPPX.SetVector3List(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetVector3TwoDList(string key, List<List<Vector3>> val)
		{
			try
			{
				GDEPPX.Set2DVector3List(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetVector4(string key, Vector4 val)
		{
			try
			{
				GDEPPX.SetVector4(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetVector4List(string key, List<Vector4> val)
		{
			try
			{
				GDEPPX.SetVector4List(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetVector4TwoDList(string key, List<List<Vector4>> val)
		{
			try
			{
				GDEPPX.Set2DVector4List(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetGameObject(string key, GameObject val)
		{
			try
			{
				GDEPPX.SetGameObject(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetGameObjectList(string key, List<GameObject> val)
		{
			try
			{
				GDEPPX.SetGameObjectList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetGameObjectTwoDList(string key, List<List<GameObject>> val)
		{
			try
			{
				GDEPPX.Set2DGameObjectList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetTexture2D(string key, Texture2D val)
		{
			try
			{
				GDEPPX.SetTexture2D(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetTexture2DList(string key, List<Texture2D> val)
		{
			try
			{
				GDEPPX.SetTexture2DList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetTexture2DTwoDList(string key, List<List<Texture2D>> val)
		{
			try
			{
				GDEPPX.Set2DTexture2DList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetMaterial(string key, Material val)
		{
			try
			{
				GDEPPX.SetMaterial(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetMaterialList(string key, List<Material> val)
		{
			try
			{
				GDEPPX.SetMaterialList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetMaterialTwoDList(string key, List<List<Material>> val)
		{
			try
			{
				GDEPPX.Set2DMaterialList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetAudioClip(string key, AudioClip val)
		{
			try
			{
				GDEPPX.SetAudioClip(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetAudioClipList(string key, List<AudioClip> val)
		{
			try
			{
				GDEPPX.SetAudioClipList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetAudioClipTwoDList(string key, List<List<AudioClip>> val)
		{
			try
			{
				GDEPPX.Set2DAudioClipList(key, val);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public static void SetCustom<T>(string key, T val) where T : IGDEData
		{
			SetString(key, val.Key);
		}

		public static void SetCustomList<T>(string key, List<T> val) where T : IGDEData
		{
			List<string> customKeys = new List<string>();
			val.ForEach(delegate(T x)
			{
				customKeys.Add(x.Key);
			});
			SetStringList(key, customKeys);
		}

		public static void SetCustomTwoDList<T>(string key, List<List<T>> val) where T : IGDEData
		{
			List<List<string>> list = new List<List<string>>();
			foreach (List<T> item in val)
			{
				List<string> subListKeys = new List<string>();
				item.ForEach(delegate(T x)
				{
					subListKeys.Add(x.Key);
				});
				list.Add(subListKeys);
			}
			SetStringTwoDList(key, list);
		}
	}
}