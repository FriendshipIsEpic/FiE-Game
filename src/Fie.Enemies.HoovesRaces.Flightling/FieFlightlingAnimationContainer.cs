using Fie.Object;

namespace Fie.Enemies.HoovesRaces.Flightling
{
	public class FieFlightlingAnimationContainer : FieEnemiesHoovesRacesAnimationContainer
	{
		public enum FlightlingAnimationList
		{
			JUMP = 8,
			JUMP_LANDING,
			FALL,
			MELEE,
			SHOOT,
			JUMPING_SHOOT,
			BACK_STEP,
			ARRIVAL,
			EMOTION_NORMAL,
			MAX_FLIGHTLING_ANIMATION
		}

		public FieFlightlingAnimationContainer()
		{
			addAnimationData(8, new FieSkeletonAnimationObject(0, "jump_idle"));
			addAnimationData(9, new FieSkeletonAnimationObject(0, "jump_end_shallow"));
			addAnimationData(10, new FieSkeletonAnimationObject(0, "fall"));
			addAnimationData(11, new FieSkeletonAnimationObject(0, "melee"));
			addAnimationData(12, new FieSkeletonAnimationObject(0, "shoot"));
			addAnimationData(13, new FieSkeletonAnimationObject(0, "jumping_shoot"));
			addAnimationData(14, new FieSkeletonAnimationObject(0, "backstep"));
			addAnimationData(15, new FieSkeletonAnimationObject(0, "arrival"));
			addAnimationData(16, new FieSkeletonAnimationObject(1, "emotion_normal"));
		}
	}
}
