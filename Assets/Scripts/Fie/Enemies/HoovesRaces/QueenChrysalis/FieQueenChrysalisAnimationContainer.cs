using Fie.Object;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieQueenChrysalisAnimationContainer : FieEnemiesHoovesRacesAnimationContainer
	{
		public enum QueenChrysalisAnimationList
		{
			BACK_STEP = 8,
			AIRRAID_ATTACKING,
			AIRRAID_FINISHMOVE,
			AIRRAID_PREPARE,
			AIRRAID_TO_IDLE,
			CALL_MINIONS_MIDAIR,
			CALL_MINIONS_ONGROUND,
			CRUCIBLE_FLYBY,
			CRUCIBLE_KICK,
			DOUBLE_SLASH,
			DOUBLE_SLASH_MIDAIR,
			GRAB_BURN,
			GRAB_FAILED,
			GRAB_PREBURN,
			GRAB_START,
			JUMP_IDLE,
			JUMP_END,
			METEOR_SHOWER_IDLE,
			METEOR_SHOWER_PREPARE,
			METEOR_SHOWER_TO_IDLE,
			SHOOT_HORMING,
			SHOOT_PENETRATION,
			EMOTION_NORMAL,
			EMOTION_EYE_CLOSE,
			MAX_CHANGELING_ALPHA_ANIMATION
		}

		public FieQueenChrysalisAnimationContainer()
		{
			addAnimationData(8, new FieSkeletonAnimationObject(0, "backstep"));
			addAnimationData(9, new FieSkeletonAnimationObject(0, "airraid_attacking"));
			addAnimationData(10, new FieSkeletonAnimationObject(0, "airraid_finishmove"));
			addAnimationData(11, new FieSkeletonAnimationObject(0, "airraid_prepare"));
			addAnimationData(12, new FieSkeletonAnimationObject(0, "airraid_to_idle"));
			addAnimationData(13, new FieSkeletonAnimationObject(0, "call_minions_midair"));
			addAnimationData(14, new FieSkeletonAnimationObject(0, "call_minions_onground"));
			addAnimationData(15, new FieSkeletonAnimationObject(0, "crucible_flyby"));
			addAnimationData(16, new FieSkeletonAnimationObject(0, "crucible_kick"));
			addAnimationData(17, new FieSkeletonAnimationObject(0, "double_slash"));
			addAnimationData(18, new FieSkeletonAnimationObject(0, "double_slash_midair"));
			addAnimationData(19, new FieSkeletonAnimationObject(0, "grab_burn"));
			addAnimationData(20, new FieSkeletonAnimationObject(0, "grab_failed"));
			addAnimationData(21, new FieSkeletonAnimationObject(0, "grab_preburn"));
			addAnimationData(22, new FieSkeletonAnimationObject(0, "grab_start"));
			addAnimationData(23, new FieSkeletonAnimationObject(0, "jump_idle"));
			addAnimationData(24, new FieSkeletonAnimationObject(0, "jump_end_shallow"));
			addAnimationData(25, new FieSkeletonAnimationObject(0, "meteor_shower_idle"));
			addAnimationData(26, new FieSkeletonAnimationObject(0, "meteor_shower_prepare"));
			addAnimationData(27, new FieSkeletonAnimationObject(0, "meteor_shower_to_idle"));
			addAnimationData(28, new FieSkeletonAnimationObject(0, "shoot_horming"));
			addAnimationData(29, new FieSkeletonAnimationObject(0, "shoot_penetration"));
			addAnimationData(30, new FieSkeletonAnimationObject(1, "emotion_eye_open"));
			addAnimationData(31, new FieSkeletonAnimationObject(1, "emotion_eye_close"));
		}
	}
}
