using Fie.Enemies.HoovesRaces.QueenChrysalis;
using Fie.Object;

namespace Fie.AI
{
	public class FieAITaskQueenChrysalisTeleportation : FieAITaskBase
	{
		private enum TeleportationState
		{
			TLEREPORT,
			TELEPORTING,
			DELAY
		}

		private const float CONCENTRATE_TIME = 2f;

		private const float SHOOTING_DELAY = 1.25f;

		private TeleportationState _state;

		private bool _isEndState;

		public override void Initialize(FieAITaskController manager)
		{
			_state = TeleportationState.TLEREPORT;
			_isEndState = false;
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
			case TeleportationState.TLEREPORT:
			{
				manager.ownerCharacter.RequestToChangeState<FieStateMachineQueenChrysalisTeleportation>(manager.ownerCharacter.flipDirectionVector, 0f, FieGameCharacter.StateMachineType.Base);
				FieStateMachineInterface currentStateMachine2 = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (currentStateMachine2 is FieStateMachineQueenChrysalisTeleportation)
				{
					currentStateMachine2.stateChangeEvent += delegate
					{
						nextStateWeightList[typeof(FieAITaskQueenChrysalisIdle)] = 100;
						_isEndState = true;
					};
				}
				_state = TeleportationState.TELEPORTING;
				break;
			}
			case TeleportationState.TELEPORTING:
			{
				FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (!(currentStateMachine is FieStateMachineQueenChrysalisTeleportation))
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
