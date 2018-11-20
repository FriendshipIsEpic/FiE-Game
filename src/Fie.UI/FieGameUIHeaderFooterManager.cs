using Fie.Manager;
using Fie.Scene;
using UnityEngine;

namespace Fie.UI
{
	public class FieGameUIHeaderFooterManager : FieGameUIComponentManagerBase
	{
		private FieGameUIHeaderFooter _headerFooter;

		public override void StartUp()
		{
			if (_headerFooter == null)
			{
				_headerFooter = FieManagerBehaviour<FieGUIManager>.I.CreateGui<FieGameUIHeaderFooter>(null);
				_headerFooter.uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
				FieManagerBehaviour<FieGUIManager>.I.uiPositionList.Add(FieGUIManager.FieUIPositionTag.HEADER_ROOT, _headerFooter.headerRootTransform);
				FieManagerBehaviour<FieGUIManager>.I.uiPositionList.Add(FieGUIManager.FieUIPositionTag.FOOTER_ROOT, _headerFooter.footerRootTransform);
				FieManagerBehaviour<FieGUIManager>.I.uiPositionList.Add(FieGUIManager.FieUIPositionTag.ABILITY_ICON_1, _headerFooter.Ability1);
				FieManagerBehaviour<FieGUIManager>.I.uiPositionList.Add(FieGUIManager.FieUIPositionTag.ABILITY_ICON_2, _headerFooter.Ability2);
				FieManagerBehaviour<FieGUIManager>.I.uiPositionList.Add(FieGUIManager.FieUIPositionTag.ABILITY_ICON_3, _headerFooter.Ability3);
				Relocate();
				_headerFooter.uiCamera.screenResizeEvent += Relocate;
			}
			FieManagerBehaviour<FieInGameStateManager>.I.GameOverEvent += GameOverCallback;
			if (FieManagerFactory.I.currentSceneType == FieSceneType.INGAME)
			{
				FieManagerBehaviour<FieInGameStateManager>.I.RetryFinishedEvent += RetryFinishedEvent;
			}
		}

		private void RetryFinishedEvent()
		{
			_headerFooter.Show();
		}

		private void GameOverCallback()
		{
			_headerFooter.Hide();
		}

		public void Relocate()
		{
			if (_headerFooter != null)
			{
				_headerFooter.header.transform.position = _headerFooter.uiCamera.camera.ScreenToWorldPoint(new Vector3(0f, (float)Screen.height + 0.025f, 0f));
				_headerFooter.footer.transform.position = _headerFooter.uiCamera.camera.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.5f, 0f, 0f));
				_headerFooter.SetUILayer(FieGUIManager.FieUILayer.BACKWORD_THIRD);
			}
		}

		public override void setComponentManagerOwner(FieGameCharacter owner)
		{
		}

		public void Show()
		{
			if (!(_headerFooter == null))
			{
				_headerFooter.Show();
			}
		}

		public void Hide()
		{
			if (!(_headerFooter == null))
			{
				_headerFooter.Hide();
			}
		}
	}
}
