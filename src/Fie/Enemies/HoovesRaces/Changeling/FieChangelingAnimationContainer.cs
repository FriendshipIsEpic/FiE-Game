using Fie.Object;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	public class FieChangelingAnimationContainer : FieEnemiesHoovesRacesAnimationContainer
	{
		public enum ChangelingAnimationList
		{
			JUMP = 8,
			JUMP_LANDING,
			FALL,
			MELEE,
			SHOOT,
			BACK_STEP,
			VORTEX,
			ARRIVAL,
			EMOTION_NORMAL,
			MAX_CHANGELING_ANIMATION
		}

		public FieChangelingAnimationContainer()
		{
			addAnimationData(8, new FieSkeletonAnimationObject(0, "jump_idle"));
			addAnimationData(9, new FieSkeletonAnimationObject(0, "jump_end_shallow"));
			addAnimationData(10, new FieSkeletonAnimationObject(0, "fall"));
			addAnimationData(11, new FieSkeletonAnimationObject(0, "melee"));
			addAnimationData(12, new FieSkeletonAnimationObject(0, "shoot"));
			addAnimationData(13, new FieSkeletonAnimationObject(0, "backstep"));
			addAnimationData(14, new FieSkeletonAnimationObject(0, "vortex"));
			addAnimationData(15, new FieSkeletonAnimationObject(0, "arrival"));
			addAnimationData(16, new FieSkeletonAnimationObject(1, "emotion_normal"));
		}
	}
}
