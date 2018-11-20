using ExitGames.Client.Photon;
using Fie.User;
using GameDataEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.NEVER_DESTROY)]
	public class FieNetworkManager : FieManagerBehaviour<FieNetworkManager>
	{
		private enum FieNetworkChannels
		{
			UNRELIABLE,
			RELIABLE,
			ALL_COST_DELIVERY,
			CHANNEL_MAX
		}

		public enum FieNetworkManagerState
		{
			IDLE,
			TRANSACTING,
			NOW_HOSTING,
			NOW_CLIENT
		}

		public enum FieNetowrkErrorCode
		{
			SUCCEED,
			ERROR_TO_CREATE_ROOM,
			ERROR_TO_JOIN_ROOM,
			ERROR_TO_CONNECT_MASTER_SERVER
		}

		public delegate void FieNetworkCallback(FieNetowrkErrorCode errorCode);

		public const string ROOM_VIEW_ID = "network_manager_view_id";

		public const string ROOM_ENEMY_FORCES = "enemy_forces";

		public const string ROOM_DIFFICULTY = "difficulty";

		public const string ROOM_GAME_MODE = "mode";

		public const string USER_CHARCTER_TYPE_KEY = "game_character_type";

		public const string USER_CHARCTER_VIEW_ID = "game_character_viwe_id";

		public const string USER_PLAYER_LEVEL = "player_level";

		public const string USER_CHARACTER_LEVEL = "character_level";

		public const string USER_SCENE_ID = "scene_id";

		private const ulong UNET_ID = 31203uL;

		private const string PROJECT_ID = "FiE";

		private const string PLAYERPREFS_CLOUD_NETWORKING_ID_KEY = "CloudNetworkingId";

		private FieNetworkCallback _createMatchCallback;

		private FieNetworkCallback _joinMatchCallback;

		private FieNetworkCallback _disconnectmatchCallback;

		private Coroutine _disconnectCoroutine;

		private ulong _currentMatchId;

		private FieNetworkManagerState _nowState;

		public CloudRegionCode masterServerRegion = CloudRegionCode.none;

		private ExitGames.Client.Photon.Hashtable _myPlayerInfoHashTable = new ExitGames.Client.Photon.Hashtable();

		private ExitGames.Client.Photon.Hashtable _myRoomInfoHashTable = new ExitGames.Client.Photon.Hashtable();

		public event FieNetworkCallback connectedToMasterServerEvent;

		public event FieNetworkCallback feiledToConnectToMasterServerEvent;

		private IEnumerator DisconnectCoroutine()
		{
			if (_nowState != 0)
			{
				if (_nowState == FieNetworkManagerState.TRANSACTING)
				{
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				PhotonNetwork.Disconnect();
			}
		}

		private void Awake()
		{
			base.gameObject.AddComponent<PhotonView>();
		}

		public void SetMyCharacterTypeKey(string characterTypeKey)
		{
			if (!PhotonNetwork.offlineMode && PhotonNetwork.player != null)
			{
				_myPlayerInfoHashTable["game_character_type"] = characterTypeKey;
				PhotonNetwork.player.SetCustomProperties(_myPlayerInfoHashTable);
			}
		}

		public void SetMyCharacterViewId(int characterViewId)
		{
			if (!PhotonNetwork.offlineMode && PhotonNetwork.player != null)
			{
				_myPlayerInfoHashTable["game_character_viwe_id"] = characterViewId;
				PhotonNetwork.player.SetCustomProperties(_myPlayerInfoHashTable);
			}
		}

		public void SetMyPlayerLevel(int playerLevel)
		{
			if (!PhotonNetwork.offlineMode && PhotonNetwork.player != null)
			{
				_myPlayerInfoHashTable["player_level"] = playerLevel;
				PhotonNetwork.player.SetCustomProperties(_myPlayerInfoHashTable);
			}
		}

		public void SetMyCharacterLevel(int characterLevel)
		{
			if (!PhotonNetwork.offlineMode && PhotonNetwork.player != null)
			{
				_myPlayerInfoHashTable["character_level"] = characterLevel;
				PhotonNetwork.player.SetCustomProperties(_myPlayerInfoHashTable);
			}
		}

		public void SetMySceneId(int sceneId)
		{
			if (!PhotonNetwork.offlineMode && PhotonNetwork.player != null)
			{
				_myPlayerInfoHashTable["scene_id"] = sceneId;
				PhotonNetwork.player.SetCustomProperties(_myPlayerInfoHashTable);
			}
		}

		public void SetRoomDifficulty(int difficulty)
		{
			if (!PhotonNetwork.offlineMode && PhotonNetwork.room != null && PhotonNetwork.inRoom && PhotonNetwork.isMasterClient)
			{
				_myRoomInfoHashTable["difficulty"] = difficulty;
				PhotonNetwork.room.SetCustomProperties(_myRoomInfoHashTable);
			}
		}

		public void SetRoomInfo(int sceneViewId, FieEnvironmentManager.GameMode gameMode, FieEnvironmentManager.Difficulty difficulty)
		{
			if (!PhotonNetwork.offlineMode && PhotonNetwork.room != null)
			{
				_myRoomInfoHashTable["network_manager_view_id"] = sceneViewId;
				_myRoomInfoHashTable["mode"] = (int)gameMode;
				_myRoomInfoHashTable["difficulty"] = (int)difficulty;
				PhotonNetwork.room.SetCustomProperties(_myRoomInfoHashTable);
			}
		}

		protected override void StartUpEntity()
		{
			PlayerPrefs.SetString("CloudNetworkingId", "MasterData/fie_master_data");
		}

		public bool CheckPlayerScenesIsSame()
		{
			if (PhotonNetwork.offlineMode || !PhotonNetwork.inRoom)
			{
				return true;
			}
			if (PhotonNetwork.otherPlayers.Length <= 0)
			{
				return true;
			}
			int num = (int)PhotonNetwork.player.CustomProperties["scene_id"];
			bool result = true;
			for (int i = 0; i < PhotonNetwork.otherPlayers.Length; i++)
			{
				if (!PhotonNetwork.otherPlayers[i].CustomProperties.ContainsKey("scene_id"))
				{
					result = false;
					break;
				}
				int num2 = (int)PhotonNetwork.otherPlayers[i].CustomProperties["scene_id"];
				if (num2 != num)
				{
					result = false;
					break;
				}
			}
			return result;
		}

		public void CreateNetowkRoom(FieNetworkCallback callback, string roomName = "")
		{
			if (_nowState == FieNetworkManagerState.IDLE)
			{
				if (roomName == string.Empty)
				{
					roomName = FieManagerBehaviour<FieUserManager>.I.CalcMd5(UnityEngine.Random.Range(0, 2147483647).ToString());
				}
				roomName = "FiERoom_" + roomName;
				RoomOptions roomOptions = new RoomOptions();
				roomOptions.MaxPlayers = 3;
				TypedLobby typedLobby = new TypedLobby();
				typedLobby.Type = LobbyType.Default;
				PhotonNetwork.CreateRoom(roomName, roomOptions, typedLobby);
				FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_LOBBY_INFORMATION_TITLE_ANY_NETWORK), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_LOBBY_WAITING_FOR_CREATE_ROOM), 2f);
				_createMatchCallback = callback;
				_nowState = FieNetworkManagerState.TRANSACTING;
			}
		}

		public void JoinNetowkRoom(FieNetworkCallback callback, string password)
		{
			if (_nowState == FieNetworkManagerState.IDLE)
			{
				PhotonNetwork.JoinRandomRoom();
				FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_LOBBY_INFORMATION_TITLE_ANY_NETWORK), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_LOBBY_WAITING_FOR_MATCH), 2f);
				_joinMatchCallback = callback;
				_nowState = FieNetworkManagerState.TRANSACTING;
			}
		}

		public void Disconnect(FieNetworkCallback callback)
		{
			if (_nowState != 0)
			{
				_disconnectmatchCallback = callback;
				if (_disconnectCoroutine != null)
				{
					StopCoroutine(_disconnectCoroutine);
				}
				_disconnectCoroutine = StartCoroutine(DisconnectCoroutine());
			}
		}

		public void RegsitSpawnablePrefab(GameObject targetObject)
		{
			ClientScene.RegisterPrefab(targetObject);
		}

		private void OnCreatedRoom()
		{
			if (_createMatchCallback != null)
			{
				FieNetowrkErrorCode errorCode = (PhotonNetwork.room == null) ? FieNetowrkErrorCode.ERROR_TO_CREATE_ROOM : FieNetowrkErrorCode.SUCCEED;
				_createMatchCallback(errorCode);
			}
			_createMatchCallback = null;
			base.photonView.viewID = PhotonNetwork.AllocateSceneViewID();
			base.photonView.RequestOwnership();
			SetRoomInfo(base.photonView.viewID, FieManagerBehaviour<FieEnvironmentManager>.I.currentGameMode, FieManagerBehaviour<FieEnvironmentManager>.I.currentDifficulty);
			FieManagerBehaviour<FieUserManager>.I.RegistUser(PhotonNetwork.player);
			_nowState = FieNetworkManagerState.IDLE;
		}

		private void OnJoinedRoom()
		{
			if (_joinMatchCallback != null)
			{
				FieNetowrkErrorCode errorCode = (PhotonNetwork.room == null) ? FieNetowrkErrorCode.ERROR_TO_JOIN_ROOM : FieNetowrkErrorCode.SUCCEED;
				_joinMatchCallback(errorCode);
			}
			int num = (int)PhotonNetwork.room.CustomProperties["network_manager_view_id"];
			if (num != base.photonView.viewID)
			{
				base.photonView.viewID = num;
				base.photonView.TransferOwnership(PhotonNetwork.masterClient);
			}
			if (!PhotonNetwork.isMasterClient)
			{
				FieManagerBehaviour<FieEnvironmentManager>.I.SetDifficulty((FieEnvironmentManager.Difficulty)PhotonNetwork.room.CustomProperties["difficulty"]);
				FieManagerBehaviour<FieEnvironmentManager>.I.currentGameMode = (FieEnvironmentManager.GameMode)PhotonNetwork.room.CustomProperties["mode"];
			}
			_joinMatchCallback = null;
			StartCoroutine(RebuildPlayersByRoomInfo());
			SetMyPlayerLevel(FieManagerBehaviour<FieSaveManager>.I.onMemorySaveData.PlayerLevel);
			_nowState = FieNetworkManagerState.IDLE;
		}

		private void OnPhotonCreateRoomFailed(object[] codeAndMsg)
		{
			if (_createMatchCallback != null)
			{
				FieNetowrkErrorCode errorCode = FieNetowrkErrorCode.ERROR_TO_CREATE_ROOM;
				_createMatchCallback(errorCode);
			}
			FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_LOBBY_ERROR_TITLE_ANY_NETWORK), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_LOBBY_ERROR_FAILD_TO_CREATE_ROOM), 4f);
			_createMatchCallback = null;
			_nowState = FieNetworkManagerState.IDLE;
		}

		private void OnPhotonJoinRoomFailed(object[] codeAndMsg)
		{
			if (_joinMatchCallback != null)
			{
				FieNetowrkErrorCode errorCode = FieNetowrkErrorCode.ERROR_TO_JOIN_ROOM;
				_joinMatchCallback(errorCode);
			}
			FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_LOBBY_ERROR_TITLE_ANY_NETWORK), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_LOBBY_ERROR_FAILD_TO_JOIN_ROOM), 4f);
			_joinMatchCallback = null;
			_nowState = FieNetworkManagerState.IDLE;
		}

		public void ConnectToMasterServer()
		{
			if (!PhotonNetwork.connected)
			{
				string gameVersion = "FiE_" + FieManagerBehaviour<FieEnvironmentManager>.I.getVersionString();
				if (masterServerRegion == CloudRegionCode.none)
				{
					PhotonNetwork.ConnectToBestCloudServer(gameVersion);
				}
				else
				{
					PhotonNetwork.ConnectToRegion(masterServerRegion, gameVersion);
				}
				PhotonNetwork.sendRate = 30;
				PhotonNetwork.sendRateOnSerialize = 30;
				PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
			}
		}

		private void OnPhotonRandomJoinFailed()
		{
			OnPhotonJoinRoomFailed(null);
		}

		private void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
		{
		}

		private IEnumerator RebuildPlayersByRoomInfo()
		{
			if (PhotonNetwork.room != null && PhotonNetwork.playerList != null && PhotonNetwork.playerList.Length > 0)
			{
				FieManagerBehaviour<FieUserManager>.I.InitializeUserData();
				yield return (object)new WaitForSeconds(1f);
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public void SetPlyaersGameCharacterInfoToNetwork(FieUser user, FieGameCharacter userCharacter)
		{
			if (user != null && user.playerInfo != null && !(userCharacter == null))
			{
				PhotonView component = userCharacter.GetComponent<PhotonView>();
				if (!(component == null))
				{
					if (!user.playerInfo.IsLocal)
					{
						ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
						hashtable.Add("game_character_type", userCharacter.GetType().FullName);
						hashtable.Add("game_character_viwe_id", component.viewID);
						user.playerInfo.SetCustomProperties(hashtable);
					}
					else
					{
						SetMyCharacterTypeKey(userCharacter.GetType().FullName);
						SetMyCharacterViewId(component.viewID);
					}
				}
			}
		}

		public KeyValuePair<int, Type> GetPlayersGameCharacterTypeFromNetwork(FieUser user)
		{
			KeyValuePair<int, Type> result = default(KeyValuePair<int, Type>);
			if (user == null || user.playerInfo == null)
			{
				return result;
			}
			ExitGames.Client.Photon.Hashtable customProperties = user.playerInfo.CustomProperties;
			if (customProperties == null || !customProperties.ContainsKey("game_character_type") || !customProperties.ContainsKey("game_character_viwe_id"))
			{
				return result;
			}
			string typeName = customProperties["game_character_type"] as string;
			int key = (int)customProperties["game_character_viwe_id"];
			return new KeyValuePair<int, Type>(key, Type.GetType(typeName));
		}

		public void OnPhotonPlayerDisconnected(PhotonPlayer player)
		{
			FieManagerBehaviour<FieUserManager>.I.RemoveUser(player);
			FieManagerBehaviour<FieUserManager>.I.CleanupUserData(narrowDistance: true);
			FieManagerBehaviour<FieGUIManager>.I.ReloadPlayerWindow();
		}

		public void OnCustomAuthenticationFailed(string debugMessage)
		{
			SetOfflineMode();
		}

		public void OnConnectionFail(DisconnectCause cause)
		{
			SetOfflineMode();
		}

		public void OnFailedToConnectToPhoton(DisconnectCause cause)
		{
			SetOfflineMode();
		}

		public void OnConnectedToMaster()
		{
			if (!PhotonNetwork.offlineMode)
			{
				FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_LOBBY_INFORMATION_TITLE_ANY_NETWORK), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_SUCCEED_TO_CONNECT_MASTER_SERVER), 3f);
				if (this.connectedToMasterServerEvent != null)
				{
					this.connectedToMasterServerEvent(FieNetowrkErrorCode.SUCCEED);
				}
			}
			else if (this.feiledToConnectToMasterServerEvent != null)
			{
				this.feiledToConnectToMasterServerEvent(FieNetowrkErrorCode.ERROR_TO_CONNECT_MASTER_SERVER);
			}
		}

		public void SetOfflineMode()
		{
			FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_LOBBY_INFORMATION_TITLE_ANY_NETWORK), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_BOOT_IN_OFFLINE_MODE), 3f);
			if (this.feiledToConnectToMasterServerEvent != null)
			{
				this.feiledToConnectToMasterServerEvent(FieNetowrkErrorCode.ERROR_TO_CONNECT_MASTER_SERVER);
			}
			PhotonNetwork.offlineMode = true;
		}

		public void RebuildPlayerInformationCommand()
		{
			base.photonView.RPC("RebuildPlayerInformationRPC", PhotonTargets.All, null);
		}

		[PunRPC]
		public void SendGameCharacterBuildDataRPC()
		{
		}

		[PunRPC]
		public void RebuildPlayerInformationRPC()
		{
			FieManagerBehaviour<FieUserManager>.I.CleanupUserData();
			FieGameCharacter[] array = UnityEngine.Object.FindObjectsOfType<FieGameCharacter>();
			int num = 0;
			PhotonPlayer[] playerList = PhotonNetwork.playerList;
			foreach (PhotonPlayer photonPlayer in playerList)
			{
				if (photonPlayer != null)
				{
					if (!photonPlayer.IsLocal)
					{
						FieUser fieUser = FieManagerBehaviour<FieUserManager>.I.GetUserData(photonPlayer);
						if (fieUser == null)
						{
							fieUser = FieManagerBehaviour<FieUserManager>.I.RegistUser(photonPlayer);
						}
						if (fieUser != null && array != null && array.Length > 0)
						{
							FieGameCharacter[] array2 = array;
							foreach (FieGameCharacter fieGameCharacter in array2)
							{
								PhotonView component = fieGameCharacter.GetComponent<PhotonView>();
								if (!(component == null) && component.owner.ID == fieUser.playerInfo.ID)
								{
									fieUser.usersCharacter = fieGameCharacter;
									fieUser.usersCharacterPrefab = fieGameCharacter;
									fieGameCharacter.position = FieManagerBehaviour<FieLobbyGameCharacterGenerateManager>.I.playerInitPosition;
									fieGameCharacter.InitializeIntelligenceSystem(FieGameCharacter.IntelligenceType.Connection);
									fieGameCharacter.transform.SetParent(FieManagerBehaviour<FieGameCharacterManager>.I.transform);
									fieGameCharacter.SetOwnerUser(fieUser);
								}
							}
						}
					}
					num++;
				}
			}
			FieManagerBehaviour<FieGUIManager>.I.ReloadPlayerWindow();
		}

		public void SendGameOverEvent()
		{
			if (PhotonNetwork.isMasterClient)
			{
				base.photonView.RPC("SendGameOverEventRpc", PhotonTargets.Others, null);
			}
		}

		[PunRPC]
		public void SendGameOverEventRpc()
		{
			FieManagerBehaviour<FieInGameStateManager>.I.SetGameOver();
		}

		public void SendRetryEvent()
		{
			if (PhotonNetwork.isMasterClient)
			{
				base.photonView.RPC("SendRetryEventRpc", PhotonTargets.Others, null);
			}
		}

		[PunRPC]
		public void SendRetryEventRpc()
		{
			FieManagerBehaviour<FieInGameStateManager>.I.SetRetry();
		}

		public void SendGameQuitEvent()
		{
			if (PhotonNetwork.isMasterClient)
			{
				base.photonView.RPC("SendGameQuitEventRpc", PhotonTargets.Others, null);
			}
		}

		[PunRPC]
		public void SendGameQuitEventRpc()
		{
			FieManagerBehaviour<FieInGameStateManager>.I.SetQuit();
		}

		private void OnApplicationQuit()
		{
			PhotonNetwork.Disconnect();
		}

		private void OnDestroy()
		{
			if (PhotonNetwork.inRoom)
			{
				PhotonNetwork.LeaveRoom();
			}
		}
	}
}
