using Fie.Utility;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieSkillTreeConfirmationUI : MonoBehaviour
	{
		public FieUIConstant2DText costText;

		public Button okButton;

		public Button cancelButton;

		public RectTransform rectTransform;

		private Tweener<TweenTypesOutSine> _transitionTweener = new Tweener<TweenTypesOutSine>();

		private Coroutine _transitionCoroutine;

		private float _currentRate;

		private IEnumerator ShowCoroutine(float targetRate)
		{
			okButton.interactable = false;
			cancelButton.interactable = false;
			_transitionTweener.InitTweener(0.1f, _currentRate, targetRate);
			if (!_transitionTweener.IsEnd())
			{
				_currentRate = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
				rectTransform.localScale = new Vector3(1f, _currentRate, 1f);
				yield return (object)null;
				/*Error: Unable to find new state assignment for yield return*/;
			}
			rectTransform.localScale = new Vector3(1f, targetRate, 1f);
			if (targetRate >= 1f)
			{
				okButton.interactable = true;
				cancelButton.interactable = true;
				EventSystem.current.SetSelectedGameObject(okButton.gameObject);
			}
		}

		public void Initialize()
		{
			_currentRate = 0f;
			rectTransform.localScale = new Vector3(1f, 0f, 1f);
			okButton.interactable = false;
			cancelButton.interactable = false;
		}

		public void Hide()
		{
			if (_transitionCoroutine != null)
			{
				StopCoroutine(_transitionCoroutine);
			}
			_transitionCoroutine = StartCoroutine(ShowCoroutine(0f));
		}

		public void Show()
		{
			if (_transitionCoroutine != null)
			{
				StopCoroutine(_transitionCoroutine);
			}
			_transitionCoroutine = StartCoroutine(ShowCoroutine(1f));
		}
	}
}
