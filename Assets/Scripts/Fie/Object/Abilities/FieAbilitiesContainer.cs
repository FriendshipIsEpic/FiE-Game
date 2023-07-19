using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Object.Abilities
{
	public class FieAbilitiesContainer
	{
		public delegate bool ActivateCheckCallback(FieGameCharacter gameCharacter);

		private FieGameCharacter ownerCharacter;

		private Dictionary<FieAbilitiesSlot.SlotType, Type> _abilitiesList = new Dictionary<FieAbilitiesSlot.SlotType, Type>();

		private Dictionary<Type, bool> _abilitiesTypes = new Dictionary<Type, bool>();

		private Dictionary<Type, FieStateMachineAbilityInterface> _abilitiesEntities = new Dictionary<Type, FieStateMachineAbilityInterface>();

		private Dictionary<Type, ActivateCheckCallback> _activationCheckCallbacks = new Dictionary<Type, ActivateCheckCallback>();

		private Dictionary<Type, FieAbilitiesCooldown> _abilitiesCooldownController = new Dictionary<Type, FieAbilitiesCooldown>();

		public FieAbilitiesContainer(FieGameCharacter owner)
		{
			ownerCharacter = owner;
		}

		public void UpdateCooldown(float deltaTime)
		{
			foreach (KeyValuePair<Type, FieAbilitiesCooldown> item in _abilitiesCooldownController)
			{
				item.Value.Update(deltaTime);
			}
		}

		public float GetCooltime<T>() where T : FieStateMachineAbilityInterface
		{
			if (!_abilitiesCooldownController.ContainsKey(typeof(T)))
			{
				return 3.40282347E+38f;
			}
			return _abilitiesCooldownController[typeof(T)].cooldown;
		}

		public float GetCooltime(FieAbilitiesSlot.SlotType slot)
		{
			if (!_abilitiesList.ContainsKey(slot))
			{
				return 3.40282347E+38f;
			}
			if (!_abilitiesCooldownController.ContainsKey(_abilitiesList[slot]))
			{
				return 3.40282347E+38f;
			}
			return _abilitiesCooldownController[_abilitiesList[slot]].cooldown;
		}

		public FieAbilitiesCooldown GetCooldownController(FieAbilitiesSlot.SlotType slot)
		{
			if (!_abilitiesList.ContainsKey(slot))
			{
				return null;
			}
			if (!_abilitiesCooldownController.ContainsKey(_abilitiesList[slot]))
			{
				return null;
			}
			return _abilitiesCooldownController[_abilitiesList[slot]];
		}

		public float SetCooldown<T>(float coolTime) where T : FieStateMachineAbilityBase
		{
			if (!_abilitiesCooldownController.ContainsKey(typeof(T)))
			{
				return 3.40282347E+38f;
			}
			if (ownerCharacter == null)
			{
				return 3.40282347E+38f;
			}
			float num = coolTime;
			if (ownerCharacter != null && ownerCharacter.unlockedSkills != null && ownerCharacter.unlockedSkills.Length > 0)
			{
				FieAbilityIDAttribute fieAbilityIDAttribute = (FieAbilityIDAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(FieAbilityIDAttribute));
				if (fieAbilityIDAttribute != null)
				{
					for (int i = 0; i < ownerCharacter.unlockedSkills.Length; i++)
					{
						if (ownerCharacter.unlockedSkills[i].Ability.ID == (int)fieAbilityIDAttribute.abilityID)
						{
							int iD = ownerCharacter.unlockedSkills[i].SkillType.ID;
							if (iD == 2)
							{
								num += coolTime * ownerCharacter.unlockedSkills[i].Value1;
							}
						}
					}
				}
			}
			num = Mathf.Max(num, 0f);
			float num2 = num;
			_abilitiesCooldownController[typeof(T)].cooldown = num2;
			return num2;
		}

		public void IncreaseOrReduceCooldownAll(float cooltime)
		{
			if (_abilitiesCooldownController != null && _abilitiesCooldownController.Count > 0)
			{
				foreach (KeyValuePair<Type, FieAbilitiesCooldown> item in _abilitiesCooldownController)
				{
					item.Value.cooldown = Mathf.Max(0f, item.Value.cooldown + cooltime);
				}
			}
		}

		public void AssignAbility(FieAbilitiesSlot.SlotType slot, FieStateMachineAbilityInterface abilityInstance, bool isForceAssign = false)
		{
			if (abilityInstance != null)
			{
				if (_abilitiesList.ContainsKey(slot) && isForceAssign)
				{
					_abilitiesList.Remove(slot);
				}
				_abilitiesList[slot] = abilityInstance.GetType();
				_abilitiesTypes[abilityInstance.GetType()] = true;
				_abilitiesEntities[abilityInstance.GetType()] = abilityInstance;
				_abilitiesCooldownController[abilityInstance.GetType()] = new FieAbilitiesCooldown();
			}
		}

		public Type getAbility(FieAbilitiesSlot.SlotType slot)
		{
			if (_abilitiesList.ContainsKey(slot))
			{
				return _abilitiesList[slot];
			}
			return null;
		}

		public bool isAbility(Type type)
		{
			if (_abilitiesTypes.ContainsKey(type))
			{
				return true;
			}
			return false;
		}

		public void setActivationCallback(Type type, ActivateCheckCallback callback)
		{
			if (_abilitiesTypes.ContainsKey(type))
			{
				_activationCheckCallbacks[type] = callback;
			}
		}

		public bool checkToActivate(FieGameCharacter gameCharacter, Type type)
		{
			if (gameCharacter == null || !_abilitiesEntities.ContainsKey(type))
			{
				return false;
			}
			switch (_abilitiesEntities[type].getActivationType())
			{
			case FieAbilityActivationType.COOLDOWN:
				return _abilitiesCooldownController[type].cooldown <= 0f;
			case FieAbilityActivationType.SPECEFIC_METHOD:
			{
				Type type2 = _abilitiesEntities[type].GetType();
				if (_activationCheckCallbacks.ContainsKey(type2))
				{
					return _activationCheckCallbacks[type2](gameCharacter);
				}
				break;
			}
			}
			return false;
		}

		internal void ResetAllCoolDown()
		{
			foreach (KeyValuePair<Type, FieAbilitiesCooldown> item in _abilitiesCooldownController)
			{
				item.Value.cooldown = 0f;
			}
		}
	}
}
