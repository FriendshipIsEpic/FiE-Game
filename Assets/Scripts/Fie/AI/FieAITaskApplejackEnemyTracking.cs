using Fie.Ponies;
using Fie.Ponies.Applejack;
using UnityEngine;

namespace Fie.AI {
    public class FieAITaskApplejackEnemyTracking : FieAITaskBase {
        public override bool Task(FieAITaskController manager) {
            if (manager.ownerCharacter.detector.lockonTargetObject == null) {
                return true;
            }

            float num = Vector3.Distance(manager.ownerCharacter.transform.position, manager.ownerCharacter.detector.lockonTargetObject.centerTransform.position);

            if (!(num > 2f)) {
                Vector3 position = manager.ownerCharacter.detector.lockonTargetObject.centerTransform.position;

                float y = position.y;

                Vector3 position2 = manager.ownerCharacter.transform.position;

                if (y > position2.y + 5f && manager.ownerCharacter.abilitiesContainer.GetCooltime<FieStateMachineApplejackRope>() <= 0f) {
                    nextStateWeightList[typeof(FieStateMachineApplejackRope)] = 100;

                    return true;
                }

                return true;
            }

            Vector3 vector = manager.ownerCharacter.detector.lockonTargetObject.centerTransform.position - manager.ownerCharacter.transform.position;
            vector.y = vector.z;
            vector.z = 0f;
            manager.ownerCharacter.RequestToChangeState<FieStateMachinePoniesGallop>(vector.normalized, 1f, FieGameCharacter.StateMachineType.Base);

            return false;
        }
    }
}
