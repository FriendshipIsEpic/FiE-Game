using Fie.Ponies;
using Fie.Ponies.RainbowDash;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskRainbowDashEnemyEvade : FieAITaskBase
	{
		public override bool Task(FieAITaskController manager)
		{
			if (manager.ownerCharacter.detector.lockonTargetObject == null)
			{
				return true;
			}
			float num = Vector3.Distance(manager.ownerCharacter.transform.position, manager.ownerCharacter.detector.lockonTargetObject.centerTransform.position);
			if (!(num < 3f))
			{
				if (manager.ownerCharacter.abilitiesContainer.GetCooltime<FieStateMachineRainbowDashDoublePayback>() <= 0f)
				{
					nextStateWeightList[typeof(FieAITaskRainbowDashDoublePayback)] = 100;
					return true;
				}
				if (manager.ownerCharacter.abilitiesContainer.GetCooltime<FieStateMachineRainbowDashRainblow>() <= 0f)
				{
					nextStateWeightList[typeof(FieAITaskRainbowDashRainblow)] = 100;
					return true;
				}
				return true;
			}
			Vector3 vector = manager.ownerCharacter.detector.lockonTargetObject.centerTransform.position - manager.ownerCharacter.transform.position;
			vector.y = vector.z;
			vector.z = 0f;
			manager.ownerCharacter.RequestToChangeState<FieStateMachinePoniesGallop>(-vector.normalized, 1f, FieGameCharacter.StateMachineType.Base);
			return false;
		}
	}
}
