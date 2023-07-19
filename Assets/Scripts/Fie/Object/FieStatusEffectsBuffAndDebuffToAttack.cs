using UnityEngine;

namespace Fie.Object
{
	public sealed class FieStatusEffectsBuffAndDebuffToAttack : FieStatusEffectsBase
	{
		[SerializeField]
		private int _skillID = -1;

		[SerializeField]
		private int _abilityID = -1;

		[SerializeField]
		private float _magni;

		[SerializeField]
		private bool _isEnableStack;

		public bool isActive
		{
			get
			{
				return _isActive;
			}
			set
			{
				_isActive = value;
				if (_entity != null)
				{
					_entity.isActive = _isActive;
				}
			}
		}

		public float duration
		{
			get
			{
				return _duration;
			}
			set
			{
				_duration = value;
				if (_entity != null)
				{
					_entity.duration = _duration;
				}
			}
		}

		public int skillID
		{
			get
			{
				return _skillID;
			}
			set
			{
				_skillID = value;
				if (_entity != null)
				{
					(_entity as FieStatusEffectsBuffAndDebuffToAttackEntity).skillID = _skillID;
				}
			}
		}

		public int abilityID
		{
			get
			{
				return _abilityID;
			}
			set
			{
				_abilityID = value;
				if (_entity != null)
				{
					(_entity as FieStatusEffectsBuffAndDebuffToAttackEntity).abilityID = _abilityID;
				}
			}
		}

		public float magni
		{
			get
			{
				return _magni;
			}
			set
			{
				_magni = value;
				if (_entity != null)
				{
					(_entity as FieStatusEffectsBuffAndDebuffToAttackEntity).magni = _magni;
				}
			}
		}

		public bool isEnableStack
		{
			get
			{
				return _isEnableStack;
			}
			set
			{
				_isEnableStack = value;
				if (_entity != null)
				{
					(_entity as FieStatusEffectsBuffAndDebuffToAttackEntity).isEnableStack = _isEnableStack;
				}
			}
		}

		public new void Awake()
		{
			_entity = new FieStatusEffectsBuffAndDebuffToAttackEntity();
			base.Awake();
		}

		public override FieStatusEffectEntityBase GetStatusEffectEntity()
		{
			return _entity;
		}
	}
}
