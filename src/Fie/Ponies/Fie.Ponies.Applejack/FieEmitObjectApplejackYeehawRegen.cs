using Fie.Object;
using GameDataEditor;
using ParticlePlayground;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Power/ApplejackYeehawRegen")]
	public class FieEmitObjectApplejackYeehawRegen : FieEmittableObjectBase
	{
		[SerializeField]
		private float yeehawRegenDuration = 13f;

		[SerializeField]
		private float yeehawRegenEnableDuration = 10f;

		[SerializeField]
		private PlaygroundParticlesC particle;

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		public override void awakeEmitObject()
		{
			particle.emit = true;
			if (base.ownerCharacter != null)
			{
				GDESkillTreeData skill = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.HONESTY_YEEHAW_LV4_SHOUT_OF_COURAGE);
				if (skill != null)
				{
					base.ownerCharacter.damageSystem.AddDefenceMagni(-1, skill.Value1, yeehawRegenEnableDuration);
				}
				GDESkillTreeData skill2 = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.HONESTY_YEEHAW_LV4_TAUNT);
				if (skill2 != null)
				{
					FieApplejack fieApplejack = base.ownerCharacter as FieApplejack;
					if (fieApplejack != null)
					{
						fieApplejack.isTauntMode = true;
					}
				}
			}
		}

		private void lifeSystem_damageEvent(FieGameCharacter attacker, FieDamage damage)
		{
			base.ownerCharacter.damageSystem.setRegenerateDelay(0f);
		}

		public void onDisable()
		{
			FieApplejack fieApplejack = base.ownerCharacter as FieApplejack;
			if (fieApplejack != null)
			{
				fieApplejack.isTauntMode = false;
			}
		}

		public void on()
		{
			FieApplejack fieApplejack = base.ownerCharacter as FieApplejack;
			if (fieApplejack != null)
			{
				fieApplejack.isTauntMode = false;
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
				base.ownerCharacter.damageSystem.setRegenerateDelay(0f);
				if (_lifeTimeCount >= yeehawRegenEnableDuration)
				{
					if (particle != null)
					{
						particle.emit = false;
					}
					FieApplejack fieApplejack = base.ownerCharacter as FieApplejack;
					if (fieApplejack != null)
					{
						fieApplejack.isTauntMode = false;
					}
					_isEndUpdate = true;
					destoryEmitObject(yeehawRegenDuration - yeehawRegenEnableDuration);
				}
			}
		}
	}
}
