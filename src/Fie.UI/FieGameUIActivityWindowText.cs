using Fie.Manager;
using Fie.Utility;
using TMPro;
using UnityEngine;

namespace Fie.UI
{
	public class FieGameUIActivityWindowText : MonoBehaviour
	{
		private TextMeshProUGUI textComponent;

		private Tweener<TweenTypesInOutSine> _colorTweener = new Tweener<TweenTypesInOutSine>();

		private float _nowRate;

		private float _delay;

		private void Awake()
		{
			textComponent = GetComponent<TextMeshProUGUI>();
			textComponent.font = FieManagerBehaviour<FieEnvironmentManager>.I.currentFont;
		}

		public Vector2 SetText(string text)
		{
			textComponent.text = text;
			textComponent.ForceMeshUpdate();
			return new Vector2(textComponent.renderedWidth, textComponent.renderedHeight);
		}

		public void HideText(float showTime = 0.3f)
		{
			if (showTime > 0f)
			{
				_delay = showTime;
				_colorTweener.InitTweener(showTime, _nowRate, 0f);
			}
			else
			{
				textComponent.color = Color.white * 0f;
				_nowRate = 0f;
				_colorTweener.Finish();
			}
		}

		public void ShowText(float showTime = 0.3f)
		{
			if (showTime > 0f)
			{
				_delay = showTime;
				_nowRate = 0f;
				_colorTweener.InitTweener(showTime, _nowRate, 1f);
			}
			else
			{
				textComponent.color = Color.white;
				_nowRate = 1f;
				_colorTweener.Finish();
			}
		}

		private void Update()
		{
			if (_delay > 0f)
			{
				_delay -= Time.deltaTime;
			}
			else
			{
				if (!_colorTweener.IsEnd())
				{
					_nowRate = _colorTweener.UpdateParameterFloat(Time.deltaTime);
				}
				textComponent.color = Color.white * _nowRate;
			}
		}
	}
}
