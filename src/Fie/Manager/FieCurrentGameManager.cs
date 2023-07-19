using Fie.User;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.INGAME)]
	public class FieCurrentGameManager : FieManagerBehaviour<FieCurrentGameManager>
	{
		private bool _isBooted;

		public bool isBooted => _isBooted;

		protected override void StartUpEntity()
		{
			InitializeCurrentGameInfo();
			_isBooted = true;
		}

		public void InitializeCurrentGameInfo()
		{
			FieManagerBehaviour<FieSaveManager>.I.ResetCurrentGameData();
			FieUser[] allUserData = FieManagerBehaviour<FieUserManager>.I.getAllUserData();
			foreach (FieUser fieUser in allUserData)
			{
				if (fieUser != null && !(fieUser.usersCharacter == null))
				{
					fieUser.usersCharacter.score = 0;
					int totalExp = fieUser.usersCharacter.totalExp;
					FieManagerBehaviour<FieSaveManager>.I.SnapshotCurrentExp(fieUser.usersCharacter, totalExp);
				}
			}
		}
	}
}
