// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.DictionaryExtensions
using System;
using System.Collections.Generic;
using System.Reflection;
using GameDataEditor;
using UnityEngine;

namespace GameDataEditor
{
	public static class DictionaryExtensions
	{
		public static MethodInfo DeepCopyMethodInfo = typeof(DictionaryExtensions).GetMethod("DeepCopy");

		public static bool TryAddOrUpdateValue<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, TValue value)
		{
			try
			{
				if (variable.ContainsKey(key))
				{
					variable[key] = value;
					return true;
				}
				return variable.TryAddValue(key, value);
			}
			catch
			{
				return false;
			}
		}

		public static bool TryAddValue<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, TValue value)
		{
			try
			{
				variable.Add(key, value);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool TryGetList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<object> value)
		{
			bool result = true;
			value = null;
			try
			{
				variable.TryGetValue(key, out var value2);
				value = value2 as List<object>;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetTwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<object>> value)
		{
			bool result = true;
			value = null;
			try
			{
				value = new List<List<object>>();
				if (variable.TryGetList(key, out var value2))
				{
					value = value2.ConvertAll((object obj) => obj as List<object>);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetBool<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out bool value)
		{
			bool result = true;
			value = false;
			try
			{
				variable.TryGetValue(key, out var value2);
				value = Convert.ToBoolean(value2);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetBoolList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<bool> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetList(key, out var value2))
				{
					value = value2.ConvertAll(Convert.ToBoolean);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetBoolTwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<bool>> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetList(key, out var value2))
				{
					value = new List<List<bool>>();
					foreach (object item2 in value2)
					{
						List<object> list = item2 as List<object>;
						List<bool> item = list.ConvertAll(Convert.ToBoolean);
						value.Add(item);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetString<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out string value)
		{
			bool result = true;
			value = string.Empty;
			try
			{
				variable.TryGetValue(key, out var value2);
				value = value2.ToString();
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetStringList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<string> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetList(key, out var value2))
				{
					value = value2.ConvertAll((object obj) => obj.ToString());
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetStringTwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<string>> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetList(key, out var value2))
				{
					value = new List<List<string>>();
					foreach (object item2 in value2)
					{
						List<object> list = item2 as List<object>;
						List<string> item = list.ConvertAll((object obj) => obj.ToString());
						value.Add(item);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetFloat<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out float value)
		{
			bool result = true;
			value = 0f;
			try
			{
				variable.TryGetValue(key, out var value2);
				value = Convert.ToSingle(value2);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetFloatList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<float> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetList(key, out var value2))
				{
					value = value2.ConvertAll(Convert.ToSingle);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetFloatTwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<float>> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetList(key, out var value2))
				{
					value = new List<List<float>>();
					foreach (object item2 in value2)
					{
						List<object> list = item2 as List<object>;
						List<float> item = list.ConvertAll(Convert.ToSingle);
						value.Add(item);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetInt<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out int value)
		{
			bool result = true;
			value = 0;
			try
			{
				variable.TryGetValue(key, out var value2);
				value = Convert.ToInt32(value2);
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetIntList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<int> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetList(key, out var value2))
				{
					value = value2.ConvertAll(Convert.ToInt32);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetIntTwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<int>> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetList(key, out var value2))
				{
					value = new List<List<int>>();
					foreach (object item2 in value2)
					{
						List<object> list = item2 as List<object>;
						List<int> item = list.ConvertAll(Convert.ToInt32);
						value.Add(item);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetVector2<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out Vector2 value)
		{
			bool result = true;
			value = Vector2.zero;
			try
			{
				variable.TryGetValue(key, out var value2);
				if ((object)value2 is Dictionary<string, object> dictionary)
				{
					value.x = Convert.ToSingle(dictionary["x"]);
					value.y = Convert.ToSingle(dictionary["y"]);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetVector2List<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<Vector2> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetList(key, out var value2))
				{
					value = new List<Vector2>();
					foreach (object item2 in value2)
					{
						Dictionary<string, object> dictionary = item2 as Dictionary<string, object>;
						Vector2 item = default(Vector2);
						if (dictionary != null)
						{
							dictionary.TryGetFloat("x", out item.x);
							dictionary.TryGetFloat("y", out item.y);
						}
						value.Add(item);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetVector2TwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<Vector2>> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetTwoDList(key, out var value2))
				{
					value = new List<List<Vector2>>();
					foreach (List<object> item2 in value2)
					{
						List<object> list = item2 as List<object>;
						List<Vector2> list2 = new List<Vector2>();
						foreach (object item3 in list)
						{
							Dictionary<string, object> dictionary = item3 as Dictionary<string, object>;
							Vector2 item = default(Vector2);
							if (dictionary != null)
							{
								dictionary.TryGetFloat("x", out item.x);
								dictionary.TryGetFloat("y", out item.y);
							}
							list2.Add(item);
						}
						value.Add(list2);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetVector3<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out Vector3 value)
		{
			bool result = true;
			value = Vector3.zero;
			try
			{
				variable.TryGetValue(key, out var value2);
				if ((object)value2 is Dictionary<string, object> dictionary)
				{
					value.x = Convert.ToSingle(dictionary["x"]);
					value.y = Convert.ToSingle(dictionary["y"]);
					value.z = Convert.ToSingle(dictionary["z"]);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetVector3List<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<Vector3> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetList(key, out var value2))
				{
					value = new List<Vector3>();
					foreach (object item2 in value2)
					{
						Dictionary<string, object> dictionary = item2 as Dictionary<string, object>;
						Vector3 item = default(Vector3);
						if (dictionary != null)
						{
							dictionary.TryGetFloat("x", out item.x);
							dictionary.TryGetFloat("y", out item.y);
							dictionary.TryGetFloat("z", out item.z);
						}
						value.Add(item);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetVector3TwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<Vector3>> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetTwoDList(key, out var value2))
				{
					value = new List<List<Vector3>>();
					foreach (List<object> item2 in value2)
					{
						List<object> list = item2 as List<object>;
						List<Vector3> list2 = new List<Vector3>();
						foreach (object item3 in list)
						{
							Dictionary<string, object> dictionary = item3 as Dictionary<string, object>;
							Vector3 item = default(Vector3);
							if (dictionary != null)
							{
								dictionary.TryGetFloat("x", out item.x);
								dictionary.TryGetFloat("y", out item.y);
								dictionary.TryGetFloat("z", out item.z);
							}
							list2.Add(item);
						}
						value.Add(list2);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetVector4<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out Vector4 value)
		{
			bool result = true;
			value = Vector4.zero;
			try
			{
				variable.TryGetValue(key, out var value2);
				if ((object)value2 is Dictionary<string, object> dictionary)
				{
					value.x = Convert.ToSingle(dictionary["x"]);
					value.y = Convert.ToSingle(dictionary["y"]);
					value.z = Convert.ToSingle(dictionary["z"]);
					value.w = Convert.ToSingle(dictionary["w"]);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetVector4List<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<Vector4> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetList(key, out var value2))
				{
					value = new List<Vector4>();
					foreach (object item2 in value2)
					{
						Dictionary<string, object> dictionary = item2 as Dictionary<string, object>;
						Vector4 item = default(Vector4);
						if (dictionary != null)
						{
							dictionary.TryGetFloat("x", out item.x);
							dictionary.TryGetFloat("y", out item.y);
							dictionary.TryGetFloat("z", out item.z);
							dictionary.TryGetFloat("w", out item.w);
						}
						value.Add(item);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetVector4TwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<Vector4>> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetTwoDList(key, out var value2))
				{
					value = new List<List<Vector4>>();
					foreach (List<object> item2 in value2)
					{
						List<object> list = item2 as List<object>;
						List<Vector4> list2 = new List<Vector4>();
						foreach (object item3 in list)
						{
							Dictionary<string, object> dictionary = item3 as Dictionary<string, object>;
							Vector4 item = default(Vector4);
							if (dictionary != null)
							{
								dictionary.TryGetFloat("x", out item.x);
								dictionary.TryGetFloat("y", out item.y);
								dictionary.TryGetFloat("z", out item.z);
								dictionary.TryGetFloat("w", out item.w);
							}
							list2.Add(item);
						}
						value.Add(list2);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetColor<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out Color value)
		{
			bool result = true;
			value = Color.black;
			try
			{
				variable.TryGetValue(key, out var value2);
				if ((object)value2 is Dictionary<string, object> variable2)
				{
					variable2.TryGetFloat("r", out value.r);
					variable2.TryGetFloat("g", out value.g);
					variable2.TryGetFloat("b", out value.b);
					variable2.TryGetFloat("a", out value.a);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetColorList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<Color> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetList(key, out var value2))
				{
					value = new List<Color>();
					foreach (object item2 in value2)
					{
						Dictionary<string, object> dictionary = item2 as Dictionary<string, object>;
						Color item = default(Color);
						if (dictionary != null)
						{
							dictionary.TryGetFloat("r", out item.r);
							dictionary.TryGetFloat("g", out item.g);
							dictionary.TryGetFloat("b", out item.b);
							dictionary.TryGetFloat("a", out item.a);
						}
						value.Add(item);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetColorTwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<Color>> value)
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetTwoDList(key, out var value2))
				{
					value = new List<List<Color>>();
					foreach (List<object> item2 in value2)
					{
						List<object> list = item2 as List<object>;
						List<Color> list2 = new List<Color>();
						foreach (object item3 in list)
						{
							Dictionary<string, object> dictionary = item3 as Dictionary<string, object>;
							Color item = default(Vector4);
							if (dictionary != null)
							{
								dictionary.TryGetFloat("r", out item.r);
								dictionary.TryGetFloat("g", out item.g);
								dictionary.TryGetFloat("b", out item.b);
								dictionary.TryGetFloat("a", out item.a);
							}
							list2.Add(item);
						}
						value.Add(list2);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetUnityType<TKey, TValue, T>(this Dictionary<TKey, TValue> variable, TKey key, out T value) where T : UnityEngine.Object
		{
			bool result = true;
			value = (T)null;
			try
			{
				if (variable.TryGetString(key, out var value2))
				{
					value = Resources.Load<T>(value2);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetUnityTypeList<TKey, TValue, T>(this Dictionary<TKey, TValue> variable, TKey key, out List<T> value) where T : UnityEngine.Object
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetStringList(key, out var value2))
				{
					value = new List<T>();
					foreach (string item in value2)
					{
						value.Add(Resources.Load<T>(item));
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetUnityTypeTwoDList<TKey, TValue, T>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<T>> value) where T : UnityEngine.Object
		{
			bool result = true;
			value = null;
			try
			{
				if (variable.TryGetStringTwoDList(key, out var value2))
				{
					value = new List<List<T>>();
					foreach (List<string> item in value2)
					{
						List<T> list = new List<T>();
						foreach (string item2 in item)
						{
							list.Add(Resources.Load<T>(item2));
						}
						value.Add(list);
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetGameObject<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out GameObject value)
		{
			return variable.TryGetUnityType<TKey, TValue, GameObject>(key, out value);
		}

		public static bool TryGetGameObjectList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<GameObject> value)
		{
			return variable.TryGetUnityTypeList(key, out value);
		}

		public static bool TryGetGameObjectTwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<GameObject>> value)
		{
			return variable.TryGetUnityTypeTwoDList(key, out value);
		}

		public static bool TryGetTexture2D<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out Texture2D value)
		{
			return variable.TryGetUnityType<TKey, TValue, Texture2D>(key, out value);
		}

		public static bool TryGetTexture2DList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<Texture2D> value)
		{
			return variable.TryGetUnityTypeList(key, out value);
		}

		public static bool TryGetTexture2DTwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<Texture2D>> value)
		{
			return variable.TryGetUnityTypeTwoDList(key, out value);
		}

		public static bool TryGetMaterial<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out Material value)
		{
			return variable.TryGetUnityType<TKey, TValue, Material>(key, out value);
		}

		public static bool TryGetMaterialList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<Material> value)
		{
			return variable.TryGetUnityTypeList(key, out value);
		}

		public static bool TryGetMaterialTwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<Material>> value)
		{
			return variable.TryGetUnityTypeTwoDList(key, out value);
		}

		public static bool TryGetAudioClip<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out AudioClip value)
		{
			return variable.TryGetUnityType<TKey, TValue, AudioClip>(key, out value);
		}

		public static bool TryGetAudioClipList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<AudioClip> value)
		{
			return variable.TryGetUnityTypeList(key, out value);
		}

		public static bool TryGetAudioClipTwoDList<TKey, TValue>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<AudioClip>> value)
		{
			return variable.TryGetUnityTypeTwoDList(key, out value);
		}

		public static bool TryGetCustom<TKey, TValue, T>(this Dictionary<TKey, TValue> variable, TKey key, out T value) where T : IGDEData, new()
		{
			bool result = true;
			value = new T();
			try
			{
				if (variable.TryGetValue(key, out var value2))
				{
					value.LoadFromDict(key.ToString(), value2 as Dictionary<string, object>);
				}
				else
				{
					value.LoadFromSavedData(key.ToString());
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetCustomList<TKey, TValue, T>(this Dictionary<TKey, TValue> variable, TKey key, out List<T> value) where T : IGDEData, new()
		{
			bool result = true;
			value = new List<T>();
			try
			{
				variable.TryGetStringList(key, out var value2);
				foreach (string item in value2)
				{
					GDEDataManager.DataDictionary.TryGetCustom<string, object, T>(item, out var value3);
					value.Add(value3);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static bool TryGetCustomTwoDList<TKey, TValue, T>(this Dictionary<TKey, TValue> variable, TKey key, out List<List<T>> value) where T : IGDEData, new()
		{
			bool result = true;
			value = new List<List<T>>();
			try
			{
				variable.TryGetStringTwoDList(key, out var value2);
				foreach (List<string> item in value2)
				{
					List<T> list = new List<T>();
					foreach (string item2 in item)
					{
						GDEDataManager.DataDictionary.TryGetCustom<string, object, T>(item2, out var value3);
						list.Add(value3);
					}
					value.Add(list);
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		public static Dictionary<TKey, TValue> DeepCopy<TKey, TValue>(this Dictionary<TKey, TValue> variable)
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			TKey val = default(TKey);
			TValue val2 = default(TValue);
			foreach (KeyValuePair<TKey, TValue> item in variable)
			{
				if (item.Key == null)
				{
					val = item.Key;
				}
				else if (item.Key.IsCloneableType())
				{
					val = (TKey)((ICloneable)(object)item.Key).Clone();
				}
				else if (item.Key.IsGenericList())
				{
					Type type = item.Key.GetType().GetGenericArguments()[0];
					MethodInfo methodInfo = ListExtensions.DeepCopyMethodInfo.MakeGenericMethod(type);
					val = (TKey)methodInfo.Invoke(item.Key, new object[1] { item.Key });
				}
				else if (item.Key.IsGenericDictionary())
				{
					Type[] genericArguments = item.Key.GetType().GetGenericArguments();
					Type type2 = genericArguments[0];
					Type type3 = genericArguments[1];
					MethodInfo methodInfo2 = DeepCopyMethodInfo.MakeGenericMethod(type2, type3);
					val = (TKey)methodInfo2.Invoke(item.Key, new object[1] { item.Key });
				}
				else
				{
					val = item.Key;
				}
				if (item.Value == null)
				{
					val2 = item.Value;
				}
				else if (item.Value.IsCloneableType())
				{
					val2 = (TValue)((ICloneable)(object)item.Value).Clone();
				}
				else if (item.Value.IsGenericList())
				{
					Type type4 = item.Value.GetType().GetGenericArguments()[0];
					MethodInfo methodInfo3 = ListExtensions.DeepCopyMethodInfo.MakeGenericMethod(type4);
					val2 = (TValue)methodInfo3.Invoke(item.Value, new object[1] { item.Value });
				}
				else if (item.Value.IsGenericDictionary())
				{
					Type[] genericArguments2 = item.Value.GetType().GetGenericArguments();
					Type type5 = genericArguments2[0];
					Type type6 = genericArguments2[1];
					MethodInfo methodInfo4 = DeepCopyMethodInfo.MakeGenericMethod(type5, type6);
					val2 = (TValue)methodInfo4.Invoke(item.Value, new object[1] { item.Value });
				}
				else
				{
					val2 = item.Value;
				}
				dictionary.Add(val, val2);
			}
			return dictionary;
		}
	}
}