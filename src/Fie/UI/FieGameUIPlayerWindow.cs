using Fie.Object;
using Fie.Ponies;
using Spine.Unity;
using System;
using TMPro;
using UnityEngine;

namespace Fie.UI
{
	[RequireComponent(typeof(SkeletonAnimation))]
	public class FieGameUIPlayerWindow : FieGameUIBase
	{
		public enum PlayerWindowAnimationTrack
		{
			CHARACTER_CHANGE,
			DAMAGE
		}

		private const string PLAYER_WINDOW_ANIMATION_DAMAGE = "damage";

		[SerializeField]
		private Transform _lifeGaugePositionTransform;

		[SerializeField]
		private Transform _frinedshipGaugePositionTransform;

		[SerializeField]
		private Transform _namePositionTransform;

		[SerializeField]
		private TextMeshPro _nameTextMesh;

		[SerializeField]
		private FieUIConstant2DText _levelText;

		private SkeletonAnimation _skeletonAnimation;

		public Transform lifeGaugePositionTransform => _lifeGaugePositionTransform;

		public Transform friendshipGaugePositionTransform => _frinedshipGaugePositionTransform;

		public Transform namePositionTransform => _namePositionTransform;

		public TextMeshPro nameTextMesh => _nameTextMesh;

		public FieUIConstant2DText levelText => _levelText;

		protected void Awake()
		{
			_skeletonAnimation = GetComponent<SkeletonAnimation>();
			if (_skeletonAnimation == null)
			{
				throw new Exception("this component require SkeletonAnimation. but didn't.");
			}
		}

		private void OnEnable()
		{
			Initialize();
		}

		private void OnDisable()
		{
			if (!(base.ownerCharacter == null))
			{
				FiePonies fiePonies = base.ownerCharacter as FiePonies;
				fiePonies.damageSystem.damagedEvent -= HealthSystem_damagedEvent;
			}
		}

		public override void Initialize()
		{
			if (!(base.ownerCharacter == null))
			{
				FiePonies fiePonies = base.ownerCharacter as FiePonies;
				if (!(fiePonies == null))
				{
					_skeletonAnimation.state.SetAnimation(0, fiePonies.getGameCharacterTypeData().Signature, loop: false);
					fiePonies.damageSystem.damagedEvent += HealthSystem_damagedEvent;
				}
			}
		}

		private void OnDestroy()
		{
			if (base.ownerCharacter != null)
			{
				base.ownerCharacter.damageSystem.damagedEvent -= HealthSystem_damagedEvent;
			}
		}

		private void HealthSystem_damagedEvent(FieGameCharacter attacker, FieDamage damage)
		{
			_skeletonAnimation.state.SetAnimation(1, "damage", loop: false);
		}
	}
}
