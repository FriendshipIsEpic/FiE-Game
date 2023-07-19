using Fie.Enemies.HoovesRaces;
using Fie.Enemies.HoovesRaces.Changeling;
using Fie.Object;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskChangelingShoot : FieAITaskBase
	{
		private enum ShootState
		{
			STATE_RANGE_CALIBRATION,
			STATE_PREPARE,
			STATE_SHOOT
		}

		private const int SHOT_COUNT_MIN = 2;

		private const int SHOT_COUNT_MAX = 2;

		private const float RUNAWAY_SEC_MAX = 2f;

		private const float RUNAWAY_SEC_MIN = 1.5f;

		public const float SHOOT_DISTANCE = 2.5f;

		public const float EXECUTABLE_INTERVAL = 1f;

		private int _maxShotCount;

		private int _shotCount;

		private bool _isEndState;

		public float _runAwayCount;

		public float _currentRunAwayCount;

		private ShootState _shootState;

		public FieAITaskChangelingShoot()
		{
			_maxShotCount = Random.Range(2, 3);
		}

		public override void Initialize(FieAITaskController manager)
		{
			_maxShotCount = 0;
			_shotCount = 0;
			_isEndState = false;
			_currentRunAwayCount = 0f;
			_shootState = ShootState.STATE_RANGE_CALIBRATION;
			_runAwayCount = Random.Range(1.5f, 2f);
		}

		public override bool Task(FieAITaskController manager)
		{
			if (manager.ownerCharacter.groundState != 0)
			{
				return true;
			}
			if (manager.getExecutedTaskInterval(typeof(FieAITaskChangelingShoot)) < 1f)
			{
				manager.AddQueueTask<FieAITaskChangelingBackstep>();
				return true;
			}
			if (!(manager.ownerCharacter.detector.lockonTargetObject != null))
			{
				return true;
			}
			switch (_shootState)
			{
			case ShootState.STATE_RANGE_CALIBRATION:
			{
				float num = Vector3.Distance(manager.ownerCharacter.transform.position, manager.ownerCharacter.detector.lockonTargetObject.transform.position);
				if (num < 2.5f)
				{
					_currentRunAwayCount += Time.deltaTime;
					if (_currentRunAwayCount > _runAwayCount)
					{
						manager.AddQueueTask<FieAITaskChangelingBackstep>();
						manager.AddQueueTask<FieAITaskChangelingMelee>();
						return true;
					}
					Vector3 vector = manager.ownerCharacter.detector.lockonTargetObject.transform.position - manager.ownerCharacter.transform.position;
					vector.y = (vector.z = 0f);
					vector *= -1f;
					vector.Normalize();
					if (vector.y > 0.5f)
					{
						vector.y = 0.5f;
						vector.Normalize();
					}
					manager.ownerCharacter.RequestToChangeState<FieStateMachineEnemiesHoovesRacesMove>(vector, 1f, FieGameCharacter.StateMachineType.Base);
				}
				else
				{
					_shootState = ShootState.STATE_PREPARE;
				}
				break;
			}
			case ShootState.STATE_PREPARE:
			{
				manager.ownerCharacter.RequestToChangeState<FieStateMachineChangelingShoot>(Vector3.zero, 1f, FieGameCharacter.StateMachineType.Base);
				FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
				if (!(currentStateMachine is FieStateMachineChangelingShoot))
				{
					return true;
				}
				currentStateMachine.stateChangeEvent += delegate
				{
					_shotCount++;
					if (_shotCount < _maxShotCount)
					{
						_shootState = ShootState.STATE_PREPARE;
					}
					else
					{
						_isEndState = true;
					}
				};
				_shootState = ShootState.STATE_SHOOT;
				break;
			}
			case ShootState.STATE_SHOOT:
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
