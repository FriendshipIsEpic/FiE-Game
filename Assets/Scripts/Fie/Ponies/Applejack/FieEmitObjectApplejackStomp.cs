using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Power/ApplejackStomp")]
	public class FieEmitObjectApplejackStomp : FieEmittableObjectBase
	{
		[SerializeField]
		private AnimationCurve damageCurve;

		[SerializeField]
		private AnimationCurve damageCurveForPowerHitter;

		[SerializeField]
		private AnimationCurve damageCurveForAverageHitter;

		[SerializeField]
		private BoxCollider StompCollider;

		[SerializeField]
		private float stompDuration = 1f;

		[SerializeField]
		private float StompEnableDuration = 0.5f;

		[SerializeField]
		private float StompMaximumDamageRate = 1f;

		[SerializeField]
		private float StompMinimumDamageRate = 0.2f;

		[SerializeField]
		private Vector3 _defaultColliderSize = new Vector3(5f, 0.75f, 5f);

		private float _lifeTimeCount;

		private bool _isEndTrail;

		private bool _isEndUpdate;

		private AnimationCurve _currentDamageCurve;

		private BoxCollider _collider;

		public void Awake()
		{
			_collider = base.gameObject.GetComponent<BoxCollider>();
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= stompDuration)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
			}
		}

		public override void awakeEmitObject()
		{
			base.awakeEmitObject();
			_currentDamageCurve = damageCurve;
			if (base.ownerCharacter != null)
			{
				GDESkillTreeData skill = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.HONESTY_STOMP_LV4_POWER_HITTER);
				if (skill != null)
				{
					_currentDamageCurve = damageCurveForPowerHitter;
				}
				else
				{
					GDESkillTreeData skill2 = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.HONESTY_STOMP_LV4_AVERAGE_HITTER);
					if (skill2 != null)
					{
						_currentDamageCurve = damageCurveForAverageHitter;
					}
				}
				Vector3 defaultColliderSize = _defaultColliderSize;
				GDESkillTreeData skill3 = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.HONESTY_STOMP_LV2_2);
				if (skill3 != null)
				{
					defaultColliderSize.x += _defaultColliderSize.x * skill3.Value1;
					defaultColliderSize.z += _defaultColliderSize.z * skill3.Value1;
				}
				if (_collider != null)
				{
					_collider.size = defaultColliderSize;
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > StompEnableDuration) && collider.gameObject.tag == getHostileTagString())
			{
				FieDamage defaultDamageObject = getDefaultDamageObject();
				Vector3 size = StompCollider.size;
				float num = size.x * 0.5f;
				float a = Vector3.Distance(collider.gameObject.transform.position, base.transform.position);
				float num2 = _currentDamageCurve.Evaluate(Mathf.Clamp(num / Mathf.Max(a, 0.01f) / num, 0f, 1f));
				defaultDamageObject.damage *= num2;
				defaultDamageObject.stagger *= num2;
				FieGameCharacter x = addDamageToCollisionCharacter(collider, defaultDamageObject);
				if (x != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackHitEffectSmall>(base.transform, collider.ClosestPointOnBounds(base.transform.position));
					if (base.ownerCharacter != null && base.ownerCharacter.friendshipStats != null)
					{
						base.ownerCharacter.friendshipStats.safeAddFriendship(base.gainedFriendshipPoint * num2);
					}
				}
			}
		}
	}
}
