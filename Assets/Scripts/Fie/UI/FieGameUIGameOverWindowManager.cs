using Fie.Manager;

namespace Fie.UI
{
	public class FieGameUIGameOverWindowManager : FieGameUIComponentManagerBase
	{
		private FieGameUIGameOverWindow _gameoverWindow;

		public override void StartUp()
		{
			if (_gameoverWindow == null)
			{
				_gameoverWindow = FieManagerBehaviour<FieGUIManager>.I.CreateGui<FieGameUIGameOverWindow>(null);
				_gameoverWindow.uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
				_gameoverWindow.Initialize();
				FieManagerBehaviour<FieInGameStateManager>.I.GameOverEvent += GameOverCallback;
				FieManagerBehaviour<FieInGameStateManager>.I.RetryEvent += RetryCallback;
			}
		}

		private void RetryCallback()
		{
			FieManagerBehaviour<FieAudioManager>.I.ChangeMixerVolume(0f, 0.5f, default(FieAudioManager.FieAudioMixerType));
			HideGameOverWindow();
		}

		private void GameOverCallback()
		{
			FieManagerBehaviour<FieAudioManager>.I.ChangeMixerVolume(-255f, 0.3f, default(FieAudioManager.FieAudioMixerType));
			ShowGameOverWindow();
		}

		public void Relocate()
		{
		}

		public override void setComponentManagerOwner(FieGameCharacter owner)
		{
		}

		public void ShowGameOverWindow()
		{
			_gameoverWindow.isEnable = true;
		}

		public void HideGameOverWindow()
		{
			_gameoverWindow.isEnable = false;
		}
	}
}
