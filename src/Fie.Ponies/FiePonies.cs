using Fie.Core;
using Fie.Manager;
using Fie.Object;
using Fie.Object.Abilities;
using Fie.Scene;
using Fie.UI;
using Fie.Utility;
using GameDataEditor;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fie.Ponies
{
	public abstract class FiePonies : FieGameCharacter
	{
		public enum RaceType
		{
			EarthPony = 1,
			Unicorn = 2,
			Pegasus = 4,
			Alicorn = 8,
			Other = 0x10
		}

		public const string PONIES_ANIMATION_TAKEOFF_EVENT_NAME = "takeOff";

		public const int PONIES_DEFAULT_FRIENDSHIP_POINT = 2;

		public const int FRIENDSHIP_COST_FOR_REVIVING_ISSELF = 3;

		public const int FRIENDSHIP_COST_FOR_REVIVING_ANYONE = 2;

		public const float LOST_SECONDRY_HP_DIALG_SEND_INTERVAL = 3f;

		public const float PRIMARY_HP_LESS_THAN_HALF_INTERVAL = 5f;

		public const float STANDARD_ATTACK_CHARGE_1_THRESHOLD = 0.5f;

		public const float STANDARD_ATTACK_CHARGE_2_THRESHOLD = 2.5f;

		public const float STANDARD_ATTACK_CHARGE_3_THRESHOLD = 4.5f;

		public const float FRIENDSHIP_ADD_RATE_FOR_OTHER_PLAYER = 0.5f;

		public const float FRIENDSHIP_SEND_INTERVAL = 0.5f;

		public static readonly Color PONIES_DEFAULT_COLOR = new Color(0.65f, 0.65f, 0.65f, 1f);

		public static readonly Color PONIES_INVISIBLE_COLOR = new Color(0f, 0f, 0f, 0f);

		[SerializeField]
		private RaceType _race = RaceType.EarthPony;

		[SerializeField]
		private FieAttribute _shieldType;

		[SerializeField]
		private FiePhysicalForce _physicalForce;

		[SerializeField]
		private SkeletonUtilityBone _headBone;

		[SerializeField]
		private Transform _torsoTransform;

		[SerializeField]
		private Transform _leftFrontHoofTransform;

		[SerializeField]
		private Transform _rightFrontHoofTransform;

		[SerializeField]
		private Transform _leftBackHoofTransform;

		[SerializeField]
		private Transform _rightBackHoofTransform;

		[SerializeField]
		private Transform _hornTransform;

		[SerializeField]
		private Transform _mouthTransform;

		[SerializeField]
		private Transform _neckTransform;

		private Transform _headTrackingTransform;

		private bool _isEnableHeadTracking = true;

		private Vector3 _headboneDefaultAngle = Vector3.zero;

		private Tweener<TweenTypesInOutSine> _headTrackingTweener = new Tweener<TweenTypesInOutSine>();

		private Dictionary<string, bool> _dialogIntervalTable = new Dictionary<string, bool>();

		protected List<Type> _staggerCancelableStateList = new List<Type>();

		private int _latestFriendshipPoint;

		private float _stackedFriendshipPointForOtherPlayer;

		private float _friendshipPointSendDelay;

		public FiePhysicalForce physicalForce => _physicalForce;

		public Transform torsoTransform => _torsoTransform;

		public Transform leftFrontHoofTransform => _leftFrontHoofTransform;

		public Transform rightFrontHoofTransform => _rightFrontHoofTransform;

		public Transform leftBackHoofTransform => _leftBackHoofTransform;

		public Transform rightBackHoofTransform => _rightBackHoofTransform;

		public Transform hornTransform => _hornTransform;

		public Transform mouthTransform => _mouthTransform;

		public Transform neckTransform => _neckTransform;

		public bool isEnableHeadTracking
		{
			get
			{
				return _isEnableHeadTracking;
			}
			set
			{
				if (_headBone != null)
				{
					if (value)
					{
						_headBone.mode = SkeletonUtilityBone.Mode.Override;
						_headBone.flip = false;
						_headBone.flipX = false;
						_headBone.overrideAlpha = 0f;
						if (_headTrackingTweener != null)
						{
							_headTrackingTweener.InitTweener(0.1f, 0f, 0f);
						}
					}
					else
					{
						_headBone.mode = SkeletonUtilityBone.Mode.Follow;
						_headBone.flip = true;
					}
				}
				_isEnableHeadTracking = value;
			}
		}

		public override Type getDefaultAttackState()
		{
			return typeof(FieStateMachinePoniesBaseAttack);
		}

		protected override void Awake()
		{
			base.Awake();
			abstractStateList.Add(typeof(FieStateMachineCommonIdle), typeof(FieStateMachinePoniesIdle));
			healthStats.shieldType = _shieldType;
			base.damageSystem.staggerEvent += delegate
			{
				if (!base.damageSystem.isDying)
				{
					setStateToStatheMachine(typeof(FieStateMachinePoniesStagger), isForceSet: true, isDupulicate: true);
					if (base.gameObject.GetInstanceID() == FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter.gameObject.GetInstanceID())
					{
						FieManagerBehaviour<FieGameCameraManager>.I.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_BIG);
					}
				}
			};
			_headboneDefaultAngle = _headBone.transform.localEulerAngles;
			base.detector.targetChangedEvent += delegate(FieGameCharacter from, FieGameCharacter to)
			{
				if (to != null)
				{
					_headTrackingTransform = to.centerTransform;
				}
				else
				{
					_headTrackingTransform = null;
					_headTrackingTweener.InitTweener(0.3f, _headBone.overrideAlpha, 0f);
				}
			};
			base.friendshipStats.friendshipAddEvent += delegate(float friendship)
			{
				if ((!(base.photonView != null) || base.photonView.isMine) && !(friendship <= 0f))
				{
					_stackedFriendshipPointForOtherPlayer += friendship * 0.5f;
				}
			};
			base.damageSystem.damagedEvent += damageEventCallback;
			base.damageSystem.deathEvent += delegate
			{
				setStateToStatheMachine(typeof(FieStateMachinePoniesDead), isForceSet: true, isDupulicate: false);
			};
			base.changeScoreEvent += ChangeScoreClalback;
			base.damageSystem.addStatusEffectCallback<FieStatusEffectsRiftEntity>(ApplyStatusEffect_PoniesCommonRift);
			base.damageSystem.addStatusEffectCallback<FieStatusEffectsGrabEntity>(ApplyStatusEffect_PoniesCommonGrab);
			FieManagerBehaviour<FieDialogManager>.I.dialogEndEvent += dialogEndEventCallback;
			PreAssignEmittableObject<FieEmitObjectPoniesArrival>();
			PreAssignEmittableObject<FieEmitObjectPoniesGainedFriendship>();
			PreAssignEmittableObject<FieEmitObjectPoniesRevive>();
			PreAssignEmittableObject<FieGameUIGainedScore>();
			_headBone.overrideAlpha = 0.5f;
			base.friendshipStats.friendship = 60f;
			_latestFriendshipPoint = 2;
			base.transform.SetParent(FieManagerBehaviour<FieGameCharacterManager>.I.transform);
		}

		private void ChangeScoreClalback(int score, bool isDefeater)
		{
			if (base.ownerUser != null)
			{
				FieManagerBehaviour<FieSaveManager>.I.AddCurrentGameExp(this, score);
				if (score > 0)
				{
					FieGameUIGainedScore fieGameUIGainedScore = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieGameUIGainedScore>(base.guiPointTransform, Vector3.zero);
					if (fieGameUIGainedScore != null)
					{
						fieGameUIGainedScore.SetScore(score, isDefeater);
					}
				}
			}
		}

		private void ApplyStatusEffect_PoniesCommonGrab(FieStatusEffectEntityBase statusEffectObject, FieGameCharacter attacker, FieDamage damage)
		{
			if (damage.statusEffects.Count > 0)
			{
				foreach (FieStatusEffectEntityBase statusEffect in damage.statusEffects)
				{
					FieStatusEffectsGrabEntity fieStatusEffectsGrabEntity = statusEffect as FieStatusEffectsGrabEntity;
					if (fieStatusEffectsGrabEntity != null)
					{
						FieStateMachinePoniesGrabbed fieStateMachinePoniesGrabbed = setStateToStatheMachine(typeof(FieStateMachinePoniesGrabbed), isForceSet: true, isDupulicate: true) as FieStateMachinePoniesGrabbed;
					}
				}
			}
		}

		private void ApplyStatusEffect_PoniesCommonRift(FieStatusEffectEntityBase statusEffectObject, FieGameCharacter attacker, FieDamage damage)
		{
			if (damage.statusEffects.Count > 0)
			{
				foreach (FieStatusEffectEntityBase statusEffect in damage.statusEffects)
				{
					FieStatusEffectsRiftEntity fieStatusEffectsRiftEntity = statusEffect as FieStatusEffectsRiftEntity;
					if (fieStatusEffectsRiftEntity != null && healthStats.stagger + damage.stagger >= healthStats.staggerResistance)
					{
						FieStateMachinePoniesRift fieStateMachinePoniesRift = setStateToStatheMachine(typeof(FieStateMachinePoniesRift), isForceSet: true, isDupulicate: true) as FieStateMachinePoniesRift;
						if (fieStateMachinePoniesRift != null)
						{
							fieStateMachinePoniesRift.ResetMoveForce(fieStatusEffectsRiftEntity.resetMoveForce);
							fieStateMachinePoniesRift.SetRiftForceRate(fieStatusEffectsRiftEntity.riftForceRate);
						}
						damage.stagger = 0f;
						healthStats.stagger = 0f;
					}
				}
			}
		}

		private void damageEventCallback(FieGameCharacter attacker, FieDamage damage)
		{
			SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_TOOK_DAMAGE), 75);
			if (healthStats.hitPoint <= healthStats.maxHitPoint * 0.5f && !_dialogIntervalTable[GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_PRIMARY_HP_LESS_THAN_HALF])
			{
				SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_PRIMARY_HP_LESS_THAN_HALF));
				_dialogIntervalTable[GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_PRIMARY_HP_LESS_THAN_HALF] = true;
			}
			else if (healthStats.shield <= 0f && !_dialogIntervalTable[GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_LOST_SECONDARY_HP])
			{
				SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_LOST_SECONDARY_HP));
				_dialogIntervalTable[GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_LOST_SECONDARY_HP] = true;
			}
		}

		private void dialogEndEventCallback(FieGameCharacter actor, GDEWordScriptsListData dialogData)
		{
			if (actor != null && actor.GetInstanceID() != GetInstanceID())
			{
				if (dialogData.Trigger.Key == GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_DEFEATED)
				{
					FieManagerBehaviour<FieDialogManager>.I.RequestDialog(this, LotteryWordScriptData(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_ALLY_DEFEATED)));
				}
				else if (dialogData.Trigger.Key == GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_ALLY_DEFEAT_ENEMY)
				{
					FieManagerBehaviour<FieDialogManager>.I.RequestDialog(this, LotteryWordScriptData(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_ALLY_DEFEAT_ENEMY)));
				}
			}
		}

		protected override void Start()
		{
			base.Start();
			base.detector.locatedEvent += Detector_locatedEvent;
			base.detector.missedEvent += Detector_missedEvent;
			base.emotionController.SetDefaultEmoteAnimationID(15);
			base.emotionController.RestoreEmotionFromDefaultData();
			base.animationManager.commonAnimationEvent += AnimationManager_commonAnimationEvent;
			if (FieBootstrap.isBootedFromBootStrap && base.photonView != null && !base.photonView.isMine)
			{
				InitializeIntelligenceSystem(IntelligenceType.Connection);
			}
			SceneManager.sceneLoaded += delegate
			{
				if (FieManagerFactory.I.currentSceneType == FieSceneType.INGAME)
				{
					FieManagerBehaviour<FieInGameStateManager>.I.RetryEvent += RetryEvent;
				}
			};
		}

		private void RetryEvent()
		{
			base.damageSystem.resetHealthSystem();
			if (base.photonView.isMine)
			{
				getStateMachine().SetStateDynamic(typeof(FieStateMachinePoniesIdle));
				getStateMachine(StateMachineType.Attack).SetStateDynamic(typeof(FieStateMachinePoniesAttackIdle));
				ReduceOrIncreaseScore(-Mathf.RoundToInt((float)base.score * 0.5f), isDefeater: false);
			}
			isEnableCollider = true;
			base.isEnableAutoFlip = true;
			base.isEnableGravity = true;
			setGravityRate(1f);
			isEnableHeadTracking = true;
			base.isSpeakable = true;
			base.friendshipStats.friendship = 60f;
			_latestFriendshipPoint = 2;
			base.abilitiesContainer.ResetAllCoolDown();
		}

		private void AnimationManager_commonAnimationEvent(TrackEntry entry)
		{
			if (entry != null)
			{
				entry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
				{
					if (e.Data.Name == "footstep" && base.currentFootstepMaterial != null)
					{
						base.currentFootstepMaterial.playFootstepAudio(base.footstepPlayer);
					}
				};
			}
		}

		private void Detector_missedEvent(FieGameCharacter targetCharacter)
		{
			if (base.isSpeakable && base.detector.lockonTargetObject == null)
			{
				base.emotionController.SetDefaultEmoteAnimationID(15);
				base.emotionController.RestoreEmotionFromDefaultData();
			}
		}

		private void Detector_locatedEvent(FieGameCharacter targetCharacter)
		{
			if (base.isSpeakable)
			{
				base.emotionController.SetDefaultEmoteAnimationID(16);
				base.emotionController.RestoreEmotionFromDefaultData();
			}
		}

		protected override void Update()
		{
			base.Update();
			if (healthStats.hitPoint >= healthStats.maxHitPoint)
			{
				_dialogIntervalTable[GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_PRIMARY_HP_LESS_THAN_HALF] = false;
			}
			if (healthStats.shield >= healthStats.maxShield)
			{
				_dialogIntervalTable[GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_LOST_SECONDARY_HP] = false;
			}
			int currentFriendshipPoint = base.friendshipStats.getCurrentFriendshipPoint();
			if (currentFriendshipPoint > _latestFriendshipPoint)
			{
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectPoniesGainedFriendship>(base.centerTransform, Vector3.forward).transform.rotation = Quaternion.identity;
			}
			_latestFriendshipPoint = currentFriendshipPoint;
			UpdateHeadTracking();
			UpdateSendingStateOfFriendship();
		}

		private void UpdateSendingStateOfFriendship()
		{
			if (_friendshipPointSendDelay > 0f)
			{
				_friendshipPointSendDelay -= Time.deltaTime;
			}
			else if (_stackedFriendshipPointForOtherPlayer > 0f)
			{
				FieManagerBehaviour<FieUserManager>.I.AddFriendshipPointToOtherPlayer(this, _stackedFriendshipPointForOtherPlayer);
				_stackedFriendshipPointForOtherPlayer = 0f;
				_friendshipPointSendDelay = 0.5f;
			}
		}

		private void UpdateHeadTracking()
		{
			if (isEnableHeadTracking)
			{
				_headBone.overrideAlpha = _headTrackingTweener.UpdateParameterFloat(Time.deltaTime);
				if (_headBone.overrideAlpha <= 0f && _headBone.mode == SkeletonUtilityBone.Mode.Override)
				{
					Transform transform = _headBone.transform;
					Vector3 localEulerAngles = _headBone.transform.localEulerAngles;
					transform.localEulerAngles = new Vector3(0f, 0f, localEulerAngles.z);
					_headBone.mode = SkeletonUtilityBone.Mode.Follow;
				}
				else if (_headBone.overrideAlpha > 0f && _headBone.mode == SkeletonUtilityBone.Mode.Follow)
				{
					_headBone.mode = SkeletonUtilityBone.Mode.Override;
				}
				if (_headTrackingTransform == null)
				{
					if (_headTrackingTweener.IsEnd())
					{
						_headTrackingTweener.InitTweener(0.3f, _headBone.overrideAlpha, 0f);
					}
				}
				else
				{
					Vector3 vector = _headBone.transform.position - _headTrackingTransform.position;
					vector.z = 0f;
					vector.Normalize();
					if (base.flipState == FieObjectFlipState.Right)
					{
						vector *= -1f;
					}
					Vector3 vector2 = Vector3.Cross(Vector3.up, vector) * -1f;
					if (vector2.z > 0.2f)
					{
						float angle = Vector3.Angle((base.flipState != 0) ? Vector3.up : Vector3.down, vector);
						_headBone.transform.rotation = Quaternion.AngleAxis((base.flipState != FieObjectFlipState.Right) ? 0f : 180f, Vector3.up) * Quaternion.AngleAxis(angle, Vector3.forward);
						Transform transform2 = _headBone.transform;
						Vector3 localEulerAngles2 = _headBone.transform.localEulerAngles;
						transform2.localEulerAngles = new Vector3(0f, 0f, localEulerAngles2.z);
						if (_headTrackingTweener.IsEnd())
						{
							_headTrackingTweener.InitTweener(0.3f, _headBone.overrideAlpha, 0.6f);
						}
					}
					else if (_headTrackingTweener.IsEnd())
					{
						_headTrackingTweener.InitTweener(0.3f, _headBone.overrideAlpha, 0f);
					}
				}
			}
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
			return new FieStateMachinePoniesBaseAttack();
		}

		protected void syncBindedAbilities()
		{
			Type ability = base.abilitiesContainer.getAbility(FieAbilitiesSlot.SlotType.SLOT_1);
			Type ability2 = base.abilitiesContainer.getAbility(FieAbilitiesSlot.SlotType.SLOT_2);
			Type ability3 = base.abilitiesContainer.getAbility(FieAbilitiesSlot.SlotType.SLOT_3);
			if (ability != null)
			{
				abstractStateList[typeof(FieStateMachinePoniesAbilitySlot1)] = ability;
			}
			if (ability != null)
			{
				abstractStateList[typeof(FieStateMachinePoniesAbilitySlot2)] = ability2;
			}
			if (ability != null)
			{
				abstractStateList[typeof(FieStateMachinePoniesAbilitySlot3)] = ability3;
			}
		}

		public FieStateMachineAbilityInterface getAbilityInstance(FieAbilitiesSlot.SlotType slotType)
		{
			Type ability = base.abilitiesContainer.getAbility(slotType);
			if (ability != null)
			{
				FieStateMachineAbilityInterface fieStateMachineAbilityInterface = Activator.CreateInstance(ability) as FieStateMachineAbilityInterface;
				if (fieStateMachineAbilityInterface != null)
				{
					return fieStateMachineAbilityInterface;
				}
			}
			return null;
		}

		public virtual List<Type> getStaggerCancelableStateList()
		{
			return _staggerCancelableStateList;
		}

		public void Revive(FieGameCharacter injured)
		{
			if (!(injured == null) && injured.damageSystem.isDead)
			{
				injured.setStateToStatheMachine(typeof(FieStateMachinePoniesRevive), isForceSet: true, isDupulicate: false);
			}
		}

		public void TryToRevive()
		{
			if (base.damageSystem.revivable)
			{
				if (base.friendshipStats.getCurrentFriendshipPoint() >= 3)
				{
					base.friendshipStats.addFriendshipPoint(-3);
					Revive(this);
				}
			}
			else
			{
				int layerMask = 16640;
				RaycastHit[] array = Physics.SphereCastAll(base.centerTransform.position, 1.5f, Vector3.down, 0.5f, layerMask);
				if (array != null && array.Length > 0)
				{
					RaycastHit[] array2 = array;
					int num = 0;
					FiePonies fiePonies;
					while (true)
					{
						if (num >= array2.Length)
						{
							return;
						}
						RaycastHit raycastHit = array2[num];
						FieCollider component = raycastHit.collider.gameObject.GetComponent<FieCollider>();
						if (component != null)
						{
							fiePonies = (component.getParentGameCharacter() as FiePonies);
							if (fiePonies != null && fiePonies.damageSystem.revivable)
							{
								break;
							}
						}
						num++;
					}
					if (base.friendshipStats.getCurrentFriendshipPoint() >= 2)
					{
						base.friendshipStats.addFriendshipPoint(-2);
						if (fiePonies.photonView == null || fiePonies.photonView.isMine)
						{
							Revive(fiePonies);
						}
						else
						{
							object[] parameters = new object[1]
							{
								base.photonView.viewID
							};
							fiePonies.photonView.RPC("ReviveRPC", PhotonTargets.Others, parameters);
						}
					}
				}
			}
		}

		[PunRPC]
		public void ReviveRPC(int reviverViewID)
		{
			if (base.photonView.isMine)
			{
				Revive(this);
			}
		}

		[PunRPC]
		public void AddFriendshipRPC(float friendship)
		{
			if (!(base.photonView != null) || base.photonView.isMine)
			{
				base.friendshipStats.safeAddFriendship(friendship, triggerEvent: false);
			}
		}

		public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			base.OnPhotonSerializeView(stream, info);
			if (stream.isWriting)
			{
				stream.SendNext(base.friendshipStats.friendship);
			}
			else
			{
				base.friendshipStats.friendship = (float)stream.ReceiveNext();
			}
		}

		protected override void initSkillTree()
		{
			GDEGameCharacterTypeData characterType = getCharacterTypeData();
			List<GDESkillTreeData> list = FieMasterData<GDESkillTreeData>.FindMasterDataList(delegate(GDESkillTreeData data)
			{
				if (characterType.Key != data.GameCharacterType.Key)
				{
					return false;
				}
				return true;
			});
		}

		public abstract Type getStormState();

		public abstract KeyValuePair<Type, string> getAbilitiesIconInfo();
	}
}
