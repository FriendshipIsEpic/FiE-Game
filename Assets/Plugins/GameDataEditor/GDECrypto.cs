// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDECrypto
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace GameDataEditor
{
	[Serializable]
	public class GDECrypto
	{
		public const int KEY_LENGTH = 256;

		public byte[] Salt;

		public byte[] IV;

		public string Pass;

		public string Decrypt(byte[] cipherTextBytes)
		{
			string result = string.Empty;
			try
			{
				byte[] bytes = new Rfc2898DeriveBytes(Pass, Salt).GetBytes(32);
				byte[] array = new byte[cipherTextBytes.Length];
				using RijndaelManaged rijndaelManaged = new RijndaelManaged();
				rijndaelManaged.Mode = CipherMode.CBC;
				using ICryptoTransform transform = rijndaelManaged.CreateDecryptor(bytes, IV);
				using MemoryStream stream = new MemoryStream(cipherTextBytes);
				using CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
				int count = cryptoStream.Read(array, 0, array.Length);
				result = Encoding.UTF8.GetString(array, 0, count);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			return result;
		}
	}
}