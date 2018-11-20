using Fie.Manager;
using TMPro;

namespace Fie.UI
{
	public class FieUITextMeshProObject : FieUILocalizedTextObjectBase
	{
		protected TMP_Text _tmpTextObject;

		public TMP_Text TmpTextObject
		{
			get
			{
				if (_tmpTextObject == null)
				{
					_tmpTextObject = base.gameObject.GetComponent<TMP_Text>();
				}
				return _tmpTextObject;
			}
		}

		protected virtual void Awake()
		{
			_tmpTextObject = base.gameObject.GetComponent<TMP_Text>();
			InitializeText();
		}

		public override void InitializeText()
		{
			_tmpTextObject.font = FieManagerBehaviour<FieEnvironmentManager>.I.currentFont;
			_tmpTextObject.ForceMeshUpdate();
		}
	}
}
