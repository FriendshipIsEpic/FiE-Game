using Fie.Object;
using Fie.Scene;
using GameDataEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.NEVER_DESTROY)]
	public sealed class FieSaveManager : FieManagerBehaviour<FieSaveManager>
	{
		private static string password = string.Empty;

		private static string directory = "FieData";

		private static string fileName = "FieSaveData.bin";

		private static string backupFileName = "FieSaveData_Backup.bin";

		public static List<GDESkillTreeData> debugSkills = new List<GDESkillTreeData>();

		public Dictionary<string, int> snapshottedExpList = new Dictionary<string, int>();

		public Dictionary<string, int> currentExpList = new Dictionary<string, int>();

		private FieSaveData _onMemorySaveData = new FieSaveData();

		public FieSaveData onMemorySaveData
		{
			get
			{
				return _onMemorySaveData;
			}
			set
			{
				_onMemorySaveData = value;
			}
		}

		protected override void StartUpEntity()
		{
			LoadAllSaveData();
			FieManagerBehaviour<FieSceneManager>.I.FiePreparedForLoadSceneEvent += I_FiePreparedForLoadSceneEvent;
		}

		private void I_FiePreparedForLoadSceneEvent(FieSceneBase targetScene)
		{
			SaveAllData();
		}

		private void LoadAllSaveData()
		{
			TryToLoad(ref _onMemorySaveData.LanguageCode, "LanguageCode");
			TryToLoad(ref _onMemorySaveData.PlayerLevel, "PlayerLevel");
			TryToLoad(ref _onMemorySaveData.LastLoginedUserName, "LastLoginedUserName");
			TryToLoad(ref _onMemorySaveData.LastLoginedPasswordPassword, "LastLoginedPasswordPassword");
			TryToLoadDictionary(ref _onMemorySaveData.CharacterExp, "CharacterExp");
			TryToLoadDictionary(ref _onMemorySaveData.CharacterSkillPoint, "CharacterSkillPoint");
			TryToLoadDictionary(ref _onMemorySaveData.PromotedCount, "PromotedCount");
			TryToLoadList(ref _onMemorySaveData.unlockedSkills, "SkillTreeUnlocked");
			TryToLoadDictionary(ref _onMemorySaveData.AchivemenetProgress, "AchivemenetProgress");
			ValidateSaveDataByMasterData();
		}

		private void ValidateSaveDataByMasterData()
		{
			bool flag = false;
			if (_onMemorySaveData.CharacterExp == null)
			{
				_onMemorySaveData.CharacterExp = new Dictionary<int, int>();
				flag = true;
			}
			if (_onMemorySaveData.CharacterSkillPoint == null)
			{
				_onMemorySaveData.CharacterSkillPoint = new Dictionary<int, int>();
				flag = true;
			}
			if (_onMemorySaveData.PromotedCount == null)
			{
				_onMemorySaveData.PromotedCount = new Dictionary<int, int>();
				flag = true;
			}
			Dictionary<string, GDEGameCharacterTypeData> allMasterData = FieMasterData<GDEGameCharacterTypeData>.I.GetAllMasterData();
			foreach (KeyValuePair<string, GDEGameCharacterTypeData> item in allMasterData)
			{
				if (!_onMemorySaveData.CharacterExp.ContainsKey(item.Value.ID))
				{
					_onMemorySaveData.CharacterExp[item.Value.ID] = 0;
					flag = true;
				}
				if (!_onMemorySaveData.CharacterSkillPoint.ContainsKey(item.Value.ID))
				{
					_onMemorySaveData.CharacterSkillPoint[item.Value.ID] = 0;
					flag = true;
				}
				if (!_onMemorySaveData.PromotedCount.ContainsKey(item.Value.ID))
				{
					_onMemorySaveData.PromotedCount[item.Value.ID] = 0;
					flag = true;
				}
			}
			if (_onMemorySaveData.AchivemenetProgress == null)
			{
				_onMemorySaveData.AchivemenetProgress = new Dictionary<int, int>();
				flag = true;
			}
			Dictionary<string, GDEAchievementData> allMasterData2 = FieMasterData<GDEAchievementData>.I.GetAllMasterData();
			foreach (KeyValuePair<string, GDEAchievementData> item2 in allMasterData2)
			{
				if (!_onMemorySaveData.AchivemenetProgress.ContainsKey(item2.Value.ID))
				{
					_onMemorySaveData.AchivemenetProgress[item2.Value.ID] = 0;
					flag = true;
				}
			}
			if (flag)
			{
				SaveAllData();
			}
		}

		internal FieGameCharacterBuildData GetGameCharacterBuildData(GDEGameCharacterTypeData gameCharacterTypeData)
		{
			FieGameCharacterBuildData result = default(FieGameCharacterBuildData);
			result.levelInfo = GetCharacterLevelInfo(gameCharacterTypeData);
			result.skillPoint = onMemorySaveData.CharacterSkillPoint[gameCharacterTypeData.ID];
			result.promotedCount = onMemorySaveData.PromotedCount[gameCharacterTypeData.ID];
			return result;
		}

		private void SaveAllData()
		{
			Save(_onMemorySaveData.LanguageCode, "LanguageCode");
			Save(_onMemorySaveData.PlayerLevel, "PlayerLevel");
			Save(_onMemorySaveData.LastLoginedUserName, "LastLoginedUserName");
			Save(_onMemorySaveData.LastLoginedPasswordPassword, "LastLoginedPasswordPassword");
			SaveDictionary(_onMemorySaveData.CharacterExp, "CharacterExp");
			SaveDictionary(_onMemorySaveData.CharacterSkillPoint, "CharacterSkillPoint");
			SaveDictionary(_onMemorySaveData.PromotedCount, "PromotedCount");
			SaveList(_onMemorySaveData.unlockedSkills, "SkillTreeUnlocked");
			SaveDictionary(_onMemorySaveData.AchivemenetProgress, "AchivemenetProgress");
		}

		private void Save<T>(T value, string variableName)
		{
			ES2.Save(value, GetIdentifier(variableName));
			ES2.Save(value, GetIdentifier(variableName, isBackupMode: true));
		}

		private void SaveList<T>(List<T> value, string variableName)
		{
			ES2.Save(value, GetIdentifier(variableName));
			ES2.Save(value, GetIdentifier(variableName, isBackupMode: true));
		}

		private void SaveDictionary<T, Y>(Dictionary<T, Y> value, string variableName)
		{
			ES2.Save(value, GetIdentifier(variableName));
			ES2.Save(value, GetIdentifier(variableName, isBackupMode: true));
		}

		private bool IsExists(string variableName, bool isBackupMode = false)
		{
			return ES2.Exists(GetIdentifier(variableName, isBackupMode));
		}

		private void TryToLoad<T>(ref T variable, string variableName)
		{
			if (IsExists(variableName))
			{
				try
				{
					variable = ES2.Load<T>(GetIdentifier(variableName));
					return;
				}
				catch
				{
				}
				if (IsExists(variableName, isBackupMode: true))
				{
					Debug.LogWarning("The save data was not found at master save data. FiE save system will try to load that from backup. The variable name :" + variableName);
					try
					{
						variable = ES2.Load<T>(GetIdentifier(variableName, isBackupMode: true));
					}
					catch
					{
						Debug.LogWarning("Faild to load a save data from backup. The save data was completery broken. It might be initialized. The variable name :" + variableName);
					}
				}
				else
				{
					Debug.Log("The variable dose not exist on save datas. It might be initialize. The valiable name : " + variableName);
				}
			}
		}

		private void TryToLoadList<T>(ref List<T> variable, string variableName)
		{
			if (IsExists(variableName))
			{
				try
				{
					variable = ES2.LoadList<T>(GetIdentifier(variableName));
					return;
				}
				catch
				{
				}
				if (IsExists(variableName, isBackupMode: true))
				{
					Debug.LogWarning("The save data was not found at master save data. FiE save system will try to load that from backup. The variable name :" + variableName);
					try
					{
						variable = ES2.LoadList<T>(GetIdentifier(variableName, isBackupMode: true));
					}
					catch
					{
						Debug.LogWarning("Failed to load a save data from backup. The save data was completery broken. It might be initialized. The variable name :" + variableName);
					}
				}
				else
				{
					Debug.Log("The variable dose not exist on save datas. It might be initialize. The valiable name : " + variableName);
				}
			}
		}

		private void TryToLoadDictionary<T, Y>(ref Dictionary<T, Y> variable, string variableName)
		{
			if (IsExists(variableName))
			{
				try
				{
					variable = ES2.LoadDictionary<T, Y>(GetIdentifier(variableName));
					return;
				}
				catch
				{
				}
				if (IsExists(variableName, isBackupMode: true))
				{
					Debug.LogWarning("The save data was not found at master save data. FiE save system will try to load that from backup. The variable name :" + variableName);
					try
					{
						variable = ES2.LoadDictionary<T, Y>(GetIdentifier(variableName, isBackupMode: true));
					}
					catch
					{
						Debug.LogWarning("Failed to load a save data from backup. The save data was completery broken. It might be initialized. The variable name :" + variableName);
					}
				}
				else
				{
					Debug.Log("The variable dose not exist on save datas. It might be initialize. The valiable name : " + variableName);
				}
			}
		}

		private string GetIdentifier(string variableName, bool isBackupMode = false)
		{
			return directory + "/" + ((!isBackupMode) ? fileName : backupFileName) + "?tag=" + variableName;
		}

		private void OnApplicationQuit()
		{
			SaveAllData();
		}

		private void OnDestroy()
		{
			SaveAllData();
		}

		public void UnlockSkills(params GDESkillTreeData[] unlockSkills)
		{
			if (unlockSkills != null && unlockSkills.Length > 0)
			{
				for (int i = 0; i < unlockSkills.Length; i++)
				{
					if (!_onMemorySaveData.unlockedSkills.Contains(unlockSkills[i].ID))
					{
						_onMemorySaveData.unlockedSkills.Add(unlockSkills[i].ID);
					}
				}
				SaveAllData();
			}
		}

		public void LockSkills(params GDESkillTreeData[] lockSkills)
		{
			if (lockSkills != null && lockSkills.Length > 0 && _onMemorySaveData.unlockedSkills.Count > 0)
			{
				List<int> list = new List<int>();
				for (int i = 0; i < lockSkills.Length; i++)
				{
					if (_onMemorySaveData.unlockedSkills.Contains(lockSkills[i].ID))
					{
						list.Add(lockSkills[i].ID);
					}
				}
				if (list.Count > 0)
				{
					for (int j = 0; j < list.Count; j++)
					{
						_onMemorySaveData.unlockedSkills.Remove(list[j]);
					}
				}
				SaveAllData();
			}
		}

		public FieLevelInfo GetCharacterLevelInfo(GDEGameCharacterTypeData gameCharacterTypeData)
		{
			return GetCharacterLevelInfo((FieConstValues.FieGameCharacter)gameCharacterTypeData.ID);
		}

		public FieLevelInfo GetCharacterLevelInfo(FieConstValues.FieGameCharacter gameCharacterTypeID)
		{
			FieLevelInfo result = default(FieLevelInfo);
			if (!_onMemorySaveData.CharacterExp.ContainsKey((int)gameCharacterTypeID))
			{
				return result;
			}
			int totalExp = _onMemorySaveData.CharacterExp[(int)gameCharacterTypeID];
			return GetLevelInfoByTotalExp(totalExp);
		}

		public void IncreaseOrReduceSkillPoints(GDEGameCharacterTypeData gameCharacterTypeData, int skillPoint)
		{
			if (gameCharacterTypeData != null && !(gameCharacterTypeData.Key == string.Empty) && _onMemorySaveData.CharacterSkillPoint.ContainsKey(gameCharacterTypeData.ID))
			{
				int num = _onMemorySaveData.CharacterSkillPoint[gameCharacterTypeData.ID] + skillPoint;
				if (num < 0)
				{
					Debug.LogError("Setted skill point is out of range. Calculated Skill Point : " + skillPoint.ToString());
				}
				num = Mathf.Max(num, 0);
				_onMemorySaveData.CharacterSkillPoint[gameCharacterTypeData.ID] = num;
				SaveAllData();
			}
		}

		public void ApplyBuildDataFromSaveData(int gameCharacterID, ref GDESkillTreeData[] _unlockedSkills)
		{
			if (debugSkills != null && debugSkills.Count > 0)
			{
				_unlockedSkills = (from skill in debugSkills
				where skill.GameCharacterType.ID == gameCharacterID
				select skill).ToArray();
			}
			else
			{
				_unlockedSkills = FieMasterData<GDESkillTreeData>.FindMasterDataList(delegate(GDESkillTreeData data)
				{
					for (int i = 0; i < _onMemorySaveData.unlockedSkills.Count; i++)
					{
						if (data.GameCharacterType.ID == gameCharacterID && _onMemorySaveData.unlockedSkills[i] == data.ID)
						{
							return true;
						}
					}
					return false;
				}).ToArray();
			}
		}

		private FieLevelInfo GetDefaultLevelInfoData()
		{
			FieLevelInfo result = default(FieLevelInfo);
			result.level = 1;
			result.levelCap = 1;
			result.totalExp = 0;
			result.requiredExpToNextLevel = 1;
			result.currentExpToNextLevel = 1;
			return result;
		}

		public FieLevelInfo GetLevelInfoByTotalExp(int totalExp)
		{
			FieLevelInfo defaultLevelInfoData = GetDefaultLevelInfoData();
			defaultLevelInfoData.totalExp = totalExp;
			Dictionary<string, GDELevelTableData> allMasterData = FieMasterData<GDELevelTableData>.I.GetAllMasterData();
			if (allMasterData == null || allMasterData.Count <= 0)
			{
				return defaultLevelInfoData;
			}
			IEnumerable<GDELevelTableData> source = from data in allMasterData
			orderby data.Value.Level
			select data.Value;
			GDELevelTableData[] array = source.ToArray();
			if (array == null || array.Length <= 0)
			{
				return defaultLevelInfoData;
			}
			defaultLevelInfoData.levelCap = array[array.Length - 1].Level;
			int num = 0;
			int num2 = 0;
			bool flag = false;
			int num3 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				num3 += array[i].ExpRequirement;
				if (!flag)
				{
					num2 = num;
					num += array[i].ExpRequirement;
					if (totalExp >= num)
					{
						defaultLevelInfoData.level = array[i].Level;
					}
					else
					{
						flag = true;
					}
				}
			}
			totalExp = Mathf.Min(totalExp, num3);
			if (defaultLevelInfoData.level == defaultLevelInfoData.levelCap)
			{
				defaultLevelInfoData.requiredExpToNextLevel = 0;
				defaultLevelInfoData.currentExpToNextLevel = 0;
			}
			else
			{
				defaultLevelInfoData.requiredExpToNextLevel = num - num2;
				defaultLevelInfoData.currentExpToNextLevel = totalExp - num2;
			}
			return defaultLevelInfoData;
		}

		public void ResetCurrentGameData()
		{
			currentExpList = new Dictionary<string, int>();
			snapshottedExpList = new Dictionary<string, int>();
		}

		public void SnapshotCurrentExp(FieGameCharacter gameCharacter, int currentExp)
		{
			if (!(gameCharacter == null) && gameCharacter.ownerUser != null)
			{
				if (!snapshottedExpList.ContainsKey(gameCharacter.ownerUser.userHash))
				{
					snapshottedExpList[gameCharacter.ownerUser.userHash] = 0;
				}
				snapshottedExpList[gameCharacter.ownerUser.userHash] = currentExp;
			}
		}

		public void AddCurrentGameExp(FieGameCharacter gameCharacter, int exp)
		{
			if (!(gameCharacter == null) && gameCharacter.ownerUser != null)
			{
				if (!currentExpList.ContainsKey(gameCharacter.ownerUser.userHash))
				{
					currentExpList[gameCharacter.ownerUser.userHash] = 0;
				}
				Dictionary<string, int> dictionary;
				string userHash;
				(dictionary = currentExpList)[userHash = gameCharacter.ownerUser.userHash] = dictionary[userHash] + exp;
			}
		}

		public Dictionary<string, int> GetSnapShottedExp()
		{
			return snapshottedExpList;
		}

		public Dictionary<string, int> GetCurrentGameExp()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (KeyValuePair<string, int> currentExp in currentExpList)
			{
				if (FieManagerBehaviour<FieUserManager>.I.getUserNumberByHash(currentExp.Key) != -1)
				{
					dictionary.Add(currentExp.Key, currentExp.Value);
				}
			}
			if (dictionary.Count <= 0)
			{
				return dictionary;
			}
			IOrderedEnumerable<KeyValuePair<string, int>> source = from x in dictionary
			orderby x.Value descending
			select x;
			return source.ToDictionary((KeyValuePair<string, int> pair) => pair.Key, (KeyValuePair<string, int> pair) => pair.Value);
		}

		public void FlushExpToSaveData(FieGameCharacter _gainExpTarget)
		{
			if (!(_gainExpTarget == null))
			{
				int num = 0;
				Dictionary<string, int> currentGameExp = GetCurrentGameExp();
				if (currentGameExp != null && currentGameExp.Count() > 0)
				{
					foreach (KeyValuePair<string, int> item in currentGameExp)
					{
						num += item.Value;
					}
					if (num > 0)
					{
						FieLevelInfo levelInfoByTotalExp = GetLevelInfoByTotalExp(_onMemorySaveData.CharacterExp[(int)_gainExpTarget.getGameCharacterID()]);
						FieLevelInfo levelInfoByTotalExp2 = GetLevelInfoByTotalExp(_onMemorySaveData.CharacterExp[(int)_gainExpTarget.getGameCharacterID()] + num);
						if (levelInfoByTotalExp.level < levelInfoByTotalExp2.level)
						{
							List<int> list = new List<int>();
							for (int i = levelInfoByTotalExp.level; i < levelInfoByTotalExp2.level; i++)
							{
								list.Add(i + 1);
							}
							int num2 = 0;
							Dictionary<string, GDELevelTableData> allMasterData = FieMasterData<GDELevelTableData>.I.GetAllMasterData();
							foreach (int item2 in list)
							{
								foreach (KeyValuePair<string, GDELevelTableData> item3 in allMasterData)
								{
									if (item3.Value.Level == item2)
									{
										num2 += item3.Value.SkillPoint;
									}
								}
							}
							Dictionary<int, int> characterSkillPoint;
							int gameCharacterID;
							(characterSkillPoint = _onMemorySaveData.CharacterSkillPoint)[gameCharacterID = (int)_gainExpTarget.getGameCharacterID()] = characterSkillPoint[gameCharacterID] + num2;
						}
						_onMemorySaveData.CharacterExp[(int)_gainExpTarget.getGameCharacterID()] = levelInfoByTotalExp2.totalExp;
						SaveAllData();
					}
				}
			}
		}

		public void PromoteGameCharacter(FieConstValues.FieGameCharacter gamecharacterId)
		{
			if (onMemorySaveData.CharacterExp.ContainsKey((int)gamecharacterId))
			{
				bool flag = false;
				int totalExp = onMemorySaveData.CharacterExp[(int)gamecharacterId];
				FieLevelInfo levelInfoByTotalExp = GetLevelInfoByTotalExp(totalExp);
				if (levelInfoByTotalExp.level == levelInfoByTotalExp.levelCap)
				{
					flag = true;
				}
				if (flag)
				{
					onMemorySaveData.CharacterExp[(int)gamecharacterId] = 0;
					onMemorySaveData.CharacterSkillPoint[(int)gamecharacterId] = 0;
					onMemorySaveData.unlockedSkills.Clear();
					Dictionary<int, int> promotedCount;
					int key;
					(promotedCount = onMemorySaveData.PromotedCount)[key = (int)gamecharacterId] = promotedCount[key] + 1;
					onMemorySaveData.PlayerLevel++;
					SaveAllData();
				}
			}
		}
	}
}
