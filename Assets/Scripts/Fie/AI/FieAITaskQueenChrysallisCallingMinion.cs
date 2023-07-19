using Fie.Enemies.HoovesRaces.QueenChrysalis;
using Fie.Object;

namespace Fie.AI
{
	public class FieAITaskQueenChrysallisCallingMinion : FieAITaskBase
	{
		private enum CallingState
		{
			CALLING_PREPARE,
			CALLING
		}

		private bool _isEndState;

		private CallingState _meleeState;

		public override void Initialize(FieAITaskController manager)
		{
			_isEndState = true;
			_meleeState = CallingState.CALLING_PREPARE;
		}

		public override bool Task(FieAITaskController manager)
		{
			if (!(manager.ownerCharacter.detector.lockonTargetObject != null))
			{
				return true;
			}
			switch (_meleeState)
			{
			case CallingState.CALLING_PREPARE:
			{
				manager.ownerCharacter.RequestToChangeState<FieStateMachineQueenChrysalisCallingMinion>(manager.ownerCharacter.flipDirectionVector, 1f, FieGameCharacter.StateMachineType.Base);
				FieStateMachineInterface currentStateMachine2 = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				_meleeState = CallingState.CALLING;
				break;
			}
			case CallingState.CALLING:
			{
				FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (!(currentStateMachine is FieStateMachineQueenChrysalisCallingMinion))
				{
					nextStateWeightList[typeof(FieAITaskQueenChrysalisIdle)] = 100;
					_isEndState = true;
					return true;
				}
				break;
			}
			}
			return false;
		}
	}
}
