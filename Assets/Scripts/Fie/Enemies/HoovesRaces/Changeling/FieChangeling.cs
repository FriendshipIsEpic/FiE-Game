using Fie.AI;
using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Changeling/Changeling")]
	public class FieChangeling : FieEnemiesHoovesRaces
	{
		public override Type getDefaultAttackState()
		{
			return typeof(FieStateMachineChangelingBaseAttack);
		}

		protected new void Awake()
		{
			base.Awake();
			base.animationManager = new FieSkeletonAnimationController(base.skeletonUtility, new FieChangelingAnimationContainer());
			base.damageSystem.deathEvent += delegate
			{
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingForcesDeadEffect>(base.centerTransform, Vector3.zero, null);
				UnbindFromDetecter();
				if (PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.Destroy(base.photonView);
				}
			};
			base.damageSystem.addStatusEffectCallback<FieStatusEffectsRiftEntity>(ApplyStatusEffect_ChangelingsCommonRift);
			base.damageSystem.addStatusEffectCallback<FieStatusEffectsPullEntity>(ApplyStatusEffect_ChangelingsCommonPull);
			PreAssignEmittableObject<FieEmitObjectChangelingBite>();
			PreAssignEmittableObject<FieEmitObjectChangelingShot>();
			PreAssignEmittableObject<FieEmitObjectChangelingHitEffectSmall>();
			PreAssignEmittableObject<FieEmitObjectChangelingBiteHitEffect>();
			PreAssignEmittableObject<FieEmitObjectChangelingVortex>();
			PreAssignEmittableObject<FieEmitObjectChangelingForcesArrivalParticleEffect>();
			PreAssignEmittableObject<FieEmitObjectChangelingForcesArrivalFireEffect>();
			PreAssignEmittableObject<FieEmitObjectChangelingForcesDeadEffect>();
		}

		private void ApplyStatusEffect_ChangelingsCommonRift(FieStatusEffectEntityBase statusEffectObject, FieGameCharacter attacker, FieDamage damage)
		{
			if (damage.statusEffects.Count > 0)
			{
				foreach (FieStatusEffectEntityBase statusEffect in damage.statusEffects)
				{
					FieStatusEffectsRiftEntity fieStatusEffectsRiftEntity = statusEffect as FieStatusEffectsRiftEntity;
					if (fieStatusEffectsRiftEntity != null && healthStats.stagger + damage.stagger >= healthStats.staggerResistance)
					{
						FieStateMachineEnemiesHoovesRacesRift fieStateMachineEnemiesHoovesRacesRift = setStateToStatheMachine(typeof(FieStateMachineEnemiesHoovesRacesRift), isForceSet: true, isDupulicate: true) as FieStateMachineEnemiesHoovesRacesRift;
						if (fieStateMachineEnemiesHoovesRacesRift != null)
						{
							fieStateMachineEnemiesHoovesRacesRift.ResetMoveForce(fieStatusEffectsRiftEntity.resetMoveForce);
							fieStateMachineEnemiesHoovesRacesRift.SetRiftForceRate(fieStatusEffectsRiftEntity.riftForceRate);
						}
						damage.stagger = 0f;
						healthStats.stagger = 0f;
					}
				}
			}
		}

		private void ApplyStatusEffect_ChangelingsCommonPull(FieStatusEffectEntityBase statusEffectObject, FieGameCharacter attacker, FieDamage damage)
		{
			if (damage.statusEffects.Count > 0)
			{
				foreach (FieStatusEffectEntityBase statusEffect in damage.statusEffects)
				{
					FieStatusEffectsPullEntity fieStatusEffectsPullEntity = statusEffect as FieStatusEffectsPullEntity;
					if (fieStatusEffectsPullEntity != null)
					{
						FieStateMachineStatusEffectPull fieStateMachineStatusEffectPull = setStateToStatheMachine(typeof(FieStateMachineStatusEffectPull), isForceSet: true, isDupulicate: true) as FieStateMachineStatusEffectPull;
						if (fieStateMachineStatusEffectPull != null)
						{
							fieStateMachineStatusEffectPull.setPullPoint(fieStatusEffectsPullEntity.pullPosition);
							fieStateMachineStatusEffectPull.setDuration(fieStatusEffectsPullEntity.pullDuration);
						}
						damage.stagger = 0f;
					}
				}
			}
		}

		protected new void Start()
		{
			base.Start();
			base.isEnableAutoFlip = false;
			base.animationManager.SetAnimation(0, isLoop: true);
		}

		public override string getDefaultName()
		{
			return FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_ENEMY_NAME_CHANGELING);
		}

		public override FieConstValues.FieGameCharacter getGameCharacterID()
		{
			return FieConstValues.FieGameCharacter.CHANGELING;
		}

		public override Type getDefaultAITask()
		{
			return typeof(FieAITaskChangelingIdle);
		}

		public virtual int getBackStepAnimationID()
		{
			return 13;
		}

		public virtual int getArrivalAnimationID()
		{
			return 15;
		}

		public override void RequestArrivalState()
		{
			RequestToChangeState<FieStateMachineChangelingArrival>(Vector3.zero, 0f, StateMachineType.Base);
		}

		public override GDEEnemyTableData GetEnemyMasterData()
		{
			return FieMasterData<GDEEnemyTableData>.I.GetMasterData(GDEItemKeys.EnemyTable_ENEMY_TABLE_CHANGELING);
		}

		public override FieConstValues.FieEnemy GetEnemyMasterDataID()
		{
			return FieConstValues.FieEnemy.ENEMY_TABLE_CHANGELING;
		}
	}
}
