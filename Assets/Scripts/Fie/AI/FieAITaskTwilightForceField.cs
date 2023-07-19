using Fie.Object;
using Fie.Ponies.Twilight;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskTwilightForceField : FieAITaskTwilightBase
	{
		private bool _isEnd;

		public override void Initialize(FieAITaskController manager)
		{
			_isEnd = false;
			manager.ownerCharacter.damageSystem.staggerEvent += healthSystem_staggerEvent;
			manager.ownerCharacter.damageSystem.damagedEvent += HealthSystem_damagedEvent;
		}

		public override void Terminate(FieAITaskController manager)
		{
			manager.ownerCharacter.damageSystem.staggerEvent -= healthSystem_staggerEvent;
			manager.ownerCharacter.damageSystem.damagedEvent -= HealthSystem_damagedEvent;
		}

		private void HealthSystem_damagedEvent(FieGameCharacter attacker, FieDamage damage)
		{
			nextStateWeightList[typeof(FieAITaskTwilightEnemyEvade)] = 100;
			_isEnd = true;
		}

		private void healthSystem_staggerEvent(FieDamage damageObject)
		{
			nextStateWeightList[typeof(FieAITaskTwilightEnemyEvade)] = 100;
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
			manager.ownerCharacter.RequestToChangeState<FieStateMachineTwilightForceField>(Vector3.zero, 0f, FieGameCharacter.StateMachineType.Attack);
			return true;
		}
	}
}
