using Fie.Ponies;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskRainbowDashEnemyTracking : FieAITaskBase
	{
		public override bool Task(FieAITaskController manager)
		{
			if (manager.ownerCharacter.detector.lockonTargetObject == null)
			{
				return true;
			}
			float num = Vector3.Distance(manager.ownerCharacter.transform.position, manager.ownerCharacter.detector.lockonTargetObject.centerTransform.position);
			if (!(num > 2f))
			{
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
