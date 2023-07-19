using Fie.Manager;
using Fie.Scene;
using GameDataEditor;

namespace Fie.UI
{
	public class FieGameUIActivityWindowManager : FieGameUIComponentManagerBase
	{
		private FieGameUIActivityWindow _activityWindow;

		public override void StartUp()
		{
			if (_activityWindow == null)
			{
				_activityWindow = FieManagerBehaviour<FieGUIManager>.I.CreateGui<FieGameUIActivityWindow>(null);
				_activityWindow.uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
				FieManagerBehaviour<FieActivityManager>.I.activityStartEvent += ActivityStartEventCallback;
				FieManagerBehaviour<FieActivityManager>.I.activityEndEvent += ActivityEndEventCallback;
				if (FieManagerFactory.I.currentSceneType == FieSceneType.INGAME)
				{
					FieManagerBehaviour<FieInGameStateManager>.I.GameOverEvent += GameOverCallback;
				}
			}
		}

		private void GameOverCallback()
		{
			FieManagerBehaviour<FieActivityManager>.I.RequestToHideActivity();
		}

		private void ActivityStartEventCallback(GDEConstantTextListData titleTextData, GDEConstantTextListData noteTextData, string titleString = "", string noteString = "")
		{
			string titleText = (!(titleString == string.Empty)) ? titleString : FieLocalizeUtility.GetConstantText(titleTextData.Key);
			string noteText = (!(noteString == string.Empty)) ? noteString : FieLocalizeUtility.GetConstantText(noteTextData.Key);
			_activityWindow.ShowText(titleText, noteText);
		}

		private void ActivityEndEventCallback(GDEConstantTextListData titleTextData, GDEConstantTextListData noteTextData, string titleString = "", string noteString = "")
		{
			_activityWindow.HideText();
		}

		public void Relocate()
		{
		}

		public FieGameUIActivityWindow.ActivityWindowState GetActivityWindowState()
		{
			if (_activityWindow == null)
			{
				return FieGameUIActivityWindow.ActivityWindowState.BUSY;
			}
			return _activityWindow.activityWindowState;
		}

		public override void setComponentManagerOwner(FieGameCharacter owner)
		{
		}
	}
}
