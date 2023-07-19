using Fie.Manager;
using GameDataEditor;
using UnityEngine;

namespace Fie.UI
{
	public class FieUIConstant2DText : FieUITextMeshProObject
	{
		public delegate void FieConstantTextReplaceCallback(ref string targetString);

		public FieConstantTextReplaceCallback replaceMethod;

		[SerializeField]
		private string _constantTextDataKey;

		private new void Awake()
		{
			base.Awake();
			InitializeText();
		}

		private void Start()
		{
			InitializeText();
		}

		private void OnEnable()
		{
			InitializeText();
		}

		public override void InitializeText()
		{
			if (_constantTextDataKey != null && !(base.TmpTextObject == null))
			{
				GDEConstantTextListData constantTextData;
				string targetString = FieLocalizeUtility.GetConstantText(_constantTextDataKey, out constantTextData);
				if (replaceMethod != null)
				{
					replaceMethod(ref targetString);
				}
				_tmpTextObject.font = ((!constantTextData.ForceEnableToUseEnglishFont) ? FieManagerBehaviour<FieEnvironmentManager>.I.currentFont : FieManagerBehaviour<FieEnvironmentManager>.I.englishFont);
				_tmpTextObject.text = targetString;
				_tmpTextObject.ForceMeshUpdate();
			}
		}
	}
}
