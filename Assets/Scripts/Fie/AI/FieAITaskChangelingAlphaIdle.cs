using Fie.Enemies.HoovesRaces;
using Fie.Object;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskChangelingAlphaIdle : FieAITaskBase
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
			FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
			if (currentStateMachine is FieStateMachineEnemiesHoovesRacesStagger)
			{
				nextStateWeightList[typeof(FieAITaskChangelingBackstep)] = 100;
				return true;
			}
			nextStateWeightList[typeof(FieAITaskChangelingAlphaShout)] = (int)Mathf.Max(2000f * manager.ownerCharacter.healthStats.nowHelthAndShieldRatePerMax, 1000f);
			nextStateWeightList[typeof(FieAITaskFlightlingAlphaShot)] = (int)Mathf.Max(1000f * (1f - manager.ownerCharacter.healthStats.nowHelthAndShieldRatePerMax), 400f);
			return true;
		}
	}
}
