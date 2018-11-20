using Fie.Manager;
using Fie.Ponies;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskPoniesRescue : FieAITaskBase
	{
		private FieGameCharacter injuryCharacter;

		public override void Initialize(FieAITaskController manager)
		{
			injuryCharacter = (injuryCharacter = FieManagerBehaviour<FieInGameCharacterStatusManager>.I.GetNearbyInjuryAllyCharacter(manager.ownerCharacter));
		}

		public override bool Task(FieAITaskController manager)
		{
			if (injuryCharacter == null)
			{
				return true;
			}
			float num = Vector3.Distance(manager.ownerCharacter.transform.position, injuryCharacter.transform.position);
			if (!(num > 2.5f))
			{
				(manager.ownerCharacter as FiePonies).TryToRevive();
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
