using UnityEngine;

namespace Fie.Object
{
	public sealed class FieStatusEffectsTimeScaler : FieStatusEffectsBase
	{
		[SerializeField]
		private float _targetTimeScale = 1f;

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

		public float targetTimeScale
		{
			get
			{
				return _targetTimeScale;
			}
			set
			{
				_targetTimeScale = value;
				if (_entity != null)
				{
					(_entity as FieStatusEffectsTimeScalerEntity).targetTimeScale = _targetTimeScale;
				}
			}
		}

		public new void Awake()
		{
			_entity = new FieStatusEffectsTimeScalerEntity();
			base.Awake();
		}

		public override FieStatusEffectEntityBase GetStatusEffectEntity()
		{
			return _entity;
		}
	}
}
