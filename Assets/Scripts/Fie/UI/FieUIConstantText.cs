using Fie.Manager;
using GameDataEditor;
using UnityEngine;

namespace Fie.UI
{
	public class FieUIConstantText : FieUITextMeshProObject
	{
		[SerializeField]
		private string _constantTextDataKey;

		private void Start()
		{
			InitializeText();
		}

		public override void InitializeText()
		{
			if (_constantTextDataKey != null && !(_tmpTextObject == null))
			{
				GDEConstantTextListData constantTextData;
				string constantText = FieLocalizeUtility.GetConstantText(_constantTextDataKey, out constantTextData);
				_tmpTextObject.font = ((!constantTextData.ForceEnableToUseEnglishFont) ? FieManagerBehaviour<FieEnvironmentManager>.I.currentFont : FieManagerBehaviour<FieEnvironmentManager>.I.englishFont);
				_tmpTextObject.text = constantText;
				_tmpTextObject.ForceMeshUpdate();
			}
		}
	}
}
