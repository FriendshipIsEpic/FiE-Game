using Fie.Manager;
using HutongGames.PlayMaker;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieLanguageChangerForPlaymaker : FsmStateAction
	{
		public override void OnEnter()
		{
			FieEnvironmentManager.Language targetLanguage = FieEnvironmentManager.Language.French;
			if (FieManagerBehaviour<FieEnvironmentManager>.I.currentLanguage == FieEnvironmentManager.Language.French)
			{
				targetLanguage = FieEnvironmentManager.Language.Japanese;
			}
			FieManagerBehaviour<FieEnvironmentManager>.I.SetLanguageSetting(targetLanguage);
			Finish();
		}
	}
}
