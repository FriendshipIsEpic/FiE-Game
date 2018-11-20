using Fie.Object;

namespace Fie.Enemies
{
	public class FieEnemiesHoovesRacesAnimationContainer : FieEnemiesAnimationContainer
	{
		public enum HoovesRacesAnimTrack
		{
			HORN = 2,
			MAX_HOOVES_RACES_TRACK
		}

		public enum HoovesRacesAnimList
		{
			WALK = 1,
			GALLOP,
			STAGGER,
			STAGGER_AIR,
			STAGGER_FALL,
			STAGGER_FALL_RECOVER,
			DEAD,
			MAX_HOOVES_RACES_ANIMATION
		}

		public FieEnemiesHoovesRacesAnimationContainer()
		{
			addAnimationData(1, new FieSkeletonAnimationObject(0, "walk"));
			addAnimationData(2, new FieSkeletonAnimationObject(0, "gallop"));
			addAnimationData(3, new FieSkeletonAnimationObject(0, "stagger"));
			addAnimationData(4, new FieSkeletonAnimationObject(0, "stagger_air"));
			addAnimationData(5, new FieSkeletonAnimationObject(0, "stagger_fall"));
			addAnimationData(6, new FieSkeletonAnimationObject(0, "stagger_fall_recover"));
			addAnimationData(7, new FieSkeletonAnimationObject(0, "dead"));
		}
	}
}
