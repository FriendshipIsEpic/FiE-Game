using Fie.Object;
using Fie.Ponies;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskApplejackMelee : FieAITaskBase
	{
		public const float MELEE_TIME_MAX = 3f;

		public const float MELEE_TIME_MIN = 1.5f;

		private bool _isEnd;

		private float _lifeCount;

		private float _meleeCount;

		public override void Initialize(FieAITaskController manager)
		{
			_isEnd = false;
			_lifeCount = 0f;
			_meleeCount = Random.Range(1.5f, 3f);
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
			_lifeCount += Time.deltaTime;
			if (manager.ownerCharacter.detector.lockonTargetObject == null)
			{
				return true;
			}
			float num = Vector3.Distance(manager.ownerCharacter.transform.position, manager.ownerCharacter.detector.lockonTargetObject.position);
			if (num > 45f)
			{
				nextStateWeightList[typeof(FieAITaskPoniesOwnerTracking)] = 100;
				return true;
			}
			if (_lifeCount >= _meleeCount)
			{
				return true;
			}
			manager.ownerCharacter.RequestToChangeState<FieStateMachinePoniesBaseAttack>(Vector3.zero, 0f, FieGameCharacter.StateMachineType.Attack);
			return false;
		}
	}
}
