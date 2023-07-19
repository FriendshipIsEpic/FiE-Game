using Fie.AI;
using Fie.Enemies.HoovesRaces.Changeling;
using Fie.Object;
using GameDataEditor;
using System;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/ChangelingAlpha/ChangelingAlpha")]
	public class FieChangelingAlpha : FieChangeling
	{
		protected new void Awake()
		{
			base.Awake();
			base.animationManager = new FieSkeletonAnimationController(base.skeletonUtility, new FieChangelingAlphaAnimationContainer());
			base.damageSystem.addStatusEffectCallback<FieStatusEffectsRiftEntity>(ApplyStatusEffect_ChangelingAlphaRift);
			PreAssignEmittableObject<FieEmitObjectChangelingAlphaConcentration>();
			PreAssignEmittableObject<FieEmitObjectChangelingAlphaShot>();
			PreAssignEmittableObject<FieEmitObjectChangelingAlphaBurst>();
			PreAssignEmittableObject<FieEmitObjectChangelingAlphaShout>();
			PreAssignEmittableObject<FieEmitObjectChangelingAlphaChargeEffect>();
			PreAssignEmittableObject<FieEmitObjectChangelingAlphaCharge>();
			PreAssignEmittableObject<FieEmitObjectChangelingAlphaChargeFinish>();
			PreAssignEmittableObject<FieEmitObjectChangelingAlphaHitEffect>();
			PreAssignEmittableObject<FieEmitObjectChangelingAlphaReflectionShout>();
			PreAssignEmittableObject<FieEmitObjectChangelingAlphaReflectionEffect>();
			PreAssignEmittableObject<FieEmitObjectChangelingForcesArrivalParticleEffect>();
			PreAssignEmittableObject<FieEmitObjectChangelingForcesArrivalFireEffect>();
			PreAssignEmittableObject<FieEmitObjectChangelingForcesDeadEffect>();
		}

		private void ApplyStatusEffect_ChangelingAlphaRift(FieStatusEffectEntityBase statusEffectObject, FieGameCharacter attacker, FieDamage damage)
		{
			if (healthStats.stagger + damage.stagger >= healthStats.staggerResistance)
			{
				setStateToStatheMachine(typeof(FieStateMachineEnemiesHoovesRacesRift), isForceSet: true, isDupulicate: true);
				damage.stagger = 0f;
				healthStats.stagger = 0f;
			}
		}

		public override string getDefaultName()
		{
			return FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_ENEMY_NAME_CHANGELING_ALPHA);
		}

		public override FieConstValues.FieGameCharacter getGameCharacterID()
		{
			return FieConstValues.FieGameCharacter.CHANGELING_ALPHA;
		}

		public override Type getDefaultAttackState()
		{
			return typeof(FieStateMachineChangelingBaseAttack);
		}

		public override Type getDefaultAITask()
		{
			return typeof(FieAITaskChangelingAlphaIdle);
		}

		public override int getBackStepAnimationID()
		{
			return 11;
		}

		public override int getArrivalAnimationID()
		{
			return 16;
		}

		public override GDEEnemyTableData GetEnemyMasterData()
		{
			return FieMasterData<GDEEnemyTableData>.I.GetMasterData(GDEItemKeys.EnemyTable_ENEMY_TABLE_CHANGELING_ALPHA);
		}

		public override FieConstValues.FieEnemy GetEnemyMasterDataID()
		{
			return FieConstValues.FieEnemy.ENEMY_TABLE_CHANGELING_ALPHA;
		}
	}
}
