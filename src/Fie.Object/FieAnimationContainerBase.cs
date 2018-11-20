using System.Collections.Generic;

namespace Fie.Object
{
	public abstract class FieAnimationContainerBase
	{
		public enum BaseAnimTrack
		{
			MOTION,
			EMOTION,
			MAX_BASE_TRACK
		}

		public enum BaseAnimList
		{
			IDLE,
			MAX_BASE_ANIMATION
		}

		private Dictionary<int, FieSkeletonAnimationObject> animationList = new Dictionary<int, FieSkeletonAnimationObject>();

		public Dictionary<int, FieSkeletonAnimationObject> getAnimationList()
		{
			return animationList;
		}

		public FieSkeletonAnimationObject getAnimation(int animationId)
		{
			if (animationList.ContainsKey(animationId))
			{
				return null;
			}
			return animationList[animationId];
		}

		public void addAnimationData(int animationID, FieSkeletonAnimationObject animationObject)
		{
			animationList.Add(animationID, animationObject);
		}
	}
}
