using Fie.Camera;
using Fie.Enemies;
using Fie.Manager;
using Fie.Object;
using Fie.Object.Abilities;
using Fie.Utility;
using ParticlePlayground;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;

namespace Fie.UI
{
	[RequireComponent(typeof(SkeletonAnimation))]
	public class FieGameUITargetIcon : FieGameUIBase
	{
		public enum CooldownGaugeAnimationTrack
		{
			TARGET,
			COOLDOWN_1,
			COOLDOWN_2,
			COOLDOWN_3
		}

		private struct CooldownParamters
		{
			public float currentCooldown;

			public float latestCooldown;

			public float maximumCoolown;

			public TrackEntry animationEntry;
		}

		public FieGUIManager.FieUILayer currentLayer;

		protected const string TARGET_ICON_ANIMATION_NOT_LOCATED = "idle";

		protected const string TARGET_ICON_ANIMATION_LOCATED = "located";

		protected const string TARGET_ICON_ANIMATION_LOCATED_FREELOCK = "located_freelock";

		protected const string TARGET_ICON_COOLDOWN_META_TAG = "[SLOT_NUM]";

		protected const string TARGET_ICON_ANIMATION_COOLDOWN = "cooldown_[SLOT_NUM]";

		protected const string TARGET_ICON_ANIMATION_COOLDOWN_IDLE = "cooldown_[SLOT_NUM]_idle";

		protected const string TARGET_ICON_ANIMATION_COOLDOWN_COMPLETE = "cooldown_[SLOT_NUM]_end";

		private const float TARGET_CHANGE_LEAP_TIME = 0.3f;

		protected SkeletonAnimation _skeletonAnimation;

		protected FieSkeletonAnimationController _animationManager;

		[SerializeField]
		public PlaygroundParticlesC targetChangeEffect;

		private FieObjectEnemies _currentMarkingTarget;

		private Transform _currentTransform;

		private Transform _currentMarkingTransform;

		private Transform _latestTransform;

		private Vector3 _latestPosition = Vector3.zero;

		private bool _isInitializePosition = true;

		private Tweener<TweenTypesInOutSine> _targetPositionTweener = new Tweener<TweenTypesInOutSine>();

		private CooldownParamters[] _cooldownParams = new CooldownParamters[3];

		private bool _isEnd;

		private bool _isLocated;

		private bool _isFreeCam;

		protected void Awake()
		{
			_skeletonAnimation = GetComponent<SkeletonAnimation>();
			if (_skeletonAnimation == null)
			{
				throw new Exception("this component require SkeletonAnimation. but didn't.");
			}
		}

		protected void Start()
		{
			Initialize();
		}

		private IEnumerator endAnimation(FieAbilitiesSlot.SlotType slot)
		{
			TrackEntry entry = null;
			switch (slot)
			{
			case FieAbilitiesSlot.SlotType.SLOT_1:
				entry = _skeletonAnimation.state.SetAnimation(1, "cooldown_[SLOT_NUM]_end".Replace("[SLOT_NUM]", "1"), loop: false);
				break;
			case FieAbilitiesSlot.SlotType.SLOT_2:
				entry = _skeletonAnimation.state.SetAnimation(2, "cooldown_[SLOT_NUM]_end".Replace("[SLOT_NUM]", "2"), loop: false);
				break;
			case FieAbilitiesSlot.SlotType.SLOT_3:
				entry = _skeletonAnimation.state.SetAnimation(3, "cooldown_[SLOT_NUM]_end".Replace("[SLOT_NUM]", "3"), loop: false);
				break;
			}
			if (entry == null)
			{
				yield return (object)null;
				/*Error: Unable to find new state assignment for yield return*/;
			}
			yield return (object)new WaitForSeconds(entry.endTime);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		protected void LateUpdate()
		{
			if (!(base.ownerCharacter == null))
			{
				bool flag = false;
				if (FieManagerBehaviour<FieGameCameraManager>.I.gameCamera != null)
				{
					FieGameCameraTaskLockOn fieGameCameraTaskLockOn = FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.GetCameraTask() as FieGameCameraTaskLockOn;
					if (fieGameCameraTaskLockOn != null)
					{
						flag = !fieGameCameraTaskLockOn.isCameraHorming;
					}
					if (_isFreeCam != flag)
					{
						_targetPositionTweener.InitTweener(0.3f, 0f, 1f);
						_latestPosition = base.transform.position;
						_isLocated = false;
					}
					_isFreeCam = flag;
				}
				Vector3 vector = Vector3.zero;
				if (_currentMarkingTransform != null)
				{
					if (!_isInitializePosition)
					{
						vector = ((!_isFreeCam) ? base.uiCamera.getPositionInUICameraWorld(_currentMarkingTransform.position) : base.uiCamera.camera.ScreenToWorldPoint(FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.tagetMakerScreenPos));
						if (!_isLocated)
						{
							if (!_isFreeCam)
							{
								_skeletonAnimation.state.SetAnimation(0, "located", loop: true);
							}
							else
							{
								_skeletonAnimation.state.SetAnimation(0, "located_freelock", loop: true);
							}
							_isLocated = true;
						}
					}
					else
					{
						vector = _currentMarkingTransform.position;
						if (_isLocated)
						{
							_skeletonAnimation.state.SetAnimation(0, "idle", loop: false);
							_isLocated = false;
						}
					}
				}
				if (_currentMarkingTarget != null)
				{
					_currentMarkingTarget.setHighLightColorByTargetStatus(base.ownerCharacter);
				}
				if (_isFreeCam)
				{
					if (Vector3.Distance(vector, base.transform.position) > 0.01f)
					{
						targetChangeEffect.emit = true;
					}
					else
					{
						targetChangeEffect.emit = false;
					}
				}
				else if (!_targetPositionTweener.IsEnd())
				{
					targetChangeEffect.emit = true;
				}
				else
				{
					targetChangeEffect.emit = false;
				}
				if (_currentMarkingTransform != null)
				{
					float t = _targetPositionTweener.UpdateParameterFloat(Time.deltaTime);
					base.transform.position = Vector3.Lerp(_latestPosition, vector, t);
				}
				for (int i = 0; i < 3; i++)
				{
					_cooldownParams[i].currentCooldown = base.ownerCharacter.abilitiesContainer.GetCooltime((FieAbilitiesSlot.SlotType)i);
					if (_cooldownParams[i].currentCooldown != _cooldownParams[i].latestCooldown)
					{
						updateGaugeState((FieAbilitiesSlot.SlotType)i);
					}
					else if (_cooldownParams[i].latestCooldown <= 0f)
					{
						continue;
					}
					if (_cooldownParams[i].currentCooldown <= 0f)
					{
						_cooldownParams[i].maximumCoolown = 0.1f;
						StartCoroutine(endAnimation((FieAbilitiesSlot.SlotType)i));
					}
					_cooldownParams[i].latestCooldown = _cooldownParams[i].currentCooldown;
				}
			}
		}

		public override void Initialize()
		{
			if (!(base.ownerCharacter == null))
			{
				base.ownerCharacter.detector.targetChangedEvent += detector_targetChangeEvent;
				_skeletonAnimation.state.SetAnimation(0, "idle", loop: false);
				_cooldownParams[0].animationEntry = _skeletonAnimation.state.SetAnimation(1, "cooldown_[SLOT_NUM]_idle".Replace("[SLOT_NUM]", "1"), loop: false);
				_cooldownParams[1].animationEntry = _skeletonAnimation.state.SetAnimation(2, "cooldown_[SLOT_NUM]_idle".Replace("[SLOT_NUM]", "2"), loop: false);
				_cooldownParams[2].animationEntry = _skeletonAnimation.state.SetAnimation(3, "cooldown_[SLOT_NUM]_idle".Replace("[SLOT_NUM]", "3"), loop: false);
				_isInitializePosition = true;
				_currentTransform = (_currentMarkingTransform = (_latestTransform = FieManagerBehaviour<FieGUIManager>.I.uiPositionList[FieGUIManager.FieUIPositionTag.ABILITY_ICON_3]));
				_latestPosition = _currentTransform.position;
				_targetPositionTweener.InitTweener(0.3f, 1f, 1f);
				FieAbilitiesCooldown cooldownController = base.ownerCharacter.abilitiesContainer.GetCooldownController(FieAbilitiesSlot.SlotType.SLOT_1);
				FieAbilitiesCooldown cooldownController2 = base.ownerCharacter.abilitiesContainer.GetCooldownController(FieAbilitiesSlot.SlotType.SLOT_2);
				FieAbilitiesCooldown cooldownController3 = base.ownerCharacter.abilitiesContainer.GetCooldownController(FieAbilitiesSlot.SlotType.SLOT_3);
				if (cooldownController != null)
				{
					cooldownController.cooldownChangeEvent += cooldownController_Ability1CooldownChangeEvent;
				}
				if (cooldownController2 != null)
				{
					cooldownController2.cooldownChangeEvent += cooldownController_Ability2CooldownChangeEvent;
				}
				if (cooldownController3 != null)
				{
					cooldownController3.cooldownChangeEvent += cooldownController_Ability3CooldownChangeEvent;
				}
				for (int i = 0; i < 3; i++)
				{
					_cooldownParams[i].currentCooldown = 0f;
					_cooldownParams[i].latestCooldown = 0f;
					_cooldownParams[i].maximumCoolown = 0f;
					updateGaugeState((FieAbilitiesSlot.SlotType)i);
				}
				if (targetChangeEffect != null)
				{
					targetChangeEffect.emit = false;
				}
				_isEnd = false;
			}
		}

		private void changeTarget(FieGameCharacter target)
		{
			_latestPosition = base.transform.position;
			if (target == null)
			{
				_currentTransform = (_currentMarkingTransform = FieManagerBehaviour<FieGUIManager>.I.uiPositionList[FieGUIManager.FieUIPositionTag.ABILITY_ICON_3]);
				_isInitializePosition = true;
			}
			else
			{
				_currentMarkingTransform = target.centerTransform;
				_currentMarkingTarget = (target as FieObjectEnemies);
				_currentTransform = target.transform;
				_isInitializePosition = false;
			}
			_targetPositionTweener.InitTweener(0.3f, 0f, 1f);
			_latestTransform = _currentTransform;
		}

		private void cooldownController_Ability1CooldownChangeEvent(float before, float after)
		{
			_cooldownParams[0].currentCooldown = after;
			_cooldownParams[0].latestCooldown = after;
			_cooldownParams[0].maximumCoolown = Mathf.Max(_cooldownParams[0].maximumCoolown, after);
			_cooldownParams[0].animationEntry = _skeletonAnimation.state.SetAnimation(1, "cooldown_[SLOT_NUM]".Replace("[SLOT_NUM]", "1"), loop: false);
		}

		private void cooldownController_Ability2CooldownChangeEvent(float before, float after)
		{
			_cooldownParams[1].currentCooldown = after;
			_cooldownParams[1].latestCooldown = after;
			_cooldownParams[1].maximumCoolown = Mathf.Max(_cooldownParams[1].maximumCoolown, after);
			_cooldownParams[1].animationEntry = _skeletonAnimation.state.SetAnimation(2, "cooldown_[SLOT_NUM]".Replace("[SLOT_NUM]", "2"), loop: false);
		}

		private void cooldownController_Ability3CooldownChangeEvent(float before, float after)
		{
			_cooldownParams[2].currentCooldown = after;
			_cooldownParams[2].latestCooldown = after;
			_cooldownParams[2].maximumCoolown = Mathf.Max(_cooldownParams[2].maximumCoolown, after);
			_cooldownParams[2].animationEntry = _skeletonAnimation.state.SetAnimation(3, "cooldown_[SLOT_NUM]".Replace("[SLOT_NUM]", "3"), loop: false);
		}

		private void OnDestroy()
		{
			if (!(base.ownerCharacter == null))
			{
				base.ownerCharacter.detector.targetChangedEvent -= detector_targetChangeEvent;
				FieAbilitiesCooldown cooldownController = base.ownerCharacter.abilitiesContainer.GetCooldownController(FieAbilitiesSlot.SlotType.SLOT_1);
				FieAbilitiesCooldown cooldownController2 = base.ownerCharacter.abilitiesContainer.GetCooldownController(FieAbilitiesSlot.SlotType.SLOT_2);
				FieAbilitiesCooldown cooldownController3 = base.ownerCharacter.abilitiesContainer.GetCooldownController(FieAbilitiesSlot.SlotType.SLOT_3);
				if (cooldownController != null)
				{
					cooldownController.cooldownChangeEvent -= cooldownController_Ability1CooldownChangeEvent;
				}
				if (cooldownController2 != null)
				{
					cooldownController2.cooldownChangeEvent -= cooldownController_Ability2CooldownChangeEvent;
				}
				if (cooldownController3 != null)
				{
					cooldownController3.cooldownChangeEvent -= cooldownController_Ability3CooldownChangeEvent;
				}
			}
		}

		private void updateGaugeState(FieAbilitiesSlot.SlotType slot)
		{
			if (_cooldownParams[(int)slot].animationEntry != null)
			{
				setGaugeAnimationFrame(_cooldownParams[(int)slot].animationEntry, _cooldownParams[(int)slot].latestCooldown, _cooldownParams[(int)slot].maximumCoolown);
			}
		}

		private void setGaugeAnimationFrame(TrackEntry entry, float latestCooldown, float maxCooldown)
		{
			float num = 1f;
			if (maxCooldown > 0f)
			{
				num = latestCooldown / maxCooldown;
			}
			entry.Time = Mathf.Max(Mathf.Min(entry.endTime - num * entry.endTime, entry.endTime), 0f);
			entry.TimeScale = 0f;
		}

		private void detector_targetChangeEvent(FieGameCharacter fromTargetCharacter, FieGameCharacter toTargetCharacter)
		{
			if (toTargetCharacter != null && !toTargetCharacter.damageSystem.isDead)
			{
				changeTarget(toTargetCharacter);
			}
			else
			{
				changeTarget(null);
			}
			FieObjectEnemies fieObjectEnemies = fromTargetCharacter as FieObjectEnemies;
			FieObjectEnemies fieObjectEnemies2 = toTargetCharacter as FieObjectEnemies;
			if (fieObjectEnemies != null)
			{
				fieObjectEnemies.isEnableHighLight = false;
			}
			if (fieObjectEnemies2 != null)
			{
				fieObjectEnemies2.isEnableHighLight = true;
				fieObjectEnemies2.setHighLightColorByTargetStatus(base.ownerCharacter);
			}
		}
	}
}
