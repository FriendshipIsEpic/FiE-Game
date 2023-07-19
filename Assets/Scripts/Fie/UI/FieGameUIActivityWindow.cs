using Fie.Utility;
using UnityEngine;

namespace Fie.UI
{
	public class FieGameUIActivityWindow : FieGameUIBase
	{
		public enum ActivityWindowState
		{
			IDLE,
			SHOWING,
			HIDING,
			BUSY
		}

		[SerializeField]
		private RectTransform windowRectTransform;

		[SerializeField]
		private float tweenTime = 0.3f;

		[SerializeField]
		private float minWindowSizeWidth = 192f;

		[SerializeField]
		private float minWindowSizeHeight = 192f;

		[SerializeField]
		private float windowSizeMarginWidth = 64f;

		[SerializeField]
		private float windowSizeMarginHeight = 64f;

		[SerializeField]
		private FieGameUIActivityWindowText titleTextComponent;

		[SerializeField]
		private FieGameUIActivityWindowText noteTextComponent;

		private Tweener<TweenTypesOutSine> _showingTweener = new Tweener<TweenTypesOutSine>();

		private Tweener<TweenTypesInSine> _hidingTweener = new Tweener<TweenTypesInSine>();

		private Vector2 _currentWindowTargetSize = Vector2.zero;

		private float _currentSizeRate;

		private ActivityWindowState _activityWindowState;

		public ActivityWindowState activityWindowState => _activityWindowState;

		private void Start()
		{
			titleTextComponent.SetText(string.Empty);
			titleTextComponent.SetText(string.Empty);
			_currentWindowTargetSize = Vector2.zero;
			windowRectTransform.sizeDelta = _currentWindowTargetSize;
		}

		public void HideText()
		{
			if (!_showingTweener.IsEnd())
			{
				_showingTweener = new Tweener<TweenTypesOutSine>();
			}
			titleTextComponent.HideText();
			noteTextComponent.HideText();
			_hidingTweener.InitTweener(tweenTime, _currentSizeRate, 0f);
			_activityWindowState = ActivityWindowState.HIDING;
		}

		public void ShowText(string titleText, string noteText)
		{
			if (!_hidingTweener.IsEnd())
			{
				_hidingTweener = new Tweener<TweenTypesInSine>();
			}
			Vector2 vector = titleTextComponent.SetText(titleText);
			Vector2 vector2 = noteTextComponent.SetText(noteText);
			_currentWindowTargetSize.x = Mathf.Max(Mathf.Max(vector.x, vector2.x) + windowSizeMarginWidth, minWindowSizeWidth);
			_currentWindowTargetSize.y = Mathf.Max(Mathf.Max(vector.y, vector2.y) + windowSizeMarginHeight, minWindowSizeHeight);
			_showingTweener.InitTweener(tweenTime, _currentSizeRate, 1f);
			_activityWindowState = ActivityWindowState.SHOWING;
		}

		private void Update()
		{
			if (!(windowRectTransform == null))
			{
				if (!_hidingTweener.IsEnd())
				{
					_currentSizeRate = _hidingTweener.UpdateParameterFloat(Time.deltaTime);
					if (_hidingTweener.IsEnd())
					{
						_activityWindowState = ActivityWindowState.IDLE;
					}
				}
				else if (!_showingTweener.IsEnd())
				{
					_currentSizeRate = _showingTweener.UpdateParameterFloat(Time.deltaTime);
					if (_showingTweener.IsEnd())
					{
						titleTextComponent.ShowText();
						noteTextComponent.ShowText();
						_activityWindowState = ActivityWindowState.IDLE;
					}
				}
				windowRectTransform.sizeDelta = _currentWindowTargetSize * _currentSizeRate;
			}
		}
	}
}
