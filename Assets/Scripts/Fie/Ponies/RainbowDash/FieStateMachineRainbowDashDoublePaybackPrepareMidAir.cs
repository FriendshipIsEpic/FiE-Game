using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies.RainbowDash
{
	public class FieStateMachineRainbowDashDoublePaybackPrepareMidAir : FieStateMachineGameCharacterBase
	{
		private enum PrepareState
		{
			AWAKE,
			PREPARE
		}

		private const float EVASION_DURATION = 0.2f;

		private Type _nextState = typeof(FieStateMachineRainbowDashFlying);

		private bool _isTakeOff;

		private bool _isEnd;

		private bool _isFinished;

		private PrepareState _prepareState;

		private Vector3 _initInputVec = Vector3.zero;

		private Vector3 _evasionTargetPos = Vector3.zero;

		private float _cooldown = 4f;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieRainbowDash)
			{
				FieRainbowDash rainbowDash = gameCharacter as FieRainbowDash;
				switch (_prepareState)
				{
				case PrepareState.AWAKE:
					if (rainbowDash.groundState != FieObjectGroundState.Flying)
					{
						_nextState = typeof(FieStateMachineCommonIdle);
						_isEnd = true;
					}
					else
					{
						int animationId = 33;
						GDESkillTreeData skill = rainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_DOUBLE_PAYBACK_LV4_MAXIMUM_AWESOMENESS);
						if (skill != null)
						{
							animationId = 35;
						}
						GDESkillTreeData skill2 = rainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_DOUBLE_PAYBACK_LV4_AWESOME_COMBO);
						if (skill2 != null)
						{
							animationId = 34;
						}
						TrackEntry trackEntry = rainbowDash.animationManager.SetAnimation(animationId, isLoop: false, isForceSet: true);
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
							{
								if (trackIndex.Data.Name == "immunity")
								{
									setCounterEvent(rainbowDash);
								}
							};
							trackEntry.Complete += delegate
							{
								rainbowDash.damageSystem.damageCheckEvent -= HealthSystem_damageCheckEvent;
								TrackEntry trackEntry2 = rainbowDash.animationManager.SetAnimation(36, isLoop: false, isForceSet: true);
								trackEntry2.Event += delegate(TrackEntry endAnimationState, Event endAnimationTrackIndex)
								{
									if (endAnimationTrackIndex.Data.Name == "finished")
									{
										_isFinished = true;
									}
								};
								trackEntry2.Complete += delegate
								{
									_isEnd = true;
								};
							};
						}
						else
						{
							_isEnd = true;
						}
						_prepareState = PrepareState.PREPARE;
					}
					break;
				}
			}
		}

		private void setCounterEvent(FieRainbowDash rainbowDash)
		{
			rainbowDash.damageSystem.damageCheckEvent += HealthSystem_damageCheckEvent;
			rainbowDash.SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_P_LOYALTY_ABILITY_DOUBLE_PAYBACK_1), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_P_LOYALTY_ABILITY_DOUBLE_PAYBACK_2));
		}

		private bool HealthSystem_damageCheckEvent(FieGameCharacter attacker, FieDamage damage)
		{
			_nextState = typeof(FieStateMachineRainbowDashDoublePaybackAttacking);
			_cooldown = 12f;
			_isEnd = true;
			return false;
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				fieRainbowDash.Decloack();
				fieRainbowDash.isEnableHeadTracking = true;
				fieRainbowDash.isEnableAutoFlip = false;
				fieRainbowDash.isEnableGravity = false;
				fieRainbowDash.resetMoveForce();
				if (fieRainbowDash.detector.lockonTargetObject != null)
				{
					fieRainbowDash.setFlipByVector(fieRainbowDash.detector.lockonTargetObject.transform.position - fieRainbowDash.transform.position);
				}
				GDESkillTreeData skill = fieRainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_DOUBLE_PAYBACK_LV2_2);
				if (skill != null)
				{
					_cooldown *= 1f + skill.Value1;
				}
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				fieRainbowDash.abilitiesContainer.SetCooldown<FieStateMachineRainbowDashDoublePayback>(_cooldown);
				fieRainbowDash.damageSystem.damageCheckEvent -= HealthSystem_damageCheckEvent;
				fieRainbowDash.isEnableHeadTracking = true;
				fieRainbowDash.isEnableGravity = true;
				fieRainbowDash.isEnableAutoFlip = true;
				fieRainbowDash.isEnableCollider = true;
			}
		}

		public override List<Type> getAllowedStateList()
		{
			if (!_isFinished)
			{
				return new List<Type>();
			}
			List<Type> list;
			if (!_isEnd)
			{
				list = new List<Type>();
				list.Add(typeof(FieStateMachineRainbowDashFlying));
				list.Add(typeof(FieStateMachineRainbowDashEvasion));
				list.Add(typeof(FieStateMachineRainbowDashRainblow));
				return list;
			}
			list = new List<Type>();
			list.Add(typeof(FieStateMachineAnyConsider));
			return list;
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
