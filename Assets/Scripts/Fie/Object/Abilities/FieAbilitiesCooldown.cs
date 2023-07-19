using UnityEngine;

namespace Fie.Object.Abilities
{
	public class FieAbilitiesCooldown
	{
		public delegate void CooldownCompleteDelegate();

		public delegate void CooldownChangeDelegate(float before, float after);

		private bool _isEndCooldown = true;

		private float _cooldown;

		public float cooldown
		{
			get
			{
				return _cooldown;
			}
			set
			{
				float num = Mathf.Max(0f, value);
				if (_cooldown != value && this.cooldownChangeEvent != null)
				{
					this.cooldownChangeEvent(_cooldown, num);
				}
				_cooldown = num;
				if (_cooldown > 0f)
				{
					_isEndCooldown = false;
				}
			}
		}

		public bool canUseAbility => _cooldown <= 0f;

		public event CooldownCompleteDelegate cooldownCompleteEvent;

		public event CooldownChangeDelegate cooldownChangeEvent;

		public void Update(float time)
		{
			if (!_isEndCooldown)
			{
				_cooldown = Mathf.Max(_cooldown - time, 0f);
				if (_cooldown <= 0f)
				{
					if (this.cooldownCompleteEvent != null)
					{
						this.cooldownCompleteEvent();
					}
					_isEndCooldown = true;
				}
			}
		}
	}
}
