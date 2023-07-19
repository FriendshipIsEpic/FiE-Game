using Fie.Object;

namespace Fie.Ponies.RainbowDash
{
	public class FieRainbowDashAnimationContainer : FiePoniesAnimationContainer
	{
		public enum RainbowDashAnimationList
		{
			JUMP_TAKEOFF = 19,
			JUMP,
			JUMP_LANDING,
			EVASION_FRONT,
			EVASION_BACK,
			EVASION_UP,
			EVASION_DOWN,
			BASE_ATTACK_1,
			BASE_ATTACK_2,
			BASE_ATTACK_3,
			DOUBLE_PAYBACK_PREPARE_ON_GROUND,
			DOUBLE_PAYBACK_PREPARE_ON_GROUND_SHORT_1,
			DOUBLE_PAYBACK_PREPARE_ON_GROUND_SHORT_2,
			DOUBLE_PAYBACK_PREPARE_ON_GROUND_END,
			DOUBLE_PAYBACK_PREPARE_MID_AIR,
			DOUBLE_PAYBACK_PREPARE_MID_AIR_SHORT_1,
			DOUBLE_PAYBACK_PREPARE_MID_AIR_SHORT_2,
			DOUBLE_PAYBACK_PREPARE_MID_AIR_END,
			DOUBLE_PAYBACK_LOOP,
			DOUBLE_PAYBACK_END,
			OMNI_SMASH_START,
			OMNI_SMASH_FINISHING_START,
			OMNI_SMASH_FINISHING_END,
			OMNI_SMASH_FINISHING_TO_IDLE,
			OMNI_SMASH_FINISHING_TO_IDLE_SHORT_1,
			OMNI_SMASH_WAITING,
			CLOAK,
			MAX_APPLEJACK_ANIMATION
		}

		public FieRainbowDashAnimationContainer()
		{
			addAnimationData(19, new FieSkeletonAnimationObject(0, "jump"));
			addAnimationData(20, new FieSkeletonAnimationObject(0, "jump_idle"));
			addAnimationData(22, new FieSkeletonAnimationObject(0, "evasion_front"));
			addAnimationData(23, new FieSkeletonAnimationObject(0, "evasion_back"));
			addAnimationData(24, new FieSkeletonAnimationObject(0, "evasion_up"));
			addAnimationData(25, new FieSkeletonAnimationObject(0, "evasion_down"));
			addAnimationData(26, new FieSkeletonAnimationObject(0, "base_attack_1"));
			addAnimationData(27, new FieSkeletonAnimationObject(0, "base_attack_2"));
			addAnimationData(28, new FieSkeletonAnimationObject(0, "base_attack_3"));
			addAnimationData(29, new FieSkeletonAnimationObject(0, "counter_strike_prepare_on_ground"));
			addAnimationData(30, new FieSkeletonAnimationObject(0, "counter_strike_prepare_on_ground_short_1"));
			addAnimationData(31, new FieSkeletonAnimationObject(0, "counter_strike_prepare_on_ground_short_2"));
			addAnimationData(32, new FieSkeletonAnimationObject(0, "counter_strike_prepare_on_ground_end"));
			addAnimationData(33, new FieSkeletonAnimationObject(0, "counter_strike_prepare_mid_air"));
			addAnimationData(34, new FieSkeletonAnimationObject(0, "counter_strike_prepare_mid_short_1"));
			addAnimationData(35, new FieSkeletonAnimationObject(0, "counter_strike_prepare_mid_short_2"));
			addAnimationData(36, new FieSkeletonAnimationObject(0, "counter_strike_prepare_mid_air_end"));
			addAnimationData(37, new FieSkeletonAnimationObject(0, "counter_strike_loop"));
			addAnimationData(38, new FieSkeletonAnimationObject(0, "counter_strike_end"));
			addAnimationData(39, new FieSkeletonAnimationObject(0, "omnismash_start"));
			addAnimationData(40, new FieSkeletonAnimationObject(0, "omnismash_finishing_start"));
			addAnimationData(41, new FieSkeletonAnimationObject(0, "omnismash_finishing_end"));
			addAnimationData(42, new FieSkeletonAnimationObject(0, "omnismash_finishing_to_idle"));
			addAnimationData(43, new FieSkeletonAnimationObject(0, "omnismash_finishing_to_idle_short_1"));
			addAnimationData(44, new FieSkeletonAnimationObject(0, "omnismash_finishing_waiting"));
			addAnimationData(45, new FieSkeletonAnimationObject(0, "cloak"));
		}
	}
}
