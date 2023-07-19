using Fie.Enemies.HoovesRaces;
using UnityEngine;

namespace Fie.AI
{
	public class FieAITaskEnemiesHoovesRacesEvadeBackWall : FieAITaskBase
	{
		public bool _isEnd;

		public Vector3 _backPoint;

		public Vector3 _directionalVec;

		public override void Initialize(FieAITaskController manager)
		{
			_isEnd = false;
			FieAITaskController.FieAIFrontAndBackPoint frontAndBackPoint = manager.GetFrontAndBackPoint();
			_backPoint = frontAndBackPoint.backPoint;
			if (frontAndBackPoint.backDistance <= 0f)
			{
				_isEnd = true;
			}
			_directionalVec = manager.ownerCharacter.centerTransform.position - _backPoint;
			_directionalVec.y = (_directionalVec.z = 0f);
			_directionalVec.Normalize();
		}

		public override bool Task(FieAITaskController manager)
		{
			if (manager.ownerCharacter.detector.lockonTargetObject == null || manager.ownerCharacter.groundState != 0 || _isEnd)
			{
				return true;
			}
			manager.ownerCharacter.RequestToChangeState<FieStateMachineEnemiesHoovesRacesMove>(_directionalVec, 1f, FieGameCharacter.StateMachineType.Base);
			if (Vector3.Distance(manager.ownerCharacter.centerTransform.position, _backPoint) > 2f)
			{
				return true;
			}
			return false;
		}
	}
}
