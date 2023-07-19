using Fie.Object;

namespace Fie.Ponies.Applejack
{
	public class FieApplejackAnimationContainer : FiePoniesAnimationContainer
	{
		public enum ApplejackAnimationList
		{
			JUMP_TAKEOFF = 19,
			JUMP,
			STEP,
			FIRE_PUNCH,
			FIRE_KICK,
			FIRE_KICK_HEAVY,
			FIRE_KICK_RIFT,
			FIRE_KICK_RIFT_HEAVY,
			FIRE_AIR,
			FIRE_AIR_HEAVY,
			FIRE_AIR_DOUBLE,
			FIRE_AIR_DOUBLE_HEAVY,
			FIRE_ROPE,
			FIRE_ROPE_AIR,
			ROPE_ACTION_AIR_RAID,
			ROPE_ACTION_AIR_RAID_END,
			ROPE_ACTION_END,
			BACK_STEP,
			YEEHAW,
			YEEHAW_MID_AIR,
			STOMP,
			CHARGE,
			EMOTION_ROPE,
			EMOTION_NIHIL,
			EMOTION_YEEHAW,
			MAX_APPLEJACK_ANIMATION
		}

		public FieApplejackAnimationContainer()
		{
			addAnimationData(19, new FieSkeletonAnimationObject(0, "jump"));
			addAnimationData(20, new FieSkeletonAnimationObject(0, "fall"));
			addAnimationData(22, new FieSkeletonAnimationObject(0, "fire_melee_punch"));
			addAnimationData(23, new FieSkeletonAnimationObject(0, "fire_melee_kick"));
			addAnimationData(24, new FieSkeletonAnimationObject(0, "fire_melee_kick_slow_1"));
			addAnimationData(25, new FieSkeletonAnimationObject(0, "fire_melee_kick_rift"));
			addAnimationData(26, new FieSkeletonAnimationObject(0, "fire_melee_kick_rift_slow_1"));
			addAnimationData(27, new FieSkeletonAnimationObject(0, "fire_melee_air"));
			addAnimationData(28, new FieSkeletonAnimationObject(0, "fire_melee_air_slow_1"));
			addAnimationData(29, new FieSkeletonAnimationObject(0, "fire_melee_air_double"));
			addAnimationData(30, new FieSkeletonAnimationObject(0, "fire_melee_air_double_slow_1"));
			addAnimationData(31, new FieSkeletonAnimationObject(0, "fire_rope"));
			addAnimationData(32, new FieSkeletonAnimationObject(0, "fire_rope_air"));
			addAnimationData(33, new FieSkeletonAnimationObject(0, "fire_rope_airraid"));
			addAnimationData(34, new FieSkeletonAnimationObject(0, "fire_rope_airraid_end"));
			addAnimationData(35, new FieSkeletonAnimationObject(0, "fire_rope_end"));
			addAnimationData(36, new FieSkeletonAnimationObject(0, "backstep"));
			addAnimationData(40, new FieSkeletonAnimationObject(0, "charge"));
			addAnimationData(37, new FieSkeletonAnimationObject(0, "yeehaw"));
			addAnimationData(38, new FieSkeletonAnimationObject(0, "yeehaw_mid_air"));
			addAnimationData(39, new FieSkeletonAnimationObject(0, "stomp"));
			addAnimationData(41, new FieSkeletonAnimationObject(1, "emotion_rope"));
			addAnimationData(42, new FieSkeletonAnimationObject(1, "emotion_nihil"));
			addAnimationData(43, new FieSkeletonAnimationObject(1, "emotion_yeehaw"));
		}
	}
}
