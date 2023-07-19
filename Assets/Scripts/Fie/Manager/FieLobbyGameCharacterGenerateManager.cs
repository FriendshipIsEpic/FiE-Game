using Fie.Object;
using Fie.Ponies;
using Fie.User;
using UnityEngine;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.OUTGAME)]
	public class FieLobbyGameCharacterGenerateManager : FieManagerBehaviour<FieLobbyGameCharacterGenerateManager>
	{
		public delegate void LobbyGameCharacterCreatedCallback();

		private Vector3 _playerInitPosition = Vector3.zero;

		public Vector3 playerInitPosition => _playerInitPosition;

		protected override void StartUpEntity()
		{
			FiePlayerSpawnPoint[] array = UnityEngine.Object.FindObjectsOfType<FiePlayerSpawnPoint>();
			if (array != null && array.Length > 0)
			{
				int num = 2147483647;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].spawnPointNumber < num)
					{
						_playerInitPosition = array[i].transform.position;
					}
				}
			}
		}

		public void RequestToCreateGameCharacter<T>(FieUser user, LobbyGameCharacterCreatedCallback lobbyCallback) where T : FieGameCharacter
		{
			FieGameCharacterManager.GameCharacterCreatedCallback<T> callback = delegate(T gameCharacter)
			{
				user.usersCharacter = gameCharacter;
				if (FieManagerBehaviour<FieUserManager>.I.myHash == user.userHash)
				{
					FieManagerBehaviour<FieGameCameraManager>.I.StartUp();
					FieManagerBehaviour<FieGUIManager>.I.ReloadUIOwner();
					FieManagerBehaviour<FieGUIManager>.I.ShowHeaderFooter();
					FieManagerBehaviour<FieUserManager>.I.SetUserCharacter(FieManagerBehaviour<FieUserManager>.I.getUserNumberByHash(FieManagerBehaviour<FieUserManager>.I.myHash), gameCharacter);
					FieManagerBehaviour<FieUserManager>.I.ValidateUserName(FieManagerBehaviour<FieUserManager>.I.myPlayerNumber);
					gameCharacter.InitializeIntelligenceSystem(FieGameCharacter.IntelligenceType.Controllable);
				}
				else if (!PhotonNetwork.offlineMode)
				{
					gameCharacter.InitializeIntelligenceSystem(FieGameCharacter.IntelligenceType.Connection);
				}
				else
				{
					gameCharacter.InitializeIntelligenceSystem(FieGameCharacter.IntelligenceType.AI);
				}
				Vector3 origin = _playerInitPosition + Vector3.up;
				Vector3 normalized = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
				Vector3 vector = _playerInitPosition + normalized * 1f;
				int layerMask = 1049088;
				if (Physics.Raycast(origin, normalized, out RaycastHit hitInfo, 1f, layerMask))
				{
					vector = hitInfo.point;
				}
				layerMask = 512;
				if (Physics.Raycast(vector, Vector3.down, out hitInfo, 20f, layerMask) && hitInfo.collider.tag == "Floor")
				{
					vector = hitInfo.point;
				}
				gameCharacter.position = vector;
				gameCharacter.transform.SetParent(FieManagerBehaviour<FieGameCharacterManager>.I.transform);
				user.usersCharacter = gameCharacter;
				gameCharacter.SetOwnerUser(user);
				gameCharacter.getStateMachine().setState(typeof(FieStateMachinePoniesArrival), isForceSet: true);
				FieManagerBehaviour<FieNetworkManager>.I.RebuildPlayerInformationCommand();
				if (lobbyCallback != null)
				{
					lobbyCallback();
				}
			};
			FieManagerBehaviour<FieGameCharacterManager>.I.CreateGameCharacter(callback);
		}
	}
}
