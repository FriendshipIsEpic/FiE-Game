using Fie.Object;
using Fie.Object.Abilities;
using Spine.Unity;
using System;
using UnityEngine;

namespace Fie.UI
{
	[RequireComponent(typeof(SkeletonAnimation))]
	public class FieGameUIAbilitiesIconBase : FieGameUIBase
	{
		public enum AbilitiesIconAnimationTrack
		{
			SET_ICON,
			ACTIVATE,
			ENABLE_OR_DISABLE
		}

		private const string ABILITYES_ICON_SET_ANIMATION_PREFIX = "ability_";

		private const string ABILITYES_ICON_ACTIVATE_ANIMATION = "activate";

		private const string ABILITYES_ICON_DISABLE_ANIMATION = "disable";

		private const string ABILITYES_ICON_ENABLE_ANIMATION = "enable";

		[SerializeField]
		private TextMesh _cooldownCounter;

		private FieAbilitiesSlot.SlotType _slotID;

		private FieAbilitiesCooldown _cooldownController;

		private SkeletonAnimation _skeletonAnimation;

		private FieStateMachineAbilityInterface _abilityInstance;

		private bool _isEnable = true;

		public FieStateMachineAbilityInterface abilityInstance
		{
			set
			{
				_abilityInstance = value;
				SetAbilityIcon(_abilityInstance);
			}
		}

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

		private void LateUpdate()
		{
			if (!(base.ownerCharacter == null) && _cooldownController != null)
			{
				if (_cooldownController.cooldown > 0f)
				{
					if (_cooldownController.cooldown >= 10f)
					{
						_cooldownCounter.text = $"{_cooldownController.cooldown:f0}";
					}
					else
					{
						_cooldownCounter.text = $"{_cooldownController.cooldown:f1}";
					}
					if (_isEnable)
					{
						_skeletonAnimation.state.SetAnimation(2, "disable", loop: false);
						_isEnable = false;
					}
				}
				else if (!_isEnable)
				{
					_cooldownCounter.text = string.Empty;
					_skeletonAnimation.state.SetAnimation(2, "enable", loop: false);
					_isEnable = true;
				}
			}
		}

		public override void Initialize()
		{
			if (!(base.ownerCharacter == null))
			{
				base.ownerCharacter.abilityActivateEvent += ownerPony_abilityActivateEvent;
			}
		}

		private void OnDestroy()
		{
			if (!(base.ownerCharacter == null))
			{
				base.ownerCharacter.abilityActivateEvent -= ownerPony_abilityActivateEvent;
			}
		}

		internal void SetSlot(FieAbilitiesSlot.SlotType slotID)
		{
			_slotID = slotID;
			_cooldownController = base.ownerCharacter.abilitiesContainer.GetCooldownController(slotID);
		}

		private void SetAbilityIcon(FieStateMachineAbilityInterface abilityInstance)
		{
			if (abilityInstance != null)
			{
				_skeletonAnimation.state.SetAnimation(0, "ability_" + abilityInstance.getSignature(), loop: false);
			}
		}

		private void ownerPony_abilityActivateEvent(Type abilityType)
		{
			if (_abilityInstance.GetType() == abilityType)
			{
				_skeletonAnimation.state.SetAnimation(1, "activate", loop: false);
			}
		}
	}
}
