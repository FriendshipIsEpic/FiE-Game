using Fie.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Fie.Object
{
	[FiePrefabInfo("")]
	public abstract class FieEmittableObjectBase : MonoBehaviour
	{
		public delegate void FieEmittableObjectCallback(FieEmittableObjectBase emitObject);

		public delegate void FieEmittableObjectReclectCallback();

		private class FieEmittableObjectInitParam<T>
		{
			public FieldInfo info;

			public T param;

			public FieEmittableObjectInitParam(FieldInfo setInfo, T setParam)
			{
				info = setInfo;
				param = setParam;
			}
		}

		public enum EmitObjectType
		{
			NONEFFECTIVE,
			RANGED_ATTACK,
			RANGED_ATTACK_REFLECTIVE,
			MELEE
		}

		public enum EmitObjectTag
		{
			PLAYER,
			ENEMY,
			EMIT_OBJECT_TAG_MAX
		}

		public enum EmitObjectEventList
		{
			HIT,
			DESTROY
		}

		public enum HittingType
		{
			NON_HIT,
			ONETIME_PER_LIFE,
			ONETIME_PER_ENEMY,
			INTERVAL
		}

		public enum FriendshipGainedType
		{
			NONE,
			HITTING,
			OTHER
		}

		public delegate void EmitObjectDestroyDelegate(FieEmittableObjectBase emitObject);

		public delegate void EmitObjectHitDelegate(FieEmittableObjectBase emitObject, FieGameCharacter hitCharacter);

		private delegate void InitializeDelegate();

		private static string[] _tagTypeToStringList = new string[2]
		{
			"Player",
			"Enemy"
		};

		[NonSerialized]
		private FieGameCharacter _ownerCharacter;

		[SerializeField]
		private EmitObjectType _emitObjectType;

		[SerializeField]
		private EmitObjectTag _defaultAllyTag;

		[SerializeField]
		private EmitObjectTag _defaultHostileTag = EmitObjectTag.ENEMY;

		[SerializeField]
		private HittingType _hittingType;

		[SerializeField]
		private float _hitInterval = 1f;

		[SerializeField]
		private FriendshipGainedType _friendshipGainedType;

		[SerializeField]
		private float _gainedFriendshipPoint;

		public FieAttribute damageAttribute;

		public float defaultDamage;

		public float defaultStagger;

		public float defaultHate;

		public float defaultGainedFriendshipPoint;

		public float baseDamage;

		public float baseStagger;

		public float baseHate;

		public float baseGainedFriendshipPoint;

		[Range(0f, 100f)]
		public float damageFluctuatingRate = 0.05f;

		[SerializeField]
		public FieConstValues.FieAbility ability = FieConstValues.FieAbility.NONE;

		private EmitObjectType _initEmitObjectType;

		private EmitObjectTag _initDefaultAllyTag;

		private EmitObjectTag _initDefaultHostileTag = EmitObjectTag.ENEMY;

		private FieAttribute _initDamageAttribute;

		private bool _isEnableCollision;

		private float _initDefaultDamage;

		private float _initDefaultStagger;

		private float _initDefaultHate;

		private float _initDefaultGainedFriendshipPoint;

		private EmitObjectTag nowAllyTag;

		private EmitObjectTag nowHostileTag;

		private bool _isDestroyed;

		protected Vector3 directionalVec = Vector3.zero;

		protected Transform initTransform;

		protected Transform targetTransform;

		protected Dictionary<int, float> _hitList = new Dictionary<int, float>();

		private InitializeDelegate initializeDelegate;

		private List<FieStatusEffectEntityBase> _statusEffects = new List<FieStatusEffectEntityBase>();

		public FieGameCharacter ownerCharacter
		{
			get
			{
				return _ownerCharacter;
			}
			set
			{
				_ownerCharacter = value;
				ChangeTagInfoByOwnerCharacter(_ownerCharacter);
			}
		}

		public EmitObjectType emitObjectType
		{
			get
			{
				return _emitObjectType;
			}
			set
			{
				_emitObjectType = value;
			}
		}

		public float HitInterval => _hitInterval;

		public float gainedFriendshipPoint
		{
			get
			{
				float num = defaultGainedFriendshipPoint;
				if (ownerCharacter != null && ownerCharacter.unlockedSkills != null && ownerCharacter.unlockedSkills.Length > 0 && ability != FieConstValues.FieAbility.NONE)
				{
					for (int i = 0; i < ownerCharacter.unlockedSkills.Length; i++)
					{
						if (ownerCharacter.unlockedSkills[i].Ability.ID == (int)ability)
						{
							int iD = ownerCharacter.unlockedSkills[i].SkillType.ID;
							if (iD == 5)
							{
								num += defaultGainedFriendshipPoint * ownerCharacter.unlockedSkills[i].Value1;
							}
						}
					}
				}
				return num;
			}
		}

		public bool isDestroyed => _isDestroyed;

		public event FieEmittableObjectCallback awakeningEvent;

		public event FieEmittableObjectReclectCallback reflectEvent;

		public event EmitObjectDestroyDelegate emitObjectDestoryEvent;

		public event EmitObjectHitDelegate emitObjectHitEvent;

		private IEnumerator SetActiveWithDelay(bool activeState, float delay)
		{
			yield return (object)new WaitForSeconds(delay);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public void Initialize(Transform initTransform, Vector3 directionalVec)
		{
			Initialize(initTransform, directionalVec, null);
		}

		public void Initialize(Transform initTransform, Vector3 directionalVec, Transform targetTransform)
		{
			if (initializeDelegate != null)
			{
				initializeDelegate();
			}
			this.initTransform = initTransform;
			this.directionalVec = directionalVec;
			this.targetTransform = targetTransform;
			nowAllyTag = _defaultAllyTag;
			nowHostileTag = _defaultHostileTag;
			_isDestroyed = false;
			base.transform.position = this.initTransform.position;
			if (_ownerCharacter != null)
			{
				ChangeTagInfoByOwnerCharacter(_ownerCharacter);
			}
			if (this.awakeningEvent != null)
			{
				this.awakeningEvent(this);
			}
		}

		public FieDamage getDefaultDamageObject()
		{
			FieDamage fieDamage = new FieDamage(damageAttribute, defaultDamage, defaultStagger, defaultHate, damageFluctuatingRate, _statusEffects);
			if (ownerCharacter != null && ownerCharacter.unlockedSkills != null && ownerCharacter.unlockedSkills.Length > 0 && ability != FieConstValues.FieAbility.NONE)
			{
				for (int i = 0; i < ownerCharacter.unlockedSkills.Length; i++)
				{
					if (ownerCharacter.unlockedSkills[i].Ability.ID == (int)ability)
					{
						switch (ownerCharacter.unlockedSkills[i].SkillType.ID)
						{
						case 1:
							fieDamage.damage += defaultDamage * ownerCharacter.unlockedSkills[i].Value1;
							break;
						case 4:
							fieDamage.hate += defaultHate * ownerCharacter.unlockedSkills[i].Value1;
							break;
						case 3:
							fieDamage.stagger += defaultStagger * ownerCharacter.unlockedSkills[i].Value1;
							break;
						}
					}
				}
			}
			if (ownerCharacter != null && ownerCharacter.damageSystem != null)
			{
				fieDamage.damage *= Mathf.Max(0f, 1f + ownerCharacter.damageSystem.GetAttackMagni((int)ability));
			}
			if (ownerCharacter != null && ownerCharacter.forces == EmitObjectTag.ENEMY)
			{
				fieDamage.damage *= FieManagerBehaviour<FieEnvironmentManager>.I.currentEnemyDamageMagnify;
			}
			return fieDamage;
		}

		protected FieGameCharacter addDamageToCharacter(FieGameCharacter gameCharacter, FieDamage damageObject, bool isPenetration = false)
		{
			if (gameCharacter == null)
			{
				return null;
			}
			int instanceID = gameCharacter.GetInstanceID();
			switch (_hittingType)
			{
			case HittingType.ONETIME_PER_LIFE:
				if (_hitList.Count > 0)
				{
					return null;
				}
				break;
			case HittingType.ONETIME_PER_ENEMY:
				if (_hitList.ContainsKey(instanceID))
				{
					return null;
				}
				_hitList[instanceID] = Time.time;
				break;
			case HittingType.INTERVAL:
				if (_hitList.ContainsKey(instanceID) && _hitList[instanceID] + _hitInterval > Time.time)
				{
					return null;
				}
				break;
			}
			_hitList[instanceID] = Time.time;
			float gainedFriendshipPoint = this.gainedFriendshipPoint;
			if (gainedFriendshipPoint > 0f && ownerCharacter != null)
			{
				FriendshipGainedType friendshipGainedType = _friendshipGainedType;
				if (friendshipGainedType == FriendshipGainedType.HITTING)
				{
					ownerCharacter.friendshipStats.safeAddFriendship(gainedFriendshipPoint);
				}
			}
			gameCharacter.AddDamage(ownerCharacter, damageObject, isPenetration);
			return gameCharacter;
		}

		protected FieGameCharacter addDamageToCollisionCharacter(Collider collider, FieDamage damageObject, bool isPenetration = false)
		{
			if (!_isEnableCollision)
			{
				return null;
			}
			if (damageObject == null)
			{
				return null;
			}
			FieCollider component = collider.gameObject.GetComponent<FieCollider>();
			if (component == null)
			{
				return null;
			}
			FieGameCharacter parentGameCharacter = component.getParentGameCharacter();
			if (parentGameCharacter == null)
			{
				return null;
			}
			parentGameCharacter = addDamageToCharacter(parentGameCharacter, damageObject, isPenetration);
			if (parentGameCharacter == null)
			{
				return null;
			}
			parentGameCharacter.latestDamageWorldPoint = collider.ClosestPointOnBounds(base.transform.position);
			return parentGameCharacter;
		}

		protected void callHitEvent(FieGameCharacter hitCharacter)
		{
			if (this.emitObjectHitEvent != null)
			{
				this.emitObjectHitEvent(this, hitCharacter);
			}
		}

		protected void destoryEmitObject()
		{
			destoryEmitObject(0f);
		}

		protected void destoryEmitObject(float duration)
		{
			if (!_isDestroyed)
			{
				if (this.emitObjectDestoryEvent != null)
				{
					this.emitObjectDestoryEvent(this);
				}
				StartCoroutine(SetActiveWithDelay(activeState: false, duration));
				_isDestroyed = true;
			}
		}

		public string getAllyTagString()
		{
			return _tagTypeToStringList[(int)nowAllyTag];
		}

		public string getHostileTagString()
		{
			return _tagTypeToStringList[(int)nowHostileTag];
		}

		public void setAllyTag(EmitObjectTag setTag)
		{
			nowAllyTag = setTag;
		}

		public void setHostileTag(EmitObjectTag setTag)
		{
			nowHostileTag = setTag;
		}

		public EmitObjectTag getArrayTag()
		{
			return nowAllyTag;
		}

		public EmitObjectTag getHostileTag()
		{
			return nowHostileTag;
		}

		public void setInitTransform(Transform transform)
		{
			initTransform = transform;
		}

		public void setTargetTransform(Transform transform)
		{
			targetTransform = transform;
		}

		public void setDirectionalVector(Vector3 vector)
		{
			directionalVec = vector;
		}

		public Vector3 getDirectionalVector()
		{
			return directionalVec;
		}

		private void ChangeTagInfoByOwnerCharacter(FieGameCharacter ownerCharacter)
		{
			if (!(ownerCharacter == null) && !(ownerCharacter.detector == null))
			{
				nowAllyTag = ((!(ownerCharacter.detector.friendTag == "Player")) ? EmitObjectTag.ENEMY : EmitObjectTag.PLAYER);
				nowHostileTag = ((ownerCharacter.detector.enemyTag == "Enemy") ? EmitObjectTag.ENEMY : EmitObjectTag.PLAYER);
			}
		}

		protected bool reflectEmitObject(FieEmittableObjectBase emitObject, EmitObjectType reflectObjectType = EmitObjectType.RANGED_ATTACK_REFLECTIVE)
		{
			if (emitObject.getHostileTagString() == getAllyTagString() && emitObject.emitObjectType == reflectObjectType)
			{
				emitObject.setAllyTag(getArrayTag());
				emitObject.setHostileTag(getHostileTag());
				if (emitObject.ownerCharacter != null)
				{
					Vector3 directionalVector = emitObject.ownerCharacter.centerTransform.position - emitObject.transform.position;
					directionalVector.Normalize();
					emitObject.setTargetTransform(emitObject.ownerCharacter.centerTransform);
					emitObject.setDirectionalVector(directionalVector);
					emitObject.ownerCharacter = ownerCharacter;
				}
				else
				{
					emitObject.setDirectionalVector(emitObject.getDirectionalVector() * -1f);
				}
				emitObject.InvokeReflectedEvent();
				return true;
			}
			return false;
		}

		private void InvokeReflectedEvent()
		{
			if (this.reflectEvent != null)
			{
				this.reflectEvent();
			}
		}

		public void InitializeInitializer<T>() where T : FieEmittableObjectBase
		{
			List<FieEmittableObjectInitParam<int>> intInitializer = new List<FieEmittableObjectInitParam<int>>();
			List<FieEmittableObjectInitParam<string>> stringInitializer = new List<FieEmittableObjectInitParam<string>>();
			List<FieEmittableObjectInitParam<bool>> boolInitializer = new List<FieEmittableObjectInitParam<bool>>();
			List<FieEmittableObjectInitParam<float>> floatInitializer = new List<FieEmittableObjectInitParam<float>>();
			List<FieEmittableObjectInitParam<Vector3>> vector3Initializer = new List<FieEmittableObjectInitParam<Vector3>>();
			List<FieEmittableObjectInitParam<Quaternion>> quaternionInitializer = new List<FieEmittableObjectInitParam<Quaternion>>();
			FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				if (fieldInfo.FieldType == typeof(int))
				{
					intInitializer.Add(new FieEmittableObjectInitParam<int>(fieldInfo, (int)fieldInfo.GetValue(this)));
				}
				else if (fieldInfo.FieldType == typeof(string))
				{
					stringInitializer.Add(new FieEmittableObjectInitParam<string>(fieldInfo, (string)fieldInfo.GetValue(this)));
				}
				else if (fieldInfo.FieldType == typeof(bool))
				{
					boolInitializer.Add(new FieEmittableObjectInitParam<bool>(fieldInfo, (bool)fieldInfo.GetValue(this)));
				}
				else if (fieldInfo.FieldType == typeof(float))
				{
					floatInitializer.Add(new FieEmittableObjectInitParam<float>(fieldInfo, (float)fieldInfo.GetValue(this)));
				}
				else if (fieldInfo.FieldType == typeof(Vector3))
				{
					vector3Initializer.Add(new FieEmittableObjectInitParam<Vector3>(fieldInfo, (Vector3)fieldInfo.GetValue(this)));
				}
				else if (fieldInfo.FieldType == typeof(Quaternion))
				{
					quaternionInitializer.Add(new FieEmittableObjectInitParam<Quaternion>(fieldInfo, (Quaternion)fieldInfo.GetValue(this)));
				}
			}
			_initEmitObjectType = _emitObjectType;
			_initDefaultAllyTag = _defaultAllyTag;
			_initDefaultHostileTag = _defaultHostileTag;
			_initDamageAttribute = damageAttribute;
			_isEnableCollision = false;
			_initDefaultDamage = defaultDamage;
			_initDefaultStagger = defaultStagger;
			_initDefaultHate = defaultHate;
			_initDefaultGainedFriendshipPoint = _gainedFriendshipPoint;
			initializeDelegate = delegate
			{
				initTransform = null;
				targetTransform = null;
				this.emitObjectDestoryEvent = null;
				this.emitObjectHitEvent = null;
				foreach (FieEmittableObjectInitParam<int> item in intInitializer)
				{
					item.info.SetValue(this, item.param);
				}
				foreach (FieEmittableObjectInitParam<string> item2 in stringInitializer)
				{
					item2.info.SetValue(this, item2.param);
				}
				foreach (FieEmittableObjectInitParam<bool> item3 in boolInitializer)
				{
					item3.info.SetValue(this, item3.param);
				}
				foreach (FieEmittableObjectInitParam<float> item4 in floatInitializer)
				{
					item4.info.SetValue(this, item4.param);
				}
				foreach (FieEmittableObjectInitParam<Vector3> item5 in vector3Initializer)
				{
					item5.info.SetValue(this, item5.param);
				}
				foreach (FieEmittableObjectInitParam<Quaternion> item6 in quaternionInitializer)
				{
					item6.info.SetValue(this, item6.param);
				}
				_hitList = new Dictionary<int, float>();
				_emitObjectType = _initEmitObjectType;
				_defaultAllyTag = _initDefaultAllyTag;
				_defaultHostileTag = _initDefaultHostileTag;
				damageAttribute = _initDamageAttribute;
				_isEnableCollision = (_hittingType != HittingType.NON_HIT);
				defaultDamage = (baseDamage = _initDefaultDamage);
				defaultStagger = (baseStagger = _initDefaultStagger);
				defaultHate = (baseHate = _initDefaultHate);
				defaultGainedFriendshipPoint = (baseGainedFriendshipPoint = _initDefaultGainedFriendshipPoint);
			};
		}

		public void AddStatusEffect(FieStatusEffectEntityBase statusEffectEntity)
		{
			if (statusEffectEntity != null)
			{
				_statusEffects.Add(statusEffectEntity);
			}
		}

		public virtual void awakeEmitObject()
		{
		}
	}
}
