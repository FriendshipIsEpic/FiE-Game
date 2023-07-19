using Fie.Object;

namespace Fie.UI
{
	public class FieGameUIPlayerLifeGauge : FieGameUILifeGaugeBase
	{
		public new void OnEnable()
		{
			base.OnEnable();
			if (base.ownerCharacter != null)
			{
				base.ownerCharacter.damageSystem.damagedEvent += lifeSystem_damageEvent;
			}
			isEnableAutoDamageAnimation = false;
		}

		private void OnDisable()
		{
			if (base.ownerCharacter != null)
			{
				base.ownerCharacter.damageSystem.damagedEvent -= lifeSystem_damageEvent;
			}
		}

		private void lifeSystem_damageEvent(FieGameCharacter attacker, FieDamage damage)
		{
			_skeletonAnimation.state.SetAnimation(1, "damage", loop: false);
		}
	}
}
