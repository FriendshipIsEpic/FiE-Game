// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// GameDataEditor.GDEDifficultyListData
using System.Collections.Generic;
using GameDataEditor;

namespace GameDataEditor
{
	public class GDEDifficultyListData : IGDEData
	{
		private static string difficultyLevelKey = "difficultyLevel";

		private int _difficultyLevel;

		private static string commonDropRateKey = "commonDropRate";

		private int _commonDropRate;

		private static string uncommonDropRateKey = "uncommonDropRate";

		private int _uncommonDropRate;

		private static string rareDropRateKey = "rareDropRate";

		private int _rareDropRate;

		private static string legendaryDropRateKey = "legendaryDropRate";

		private int _legendaryDropRate;

		private static string epicDropRateKey = "epicDropRate";

		private int _epicDropRate;

		private static string enemyCostMagnifyKey = "enemyCostMagnify";

		private float _enemyCostMagnify;

		private static string enemyCostCapMagnifyKey = "enemyCostCapMagnify";

		private float _enemyCostCapMagnify;

		private static string enemyDamageMagnifyKey = "enemyDamageMagnify";

		private float _enemyDamageMagnify;

		private static string enemyHealthMagnifyKey = "enemyHealthMagnify";

		private float _enemyHealthMagnify;

		private static string enemyStaggerRegsitMagnifyKey = "enemyStaggerRegsitMagnify";

		private float _enemyStaggerRegsitMagnify;

		private static string playerRegenDelayMagnifyKey = "playerRegenDelayMagnify";

		private float _playerRegenDelayMagnify;

		private static string friendshipPointMagnifyKey = "friendshipPointMagnify";

		private float _friendshipPointMagnify;

		private static string expMagnifyKey = "expMagnify";

		private float _expMagnify;

		private static string difficultyNameKey = "difficultyName";

		private GDEConstantTextListData _difficultyName;

		private static string descriptionKey = "description";

		private GDEConstantTextListData _description;

		public int difficultyLevel
		{
			get
			{
				return _difficultyLevel;
			}
			set
			{
				if (_difficultyLevel != value)
				{
					_difficultyLevel = value;
					GDEDataManager.SetInt(_key + "_" + difficultyLevelKey, _difficultyLevel);
				}
			}
		}

		public int commonDropRate
		{
			get
			{
				return _commonDropRate;
			}
			set
			{
				if (_commonDropRate != value)
				{
					_commonDropRate = value;
					GDEDataManager.SetInt(_key + "_" + commonDropRateKey, _commonDropRate);
				}
			}
		}

		public int uncommonDropRate
		{
			get
			{
				return _uncommonDropRate;
			}
			set
			{
				if (_uncommonDropRate != value)
				{
					_uncommonDropRate = value;
					GDEDataManager.SetInt(_key + "_" + uncommonDropRateKey, _uncommonDropRate);
				}
			}
		}

		public int rareDropRate
		{
			get
			{
				return _rareDropRate;
			}
			set
			{
				if (_rareDropRate != value)
				{
					_rareDropRate = value;
					GDEDataManager.SetInt(_key + "_" + rareDropRateKey, _rareDropRate);
				}
			}
		}

		public int legendaryDropRate
		{
			get
			{
				return _legendaryDropRate;
			}
			set
			{
				if (_legendaryDropRate != value)
				{
					_legendaryDropRate = value;
					GDEDataManager.SetInt(_key + "_" + legendaryDropRateKey, _legendaryDropRate);
				}
			}
		}

		public int epicDropRate
		{
			get
			{
				return _epicDropRate;
			}
			set
			{
				if (_epicDropRate != value)
				{
					_epicDropRate = value;
					GDEDataManager.SetInt(_key + "_" + epicDropRateKey, _epicDropRate);
				}
			}
		}

		public float enemyCostMagnify
		{
			get
			{
				return _enemyCostMagnify;
			}
			set
			{
				if (_enemyCostMagnify != value)
				{
					_enemyCostMagnify = value;
					GDEDataManager.SetFloat(_key + "_" + enemyCostMagnifyKey, _enemyCostMagnify);
				}
			}
		}

		public float enemyCostCapMagnify
		{
			get
			{
				return _enemyCostCapMagnify;
			}
			set
			{
				if (_enemyCostCapMagnify != value)
				{
					_enemyCostCapMagnify = value;
					GDEDataManager.SetFloat(_key + "_" + enemyCostCapMagnifyKey, _enemyCostCapMagnify);
				}
			}
		}

		public float enemyDamageMagnify
		{
			get
			{
				return _enemyDamageMagnify;
			}
			set
			{
				if (_enemyDamageMagnify != value)
				{
					_enemyDamageMagnify = value;
					GDEDataManager.SetFloat(_key + "_" + enemyDamageMagnifyKey, _enemyDamageMagnify);
				}
			}
		}

		public float enemyHealthMagnify
		{
			get
			{
				return _enemyHealthMagnify;
			}
			set
			{
				if (_enemyHealthMagnify != value)
				{
					_enemyHealthMagnify = value;
					GDEDataManager.SetFloat(_key + "_" + enemyHealthMagnifyKey, _enemyHealthMagnify);
				}
			}
		}

		public float enemyStaggerRegsitMagnify
		{
			get
			{
				return _enemyStaggerRegsitMagnify;
			}
			set
			{
				if (_enemyStaggerRegsitMagnify != value)
				{
					_enemyStaggerRegsitMagnify = value;
					GDEDataManager.SetFloat(_key + "_" + enemyStaggerRegsitMagnifyKey, _enemyStaggerRegsitMagnify);
				}
			}
		}

		public float playerRegenDelayMagnify
		{
			get
			{
				return _playerRegenDelayMagnify;
			}
			set
			{
				if (_playerRegenDelayMagnify != value)
				{
					_playerRegenDelayMagnify = value;
					GDEDataManager.SetFloat(_key + "_" + playerRegenDelayMagnifyKey, _playerRegenDelayMagnify);
				}
			}
		}

		public float friendshipPointMagnify
		{
			get
			{
				return _friendshipPointMagnify;
			}
			set
			{
				if (_friendshipPointMagnify != value)
				{
					_friendshipPointMagnify = value;
					GDEDataManager.SetFloat(_key + "_" + friendshipPointMagnifyKey, _friendshipPointMagnify);
				}
			}
		}

		public float expMagnify
		{
			get
			{
				return _expMagnify;
			}
			set
			{
				if (_expMagnify != value)
				{
					_expMagnify = value;
					GDEDataManager.SetFloat(_key + "_" + expMagnifyKey, _expMagnify);
				}
			}
		}

		public GDEConstantTextListData difficultyName
		{
			get
			{
				return _difficultyName;
			}
			set
			{
				if (_difficultyName != value)
				{
					_difficultyName = value;
					GDEDataManager.SetCustom(_key + "_" + difficultyNameKey, _difficultyName);
				}
			}
		}

		public GDEConstantTextListData description
		{
			get
			{
				return _description;
			}
			set
			{
				if (_description != value)
				{
					_description = value;
					GDEDataManager.SetCustom(_key + "_" + descriptionKey, _description);
				}
			}
		}

		public GDEDifficultyListData()
		{
			_key = string.Empty;
		}

		public GDEDifficultyListData(string key)
		{
			_key = key;
		}

		public override void LoadFromDict(string dataKey, Dictionary<string, object> dict)
		{
			_key = dataKey;
			if (dict == null)
			{
				LoadFromSavedData(dataKey);
				return;
			}
			dict.TryGetInt(difficultyLevelKey, out _difficultyLevel);
			dict.TryGetInt(commonDropRateKey, out _commonDropRate);
			dict.TryGetInt(uncommonDropRateKey, out _uncommonDropRate);
			dict.TryGetInt(rareDropRateKey, out _rareDropRate);
			dict.TryGetInt(legendaryDropRateKey, out _legendaryDropRate);
			dict.TryGetInt(epicDropRateKey, out _epicDropRate);
			dict.TryGetFloat(enemyCostMagnifyKey, out _enemyCostMagnify);
			dict.TryGetFloat(enemyCostCapMagnifyKey, out _enemyCostCapMagnify);
			dict.TryGetFloat(enemyDamageMagnifyKey, out _enemyDamageMagnify);
			dict.TryGetFloat(enemyHealthMagnifyKey, out _enemyHealthMagnify);
			dict.TryGetFloat(enemyStaggerRegsitMagnifyKey, out _enemyStaggerRegsitMagnify);
			dict.TryGetFloat(playerRegenDelayMagnifyKey, out _playerRegenDelayMagnify);
			dict.TryGetFloat(friendshipPointMagnifyKey, out _friendshipPointMagnify);
			dict.TryGetFloat(expMagnifyKey, out _expMagnify);
			dict.TryGetString(difficultyNameKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _difficultyName);
			dict.TryGetString(descriptionKey, out value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _description);
			LoadFromSavedData(dataKey);
		}

		public override void LoadFromSavedData(string dataKey)
		{
			_key = dataKey;
			_difficultyLevel = GDEDataManager.GetInt(_key + "_" + difficultyLevelKey, _difficultyLevel);
			_commonDropRate = GDEDataManager.GetInt(_key + "_" + commonDropRateKey, _commonDropRate);
			_uncommonDropRate = GDEDataManager.GetInt(_key + "_" + uncommonDropRateKey, _uncommonDropRate);
			_rareDropRate = GDEDataManager.GetInt(_key + "_" + rareDropRateKey, _rareDropRate);
			_legendaryDropRate = GDEDataManager.GetInt(_key + "_" + legendaryDropRateKey, _legendaryDropRate);
			_epicDropRate = GDEDataManager.GetInt(_key + "_" + epicDropRateKey, _epicDropRate);
			_enemyCostMagnify = GDEDataManager.GetFloat(_key + "_" + enemyCostMagnifyKey, _enemyCostMagnify);
			_enemyCostCapMagnify = GDEDataManager.GetFloat(_key + "_" + enemyCostCapMagnifyKey, _enemyCostCapMagnify);
			_enemyDamageMagnify = GDEDataManager.GetFloat(_key + "_" + enemyDamageMagnifyKey, _enemyDamageMagnify);
			_enemyHealthMagnify = GDEDataManager.GetFloat(_key + "_" + enemyHealthMagnifyKey, _enemyHealthMagnify);
			_enemyStaggerRegsitMagnify = GDEDataManager.GetFloat(_key + "_" + enemyStaggerRegsitMagnifyKey, _enemyStaggerRegsitMagnify);
			_playerRegenDelayMagnify = GDEDataManager.GetFloat(_key + "_" + playerRegenDelayMagnifyKey, _playerRegenDelayMagnify);
			_friendshipPointMagnify = GDEDataManager.GetFloat(_key + "_" + friendshipPointMagnifyKey, _friendshipPointMagnify);
			_expMagnify = GDEDataManager.GetFloat(_key + "_" + expMagnifyKey, _expMagnify);
			_difficultyName = GDEDataManager.GetCustom(_key + "_" + difficultyNameKey, _difficultyName);
			_description = GDEDataManager.GetCustom(_key + "_" + descriptionKey, _description);
		}

		public void Reset_difficultyLevel()
		{
			GDEDataManager.ResetToDefault(_key, difficultyLevelKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(difficultyLevelKey, out _difficultyLevel);
		}

		public void Reset_commonDropRate()
		{
			GDEDataManager.ResetToDefault(_key, commonDropRateKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(commonDropRateKey, out _commonDropRate);
		}

		public void Reset_uncommonDropRate()
		{
			GDEDataManager.ResetToDefault(_key, uncommonDropRateKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(uncommonDropRateKey, out _uncommonDropRate);
		}

		public void Reset_rareDropRate()
		{
			GDEDataManager.ResetToDefault(_key, rareDropRateKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(rareDropRateKey, out _rareDropRate);
		}

		public void Reset_legendaryDropRate()
		{
			GDEDataManager.ResetToDefault(_key, legendaryDropRateKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(legendaryDropRateKey, out _legendaryDropRate);
		}

		public void Reset_epicDropRate()
		{
			GDEDataManager.ResetToDefault(_key, epicDropRateKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetInt(epicDropRateKey, out _epicDropRate);
		}

		public void Reset_enemyCostMagnify()
		{
			GDEDataManager.ResetToDefault(_key, enemyCostMagnifyKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(enemyCostMagnifyKey, out _enemyCostMagnify);
		}

		public void Reset_enemyCostCapMagnify()
		{
			GDEDataManager.ResetToDefault(_key, enemyCostCapMagnifyKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(enemyCostCapMagnifyKey, out _enemyCostCapMagnify);
		}

		public void Reset_enemyDamageMagnify()
		{
			GDEDataManager.ResetToDefault(_key, enemyDamageMagnifyKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(enemyDamageMagnifyKey, out _enemyDamageMagnify);
		}

		public void Reset_enemyHealthMagnify()
		{
			GDEDataManager.ResetToDefault(_key, enemyHealthMagnifyKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(enemyHealthMagnifyKey, out _enemyHealthMagnify);
		}

		public void Reset_enemyStaggerRegsitMagnify()
		{
			GDEDataManager.ResetToDefault(_key, enemyStaggerRegsitMagnifyKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(enemyStaggerRegsitMagnifyKey, out _enemyStaggerRegsitMagnify);
		}

		public void Reset_playerRegenDelayMagnify()
		{
			GDEDataManager.ResetToDefault(_key, playerRegenDelayMagnifyKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(playerRegenDelayMagnifyKey, out _playerRegenDelayMagnify);
		}

		public void Reset_friendshipPointMagnify()
		{
			GDEDataManager.ResetToDefault(_key, friendshipPointMagnifyKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(friendshipPointMagnifyKey, out _friendshipPointMagnify);
		}

		public void Reset_expMagnify()
		{
			GDEDataManager.ResetToDefault(_key, expMagnifyKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetFloat(expMagnifyKey, out _expMagnify);
		}

		public void Reset_difficultyName()
		{
			GDEDataManager.ResetToDefault(_key, difficultyNameKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(difficultyNameKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _difficultyName);
			difficultyName = GDEDataManager.GetCustom(_key + "_" + difficultyNameKey, _difficultyName);
			difficultyName.ResetAll();
		}

		public void Reset_description()
		{
			GDEDataManager.ResetToDefault(_key, descriptionKey);
			GDEDataManager.Get(_key, out var data);
			data.TryGetString(descriptionKey, out var value);
			GDEDataManager.DataDictionary.TryGetCustom<string, object, GDEConstantTextListData>(value, out _description);
			description = GDEDataManager.GetCustom(_key + "_" + descriptionKey, _description);
			description.ResetAll();
		}

		public void ResetAll()
		{
			GDEDataManager.ResetToDefault(_key, difficultyNameKey);
			GDEDataManager.ResetToDefault(_key, descriptionKey);
			GDEDataManager.ResetToDefault(_key, difficultyLevelKey);
			GDEDataManager.ResetToDefault(_key, enemyCostMagnifyKey);
			GDEDataManager.ResetToDefault(_key, enemyCostCapMagnifyKey);
			GDEDataManager.ResetToDefault(_key, enemyDamageMagnifyKey);
			GDEDataManager.ResetToDefault(_key, enemyHealthMagnifyKey);
			GDEDataManager.ResetToDefault(_key, enemyStaggerRegsitMagnifyKey);
			GDEDataManager.ResetToDefault(_key, playerRegenDelayMagnifyKey);
			GDEDataManager.ResetToDefault(_key, friendshipPointMagnifyKey);
			GDEDataManager.ResetToDefault(_key, expMagnifyKey);
			GDEDataManager.ResetToDefault(_key, commonDropRateKey);
			GDEDataManager.ResetToDefault(_key, uncommonDropRateKey);
			GDEDataManager.ResetToDefault(_key, rareDropRateKey);
			GDEDataManager.ResetToDefault(_key, legendaryDropRateKey);
			GDEDataManager.ResetToDefault(_key, epicDropRateKey);
			Reset_difficultyName();
			Reset_description();
			GDEDataManager.Get(_key, out var data);
			LoadFromDict(_key, data);
		}
	}
}