using UnityEngine;

namespace Fie.Object
{
	public class FieStatusEffectsBase : MonoBehaviour
	{
		[SerializeField]
		protected bool _isActive = true;

		[SerializeField]
		protected bool _isOnlyStatusEffect;

		[SerializeField]
		protected float _duration;

		protected FieStatusEffectEntityBase _entity;

		protected void Awake()
		{
			FieEmittableObjectBase component = GetComponent<FieEmittableObjectBase>();
			if (!(component == null))
			{
				if (_entity == null)
				{
					_entity = GetStatusEffectEntity();
				}
				_entity.isOnlyStatusEffect = _isOnlyStatusEffect;
				_entity.duration = _duration;
				component.AddStatusEffect(_entity);
			}
		}

		public virtual FieStatusEffectEntityBase GetStatusEffectEntity()
		{
			return null;
		}
	}
}
