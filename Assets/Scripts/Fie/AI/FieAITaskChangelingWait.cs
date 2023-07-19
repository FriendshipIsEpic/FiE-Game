using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskChangelingWait : FieAITaskBase
	{
		private const float EXECUTABLE_INTERVAL = 5f;

		public const float WAIT_SEC_MAX = 1f;

		public const float WAIT_SEC_MIN = 0.5f;

		public float _waitCount;

		public float _currentWaitCount;

		public override void Initialize(FieAITaskController manager)
		{
			_currentWaitCount = 0f;
			_waitCount = Random.Range(0.5f, 1f);
		}

		public override bool Task(FieAITaskController manager)
		{
			if (manager.getExecutedTaskInterval(typeof(FieAITaskChangelingWait)) < 5f)
			{
				return true;
			}
			_currentWaitCount += Time.deltaTime;
			if (_currentWaitCount >= _waitCount)
			{
				return true;
			}
			return false;
		}
	}
}
