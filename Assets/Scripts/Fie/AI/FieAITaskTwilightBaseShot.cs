using Fie.Object;
using Fie.Ponies;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskTwilightBaseShot : FieAITaskTwilightBase
	{
		public const float SHOOTING_TIME_MAX = 1.5f;

		public const float SHOOTING_TIME_MIN = 0.5f;

		private bool _isEnd;

		private float _lifeCount;

		private float _meleeCount;

		public override void Initialize(FieAITaskController manager)
		{
			_isEnd = false;
			_lifeCount = 0f;
			_meleeCount = Random.Range(0.5f, 1.5f);
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
			_lifeCount += Time.deltaTime;
			if (manager.ownerCharacter.detector.lockonTargetObject == null)
			{
				return true;
			}
			float num = Vector3.Distance(manager.ownerCharacter.transform.position, manager.ownerCharacter.detector.lockonTargetObject.position);
			if (num > 4.5f)
			{
				nextStateWeightList[typeof(FieAITaskPoniesOwnerTracking)] = 100;
				return true;
			}
			if (_lifeCount >= _meleeCount)
			{
				return true;
			}
			if (manager.ownerCharacter.healthStats.shield <= 0f)
			{
				return true;
			}
			if (AdjustDirectionByBasicMovement(manager))
			{
				return false;
			}
			manager.ownerCharacter.RequestToChangeState<FieStateMachinePoniesBaseAttack>(Vector3.zero, 0f, FieGameCharacter.StateMachineType.Attack);
			return false;
		}
	}
}
