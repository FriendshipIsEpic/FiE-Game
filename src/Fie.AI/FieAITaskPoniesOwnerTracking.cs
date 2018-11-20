using Fie.Manager;
using Fie.Object;
using Fie.Ponies;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskPoniesOwnerTracking : FieAITaskBase
	{
		public override bool Task(FieAITaskController manager)
		{
			if (FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter == null)
			{
				return true;
			}
			float num = Vector3.Distance(manager.ownerCharacter.transform.position, FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter.transform.position);
			if (!(num > 2.5f))
			{
				manager.ownerCharacter.RequestToChangeState<FieStateMachinePoniesIdle>(Vector3.zero, 0f, FieGameCharacter.StateMachineType.Base);
				return true;
			}
			Vector3 vector = FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter.transform.position - manager.ownerCharacter.transform.position;
			vector.y = vector.z;
			vector.z = 0f;
			manager.ownerCharacter.RequestToChangeState<FieStateMachinePoniesGallop>(vector.normalized, 1f, FieGameCharacter.StateMachineType.Base);
			return false;
		}
	}
}
