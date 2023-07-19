using Fie.Object;
using Fie.Utility;
using System;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisTeleportation : FieStateMachineGameCharacterBase
	{
		private enum TeleportationState
		{
			TELEPORTATION_START,
			TELEPORTATION_STANDBY,
			TELEPORTATION,
			TELEPORTATION_END
		}

		private const float TELEPORT_DELAY = 0.2f;

		private const float TELEPORT_DURATION = 1f;

		private const float TELEPORT_DISTANCE = 8f;

		private TeleportationState _teleportationState;

		private bool _isEnd;

		private Type _nextState;

		private Vector3 _teleportNormalVec = Vector3.zero;

		private Vector3 _teleportTargetPos = Vector3.zero;

		private Tweener<TweenTypesInOutSine> _teleportTweener = new Tweener<TweenTypesInOutSine>();

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis fieQueenChrysalis = gameCharacter as FieQueenChrysalis;
			if (!(fieQueenChrysalis == null))
			{
				switch (_teleportationState)
				{
				case TeleportationState.TELEPORTATION_START:
					if (fieQueenChrysalis.groundState != 0)
					{
						_isEnd = true;
					}
					else
					{
						if (fieQueenChrysalis.flipState == FieObjectFlipState.Left)
						{
							_teleportNormalVec = Vector3.left;
						}
						else
						{
							_teleportNormalVec = Vector3.right;
						}
						if (fieQueenChrysalis.detector.lockonTargetObject != null)
						{
							_teleportNormalVec = fieQueenChrysalis.detector.lockonTargetObject.position - fieQueenChrysalis.position;
							_teleportNormalVec.y = 0f;
							_teleportNormalVec.Normalize();
						}
						_teleportTargetPos = fieQueenChrysalis.position + _teleportNormalVec * 8f;
						int layerMask = 1049088;
						if (Physics.Raycast(fieQueenChrysalis.centerTransform.position, _teleportNormalVec, out RaycastHit hitInfo, 8f, layerMask) && Physics.Raycast(hitInfo.point, Vector3.down, out hitInfo, 8f, layerMask))
						{
							_teleportTargetPos = hitInfo.point;
						}
						fieQueenChrysalis.isEnableCollider = false;
						_teleportTweener.InitTweener(1f, fieQueenChrysalis.position, _teleportTargetPos);
						_teleportationState = TeleportationState.TELEPORTATION;
					}
					break;
				case TeleportationState.TELEPORTATION:
				{
					Vector3 vector2 = fieQueenChrysalis.position = _teleportTweener.UpdateParameterVec3(Time.deltaTime);
					fieQueenChrysalis.setFlipByVector(fieQueenChrysalis.externalInputVector.normalized);
					if (_teleportTweener.IsEnd())
					{
						fieQueenChrysalis.isEnableCollider = true;
						_teleportationState = TeleportationState.TELEPORTATION_END;
						_isEnd = true;
						autoFlipToEnemy(gameCharacter);
						_nextState = typeof(FieStateMachineCommonIdle);
					}
					break;
				}
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				autoFlipToEnemy(gameCharacter);
				gameCharacter.transform.localRotation = Quaternion.identity;
				gameCharacter.isEnableGravity = false;
				gameCharacter.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.transform.localRotation = Quaternion.identity;
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableCollider = true;
				gameCharacter.isEnableAutoFlip = false;
			}
		}

		public override float getDelay()
		{
			return 0.2f;
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return _nextState;
		}
	}
}
