using Fie.Ponies;
using Fie.Ponies.Twilight;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskTwilightEnemyEvade : FieAITaskTwilightBase
	{
		public override bool Task(FieAITaskController manager)
		{
			if (manager.ownerCharacter.detector.lockonTargetObject == null)
			{
				return true;
			}
			float num = Vector3.Distance(manager.ownerCharacter.transform.position, manager.ownerCharacter.detector.lockonTargetObject.centerTransform.position);
			FieAITaskController.FieAIFrontAndBackPoint frontAndBackPoint = manager.GetFrontAndBackPoint(3f);
			bool flag = false;
			Vector3 vector = manager.ownerCharacter.detector.lockonTargetObject.centerTransform.position - manager.ownerCharacter.transform.position;
			vector.y = vector.z;
			vector.z = 0f;
			if (num < 1.5f)
			{
				if (Random.Range(0, 100) > 50)
				{
					flag = ((byte)((flag ? 1 : 0) | 1) != 0);
				}
				if (frontAndBackPoint.backDistance > 0f)
				{
					flag = ((byte)((flag ? 1 : 0) | 1) != 0);
				}
				manager.ownerCharacter.RequestToChangeState<FieStateMachineTwilightTeleportation>((!flag) ? (-vector) : vector, 1f, FieGameCharacter.StateMachineType.Base);
			}
			else if (num < 3f)
			{
				if (frontAndBackPoint.backDistance > 0f)
				{
					flag = ((byte)((flag ? 1 : 0) | 1) != 0);
				}
				manager.ownerCharacter.RequestToChangeState<FieStateMachinePoniesGallop>((!flag) ? (-vector) : vector, 1f, FieGameCharacter.StateMachineType.Base);
			}
			return true;
		}
	}
}
