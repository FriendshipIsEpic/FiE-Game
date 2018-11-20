using Fie.Object;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashCloakEffect")]
	public class FieEmitObjectRainbowDashCloaking : FieEmittableObjectBase
	{
		private float cloakDuration = 3f;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		private FieRainbowDash ownerRainbowDash;

		private bool HealthSystem_damageCheckEvent(FieGameCharacter attacker, FieDamage damage)
		{
			return false;
		}

		public void Update()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= cloakDuration)
				{
					Disable();
				}
			}
		}

		public void Enable()
		{
			ownerRainbowDash = (base.ownerCharacter as FieRainbowDash);
			if (!(ownerRainbowDash == null) && base.ownerCharacter != null)
			{
				ownerRainbowDash.damageSystem.damageCheckEvent += HealthSystem_damageCheckEvent;
			}
		}

		public void Disable()
		{
			if (!_isEndUpdate)
			{
				if (base.ownerCharacter != null)
				{
					base.ownerCharacter.damageSystem.damageCheckEvent -= HealthSystem_damageCheckEvent;
				}
				if (ownerRainbowDash != null)
				{
					ownerRainbowDash.Decloack();
				}
				destoryEmitObject();
				_isEndUpdate = true;
			}
		}
	}
}
