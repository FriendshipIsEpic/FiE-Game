using System.Collections.Generic;

namespace Fie.Manager
{
	public class FieSaveData
	{
		public int LanguageCode;

		public int PlayerLevel = 1;

		public string LastLoginedUserName = string.Empty;

		public string LastLoginedPasswordPassword = string.Empty;

		public Dictionary<int, int> CharacterExp = new Dictionary<int, int>();

		public Dictionary<int, int> CharacterSkillPoint = new Dictionary<int, int>();

		public Dictionary<int, int> PromotedCount = new Dictionary<int, int>();

		public List<int> unlockedSkills = new List<int>();

		public Dictionary<int, int> AchivemenetProgress = new Dictionary<int, int>();
	}
}
