using Fie.Object;
using Fie.Object.Abilities;
using Fie.UI;
using GameDataEditor;
using System;
using System.Collections.Generic;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/RisingSun")]
	public class FieRisingSun : FiePonies
	{
		private static List<Type> _ignoreAttackState = new List<Type>
		{
			typeof(FieStateMachineRisingSunTeleportation),
			typeof(FieStateMachinePoniesStagger),
			typeof(FieStateMachinePoniesStaggerFall),
			typeof(FieStateMachinePoniesDead),
			typeof(FieStateMachinePoniesRevive)
		};

		public const string SIGNATURE = "rising_sun";

		public static List<Type> ignoreAttackState => _ignoreAttackState;

		public override Type getDefaultAttackState()
		{
			return typeof(FieStateMachineRisingSunBaseShot);
		}

		public override Type getStormState()
		{
			return typeof(FieStateMachineRisingSunTeleportation);
		}

		protected new void Awake()
		{
			base.Awake();
			base.animationManager = new FieSkeletonAnimationController(base.skeletonUtility, new FieRisingSunAnimationContainer());
			_staggerCancelableStateList.Add(typeof(FieStateMachineRisingSunTeleportation));
			abstractStateList.Add(typeof(FieStateMachinePoniesJump), typeof(FieStateMachineRisingSunJump));
			abstractStateList.Add(typeof(FieStateMachinePoniesEvasion), typeof(FieStateMachineRisingSunTeleportation));
			abstractStateList.Add(typeof(FieStateMachinePoniesBaseAttack), typeof(FieStateMachineRisingSunBaseShot));
			base.abilitiesContainer.AssignAbility(FieAbilitiesSlot.SlotType.SLOT_1, new FieStateMachineRisingSunShinyArrow());
			base.abilitiesContainer.AssignAbility(FieAbilitiesSlot.SlotType.SLOT_2, new FieStateMachineRisingSunSummonArrow());
			base.abilitiesContainer.AssignAbility(FieAbilitiesSlot.SlotType.SLOT_3, new FieStateMachineRisingSunEmission());
			syncBindedAbilities();
			SetStateActivateCheckCallback<FieStateMachineRisingSunTeleportation>(TeleportationActivateCheck);
			PreAssignEmittableObject<FieEmitObjectRisingSunBaseShot>();
			PreAssignEmittableObject<FieEmitObjectRisingSunShinyArrow>();
			PreAssignEmittableObject<FieEmitObjectRisingSunSummonArrow>();
			PreAssignEmittableObject<FieEmitObjectRisingSunSummonArrowChild>();
			PreAssignEmittableObject<FieEmitObjectRisingSunForceField>();
			PreAssignEmittableObject<FieEmitObjectRisingSunForceFieldEmitEffect>();
			PreAssignEmittableObject<FieEmitObjectRisingSunForceFieldEntity>();
			PreAssignEmittableObject<FieEmitObjectRisingSunLaser>();
			PreAssignEmittableObject<FieEmitObjectRisingSunLaserChild>();
			PreAssignEmittableObject<FieEmitObjectRisingSunLaserDust>();
			PreAssignEmittableObject<FieEmitObjectRisingSunSpellEffect>();
			PreAssignEmittableObject<FieEmitObjectRisingSunHitEffectSmall>();
			PreAssignEmittableObject<FieEmitObjectRisingSunHitEffectMiddle>();
			PreAssignEmittableObject<FieEmitObjectRisingSunForceFieldReflectEffect>();
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
		}

		public override string getDefaultName()
		{
			return FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_ELEMENT_NAME_PONICO_SIMPLE);
		}

		public override FieConstValues.FieGameCharacter getGameCharacterID()
		{
			return FieConstValues.FieGameCharacter.NONE;
		}

		public override KeyValuePair<Type, string> getAbilitiesIconInfo()
		{
			return new KeyValuePair<Type, string>(typeof(FieGameUIAbilitiesIconRisingSun), "Prefabs/UI/AbilitiesIcons/RisingSunAbilityIcon");
		}

		public override GDEGameCharacterTypeData getCharacterTypeData()
		{
			return FieMasterData<GDEGameCharacterTypeData>.I.GetMasterData(GDEItemKeys.GameCharacterType_MAGIC);
		}
	}
}
