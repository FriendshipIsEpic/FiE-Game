using Fie.AI;
using Fie.Object;
using Fie.Object.Abilities;
using Fie.UI;
using GameDataEditor;
using System;
using System.Collections.Generic;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Applejack")]
	public class FieApplejack : FiePonies, FiePlayableGameCharacterInterface
	{
		private bool _isHeavyKickMode;

		private float _kickDamageRatio = 1f;

		private float _reducingCooltimeWithKick;

		private float _reducingShieldRateWithKick;

		private bool _hasTountSkill;

		private bool _isTauntMode;

		private float _tauntDamageRate;

		public bool isHeavyKickMode => _isHeavyKickMode;

		public float kickDamageRatio => _kickDamageRatio;

		public bool hasTountSkill => _hasTountSkill;

		public bool isTauntMode
		{
			get
			{
				return _isTauntMode;
			}
			set
			{
				_isTauntMode = value;
			}
		}

		public override Type getDefaultAttackState()
		{
			return typeof(FieStateMachineApplejackBaseAttack);
		}

		public override Type getStormState()
		{
			return typeof(FieStateMachineApplejackEvasion);
		}

		protected new void Awake()
		{
			base.Awake();
			base.animationManager = new FieSkeletonAnimationController(base.skeletonUtility, new FieApplejackAnimationContainer());
			_staggerCancelableStateList.Add(typeof(FieStateMachineApplejackFireRope));
			_staggerCancelableStateList.Add(typeof(FieStateMachineApplejackFireAir));
			abstractStateList.Add(typeof(FieStateMachinePoniesJump), typeof(FieStateMachineApplejackJump));
			abstractStateList.Add(typeof(FieStateMachinePoniesEvasion), typeof(FieStateMachineApplejackEvasion));
			abstractStateList.Add(typeof(FieStateMachinePoniesBaseAttack), typeof(FieStateMachineApplejackBaseAttack));
			base.abilitiesContainer.AssignAbility(FieAbilitiesSlot.SlotType.SLOT_1, new FieStateMachineApplejackRope());
			base.abilitiesContainer.AssignAbility(FieAbilitiesSlot.SlotType.SLOT_2, new FieStateMachineApplejackYeehaw());
			base.abilitiesContainer.AssignAbility(FieAbilitiesSlot.SlotType.SLOT_3, new FieStateMachineApplejackStomp());
			syncBindedAbilities();
			PreAssignEmittableObject<FieEmitObjectApplejackCharge>();
			PreAssignEmittableObject<FieEmitObjectApplejackPunch>();
			PreAssignEmittableObject<FieEmitObjectApplejackKick>();
			PreAssignEmittableObject<FieEmitObjectApplejackKickRift>();
			PreAssignEmittableObject<FieEmitObjectApplejackAirKick>();
			PreAssignEmittableObject<FieEmitObjectApplejackRope>();
			PreAssignEmittableObject<FieEmitObjectApplejackYeehaw>();
			PreAssignEmittableObject<FieEmitObjectApplejackYeehawReflect>();
			PreAssignEmittableObject<FieEmitObjectApplejackYeehawRegen>();
			PreAssignEmittableObject<FieEmitObjectApplejackYeehawFriendly>();
			PreAssignEmittableObject<FieEmitObjectApplejackStomp>();
			PreAssignEmittableObject<FieEmitObjectApplejackChargeEffect>();
			PreAssignEmittableObject<FieEmitObjectApplejackKickEffect>();
			PreAssignEmittableObject<FieEmitObjectApplejackKickEffect2>();
			PreAssignEmittableObject<FieEmitObjectApplejackHitEffectSmall>();
			PreAssignEmittableObject<FieEmitObjectApplejackHitEffectMiddle>();
			base.emotionController.SetDefaultEmoteAnimationID(15);
			base.emotionController.RestoreEmotionFromDefaultData();
			ReCalcSkillData();
			base.damageSystem.beforeDamageEvent += this.ApplyYeehawTauntSkill;
			base.detector.intoTheBattleEvent += Detector_intoTheBattleEvent;
		}

		protected override void ReCalcSkillData()
		{
			_isHeavyKickMode = false;
			_kickDamageRatio = 1f;
			GDESkillTreeData skill = GetSkill(FieConstValues.FieSkill.HONESTY_ATTACK_PASSIVE_LV4_HIGH_RISK_AND_HIGH_RETURN);
			if (skill != null)
			{
				_isHeavyKickMode = true;
				_kickDamageRatio = skill.Value1;
			}
			GDESkillTreeData skill2 = GetSkill(FieConstValues.FieSkill.HONESTY_ATTACK_PASSIVE_LV4_BATTLE_CYCLE);
			if (skill2 != null)
			{
				_reducingCooltimeWithKick = skill2.Value1;
				_reducingShieldRateWithKick = skill2.Value2;
			}
			GDESkillTreeData skill3 = GetSkill(FieConstValues.FieSkill.HONESTY_YEEHAW_LV4_TAUNT);
			if (skill3 != null)
			{
				_tauntDamageRate = skill3.Value1;
			}
		}

		private void ApplyYeehawTauntSkill(FieGameCharacter attacker, ref FieDamage damage)
		{
			if (damage != null && attacker != null && isTauntMode)
			{
				FieDamage damageObject = new FieDamage(FieAttribute.NONE, damage.damage * _tauntDamageRate, 0f, 0f, 0f, null);
				attacker.AddDamage(this, damageObject);
			}
		}

		internal void ApplyShoutAdditionalDamage(ref FieDamage damageObject)
		{
			GDESkillTreeData skill = GetSkill(FieConstValues.FieSkill.HONESTY_YEEHAW_LV2_1);
			if (skill != null)
			{
				damageObject.damage += skill.Value1;
			}
		}

		public void applySideEffectOfStep()
		{
			GDESkillTreeData skill = GetSkill(FieConstValues.FieSkill.HONESTY_DIFFENCE_PASSIVE_LV4_OFFENSIVE_STEP);
			if (skill != null)
			{
				base.damageSystem.AddAttackMagni(1031, skill.Value2, skill.Value1, -1, isEnableStack: true);
				base.damageSystem.calcShieldDirect(healthStats.maxShield * (0f - skill.Value3));
				base.damageSystem.setRegenerateDelay(healthStats.regenerateDelay * 0.5f, roundToBigger: true);
			}
			GDESkillTreeData skill2 = GetSkill(FieConstValues.FieSkill.HONESTY_DIFFENCE_PASSIVE_LV4_DEFFENSIVE_STEP);
			if (skill2 != null)
			{
				base.damageSystem.AddDefenceMagni(1032, skill2.Value2, skill2.Value1, isEnableStack: true);
				base.abilitiesContainer.IncreaseOrReduceCooldownAll(skill2.Value3);
			}
		}

		private void Detector_intoTheBattleEvent(FieGameCharacter targetCharacter)
		{
			SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_ENEMY_DETECTED));
		}

		protected new void Start()
		{
			base.Start();
			getStateMachine(StateMachineType.Attack).setState(typeof(FieStateMachinePoniesAttackIdle), isForceSet: true);
			getStateMachine().setState(typeof(FieStateMachineCommonIdle), isForceSet: false);
		}

		public void CalcBatteCicleSkill()
		{
			if (_reducingShieldRateWithKick > 0f)
			{
				base.damageSystem.calcShieldDirect(healthStats.maxShield * (0f - _reducingShieldRateWithKick));
				base.damageSystem.setRegenerateDelay(healthStats.regenerateDelay * 0.25f, roundToBigger: true);
			}
			if (_reducingCooltimeWithKick > 0f)
			{
				base.abilitiesContainer.IncreaseOrReduceCooldownAll(0f - _reducingCooltimeWithKick);
			}
		}

		public void ApplyKickDamageMagni(ref FieDamage damageObject)
		{
			if (damageObject != null)
			{
				damageObject.damage *= kickDamageRatio;
				damageObject.stagger *= kickDamageRatio;
			}
		}

		public override string getDefaultName()
		{
			return FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_ELEMENT_NAME_HONESTY_SIMPLE);
		}

		public override FieConstValues.FieGameCharacter getGameCharacterID()
		{
			return FieConstValues.FieGameCharacter.HONESTY;
		}

		public override Type getDefaultAITask()
		{
			return typeof(FieAITaskApplejackIdle);
		}

		public override KeyValuePair<Type, string> getAbilitiesIconInfo()
		{
			return new KeyValuePair<Type, string>(typeof(FieGameUIAbilitiesIconApplejack), "Prefabs/UI/AbilitiesIcons/ApplejackAbilityIcon");
		}

		public override GDEGameCharacterTypeData getCharacterTypeData()
		{
			return FieMasterData<GDEGameCharacterTypeData>.I.GetMasterData(GDEItemKeys.GameCharacterType_HONESTY);
		}
	}
}
