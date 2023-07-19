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
	public class FieGameUIFriendshipGauge : FieGameUIBase
	{
		public enum FriendshipGaugeAnimationTrack
		{
			COLOR_AND_ALPHA,
			GAUGE
		}

		public FieGUIManager.FieUILayer currentLayer = FieGUIManager.FieUILayer.DEFAULT;

		protected const float GAUGE_ANIMATION_DURATION = 0.2f;

		protected const string GAUGE_ANIMATION = "gauge_animation";

		protected const string GAUGE_ANIMATION_OFF = "off";

		protected const string GAUGE_ANIMATION_ON = "on";

		protected SkeletonAnimation _skeletonAnimation;

		protected FieSkeletonAnimationController _animationManager;

		private float _currentFriendship;

		private float _maxFriendship;

		private float _animationFriendship;

		private bool _isEnd;

		private Tweener<TweenTypesOutSine> _gaugeTweener = new Tweener<TweenTypesOutSine>();

		private TrackEntry _gaugeAnimationEntry;

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
			_animationFriendship = _gaugeTweener.UpdateParameterFloat(Time.deltaTime);
			if (!_gaugeTweener.IsEnd())
			{
				updateGaugeState(_gaugeAnimationEntry);
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
				else if (base.ownerCharacter.friendshipStats.friendship != _currentFriendship)
				{
					_currentFriendship = base.ownerCharacter.friendshipStats.friendship;
					_gaugeTweener.InitTweener(0.2f, _animationFriendship, _currentFriendship);
				}
			}
		}

		public override void Initialize()
		{
			if (!(base.ownerCharacter == null))
			{
				TrackEntry trackEntry = _skeletonAnimation.state.SetAnimation(0, "on", loop: true);
				if (trackEntry != null)
				{
					trackEntry.timeScale = 1f;
				}
				_maxFriendship = base.ownerCharacter.friendshipStats.getMaxFriendship();
				_animationFriendship = (_currentFriendship = base.ownerCharacter.friendshipStats.friendship);
				_gaugeTweener.InitTweener(0.2f, _currentFriendship, _currentFriendship);
				initGaugeAnimation();
				updateGaugeState(_gaugeAnimationEntry);
				_isEnd = false;
			}
		}

		private void initGaugeAnimation()
		{
			_gaugeAnimationEntry = _skeletonAnimation.state.SetAnimation(1, "gauge_animation", loop: false);
		}

		public override void Terminate()
		{
			if (base.gameObject.activeSelf)
			{
				StartCoroutine(endAnimation());
			}
		}

		private void updateGaugeState(TrackEntry entry)
		{
			if (entry != null)
			{
				float num = _animationFriendship / _maxFriendship;
				float num2 = entry.endTime - 0.024f;
				entry.Time = Mathf.Max(Mathf.Min(num * num2, num2), 0f);
				entry.TimeScale = 0f;
			}
		}
	}
}
