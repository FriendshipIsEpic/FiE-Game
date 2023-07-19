using Fie.Enemies.HoovesRaces.Flightling;
using Fie.Object;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskFlightlingFlyMove : FieAITaskBase
	{
		private enum FlightState
		{
			PREPARE,
			SET_ARTITUDE,
			DO
		}

		public override bool Task(FieAITaskController manager)
		{
			if (!(manager.ownerCharacter != null))
			{
				return true;
			}
			if (manager.ownerCharacter.groundState == FieObjectGroundState.Grounding)
			{
				return true;
			}
			if (manager.ownerCharacter.detector.lockonTargetObject == null)
			{
				return true;
			}
			float num = Vector3.Distance(manager.ownerCharacter.transform.position, manager.ownerCharacter.detector.lockonTargetObject.transform.position);
			if (!(num >= 4f))
			{
				nextStateWeightList[typeof(FieAITaskFlightlingFly)] = 100;
				return true;
			}
			Vector3 vector = manager.ownerCharacter.detector.lockonTargetObject.transform.position - manager.ownerCharacter.transform.position;
			vector.y = vector.z;
			vector.z = 0f;
			manager.ownerCharacter.RequestToChangeState<FieStateMachineFlightlingFlyMove>(vector.normalized, 1f, FieGameCharacter.StateMachineType.Base);
			return false;
		}
	}
}
