using UnityEngine;

namespace Fie.Utility
{
	public class FieRandom
	{
		private static void InitRandomSeed()
		{
			if (PhotonNetwork.room != null)
			{
				Random.seed = (PhotonNetwork.room.name + PhotonNetwork.time.ToString()).GetHashCode();
			}
			else
			{
				Random.seed = (int)Time.time;
			}
		}

		public static int Range(int a, int b)
		{
			InitRandomSeed();
			return Random.Range(a, b);
		}

		public static float Range(float a, float b)
		{
			InitRandomSeed();
			return Random.Range(a, b);
		}
	}
}
