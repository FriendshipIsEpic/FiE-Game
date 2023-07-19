using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	public class FieStateMachineRisingSunTeleportation : FieStateMachineRisingSunInterface
	{
		private enum TeleportationState
		{
			TELEPORTATION_START,
			TELEPORTATION_STANDBY,
			TELEPORTATION,
			TELEPORTATION_END
		}

		private const float TELEPORT_DEFAULT_SHILED_COST = 0.1f;

		private const float TELEPORT_DELAY = 0.2f;

		private const float TELEPORT_DURATION = 0.3f;

		private const float TELEPORT_DISTANCE = 1.5f;

		private TeleportationState _teleportationState;

		private bool _isEnd;

		private Type _nextState;

		private Vector3 _teleportNormalVec = Vector3.zero;

		private Vector3 _teleportTargetPos = Vector3.zero;

		private FieObjectGroundState _startGroundState;

		private Tweener<TweenTypesInOutSine> _teleportTweener = new Tweener<TweenTypesInOutSine>();

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieRisingSun)
			{
				FieRisingSun fieRisingSun = gameCharacter as FieRisingSun;
				switch (_teleportationState)
				{
				case TeleportationState.TELEPORTATION_START:
				{
					_teleportNormalVec = fieRisingSun.externalInputVector.normalized;
					_teleportNormalVec.y *= 0.5f;
					if (fieRisingSun.groundState == FieObjectGroundState.Grounding && _teleportNormalVec.y <= 0f)
					{
						_teleportNormalVec.y = 0f;
						_teleportNormalVec.Normalize();
					}
					if (_teleportNormalVec == Vector3.zero)
					{
						if (fieRisingSun.flipState == FieObjectFlipState.Left)
						{
							_teleportNormalVec = Vector3.left;
						}
						else
						{
							_teleportNormalVec = Vector3.right;
						}
					}
					_teleportTargetPos = fieRisingSun.position + _teleportNormalVec * 1.5f;
					int layerMask = 262656;
					if (Physics.Raycast(fieRisingSun.position, _teleportNormalVec, out RaycastHit hitInfo, 1.5f, layerMask))
					{
						_teleportTargetPos = hitInfo.point;
					}
					fieRisingSun.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_EVADED), 25);
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_RISING_SUN_EVADE), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_RISING_SUN_EVADE));
					fieRisingSun.isEnableCollider = false;
					_teleportTweener.InitTweener(0.3f, fieRisingSun.position, _teleportTargetPos);
					_startGroundState = fieRisingSun.groundState;
					_teleportationState = TeleportationState.TELEPORTATION;
					break;
				}
				case TeleportationState.TELEPORTATION:
				{
					Vector3 lhs = fieRisingSun.position = _teleportTweener.UpdateParameterVec3(Time.deltaTime);
					fieRisingSun.setFlipByVector(fieRisingSun.externalInputVector.normalized);
					if (lhs == _teleportTargetPos)
					{
						fieRisingSun.damageSystem.calcShieldDirect((0f - fieRisingSun.healthStats.maxShield) * 0.1f);
						fieRisingSun.damageSystem.setRegenerateDelay(fieRisingSun.healthStats.regenerateDelay * 0.75f, roundToBigger: true);
						fieRisingSun.isEnableCollider = true;
						_teleportationState = TeleportationState.TELEPORTATION_END;
						_isEnd = true;
						if (fieRisingSun.groundState == FieObjectGroundState.Grounding)
						{
							if (_startGroundState == FieObjectGroundState.Flying)
							{
								fieRisingSun.animationManager.SetAnimation(4);
							}
							_nextState = typeof(FieStateMachineCommonIdle);
						}
						else
						{
							_nextState = typeof(FieStateMachineRisingSunTeleportationEndAir);
						}
					}
					break;
				}
				}
				fieRisingSun.physicalForce.SetPhysicalForce(gameCharacter.getNowMoveForce() * -1f, 500f);
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			gameCharacter.transform.localRotation = Quaternion.identity;
			gameCharacter.isEnableGravity = false;
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			gameCharacter.transform.localRotation = Quaternion.identity;
			gameCharacter.isEnableGravity = true;
			gameCharacter.isEnableCollider = true;
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
