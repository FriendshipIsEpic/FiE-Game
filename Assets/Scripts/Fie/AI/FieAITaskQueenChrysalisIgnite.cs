using Fie.Enemies.HoovesRaces.QueenChrysalis;
using Fie.Object;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskQueenChrysalisIgnite : FieAITaskBase
	{
		private enum IgniteState
		{
			IGNITE,
			IGNITE_ATTACKING,
			DELAY
		}

		private const float CONCENTRATE_TIME = 2f;

		private const float SHOOTING_DELAY = 1.25f;

		private float timeCount;

		private IgniteState _state;

		private bool _isEndState;

		public override void Initialize(FieAITaskController manager)
		{
			timeCount = 0f;
			_state = IgniteState.IGNITE;
			_isEndState = false;
		}

		public override bool Task(FieAITaskController manager)
		{
			timeCount += Time.deltaTime;
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
			case IgniteState.IGNITE:
				manager.ownerCharacter.RequestToChangeState<FieStateMachineQueenChrysalisIgniteStart>(manager.ownerCharacter.flipDirectionVector, 0f, FieGameCharacter.StateMachineType.Base);
				_state = IgniteState.IGNITE_ATTACKING;
				break;
			case IgniteState.IGNITE_ATTACKING:
			{
				FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (!(currentStateMachine is FieStateMachineQueenChrysalisIgniteStart) && !(currentStateMachine is FieStateMachineQueenChrysalisIgnitePrepare) && !(currentStateMachine is FieStateMachineQueenChrysalisIgniteFailed) && !(currentStateMachine is FieStateMachineQueenChrysalisIgniteSucceed))
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
