using Fie.Manager;
using Fie.Portal;
using Fie.Scene;
using Fie.Utility;
using GameDataEditor;
using Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieWaveController : Photon.MonoBehaviour
{
	private const float FIRST_WAVE_INTERVAL = 8f;

	private const float NEXT_WAVE_INTERVAL = 6f;

	[SerializeField]
	private List<Transform> _enemySpawnPoint = new List<Transform>();

	[SerializeField]
	private int _totalEnemyCapacity;

	[SerializeField]
	private int _eachWaveEnemyCapacity;

	[SerializeField]
	private float _IncreasedEachWaveCapacityRate = 0.1f;

	[SerializeField]
	private List<ValueList> _spawnableEnemies = new List<ValueList>();

	[SerializeField]
	private float _firstInterval = 8f;

	[SerializeField]
	private float _nextInterval = 6f;

	[SerializeField]
	public bool _isEnable;

	[SerializeField]
	public bool _willStartWithCllider = true;

	[SerializeField]
	public bool _nonEffectiveDifficulty;

	[SerializeField]
	public FieVisualizedPortal _activatePortal;

	private int _currentEnemyCapacity;

	private int _latestEnemyCapacity;

	private int _currentAllivedEnemyNum;

	private float _waveInterval = 8f;

	private float _enemyCheckInterval;

	private float _everyoneDiedCounter;

	private Dictionary<int, FieGameCharacter> _currentEnemies = new Dictionary<int, FieGameCharacter>();

	private Dictionary<FieConstValues.FieEnemy, int> _spawnedNum = new Dictionary<FieConstValues.FieEnemy, int>();

	private IEnumerator enemySpawnTask;

	private bool _isGenerating;

	private bool _prepareForActivity;

	private bool _isEnd;

	private int _totalEnemyCapacityModifyed;

	private int _eachWaveEnemyCapacityModifyed;

	private int _currentWaveCount;

	private int _latestWaveCount;

	public void Initialize()
	{
		_isEnable = false;
		_waveInterval = _firstInterval;
		_enemyCheckInterval = 0f;
		_everyoneDiedCounter = 0f;
		_currentEnemies = new Dictionary<int, FieGameCharacter>();
		_spawnedNum = new Dictionary<FieConstValues.FieEnemy, int>();
		_isGenerating = false;
		_prepareForActivity = false;
		_isEnd = false;
		_currentAllivedEnemyNum = 2147483647;
		_currentEnemyCapacity = _latestEnemyCapacity;
		_currentWaveCount = _latestWaveCount;
		ApplyDifficultyDataToEnemyCap();
		_currentEnemyCapacity = (_latestEnemyCapacity = _totalEnemyCapacityModifyed);
	}

	private void ApplyDifficultyDataToEnemyCap()
	{
		if (_nonEffectiveDifficulty)
		{
			_totalEnemyCapacityModifyed = _totalEnemyCapacity;
			_eachWaveEnemyCapacityModifyed = _eachWaveEnemyCapacity;
		}
		else
		{
			GDEDifficultyListData difficultyData = FieManagerBehaviour<FieEnvironmentManager>.I.GetDifficultyData(FieManagerBehaviour<FieEnvironmentManager>.I.currentDifficulty);
			_totalEnemyCapacityModifyed = Mathf.RoundToInt((float)_totalEnemyCapacity * difficultyData.enemyCostCapMagnify);
			_eachWaveEnemyCapacityModifyed = Mathf.RoundToInt((float)_eachWaveEnemyCapacity * difficultyData.enemyCostCapMagnify);
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(_currentEnemyCapacity);
			stream.SendNext(_latestEnemyCapacity);
			stream.SendNext(_currentWaveCount);
		}
		else
		{
			_currentEnemyCapacity = (int)stream.ReceiveNext();
			_latestEnemyCapacity = (int)stream.ReceiveNext();
			_currentWaveCount = (int)stream.ReceiveNext();
		}
	}

	private void Start()
	{
		ApplyDifficultyDataToEnemyCap();
		_currentEnemyCapacity = (_latestEnemyCapacity = _totalEnemyCapacityModifyed);
		_waveInterval = _firstInterval;
		if (_activatePortal != null)
		{
			_activatePortal.gameObject.SetActive(value: false);
		}
		FieManagerBehaviour<FieInGameStateManager>.I.RetryEvent += RetryEvent;
	}

	private void RetryEvent()
	{
		Initialize();
	}

	private void Update()
	{
		if (_isEnable)
		{
			WatchEnemyState();
			UpdateWaveState();
		}
	}

	private void WatchEnemyState()
	{
		if (!_isEnd && base.photonView.isMine)
		{
			if (_enemyCheckInterval > 0f)
			{
				_enemyCheckInterval -= Time.deltaTime;
			}
			else
			{
				if (_currentEnemies.Count > 0)
				{
					_currentAllivedEnemyNum = 0;
					foreach (KeyValuePair<int, FieGameCharacter> currentEnemy in _currentEnemies)
					{
						if (currentEnemy.Value != null && !currentEnemy.Value.damageSystem.isDead)
						{
							_currentAllivedEnemyNum++;
						}
					}
				}
				else
				{
					_currentAllivedEnemyNum = 0;
				}
				_enemyCheckInterval = 1f;
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (_willStartWithCllider && !_isEnable)
		{
			FieCollider component = collider.gameObject.GetComponent<FieCollider>();
			if (component != null && component.getParentGameCharacter() != null && collider.gameObject.tag == "Player")
			{
				_isEnable = true;
			}
		}
	}

	private void UpdateWaveState()
	{
		if (!_isEnd && base.photonView.isMine && !_isGenerating && _currentAllivedEnemyNum <= 0)
		{
			if (_waveInterval > 0f)
			{
				_waveInterval -= Time.deltaTime;
				_prepareForActivity = false;
			}
			else if (_currentEnemyCapacity <= 0)
			{
				if (_activatePortal != null)
				{
					_activatePortal.gameObject.SetActive(value: true);
				}
				_isEnd = true;
				base.photonView.RPC("SetActivePortalRPC", PhotonTargets.Others, null);
			}
			else
			{
				_latestEnemyCapacity = _currentEnemyCapacity;
				_latestWaveCount = _currentWaveCount;
				StartCoroutine(EnemiesSpawnCoroutine(_spawnableEnemies));
				_waveInterval = _nextInterval;
			}
		}
	}

	[PunRPC]
	public void SetActivePortalRPC()
	{
		if (_activatePortal != null)
		{
			_activatePortal.gameObject.SetActive(value: true);
		}
	}

	private void ShowResult()
	{
		if (!PhotonNetwork.offlineMode)
		{
			base.photonView.RPC("ShowResultRPC", PhotonTargets.All, null);
		}
		else
		{
			ShowResultRPC();
		}
	}

	public void ShowActivity(string titleKey, string noteKey, string fromString, string toString)
	{
		if (!PhotonNetwork.offlineMode)
		{
			object[] parameters = new object[4]
			{
				titleKey,
				noteKey,
				fromString,
				toString
			};
			base.photonView.RPC("ShowActivityRPC", PhotonTargets.All, parameters);
		}
		else
		{
			ShowActivityRPC(titleKey, noteKey, fromString, toString);
		}
	}

	[PunRPC]
	public void ShowActivityRPC(string titleKey, string noteKey, string fromString, string toString)
	{
		FieManagerBehaviour<FieActivityManager>.I.RegistReplaceString(fromString, toString);
		FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(titleKey), FieMasterData<GDEConstantTextListData>.I.GetMasterData(noteKey));
	}

	[PunRPC]
	public void ShowResultRPC()
	{
		FieManagerBehaviour<FieSceneManager>.I.LoadScene(new FieSceneResult(), allowSceneActivation: true, FieFaderManager.FadeType.OUT_TO_WHITE, 1f);
	}

	private IEnumerator EnemiesSpawnCoroutine(List<ValueList> _enemies)
	{
		_isGenerating = true;
		Lottery<ValueList> _spawnerLot = new Lottery<ValueList>();
		Lottery<Transform> _spawnPoinLot = new Lottery<Transform>();
		_spawnerLot.InitializeFromListData(_enemies);
		_spawnPoinLot.InitializeFromListData(_enemySpawnPoint);
		_spawnedNum.Clear();
		int currentCap = _eachWaveEnemyCapacityModifyed + Mathf.RoundToInt((float)_eachWaveEnemyCapacityModifyed * (_IncreasedEachWaveCapacityRate * (float)_currentWaveCount));
		int consumedCap = 0;
		int retryCount = 0;
		while (currentCap > 0 && _spawnerLot.IsExecutable())
		{
			ValueList hittedEnemy = _spawnerLot.Lot();
			FieConstValues.FieEnemy hittedEnemyTypeID = hittedEnemy._enemy.GetEnemyMasterDataID();
			int capNum = 0;
			switch (FieManagerBehaviour<FieEnvironmentManager>.I.currentDifficulty)
			{
			case FieEnvironmentManager.Difficulty.EASY:
				capNum = hittedEnemy._spawnableCapForEasy;
				break;
			case FieEnvironmentManager.Difficulty.NORMAL:
				capNum = hittedEnemy._spawnableCapForNormal;
				break;
			case FieEnvironmentManager.Difficulty.HARD:
				capNum = hittedEnemy._spawnableCapForHard;
				break;
			case FieEnvironmentManager.Difficulty.VERY_HARD:
				capNum = hittedEnemy._spawnableCapForVeryHard;
				break;
			case FieEnvironmentManager.Difficulty.NIGHTMARE:
				capNum = hittedEnemy._spawnableCapForNightmare;
				break;
			case FieEnvironmentManager.Difficulty.CHAOS:
				capNum = hittedEnemy._spawnableCapForChaos;
				break;
			}
			if (capNum <= 0 || (_spawnedNum.ContainsKey(hittedEnemyTypeID) && _spawnedNum[hittedEnemyTypeID] >= capNum))
			{
				retryCount++;
				if (retryCount < 200)
				{
					continue;
				}
			}
			if (currentCap >= consumedCap + hittedEnemy._enemy.GetEnemyMasterData().Cost)
			{
				FieGameCharacter createdEnemy = CreateEnemy(position: (!_spawnPoinLot.IsExecutable()) ? Vector3.zero : _spawnPoinLot.Lot().position, waveObject: hittedEnemy._enemy);
				_currentEnemies[createdEnemy.GetInstanceID()] = createdEnemy;
				if (!_spawnedNum.ContainsKey(hittedEnemyTypeID))
				{
					_spawnedNum[hittedEnemyTypeID] = 0;
				}
				Dictionary<FieConstValues.FieEnemy, int> spawnedNum;
				FieConstValues.FieEnemy key;
				(spawnedNum = _spawnedNum)[key = hittedEnemyTypeID] = spawnedNum[key] + 1;
				consumedCap += hittedEnemy._enemy.GetEnemyMasterData().Cost;
				if (consumedCap >= currentCap)
				{
					break;
				}
				yield return (object)new WaitForSeconds(1f);
				/*Error: Unable to find new state assignment for yield return*/;
			}
			retryCount++;
			if (retryCount >= 200)
			{
				break;
			}
		}
		_currentEnemyCapacity = Mathf.Max(0, _currentEnemyCapacity - consumedCap);
		_currentWaveCount++;
		_isGenerating = false;
		_prepareForActivity = true;
	}

	private FieGameCharacter CreateEnemy(FieGameCharacter waveObject, Vector3 position)
	{
		FieGameCharacter component = waveObject.GetComponent<FieGameCharacter>();
		return FieManagerBehaviour<FieInGameEnemyManager>.I.CreateEnemy(waveObject.GetType(), position);
	}
}
