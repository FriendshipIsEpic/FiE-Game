using Fie.AI;
using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using System;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/QueenChrysalis")]
	public class FieQueenChrysalis : FieEnemiesHoovesRaces
	{
		public const int MAXIMUM_MINION_NUMBER = 2;

		public const float DEFAULT_CALLED_MINION_DELAY = 15f;

		public const float DIALOUGE_INTERVAL = 30f;

		public const float DIALOUGE_INTERVAL_FIRST_TIME = 3f;

		[SerializeField]
		private Transform _concentrationTransform;

		private const float STAGGER_IMUNITE_DELAY = 5f;

		public FieGameCharacter grabbingGameCharacter;

		private float _callingMinionDelay;

		private float _maximumCallingMinionDelay;

		private int _maximumCallingMinionCount = 2;

		public bool _isImmuniteStaggerDamage;

		public bool _isEffectivePull;

		public float _staggerDamageRate = 1f;

		public float _staggerImuniteDelay;

		public float _staggerImuniteDelayInitializedParam;

		public float _dialougeInterval = 3f;

		private FieGameCharacter[] _currentMinions = new FieGameCharacter[2];

		public Transform concentrationTransform => _concentrationTransform;

		public FieGameCharacter[] currentMinions => _currentMinions;

		public int currentMinionCount
		{
			get
			{
				int num = 0;
				FieGameCharacter[] currentMinions = _currentMinions;
				foreach (FieGameCharacter x in currentMinions)
				{
					if (x != null)
					{
						num++;
					}
				}
				return num;
			}
		}

		public bool canAbleToCallMinion => currentMinionCount <= 0 && _callingMinionDelay <= 0f;

		public int maximumCallingMinionCount => _maximumCallingMinionCount;

		protected new void Awake()
		{
			base.Awake();
			base.animationManager = new FieSkeletonAnimationController(base.skeletonUtility, new FieQueenChrysalisAnimationContainer());
			base.damageSystem.deathEvent += delegate
			{
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingForcesDeadEffect>(base.centerTransform, Vector3.zero, null);
				UnbindFromDetecter();
				if (PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.Destroy(base.photonView);
				}
			};
			base.damageSystem.beforeDamageEvent += this.HealthSystem_beforeDamageEvent;
			base.damageSystem.ResetStaggerEvent();
			base.damageSystem.staggerEvent += delegate
			{
				setStateToStatheMachine(typeof(FieStateMachineEnemiesQueenChrysalisStagger), isForceSet: true, isDupulicate: true);
			};
			base.damageSystem.addStatusEffectCallback<FieStatusEffectsRiftEntity>(ApplyStatusEffect_QueenChrysalisRift);
			base.damageSystem.addStatusEffectCallback<FieStatusEffectsPullEntity>(ApplyStatusEffect_QueenChrysalisPull);
			PreAssignEmittableObject<FieEmitObjectChangelingForcesArrivalParticleEffect>();
			PreAssignEmittableObject<FieEmitObjectChangelingForcesArrivalFireEffect>();
			PreAssignEmittableObject<FieEmitObjectChangelingForcesDeadEffect>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisHitEffectSmall>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisHitEffectBurned>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisReflectionEffect>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisHornEffect>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisStaggerRecoveringBurst>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisCommonActivationEffect>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisHormingShot>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisHormingShotActivateEffect>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisPenetrateShot>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisPenetrateShotActivateEffect>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisPenetrateShotBurst>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisShootingConcentration>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisDoubleSlashFirst>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisDoubleSlashSecond>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisCrucibleCircle>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisCrucibleBurst>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisIgniteBurst>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisIgniteCollision>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisIgniteConcentration>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisAirRiad>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisAirRaidPreHit>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisAirRaidConcentration>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisMeteorShowerMeteor>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisMeteorShowerBurst>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisMeteorShowerEmittingEffect>();
			PreAssignEmittableObject<FieEmitObjectQueenChrysalisMeteorShowerConcentration>();
		}

		private void HealthSystem_beforeDamageEvent(FieGameCharacter attacker, ref FieDamage damage)
		{
			if (_isImmuniteStaggerDamage)
			{
				damage.stagger = 0f;
			}
			damage.stagger *= _staggerDamageRate;
		}

		protected new void Start()
		{
			base.Start();
			base.emotionController.SetDefaultEmoteAnimationID(30);
			base.emotionController.SetEmoteAnimation(30, isForceSet: true);
			_maximumCallingMinionDelay = 15f;
			_maximumCallingMinionCount = 2;
		}

		private new void Update()
		{
			base.Update();
			if (_staggerImuniteDelay > 0f)
			{
				_staggerImuniteDelay -= Time.deltaTime;
				_staggerDamageRate = ((!(_staggerImuniteDelay <= 0f)) ? (1f - _staggerDamageRate / _staggerImuniteDelayInitializedParam) : 1f);
			}
			if (_callingMinionDelay > 0f && currentMinionCount <= 0)
			{
				_callingMinionDelay -= Time.deltaTime;
			}
			if (_dialougeInterval > 0f && !base.voiceController.isPlaying)
			{
				_dialougeInterval -= Time.deltaTime;
			}
			if (base.isSpeakable && _dialougeInterval <= 0f)
			{
				SetRandomDialouge();
				_dialougeInterval = 30f;
			}
		}

		private void SetRandomDialouge()
		{
			if (healthStats.shield > 0f)
			{
				float num = healthStats.shield / healthStats.maxShield;
				if (num > 0.5f)
				{
					SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_APPEAR_1), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_APPEAR_2));
				}
				else
				{
					SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_RANDOM_1), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_RANDOM_2), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_RANDOM_3));
				}
			}
			else
			{
				float num2 = healthStats.hitPoint / healthStats.maxHitPoint;
				if (num2 > 0.7f)
				{
					SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_RANDOM_6), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_RANDOM_7));
				}
				else if (num2 > 0.3f)
				{
					SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_RANDOM_8), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_RANDOM_9));
				}
				else
				{
					SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_RANDOM_10), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_RANDOM_11), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_RANDOM_12));
				}
			}
		}

		public void SetCalledMinionDelay()
		{
			_callingMinionDelay = _maximumCallingMinionDelay;
		}

		private void ApplyStatusEffect_QueenChrysalisRift(FieStatusEffectEntityBase statusEffectObject, FieGameCharacter attacker, FieDamage damage)
		{
			if (damage.statusEffects.Count > 0)
			{
				foreach (FieStatusEffectEntityBase statusEffect in damage.statusEffects)
				{
					FieStatusEffectsRiftEntity fieStatusEffectsRiftEntity = statusEffect as FieStatusEffectsRiftEntity;
					if (fieStatusEffectsRiftEntity != null && healthStats.stagger + damage.stagger >= healthStats.staggerResistance)
					{
						FieStateMachineQueenChrysalisRift fieStateMachineQueenChrysalisRift = setStateToStatheMachine(typeof(FieStateMachineQueenChrysalisRift), isForceSet: true, isDupulicate: true) as FieStateMachineQueenChrysalisRift;
						if (fieStateMachineQueenChrysalisRift != null)
						{
							fieStateMachineQueenChrysalisRift.ResetMoveForce(fieStatusEffectsRiftEntity.resetMoveForce);
							fieStateMachineQueenChrysalisRift.SetRiftForceRate(fieStatusEffectsRiftEntity.riftForceRate);
						}
						damage.stagger = 0f;
						healthStats.stagger = 0f;
					}
				}
			}
		}

		private void ApplyStatusEffect_QueenChrysalisPull(FieStatusEffectEntityBase statusEffectObject, FieGameCharacter attacker, FieDamage damage)
		{
			if (damage.statusEffects.Count > 0)
			{
				foreach (FieStatusEffectEntityBase statusEffect in damage.statusEffects)
				{
					FieStatusEffectsPullEntity fieStatusEffectsPullEntity = statusEffect as FieStatusEffectsPullEntity;
					if (fieStatusEffectsPullEntity != null)
					{
						if (_isEffectivePull)
						{
							FieStateMachineStatusEffectPull fieStateMachineStatusEffectPull = setStateToStatheMachine(typeof(FieStateMachineStatusEffectPull), isForceSet: true, isDupulicate: true) as FieStateMachineStatusEffectPull;
							if (fieStateMachineStatusEffectPull != null)
							{
								fieStateMachineStatusEffectPull.setPullPoint(fieStatusEffectsPullEntity.pullPosition);
								fieStateMachineStatusEffectPull.setDuration(fieStatusEffectsPullEntity.pullDuration);
							}
						}
						damage.stagger = 0f;
					}
				}
			}
		}

		public void SetStaggerImmuniteDelay(float delay = 5f)
		{
			_staggerImuniteDelay = (_staggerImuniteDelayInitializedParam = delay);
		}

		public override string getDefaultName()
		{
			return FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_ENEMY_NAME_THE_QUEEN_OF_INSECTS);
		}

		public override FieConstValues.FieGameCharacter getGameCharacterID()
		{
			return FieConstValues.FieGameCharacter.THE_INSECT_QUEEN;
		}

		public override Type getDefaultAttackState()
		{
			return typeof(FieStateMachineQueenChrysalisBaseAttack);
		}

		public override Type getDefaultAITask()
		{
			return typeof(FieAITaskQueenChrysalisIdle);
		}

		public override FieStateMachineInterface getDefaultState(StateMachineType type)
		{
			if (type == StateMachineType.Base)
			{
				return new FieStateMachineQueenChrysalisIdle();
			}
			return new FieStateMachineQueenChrysalisBaseAttack();
		}

		public override GDEGameCharacterTypeData getCharacterTypeData()
		{
			return FieMasterData<GDEGameCharacterTypeData>.I.GetMasterData(GDEItemKeys.GameCharacterType_THE_INSECT_QUEEN);
		}

		public override void RequestArrivalState()
		{
		}

		public override GDEEnemyTableData GetEnemyMasterData()
		{
			return FieMasterData<GDEEnemyTableData>.I.GetMasterData(GDEItemKeys.EnemyTable_ENEMY_TABLE_CHRYSALIS);
		}

		public override FieConstValues.FieEnemy GetEnemyMasterDataID()
		{
			return FieConstValues.FieEnemy.ENEMY_TABLE_CHRYSALIS;
		}
	}
}
