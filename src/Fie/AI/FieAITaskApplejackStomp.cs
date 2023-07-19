using Fie.Object;
using Fie.Ponies;
using Fie.Ponies.Applejack;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskApplejackStomp : FieAITaskBase
	{
		private bool _isAssigned;

		private bool _isEnd;

		public override void Initialize(FieAITaskController manager)
		{
			_isAssigned = false;
			_isEnd = false;
			manager.ownerCharacter.damageSystem.staggerEvent += healthSystem_staggerEvent;
		}

		public override void Terminate(FieAITaskController manager)
		{
			manager.ownerCharacter.damageSystem.staggerEvent -= healthSystem_staggerEvent;
		}

		private void healthSystem_staggerEvent(FieDamage damageObject)
		{
			nextStateWeightList[typeof(FieAITaskApplejackBackstep)] = 100;
			_isEnd = true;
		}

		public override bool Task(FieAITaskController manager)
		{
			if (_isEnd)
			{
				return true;
			}
			if (manager.ownerCharacter.detector.lockonTargetObject == null)
			{
				return true;
			}
			if (!_isAssigned)
			{
				manager.ownerCharacter.RequestToChangeState<FieStateMachinePoniesAbilitySlot3>(Vector3.zero, 0f, FieGameCharacter.StateMachineType.Attack);
				FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (!(currentStateMachine is FieStateMachineApplejackStompAction))
				{
					return true;
				}
				currentStateMachine.stateChangeEvent += delegate
				{
					nextStateWeightList[typeof(FieAITaskApplejackBackstep)] = 100;
					_isEnd = true;
				};
				_isAssigned = true;
			}
			return false;
		}
	}
}
