// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.ListExtensions
using System;
using System.Collections.Generic;
using System.Reflection;
using GameDataEditor;

namespace GameDataEditor
{
	public static class ListExtensions
	{
		public static MethodInfo DeepCopyMethodInfo = typeof(ListExtensions).GetMethod("DeepCopy");

		public static bool IsValidIndex<T>(this List<T> variable, int index)
		{
			return index > -1 && variable != null && index < variable.Count;
		}

		public static List<T> DeepCopy<T>(this List<T> variable)
		{
			List<T> list = new List<T>();
			T val = default(T);
			foreach (T item in variable)
			{
				if (item == null)
				{
					val = item;
				}
				else if (item.IsCloneableType())
				{
					val = (T)((ICloneable)(object)item).Clone();
				}
				else if (item.IsGenericList())
				{
					Type type = item.GetType().GetGenericArguments()[0];
					MethodInfo methodInfo = DeepCopyMethodInfo.MakeGenericMethod(type);
					val = (T)methodInfo.Invoke(item, new object[1] { item });
				}
				else if (item.IsGenericDictionary())
				{
					Type[] genericArguments = item.GetType().GetGenericArguments();
					Type type2 = genericArguments[0];
					Type type3 = genericArguments[1];
					MethodInfo methodInfo2 = DictionaryExtensions.DeepCopyMethodInfo.MakeGenericMethod(type2, type3);
					val = (T)methodInfo2.Invoke(item, new object[1] { item });
				}
				else
				{
					val = item;
				}
				list.Add(val);
			}
			return list;
		}

		public static List<int> AllIndexesOf<T>(this List<T> variable, T searchValue)
		{
			List<int> list = new List<int>();
			int num;
			for (num = 0; num <= variable.Count; num++)
			{
				num = variable.IndexOf(searchValue, num);
				if (num == -1)
				{
					break;
				}
				list.Add(num);
			}
			return list;
		}
	}
}