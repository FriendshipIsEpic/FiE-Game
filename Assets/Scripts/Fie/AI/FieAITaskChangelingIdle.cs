using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskChangelingIdle : FieAITaskBase
	{
		public override bool Task(FieAITaskController manager)
		{
			if (manager.ownerCharacter.detector.lockonTargetObject == null)
			{
				return false;
			}
			FieAITaskController.FieAIFrontAndBackPoint frontAndBackPoint = manager.GetFrontAndBackPoint();
			if (frontAndBackPoint.backDistance > 0f)
			{
				nextStateWeightList[typeof(FieAITaskEnemiesHoovesRacesEvadeBackWall)] = 100;
				return true;
			}
			nextStateWeightList[typeof(FieAITaskChangelingMelee)] = (int)Mathf.Max(1000f * manager.ownerCharacter.healthStats.nowHelthAndShieldRatePerMax, 400f);
			nextStateWeightList[typeof(FieAITaskChangelingShoot)] = (int)Mathf.Max(1000f * (1f - manager.ownerCharacter.healthStats.nowHelthAndShieldRatePerMax), 400f);
			return true;
		}
	}
}
