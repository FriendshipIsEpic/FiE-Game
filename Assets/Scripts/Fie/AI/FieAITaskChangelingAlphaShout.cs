using Fie.Enemies.HoovesRaces.ChangelingAlpha;
using Fie.Object;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskChangelingAlphaShout : FieAITaskBase
	{
		private enum ShoutState
		{
			PREPARE,
			CONCENTRATE,
			DO,
			DELAY
		}

		private const float SHOUT_TIME = 0.8f;

		private float timeCount;

		private ShoutState _state;

		private bool _isEndState;

		public override void Initialize(FieAITaskController manager)
		{
			timeCount = 0f;
			_state = ShoutState.PREPARE;
			_isEndState = false;
			if (manager.ownerCharacter != null)
			{
				manager.ownerCharacter.damageSystem.staggerEvent += healthSystem_staggerEvent;
			}
		}

		public override void Terminate(FieAITaskController manager)
		{
			if (manager.ownerCharacter != null)
			{
				manager.ownerCharacter.damageSystem.staggerEvent -= healthSystem_staggerEvent;
			}
		}

		private void healthSystem_staggerEvent(FieDamage damageObject)
		{
			nextStateWeightList[typeof(FieAITaskChangelingBackstep)] = 100;
			_isEndState = true;
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
			case ShoutState.PREPARE:
				manager.ownerCharacter.RequestToChangeState<FieStateMachineChangelingAlphaShout>(manager.ownerCharacter.flipDirectionVector, 0f, FieGameCharacter.StateMachineType.Base);
				_state = ShoutState.CONCENTRATE;
				break;
			case ShoutState.CONCENTRATE:
				if (!(timeCount >= 0.8f))
				{
					break;
				}
				_state = ShoutState.DO;
				goto case ShoutState.DO;
			case ShoutState.DO:
				nextStateWeightList[typeof(FieAITaskChangelingAlphaCharge)] = 100;
				return true;
			}
			return false;
		}
	}
}
