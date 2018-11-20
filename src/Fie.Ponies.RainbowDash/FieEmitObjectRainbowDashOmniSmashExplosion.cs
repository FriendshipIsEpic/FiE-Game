using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashOmniSmashExplosion")]
	public class FieEmitObjectRainbowDashOmniSmashExplosion : FieEmittableObjectBase
	{
		[SerializeField]
		private float DURATION = 1.5f;

		[SerializeField]
		private float DAMAGE_DELAY = 0.5f;

		[SerializeField]
		private float DAMAGE_DURATION = 1.2f;

		[SerializeField]
		private SphereCollider hitCollider;

		public float scale = 1f;

		private float _lifeTimeCount;

		private bool _isEndTrail;

		private bool _isEndUpdate;

		private float _scaleMagni = 1f;

		public override void awakeEmitObject()
		{
			base.awakeEmitObject();
			scale = 1f;
			hitCollider.enabled = false;
			_scaleMagni = 1f;
			FieRainbowDash fieRainbowDash = base.ownerCharacter as FieRainbowDash;
			if (fieRainbowDash != null)
			{
				GDESkillTreeData skill = fieRainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_OMNISMASH_LV3_2);
				if (skill != null)
				{
					_scaleMagni += skill.Value1;
				}
			}
		}

		public void SetScale(float newScale)
		{
			scale = newScale * _scaleMagni;
			hitCollider.gameObject.transform.localScale = Vector3.one * scale;
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
				if (_lifeTimeCount >= DURATION)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
				if (_lifeTimeCount >= DAMAGE_DELAY)
				{
					hitCollider.enabled = true;
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString() && !(base.ownerCharacter == null))
			{
				FieRainbowDash fieRainbowDash = base.ownerCharacter as FieRainbowDash;
				if (!(fieRainbowDash == null))
				{
					FieDamage defaultDamageObject = getDefaultDamageObject();
					defaultDamageObject.damage *= (float)fieRainbowDash.omniSmashAttackingCount;
					FieGameCharacter x = addDamageToCollisionCharacter(collider, defaultDamageObject);
					if (x != null)
					{
						FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_SMALL);
					}
				}
			}
		}
	}
}
