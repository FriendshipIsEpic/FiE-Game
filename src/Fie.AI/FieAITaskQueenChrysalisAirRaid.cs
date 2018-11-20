using Fie.Enemies.HoovesRaces.QueenChrysalis;
using Fie.Object;

namespace Fie.AI
{
	public class FieAITaskQueenChrysalisAirRaid : FieAITaskBase
	{
		private enum AirRaidState
		{
			START,
			ATTACKING
		}

		private const float CONCENTRATE_TIME = 2f;

		private const float SHOOTING_DELAY = 1.25f;

		private AirRaidState _state;

		private bool _isEndState;

		public override void Initialize(FieAITaskController manager)
		{
			_isEndState = false;
			_state = AirRaidState.START;
		}

		public override bool Task(FieAITaskController manager)
		{
			if (_isEndState)
			{
				return true;
			}
			if (!(manager.ownerCharacter != null))
			{
				return true;
			}
			if (manager.ownerCharacter.detector.lockonTargetObject == null)
			{
				return true;
			}
			switch (_state)
			{
			case AirRaidState.START:
				manager.ownerCharacter.RequestToChangeState<FieStateMachineQueenChrysalisAirRiadPrepare>(manager.ownerCharacter.flipDirectionVector, 0f, FieGameCharacter.StateMachineType.Base);
				_state = AirRaidState.ATTACKING;
				break;
			case AirRaidState.ATTACKING:
			{
				FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (!(currentStateMachine is FieStateMachineQueenChrysalisAirRiadPrepare) && !(currentStateMachine is FieStateMachineQueenChrysalisAirRiadAttacking) && !(currentStateMachine is FieStateMachineQueenChrysalisAirRaidFinished))
				{
					nextStateWeightList[typeof(FieAITaskQueenChrysalisIdle)] = 100;
					_isEndState = true;
				}
				break;
			}
			}
			return false;
		}
	}
}
