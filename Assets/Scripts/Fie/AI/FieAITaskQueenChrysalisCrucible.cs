using Fie.Enemies.HoovesRaces.QueenChrysalis;
using Fie.Object;

namespace Fie.AI
{
	public class FieAITaskQueenChrysalisCrucible : FieAITaskBase
	{
		private enum CrucibleState
		{
			START,
			DOING
		}

		private const float CONCENTRATE_TIME = 2f;

		private const float SHOOTING_DELAY = 1.25f;

		private CrucibleState _state;

		private bool _isEndState;

		public override void Initialize(FieAITaskController manager)
		{
			_isEndState = false;
			_state = CrucibleState.START;
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
			switch (_state)
			{
			case CrucibleState.START:
			{
				manager.ownerCharacter.RequestToChangeState<FieStateMachineQueenChrysalisCrucible>(manager.ownerCharacter.flipDirectionVector, 1f, FieGameCharacter.StateMachineType.Base);
				FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (currentStateMachine is FieStateMachineQueenChrysalisCrucible)
				{
					currentStateMachine.stateChangeEvent += delegate
					{
						_isEndState = true;
					};
				}
				else
				{
					_isEndState = true;
				}
				break;
			}
			}
			return false;
		}
	}
}
