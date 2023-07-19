using Fie.Enemies.HoovesRaces.QueenChrysalis;
using Fie.Object;

namespace Fie.AI
{
	public class FieAITaskQueenChrysalisDoubleSlash : FieAITaskBase
	{
		private enum MeleeState
		{
			STATE_PREPARE,
			STATE_MELEE
		}

		private bool _isEndState;

		private MeleeState _meleeState;

		public override void Initialize(FieAITaskController manager)
		{
			_isEndState = true;
			_meleeState = MeleeState.STATE_PREPARE;
		}

		public override bool Task(FieAITaskController manager)
		{
			if (!(manager.ownerCharacter.detector.lockonTargetObject != null))
			{
				return true;
			}
			switch (_meleeState)
			{
			case MeleeState.STATE_PREPARE:
			{
				manager.ownerCharacter.RequestToChangeState<FieStateMachineQueenChrysalisDoubleSlash>(manager.ownerCharacter.flipDirectionVector, 1f, FieGameCharacter.StateMachineType.Base);
				FieStateMachineInterface currentStateMachine2 = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				_meleeState = MeleeState.STATE_MELEE;
				break;
			}
			case MeleeState.STATE_MELEE:
			{
				FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (!(currentStateMachine is FieStateMachineQueenChrysalisDoubleSlash))
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
