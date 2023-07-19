// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.StringExtensions
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace GameDataEditor
{
	public static class StringExtensions
	{
		public static string HighlightSubstring(this string variable, string substring, string color)
		{
			string result = variable;
			if (!Application.unityVersion.StartsWith("3") && !string.IsNullOrEmpty(substring))
			{
				int num = variable.Replace("Schema:", "       ").IndexOf(substring, StringComparison.CurrentCultureIgnoreCase);
				if (num != -1)
				{
					result = $"{variable.Substring(0, num)}<color={color}>{variable.Substring(num, substring.Length)}</color>{variable.Substring(num + substring.Length)}";
				}
			}
			return result;
		}

		public static string Md5Sum(this string strToEncrypt)
		{
			UTF8Encoding uTF8Encoding = new UTF8Encoding();
			byte[] bytes = uTF8Encoding.GetBytes(strToEncrypt);
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				text += Convert.ToString(array[i], 16).PadLeft(2, '0');
			}
			return text.PadLeft(32, '0');
		}

		public static string UppercaseFirst(this string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			char[] array = s.ToCharArray();
			array[0] = char.ToUpper(array[0]);
			return new string(array);
		}

		public static bool Contains(this string source, string substring, StringComparison comp)
		{
			if (string.IsNullOrEmpty(substring) || string.IsNullOrEmpty(source))
			{
				return true;
			}
			return source.IndexOf(substring, comp) >= 0;
		}

		public static string StripAssetPath(this string source)
		{
			string text = string.Empty;
			string value = "Resources";
			string[] array = source.Split(Path.DirectorySeparatorChar);
			int num = Array.IndexOf(array, value);
			if (num >= 0)
			{
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				text = string.Join(directorySeparatorChar.ToString(), array, num + 1, array.Length - num - 2);
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = text;
					char directorySeparatorChar2 = Path.DirectorySeparatorChar;
					text = text2 + directorySeparatorChar2;
				}
			}
			return text + Path.GetFileNameWithoutExtension(source);
		}
	}
}