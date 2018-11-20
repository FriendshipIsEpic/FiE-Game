using Fie.Manager;
using GameDataEditor;
using System.Collections;
using UnityEngine;

namespace Fie.UI
{
	public class FieGameUIDialogCaptionManager : FieGameUIComponentManagerBase
	{
		private const float DIALOG_CLEAR_DELAY = 1.25f;

		private const string DEFAULT_TEXT_USERNAME_SIGNATURE = "%USERNAME%";

		private const string DEFAULT_TEXT_ENTITY_SIGNATURE = "%ENTITY%";

		private const string DEFAULT_TEXT_FORMAT = "%USERNAME% : %ENTITY%";

		private FieGameUIDialogCaption _dialogCaption;

		private Coroutine dialogClearTask;

		private IEnumerator DialogClaerTask(float delay)
		{
			if (!(_dialogCaption == null))
			{
				yield return (object)new WaitForSeconds(delay);
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public override void StartUp()
		{
			if (_dialogCaption == null)
			{
				_dialogCaption = FieManagerBehaviour<FieGUIManager>.I.CreateGui<FieGameUIDialogCaption>(null);
				_dialogCaption.uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
				FieManagerBehaviour<FieDialogManager>.I.dialogStartEvent += DialogChangedCallback;
				FieManagerBehaviour<FieDialogManager>.I.dialogEndEvent += DialogEndCallback;
			}
		}

		private void DialogChangedCallback(FieGameCharacter actor, GDEWordScriptsListData dialogData)
		{
			if (!(_dialogCaption == null))
			{
				if (dialogClearTask != null)
				{
					StopCoroutine(dialogClearTask);
				}
				string text = "%USERNAME% : %ENTITY%";
				if (!(actor == null))
				{
					text = ((actor.ownerUser != null) ? text.Replace("%USERNAME%", actor.ownerUser.userName) : text.Replace("%USERNAME%", actor.getDefaultName()));
					text = text.Replace("%ENTITY%", FieLocalizeUtility.GetWordScriptText(dialogData.Key));
					_dialogCaption.SetText(text);
				}
			}
		}

		private void DialogEndCallback(FieGameCharacter actor, GDEWordScriptsListData dialogData)
		{
			if (this != null && base.gameObject != null)
			{
				if (dialogClearTask != null)
				{
					StopCoroutine(dialogClearTask);
				}
				dialogClearTask = StartCoroutine(DialogClaerTask(1.25f));
			}
		}

		public void Relocate()
		{
		}

		public override void setComponentManagerOwner(FieGameCharacter owner)
		{
		}

		public void SetText(string text)
		{
			_dialogCaption.SetText(text);
		}

		public void ClearText()
		{
			_dialogCaption.ClearText();
		}
	}
}
