// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEPPX
using System;
using System.Collections;
using System.Collections.Generic;
using GameDataEditor;
using UnityEngine;

namespace GameDataEditor
{
	public class GDEPPX
	{
		private enum ListType
		{
			Float,
			Int32,
			Bool,
			String,
			Vector2,
			Vector3,
			Quaternion,
			Color,
			Vector4,
			Float_2D,
			Int32_2D,
			Bool_2D,
			String_2D,
			Vector2_2D,
			Vector3_2D,
			Vector4_2D,
			Quaternion_2D,
			Color_2D
		}

		private static int endianDiff1;

		private static int endianDiff2;

		private static int idx;

		private static byte[] byteBlock;

		private static void ConvertToInt(List<int> list, byte[] bytes)
		{
			ConvertToInt((IList)list, bytes);
		}

		private static void ConvertToInt(IList list, byte[] bytes)
		{
			list.Add(ConvertBytesToInt32(bytes));
		}

		private static void ConvertToFloat(List<float> list, byte[] bytes)
		{
			ConvertToFloat((IList)list, bytes);
		}

		private static void ConvertToFloat(IList list, byte[] bytes)
		{
			list.Add(ConvertBytesToFloat(bytes));
		}

		private static void ConvertToVector2(List<Vector2> list, byte[] bytes)
		{
			ConvertToVector2((IList)list, bytes);
		}

		private static void ConvertToVector2(IList list, byte[] bytes)
		{
			list.Add(new Vector2(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
		}

		private static void ConvertToVector3(List<Vector3> list, byte[] bytes)
		{
			ConvertToVector3((IList)list, bytes);
		}

		private static void ConvertToVector3(IList list, byte[] bytes)
		{
			list.Add(new Vector3(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
		}

		private static void ConvertToVector4(List<Vector4> list, byte[] bytes)
		{
			ConvertToVector4((IList)list, bytes);
		}

		private static void ConvertToVector4(IList list, byte[] bytes)
		{
			list.Add(new Vector4(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
		}

		private static void ConvertToQuaternion(List<Quaternion> list, byte[] bytes)
		{
			ConvertToQuaternion((IList)list, bytes);
		}

		private static void ConvertToQuaternion(IList list, byte[] bytes)
		{
			list.Add(new Quaternion(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
		}

		private static void ConvertToColor(List<Color> list, byte[] bytes)
		{
			ConvertToColor((IList)list, bytes);
		}

		private static void ConvertToColor(IList list, byte[] bytes)
		{
			list.Add(new Color(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
		}

		private static void ConvertFromInt(int[] array, byte[] bytes, int i)
		{
			ConvertInt32ToBytes(array[i], bytes);
		}

		private static void ConvertFromInt(IList list, byte[] bytes, int i)
		{
			ConvertInt32ToBytes((int)list[i], bytes);
		}

		private static void ConvertFromFloat(float[] array, byte[] bytes, int i)
		{
			ConvertFloatToBytes(array[i], bytes);
		}

		private static void ConvertFromFloat(IList list, byte[] bytes, int i)
		{
			ConvertFloatToBytes((float)list[i], bytes);
		}

		private static void ConvertFromVector2(Vector2[] array, byte[] bytes, int i)
		{
			ConvertFloatToBytes(array[i].x, bytes);
			ConvertFloatToBytes(array[i].y, bytes);
		}

		private static void ConvertFromVector2(IList list, byte[] bytes, int i)
		{
			ConvertFloatToBytes(((Vector2)list[i]).x, bytes);
			ConvertFloatToBytes(((Vector2)list[i]).y, bytes);
		}

		private static void ConvertFromVector3(Vector3[] array, byte[] bytes, int i)
		{
			ConvertFloatToBytes(array[i].x, bytes);
			ConvertFloatToBytes(array[i].y, bytes);
			ConvertFloatToBytes(array[i].z, bytes);
		}

		private static void ConvertFromVector3(IList list, byte[] bytes, int i)
		{
			ConvertFloatToBytes(((Vector3)list[i]).x, bytes);
			ConvertFloatToBytes(((Vector3)list[i]).y, bytes);
			ConvertFloatToBytes(((Vector3)list[i]).z, bytes);
		}

		private static void ConvertFromVector4(Vector4[] array, byte[] bytes, int i)
		{
			ConvertFloatToBytes(array[i].x, bytes);
			ConvertFloatToBytes(array[i].y, bytes);
			ConvertFloatToBytes(array[i].z, bytes);
			ConvertFloatToBytes(array[i].w, bytes);
		}

		private static void ConvertFromVector4(IList list, byte[] bytes, int i)
		{
			ConvertFloatToBytes(((Vector4)list[i]).x, bytes);
			ConvertFloatToBytes(((Vector4)list[i]).y, bytes);
			ConvertFloatToBytes(((Vector4)list[i]).z, bytes);
			ConvertFloatToBytes(((Vector4)list[i]).w, bytes);
		}

		private static void ConvertFromQuaternion(Quaternion[] array, byte[] bytes, int i)
		{
			ConvertFloatToBytes(array[i].x, bytes);
			ConvertFloatToBytes(array[i].y, bytes);
			ConvertFloatToBytes(array[i].z, bytes);
			ConvertFloatToBytes(array[i].w, bytes);
		}

		private static void ConvertFromQuaternion(IList list, byte[] bytes, int i)
		{
			ConvertFloatToBytes(((Quaternion)list[i]).x, bytes);
			ConvertFloatToBytes(((Quaternion)list[i]).y, bytes);
			ConvertFloatToBytes(((Quaternion)list[i]).z, bytes);
			ConvertFloatToBytes(((Quaternion)list[i]).w, bytes);
		}

		private static void ConvertFromColor(Color[] array, byte[] bytes, int i)
		{
			ConvertFloatToBytes(array[i].r, bytes);
			ConvertFloatToBytes(array[i].g, bytes);
			ConvertFloatToBytes(array[i].b, bytes);
			ConvertFloatToBytes(array[i].a, bytes);
		}

		private static void ConvertFromColor(IList list, byte[] bytes, int i)
		{
			ConvertFloatToBytes(((Color)list[i]).r, bytes);
			ConvertFloatToBytes(((Color)list[i]).g, bytes);
			ConvertFloatToBytes(((Color)list[i]).b, bytes);
			ConvertFloatToBytes(((Color)list[i]).a, bytes);
		}

		private static void ConvertFloatToBytes(float f, byte[] bytes)
		{
			byteBlock = BitConverter.GetBytes(f);
			ConvertTo4Bytes(bytes);
		}

		private static float ConvertBytesToFloat(byte[] bytes)
		{
			ConvertFrom4Bytes(bytes);
			return BitConverter.ToSingle(byteBlock, 0);
		}

		private static void ConvertInt32ToBytes(int i, byte[] bytes)
		{
			byteBlock = BitConverter.GetBytes(i);
			ConvertTo4Bytes(bytes);
		}

		private static int ConvertBytesToInt32(byte[] bytes)
		{
			ConvertFrom4Bytes(bytes);
			return BitConverter.ToInt32(byteBlock, 0);
		}

		private static void ConvertTo4Bytes(byte[] bytes)
		{
			bytes[idx] = byteBlock[endianDiff1];
			bytes[idx + 1] = byteBlock[1 + endianDiff2];
			bytes[idx + 2] = byteBlock[2 - endianDiff2];
			bytes[idx + 3] = byteBlock[3 - endianDiff1];
			idx += 4;
		}

		private static void ConvertFrom4Bytes(byte[] bytes)
		{
			byteBlock[endianDiff1] = bytes[idx];
			byteBlock[1 + endianDiff2] = bytes[idx + 1];
			byteBlock[2 - endianDiff2] = bytes[idx + 2];
			byteBlock[3 - endianDiff1] = bytes[idx + 3];
			idx += 4;
		}

		public static bool GetBool(string name)
		{
			return PlayerPrefs.GetInt(name) == 1;
		}

		public static bool GetBool(string name, bool defaultValue)
		{
			return 1 == PlayerPrefs.GetInt(name, defaultValue ? 1 : 0);
		}

		public static long GetLong(string key, long defaultValue)
		{
			SplitLong(defaultValue, out var lowBits, out var highBits);
			lowBits = PlayerPrefs.GetInt(key + "_lowBits", lowBits);
			highBits = PlayerPrefs.GetInt(key + "_highBits", highBits);
			ulong num = (uint)highBits;
			num <<= 32;
			return (long)(num | (uint)lowBits);
		}

		public static long GetLong(string key)
		{
			int @int = PlayerPrefs.GetInt(key + "_lowBits");
			int int2 = PlayerPrefs.GetInt(key + "_highBits");
			ulong num = (uint)int2;
			num <<= 32;
			return (long)(num | (uint)@int);
		}

		private static Vector2 GetVector2(string key)
		{
			List<float> floatList = GetFloatList(key);
			if (floatList.Count < 2)
			{
				return Vector2.zero;
			}
			return new Vector2(floatList[0], floatList[1]);
		}

		public static Vector2 GetVector2(string key, Vector2 defaultValue)
		{
			Vector2 result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetVector2(key);
			}
			return result;
		}

		public static Color GetColor(string key)
		{
			List<float> floatList = GetFloatList(key);
			if (floatList.Count < 4)
			{
				return Color.black;
			}
			return new Color(floatList[0], floatList[1], floatList[2], floatList[3]);
		}

		public static Color GetColor(string key, Color defaultValue)
		{
			Color result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetColor(key);
			}
			return result;
		}

		public static Vector3 GetVector3(string key)
		{
			List<float> floatList = GetFloatList(key);
			if (floatList.Count < 3)
			{
				return Vector3.zero;
			}
			return new Vector3(floatList[0], floatList[1], floatList[2]);
		}

		public static Vector3 GetVector3(string key, Vector3 defaultValue)
		{
			Vector3 result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetVector3(key);
			}
			return result;
		}

		public static Vector4 GetVector4(string key)
		{
			List<float> floatList = GetFloatList(key);
			if (floatList.Count < 4)
			{
				return Vector4.zero;
			}
			return new Vector4(floatList[0], floatList[1], floatList[2], floatList[3]);
		}

		public static Vector4 GetVector4(string key, Vector4 defaultValue)
		{
			Vector4 result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetVector4(key);
			}
			return result;
		}

		public static List<bool> GetBoolList(string key)
		{
			List<bool> list = new List<bool>();
			if (PlayerPrefs.HasKey(key))
			{
				byte[] array = Convert.FromBase64String(PlayerPrefs.GetString(key));
				if (array.Length < 5)
				{
					Debug.LogError($"Corrupt preference file for: {key}");
					return list;
				}
				if (array[0] != 2)
				{
					Debug.LogError($"{key} is not a boolean array.");
					return list;
				}
				Initialize();
				byte[] array2 = new byte[array.Length - 5];
				Array.Copy(array, 5, array2, 0, array2.Length);
				BitArray bitArray = new BitArray(array2);
				bitArray.Length = ConvertBytesToInt32(array);
				bool[] array3 = new bool[bitArray.Count];
				bitArray.CopyTo(array3, 0);
				list.AddRange(array3);
			}
			return list;
		}

		public static List<bool> GetBoolList(string key, List<bool> defaultValue)
		{
			List<bool> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetBoolList(key);
			}
			return result;
		}

		public static List<List<bool>> Get2DBoolList(string key)
		{
			List<List<bool>> list = new List<List<bool>>();
			if (PlayerPrefs.HasKey(key))
			{
				byte[] array = Convert.FromBase64String(PlayerPrefs.GetString(key));
				if (array.Length < 5)
				{
					Debug.LogError($"Corrupt preference file for: {key}");
					return list;
				}
				if (array[0] != 11)
				{
					Debug.LogError($"{key} is not a boolean array.");
					return list;
				}
				int num = BitConverter.ToInt32(array, 1);
				int num2 = num * 4 + 5;
				byte[] array2 = new byte[array.Length - num2];
				Array.Copy(array, num2, array2, 0, array2.Length);
				BitArray bitArray = new BitArray(array2);
				int num3 = 5;
				int num4 = 0;
				int num5 = 0;
				for (int i = 0; i < num; i++)
				{
					num4 = BitConverter.ToInt32(array, num3);
					num3 += 4;
					List<bool> list2 = new List<bool>();
					for (int j = 0; j < num4; j++)
					{
						list2.Add(bitArray.Get(num5++));
					}
					list.Add(list2);
				}
			}
			return list;
		}

		public static List<List<bool>> Get2DBoolList(string key, List<List<bool>> defaultValue)
		{
			List<List<bool>> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = Get2DBoolList(key);
			}
			return result;
		}

		public static List<string> GetStringList(string key)
		{
			if (PlayerPrefs.HasKey(key))
			{
				string @string = PlayerPrefs.GetString(key);
				List<object> list = Json.Deserialize(@string) as List<object>;
				return list.ConvertAll((object obj) => obj.ToString());
			}
			return new List<string>();
		}

		public static List<string> GetStringList(string key, List<string> defaultValue)
		{
			List<string> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetStringList(key);
			}
			return result;
		}

		public static List<List<string>> Get2DStringList(string key)
		{
			if (PlayerPrefs.HasKey(key))
			{
				string @string = PlayerPrefs.GetString(key);
				List<object> list = Json.Deserialize(@string) as List<object>;
				List<List<string>> list2 = new List<List<string>>();
				{
					foreach (object item in list)
					{
						List<object> list3 = item as List<object>;
						list2.Add(list3.ConvertAll((object obj) => obj.ToString()));
					}
					return list2;
				}
			}
			return new List<List<string>>();
		}

		public static List<List<string>> Get2DStringList(string key, List<List<string>> defaultValue)
		{
			List<List<string>> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = Get2DStringList(key);
			}
			return result;
		}

		public static List<int> GetIntList(string key)
		{
			List<int> list = new List<int>();
			GetValue(key, list, ListType.Int32, 1, ConvertToInt);
			return list;
		}

		public static List<int> GetIntList(string key, List<int> defaultValue)
		{
			List<int> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetIntList(key);
			}
			return result;
		}

		public static List<List<int>> Get2DIntList(string key)
		{
			List<List<int>> list = new List<List<int>>();
			Get2DValue(key, list, ListType.Int32_2D, 1, ConvertToInt);
			return list;
		}

		public static List<List<int>> Get2DIntList(string key, List<List<int>> defaultValue)
		{
			List<List<int>> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = Get2DIntList(key);
			}
			return result;
		}

		public static List<float> GetFloatList(string key)
		{
			List<float> list = new List<float>();
			GetValue(key, list, ListType.Float, 1, ConvertToFloat);
			return list;
		}

		public static List<float> GetFloatList(string key, List<float> defaultValue)
		{
			List<float> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetFloatList(key);
			}
			return result;
		}

		public static List<List<float>> Get2DFloatList(string key)
		{
			List<List<float>> list = new List<List<float>>();
			Get2DValue(key, list, ListType.Float_2D, 1, ConvertToFloat);
			return list;
		}

		public static List<List<float>> Get2DFloatList(string key, List<List<float>> defaultValue)
		{
			List<List<float>> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = Get2DFloatList(key);
			}
			return result;
		}

		public static List<Vector2> GetVector2List(string key)
		{
			List<Vector2> list = new List<Vector2>();
			GetValue(key, list, ListType.Vector2, 2, ConvertToVector2);
			return list;
		}

		public static List<Vector2> GetVector2List(string key, List<Vector2> defaultValue)
		{
			List<Vector2> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetVector2List(key);
			}
			return result;
		}

		public static List<List<Vector2>> Get2DVector2List(string key)
		{
			List<List<Vector2>> list = new List<List<Vector2>>();
			Get2DValue(key, list, ListType.Vector2_2D, 2, ConvertToVector2);
			return list;
		}

		public static List<List<Vector2>> Get2DVector2List(string key, List<List<Vector2>> defaultValue)
		{
			List<List<Vector2>> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = Get2DVector2List(key);
			}
			return result;
		}

		public static List<Vector3> GetVector3List(string key)
		{
			List<Vector3> list = new List<Vector3>();
			GetValue(key, list, ListType.Vector3, 3, ConvertToVector3);
			return list;
		}

		public static List<Vector3> GetVector3List(string key, List<Vector3> defaultValue)
		{
			List<Vector3> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetVector3List(key);
			}
			return result;
		}

		public static List<List<Vector3>> Get2DVector3List(string key)
		{
			List<List<Vector3>> list = new List<List<Vector3>>();
			Get2DValue(key, list, ListType.Vector3_2D, 3, ConvertToVector3);
			return list;
		}

		public static List<List<Vector3>> Get2DVector3List(string key, List<List<Vector3>> defaultValue)
		{
			List<List<Vector3>> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = Get2DVector3List(key);
			}
			return result;
		}

		public static List<Vector4> GetVector4List(string key)
		{
			List<Vector4> list = new List<Vector4>();
			GetValue(key, list, ListType.Vector4, 4, ConvertToVector4);
			return list;
		}

		public static List<Vector4> GetVector4List(string key, List<Vector4> defaultValue)
		{
			List<Vector4> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetVector4List(key);
			}
			return result;
		}

		public static List<List<Vector4>> Get2DVector4List(string key)
		{
			List<List<Vector4>> list = new List<List<Vector4>>();
			Get2DValue(key, list, ListType.Vector4_2D, 4, ConvertToVector4);
			return list;
		}

		public static List<List<Vector4>> Get2DVector4List(string key, List<List<Vector4>> defaultValue)
		{
			List<List<Vector4>> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = Get2DVector4List(key);
			}
			return result;
		}

		public static List<Quaternion> GetQuaternionList(string key)
		{
			List<Quaternion> list = new List<Quaternion>();
			GetValue(key, list, ListType.Quaternion, 4, ConvertToQuaternion);
			return list;
		}

		public static List<Quaternion> GetQuaternionList(string key, List<Quaternion> defaultValue)
		{
			List<Quaternion> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetQuaternionList(key);
			}
			return result;
		}

		public static List<List<Quaternion>> Get2DQuaternionList(string key)
		{
			List<List<Quaternion>> list = new List<List<Quaternion>>();
			Get2DValue(key, list, ListType.Quaternion_2D, 4, ConvertToQuaternion);
			return list;
		}

		public static List<List<Quaternion>> Get2DQuaternionList(string key, List<List<Quaternion>> defaultValue)
		{
			List<List<Quaternion>> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = Get2DQuaternionList(key);
			}
			return result;
		}

		public static List<Color> GetColorList(string key)
		{
			List<Color> list = new List<Color>();
			GetValue(key, list, ListType.Color, 4, ConvertToColor);
			return list;
		}

		public static List<Color> GetColorList(string key, List<Color> defaultValue)
		{
			List<Color> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetColorList(key);
			}
			return result;
		}

		public static List<List<Color>> Get2DColorList(string key)
		{
			List<List<Color>> list = new List<List<Color>>();
			Get2DValue(key, list, ListType.Color_2D, 4, ConvertToColor);
			return list;
		}

		public static List<List<Color>> Get2DColorList(string key, List<List<Color>> defaultValue)
		{
			List<List<Color>> result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = Get2DColorList(key);
			}
			return result;
		}

		private static void GetValue<T>(string key, T list, ListType arrayType, int vectorNumber, Action<T, byte[]> convert) where T : IList
		{
			if (!PlayerPrefs.HasKey(key))
			{
				return;
			}
			byte[] array = Convert.FromBase64String(PlayerPrefs.GetString(key));
			if ((array.Length - 1) % (vectorNumber * 4) != 0)
			{
				Debug.LogError($"Corrupt preference file for: {key}");
				return;
			}
			if ((ListType)array[0] != arrayType)
			{
				Debug.LogError($"{key} is not a {arrayType.ToString()} array.");
				return;
			}
			Initialize();
			int num = (array.Length - 1) / (vectorNumber * 4);
			for (int i = 0; i < num; i++)
			{
				convert(list, array);
			}
		}

		private static void Get2DValue<T>(string key, T list, ListType arrayType, int vectorNumber, Action<IList, byte[]> convert) where T : IList
		{
			if (!PlayerPrefs.HasKey(key))
			{
				return;
			}
			byte[] array = Convert.FromBase64String(PlayerPrefs.GetString(key));
			if (array.Length < 5)
			{
				Debug.LogError($"Corrupt preference file for: {key}");
				return;
			}
			int num = BitConverter.ToInt32(array, 1);
			byte[] array2 = new byte[num * 4 + 5];
			Array.Copy(array, 0, array2, 0, array2.Length);
			List<byte> list2 = new List<byte>(array);
			list2.RemoveRange(1, array2.Length - 1);
			array = list2.ToArray();
			if ((array.Length - 1) % (vectorNumber * 4) != 0)
			{
				Debug.LogError($"Corrupt preference file for: {key}");
				return;
			}
			if ((ListType)array[0] != arrayType)
			{
				Debug.LogError(string.Format("Corrupt preference file for: {0}", key, arrayType.ToString()));
				return;
			}
			Initialize();
			GetNewListForType(arrayType, out var list3);
			int num2 = (array.Length - 1) / (vectorNumber * 4);
			for (int i = 0; i < num2; i++)
			{
				convert(list3, array);
			}
			int num3 = 0;
			int num4 = 0;
			int num5 = 5;
			for (int j = 0; j < num; j++)
			{
				num3 = BitConverter.ToInt32(array2, num5);
				num5 += 4;
				GetNewListForType(arrayType, out var list4);
				for (int k = 0; k < num3; k++)
				{
					list4.Add(list3[num4++]);
				}
				list.Add(list4);
			}
		}

		private static void GetNewListForType(ListType arrayType, out IList list)
		{
			list = null;
			if (arrayType.Equals(ListType.Bool_2D))
			{
				list = new List<bool>();
			}
			else if (arrayType.Equals(ListType.Int32_2D))
			{
				list = new List<int>();
			}
			else if (arrayType.Equals(ListType.Float_2D))
			{
				list = new List<float>();
			}
			else if (arrayType.Equals(ListType.Vector2_2D))
			{
				list = new List<Vector2>();
			}
			else if (arrayType.Equals(ListType.Vector3_2D))
			{
				list = new List<Vector3>();
			}
			else if (arrayType.Equals(ListType.Vector4_2D))
			{
				list = new List<Vector4>();
			}
			else if (arrayType.Equals(ListType.Quaternion_2D))
			{
				list = new List<Quaternion>();
			}
			else if (arrayType.Equals(ListType.Color_2D))
			{
				list = new List<Color>();
			}
		}

		public static Quaternion GetQuaternion(string key)
		{
			List<float> floatList = GetFloatList(key);
			if (floatList.Count < 4)
			{
				return Quaternion.identity;
			}
			return new Quaternion(floatList[0], floatList[1], floatList[2], floatList[3]);
		}

		public static Quaternion GetQuaternion(string key, Quaternion defaultValue)
		{
			Quaternion result = defaultValue;
			if (PlayerPrefs.HasKey(key))
			{
				result = GetQuaternion(key);
			}
			return result;
		}

		private static void Initialize(bool is2DList = false)
		{
			if (BitConverter.IsLittleEndian)
			{
				endianDiff1 = 0;
				endianDiff2 = 0;
			}
			else
			{
				endianDiff1 = 3;
				endianDiff2 = 1;
			}
			if (byteBlock == null)
			{
				byteBlock = new byte[4];
			}
			if (is2DList)
			{
				idx = 0;
			}
			else
			{
				idx = 1;
			}
		}

		private static void SplitLong(long input, out int lowBits, out int highBits)
		{
			lowBits = (int)input;
			highBits = (int)(input >> 32);
		}

		public static void ShowArrayType(string key)
		{
			byte[] array = Convert.FromBase64String(PlayerPrefs.GetString(key));
			if (array.Length > 0)
			{
				ListType listType = (ListType)array[0];
				Debug.Log(string.Format("Corrupt preference file for: {0}", key, listType.ToString()));
			}
		}

		private static bool SaveBytes(string key, byte[] bytes)
		{
			try
			{
				PlayerPrefs.SetString(key, Convert.ToBase64String(bytes));
			}
			catch
			{
				return false;
			}
			return true;
		}

		public static bool SetBool(string name, bool value)
		{
			try
			{
				PlayerPrefs.SetInt(name, value ? 1 : 0);
			}
			catch
			{
				return false;
			}
			return true;
		}

		public static void SetLong(string key, long value)
		{
			SplitLong(value, out var lowBits, out var highBits);
			PlayerPrefs.SetInt(key + "_lowBits", lowBits);
			PlayerPrefs.SetInt(key + "_highBits", highBits);
		}

		public static bool SetVector2(string key, Vector2 vector)
		{
			return SetFloatList(key, new List<float> { vector.x, vector.y });
		}

		public static bool SetVector3(string key, Vector3 vector)
		{
			return SetFloatList(key, new List<float> { vector.x, vector.y, vector.z });
		}

		public static bool SetVector4(string key, Vector4 vector)
		{
			return SetFloatList(key, new List<float> { vector.x, vector.y, vector.z, vector.w });
		}

		public static bool SetQuaternion(string key, Quaternion vector)
		{
			return SetFloatList(key, new List<float> { vector.x, vector.y, vector.z, vector.w });
		}

		public static bool SetColor(string key, Color color)
		{
			return SetFloatList(key, new List<float> { color.r, color.g, color.b, color.a });
		}

		public static bool SetGameObject(string key, GameObject go)
		{
			return true;
		}

		public static bool SetTexture2D(string key, Texture2D tex)
		{
			return true;
		}

		public static bool SetMaterial(string key, Material mat)
		{
			return true;
		}

		public static bool SetAudioClip(string key, AudioClip aud)
		{
			return true;
		}

		public static bool SetBoolList(string key, List<bool> boolList)
		{
			byte[] array = new byte[(boolList.Count + 7) / 8 + 5];
			array[0] = Convert.ToByte(ListType.Bool);
			BitArray bitArray = new BitArray(boolList.ToArray());
			bitArray.CopyTo(array, 5);
			Initialize();
			ConvertInt32ToBytes(boolList.Count, array);
			return SaveBytes(key, array);
		}

		public static bool Set2DBoolList(string key, List<List<bool>> list)
		{
			int num = 5;
			List<bool> list2 = new List<bool>();
			foreach (List<bool> item in list)
			{
				num += 4;
				list2.AddRange(item);
			}
			num += (list2.Count + 7) / 8;
			int num2 = 0;
			byte[] array = new byte[num];
			array[0] = Convert.ToByte(ListType.Bool_2D);
			num2++;
			byte[] bytes = BitConverter.GetBytes(list.Count);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			bytes.CopyTo(array, num2);
			num2 += bytes.Length;
			foreach (List<bool> item2 in list)
			{
				bytes = BitConverter.GetBytes(item2.Count);
				if (!BitConverter.IsLittleEndian)
				{
					Array.Reverse(bytes);
				}
				bytes.CopyTo(array, num2);
				num2 += bytes.Length;
			}
			BitArray bitArray = new BitArray(list2.ToArray());
			bitArray.CopyTo(array, num2);
			return SaveBytes(key, array);
		}

		public static bool SetStringList(string key, List<string> list)
		{
			bool result = false;
			try
			{
				string value = Json.Serialize(list);
				PlayerPrefs.SetString(key, value);
				result = true;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static bool Set2DStringList(string key, List<List<string>> list)
		{
			bool result = false;
			try
			{
				string value = Json.Serialize(list);
				PlayerPrefs.SetString(key, value);
				result = true;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}

		public static bool SetIntList(string key, List<int> list)
		{
			return SetValue(key, list, ListType.Int32, 1, ConvertFromInt);
		}

		public static bool SetFloatList(string key, List<float> list)
		{
			return SetValue(key, list, ListType.Float, 1, ConvertFromFloat);
		}

		public static bool SetVector2List(string key, List<Vector2> list)
		{
			return SetValue(key, list, ListType.Vector2, 2, ConvertFromVector2);
		}

		public static bool SetVector3List(string key, List<Vector3> list)
		{
			return SetValue(key, list, ListType.Vector3, 3, ConvertFromVector3);
		}

		public static bool SetVector4List(string key, List<Vector4> list)
		{
			return SetValue(key, list, ListType.Vector4, 4, ConvertFromVector4);
		}

		public static bool SetQuaternionList(string key, List<Quaternion> list)
		{
			return SetValue(key, list, ListType.Quaternion, 4, ConvertFromQuaternion);
		}

		public static bool SetColorList(string key, List<Color> list)
		{
			return SetValue(key, list, ListType.Color, 4, ConvertFromColor);
		}

		public static bool SetGameObjectList(string key, List<GameObject> list)
		{
			return true;
		}

		public static bool SetTexture2DList(string key, List<Texture2D> list)
		{
			return true;
		}

		public static bool SetMaterialList(string key, List<Material> List)
		{
			return true;
		}

		public static bool SetAudioClipList(string key, List<AudioClip> list)
		{
			return true;
		}

		private static bool SetValue<T>(string key, T array, ListType arrayType, int vectorNumber, Action<T, byte[], int> convert) where T : IList
		{
			byte[] array2 = new byte[4 * array.Count * vectorNumber + 1];
			array2[0] = Convert.ToByte(arrayType);
			Initialize();
			for (int i = 0; i < array.Count; i++)
			{
				convert(array, array2, i);
			}
			return SaveBytes(key, array2);
		}

		public static bool Set2DIntList(string key, List<List<int>> int2DList)
		{
			return Set2DValue(key, int2DList, ListType.Int32_2D, 1, ConvertFromInt);
		}

		public static bool Set2DFloatList(string key, List<List<float>> float2DList)
		{
			return Set2DValue(key, float2DList, ListType.Float_2D, 1, ConvertFromFloat);
		}

		public static bool Set2DVector2List(string key, List<List<Vector2>> vec2_2DList)
		{
			return Set2DValue(key, vec2_2DList, ListType.Vector2_2D, 2, ConvertFromVector2);
		}

		public static bool Set2DVector3List(string key, List<List<Vector3>> vec3_2DList)
		{
			return Set2DValue(key, vec3_2DList, ListType.Vector3_2D, 3, ConvertFromVector3);
		}

		public static bool Set2DVector4List(string key, List<List<Vector4>> vec4_2DList)
		{
			return Set2DValue(key, vec4_2DList, ListType.Vector4_2D, 4, ConvertFromVector4);
		}

		public static bool Set2DGameObjectList(string key, List<List<GameObject>> go_2DList)
		{
			return true;
		}

		public static bool Set2DTexture2DList(string key, List<List<Texture2D>> tex_2DList)
		{
			return true;
		}

		public static bool Set2DMaterialList(string key, List<List<Material>> mat_2DList)
		{
			return true;
		}

		public static bool Set2DAudioClipList(string key, List<List<AudioClip>> aud_2DList)
		{
			return true;
		}

		public static bool Set2DQuaternionList(string key, List<List<Quaternion>> quaternion_2DList)
		{
			return Set2DValue(key, quaternion_2DList, ListType.Quaternion_2D, 4, ConvertFromQuaternion);
		}

		public static bool Set2DColorList(string key, List<List<Color>> color_2DList)
		{
			return Set2DValue(key, color_2DList, ListType.Color_2D, 4, ConvertFromColor);
		}

		private static bool Set2DValue(string key, IList list, ListType arrayType, int vectorNumber, Action<IList, byte[], int> convert)
		{
			List<byte> list2 = new List<byte>();
			list2.Add(Convert.ToByte(arrayType));
			byte[] bytes = BitConverter.GetBytes(list.Count);
			if (!BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes);
			}
			list2.AddRange(bytes);
			foreach (IList item in list)
			{
				bytes = BitConverter.GetBytes(item.Count);
				if (!BitConverter.IsLittleEndian)
				{
					Array.Reverse(bytes);
				}
				list2.AddRange(bytes);
			}
			for (int i = 0; i < list.Count; i++)
			{
				IList list4 = list[i] as IList;
				byte[] array = new byte[4 * list4.Count * vectorNumber];
				Initialize(is2DList: true);
				for (int j = 0; j < list4.Count; j++)
				{
					convert(list4, array, j);
				}
				list2.AddRange(array);
			}
			return SaveBytes(key, list2.ToArray());
		}
	}
}