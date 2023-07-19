using Fie.Enemies.HoovesRaces;
using Fie.Object;

namespace Fie.AI
{
	public class FieAITaskFlightlingIdle : FieAITaskBase
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
			nextStateWeightList[typeof(FieAITaskFlightlingFly)] = 100;
			return true;
		}
	}
}
