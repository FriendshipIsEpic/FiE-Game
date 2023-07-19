using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Power/ApplejackYeehaw")]
	public class FieEmitObjectApplejackYeehaw : FieEmittableObjectBase
	{
		[SerializeField]
		private float yeehawDuration = 1.5f;

		[SerializeField]
		private float yeehawEnableDuration = 0.8f;

		private float _lifeTimeCount;

		private bool _isEndTrail;

		private bool _isEndUpdate;

		private bool _isShoutOfCourage;

		public override void awakeEmitObject()
		{
			base.awakeEmitObject();
			if (base.ownerCharacter != null)
			{
				GDESkillTreeData skill = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.HONESTY_YEEHAW_LV4_SHOUT_OF_COURAGE);
				if (skill != null)
				{
					_isShoutOfCourage = true;
				}
			}
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
				if (_lifeTimeCount >= yeehawDuration)
				{
					_isEndUpdate = true;
					destoryEmitObject();
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > yeehawEnableDuration))
			{
				if (collider.gameObject.tag == getHostileTagString())
				{
					FieDamage damageObject = getDefaultDamageObject();
					FieApplejack fieApplejack = base.ownerCharacter as FieApplejack;
					if (fieApplejack != null)
					{
						fieApplejack.ApplyShoutAdditionalDamage(ref damageObject);
					}
					addDamageToCollisionCharacter(collider, damageObject);
				}
				else if (base.ownerCharacter != null && _isShoutOfCourage && collider.gameObject.tag == getAllyTagString())
				{
					FieCollider component = collider.gameObject.GetComponent<FieCollider>();
					if (!(component == null))
					{
						FieGameCharacter parentGameCharacter = component.getParentGameCharacter();
						if (!(parentGameCharacter == null))
						{
							int instanceID = parentGameCharacter.gameObject.GetInstanceID();
							if (instanceID != base.ownerCharacter.GetInstanceID())
							{
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackYeehawFriendly>(parentGameCharacter.centerTransform, Vector3.zero, null, parentGameCharacter);
							}
						}
					}
				}
			}
		}
	}
}
