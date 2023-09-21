using Fie.AI;
using Fie.Manager;
using Fie.Object;
using Fie.Object.Abilities;
using Fie.UI;
using GameDataEditor;
using Spine.Unity.Modules;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/RainbowDash")]
	public class FieRainbowDash : FiePonies, FiePlayableGameCharacterInterface
	{
		public const int MAXIMUM_AWESOME_EFFECT_COUNT = 4;

		public const float AWESOME_DAMAGE_CUTOFF_RATE = 0.2f;

		public const float AWESOME_STAGGER_CUTOFF_RATE = 0.2f;

		private int _maximumAwesomeCount = 4;

		private bool _isCloackMode;

		private bool _reserveColliderState;

		private int _omniSmashAttackingCount;

		private Dictionary<int, FieEmitObjectRainbowDashAwesomeEffect> _awesomeEffectList = new Dictionary<int, FieEmitObjectRainbowDashAwesomeEffect>();

		private SkeletonRendererCustomMaterials _customSpineMaterialComponent;

		private FieEmitObjectRainbowDashCloaking _cloackObject;

		public int maximumAwesomeCount => _maximumAwesomeCount;

		public override bool isEnableCollider
		{
			get
			{
				return _isEnableCollider;
			}
			set
			{
				if (!_isCloackMode)
				{
					if (_colliderList.Count > 0)
					{
						foreach (FieCollider collider in _colliderList)
						{
							collider.isEnable = value;
						}
					}
				}
				else
				{
					_reserveColliderState = value;
				}
				_isEnableCollider = value;
			}
		}

		public bool isCloackMode => _isCloackMode;

		public int omniSmashAttackingCount => _omniSmashAttackingCount;

		public int awesomeCount
		{
			get
			{
				CleanUpAwesomeCount();
				if (_awesomeEffectList == null)
				{
					return 0;
				}
				return _awesomeEffectList.Count;
			}
		}

		public void Cloack()
		{
			if (_cloackObject != null && _cloackObject.enabled)
			{
				_cloackObject.Disable();
				_cloackObject = null;
			}
			FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashDoublePaybackActivationEffect>(base.centerTransform, Vector3.up);
			_cloackObject = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashCloaking>(base.transform, Vector3.zero, null, this);
			if (!(_cloackObject == null))
			{
				_cloackObject.Enable();
				if (_colliderList.Count > 0)
				{
					foreach (FieCollider collider in _colliderList)
					{
						collider.isEnable = false;
					}
				}
				// _customSpineMaterialComponent.SetCustomMaterialOverrides(forceEnable: true);
				isEnableCollider = false;
				_isCloackMode = true;
				UnbindFromDetecter();
			}
		}

		public void Decloack()
		{
			if (_isCloackMode)
			{
				if (_colliderList.Count > 0)
				{
					foreach (FieCollider collider in _colliderList)
					{
						collider.isEnable = _reserveColliderState;
					}
				}
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashDoublePaybackActivationEffect>(base.centerTransform, Vector3.up);
				//_customSpineMaterialComponent.RemoveCustomMaterialOverrides();
				_isCloackMode = false;
				_cloackObject = null;
			}
		}

		private void CleanUpAwesomeCount()
		{
			List<int> list = new List<int>();
			for (int i = 1; i <= maximumAwesomeCount; i++)
			{
				if (_awesomeEffectList.ContainsKey(i) && (_awesomeEffectList[i] == null || _awesomeEffectList[i].gameObject == null))
				{
					list.Add(i);
				}
			}
			if (list.Count > 0)
			{
				foreach (int item in list)
				{
					_awesomeEffectList.Remove(item);
				}
			}
		}

		public override Type getDefaultAttackState()
		{
			return null;
		}

		public override Type getStormState()
		{
			return null;
		}

		protected new void Awake()
		{
			base.Awake();
			base.animationManager = new FieSkeletonAnimationController(base.skeletonUtility, new FieRainbowDashAnimationContainer());
			_customSpineMaterialComponent = base.gameObject.GetComponent<SkeletonRendererCustomMaterials>();
			_staggerCancelableStateList.Add(typeof(FieStateMachineRainbowDashEvasion));
			_staggerCancelableStateList.Add(typeof(FieStateMachineRainbowDashRainblowCloack));
			_staggerCancelableStateList.Add(typeof(FieStateMachineRainbowDashDoublePaybackPrepareMidAir));
			abstractStateList.Add(typeof(FieStateMachinePoniesJump), typeof(FieStateMachineRainbowDashJump));
			abstractStateList.Add(typeof(FieStateMachinePoniesEvasion), typeof(FieStateMachineRainbowDashEvasion));
			abstractStateList.Add(typeof(FieStateMachinePoniesBaseAttack), typeof(FieStateMachineRainbowDashBaseAttack));
			SetStateActivateCheckCallback<FieStateMachineRainbowDashEvasion>(EvasionActivateCheck);
			base.abilitiesContainer.AssignAbility(FieAbilitiesSlot.SlotType.SLOT_1, new FieStateMachineRainbowDashDoublePayback());
			base.abilitiesContainer.AssignAbility(FieAbilitiesSlot.SlotType.SLOT_2, new FieStateMachineRainbowDashRainblow());
			base.abilitiesContainer.AssignAbility(FieAbilitiesSlot.SlotType.SLOT_3, new FieStateMachineRainbowDashOmniSmash());
			syncBindedAbilities();
			base.abilitiesContainer.setActivationCallback(typeof(FieStateMachineRainbowDashOmniSmash), OmniSmashActivationCallback);
			PreAssignEmittableObject<FieEmitObjectRainbowDashAwesomeEffect>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashLoosingAwesomeEffect>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashEvasionEffect>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashBaseAttack1>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashBaseAttack2>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashBaseAttack3>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashHitEffectSmall>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashHitEffectMiddle>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashStabAttack>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashHitEffectStab>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashDoublePaybackActivationEffect>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashDoublePaybackAttackingEffect>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashDoublePaybackCollision>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashDoublePaybackHitEffect>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashRainblowSeed>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashRainblowEntity>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashRainblowEntityMidAir>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashCloaking>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashOmniSmashActivationEffect>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashOmniSmashExplosion>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashOmniSmashExplosionEffect>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashOmniSmashFirstHit>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashOmniSmashSecondHit>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashOmniSmashFinishHit>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashOmniSmashImpactEffect>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashOmniSmashRainboomEffect>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashOmniSmashTrailEffect>();
			PreAssignEmittableObject<FieEmitObjectRainbowDashOmniSmashHitEffect>();
			base.emotionController.SetDefaultEmoteAnimationID(15);
			base.emotionController.RestoreEmotionFromDefaultData();
			base.detector.intoTheBattleEvent += Detector_intoTheBattleEvent;
			base.damageSystem.beforeDamageEvent += this.HealthSystem_beforeDamageEvent;
			ReCalcSkillData();
		}

		protected override void ReCalcSkillData()
		{
			_maximumAwesomeCount = 4;
			GDESkillTreeData skill = GetSkill(FieConstValues.FieSkill.LOYALTY_ATTACK_PASSIVE_LV4_HELLO_CAPTAIN_AWESOME);
			if (skill != null)
			{
				_maximumAwesomeCount += (int)skill.Value1;
			}
		}

		private bool OmniSmashActivationCallback(FieGameCharacter gameCharacter)
		{
			FieManagerBehaviour<FieActivityManager>.I.RequestLobbyOnlyActivity(gameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_LOYALTY_ABILITY_3), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_LOYALTY_ABILITY_3));
			if (base.detector.lockonTargetObject == null || awesomeCount <= 0)
			{
				return false;
			}
			return true;
		}

		public void CalcOmniSmashAttackCount()
		{
			_omniSmashAttackingCount = 2 * Mathf.Max(0, awesomeCount);
			GDESkillTreeData skill = GetSkill(FieConstValues.FieSkill.LOYALTY_OMNISMASH_LV4_LET_ME_SHOW_YOU_MY_TRUE_POWER);
			if (skill != null)
			{
				_omniSmashAttackingCount *= (int)skill.Value1;
			}
			requestSetAwesomeEffect(-maximumAwesomeCount);
		}

		private void HealthSystem_beforeDamageEvent(FieGameCharacter attacker, ref FieDamage damage)
		{
			if (damage != null && !(damage.damage <= 0f))
			{
				int num = requestSetAwesomeEffect(-1);
				if (num < 0)
				{
					damage.damage *= 0.2f;
					damage.stagger *= 0.2f;
					GDESkillTreeData skill = GetSkill(FieConstValues.FieSkill.LOYALTY_DIFFENCE_PASSIVE_LV4_DEAL_WITH_IT);
					if (skill != null)
					{
						base.damageSystem.AddDefenceMagni(skill.ID, 100f, skill.Value1);
					}
					GDESkillTreeData skill2 = GetSkill(FieConstValues.FieSkill.LOYALTY_DIFFENCE_PASSIVE_LV4_IT_IS_MY_TURN);
					if (skill2 != null)
					{
						base.abilitiesContainer.IncreaseOrReduceCooldownAll(0f - skill.Value1);
					}
				}
			}
		}

		private void Detector_intoTheBattleEvent(FieGameCharacter targetCharacter)
		{
			SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_ENEMY_DETECTED));
		}

		private bool EvasionActivateCheck()
		{
			if (healthStats.shield <= 0f)
			{
				return false;
			}
			return true;
		}

		protected new void Start()
		{
			base.Start();
			getStateMachine(StateMachineType.Attack).setState(typeof(FieStateMachinePoniesAttackIdle), isForceSet: true);
			getStateMachine().setState(typeof(FieStateMachineCommonIdle), isForceSet: false);
		}

		public int requestSetAwesomeEffect(int calcCount)
		{
			if (base.photonView == null || base.photonView.isMine)
			{
				int num = Mathf.Clamp(_awesomeEffectList.Count + calcCount, 0, maximumAwesomeCount);
				if (base.photonView != null)
				{
					object[] parameters = new object[1]
					{
						num
					};
					base.photonView.RPC("SetAwesomeEffectRPC", PhotonTargets.Others, parameters);
				}
				return RebuildAwesomeStats(num);
			}
			return 0;
		}

		private int RebuildAwesomeStats(int settleCount)
		{
			int count = _awesomeEffectList.Count;
			List<int> list = new List<int>();
			for (int i = 1; i <= maximumAwesomeCount; i++)
			{
				if (_awesomeEffectList.ContainsKey(i))
				{
					if (i > settleCount)
					{
						list.Add(i);
					}
				}
				else if (i <= settleCount)
				{
					_awesomeEffectList[i] = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashAwesomeEffect>(base.centerTransform, Vector3.up);
				}
			}
			if (list.Count > 0)
			{
				foreach (int item in list)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashLoosingAwesomeEffect>(_awesomeEffectList[item].transform, Vector3.up);
					_awesomeEffectList[item].stopEffect(1f);
					_awesomeEffectList.Remove(item);
				}
			}
			return settleCount - count;
		}

		[PunRPC]
		public void SetAwesomeEffectRPC(int settleCount)
		{
			RebuildAwesomeStats(settleCount);
		}

		public override string getDefaultName()
		{
			return FieLocalizeUtility.GetWordScriptText(GDEItemKeys.ConstantTextList_ELEMENT_NAME_LOYALTY_SIMPLE);
		}

		public override FieConstValues.FieGameCharacter getGameCharacterID()
		{
			return FieConstValues.FieGameCharacter.LOYALTY;
		}

		public override Type getDefaultAITask()
		{
			return typeof(FieAITaskRainbowDashIdle);
		}

		public override KeyValuePair<Type, string> getAbilitiesIconInfo()
		{
			return new KeyValuePair<Type, string>(typeof(FieGameUIAbilitiesIconRainbowDash), "Prefabs/UI/AbilitiesIcons/RainbowDashAbilityIcon");
		}

		public override GDEGameCharacterTypeData getCharacterTypeData()
		{
			return FieMasterData<GDEGameCharacterTypeData>.I.GetMasterData(GDEItemKeys.GameCharacterType_LOYALTY);
		}
	}
}
