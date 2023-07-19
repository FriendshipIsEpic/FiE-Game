using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;

namespace Fie.UI
{
	[RequireComponent(typeof(SkeletonAnimation))]
	public class FieGameUILifeGaugeBase : FieGameUIBase
	{
		public enum LifeGaugeAnimationTrack
		{
			COLOR_AND_ALPHA,
			DAMAGE,
			HEALTH,
			SHIELD
		}

		public FieGUIManager.FieUILayer currentLayer = FieGUIManager.FieUILayer.DEFAULT;

		public bool isTrackingCharacterPosition = true;

		public bool isEnableAutoDamageAnimation = true;

		protected const float GAUGE_ANIMATION_DURATION = 0.2f;

		protected const string GAUGE_ANIMATION_DAMAGE_HEALTH = "damage";

		protected const string GAUGE_ANIMATION_DAMAGE_SHIELD = "damage";

		protected const string GAUGE_ANIMATION_EARTH = "earth";

		protected const string GAUGE_ANIMATION_WING = "wing";

		protected const string GAUGE_ANIMATION_MAGIC = "magic";

		protected const string GAUGE_ANIMATION_HEALTH = "health";

		protected const string GAUGE_ANIMATION_OFF = "off";

		protected const string GAUGE_ANIMATION_ON = "on";

		protected SkeletonAnimation _skeletonAnimation;

		protected FieSkeletonAnimationController _animationManager;

		private float _currentHealth;

		private float _currentShield;

		private float _maxHealth;

		private float _maxShield;

		private float _animationHealth;

		private float _animationShield;

		private bool _isEnd;

		private FieAttribute _shieldType;

		private Tweener<TweenTypesOutSine> _healthTweener = new Tweener<TweenTypesOutSine>();

		private Tweener<TweenTypesOutSine> _shieldTweener = new Tweener<TweenTypesOutSine>();

		private TrackEntry _shieldAnimationEntry;

		private TrackEntry _healthAnimationEntry;

		protected void Awake()
		{
			_skeletonAnimation = GetComponent<SkeletonAnimation>();
			if (_skeletonAnimation == null)
			{
				throw new Exception("this component require SkeletonAnimation. but didn't.");
			}
		}

		protected void OnEnable()
		{
			Initialize();
		}

		private IEnumerator endAnimation()
		{
			yield return (object)new WaitForSeconds(0.2f);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		protected void LateUpdate()
		{
			_animationHealth = _healthTweener.UpdateParameterFloat(Time.deltaTime);
			_animationShield = _shieldTweener.UpdateParameterFloat(Time.deltaTime);
			if (!_healthTweener.IsEnd() || !_shieldTweener.IsEnd())
			{
				updateGaugeState();
			}
			if (!(base.uiCamera == null))
			{
				if (base.ownerCharacter == null)
				{
					if (!_isEnd)
					{
						Terminate();
						_isEnd = true;
					}
				}
				else
				{
					if (isTrackingCharacterPosition)
					{
						base.transform.position = base.uiCamera.getPositionInUICameraWorld(base.ownerCharacter.guiPointTransform.position);
						SetUILayer(currentLayer);
					}
					if (base.ownerCharacter.healthStats.hitPoint != _currentHealth || base.ownerCharacter.healthStats.shield != _currentShield)
					{
						if (isEnableAutoDamageAnimation)
						{
							if (!(base.ownerCharacter.healthStats.shield > _currentShield) && base.ownerCharacter.healthStats.shield < _currentShield)
							{
								_skeletonAnimation.state.SetAnimation(1, "damage", loop: false);
							}
							if (!(base.ownerCharacter.healthStats.hitPoint > _currentHealth) && base.ownerCharacter.healthStats.hitPoint < _currentHealth)
							{
								_skeletonAnimation.state.SetAnimation(1, "damage", loop: false);
							}
						}
						_currentHealth = base.ownerCharacter.healthStats.hitPoint;
						_currentShield = base.ownerCharacter.healthStats.shield;
						_healthTweener.InitTweener(0.2f, _animationHealth, _currentHealth);
						_shieldTweener.InitTweener(0.2f, _animationShield, _currentShield);
					}
				}
			}
		}

		public override void Initialize()
		{
			if (!(base.ownerCharacter == null))
			{
				TrackEntry trackEntry = _skeletonAnimation.state.SetAnimation(0, "on", loop: false);
				if (trackEntry != null)
				{
					trackEntry.timeScale = 1f;
				}
				_maxHealth = base.ownerCharacter.healthStats.maxHitPoint;
				_maxShield = base.ownerCharacter.healthStats.maxShield;
				_animationHealth = (_currentHealth = base.ownerCharacter.healthStats.hitPoint);
				_animationShield = (_currentShield = base.ownerCharacter.healthStats.shield);
				_healthTweener.InitTweener(0.2f, _currentHealth, _currentHealth);
				_shieldTweener.InitTweener(0.2f, _currentShield, _currentShield);
				_shieldType = base.ownerCharacter.healthStats.shieldType;
				initGaugeAnimation();
				updateGaugeState();
				_isEnd = false;
			}
		}

		private void initGaugeAnimation()
		{
			string animationName = null;
			switch (_shieldType)
			{
			case FieAttribute.EARTH:
				animationName = "earth";
				break;
			case FieAttribute.WING:
				animationName = "wing";
				break;
			case FieAttribute.MAGIC:
				animationName = "magic";
				break;
			}
			_shieldAnimationEntry = _skeletonAnimation.state.SetAnimation(3, animationName, loop: false);
			_healthAnimationEntry = _skeletonAnimation.state.SetAnimation(2, "health", loop: false);
		}

		public override void Terminate()
		{
			_healthTweener.InitTweener(0.2f, _animationHealth, 0f);
			_shieldTweener.InitTweener(0.2f, _animationShield, 0f);
			if (base.gameObject.activeSelf)
			{
				StartCoroutine(endAnimation());
			}
		}

		private void updateGaugeState()
		{
			if (_shieldAnimationEntry != null)
			{
				setGaugeAnimationFrame(_shieldAnimationEntry, LifeGaugeAnimationTrack.SHIELD);
			}
			if (_healthAnimationEntry != null)
			{
				setGaugeAnimationFrame(_healthAnimationEntry, LifeGaugeAnimationTrack.HEALTH);
			}
		}

		private void setGaugeAnimationFrame(TrackEntry entry, LifeGaugeAnimationTrack trackType)
		{
			if (trackType == LifeGaugeAnimationTrack.HEALTH || trackType == LifeGaugeAnimationTrack.SHIELD)
			{
				float num = 1f;
				if (trackType == LifeGaugeAnimationTrack.HEALTH)
				{
					if (_maxHealth > 0f)
					{
						num = _animationHealth / _maxHealth;
					}
				}
				else if (_maxShield > 0f)
				{
					num = _animationShield / _maxShield;
				}
				float num2 = entry.endTime - 0.0333f;
				entry.Time = Mathf.Max(Mathf.Min(num2 - num * num2, num2), 0f);
				entry.TimeScale = 0f;
			}
		}
	}
}
