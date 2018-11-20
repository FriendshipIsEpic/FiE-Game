using Fie.Enemies.HoovesRaces;
using Fie.Enemies.HoovesRaces.Changeling;
using Fie.Object;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskChangelingMelee : FieAITaskBase
	{
		private enum MeleeState
		{
			STATE_PREPARE,
			STATE_MELEE
		}

		private const float HORMING_SEC_MAX = 2f;

		private const float HORMING_SEC_MIN = 1.5f;

		private const float SHOOT_THLESHOLD_HEIGHT = 1.5f;

		public const float VORTEX_DISTANCE = 2f;

		public const float ATTACK_DISTANCE = 2.5f;

		public float _hormingCount;

		public float _currentHormingCount;

		private bool _isFirstCheck = true;

		private bool _isEndState;

		private MeleeState _meleeState;

		public override void Initialize(FieAITaskController manager)
		{
			_currentHormingCount = 0f;
			_isFirstCheck = true;
			_isEndState = false;
			_meleeState = MeleeState.STATE_PREPARE;
			_hormingCount = Random.Range(1.5f, 2f);
		}

		public override bool Task(FieAITaskController manager)
		{
			_currentHormingCount += Time.deltaTime;
			if (manager.ownerCharacter.groundState != 0)
			{
				return true;
			}
			if (_currentHormingCount > _hormingCount)
			{
				nextStateWeightList[typeof(FieAITaskChangelingBackstep)] = 100;
				return true;
			}
			if (!(manager.ownerCharacter.detector.lockonTargetObject != null))
			{
				return true;
			}
			switch (_meleeState)
			{
			case MeleeState.STATE_PREPARE:
			{
				float num = Vector3.Distance(manager.ownerCharacter.transform.position, manager.ownerCharacter.detector.lockonTargetObject.transform.position);
				if (num > 2.5f)
				{
					Vector3 vector = manager.ownerCharacter.detector.lockonTargetObject.transform.position - manager.ownerCharacter.transform.position;
					vector.y = vector.z;
					vector.z = 0f;
					manager.ownerCharacter.RequestToChangeState<FieStateMachineEnemiesHoovesRacesMove>(vector.normalized, 1f, FieGameCharacter.StateMachineType.Base);
					Vector3 position = manager.ownerCharacter.detector.lockonTargetObject.transform.position;
					float y = position.y;
					Vector3 position2 = manager.ownerCharacter.position;
					if (Mathf.Abs(y - position2.y) > 1.5f && manager.getExecutedTaskInterval(typeof(FieAITaskChangelingShoot)) > 1f)
					{
						manager.ResetQueueTask();
						if (num < 2.5f)
						{
							manager.AddQueueTask<FieAITaskChangelingBackstep>();
						}
						manager.AddQueueTask<FieAITaskChangelingShoot>();
						_isEndState = true;
						return true;
					}
				}
				else if (num < 2f && _isFirstCheck)
				{
					Vector3 vector2 = manager.ownerCharacter.detector.lockonTargetObject.transform.position - manager.ownerCharacter.transform.position;
					vector2.y = vector2.z;
					vector2.z = 0f;
					manager.ownerCharacter.RequestToChangeState<FieStateMachineChangelingVortex>(vector2.normalized, 1f, FieGameCharacter.StateMachineType.Base);
					FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
					if (!(currentStateMachine is FieStateMachineChangelingVortex))
					{
						return true;
					}
					currentStateMachine.stateChangeEvent += delegate
					{
						nextStateWeightList[typeof(FieAITaskChangelingBackstep)] = 100;
						_isEndState = true;
					};
					_meleeState = MeleeState.STATE_MELEE;
				}
				else
				{
					manager.ownerCharacter.RequestToChangeState<FieStateMachineChangelingMelee>(Vector3.zero, 1f, FieGameCharacter.StateMachineType.Base);
					FieStateMachineInterface currentStateMachine2 = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
					if (!(currentStateMachine2 is FieStateMachineChangelingMelee))
					{
						return true;
					}
					currentStateMachine2.stateChangeEvent += delegate
					{
						nextStateWeightList[typeof(FieAITaskChangelingBackstep)] = 100;
						_isEndState = true;
					};
					_meleeState = MeleeState.STATE_MELEE;
				}
				_isFirstCheck = false;
				break;
			}
			case MeleeState.STATE_MELEE:
				if (_isEndState)
				{
					return true;
				}
				break;
			}
			return false;
		}
	}
}
