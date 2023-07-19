using Fie.Object;
using Fie.User;
using Fie.Utility;
using GameDataEditor;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.NEVER_DESTROY)]
	public sealed class FieUserManager : FieManagerBehaviour<FieUserManager>
	{
		public delegate void UserNumChangeDelegate(int beforeNum, int afterNum);

		public const int MAXIMUM_PLAYER_NUM = 3;

		private FieUser[] _userDatas = new FieUser[3];

		private Dictionary<string, int> _userNumCache = new Dictionary<string, int>();

		private int _nowPlayerNum;

		private int _myPlayerNumber;

		private string _myHash;

		private string _myName;

		public string myHash => _myHash;

		public string myName => myName;

		public int myPlayerNumber => _myPlayerNumber;

		public int nowPlayerNum
		{
			get
			{
				return _nowPlayerNum;
			}
			set
			{
				int nowPlayerNum = _nowPlayerNum;
				_nowPlayerNum = Mathf.Min(Mathf.Max(1, value), 3);
				if (nowPlayerNum != _nowPlayerNum && this.UserNumChangeEvent != null)
				{
					this.UserNumChangeEvent(nowPlayerNum, _nowPlayerNum);
					_userNumCache = new Dictionary<string, int>();
				}
			}
		}

		public FieGameCharacter gameOwnerCharacter
		{
			get
			{
				if (this == null)
				{
					return null;
				}
				if (_userDatas == null)
				{
					return null;
				}
				if (_userDatas[myPlayerNumber] == null)
				{
					return null;
				}
				return _userDatas[_myPlayerNumber].usersCharacter;
			}
		}

		public event UserNumChangeDelegate UserNumChangeEvent;

		public FieUser[] getAllUserData()
		{
			return _userDatas;
		}

		public FieUser RegistUser(PhotonPlayer player)
		{
			if (_nowPlayerNum >= 3)
			{
				return null;
			}
			string text = GenerateUserHashByPlayerInfo(player);
			FieUser[] userDatas = _userDatas;
			foreach (FieUser fieUser in userDatas)
			{
				if (fieUser.userHash != null && fieUser.userHash == text)
				{
					return fieUser;
				}
			}
			int num = 0;
			FieUser[] userDatas2 = _userDatas;
			foreach (FieUser fieUser2 in userDatas2)
			{
				if (fieUser2.playerInfo == null)
				{
					break;
				}
				num++;
			}
			string nickName = player.NickName;
			_userDatas[num].playerInfo = player;
			_userDatas[num].userHash = text;
			_userDatas[num].userName = ((!(player.NickName == string.Empty)) ? player.NickName : GetDefaultName(num));
			int num2 = 0;
			FieUser[] userDatas3 = _userDatas;
			foreach (FieUser fieUser3 in userDatas3)
			{
				if (fieUser3.playerInfo != null)
				{
					num2++;
				}
			}
			if (PhotonNetwork.player.ID == player.ID)
			{
				SetMyHash(text);
			}
			nowPlayerNum = num2;
			int num3 = 0;
			FieUser[] userDatas4 = _userDatas;
			foreach (FieUser fieUser4 in userDatas4)
			{
				if (fieUser4.userHash == _myHash)
				{
					_myPlayerNumber = num3;
				}
				num3++;
			}
			return _userDatas[num];
		}

		public void UnregistUser(PhotonPlayer player)
		{
			if (player != null && _nowPlayerNum > 0)
			{
				string text = GenerateUserHashByPlayerInfo(player);
				bool flag = false;
				FieUser[] userDatas = _userDatas;
				foreach (FieUser fieUser in userDatas)
				{
					if (fieUser.userHash == text)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					_userDatas[FieManagerBehaviour<FieUserManager>.I.getUserNumberByHash(text)] = new FieUser();
				}
				int num = 0;
				FieUser[] userDatas2 = _userDatas;
				foreach (FieUser fieUser2 in userDatas2)
				{
					if (fieUser2.playerInfo != null)
					{
						num++;
					}
				}
				nowPlayerNum = num;
			}
		}

		public FieUser GetUserData(int userNumber)
		{
			if (!ValidateUserNumber(userNumber))
			{
				return null;
			}
			return _userDatas[userNumber];
		}

		public FieUser GetUserData(PhotonPlayer playerInfo)
		{
			string b = GenerateUserHashByPlayerInfo(playerInfo);
			FieUser[] userDatas = _userDatas;
			foreach (FieUser fieUser in userDatas)
			{
				if (fieUser.userHash != null && fieUser.userHash == b)
				{
					return fieUser;
				}
			}
			return null;
		}

		public int getUserNumberByHash(string hash)
		{
			if (_userNumCache.ContainsKey(hash))
			{
				return _userNumCache[hash];
			}
			int result = -1;
			for (int i = 0; i < _userDatas.Length; i++)
			{
				if (_userDatas[i].userHash == hash)
				{
					result = i;
					_userNumCache[_userDatas[i].userHash] = i;
					break;
				}
			}
			return result;
		}

		protected override void StartUpEntity()
		{
			InitializeUserData();
		}

		public void RemoveUser(PhotonPlayer targetPlayer)
		{
			if (targetPlayer != null)
			{
				for (int i = 0; i < _userDatas.Length; i++)
				{
					if (_userDatas[i].playerInfo != null && _userDatas[i].playerInfo.ID == targetPlayer.ID)
					{
						_userDatas[i] = new FieUser();
						_userDatas[i].userHash = string.Empty;
						_userDatas[i].userName = string.Empty;
						_userDatas[i].usersCharacterPrefab = null;
						_userDatas[i].usersCharacter = null;
					}
				}
			}
		}

		public void CleanupUserData(bool narrowDistance = false)
		{
			if (PhotonNetwork.connected && PhotonNetwork.room != null)
			{
				for (int i = 0; i < _userDatas.Length; i++)
				{
					if (_userDatas[i] != null && _userDatas[i].playerInfo == null)
					{
						_userDatas[i] = new FieUser();
						_userDatas[i].userHash = string.Empty;
						_userDatas[i].userName = string.Empty;
						_userDatas[i].usersCharacterPrefab = null;
						_userDatas[i].usersCharacter = null;
					}
				}
				if (narrowDistance)
				{
					FieUser[] array = new FieUser[3];
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = new FieUser();
						array[j].userHash = string.Empty;
						array[j].userName = string.Empty;
						array[j].usersCharacterPrefab = null;
						array[j].usersCharacter = null;
					}
					int num = 0;
					for (int k = 0; k < _userDatas.Length; k++)
					{
						if (_userDatas[k] != null && _userDatas[k].playerInfo != null)
						{
							array[num] = _userDatas[k];
							num++;
						}
					}
					_userDatas = array;
				}
				int num2 = 0;
				for (int l = 0; l < _userDatas.Length; l++)
				{
					if (_userDatas[l] != null && _userDatas[l].playerInfo != null)
					{
						_userDatas[l].userName = ((!(_userDatas[l].playerInfo.NickName == string.Empty)) ? _userDatas[l].playerInfo.NickName : GetDefaultName(l));
						num2++;
					}
				}
				nowPlayerNum = num2;
			}
		}

		public void SetMyHash(string hash)
		{
			_myHash = hash;
		}

		public string GenerateUserHash()
		{
			string str = Random.Range(-2147483648, 2147483647).ToString();
			string str2 = Random.Range(-2147483648, 2147483647).ToString();
			string str3 = Random.Range(-2147483648, 2147483647).ToString();
			return CalcMd5(str + str2 + str3);
		}

		public string GenerateUserHashByPlayerInfo(PhotonPlayer playerInfo)
		{
			return CalcMd5(playerInfo.ID.ToString());
		}

		public string GetUserName(int userNumber)
		{
			if (!ValidateUserNumber(userNumber))
			{
				return string.Empty;
			}
			return _userDatas[userNumber].userName;
		}

		public void SetUserName(int userNumber, string userName)
		{
			if (ValidateUserNumber(userNumber))
			{
				_userDatas[userNumber].userName = userName;
			}
		}

		public void SetMyName(string name)
		{
			_myName = name;
			if (PhotonNetwork.player != null)
			{
				PhotonNetwork.player.NickName = _myName;
			}
			for (int i = 0; i < 3; i++)
			{
				if (_userDatas[i] != null && _userDatas[i].playerInfo != null && _userDatas[i].playerInfo.ID == PhotonNetwork.player.ID)
				{
					_userDatas[i].userName = _myName;
				}
			}
		}

		public FieGameCharacter GetUserCharacterPrefab(int userNumber)
		{
			if (!ValidateUserNumber(userNumber))
			{
				return null;
			}
			return _userDatas[userNumber].usersCharacterPrefab;
		}

		public void SetUserCharacterPrefab(int userNumber, FieGameCharacter userCharacterPrefab)
		{
			if (ValidateUserNumber(userNumber))
			{
				_userDatas[userNumber].usersCharacterPrefab = userCharacterPrefab;
			}
		}

		public FieGameCharacter GetUserCharacter(int userNumber)
		{
			if (!ValidateUserNumber(userNumber))
			{
				return null;
			}
			return _userDatas[userNumber].usersCharacter;
		}

		public void SetUserCharacter(int userNumber, FieGameCharacter userCharacter)
		{
			if (ValidateUserNumber(userNumber))
			{
				_userDatas[userNumber].usersCharacter = userCharacter;
				if (userCharacter != null)
				{
					userCharacter.forces = FieEmittableObjectBase.EmitObjectTag.PLAYER;
				}
				if (_userDatas[userNumber].playerInfo != null && _userDatas[userNumber].playerInfo.IsLocal)
				{
					FieManagerBehaviour<FieNetworkManager>.I.SetPlyaersGameCharacterInfoToNetwork(_userDatas[userNumber], userCharacter);
				}
			}
		}

		public void ValidateUserName(int playerNumber)
		{
			if (_userDatas[playerNumber].userName == string.Empty)
			{
				_userDatas[playerNumber].userName = GetDefaultName(playerNumber);
			}
		}

		public void InitializeUserData()
		{
			for (int i = 0; i < 3; i++)
			{
				if (_userDatas[i] == null)
				{
					_userDatas[i] = new FieUser();
					_userDatas[i].userHash = string.Empty;
					_userDatas[i].userName = string.Empty;
					_userDatas[i].usersCharacterPrefab = null;
					_userDatas[i].usersCharacter = null;
				}
			}
		}

		public bool AmIHost()
		{
			if (!PhotonNetwork.connected || PhotonNetwork.room == null || PhotonNetwork.playerList == null || PhotonNetwork.playerList.Length <= 0)
			{
				return true;
			}
			if (PhotonNetwork.masterClient.ID == PhotonNetwork.player.ID)
			{
				return true;
			}
			return false;
		}

		public string GetDefaultName(int playerNumber)
		{
			return "Player " + (playerNumber + 1).ToString();
		}

		public List<T> GetAllPlayerCharacters<T>() where T : FieGameCharacter
		{
			if (_userDatas == null || _userDatas.Length <= 0)
			{
				return null;
			}
			List<T> list = new List<T>();
			FieUser[] userDatas = _userDatas;
			foreach (FieUser fieUser in userDatas)
			{
				if (fieUser != null)
				{
					T val = fieUser.usersCharacter as T;
					if (!((UnityEngine.Object)val == (UnityEngine.Object)null) && !(val.detector.friendTag != "Player"))
					{
						list.Add(val);
					}
				}
			}
			return list;
		}

		public FieGameCharacter GetExistingGameCharacterRandom(GDEGameCharacterTypeData gameCharacterType)
		{
			if (gameCharacterType == null)
			{
				return null;
			}
			if (_nowPlayerNum <= 0)
			{
				return null;
			}
			Lottery<FieGameCharacter> lottery = new Lottery<FieGameCharacter>();
			FieUser[] userDatas = _userDatas;
			foreach (FieUser fieUser in userDatas)
			{
				if (fieUser != null && fieUser.usersCharacter != null && fieUser.usersCharacter.getCharacterTypeData().Key == gameCharacterType.Key)
				{
					lottery.AddItem(fieUser.usersCharacter);
				}
			}
			if (!lottery.IsExecutable())
			{
				return null;
			}
			return lottery.Lot();
		}

		public void AddFriendshipPointToOtherPlayer(FieGameCharacter gainedCharacter, float friendhsip)
		{
			FieUser[] userDatas = _userDatas;
			foreach (FieUser fieUser in userDatas)
			{
				if (!(fieUser.usersCharacter == null) && fieUser.usersCharacter.GetInstanceID() != gainedCharacter.GetInstanceID() && fieUser.usersCharacter != null)
				{
					if (fieUser.usersCharacter.photonView == null || fieUser.usersCharacter.photonView.isMine)
					{
						fieUser.usersCharacter.friendshipStats.safeAddFriendship(friendhsip, triggerEvent: false);
					}
					else
					{
						object[] parameters = new object[1]
						{
							friendhsip
						};
						fieUser.usersCharacter.photonView.RPC("AddFriendshipRPC", PhotonTargets.Others, parameters);
					}
				}
			}
		}

		private bool ValidateUserNumber(int number)
		{
			return number >= 0 && number < 3;
		}

		public string CalcMd5(string srcStr)
		{
			MD5 mD = MD5.Create();
			byte[] bytes = Encoding.UTF8.GetBytes(srcStr);
			byte[] array = mD.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				byte b = array2[i];
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}
	}
}
