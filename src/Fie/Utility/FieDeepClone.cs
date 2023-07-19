using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Fie.Utility
{
	public static class FieDeepClone
	{
		public static T CloneDeep<T>(this T target)
		{
			object obj = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, target);
				memoryStream.Position = 0L;
				obj = binaryFormatter.Deserialize(memoryStream);
			}
			return (T)obj;
		}
	}
}
