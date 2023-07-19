using Fie.Enemies.HoovesRaces.Flightling;
using Fie.Object;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskFlightlingShot : FieAITaskBase
	{
		private enum ShotState
		{
			PREPARE,
			CONCENTRATE,
			DO
		}

		private const float CONCENTRATE_TIME = 1.5f;

		private float timeCount;

		private ShotState _state;

		public override void Initialize(FieAITaskController manager)
		{
			timeCount = 0f;
			_state = ShotState.PREPARE;
		}

		public override bool Task(FieAITaskController manager)
		{
			timeCount += Time.deltaTime;
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
			switch (_state)
			{
			case ShotState.PREPARE:
				manager.ownerCharacter.RequestToChangeState<FieStateMachineFlightlingConcentration>(manager.ownerCharacter.flipDirectionVector, 0f, FieGameCharacter.StateMachineType.Base);
				_state = ShotState.CONCENTRATE;
				break;
			case ShotState.CONCENTRATE:
				if (timeCount >= 1.5f)
				{
					_state = ShotState.DO;
				}
				break;
			case ShotState.DO:
				manager.ownerCharacter.RequestToChangeState<FieStateMachineFlightlingShoot>(manager.ownerCharacter.flipDirectionVector, 0f, FieGameCharacter.StateMachineType.Base);
				return true;
			}
			return false;
		}
	}
}
