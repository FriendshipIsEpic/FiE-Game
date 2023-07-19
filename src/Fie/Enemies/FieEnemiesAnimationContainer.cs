using Fie.Object;

namespace Fie.Enemies {
    public class FieEnemiesAnimationContainer : FieAnimationContainerBase {
        public FieEnemiesAnimationContainer() {
            addAnimationData(0, new FieSkeletonAnimationObject(0, "idle"));
        }
    }
}
