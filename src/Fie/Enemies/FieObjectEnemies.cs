using Fie.Manager;
using Fie.Object;
using Fie.Scene;
using Fie.Utility;
using GameDataEditor;
using HighlightingSystem;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies
{
	[RequireComponent(typeof(SkeletonUtility))]
	public abstract class FieObjectEnemies : FieGameCharacter
	{
		public const float SCORE_BONUS_RATE_FOR_DEFEAT = 0.25f;

		public const float SCORE_BONUS_RATE_FOR_ASSIT_PER_PLAYER = 0.1f;

		public const float SCORE_MINIMUM_GUARANTEED = 0.1f;

		private static Color HighlightStartColor = new Color(1f, 0f, 0f, 0.5f);

		private static Color HighlightEndColor = new Color(1f, 0f, 0f, 0f);

		private static Color HighlightStartColorNonTargetting = new Color(0.95f, 0.5f, 0f, 0.5f);

		private static Color HighlightEndColorNonTargetting = new Color(0.95f, 0.5f, 0f, 0f);

		private const float HighlightFlashCountPerSec = 0.5f;

		[SerializeField]
		private FieAttribute _shieldType;

		private Highlighter _highlighter;

		private bool _isEnableHighLight;

		public bool isEnableHighLight
		{
			get
			{
				return _isEnableHighLight;
			}
			set
			{
				if (value != _isEnableHighLight)
				{
					SwitchHighLighter(value);
				}
				_isEnableHighLight = value;
			}
		}

		public void setHighLightColorByTargetStatus(FieGameCharacter gameCharacter)
		{
			if (base.detector.isTargetting(gameCharacter))
			{
				_highlighter.FlashingParams(HighlightStartColor, HighlightEndColor, 0.5f);
			}
			else
			{
				_highlighter.FlashingParams(HighlightStartColorNonTargetting, HighlightEndColorNonTargetting, 0.5f);
			}
		}

		private void SwitchHighLighter(bool isEnable)
		{
			if (!(_highlighter == null))
			{
				if (isEnable)
				{
					_highlighter.enabled = true;
					_highlighter.FlashingOn();
					_highlighter.SeeThroughOn();
				}
				else
				{
					_highlighter.enabled = false;
					_highlighter.FlashingOff();
				}
			}
		}

		private void InitializeHighlighter()
		{
			if (!(_highlighter == null))
			{
				_highlighter.enabled = false;
				_isEnableHighLight = false;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			healthStats.shieldType = _shieldType;
			base.damageSystem.isEnableHitPointGate = false;
			base.damageSystem.isEnableShieldGate = false;
			base.damageSystem.dyingNeedSec = 0f;
			_highlighter = base.submeshObject.gameObject.AddComponent<Highlighter>();
			if (_highlighter != null)
			{
				InitializeHighlighter();
			}
			base.damageSystem.deathEvent += FieEnemiesDeathEventCallback;
			FieManagerBehaviour<FieInGameCharacterStatusManager>.I.AssignCharacter(FieInGameCharacterStatusManager.ForcesTag.ENEMY, this);
		}

		private void FieEnemiesDeathEventCallback(FieGameCharacter killer, FieDamage damage)
		{
			if (!(killer == null) && damage != null)
			{
				FieGameCharacter usersCharacter = FieManagerBehaviour<FieUserManager>.I.GetUserData(FieManagerBehaviour<FieUserManager>.I.myPlayerNumber).usersCharacter;
				if (killer.GetInstanceID() != usersCharacter.GetInstanceID() && FieRandom.Range(0, 100) >= 50)
				{
					usersCharacter.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_ALLY_DEFEAT_ENEMY));
				}
				else
				{
					killer.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_DEFEAT_ENEMY));
				}
				FieManagerBehaviour<FieInGameCharacterStatusManager>.I.DissociateCharacter(FieInGameCharacterStatusManager.ForcesTag.ENEMY, this);
				if (base.photonView.isMine)
				{
					DeployScoresToDefeaters(killer, base.damageSystem);
				}
			}
		}

		private void DeployScoresToDefeaters(FieGameCharacter killer, FieDamageSystem damageSystem)
		{
			if (!(base.photonView == null) && base.photonView.isMine && !(damageSystem == null) && damageSystem.takenDamage != null && damageSystem.takenDamage.Count > 0)
			{
				float num = 0f;
				float num2 = 0f;
				foreach (KeyValuePair<FieGameCharacter, float> item in damageSystem.takenDamage)
				{
					if (item.Key != killer)
					{
						num += 0.1f;
					}
					num2 += item.Value;
				}
				if (!(num2 <= 0f))
				{
					foreach (KeyValuePair<FieGameCharacter, float> item2 in damageSystem.takenDamage)
					{
						int exp = GetEnemyMasterData().Exp;
						bool isDefeater = false;
						float num3 = item2.Value / num2;
						if (item2.Key == killer)
						{
							num3 += 0.35f;
							isDefeater = true;
						}
						else
						{
							num3 += num + 0.1f;
						}
						exp = Mathf.FloorToInt((float)exp * FieManagerBehaviour<FieEnvironmentManager>.I.currentDifficultyData.expMagnify);
						exp = Mathf.FloorToInt((float)exp * base.expRate);
						int score = Mathf.FloorToInt((float)exp * num3);
						item2.Key.ReduceOrIncreaseScore(score, isDefeater);
					}
				}
			}
		}

		protected override void Start()
		{
			base.Start();
			if (FieManagerFactory.I.currentSceneType == FieSceneType.INGAME)
			{
				FieManagerBehaviour<FieInGameStateManager>.I.RetryEvent += RetryEvent;
			}
		}

		private void RetryEvent()
		{
			if (PhotonNetwork.isMasterClient && this != null)
			{
				PhotonNetwork.Destroy(base.photonView);
			}
		}

		protected override void Update()
		{
			base.Update();
		}

		public override void RequestToChangeState<T>(Vector3 directionalVec, float force, StateMachineType type)
		{
			getStateMachine(type).setState(typeof(T), isForceSet: false);
			if (type == StateMachineType.Base)
			{
				base.externalInputVector = directionalVec;
				base.externalInputForce = force;
			}
		}

		public override FieStateMachineInterface getDefaultState(StateMachineType type)
		{
			if (type == StateMachineType.Base)
			{
				return new FieStateMachineCommonIdle();
			}
			return new FieStateMachineEnemiesAttackIdle();
		}

		public abstract void RequestArrivalState();

		public abstract GDEEnemyTableData GetEnemyMasterData();

		public abstract FieConstValues.FieEnemy GetEnemyMasterDataID();
	}
}
