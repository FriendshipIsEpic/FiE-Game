using UnityEngine;

namespace Fie.UI
{
	public class FieLobbySelectableUIHeader : MonoBehaviour
	{
		[SerializeField]
		private FieUIConstant2DText _titleText;

		[SerializeField]
		private FieUIConstant2DText _descText;

		public void SetHeaderTexts(string titleText, string descText)
		{
			_titleText.TmpTextObject.text = titleText;
			_descText.TmpTextObject.text = descText;
		}
	}
}
