using Fie.Enemies.HoovesRaces.QueenChrysalis;
using Fie.Object;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskQueenChrysalisMeteoShower : FieAITaskBase
	{
		private enum ShotState
		{
			PREPARE,
			ATTACKING
		}

		private float timeCount;

		private ShotState _state;

		private bool _isEndState;

		public override void Initialize(FieAITaskController manager)
		{
			timeCount = 0f;
			_state = ShotState.PREPARE;
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
			case ShotState.PREPARE:
				manager.ownerCharacter.RequestToChangeState<FieStateMachineQueenChrysalisMeteorShowerPrepare>(manager.ownerCharacter.flipDirectionVector, 0f, FieGameCharacter.StateMachineType.Base);
				_state = ShotState.ATTACKING;
				break;
			case ShotState.ATTACKING:
			{
				FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (!(currentStateMachine is FieStateMachineQueenChrysalisMeteorShowerPrepare) && !(currentStateMachine is FieStateMachineQueenChrysalisMeteorShowerAttacking) && !(currentStateMachine is FieStateMachineQueenChrysalisMeteorShowerFinished))
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
