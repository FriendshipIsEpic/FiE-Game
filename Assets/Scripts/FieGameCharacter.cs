using Fie.AI;
using Fie.Footstep;
using Fie.Manager;
using Fie.Object;
using Fie.Object.Abilities;
using Fie.User;
using Fie.Utility;
using Fie.Voice;
using GameDataEditor;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[RequireComponent(typeof(SkeletonUtility))]
[RequireComponent(typeof(Rigidbody))]
[FiePrefabInfo("")]
public abstract class FieGameCharacter : FieNetworkObjectBase
{
	private struct FieGameCharacterExternalInputContainer
	{
		public Vector3 inputVector;

		public float inputForce;

		public float latestSendTime;

		public bool isDirty;
	}

	public enum StateMachineType
	{
		Base,
		Attack,
		MAXIMUM_MACHINE
	}

	public enum IntelligenceType
	{
		Controllable = 1,
		Connection,
		AI,
		Auto
	}

	public enum SkillTreeSlot
	{
		SKILL_TREE_SLOT_1,
		SKILL_TREE_SLOT_2,
		SKILL_TREE_SLOT_3,
		SKILL_TREE_SLOT_4,
		SKILL_TREE_SLOT_5,
		MAX_SKILL_TREE_SLOT_NUM
	}

	protected delegate bool LoadResourcesDelegate();

	public delegate void AbilityActivateDelegate(Type abilityType);

	public delegate void AbstractStatCheckDelegate();

	public delegate void ChangeScoreDelegate(int score, bool isDefeater);

	public const string COMMON_ANIMATION_MOVE_EVENT_NAME = "move";

	public const string COMMON_ANIMATION_FINISHED_EVENT_NAME = "finished";

	public const string COMMON_ANIMATION_ATTACK_EVENT_NAME = "fire";

	public const string COMMON_ANIMATION_CANCELLABLE = "cancellable";

	public const string COMMON_ANIMATION_IMMUNITY = "immunity";

	public const string COMMON_ANIMATION_FOOTSTEP = "footstep";

	public const float GAME_CHARACTER_JUDGE_TO_FLYING_TIME = 0.05f;

	public const float GAME_CHARACTER_JUDGE_TO_FLYING_HIGHT = 0.2f;

	public const float GAME_CHARACTER_DEFAULT_MOVE_FORCE = 30f;

	public const float GAME_CHARACTER_DEFAULT_MAX_MOVE_SPEED = 5f;

	public const float GAME_CHARACTER_DEFAULT_GRAVITY = 15f;

	public const float GAME_CHARACTER_AUTO_FLIP_THRESHOLD = 0.2f;

	public const float SKELETON_Z_SPACE = 0.002f;

	private List<LoadResourcesDelegate> stackedLoadResouceDelegate = new List<LoadResourcesDelegate>();

	private FieUser _ownerUser;

	[SerializeField]
	private IntelligenceType _intelligenceType = IntelligenceType.Controllable;

	[SerializeField]
	private FieEmittableObjectBase.EmitObjectTag _forces;

	[SerializeField]
	private SkeletonUtility _skeletonUtility;

	[SerializeField]
	private SkeletonAnimation _submeshObject;

	[SerializeField]
	private SkeletonUtilityBone _rootBone;

	[SerializeField]
	private Transform _centerTransform;

	[SerializeField]
	private Transform _guiPointTransform;

	[SerializeField]
	private FieDetector _detector;

	[SerializeField]
	private FieFootstepPlayer _footstepPlayer;

	[SerializeField]
	private int[] _skillTreeSlot = new int[5];

	private Rigidbody _rigidBody;

	private bool _isEndLoadedResources;

	private bool _isAlive = true;

	private bool _isEnableMove = true;

	private bool _isEnableAutoFlip = true;

	private bool _isFlyingCheck;

	private float _isFlyingCheckTime;

	private Vector3 _position = Vector3.zero;

	private Vector3 _externalInputVector = Vector3.zero;

	private float _externalInputForce;

	private FieGameCharacterExternalInputContainer _externalInputContainer = default(FieGameCharacterExternalInputContainer);

	private FieGameCharacterExternalInputContainer _latestSendExternalInputContainer = default(FieGameCharacterExternalInputContainer);

	private Vector3 _latestPosition = Vector3.zero;

	private Vector3 _syncVelocity = Vector3.zero;

	private Vector3 _newVelocity = Vector3.zero;

	private Vector3 _moveForce = Vector3.zero;

	private Vector3 _nowMoveFoce = Vector3.zero;

	private Vector3 _groundPosition = Vector3.zero;

	private Vector3 _anchorOfJudgeToFlying = Vector3.zero;

	private float _nowGravity = 15f;

	private bool _isSpeakable = true;

	private bool _roundVelocity;

	private string _characterName = string.Empty;

	private string _currentDialogKey = string.Empty;

	protected bool _isEnableCollider = true;

	public Vector3 latestDamageWorldPoint;

	private float _defaultMaxHitPoint;

	private float _defaultStaggerRegist;

	private float _defaultMaxShield;

	private float _defaultRegenerateDelay;

	private FieGameCharacterBuildData _buildData;

	private GDESkillTreeData[] _unlockedSkills;

	private FieSkeletonAnimationController _animationController;

	private FieEmotion _emotionController;

	private FieVoiceController _voiceController;

	public FieHealthStats healthStats;

	private FieDamageSystem _damageSystem;

	private FieFriendshipdStats _friendshipStats = new FieFriendshipdStats();

	private FieAbilitiesContainer _abilitiesContainer;

	protected List<FieCollider> _colliderList = new List<FieCollider>();

	private object[] _stateMachine = new object[2];

	protected Dictionary<Type, Type> abstractStateList = new Dictionary<Type, Type>();

	private FieFootstepMaterial _currentFootstepMaterial;

	private float _baseTimeScale = 1f;

	private float _timeScaleMagni = 1f;

	private float _timeScaleMagniSec;

	public int _score;

	private float _expRate = 1f;

	public int totalExp;

	public Vector3 currentMovingVec = Vector3.zero;

	public int score
	{
		get
		{
			return _score;
		}
		set
		{
			_score = value;
		}
	}

	public float expRate
	{
		get
		{
			return _expRate;
		}
		set
		{
			_expRate = value;
		}
	}

	public SkeletonUtilityBone rootBone => _rootBone;

	public FieUser ownerUser => _ownerUser;

	public FieEmittableObjectBase.EmitObjectTag forces
	{
		get
		{
			return _forces;
		}
		set
		{
			_forces = value;
		}
	}

	public IntelligenceType intelligenceType
	{
		get
		{
			return _intelligenceType;
		}
		set
		{
			_intelligenceType = value;
		}
	}

	public bool isEndLoadedResources => _isEndLoadedResources;

	public bool isEnableMove
	{
		get
		{
			return _isEnableMove;
		}
		set
		{
			isEnableMove = value;
		}
	}

	public bool isEnableAutoFlip
	{
		get
		{
			return _isEnableAutoFlip;
		}
		set
		{
			_isEnableAutoFlip = value;
		}
	}

	public Vector3 flipDirectionVector => (base.flipState != 0) ? Vector3.right : Vector3.left;

	public Vector3 position
	{
		get
		{
			return base.transform.position;
		}
		set
		{
			if (base.photonView == null || base.photonView.isMine)
			{
				currentMovingVec += (value - base.transform.position) * (1f / Time.deltaTime);
				base.transform.position = value;
			}
		}
	}

	public Vector3 groundPosition => _groundPosition;

	public Vector3 externalInputVector
	{
		get
		{
			return _externalInputVector;
		}
		set
		{
			_externalInputVector = value;
		}
	}

	public float externalInputForce
	{
		get
		{
			return _externalInputForce;
		}
		protected set
		{
			_externalInputForce = value;
		}
	}

	public FieSkeletonAnimationController animationManager
	{
		get
		{
			return _animationController;
		}
		protected set
		{
			_animationController = value;
		}
	}

	public FieEmotion emotionController
	{
		get
		{
			return _emotionController;
		}
		protected set
		{
			_emotionController = value;
		}
	}

	public FieVoiceController voiceController
	{
		get
		{
			return _voiceController;
		}
		protected set
		{
			_voiceController = value;
		}
	}

	public SkeletonUtility skeletonUtility => _skeletonUtility;

	public MeshRenderer submeshObject => _submeshObject.meshRenderer;

	public FieDetector detector => _detector;

	public FieAbilitiesContainer abilitiesContainer => _abilitiesContainer;

	public bool isEnableGravity
	{
		get
		{
			return _rigidBody.useGravity;
		}
		set
		{
			if (_rigidBody != null)
			{
				if (_rigidBody.useGravity != value)
				{
					Rigidbody rigidBody = _rigidBody;
					Vector3 velocity = _rigidBody.velocity;
					float x = velocity.x;
					Vector3 velocity2 = _rigidBody.velocity;
					rigidBody.velocity = new Vector3(x, 0f, velocity2.z);
				}
				_rigidBody.useGravity = value;
			}
		}
	}

	public Transform centerTransform => _centerTransform;

	public Transform guiPointTransform => _guiPointTransform;

	public FieDamageSystem damageSystem => _damageSystem;

	public List<FieCollider> colliderList
	{
		get
		{
			return _colliderList;
		}
		set
		{
			_colliderList = value;
		}
	}

	public virtual bool isEnableCollider
	{
		get
		{
			return _isEnableCollider;
		}
		set
		{
			if (_colliderList.Count > 0)
			{
				foreach (FieCollider collider in _colliderList)
				{
					collider.isEnable = value;
				}
			}
			_isEnableCollider = value;
		}
	}

	public string characterName
	{
		get
		{
			return _characterName;
		}
		set
		{
			_characterName = value;
		}
	}

	public FieFriendshipdStats friendshipStats => _friendshipStats;

	public FieFootstepPlayer footstepPlayer => _footstepPlayer;

	public FieFootstepMaterial currentFootstepMaterial => _currentFootstepMaterial;

	public bool isSpeakable
	{
		get
		{
			return _isSpeakable;
		}
		set
		{
			_isSpeakable = value;
		}
	}

	public FieGameCharacterBuildData buildData
	{
		get
		{
			return _buildData;
		}
		set
		{
			_buildData = value;
		}
	}

	public GDESkillTreeData[] unlockedSkills
	{
		get
		{
			return _unlockedSkills;
		}
		set
		{
			_unlockedSkills = value;
		}
	}

	public float baseTimeScale
	{
		get
		{
			return _baseTimeScale;
		}
		set
		{
			_baseTimeScale = value;
		}
	}

	public int[] skillTreeSlot => _skillTreeSlot;

	public event AbilityActivateDelegate abilityActivateEvent;

	public event ChangeScoreDelegate changeScoreEvent;

	private void SetPosition(Vector3 value)
	{
		base.transform.position = value;
	}

	private IEnumerator LoadBindedResources(List<LoadResourcesDelegate> resouceDelegateStack)
	{
		if (resouceDelegateStack.Count > 0)
		{
			foreach (LoadResourcesDelegate item in resouceDelegateStack)
			{
				float time = 0f;
				if (time < 3f && !item())
				{
					float num = time + Time.deltaTime;
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
		}
		_isEndLoadedResources = true;
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
		IL_012d:
		/*Error near IL_012e: Unexpected return in MoveNext()*/;
	}

	private void calcGravity()
	{
		if (isEnableGravity)
		{
			if (_nowGravity > 0f)
			{
				_rigidBody.velocity += Vector3.down * _nowGravity * Time.deltaTime;
			}
			else if (_nowGravity < 0f)
			{
				_rigidBody.velocity += Vector3.up * _nowGravity * Time.deltaTime;
			}
		}
	}

	public void setGravityRate(float rate)
	{
		_nowGravity = 15f * rate;
	}

	public void setFlipByVector(Vector3 directionalVector)
	{
		directionalVector.y = (directionalVector.z = 0f);
		Vector3 normalized = directionalVector.normalized;
		if (normalized.x < 0f)
		{
			setFlip(FieObjectFlipState.Left);
		}
		else
		{
			Vector3 normalized2 = directionalVector.normalized;
			if (normalized2.x > 0f)
			{
				setFlip(FieObjectFlipState.Right);
			}
		}
	}

	public int[] GetUnlockedSkillIDs()
	{
		if (_unlockedSkills == null)
		{
			return null;
		}
		int[] array = new int[_unlockedSkills.Length];
		for (int i = 0; i < _unlockedSkills.Length; i++)
		{
			array[i] = _unlockedSkills[i].ID;
		}
		return array;
	}

	public void DirtExternalInputValues()
	{
		if (Vector3.Distance(_externalInputContainer.inputVector, _latestSendExternalInputContainer.inputVector) > 0.2f)
		{
			sendToChangeExternalInputContainerCommand();
		}
		else if (Mathf.Abs(Mathf.Abs(_externalInputContainer.inputForce) - Mathf.Abs(_latestSendExternalInputContainer.inputForce)) > 0.2f)
		{
			sendToChangeExternalInputContainerCommand();
		}
	}

	public void AddDamage(FieGameCharacter attacker, FieDamage damageObject, bool isPenetration = false)
	{
		switch (intelligenceType)
		{
		case IntelligenceType.Controllable:
			if (base.photonView == null || base.photonView.isMine)
			{
				_damageSystem.addDamage(attacker, damageObject, isPenetration);
			}
			break;
		case IntelligenceType.AI:
			if (PhotonNetwork.offlineMode)
			{
				_damageSystem.addDamage(attacker, damageObject, isPenetration);
			}
			else if (attacker != null && attacker.photonView != null && attacker.photonView.owner.isLocal)
			{
				if (PhotonNetwork.isMasterClient)
				{
					_damageSystem.addDamage(attacker, damageObject, isPenetration);
				}
				else
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					byte[] array;
					using (MemoryStream memoryStream = new MemoryStream())
					{
						binaryFormatter.Serialize(memoryStream, damageObject);
						array = memoryStream.ToArray();
					}
					object[] parameters = new object[4]
					{
						attacker.photonView.viewID,
						base.photonView.viewID,
						isPenetration,
						array
					};
					base.photonView.RPC("AddDamageRPC", PhotonTargets.MasterClient, parameters);
					_damageSystem.addDamage(attacker, damageObject, isPenetration);
				}
			}
			break;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		_abilitiesContainer = new FieAbilitiesContainer(this);
		_rigidBody = GetComponent<Rigidbody>();
		position = base.transform.position;
		_latestPosition = position;
		_latestSendExternalInputContainer.latestSendTime = (_externalInputContainer.latestSendTime = Time.time);
		_latestSendExternalInputContainer.isDirty = (_externalInputContainer.isDirty = false);
		_stateMachine[0] = new FieStateMachineManager(this, StateMachineType.Base);
		_stateMachine[1] = new FieStateMachineManager(this, StateMachineType.Attack);
		_emotionController = new FieEmotion(this);
		_voiceController = base.gameObject.GetComponent<FieVoiceController>();
		_defaultMaxHitPoint = healthStats.maxHitPoint;
		_defaultStaggerRegist = healthStats.staggerResistance;
		_defaultMaxShield = healthStats.maxShield;
		_defaultRegenerateDelay = healthStats.regenerateDelay;
		_damageSystem = base.gameObject.AddComponent<FieDamageSystem>();
		InitHealthSystem();
		Type type = GetType();
		if (typeof(FiePlayableGameCharacterInterface).IsInstanceOfType(this))
		{
			DeployBuildData();
		}
		FieManagerBehaviour<FieEnvironmentManager>.I.DifficultyChangedEvent += DifficultyChangedEvent;
		detector.targetChangedEvent += ChangedTargetSyncEvent;
		if (base.photonView != null)
		{
			base.photonView.ObservedComponents.Add(this);
		}
		_groundPosition = base.transform.position;
		_isEndLoadedResources = false;
		initSkillTree();
		damageSystem.addStatusEffectCallback<FieStatusEffectsBuffAndDebuffToAttackEntity>(ApplyStatusEffect_CommonBuffAndDebuffToAttack);
		damageSystem.addStatusEffectCallback<FieStatusEffectsBuffAndDebuffToDeffenceEntity>(ApplyStatusEffect_CommonBuffAndDebuffToDeffence);
		damageSystem.addStatusEffectCallback<FieStatusEffectsTimeScalerEntity>(ApplyStatusEffect_CommonTimeScaler);
	}

	private void ApplyStatusEffect_CommonTimeScaler(FieStatusEffectEntityBase statusEffect, FieGameCharacter attacker, FieDamage damage)
	{
		if (damage.statusEffects.Count > 0)
		{
			foreach (FieStatusEffectEntityBase statusEffect2 in damage.statusEffects)
			{
				FieStatusEffectsTimeScalerEntity fieStatusEffectsTimeScalerEntity = statusEffect2 as FieStatusEffectsTimeScalerEntity;
				if (fieStatusEffectsTimeScalerEntity != null && fieStatusEffectsTimeScalerEntity.isActive)
				{
					SetTimeScaleMagni(fieStatusEffectsTimeScalerEntity.targetTimeScale, fieStatusEffectsTimeScalerEntity.duration);
				}
			}
		}
	}

	private void ApplyStatusEffect_CommonBuffAndDebuffToDeffence(FieStatusEffectEntityBase statusEffect, FieGameCharacter attacker, FieDamage damage)
	{
		if (damage.statusEffects.Count > 0)
		{
			foreach (FieStatusEffectEntityBase statusEffect2 in damage.statusEffects)
			{
				FieStatusEffectsBuffAndDebuffToDeffenceEntity fieStatusEffectsBuffAndDebuffToDeffenceEntity = statusEffect2 as FieStatusEffectsBuffAndDebuffToDeffenceEntity;
				if (fieStatusEffectsBuffAndDebuffToDeffenceEntity != null && fieStatusEffectsBuffAndDebuffToDeffenceEntity.isActive)
				{
					damageSystem.AddDefenceMagni(fieStatusEffectsBuffAndDebuffToDeffenceEntity.skillID, fieStatusEffectsBuffAndDebuffToDeffenceEntity.magni, fieStatusEffectsBuffAndDebuffToDeffenceEntity.duration, fieStatusEffectsBuffAndDebuffToDeffenceEntity.isEnableStack);
				}
			}
		}
	}

	private void ApplyStatusEffect_CommonBuffAndDebuffToAttack(FieStatusEffectEntityBase statusEffect, FieGameCharacter attacker, FieDamage damage)
	{
		if (damage.statusEffects.Count > 0)
		{
			foreach (FieStatusEffectEntityBase statusEffect2 in damage.statusEffects)
			{
				FieStatusEffectsBuffAndDebuffToAttackEntity fieStatusEffectsBuffAndDebuffToAttackEntity = statusEffect2 as FieStatusEffectsBuffAndDebuffToAttackEntity;
				if (fieStatusEffectsBuffAndDebuffToAttackEntity != null && fieStatusEffectsBuffAndDebuffToAttackEntity.isActive)
				{
					damageSystem.AddAttackMagni(fieStatusEffectsBuffAndDebuffToAttackEntity.skillID, fieStatusEffectsBuffAndDebuffToAttackEntity.magni, fieStatusEffectsBuffAndDebuffToAttackEntity.duration, fieStatusEffectsBuffAndDebuffToAttackEntity.abilityID, fieStatusEffectsBuffAndDebuffToAttackEntity.isEnableStack);
				}
			}
		}
	}

	public GDESkillTreeData GetSkill(FieConstValues.FieSkill skillID)
	{
		if (_unlockedSkills == null || _unlockedSkills.Length <= 0)
		{
			return null;
		}
		for (int i = 0; i < _unlockedSkills.Length; i++)
		{
			if (_unlockedSkills[i].ID == (int)skillID)
			{
				return _unlockedSkills[i];
			}
		}
		return null;
	}

	private void DeployBuildData()
	{
		if (base.photonView != null && base.photonView.isMine)
		{
			FieManagerBehaviour<FieSaveManager>.I.ApplyBuildDataFromSaveData((int)getGameCharacterID(), ref _unlockedSkills);
			int num = totalExp = FieManagerBehaviour<FieSaveManager>.I.onMemorySaveData.CharacterExp[(int)getGameCharacterID()];
			base.photonView.RPC("DeployBuildDataRPC", PhotonTargets.OthersBuffered, num, GetUnlockedSkillIDs());
			ApplyPassiveSkills();
		}
	}

	private void ApplyPassiveSkills()
	{
		if (unlockedSkills != null && unlockedSkills.Length > 0)
		{
			List<GDESkillTreeData> list = unlockedSkills.ToList();
			for (int i = 0; i < unlockedSkills.Length; i++)
			{
				if (unlockedSkills[i].RelatedSkill != null && unlockedSkills[i].RelatedSkill.Key != string.Empty)
				{
					list.Add(unlockedSkills[i].RelatedSkill);
				}
			}
			unlockedSkills = list.ToArray();
			float maxHitPoint = healthStats.maxHitPoint;
			float maxShield = healthStats.maxShield;
			float staggerResistance = healthStats.staggerResistance;
			for (int j = 0; j < unlockedSkills.Length; j++)
			{
				if (unlockedSkills[j].GameCharacterType.Key == getCharacterTypeData().Key && unlockedSkills[j].SkillType != null)
				{
					switch (unlockedSkills[j].SkillType.ID)
					{
					case 6:
						healthStats.maxHitPoint += maxHitPoint * unlockedSkills[j].Value1;
						healthStats.maxShield += maxShield * unlockedSkills[j].Value2;
						break;
					case 7:
						healthStats.staggerResistance += staggerResistance * unlockedSkills[j].Value1;
						break;
					}
				}
			}
		}
	}

	[PunRPC]
	public void DeployBuildDataRPC(int totalExp, int[] unlockedSkillIDs)
	{
		this.totalExp = totalExp;
		if (unlockedSkillIDs != null && unlockedSkillIDs.Length > 0)
		{
			_unlockedSkills = FieMasterData<GDESkillTreeData>.FindMasterDataList(delegate(GDESkillTreeData data)
			{
				for (int i = 0; i < unlockedSkillIDs.Length; i++)
				{
					if (unlockedSkillIDs[i] == data.ID)
					{
						return true;
					}
				}
				return false;
			}).ToArray();
			ApplyPassiveSkills();
			ReCalcSkillData();
		}
	}

	protected virtual void ReCalcSkillData()
	{
	}

	private void DifficultyChangedEvent()
	{
		InitHealthSystem();
	}

	private void InitHealthSystem()
	{
		if (forces == FieEmittableObjectBase.EmitObjectTag.ENEMY)
		{
			healthStats.maxHitPoint = _defaultMaxHitPoint * FieManagerBehaviour<FieEnvironmentManager>.I.currentEnemyHealthMagnify;
			healthStats.maxShield = _defaultMaxShield * FieManagerBehaviour<FieEnvironmentManager>.I.currentEnemyHealthMagnify;
			healthStats.staggerResistance = _defaultStaggerRegist * FieManagerBehaviour<FieEnvironmentManager>.I.currentStaggerRegistMagnify;
		}
		else if (forces == FieEmittableObjectBase.EmitObjectTag.PLAYER)
		{
			healthStats.regenerateDelay = _defaultRegenerateDelay * FieManagerBehaviour<FieEnvironmentManager>.I.currentPlayerRegenDelayMagnify;
		}
		if (_damageSystem != null)
		{
			_damageSystem.initHealthSystem(this, ref healthStats);
		}
	}

	protected virtual void Start()
	{
		StartCoroutine(LoadBindedResources(stackedLoadResouceDelegate));
		_rootBone.mode = SkeletonUtilityBone.Mode.Override;
		_rootBone.overrideAlpha = 1f;
		setFlip(base.flipState);
	}

	public void ReloadPreloadedResouces()
	{
		StartCoroutine(LoadBindedResources(stackedLoadResouceDelegate));
	}

	public void InitializeIntelligenceSystem(IntelligenceType settleType = IntelligenceType.Auto)
	{
		if (settleType != IntelligenceType.Auto)
		{
			intelligenceType = settleType;
		}
		switch (intelligenceType)
		{
		case IntelligenceType.Controllable:
			if (base.gameObject.GetComponent<FieInputGamePadAndKeyboard>() == null)
			{
				FieInputGamePadAndKeyboard fieInputGamePadAndKeyboard = base.gameObject.AddComponent<FieInputGamePadAndKeyboard>();
				fieInputGamePadAndKeyboard.SetOwner(this);
			}
			break;
		case IntelligenceType.AI:
			if (base.gameObject.GetComponent<FieAITaskController>() == null)
			{
				FieAITaskController component = base.gameObject.GetComponent<FieAITaskController>();
				if (component == null)
				{
					component = base.gameObject.AddComponent<FieAITaskController>();
					component.SetOwner(this);
				}
			}
			if (base.gameObject.GetComponent<FieAIHateController>() == null)
			{
				FieAIHateController component2 = base.gameObject.GetComponent<FieAIHateController>();
				if (component2 == null)
				{
					component2 = base.gameObject.AddComponent<FieAIHateController>();
					component2.SetOwner(this);
				}
			}
			break;
		}
	}

	protected virtual void Update()
	{
		getStateMachine(StateMachineType.Attack).updateState();
		getStateMachine().updateState();
		if (base.photonView == null || base.photonView.isMine)
		{
			damageSystem.updateHealthSystem(Time.deltaTime);
		}
		_nowMoveFoce = Vector3.zero;
		if (_isEnableAutoFlip)
		{
			Vector3 vector = (!(externalInputVector != Vector3.zero) || !(externalInputForce > 0.25f)) ? _rigidBody.velocity : externalInputVector;
			if (vector.x < -0.2f)
			{
				setFlip(FieObjectFlipState.Left);
			}
			else if (vector.x > 0.2f)
			{
				setFlip(FieObjectFlipState.Right);
			}
		}
		UpdateTimeScale(Time.deltaTime);
		_abilitiesContainer.UpdateCooldown(Time.deltaTime);
		ref Vector3 groundPosition = ref _groundPosition;
		Vector3 position = base.transform.position;
		groundPosition.x = position.x;
		ref Vector3 groundPosition2 = ref _groundPosition;
		Vector3 position2 = base.transform.position;
		groundPosition2.z = position2.z;
		if (base.flipState == FieObjectFlipState.Left)
		{
			base.transform.rotation = Quaternion.AngleAxis(0f, Vector3.up);
			_skeletonUtility.skeletonAnimation.zSpacing = -0.002f;
			_skeletonUtility.skeletonRenderer.isFlippedNormal = false;
		}
		else
		{
			base.transform.rotation = Quaternion.AngleAxis(180f, Vector3.up);
			_skeletonUtility.skeletonAnimation.zSpacing = 0.002f;
			_skeletonUtility.skeletonRenderer.isFlippedNormal = true;
		}
	}

	public void SetTimeScaleMagni(float targetTimeScaleMagni, float duration)
	{
		_timeScaleMagni = Mathf.Max(0f, targetTimeScaleMagni);
		_timeScaleMagniSec = Mathf.Max(0f, duration);
	}

	private void UpdateTimeScale(float deltaTime)
	{
		if (animationManager != null)
		{
			if (_timeScaleMagniSec > 0f)
			{
				_timeScaleMagniSec -= Time.deltaTime;
			}
			else
			{
				_timeScaleMagni = 1f;
			}
			animationManager.SetAnimationTimeScale(Mathf.Clamp(baseTimeScale * _timeScaleMagni, 0f, 10f));
		}
	}

	protected virtual void FixedUpdate()
	{
		if (_newVelocity != Vector3.zero)
		{
			if (_roundVelocity)
			{
				_rigidBody.velocity = roundMoveVelocity(_newVelocity, _rigidBody.velocity);
				_roundVelocity = false;
			}
			else
			{
				_rigidBody.velocity += _newVelocity;
			}
			_newVelocity = Vector3.zero;
		}
		_isFlyingCheckTime += Time.deltaTime;
		if (!(_isFlyingCheckTime >= 0.05f))
		{
			float y = _anchorOfJudgeToFlying.y;
			Vector3 position = base.transform.position;
			if (!(Mathf.Abs(y - position.y) > 0.2f))
			{
				goto IL_00cc;
			}
		}
		base.groundState = FieObjectGroundState.Flying;
		goto IL_00cc;
		IL_00cc:
		if (base.groundState == FieObjectGroundState.Flying || _isFlyingCheck)
		{
			calcGravity();
		}
		_nowMoveFoce = _rigidBody.velocity;
		currentMovingVec += _rigidBody.velocity;
		_latestPosition = base.transform.position;
	}

	private Vector3 roundMoveVelocity(Vector3 _addVelocity, Vector3 _baseVelocity)
	{
		Vector3 result = _baseVelocity + _addVelocity;
		if (Mathf.Abs(result.x) > Mathf.Abs(5f))
		{
			result.x = ((!(result.x < 0f)) ? 5f : (-5f));
		}
		if (Mathf.Abs(result.z) > Mathf.Abs(5f))
		{
			result.z = ((!(result.z < 0f)) ? 5f : (-5f));
		}
		return result;
	}

	protected virtual void LateUpdate()
	{
		if (base.photonView != null && base.photonView.isMine)
		{
			base.photonTransformView.SetSynchronizedValues(currentMovingVec, 0f);
		}
		currentMovingVec = Vector3.zero;
		UpdateDialogState();
	}

	private void UpdateDialogState()
	{
		if (_currentDialogKey != string.Empty)
		{
			if (isSpeakable)
			{
				FieManagerBehaviour<FieDialogManager>.I.RequestDialog(this, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(_currentDialogKey));
			}
			_currentDialogKey = string.Empty;
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.transform.tag == "Floor")
		{
			bool flag = false;
			ContactPoint[] contacts = collision.contacts;
			for (int i = 0; i < contacts.Length; i++)
			{
				ContactPoint contactPoint = contacts[i];
				flag |= (Vector3.Dot(Vector3.up, contactPoint.normal) > 0.9f);
				_anchorOfJudgeToFlying = collision.contacts[0].point;
			}
			if (flag)
			{
				base.groundState = FieObjectGroundState.Grounding;
				setGravityRate(1f);
				_isFlyingCheck = false;
				_isFlyingCheckTime = 0f;
				ref Vector3 groundPosition = ref _groundPosition;
				Vector3 position = base.transform.position;
				groundPosition.y = position.y;
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(collision.transform.tag != "Floor") && !(collision.gameObject == null))
		{
			bool flag = false;
			ContactPoint[] contacts = collision.contacts;
			for (int i = 0; i < contacts.Length; i++)
			{
				ContactPoint contactPoint = contacts[i];
				flag |= (Vector3.Dot(Vector3.up, contactPoint.normal) > 0.9f);
			}
			if (flag)
			{
				_currentFootstepMaterial = collision.gameObject.GetComponent<FieFootstepMaterial>();
			}
		}
	}

	public void setFlip(FieObjectFlipState flipState)
	{
		if (flipState == FieObjectFlipState.Left)
		{
			base.flipState = FieObjectFlipState.Left;
		}
		else
		{
			base.flipState = FieObjectFlipState.Right;
		}
	}

	public void switchFlip()
	{
		if (base.flipState == FieObjectFlipState.Left)
		{
			setFlip(FieObjectFlipState.Right);
		}
		else
		{
			setFlip(FieObjectFlipState.Left);
		}
	}

	public Vector3 getNowMoveForce()
	{
		return _nowMoveFoce;
	}

	public void addMoveForce(Vector3 moveForce, float duration, bool useRound = true)
	{
		_newVelocity += moveForce * Time.deltaTime;
		currentMovingVec += _newVelocity;
		_roundVelocity |= useRound;
	}

	public void setMoveForce(Vector3 moveForce, float duration, bool useRound = true)
	{
		Vector3 vector = _rigidBody.velocity + moveForce;
		_rigidBody.velocity = ((!useRound) ? vector : roundMoveVelocity(vector, _rigidBody.velocity));
	}

	public void resetMoveForce()
	{
		_rigidBody.velocity = Vector3.zero;
	}

	public void adjustMoveForce(float adjustRate)
	{
		_rigidBody.velocity *= adjustRate;
	}

	public virtual float getDefaultMoveSpeed()
	{
		return 30f;
	}

	public FieStateMachineManager getStateMachine(StateMachineType type = StateMachineType.Base)
	{
		return _stateMachine[(int)type] as FieStateMachineManager;
	}

	public FieStateMachineInterface setState<T>(Type state, bool isForceSet, bool isDupulicate, StateMachineType type = StateMachineType.Base) where T : FieGameCharacter
	{
		return (_stateMachine[(int)type] as FieStateMachineManager)?.setState(state, isForceSet, isDupulicate);
	}

	public FieStateMachineInterface setStateToStatheMachine(Type state, bool isForceSet, bool isDupulicate, StateMachineType type = StateMachineType.Base)
	{
		Type entityState = getEntityState(state);
		MethodInfo method = GetType().GetMethod("setState");
		if (method == null)
		{
			return null;
		}
		MethodInfo methodInfo = method.MakeGenericMethod(GetType());
		if (methodInfo == null)
		{
			return null;
		}
		object[] parameters = new object[4]
		{
			entityState,
			isForceSet,
			isDupulicate,
			type
		};
		return methodInfo.Invoke(this, parameters) as FieStateMachineInterface;
	}

	public void SetStateActivateCheckCallback<T>(FieStateMachineManager.FieStateMachineActivateCheckDelegate callback, StateMachineType type = StateMachineType.Base) where T : FieStateMachineInterface
	{
		(_stateMachine[(int)type] as FieStateMachineManager)?.SetActivateCheckEvent<T>(callback);
	}

	public virtual Type getDefaultAITask()
	{
		return typeof(FieAITaskCommonIdle);
	}

	protected void PreAssignEmittableObject<T>() where T : FieEmittableObjectBase
	{
		stackedLoadResouceDelegate.Add(FieManagerBehaviour<FieEmittableObjectManager>.I.LoadAsync<T>);
	}

	public void UnbindFromDetecter()
	{
		if (_colliderList.Count > 0)
		{
			foreach (FieCollider collider in _colliderList)
			{
				collider.unbindFromDetector();
			}
		}
	}

	public Type getEntityState(Type type)
	{
		Type type2 = type;
		if (abstractStateList.ContainsKey(type))
		{
			type2 = abstractStateList[type];
		}
		if (abilitiesContainer.isAbility(type2))
		{
			if (!abilitiesContainer.checkToActivate(this, type2))
			{
				return null;
			}
			if (this.abilityActivateEvent != null)
			{
				this.abilityActivateEvent(type2);
			}
		}
		return type2;
	}

	public void SetExternalForces(Vector3 directionalVec, float force)
	{
		externalInputVector = directionalVec;
		externalInputForce = force;
	}

	public GDEWordScriptsListData LotteryWordScriptData(GDEWordScriptTriggerTypeData triggerType)
	{
		if (triggerType == null)
		{
			return null;
		}
		GDEGameCharacterTypeData characterType = getCharacterTypeData();
		if (getCharacterTypeData() == null)
		{
			return null;
		}
		Dictionary<string, GDEWordScriptsListData> dictionary = FieMasterData<GDEWordScriptsListData>.I.FindMasterDataDictionary(delegate(GDEWordScriptsListData data)
		{
			if (data.Actor.Key != characterType.Key)
			{
				return false;
			}
			if (data.Trigger.Key != triggerType.Key)
			{
				return false;
			}
			return true;
		});
		if (dictionary.Count <= 0)
		{
			return null;
		}
		Lottery<GDEWordScriptsListData> lottery = new Lottery<GDEWordScriptsListData>();
		foreach (KeyValuePair<string, GDEWordScriptsListData> item in dictionary)
		{
			lottery.AddItem(item.Value);
		}
		if (!lottery.IsExecutable())
		{
			return null;
		}
		return lottery.Lot();
	}

	public void SetDialog(GDEWordScriptTriggerTypeData triggerType, int activatePercent = 100)
	{
		SetDialog(activatePercent, LotteryWordScriptData(triggerType));
	}

	public void SetDialog(int activatePercent, params GDEWordScriptsListData[] dialogItem)
	{
		if (dialogItem != null && dialogItem.Length > 0)
		{
			if (activatePercent < 100)
			{
				int num = UnityEngine.Random.Range(0, 100) + 1;
				if (num > activatePercent)
				{
					return;
				}
			}
			if (dialogItem.Length == 1)
			{
				_currentDialogKey = dialogItem[0].Key;
			}
			else
			{
				Lottery<GDEWordScriptsListData> lottery = new Lottery<GDEWordScriptsListData>();
				foreach (GDEWordScriptsListData item in dialogItem)
				{
					lottery.AddItem(item);
				}
				if (lottery.IsExecutable())
				{
					_currentDialogKey = lottery.Lot().Key;
				}
			}
		}
	}

	public void SetOwnerUser(FieUser newOwnerUser)
	{
		_ownerUser = newOwnerUser;
	}

	public void RequestToSetStatusEffect<T>(T statusEffectObject) where T : FieStatusEffectsBase
	{
	}

	public virtual GDEGameCharacterTypeData getCharacterTypeData()
	{
		return null;
	}

	public FieStateMachineInterface sendStateChangeCommand(FieStateMachineManager stateMachineManager, Type nextState)
	{
		if (base.photonView == null || base.photonView.isMine)
		{
			FieStateMachineInterface fieStateMachineInterface = Activator.CreateInstance(nextState) as FieStateMachineInterface;
			if (fieStateMachineInterface == null)
			{
				return null;
			}
			if (!fieStateMachineInterface.isNotNetworkSync())
			{
				object[] parameters = new object[2]
				{
					stateMachineManager.stateMachineType,
					nextState.FullName
				};
				if (base.photonView != null)
				{
					base.photonView.RPC("changeStateRPC", PhotonTargets.Others, parameters);
				}
			}
			return stateMachineManager.SetStateDynamic(nextState);
		}
		return null;
	}

	[PunRPC]
	public void changeStateRPC(int stateMachineTypeIntValue, string nextStateName)
	{
		Type type = Type.GetType(nextStateName);
		getStateMachine((StateMachineType)stateMachineTypeIntValue)?.SetStateDynamic(type);
	}

	private void sendToChangeExternalInputContainerCommand()
	{
		if (base.photonView != null && base.photonView.isMine)
		{
			_latestSendExternalInputContainer.inputVector = _externalInputContainer.inputVector;
			_latestSendExternalInputContainer.inputForce = _externalInputContainer.inputForce;
			_latestSendExternalInputContainer.latestSendTime = (_externalInputContainer.latestSendTime = Time.time);
			_externalInputContainer.isDirty = true;
			object[] parameters = new object[2]
			{
				_latestSendExternalInputContainer.inputVector,
				_latestSendExternalInputContainer.inputForce
			};
			base.photonView.RPC("ChangeExternalInputValuesRPC", PhotonTargets.Others, parameters);
		}
	}

	[PunRPC]
	public void ChangeExternalInputValuesRPC(Vector3 inputVector, float inputForce)
	{
		_externalInputVector = inputVector;
		_externalInputForce = inputForce;
	}

	private void ChangedTargetSyncEvent(FieGameCharacter fromCharacter, FieGameCharacter toCharacter)
	{
		if (!(toCharacter == null) && base.photonView != null && base.photonView.isMine)
		{
			PhotonView component = toCharacter.GetComponent<PhotonView>();
			if (!(component == null))
			{
				object[] parameters = new object[1]
				{
					component.viewID
				};
				base.photonView.RPC("ChangeTargetRPC", PhotonTargets.Others, parameters);
			}
		}
	}

	[PunRPC]
	public void ChangeTargetRPC(int viewID)
	{
		PhotonView photonView = PhotonView.Find(viewID);
		if (photonView == null)
		{
			detector.setLockonTargetDynamic(null);
		}
		else
		{
			FieGameCharacter component = photonView.GetComponent<FieGameCharacter>();
			if (component != null)
			{
				detector.setLockonTargetDynamic(component);
			}
		}
	}

	[PunRPC]
	public void AddDamageRPC(int attackerViewID, int targetViewID, bool isPenetration, byte[] damageObject)
	{
		PhotonView photonView = PhotonView.Find(attackerViewID);
		PhotonView photonView2 = PhotonView.Find(targetViewID);
		if (!(photonView == null) && !(photonView2 == null))
		{
			FieGameCharacter component = photonView.GetComponent<FieGameCharacter>();
			FieGameCharacter component2 = photonView2.GetComponent<FieGameCharacter>();
			if (component2 != null)
			{
				MemoryStream memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				memoryStream.Write(damageObject, 0, damageObject.Length);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				FieDamage damageObject2 = (FieDamage)binaryFormatter.Deserialize(memoryStream);
				component2.damageSystem.addDamage(component, damageObject2, isPenetration);
			}
		}
	}

	[PunRPC]
	public void ChangeViewIDRPC(int viewID)
	{
		base.photonView.viewID = viewID;
	}

	[PunRPC]
	public void SetScoreRPC(int score, bool isDefeater)
	{
		_score += score;
		if (this.changeScoreEvent != null)
		{
			this.changeScoreEvent(score, isDefeater);
		}
	}

	public void ReduceOrIncreaseScore(int score, bool isDefeater)
	{
		object[] parameters = new object[2]
		{
			score,
			isDefeater
		};
		base.photonView.RPC("SetScoreRPC", PhotonTargets.Others, parameters);
		SetScoreRPC(score, isDefeater);
	}

	public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(_externalInputVector);
			stream.SendNext(_externalInputForce);
			stream.SendNext(healthStats.maxHitPoint);
			stream.SendNext(healthStats.maxShield);
			stream.SendNext(healthStats.hitPoint);
			stream.SendNext(healthStats.shield);
		}
		else
		{
			_externalInputVector = (Vector3)stream.ReceiveNext();
			_externalInputForce = (float)stream.ReceiveNext();
			healthStats.maxHitPoint = (float)stream.ReceiveNext();
			healthStats.maxShield = (float)stream.ReceiveNext();
			healthStats.hitPoint = (float)stream.ReceiveNext();
			healthStats.shield = (float)stream.ReceiveNext();
		}
	}

	protected virtual void initSkillTree()
	{
	}

	public GDEGameCharacterTypeData getGameCharacterTypeData()
	{
		return FieMasterData<GDEGameCharacterTypeData>.FindMasterData(delegate(GDEGameCharacterTypeData data)
		{
			if (data.ID == (int)getGameCharacterID())
			{
				return true;
			}
			return false;
		});
	}

	public abstract Type getDefaultAttackState();

	public abstract FieConstValues.FieGameCharacter getGameCharacterID();

	public abstract string getDefaultName();

	public abstract void RequestToChangeState<T>(Vector3 directionalVec, float force, StateMachineType type) where T : FieStateMachineInterface;

	public abstract FieStateMachineInterface getDefaultState(StateMachineType type);
}
