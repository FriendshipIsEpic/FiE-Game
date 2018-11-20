using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightTeleportation : FieStateMachineGameCharacterBase
	{
		private enum TeleportationState
		{
			TELEPORTATION_START,
			TELEPORTATION_STANDBY,
			TELEPORTATION,
			TELEPORTATION_END
		}

		private const float TELEPORT_DEFAULT_SHILED_COST = 0.03f;

		private const float TELEPORT_DELAY = 0.2f;

		private const float TELEPORT_DURATION = 0.5f;

		private const float TELEPORT_DISTANCE = 3f;

		private TeleportationState _teleportationState;

		private bool _isEnd;

		private Type _nextState;

		private Vector3 _teleportNormalVec = Vector3.zero;

		private Vector3 _teleportTargetPos = Vector3.zero;

		private FieObjectGroundState _startGroundState;

		private Tweener<TweenTypesInOutSine> _teleportTweener = new Tweener<TweenTypesInOutSine>();

		private float _consumeShield;

		private float _reduceCooldownTime;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieTwilight)
			{
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				switch (_teleportationState)
				{
				case TeleportationState.TELEPORTATION_START:
				{
					_teleportNormalVec = fieTwilight.externalInputVector.normalized;
					_teleportNormalVec.y *= 0.5f;
					if (fieTwilight.groundState == FieObjectGroundState.Grounding && _teleportNormalVec.y <= 0f)
					{
						_teleportNormalVec.y = 0f;
						_teleportNormalVec.Normalize();
					}
					if (_teleportNormalVec == Vector3.zero)
					{
						if (fieTwilight.flipState == FieObjectFlipState.Left)
						{
							_teleportNormalVec = Vector3.left;
						}
						else
						{
							_teleportNormalVec = Vector3.right;
						}
					}
					_teleportTargetPos = fieTwilight.position + _teleportNormalVec * 3f;
					int layerMask = 262656;
					if (Physics.Raycast(fieTwilight.centerTransform.position, _teleportNormalVec, out RaycastHit hitInfo, 3f, layerMask))
					{
						_teleportTargetPos = hitInfo.point;
						if (Physics.Raycast(hitInfo.point, Vector3.down, out hitInfo, 0.75f, layerMask))
						{
							_teleportTargetPos = hitInfo.point;
						}
					}
					fieTwilight.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_EVADED), 25);
					FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_MAGIC_EVADE), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_MAGIC_EVADE));
					fieTwilight.isEnableCollider = false;
					_teleportTweener.InitTweener(0.5f, fieTwilight.position, _teleportTargetPos);
					_startGroundState = fieTwilight.groundState;
					_teleportationState = TeleportationState.TELEPORTATION;
					break;
				}
				case TeleportationState.TELEPORTATION:
				{
					Vector3 vector2 = fieTwilight.position = _teleportTweener.UpdateParameterVec3(Time.deltaTime);
					fieTwilight.setFlipByVector(fieTwilight.externalInputVector.normalized);
					if (_teleportTweener.IsEnd())
					{
						fieTwilight.damageSystem.calcShieldDirect(0f - _consumeShield);
						fieTwilight.damageSystem.setRegenerateDelay(fieTwilight.healthStats.regenerateDelay * 0.75f, roundToBigger: true);
						if (_reduceCooldownTime < 0f)
						{
							fieTwilight.abilitiesContainer.IncreaseOrReduceCooldownAll(_reduceCooldownTime);
						}
						fieTwilight.isEnableCollider = true;
						_teleportationState = TeleportationState.TELEPORTATION_END;
						_isEnd = true;
						if (fieTwilight.groundState == FieObjectGroundState.Grounding)
						{
							if (_startGroundState == FieObjectGroundState.Flying)
							{
								fieTwilight.animationManager.SetAnimation(4);
							}
							_nextState = typeof(FieStateMachineCommonIdle);
						}
						else
						{
							_nextState = typeof(FieStateMachineTwilightTeleportationEndAir);
						}
						if (fieTwilight.GetSkill(FieConstValues.FieSkill.MAGIC_DIFFENCE_PASSIVE_LV4_EXPLOSIVE_TELEPORTATION) != null)
						{
							FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightExplosiveTeleportation>(fieTwilight.centerTransform, Vector3.zero, null, fieTwilight);
						}
					}
					break;
				}
				}
				fieTwilight.physicalForce.SetPhysicalForce(gameCharacter.getNowMoveForce() * -1f, 500f);
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (gameCharacter != null)
			{
				_consumeShield = gameCharacter.healthStats.maxShield * 0.03f;
				float consumeShield = _consumeShield;
				GDESkillTreeData skill = gameCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_DIFFENCE_PASSIVE_LV2_1);
				if (skill != null)
				{
					_consumeShield += consumeShield * skill.Value1;
				}
				GDESkillTreeData skill2 = gameCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_DIFFENCE_PASSIVE_LV4_EXPLOSIVE_TELEPORTATION);
				if (skill2 != null)
				{
					_consumeShield += consumeShield * skill2.Value1;
				}
				GDESkillTreeData skill3 = gameCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_DIFFENCE_PASSIVE_LV4_CONCENTRATIVE_TELEPORTATION);
				if (skill3 != null)
				{
					_reduceCooldownTime -= skill3.Value1;
					_consumeShield += consumeShield * skill3.Value2;
				}
			}
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
