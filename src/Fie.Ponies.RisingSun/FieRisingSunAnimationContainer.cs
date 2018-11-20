using Fie.Object;

namespace Fie.Ponies.RisingSun
{
	public class FieRisingSunAnimationContainer : FiePoniesAnimationContainer
	{
		public enum RisingSunAnimationList
		{
			JUMP_TAKEOFF = 19,
			JUMP,
			TELEPORTATION_AIR,
			TELEPORTATION_GROUND,
			FIRE_SMALL,
			FIRE_SMALL2,
			FIRE_LARGE,
			FIRE_LARGE_IDLE,
			FIRE_LARGE_END,
			EMOTION_FIRE_LARGE,
			HORN_SHINE,
			MAX_RISING_SUN_ANIMATION
		}

		public FieRisingSunAnimationContainer()
		{
			addAnimationData(19, new FieSkeletonAnimationObject(0, "jump"));
			addAnimationData(20, new FieSkeletonAnimationObject(0, "jump_idle"));
			addAnimationData(21, new FieSkeletonAnimationObject(0, "teleportation_air"));
			addAnimationData(22, new FieSkeletonAnimationObject(0, "teleportation_ground"));
			addAnimationData(23, new FieSkeletonAnimationObject(0, "fire_small"));
			addAnimationData(25, new FieSkeletonAnimationObject(0, "fire_large"));
			addAnimationData(26, new FieSkeletonAnimationObject(0, "fire_large_idle"));
			addAnimationData(27, new FieSkeletonAnimationObject(0, "fire_large_end"));
			addAnimationData(28, new FieSkeletonAnimationObject(1, "emotion_fire_large"));
			addAnimationData(29, new FieSkeletonAnimationObject(2, "horn_shine"));
		}
	}
}
