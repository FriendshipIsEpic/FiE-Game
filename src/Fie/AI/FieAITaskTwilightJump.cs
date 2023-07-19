using Fie.Object;
using Fie.Ponies;
using Fie.Ponies.Twilight;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskTwilightJump : FieAITaskBase
	{
		public const float JUMPING_MAXIMUM_TIME = 3f;

		public const float JUMPING_THRESHOLD_TIME = 0.3f;

		private float _lifeCount;

		private float _jumpCount;

		private bool _isJump;

		public override void Initialize(FieAITaskController manager)
		{
			_lifeCount = 0f;
			_jumpCount = 0f;
			_isJump = false;
		}

		public override bool Task(FieAITaskController manager)
		{
			if (manager.ownerCharacter.groundState == FieObjectGroundState.Flying)
			{
				return true;
			}
			_lifeCount += Time.deltaTime;
			if (_lifeCount >= 3f)
			{
				return true;
			}
			if (manager.ownerCharacter.detector.lockonTargetObject == null)
			{
				return true;
			}
			if (_isJump)
			{
				return true;
			}
			manager.ownerCharacter.RequestToChangeState<FieStateMachinePoniesJump>(Vector3.up, 1f, FieGameCharacter.StateMachineType.Base);
			FieStateMachineInterface currentStateMachine = manager.ownerCharacter.getStateMachine().getCurrentStateMachine();
			if (!(currentStateMachine is FieStateMachineTwilightFlying))
			{
				return true;
			}
			_jumpCount += Time.deltaTime;
			if (_jumpCount > 0.3f)
			{
				_isJump = true;
			}
			return false;
		}
	}
}
