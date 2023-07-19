using Fie.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Object
{
	public class FieDamageSystem : MonoBehaviour
	{
		public struct DamageMagniContainer
		{
			public int abilityID;

			public float magni;

			public DamageMagniContainer(int abilityID, float magni)
			{
				this.abilityID = abilityID;
				this.magni = magni;
			}
		}

		public delegate void ReviveDelegate();

		public delegate void DeathDelegate(FieGameCharacter killer, FieDamage damage);

		public delegate void StaggerDelegate(FieDamage damage);

		public delegate bool DamageCheckDelegate(FieGameCharacter attacker, FieDamage damage);

		public delegate void BeforeDamageDelegate(FieGameCharacter attacker, ref FieDamage damage);

		public delegate void DamageDelegate(FieGameCharacter attacker, FieDamage damage);

		public delegate void StatusEffectDelegate(FieStatusEffectEntityBase statusEffect, FieGameCharacter attacker, FieDamage damage);

		public const float HITPOINT_GATE_RATE = 0.05f;

		public const float HITPOINT_GATE_DELAY = 12f;

		public const float HITPOINT_GATE_IMMUNITY_SEC = 0.3f;

		public const float SHIELD_GATE_DELAY = 8f;

		public const float DEFAULT_DYING_SEC = 12f;

		public const float DEFAULT_REVIVE_SEC = 3f;

		private Dictionary<Type, StatusEffectDelegate> _statusEffectCallbacks = new Dictionary<Type, StatusEffectDelegate>();

		private FieGameCharacter _ownerCharacter;

		private FieGameCharacter _latestPerpetrator;

		private FieDamage _latestDamage;

		private FieHealthStats _healthStats = new FieHealthStats();

		private FieHealthStats _healthStatsSandbox = new FieHealthStats();

		private bool _isDead;

		private float _dyingCount;

		private float _reviveCount;

		private float _currentRegenerateDelay;

		private float _hitPointGateDelay;

		private float _hitPointGateImunitySec;

		private float _shieldGateDelay;

		private bool _isEnableRegenerate = true;

		private bool _isEnableHitPointGate = true;

		private bool _isEnableShieldGate = true;

		private bool _isEnableHealthImmunity;

		private bool _isEnableStaggerImmunity;

		private bool _revivable;

		private float _dyingNeedSec = 12f;

		private float _reviveNeedSec = 3f;

		private Dictionary<int, Queue<DamageMagniContainer>> _attackMagniStack = new Dictionary<int, Queue<DamageMagniContainer>>();

		private Dictionary<int, Queue<float>> _defenceMagniStack = new Dictionary<int, Queue<float>>();

		private Dictionary<FieGameCharacter, float> _takenDamages = new Dictionary<FieGameCharacter, float>();

		private FieHealthStats healthStats
		{
			get
			{
				if (_ownerCharacter == null || _ownerCharacter.photonView == null || !_ownerCharacter.photonView.isMine)
				{
					return _healthStatsSandbox;
				}
				return _healthStats;
			}
		}

		public bool isEnableRegenerate
		{
			get
			{
				return _isEnableRegenerate;
			}
			set
			{
				_isEnableRegenerate = value;
			}
		}

		public bool isEnableHitPointGate
		{
			get
			{
				return _isEnableHitPointGate;
			}
			set
			{
				_isEnableHitPointGate = value;
			}
		}

		public bool isEnableShieldGate
		{
			get
			{
				return _isEnableShieldGate;
			}
			set
			{
				_isEnableShieldGate = value;
			}
		}

		public bool isEnableHealthImmunity
		{
			get
			{
				return _isEnableHealthImmunity;
			}
			set
			{
				_isEnableHealthImmunity = value;
			}
		}

		public bool isEnableStaggerImmunity
		{
			get
			{
				return _isEnableStaggerImmunity;
			}
			set
			{
				_isEnableStaggerImmunity = value;
			}
		}

		public float dyingNeedSec
		{
			get
			{
				return _dyingNeedSec;
			}
			set
			{
				_dyingNeedSec = Mathf.Max(value, 0f);
			}
		}

		public float reviveNeedSec
		{
			get
			{
				return _reviveNeedSec;
			}
			set
			{
				_reviveNeedSec = Mathf.Max(value, 0f);
			}
		}

		public bool isDying
		{
			get
			{
				if (healthStats == null)
				{
					return true;
				}
				return healthStats.hitPoint <= 0f;
			}
		}

		public bool isDead => _isDead;

		public bool revivable
		{
			get
			{
				return _revivable;
			}
			set
			{
				if (_revivable != value)
				{
					_reviveCount = 0f;
					_revivable = value;
				}
			}
		}

		public Dictionary<FieGameCharacter, float> takenDamage => _takenDamages;

		public event DeathDelegate deathEvent;

		public event ReviveDelegate reviveEvent;

		public event StaggerDelegate staggerEvent;

		public event DamageCheckDelegate damageCheckEvent;

		public event BeforeDamageDelegate beforeDamageEvent;

		public event DamageDelegate damagedEvent;

		private IEnumerator addAttackMagniStackCoroutine(int skillID, int abilityID, float magni, float duration, bool isEnableStack = false)
		{
			if (!_attackMagniStack.ContainsKey(skillID) || _attackMagniStack[skillID] == null)
			{
				_attackMagniStack[skillID] = new Queue<DamageMagniContainer>();
			}
			int stackedCount = _attackMagniStack[skillID].Count;
			if (stackedCount > 0 && !isEnableStack)
			{
				if (stackedCount == 1)
				{
					_attackMagniStack[skillID].Dequeue();
				}
				else
				{
					_attackMagniStack[skillID].Clear();
				}
			}
			_attackMagniStack[skillID].Enqueue(new DamageMagniContainer(abilityID, magni));
			yield return (object)new WaitForSeconds(duration);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		private IEnumerator addDefenceMagniStackCoroutine(int skillID, float magni, float duration, bool isEnableStack = false)
		{
			if (!_defenceMagniStack.ContainsKey(skillID) || _defenceMagniStack[skillID] == null)
			{
				_defenceMagniStack[skillID] = new Queue<float>();
			}
			int stackedCount = _defenceMagniStack[skillID].Count;
			if (stackedCount > 0 && !isEnableStack)
			{
				if (stackedCount == 1)
				{
					_defenceMagniStack[skillID].Dequeue();
				}
				else
				{
					_defenceMagniStack[skillID].Clear();
				}
			}
			_defenceMagniStack[skillID].Enqueue(magni);
			yield return (object)new WaitForSeconds(duration);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public void AddAttackMagni(int skillID, float magni, float duration, int abilityID = -1, bool isEnableStack = false)
		{
			StartCoroutine(addAttackMagniStackCoroutine(skillID, abilityID, magni, duration, isEnableStack));
		}

		public void AddDefenceMagni(int skillID, float magni, float duration, bool isEnableStack = false)
		{
			StartCoroutine(addDefenceMagniStackCoroutine(skillID, magni, duration, isEnableStack));
		}

		public float GetAttackMagni(int abilityID = -1)
		{
			float num = 0f;
			if (_attackMagniStack == null || _attackMagniStack.Count <= 0)
			{
				return num;
			}
			foreach (KeyValuePair<int, Queue<DamageMagniContainer>> item in _attackMagniStack)
			{
				if (item.Value != null && item.Value.Count > 0)
				{
					foreach (DamageMagniContainer item2 in item.Value)
					{
						DamageMagniContainer current2 = item2;
						if (current2.abilityID == abilityID || current2.abilityID == -1)
						{
							num += current2.magni;
						}
					}
				}
			}
			return num;
		}

		public float GetDeffenceMagni()
		{
			float num = 0f;
			if (_defenceMagniStack == null || _defenceMagniStack.Count <= 0)
			{
				return num;
			}
			foreach (KeyValuePair<int, Queue<float>> item in _defenceMagniStack)
			{
				if (item.Value != null && item.Value.Count > 0)
				{
					foreach (float item2 in item.Value)
					{
						num += item2;
					}
				}
			}
			return num;
		}

		public void Regen(float regenPoint)
		{
			if (healthStats.shield <= 0f)
			{
				float num = Mathf.Clamp(healthStats.hitPoint + regenPoint, 0f, healthStats.maxHitPoint);
				regenPoint -= num - healthStats.hitPoint;
				healthStats.hitPoint = num;
			}
			healthStats.shield = Mathf.Clamp(healthStats.shield + regenPoint, 0f, healthStats.maxShield);
		}

		public void initHealthSystem(FieGameCharacter ownerCharacter, ref FieHealthStats parameters)
		{
			_ownerCharacter = ownerCharacter;
			_healthStats = parameters;
			resetHealthSystem();
		}

		public void resetHealthSystem()
		{
			healthStats.hitPoint = healthStats.maxHitPoint;
			healthStats.shield = healthStats.maxShield;
			_isEnableRegenerate = true;
			_isEnableHitPointGate = true;
			_isEnableShieldGate = true;
			_isEnableHealthImmunity = false;
			_isEnableStaggerImmunity = false;
			_revivable = false;
			_isDead = false;
			_dyingCount = 0f;
			_reviveCount = 0f;
			_currentRegenerateDelay = 0f;
			_hitPointGateDelay = 0f;
			_hitPointGateImunitySec = 0f;
			_shieldGateDelay = 0f;
			_dyingNeedSec = 12f;
			_reviveNeedSec = 3f;
		}

		public void calcHitPoitDirect(float additionalHitpoint)
		{
			if (!(_ownerCharacter.photonView != null) || _ownerCharacter.photonView.isMine)
			{
				healthStats.hitPoint += additionalHitpoint;
				healthStats.hitPoint = Mathf.Max(Mathf.Min(healthStats.hitPoint, healthStats.maxHitPoint), 0f);
			}
		}

		public void calcShieldDirect(float additionalShield)
		{
			if (!(_ownerCharacter.photonView != null) || _ownerCharacter.photonView.isMine)
			{
				healthStats.shield += additionalShield;
				healthStats.shield = Mathf.Max(Mathf.Min(healthStats.shield, healthStats.maxShield), 0f);
			}
		}

		public void setRegenerateDelay(float delayTime, bool roundToBigger = false)
		{
			if (!roundToBigger || !(delayTime < _currentRegenerateDelay))
			{
				_currentRegenerateDelay = Mathf.Max(delayTime, 0f);
			}
		}

		public void ResetStaggerEvent()
		{
			this.staggerEvent = null;
		}

		public void updateHealthSystem(float time)
		{
			if (isDying)
			{
				if (!_isDead)
				{
					_isDead = true;
					if (this.deathEvent != null)
					{
						this.deathEvent(_latestPerpetrator, _latestDamage);
					}
				}
			}
			else
			{
				if (healthStats.stagger > 0f)
				{
					healthStats.stagger -= healthStats.staggerResistance * healthStats.staggerAttenuationPerSec * time;
					healthStats.stagger = Mathf.Max(healthStats.stagger, 0f);
				}
				if (_hitPointGateImunitySec > 0f)
				{
					_hitPointGateImunitySec -= time;
				}
				if (_hitPointGateDelay > 0f)
				{
					_hitPointGateDelay -= time;
				}
				if (_shieldGateDelay > 0f)
				{
					_shieldGateDelay -= time;
				}
				if (_currentRegenerateDelay > 0f)
				{
					_currentRegenerateDelay -= time;
				}
				else if (_isEnableRegenerate)
				{
					if (healthStats.hitPoint < healthStats.maxHitPoint)
					{
						healthStats.hitPoint += healthStats.maxHitPoint * healthStats.hitPointRegeneratePerSec * time;
					}
					if (healthStats.hitPoint >= healthStats.maxHitPoint && healthStats.shield < healthStats.maxShield)
					{
						healthStats.shield += healthStats.maxShield * healthStats.shieldRegeneratePerSec * time;
					}
					healthStats.hitPoint = Mathf.Min(healthStats.hitPoint, healthStats.maxHitPoint);
					healthStats.shield = Mathf.Min(healthStats.shield, healthStats.maxShield);
				}
			}
		}

		public FieDamageSystem addDamage(FieGameCharacter attacker, FieDamage damageObject, bool isPenetration = false)
		{
			if (damageObject == null)
			{
				return this;
			}
			bool flag = false;
			if (this.damageCheckEvent != null && !this.damageCheckEvent(attacker, damageObject))
			{
				return this;
			}
			if (damageObject.statusEffects != null)
			{
				foreach (FieStatusEffectEntityBase statusEffect in damageObject.statusEffects)
				{
					flag |= statusEffect.ApplyStatusEffect(this, attacker, damageObject);
				}
			}
			if (flag)
			{
				return this;
			}
			damageObject.damage *= Mathf.Max(0f, 1f - GetDeffenceMagni());
			if (this.beforeDamageEvent != null)
			{
				this.beforeDamageEvent(attacker, ref damageObject);
			}
			healthStats.stagger += damageObject.stagger;
			if (_healthStats.stagger >= _healthStats.staggerResistance && !isEnableStaggerImmunity)
			{
				if (this.staggerEvent != null)
				{
					this.staggerEvent(damageObject);
				}
				healthStats.stagger = 0f;
			}
			if (damageObject.damage > 0f && !isEnableHealthImmunity)
			{
				float num = damageObject.damage + damageObject.damage * (((UnityEngine.Random.Range(0, 100) < 50) ? (-1f) : 1f) * UnityEngine.Random.Range(0f, damageObject.fluctuatingRate));
				damageObject.finallyDamage = 0f;
				bool flag2 = false;
				if (_healthStats.shield > 0f)
				{
					float num2 = 1f;
					switch (damageObject.attribute)
					{
					case FieAttribute.MAGIC:
						if (_healthStats.shieldType == FieAttribute.WING)
						{
							num2 += _healthStats.weakAttributeDamageMagnify;
							damageObject.attributeDamageState = FieDamage.FieAttributeDamageState.EFFECTIVE;
						}
						else if (_healthStats.shieldType == FieAttribute.EARTH)
						{
							num2 += _healthStats.strongAttributeDamageMagnify;
							damageObject.attributeDamageState = FieDamage.FieAttributeDamageState.NONEFFECTIVE;
						}
						break;
					case FieAttribute.WING:
						if (_healthStats.shieldType == FieAttribute.EARTH)
						{
							num2 += _healthStats.weakAttributeDamageMagnify;
							damageObject.attributeDamageState = FieDamage.FieAttributeDamageState.EFFECTIVE;
						}
						else if (_healthStats.shieldType == FieAttribute.MAGIC)
						{
							num2 += _healthStats.strongAttributeDamageMagnify;
							damageObject.attributeDamageState = FieDamage.FieAttributeDamageState.NONEFFECTIVE;
						}
						break;
					case FieAttribute.EARTH:
						if (_healthStats.shieldType == FieAttribute.MAGIC)
						{
							damageObject.attributeDamageState = FieDamage.FieAttributeDamageState.EFFECTIVE;
							num2 += _healthStats.weakAttributeDamageMagnify;
						}
						else if (_healthStats.shieldType == FieAttribute.WING)
						{
							num2 += _healthStats.strongAttributeDamageMagnify;
							damageObject.attributeDamageState = FieDamage.FieAttributeDamageState.NONEFFECTIVE;
						}
						break;
					}
					num = damageObject.damage * num2;
					float num3 = Mathf.Max(_healthStats.shield - num, 0f);
					if (num3 <= 0f)
					{
						num = Mathf.Abs(_healthStats.shield - num);
						flag2 = true;
					}
					if (_isEnableShieldGate && _shieldGateDelay <= 0f && !isPenetration && FieManagerBehaviour<FieEnvironmentManager>.I.currentDifficulty < FieEnvironmentManager.Difficulty.NIGHTMARE)
					{
						flag2 = false;
						if (num3 <= 0f)
						{
							_shieldGateDelay = 8f;
						}
					}
					float shield = _healthStats.shield;
					healthStats.shield = num3;
					damageObject.finallyDamage += shield - healthStats.shield;
				}
				else
				{
					flag2 = true;
				}
				if (flag2 && num > 0f)
				{
					float hitPoint = _healthStats.hitPoint;
					float num4 = hitPoint;
					if (_isEnableHitPointGate && !isPenetration && FieManagerBehaviour<FieEnvironmentManager>.I.currentDifficulty != FieEnvironmentManager.Difficulty.CHAOS)
					{
						if (_hitPointGateDelay <= 0f && _healthStats.hitPoint - num <= _healthStats.maxHitPoint * 0.05f)
						{
							_hitPointGateImunitySec = 0.3f;
							_hitPointGateDelay = 12f;
						}
						float b = (!(_hitPointGateImunitySec > 0f)) ? 0f : (_healthStats.maxHitPoint * 0.05f);
						healthStats.hitPoint = Mathf.Max(_healthStats.hitPoint - num, b);
						num4 = healthStats.hitPoint;
					}
					else
					{
						num4 = _healthStats.hitPoint - num;
						healthStats.hitPoint = Mathf.Max(_healthStats.hitPoint - num, 0f);
					}
					damageObject.finallyDamage += hitPoint - num4;
				}
				_currentRegenerateDelay = _healthStats.regenerateDelay;
			}
			if (this.damagedEvent != null)
			{
				this.damagedEvent(attacker, damageObject);
			}
			if (attacker != null && damageObject.finallyDamage > 0f)
			{
				_latestPerpetrator = attacker;
				if (!_takenDamages.ContainsKey(attacker))
				{
					_takenDamages[attacker] = 0f;
				}
				Dictionary<FieGameCharacter, float> takenDamages;
				FieGameCharacter key;
				(takenDamages = _takenDamages)[key = attacker] = takenDamages[key] + damageObject.finallyDamage;
			}
			if (damageObject != null)
			{
				_latestDamage = damageObject;
			}
			return this;
		}

		public void addStatusEffectCallback<T>(StatusEffectDelegate callback) where T : FieStatusEffectEntityBase
		{
			_statusEffectCallbacks[typeof(T)] = callback;
		}

		public void applyStatusEffectCallback<T>(T statusEffectObject, FieGameCharacter attacker, FieDamage damage) where T : FieStatusEffectEntityBase
		{
			if (statusEffectObject != null)
			{
				Type type = statusEffectObject.GetType();
				if (_statusEffectCallbacks.ContainsKey(type) && _statusEffectCallbacks[type] != null)
				{
					_statusEffectCallbacks[type](statusEffectObject, attacker, damage);
				}
			}
		}

		public void Heal(float healingRate)
		{
		}

		private void initializeByRevive()
		{
			healthStats.hitPoint = healthStats.maxHitPoint;
			healthStats.shield = 0f;
			_isDead = false;
			_revivable = false;
			_reviveCount = 0f;
			_dyingCount = 0f;
			_currentRegenerateDelay = healthStats.regenerateDelay;
			_hitPointGateDelay = 0f;
			_hitPointGateImunitySec = 0f;
		}
	}
}
