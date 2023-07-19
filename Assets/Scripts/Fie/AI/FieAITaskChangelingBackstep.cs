using Fie.Enemies.HoovesRaces.Changeling;
using Fie.Object;

namespace Fie.AI
{
	public class FieAITaskChangelingBackstep : FieAITaskBase
	{
		private enum StepState
		{
			STATE_PREPARE,
			STATE_STEP
		}

		private const float ATTACK_DISTANCE = 1.5f;

		public const float EXECUTABLE_INTERVAL = 0.5f;

		private bool _isEndState;

		private StepState _stepState;

		public override void Initialize(FieAITaskController manager)
		{
			_isEndState = false;
			_stepState = StepState.STATE_PREPARE;
		}

		public override bool Task(FieAITaskController manager)
		{
			if (manager.getExecutedTaskInterval(typeof(FieAITaskChangelingBackstep)) < 0.5f)
			{
				return true;
			}
			switch (_stepState)
			{
			case StepState.STATE_PREPARE:
			{
				manager.ownerCharacter.RequestToChangeState<FieStateMachineChangelingBackstep>(manager.ownerCharacter.flipDirectionVector, 1f, FieGameCharacter.StateMachineType.Base);
				FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (currentStateMachine is FieStateMachineChangelingBackstep)
				{
					currentStateMachine.stateChangeEvent += delegate
					{
						_isEndState = true;
					};
					_stepState = StepState.STATE_STEP;
				}
				break;
			}
			case StepState.STATE_STEP:
				if (_isEndState)
				{
					return true;
				}
				break;
			}
			return false;
		}
	}
}
