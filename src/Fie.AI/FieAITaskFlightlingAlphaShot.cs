using Fie.Enemies.HoovesRaces.ChangelingAlpha;
using Fie.Object;
using System;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskFlightlingAlphaShot : FieAITaskBase
	{
		private enum ShotState
		{
			PREPARE,
			CONCENTRATE,
			DO,
			DELAY
		}

		private const float CONCENTRATE_TIME = 2f;

		private const float SHOOTING_DELAY = 1.25f;

		private float _timeCount;

		private ShotState _state;

		private bool _isEndState;

		public override void Initialize(FieAITaskController manager)
		{
			_timeCount = 0f;
			_state = ShotState.PREPARE;
			_isEndState = false;
		}

		public override bool Task(FieAITaskController manager)
		{
			_timeCount += Time.deltaTime;
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
			{
				manager.ownerCharacter.RequestToChangeState<FieStateMachineChangelingAlphaConcentration>(manager.ownerCharacter.flipDirectionVector, 0f, FieGameCharacter.StateMachineType.Base);
				FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (currentStateMachine is FieStateMachineChangelingAlphaConcentration)
				{
					currentStateMachine.stateChangeEvent += delegate(Type fromType, Type toType)
					{
						if (toType != typeof(FieStateMachineChangelingAlphaShoot))
						{
							nextStateWeightList[typeof(FieAITaskChangelingBackstep)] = 100;
							_isEndState = true;
						}
					};
				}
				if (!(_timeCount >= 2f))
				{
					break;
				}
				_state = ShotState.DO;
				goto case ShotState.DO;
			}
			case ShotState.DO:
				manager.ownerCharacter.RequestToChangeState<FieStateMachineChangelingAlphaShoot>(manager.ownerCharacter.flipDirectionVector, 0f, FieGameCharacter.StateMachineType.Base);
				_timeCount = 0f;
				_state = ShotState.DELAY;
				break;
			case ShotState.DELAY:
				if (_timeCount >= 1.25f)
				{
					return true;
				}
				break;
			}
			return false;
		}
	}
}
