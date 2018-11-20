using Fie.UI;
using GameDataEditor;
using TMPro;
using UnityEngine;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.NEVER_DESTROY)]
	public sealed class FieEnvironmentManager : FieManagerBehaviour<FieEnvironmentManager>
	{
		public enum Language
		{
			None = -1,
			English,
			Japanese,
			French,
			LANGUAGE_MAX
		}

		public enum QualitySettings
		{
			None = -1,
			Fast_and_Cheap,
			Good,
			Beautiful,
			Fantastic,
			Ultimate,
			QUALITY_SETTINGS_MAX
		}

		public enum Difficulty
		{
			DIFFICULTY_NONE = -1,
			EASY,
			NORMAL,
			HARD,
			VERY_HARD,
			NIGHTMARE,
			CHAOS,
			DIFFICULTY_MAX
		}

		public enum GameMode
		{
			ADVENTURE,
			SURVIVAL,
			GAME_MODE_MAX
		}

		public delegate void FieEnviromentChangedCallback();

		public const string FIE_VERSION_STRING = "v_1_0_211_Prototype(In-House Build)";

		public const string FIE_APP_ID = "7cae3868-8823-43e8-9729-588152a87571";

		public const string FIE_MASTER_DATA_FILE_NAME = "MasterData/fie_master_data";

		private const string DIFFICULTY_SETTING_USERPREF_KEY = "DIFFICULTY";

		private const string LANGUAGE_SETTING_USERPREF_KEY = "LANGUAGE";

		public const int DEFAULT_FRAME_RATE = 60;

		public GameMode currentGameMode;

		private GDEDifficultyListData _currentDifficultyData;

		public Language currentLanguage
		{
			get;
			private set;
		}

		public TMP_FontAsset englishFont
		{
			get;
			private set;
		}

		public TMP_FontAsset currentFont
		{
			get;
			private set;
		}

		public float currentFontSizeRate
		{
			get;
			private set;
		}

		public int currentFrameRate
		{
			get;
			private set;
		}

		public Difficulty currentDifficulty
		{
			get;
			private set;
		}

		public GDEDifficultyListData currentDifficultyData => _currentDifficultyData;

		public float currentEnemyDamageMagnify
		{
			get
			{
				if (currentDifficultyData != null)
				{
					return currentDifficultyData.enemyDamageMagnify;
				}
				return 1f;
			}
		}

		public float currentStaggerRegistMagnify
		{
			get
			{
				if (currentDifficultyData != null)
				{
					return currentDifficultyData.enemyStaggerRegsitMagnify;
				}
				return 1f;
			}
		}

		public float currentEnemyHealthMagnify
		{
			get
			{
				if (currentDifficultyData != null)
				{
					return currentDifficultyData.enemyHealthMagnify;
				}
				return 1f;
			}
		}

		public float currentPlayerRegenDelayMagnify
		{
			get
			{
				if (currentDifficultyData != null)
				{
					return currentDifficultyData.playerRegenDelayMagnify;
				}
				return 1f;
			}
		}

		public float currentFriendshipPointMagnify
		{
			get
			{
				if (currentDifficultyData != null)
				{
					return currentDifficultyData.friendshipPointMagnify;
				}
				return 1f;
			}
		}

		public float currentExpMagnify
		{
			get
			{
				if (currentDifficultyData != null)
				{
					return currentDifficultyData.expMagnify;
				}
				return 1f;
			}
		}

		public event FieEnviromentChangedCallback DifficultyChangedEvent;

		protected override void StartUpEntity()
		{
			GDEDataManager.Init("MasterData/fie_master_data");
			InitializePrefs();
			InitializeFont();
			if (UnityEngine.QualitySettings.GetQualityLevel() == 0)
			{
				FieManagerBehaviour<FieEnvironmentManager>.I.CalibrateTimeEnviroment(1f, 30);
			}
			else
			{
				FieManagerBehaviour<FieEnvironmentManager>.I.CalibrateTimeEnviroment();
			}
		}

		private void InitializePrefs()
		{
			currentLanguage = (Language)FieManagerBehaviour<FieSaveManager>.I.onMemorySaveData.LanguageCode;
			currentDifficulty = Difficulty.NORMAL;
			currentGameMode = GameMode.ADVENTURE;
			_currentDifficultyData = FieMasterData<GDEDifficultyListData>.I.GetMasterData(GetDifficultyItemKeyName(currentDifficulty));
			SetLanguageSetting(currentLanguage, reloadAllTextUI: false);
		}

		private void InitializeFont()
		{
			GDEConstantTextListData masterData = FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_FONT);
			GDEConstantTextListData masterData2 = FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_FONT_SIZE_RATE);
			if (masterData == null || masterData2 == null)
			{
				Debug.LogError("Faild to load font parameters.");
			}
			else
			{
				englishFont = Resources.Load<TMP_FontAsset>(masterData.English);
				if (englishFont == null)
				{
					Debug.LogError("Faild to load an english font data.");
				}
				ReloadCurrentFont();
			}
		}

		private void ReloadCurrentFont()
		{
			GDEConstantTextListData masterData = FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_FONT);
			GDEConstantTextListData masterData2 = FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_FONT_SIZE_RATE);
			if (masterData == null || masterData2 == null)
			{
				Debug.LogError("Faild to load font parameters.");
			}
			else
			{
				switch (currentLanguage)
				{
				case Language.English:
					currentFont = englishFont;
					currentFontSizeRate = float.Parse(masterData2.English);
					break;
				case Language.Japanese:
					currentFont = Resources.Load<TMP_FontAsset>(masterData.Japanese);
					currentFontSizeRate = float.Parse(masterData2.Japanese);
					break;
				case Language.French:
					currentFont = Resources.Load<TMP_FontAsset>(masterData.French);
					currentFontSizeRate = float.Parse(masterData2.French);
					break;
				}
				if (currentFont == null)
				{
					Debug.LogError("Faild to load a current language font data. Language :" + currentLanguage.ToString());
				}
			}
		}

		public void CalibrateTimeEnviroment(float timeScale = 1f, int targetFrameRate = 60)
		{
			Time.timeScale = timeScale;
			Application.targetFrameRate = targetFrameRate;
			Time.fixedDeltaTime = Time.timeScale / (float)Application.targetFrameRate;
			currentFrameRate = targetFrameRate;
		}

		public void SetLanguageSetting(Language targetLanguage, bool reloadAllTextUI = true)
		{
			if (targetLanguage != currentLanguage)
			{
				FieManagerBehaviour<FieSaveManager>.I.onMemorySaveData.LanguageCode = (int)targetLanguage;
				currentLanguage = targetLanguage;
				ReloadCurrentFont();
				if (reloadAllTextUI)
				{
					ReloadAllTextUI(currentLanguage);
				}
			}
		}

		public void SetDifficulty(Difficulty targetDifficulty)
		{
			if (targetDifficulty != currentDifficulty)
			{
				currentDifficulty = targetDifficulty;
				_currentDifficultyData = FieMasterData<GDEDifficultyListData>.I.GetMasterData(GetDifficultyItemKeyName(currentDifficulty));
				if (this.DifficultyChangedEvent != null)
				{
					this.DifficultyChangedEvent();
				}
			}
		}

		public GDEDifficultyListData GetDifficultyData(Difficulty difficulty)
		{
			return FieMasterData<GDEDifficultyListData>.I.GetMasterData(GetDifficultyItemKeyName(difficulty));
		}

		private string GetDifficultyItemKeyName(Difficulty difficulty)
		{
			switch (difficulty)
			{
			case Difficulty.EASY:
				return GDEItemKeys.DifficultyList_EASY;
			case Difficulty.NORMAL:
				return GDEItemKeys.DifficultyList_NORMAL;
			case Difficulty.HARD:
				return GDEItemKeys.DifficultyList_HARD;
			case Difficulty.VERY_HARD:
				return GDEItemKeys.DifficultyList_VERY_HARD;
			case Difficulty.NIGHTMARE:
				return GDEItemKeys.DifficultyList_NIGHTMARE;
			case Difficulty.CHAOS:
				return GDEItemKeys.DifficultyList_CHAOS;
			default:
				return GDEItemKeys.DifficultyList_NORMAL;
			}
		}

		public void ReloadAllTextUI(Language targetLanguage)
		{
			FieUILocalizedTextObjectBase[] array = UnityEngine.Object.FindObjectsOfType<FieUILocalizedTextObjectBase>();
			if (array.Length > 0)
			{
				FieUILocalizedTextObjectBase[] array2 = array;
				foreach (FieUILocalizedTextObjectBase fieUILocalizedTextObjectBase in array2)
				{
					fieUILocalizedTextObjectBase.InitializeText();
				}
			}
		}

		public string getVersionString()
		{
			return "v_1_0_211_Prototype(In-House Build)";
		}
	}
}
