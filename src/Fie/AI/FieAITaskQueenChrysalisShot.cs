using Fie.Enemies.HoovesRaces.QueenChrysalis;
using Fie.Object;
using System;

namespace Fie.AI
{
	public class FieAITaskQueenChrysalisShot : FieAITaskBase
	{
		private enum ShotState
		{
			SHOOT,
			SHOOTING,
			DELAY
		}

		private const float CONCENTRATE_TIME = 2f;

		private const float SHOOTING_DELAY = 1.25f;

		private ShotState _state;

		private bool _isEndState;

		public override void Initialize(FieAITaskController manager)
		{
			_state = ShotState.SHOOT;
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
			case ShotState.SHOOT:
			{
				manager.ownerCharacter.RequestToChangeState<FieStateMachineQueenChrysalisHormingShot>(manager.ownerCharacter.flipDirectionVector, 0f, FieGameCharacter.StateMachineType.Base);
				FieStateMachineInterface currentStateMachine2 = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (currentStateMachine2 is FieStateMachineQueenChrysalisHormingShot)
				{
					currentStateMachine2.stateChangeEvent += delegate(Type fromType, Type toType)
					{
						if (toType != typeof(FieStateMachineQueenChrysalisPenetrateShot))
						{
							nextStateWeightList[typeof(FieAITaskQueenChrysalisIdle)] = 100;
							_isEndState = true;
						}
					};
				}
				_state = ShotState.SHOOTING;
				break;
			}
			case ShotState.SHOOTING:
			{
				FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (!(currentStateMachine is FieStateMachineQueenChrysalisHormingShot) && !(currentStateMachine is FieStateMachineQueenChrysalisPenetrateShot))
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
