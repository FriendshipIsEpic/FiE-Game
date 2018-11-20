using Fie.Object;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	public class FieChangelingAlphaAnimationContainer : FieEnemiesHoovesRacesAnimationContainer
	{
		public enum ChangelingAlphaAnimationList
		{
			SHOOT = 8,
			SHOOT_CONCENTRATION,
			MELEE,
			BACK_STEP,
			SHOUT,
			CHARGE,
			CHARGE_FINISH,
			ZERO_DISTANCE_CHARGE,
			ARRIVAL,
			EMOTION_NORMAL,
			MAX_CHANGELING_ALPHA_ANIMATION
		}

		public FieChangelingAlphaAnimationContainer()
		{
			addAnimationData(8, new FieSkeletonAnimationObject(0, "shoot"));
			addAnimationData(9, new FieSkeletonAnimationObject(0, "shoot_concentration"));
			addAnimationData(10, new FieSkeletonAnimationObject(0, "melee"));
			addAnimationData(11, new FieSkeletonAnimationObject(0, "backstep"));
			addAnimationData(12, new FieSkeletonAnimationObject(0, "shout"));
			addAnimationData(13, new FieSkeletonAnimationObject(0, "charge"));
			addAnimationData(14, new FieSkeletonAnimationObject(0, "charge_finish"));
			addAnimationData(15, new FieSkeletonAnimationObject(0, "zero_distance_charge"));
			addAnimationData(16, new FieSkeletonAnimationObject(0, "arrival"));
			addAnimationData(17, new FieSkeletonAnimationObject(1, "emotion_normal"));
		}
	}
}
