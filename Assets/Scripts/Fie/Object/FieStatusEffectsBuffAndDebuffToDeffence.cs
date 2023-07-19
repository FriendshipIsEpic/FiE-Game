using UnityEngine;

namespace Fie.Object
{
	public sealed class FieStatusEffectsBuffAndDebuffToDeffence : FieStatusEffectsBase
	{
		[SerializeField]
		public int _skillID = -1;

		[SerializeField]
		public float _magni;

		[SerializeField]
		public bool _isEnableStack;

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
					(_entity as FieStatusEffectsBuffAndDebuffToDeffenceEntity).skillID = _skillID;
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
					(_entity as FieStatusEffectsBuffAndDebuffToDeffenceEntity).magni = _magni;
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
					(_entity as FieStatusEffectsBuffAndDebuffToDeffenceEntity).isEnableStack = _isEnableStack;
				}
			}
		}

		public new void Awake()
		{
			_entity = new FieStatusEffectsBuffAndDebuffToDeffenceEntity();
			base.Awake();
		}

		public override FieStatusEffectEntityBase GetStatusEffectEntity()
		{
			return _entity;
		}
	}
}
