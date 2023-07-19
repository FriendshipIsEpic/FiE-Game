using UnityEngine;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieSkillTreeDetailCursorUI : MonoBehaviour
	{
		[SerializeField]
		private Image _backGround;

		public RectTransform rectTransform;

		private void Awake()
		{
			rectTransform.localScale = Vector3.zero;
			_backGround.color = Color.clear;
		}

		public void Hide()
		{
			rectTransform.localScale = Vector3.zero;
			_backGround.color = Color.clear;
		}

		public void Show()
		{
			rectTransform.localScale = Vector3.one;
			_backGround.color = Color.white * 0.75f;
		}
	}
}
