#if UNITY_WSA && !UNITY_EDITOR
#else
using UnityEngine;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

namespace ParticlePlayground {
	public class PlaygroundCompression 
	{
		public static byte[] SerializeAndCompress(object obj) 
		{
			using (MemoryStream ms = new MemoryStream()) 
			{
				using (GZipStream zs = new GZipStream(ms, CompressionMode.Compress, true))
				{
					BinaryFormatter bf = new BinaryFormatter();
					bf.Serialize(zs, obj);
				}
				
				return ms.ToArray();
			}
		}
		
		public static T DecompressAndDeserialize<T>(byte[] data)
		{
			using (MemoryStream ms = new MemoryStream(data)) 
			{
				using (GZipStream zs = new GZipStream(ms, CompressionMode.Decompress, true))
				{
					BinaryFormatter bf = new BinaryFormatter();
					return (T) bf.Deserialize(zs);
				}
			}
		}
	}
}
#endif
