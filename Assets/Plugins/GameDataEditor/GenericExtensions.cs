// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GenericExtensions
using System;
using System.Collections.Generic;
using System.Reflection;
using GameDataEditor;
using UnityEngine;

namespace GameDataEditor
{
	public static class GenericExtensions
	{
		public static bool IsCloneableType<T>(this T variable)
		{
			return typeof(ICloneable).IsAssignableFrom(variable.GetType());
		}

		public static bool IsGenericList<T>(this T variable)
		{
			Type[] interfaces = variable.GetType().GetInterfaces();
			foreach (Type type in interfaces)
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsGenericDictionary<T>(this T variable)
		{
			Type[] interfaces = variable.GetType().GetInterfaces();
			foreach (Type type in interfaces)
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<, >))
				{
					return true;
				}
			}
			return false;
		}

		public static bool DerivesFromUnityObject<T>(this T variable)
		{
			return typeof(UnityEngine.Object).IsAssignableFrom(variable.GetType());
		}

		public static T DeepCopyCollection<T>(this T variable)
		{
			if (variable == null)
			{
				return variable;
			}
			T result = variable;
			if (variable.IsGenericDictionary())
			{
				Type[] genericArguments = variable.GetType().GetGenericArguments();
				Type type = genericArguments[0];
				Type type2 = genericArguments[1];
				MethodInfo methodInfo = DictionaryExtensions.DeepCopyMethodInfo.MakeGenericMethod(type, type2);
				result = (T)methodInfo.Invoke(variable, new object[1] { variable });
			}
			else if (variable.IsGenericList())
			{
				Type type3 = variable.GetType().GetGenericArguments()[0];
				MethodInfo methodInfo2 = ListExtensions.DeepCopyMethodInfo.MakeGenericMethod(type3);
				result = (T)methodInfo2.Invoke(variable, new object[1] { variable });
			}
			return result;
		}
	}
}