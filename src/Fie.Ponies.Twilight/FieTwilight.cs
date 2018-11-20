using Fie.AI;
using Fie.Manager;
using Fie.Object;
using Fie.Object.Abilities;
using Fie.UI;
using GameDataEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/TwilightSparkle")]
	public class FieTwilight : FiePonies, FiePlayableGameCharacterInterface
	{
		public const float TWILIGHT_BASE_SHOT_CHARGE_DURATION_PER_SEGMENT = 0.8f;

		public const float TWILIGHT_BASE_SHOT_SHIELD_COST_FOR_CHARGNING_PER_SEC = 0.1f;

		public const int TWILIGHT_BASE_SHOT_DEFAULT_MAXIMUM_CHARGE_COUNT = 2;

		private int _maximumCharge = 2;

		private int _currentBaseAttackChargedCount;

		public float baseAttackChargedForce;

		public float baseAttackConsumeShieldRate = 1f;

		public float baseAttackChargingTimeRate = 1f;

		private static List<Type> _ignoreAttackState = new List<Type>
		{
			typeof(FieStateMachineTwilightTeleportation),
			typeof(FieStateMachineTwilightSparklyCannonShooting),
			typeof(FieStateMachinePoniesStagger),
			typeof(FieStateMachinePoniesStaggerFall),
			typeof(FieStateMachinePoniesStaggerFallRecover),
			typeof(FieStateMachinePoniesDead),
			typeof(FieStateMachinePoniesRevive),
			typeof(FieStateMachinePoniesGrabbed)
		};

		public int chargedCount
		{
			get
			{
				if (Mathf.FloorToInt(baseAttackChargedForce / 3.2f) > 0)
				{
					return 2;
				}
				if (Mathf.FloorToInt(baseAttackChargedForce / 0.8f) > 0)
				{
					return 1;
				}
				return 0;
			}
		}

		public static List<Type> ignoreAttackState => _ignoreAttackState;

		public override Type getDefaultAttackState()
		{
			return typeof(FieStateMachineTwilightBaseShotCharging);
		}

		public override Type getStormState()
		{
			return typeof(FieStateMachineTwilightTeleportation);
		}

		protected new void Awake()
		{
			base.Awake();
			base.animationManager = new FieSkeletonAnimationController(base.skeletonUtility, new FieTwilightAnimationContainer());
			_staggerCancelableStateList.Add(typeof(FieStateMachineTwilightTeleportation));
			abstractStateList.Add(typeof(FieStateMachinePoniesJump), typeof(FieStateMachineTwilightJump));
			abstractStateList.Add(typeof(FieStateMachinePoniesEvasion), typeof(FieStateMachineTwilightTeleportation));
			abstractStateList.Add(typeof(FieStateMachinePoniesBaseAttack), typeof(FieStateMachineTwilightBaseShotCharging));
			abstractStateList.Add(typeof(FieStateMachinePoniesAttackIdle), typeof(FieStateMachineTwilightBaseShotActivator));
			base.abilitiesContainer.AssignAbility(FieAbilitiesSlot.SlotType.SLOT_1, new FieStateMachineTwilightSparklyCannon());
			base.abilitiesContainer.AssignAbility(FieAbilitiesSlot.SlotType.SLOT_2, new FieStateMachineTwilightSummonArrow());
			base.abilitiesContainer.AssignAbility(FieAbilitiesSlot.SlotType.SLOT_3, new FieStateMachineTwilightForceField());
			syncBindedAbilities();
			SetStateActivateCheckCallback<FieStateMachineTwilightTeleportation>(TeleportationActivateCheck);
			PreAssignEmittableObject<FieEmitObjectTwilightExplosiveTeleportation>();
			PreAssignEmittableObject<FieEmitObjectTwilightHitEffectSmall>();
			PreAssignEmittableObject<FieEmitObjectTwilightHitEffectSemiMiddle>();
			PreAssignEmittableObject<FieEmitObjectTwilightHitEffectMiddle>();
			PreAssignEmittableObject<FieEmitObjectTwilightChargedEffect>();
			PreAssignEmittableObject<FieEmitObjectTwilightBaseShot>();
			PreAssignEmittableObject<FieEmitObjectTwilightTwinkleArrow>();
			PreAssignEmittableObject<FieEmitObjectTwilightShinyArrow>();
			PreAssignEmittableObject<FieEmitObjectTwilightSummonArrow>();
			PreAssignEmittableObject<FieEmitObjectTwilightSummonArrowChild>();
			PreAssignEmittableObject<FieEmitObjectTwilightSummonTrap>();
			PreAssignEmittableObject<FieEmitObjectTwilightSummonTrapEntity>();
			PreAssignEmittableObject<FieEmitObjectTwilightStunningSummonArrowChild>();
			PreAssignEmittableObject<FieEmitObjectTwilightForceField>();
			PreAssignEmittableObject<FieEmitObjectTwilightForceFieldEmitEffect>();
			PreAssignEmittableObject<FieEmitObjectTwilightForceFieldEntity>();
			PreAssignEmittableObject<FieEmitObjectTwilightLaser>();
			PreAssignEmittableObject<FieEmitObjectTwilightLaserChild>();
			PreAssignEmittableObject<FieEmitObjectTwilightLaserDust>();
			PreAssignEmittableObject<FieEmitObjectTwilightSpellEffect>();
			PreAssignEmittableObject<FieEmitObjectTwilightForceFieldReflectEffect>();
			base.detector.intoTheBattleEvent += Detector_intoTheBattleEvent;
		}

		private void Detector_intoTheBattleEvent(FieGameCharacter targetCharacter)
		{
			SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_ENEMY_DETECTED));
		}

		private bool TeleportationActivateCheck()
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
			ReCalcSkillData();
		}

		protected override void ReCalcSkillData()
		{
			baseAttackConsumeShieldRate = 1f;
			baseAttackChargingTimeRate = 1f;
			GDESkillTreeData skill = GetSkill(FieConstValues.FieSkill.MAGIC_ATTACK_PASSIVE_LV1_2);
			if (skill != null)
			{
				baseAttackConsumeShieldRate += skill.Value1;
			}
			GDESkillTreeData skill2 = GetSkill(FieConstValues.FieSkill.MAGIC_ATTACK_PASSIVE_LV4_AGRESSIVE_CASTING);
			if (skill2 != null)
			{
				baseAttackConsumeShieldRate += skill2.Value1;
			}
			GDESkillTreeData skill3 = GetSkill(FieConstValues.FieSkill.MAGIC_ATTACK_PASSIVE_LV4_SMART_CASTING);
			if (skill3 != null)
			{
				baseAttackChargingTimeRate *= skill3.Value1;
				baseAttackConsumeShieldRate += skill3.Value2;
			}
			GDESkillTreeData skill4 = GetSkill(FieConstValues.FieSkill.MAGIC_ATTACK_PASSIVE_LV2_1);
			if (skill4 != null)
			{
				baseAttackChargingTimeRate *= 1f + skill4.Value1;
			}
			GDESkillTreeData skill5 = GetSkill(FieConstValues.FieSkill.MAGIC_ATTACK_PASSIVE_LV3_2);
			if (skill5 != null)
			{
				baseAttackChargingTimeRate *= 1f + skill5.Value1;
			}
		}

		public new void Update()
		{
			base.Update();
			int chargedCount = this.chargedCount;
			if (_currentBaseAttackChargedCount != chargedCount)
			{
				if (chargedCount > _currentBaseAttackChargedCount)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightChargedEffect>(base.hornTransform, Vector3.zero, null);
				}
				_currentBaseAttackChargedCount = chargedCount;
			}
		}

		public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			base.OnPhotonSerializeView(stream, info);
			if (stream.isWriting)
			{
				stream.SendNext(baseAttackChargedForce);
			}
			else
			{
				baseAttackChargedForce = (float)stream.ReceiveNext();
			}
		}

		public override string getDefaultName()
		{
			return FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_ELEMENT_NAME_MAGIC_SIMPLE);
		}

		public override Type getDefaultAITask()
		{
			return typeof(FieAITaskTwilightIdle);
		}

		public override FieConstValues.FieGameCharacter getGameCharacterID()
		{
			return FieConstValues.FieGameCharacter.MAGIC;
		}

		public override KeyValuePair<Type, string> getAbilitiesIconInfo()
		{
			return new KeyValuePair<Type, string>(typeof(FieGameUIAbilitiesIconTwilight), "Prefabs/UI/AbilitiesIcons/TwilightAbilityIcon");
		}

		public override GDEGameCharacterTypeData getCharacterTypeData()
		{
			return FieMasterData<GDEGameCharacterTypeData>.I.GetMasterData(GDEItemKeys.GameCharacterType_MAGIC);
		}
	}
}
