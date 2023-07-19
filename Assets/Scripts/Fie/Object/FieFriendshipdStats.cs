using Fie.Manager;
using UnityEngine;

namespace Fie.Object
{
	public class FieFriendshipdStats
	{
		public delegate void FriendshipCalcCallback(float friendship);

		public const int MAXIMUM_FRIENDSHIP_SEGMENT = 6;

		public const float FRIENDSHIP_SEGMENTATION_THRESHOLD = 30f;

		public float friendship;

		public event FriendshipCalcCallback friendshipAddEvent;

		public void safeAddFriendship(float additionalFriendship, bool triggerEvent = true)
		{
			if (additionalFriendship > 0f)
			{
				additionalFriendship *= FieManagerBehaviour<FieEnvironmentManager>.I.currentFriendshipPointMagnify;
			}
			friendship = Mathf.Clamp(friendship + additionalFriendship, 0f, getMaxFriendship());
			if (triggerEvent && this.friendshipAddEvent != null)
			{
				this.friendshipAddEvent(additionalFriendship);
			}
		}

		public void resetFriendship()
		{
			friendship = 0f;
		}

		public float getMaxFriendship()
		{
			return 180f;
		}

		public void addFriendshipPoint(int num)
		{
			safeAddFriendship((float)num * 30f);
		}

		public int getCurrentFriendshipPoint()
		{
			return (int)(friendship / 30f);
		}
	}
}
