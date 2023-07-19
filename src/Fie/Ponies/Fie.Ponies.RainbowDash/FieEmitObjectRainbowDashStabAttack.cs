using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using PigeonCoopToolkit.Effects.Trails;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashStabAttack")]
	public class FieEmitObjectRainbowDashStabAttack : FieEmittableObjectBase
	{
		[SerializeField]
		private float DURATION = 0.3f;

		[SerializeField]
		private float TRAIL_DURATION = 0.4f;

		[SerializeField]
		private float DAMAGE_DURATION = 0.2f;

		[Range(0f, 4f)]
		[SerializeField]
		private int GAIND_AWESOME_COUNT = 1;

		[SerializeField]
		private float RAINBOW_DASH_STAB_ATTACK_PULL_FORCE = 8f;

		[SerializeField]
		private SmoothTrail _airKickTrail;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		public override void awakeEmitObject()
		{
			_airKickTrail.Emit = true;
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= DURATION)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
				if (_lifeTimeCount >= TRAIL_DURATION)
				{
					_airKickTrail.Emit = false;
				}
				if (initTransform != null)
				{
					base.transform.position = initTransform.position;
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
			{
				FieRainbowDash fieRainbowDash = base.ownerCharacter as FieRainbowDash;
				if (!(fieRainbowDash == null))
				{
					FieDamage defaultDamageObject = getDefaultDamageObject();
					float damage = defaultDamageObject.damage;
					float num = defaultDamageObject.damage;
					bool flag = false;
					FieCollider component = collider.gameObject.GetComponent<FieCollider>();
					if (!(component == null))
					{
						FieGameCharacter parentGameCharacter = component.getParentGameCharacter();
						if (parentGameCharacter != null && Vector3.Dot(fieRainbowDash.flipDirectionVector, parentGameCharacter.flipDirectionVector) > 0f)
						{
							flag = true;
						}
						GDESkillTreeData skill = fieRainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_RAINBLOW_LV1_1);
						if (skill != null)
						{
							num += damage * skill.Value1;
						}
						GDESkillTreeData skill2 = fieRainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_RAINBLOW_LV3_1);
						if (skill2 != null)
						{
							num += damage * skill2.Value1;
						}
						if (flag)
						{
							defaultDamageObject.damage = num * 3f;
						}
						FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, defaultDamageObject);
						if (fieGameCharacter != null)
						{
							FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashHitEffectStab>(base.transform, collider.ClosestPointOnBounds(base.transform.position));
							FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_MIDDLE);
							if (base.ownerCharacter != null && fieRainbowDash != null)
							{
								fieRainbowDash.requestSetAwesomeEffect(GAIND_AWESOME_COUNT);
							}
							fieGameCharacter.setMoveForce(directionalVec * (RAINBOW_DASH_STAB_ATTACK_PULL_FORCE * -1f), 0f, useRound: false);
						}
					}
				}
			}
		}
	}
}
