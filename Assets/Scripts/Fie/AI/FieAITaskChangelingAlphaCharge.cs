using Fie.Enemies.HoovesRaces.ChangelingAlpha;
using Fie.Object;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskChangelingAlphaCharge : FieAITaskBase
	{
		private enum MeleeState
		{
			STATE_PREPARE,
			STATE_MELEE
		}

		private const float HORMING_SEC_MAX = 2f;

		private const float HORMING_SEC_MIN = 1.5f;

		private const float ATTACK_DISTANCE = 2.75f;

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
			if (_isEndState)
			{
				return true;
			}
			_currentHormingCount += Time.deltaTime;
			if (!(manager.ownerCharacter.detector.lockonTargetObject != null))
			{
				return true;
			}
			switch (_meleeState)
			{
			case MeleeState.STATE_PREPARE:
			{
				float num = Vector3.Distance(manager.ownerCharacter.transform.position, manager.ownerCharacter.detector.lockonTargetObject.transform.position);
				if (num > 2.75f)
				{
					Vector3 vector = manager.ownerCharacter.detector.lockonTargetObject.transform.position - manager.ownerCharacter.transform.position;
					vector.y = vector.z;
					vector.z = 0f;
					manager.ownerCharacter.RequestToChangeState<FieStateMachineChangelingAlphaCharge>(vector.normalized, 1f, FieGameCharacter.StateMachineType.Base);
				}
				else if (_isFirstCheck)
				{
					Vector3 vector2 = manager.ownerCharacter.detector.lockonTargetObject.transform.position - manager.ownerCharacter.transform.position;
					vector2.y = vector2.z;
					vector2.z = 0f;
					manager.ownerCharacter.RequestToChangeState<FieStateMachineChangelingAlphaZeroDistanceCharge>(vector2.normalized, 1f, FieGameCharacter.StateMachineType.Base);
					FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
					if (!(currentStateMachine is FieStateMachineChangelingAlphaZeroDistanceCharge))
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
					manager.ownerCharacter.RequestToChangeState<FieStateMachineChangelingAlphaChargeFinish>(Vector3.zero, 1f, FieGameCharacter.StateMachineType.Base);
					FieStateMachineInterface currentStateMachine2 = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
					if (!(currentStateMachine2 is FieStateMachineChangelingAlphaChargeFinish))
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
