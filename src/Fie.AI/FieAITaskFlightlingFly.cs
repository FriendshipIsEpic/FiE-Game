using Fie.Enemies.HoovesRaces.Flightling;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskFlightlingFly : FieAITaskBase
	{
		private enum FlightState
		{
			PREPARE,
			SET_ARTITUDE,
			DO
		}

		public const float HORMING_DISTANCE = 4f;

		private const int FLY_COUNT = 2;

		private const float MAXIMUM_FLYING_COUNT = 3f;

		private const float FLYING_DURATION = 0.75f;

		private const float FLY_VECTOR_RANGE_X_MAX = 0.5f;

		private const float FLY_VECTOR_RANGE_X_MIN = 0.1f;

		private float _lifeCount;

		private int _flyingCount;

		private bool _isEndState;

		private FlightState _state;

		public override void Initialize(FieAITaskController manager)
		{
			_lifeCount = 0f;
			_flyingCount = 0;
			_isEndState = false;
			_state = FlightState.PREPARE;
		}

		public override bool Task(FieAITaskController manager)
		{
			_lifeCount += Time.deltaTime;
			if (_lifeCount > 3f)
			{
				return true;
			}
			if (!(manager.ownerCharacter != null))
			{
				return true;
			}
			switch (_state)
			{
			case FlightState.PREPARE:
				manager.ownerCharacter.RequestToChangeState<FieStateMachineFlightlingFly>(Vector3.zero, 0f, FieGameCharacter.StateMachineType.Base);
				_state = FlightState.SET_ARTITUDE;
				break;
			case FlightState.SET_ARTITUDE:
			{
				if (_flyingCount >= 2)
				{
					nextStateWeightList[typeof(FieAITaskFlightlingShot)] = 100;
					return true;
				}
				if (manager.ownerCharacter.detector.lockonTargetObject == null)
				{
					return true;
				}
				float num = Vector3.Distance(manager.ownerCharacter.transform.position, manager.ownerCharacter.detector.lockonTargetObject.transform.position);
				if (num > 4f && manager.ownerCharacter.groundState != 0)
				{
					nextStateWeightList[typeof(FieAITaskFlightlingFlyMove)] = 100;
					return true;
				}
				FieStateMachineFlightlingFly fieStateMachineFlightlingFly2 = manager.ownerCharacter.getStateMachine().getCurrentStateMachine() as FieStateMachineFlightlingFly;
				if (fieStateMachineFlightlingFly2 == null)
				{
					return true;
				}
				Vector3 vector = Vector3.down;
				if (manager.ownerCharacter.groundState != 0)
				{
					float num2 = Random.Range(0.1f, 0.5f);
					if (Random.Range(0, 100) >= 50)
					{
						num2 *= -1f;
					}
					vector += new Vector3(num2, 0f, 0f);
				}
				vector.Normalize();
				Vector3 vector2 = fieStateMachineFlightlingFly2.calcArtitude(manager.ownerCharacter, vector);
				if (!(vector2 != Vector3.zero))
				{
					return true;
				}
				fieStateMachineFlightlingFly2.setNextPosition(manager.ownerCharacter.position, vector2, 0.75f);
				_flyingCount++;
				_state = FlightState.DO;
				break;
			}
			case FlightState.DO:
			{
				FieStateMachineFlightlingFly fieStateMachineFlightlingFly = manager.ownerCharacter.getStateMachine().getCurrentStateMachine() as FieStateMachineFlightlingFly;
				if (fieStateMachineFlightlingFly != null && fieStateMachineFlightlingFly.isEndMoving())
				{
					_state = FlightState.SET_ARTITUDE;
				}
				break;
			}
			}
			return false;
		}
	}
}
