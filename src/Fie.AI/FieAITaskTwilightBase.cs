using Fie.Ponies;
using UnityEngine;

namespace Fie.AI
{
	public abstract class FieAITaskTwilightBase : FieAITaskBase
	{
		protected bool AdjustDirectionByBasicMovement(FieAITaskController manager)
		{
			if (manager.ownerCharacter.detector.lockonTargetObject.flipDirectionVector != manager.ownerCharacter.flipDirectionVector)
			{
				return false;
			}
			Vector3 vector = manager.ownerCharacter.detector.lockonTargetObject.position - manager.ownerCharacter.transform.position;
			vector.y = vector.z;
			vector.z = 0f;
			manager.ownerCharacter.RequestToChangeState<FieStateMachinePoniesGallop>(vector.normalized, 1f, FieGameCharacter.StateMachineType.Base);
			return true;
		}
	}
}
