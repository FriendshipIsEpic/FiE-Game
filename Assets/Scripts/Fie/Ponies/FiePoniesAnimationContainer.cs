using Fie.Object;

namespace Fie.Ponies
{
	public class FiePoniesAnimationContainer : FieAnimationContainerBase
	{
		public enum PoniesAnimTrack
		{
			HORN = 2,
			MAX_PONIES_TRACK
		}

		public enum PoniesAnimList
		{
			IDLE_RELAX = 1,
			WALK,
			GALLOP,
			LANDING,
			LANDING_SMOOTH,
			STAGGER,
			STAGGER_AIR,
			STAGGER_FALL,
			STAGGER_FALL_RECOVER,
			ARRIVAL,
			DEAD,
			DEAD_LOOP,
			DEAD_RECOVER,
			GRABBED,
			EMOTION_NORMAL,
			EMOTION_SERIOUS,
			EMOTION_DAMAGE,
			EMOTION_DEAD,
			MAX_PONIES_ANIMATION
		}

		public FiePoniesAnimationContainer()
		{
			addAnimationData(0, new FieSkeletonAnimationObject(0, "idle"));
			addAnimationData(1, new FieSkeletonAnimationObject(0, "idle_relax"));
			addAnimationData(2, new FieSkeletonAnimationObject(0, "walk"));
			addAnimationData(3, new FieSkeletonAnimationObject(0, "gallop"));
			addAnimationData(4, new FieSkeletonAnimationObject(0, "landing"));
			addAnimationData(5, new FieSkeletonAnimationObject(0, "landing_smooth"));
			addAnimationData(6, new FieSkeletonAnimationObject(0, "stagger"));
			addAnimationData(7, new FieSkeletonAnimationObject(0, "stagger_air"));
			addAnimationData(8, new FieSkeletonAnimationObject(0, "stagger_fall"));
			addAnimationData(9, new FieSkeletonAnimationObject(0, "stagger_fall_recover"));
			addAnimationData(10, new FieSkeletonAnimationObject(0, "arrival"));
			addAnimationData(11, new FieSkeletonAnimationObject(0, "dead"));
			addAnimationData(12, new FieSkeletonAnimationObject(0, "dead_loop"));
			addAnimationData(13, new FieSkeletonAnimationObject(0, "dead_recover"));
			addAnimationData(14, new FieSkeletonAnimationObject(0, "grabbed"));
			addAnimationData(15, new FieSkeletonAnimationObject(1, "emotion_normal"));
			addAnimationData(16, new FieSkeletonAnimationObject(1, "emotion_serious"));
			addAnimationData(17, new FieSkeletonAnimationObject(1, "emotion_damage"));
			addAnimationData(18, new FieSkeletonAnimationObject(1, "emotion_dead"));
		}
	}
}
