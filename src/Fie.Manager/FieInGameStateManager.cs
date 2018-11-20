using Fie.Camera;
using Fie.Core;
using Fie.Object;
using Fie.Scene;
using Fie.User;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.ANYTIME_DESTROY)]
	public class FieInGameStateManager : FieManagerBehaviour<FieInGameStateManager>
	{
		public enum FieInGameState
		{
			STATE_IS_INITIALIZING,
			STATE_IS_RETRYING,
			STATE_NOW_ON_GAME,
			STATE_CUTSCENE,
			STATE_GAMEOVER,
			STATE_ON_QUIT
		}

		public delegate void FieInGameStateChangedCallback();

		public FieInGameState _gameState;

		public Coroutine _checkingGameOverCoroutine;

		private bool _isBooted;

		private int _gameOverCount;

		public bool isBootComplete => _isBooted;

		public event FieInGameStateChangedCallback GameOverEvent;

		public event FieInGameStateChangedCallback RetryEvent;

		public event FieInGameStateChangedCallback RetryFinishedEvent;

		private IEnumerator InGameBootTask()
		{
			_isBooted = false;
			FiePlayerSpawnPoint[] array = UnityEngine.Object.FindObjectsOfType<FiePlayerSpawnPoint>();
			Dictionary<int, Vector3> dictionary = new Dictionary<int, Vector3>();
			if (array != null && array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					dictionary[array[i].spawnPointNumber] = array[i].transform.position;
				}
			}
			if (!FieBootstrap.isBootedFromBootStrap)
			{
				FieManagerBehaviour<FieInputManager>.I.StartUp();
				FieManagerBehaviour<FieAudioManager>.I.StartUp();
				FieManagerBehaviour<FieUserManager>.I.StartUp();
				FieManagerBehaviour<FieVibrationManager>.I.StartUp();
				int num = 0;
				FieUser[] allUserData = FieManagerBehaviour<FieUserManager>.I.getAllUserData();
				foreach (FieUser fieUser in allUserData)
				{
					FieGameCharacter fieGameCharacter = null;
					if (fieUser != null)
					{
						Vector3 position = (!dictionary.ContainsKey(num)) ? Vector3.zero : dictionary[num];
						if (fieUser.usersCharacter != null)
						{
							fieGameCharacter = fieUser.usersCharacter;
							fieGameCharacter.transform.position = position;
							fieGameCharacter.transform.rotation = Quaternion.LookRotation(Vector3.forward);
						}
						else
						{
							if (FieBootstrap.isBootedFromBootStrap)
							{
								Debug.LogError("An users character prefab was missed! please check the class : " + fieUser.usersCharacterPrefab.GetType());
								continue;
							}
							if (fieUser.usersCharacter == null && fieUser.usersCharacterPrefab != null)
							{
								string empty = string.Empty;
								FiePrefabInfo fiePrefabInfo = (FiePrefabInfo)Attribute.GetCustomAttribute(fieUser.usersCharacterPrefab.GetType(), typeof(FiePrefabInfo));
								if (fiePrefabInfo == null)
								{
									Debug.LogError("The debug character prefab dose not have FiePrefabInfo attribute! Please check the class : " + fieUser.usersCharacterPrefab.GetType());
									continue;
								}
								string path = fiePrefabInfo.path;
								if (path == string.Empty)
								{
									Debug.LogError("A prefab path was not found! Please check the class : " + fieUser.usersCharacterPrefab.GetType());
									continue;
								}
								GameObject gameObject = PhotonNetwork.Instantiate(path, position, Quaternion.LookRotation(Vector3.forward), 0);
								fieGameCharacter = (fieUser.usersCharacter = gameObject.GetComponent<FieGameCharacter>());
							}
						}
						if (fieGameCharacter != null)
						{
							if (!FieBootstrap.isBootedFromBootStrap)
							{
								fieGameCharacter.InitializeIntelligenceSystem();
							}
							if (PhotonNetwork.offlineMode && fieUser.usersCharacter.intelligenceType == FieGameCharacter.IntelligenceType.Controllable)
							{
								fieGameCharacter.SetOwnerUser(fieUser);
							}
							else if (fieGameCharacter.photonView.ownerId == PhotonNetwork.player.ID)
							{
								fieGameCharacter.SetOwnerUser(fieUser);
							}
							FieManagerBehaviour<FieInGameCharacterStatusManager>.I.AssignCharacter(FieInGameCharacterStatusManager.ForcesTag.PLAYER, fieGameCharacter);
						}
						num++;
					}
				}
			}
			else
			{
				int num2 = 0;
				List<FieGameCharacter> allPlayerCharacters = FieManagerBehaviour<FieUserManager>.I.GetAllPlayerCharacters<FieGameCharacter>();
				foreach (FieGameCharacter item in allPlayerCharacters)
				{
					if (item.forces == FieEmittableObjectBase.EmitObjectTag.PLAYER)
					{
						Vector3 vector2 = item.position = ((!dictionary.ContainsKey(num2)) ? Vector3.zero : dictionary[num2]);
						FieManagerBehaviour<FieInGameCharacterStatusManager>.I.AssignCharacter(FieInGameCharacterStatusManager.ForcesTag.PLAYER, item);
						num2++;
					}
				}
			}
			FieManagerBehaviour<FieGameCameraManager>.I.StartUp();
			FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.SetCameraTask<FieGameCameraTaskStop>();
			FieManagerBehaviour<FieGUIManager>.I.StartUp();
			_isBooted = true;
			_gameState = FieInGameState.STATE_NOW_ON_GAME;
			yield break;
		}

		private IEnumerator RetryTask()
		{
			yield return (object)new WaitForSeconds(0.1f);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		private void GameOverCallback()
		{
			FieManagerBehaviour<FieInputManager>.I.isEnableControll = false;
		}

		protected override void StartUpEntity()
		{
			_gameState = FieInGameState.STATE_IS_INITIALIZING;
			StartCoroutine(InGameBootTask());
			if (_checkingGameOverCoroutine != null)
			{
				StopCoroutine(_checkingGameOverCoroutine);
			}
			_checkingGameOverCoroutine = StartCoroutine(CheckingGameOverTask());
		}

		private IEnumerator CheckingGameOverTask()
		{
			if (_gameState == FieInGameState.STATE_NOW_ON_GAME)
			{
				if (FieManagerBehaviour<FieInGameCharacterStatusManager>.I.isAllPlayerDied())
				{
					_gameOverCount = Mathf.Clamp(_gameOverCount + 1, 1, 5);
				}
				if (_gameOverCount > 3)
				{
					SetGameState(FieInGameState.STATE_GAMEOVER);
				}
				yield return (object)new WaitForSeconds(1f);
				/*Error: Unable to find new state assignment for yield return*/;
			}
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public void SetGameState(FieInGameState state)
		{
			if (state != _gameState)
			{
				switch (state)
				{
				case FieInGameState.STATE_GAMEOVER:
					FieManagerBehaviour<FieNetworkManager>.I.SendGameOverEvent();
					if (_checkingGameOverCoroutine != null)
					{
						StopCoroutine(_checkingGameOverCoroutine);
					}
					if (this.GameOverEvent != null)
					{
						this.GameOverEvent();
					}
					break;
				case FieInGameState.STATE_IS_RETRYING:
					FieManagerBehaviour<FieNetworkManager>.I.SendRetryEvent();
					StartCoroutine(RetryTask());
					break;
				case FieInGameState.STATE_ON_QUIT:
					FieManagerBehaviour<FieNetworkManager>.I.SendGameQuitEvent();
					QuitGame();
					break;
				}
				_gameState = state;
			}
		}

		private void QuitGame()
		{
			FieManagerBehaviour<FieSceneManager>.I.LoadScene(new FieSceneResult(), allowSceneActivation: true, FieFaderManager.FadeType.OUT_TO_WHITE, 1f);
			FieManagerBehaviour<FieAudioManager>.I.ChangeMixerVolume(0f, 0.5f, FieAudioManager.FieAudioMixerType.BGM, FieAudioManager.FieAudioMixerType.Voice, FieAudioManager.FieAudioMixerType.SE);
			if (_checkingGameOverCoroutine != null)
			{
				StopCoroutine(_checkingGameOverCoroutine);
			}
		}

		public void SetGameOver()
		{
			if (this.GameOverEvent != null)
			{
				this.GameOverEvent();
			}
		}

		internal void SetRetry()
		{
			if (_checkingGameOverCoroutine != null)
			{
				StopCoroutine(_checkingGameOverCoroutine);
			}
			StartCoroutine(RetryTask());
		}

		internal void SetQuit()
		{
			QuitGame();
		}

		private void OnDestroy()
		{
			if (_checkingGameOverCoroutine != null)
			{
				StopCoroutine(_checkingGameOverCoroutine);
			}
		}
	}
}
