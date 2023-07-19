using Fie.Manager;
using TMPro;
using UnityEngine;

namespace Fie.UI
{
	public class FieGameUIDialogCaption : FieGameUIBase
	{
		[SerializeField]
		private TextMeshProUGUI _captionText;

		private void Awake()
		{
			_captionText.font = FieManagerBehaviour<FieEnvironmentManager>.I.currentFont;
		}

		public void SetText(string text)
		{
			if (!(_captionText == null))
			{
				_captionText.text = text;
			}
		}

		public void ClearText()
		{
			if (!(_captionText == null))
			{
				_captionText.text = string.Empty;
			}
		}
	}
}
