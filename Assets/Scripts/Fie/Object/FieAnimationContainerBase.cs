using System.Collections.Generic;

namespace Fie.Object {

    public abstract class FieAnimationContainerBase {
        public enum BaseAnimTrack {
            MOTION,
            EMOTION,
            MAX_BASE_TRACK
        }

        public enum BaseAnimList {
            IDLE,
            MAX_BASE_ANIMATION
        }

        private Dictionary<int, FieSkeletonAnimationObject> animationList = new Dictionary<int, FieSkeletonAnimationObject>();

        /// <summary>
        /// Gets the list of all animations registered to this container.
        /// </summary>
        public Dictionary<int, FieSkeletonAnimationObject> getAnimationList() {
            return animationList;
        }

        /// <summary>
        /// Gets the skeleton animation for the provided animation id.
        /// Returns null of one such animation exists (????)
        /// </summary>
		public FieSkeletonAnimationObject getAnimation(int animationId) {
            // TODO: Shouldn't this be inverted?
            if (animationList.ContainsKey(animationId)) {
                return null;
            }
            return animationList[animationId];
        }

        /// <summary>
        /// Registers animation details for the given id to this container.
        /// </summary>
        public void addAnimationData(int animationID, FieSkeletonAnimationObject animationObject) {
            animationList.Add(animationID, animationObject);
        }
    }
}
