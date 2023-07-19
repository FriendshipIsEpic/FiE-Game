using Fie.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieSkillTreeDetailGuideUI : MonoBehaviour
	{
		private Color guideImageColor = new Color(0.5f, 0.5f, 0.5f, 1f);

		private Color guideTextColor = new Color(1f, 1f, 1f, 1f);

		[SerializeField]
		private FieUIConstant2DText[] _skllLevelTexts = new FieUIConstant2DText[4];

		[SerializeField]
		private Image _guideImage;

		private Tweener<TweenTypesInOutSine> _alphaTweener = new Tweener<TweenTypesInOutSine>();

		private float currentDispalyingRate;

		private Vector3 initializedLocalPosition = Vector3.zero;

		private void Awake()
		{
			initializedLocalPosition = base.transform.localPosition;
		}

		private void Start()
		{
			_skllLevelTexts[0].replaceMethod = delegate(ref string targetString)
			{
				targetString = targetString.Replace("___Value1___", 1.ToString());
			};
			_skllLevelTexts[1].replaceMethod = delegate(ref string targetString)
			{
				targetString = targetString.Replace("___Value1___", 2.ToString());
			};
			_skllLevelTexts[2].replaceMethod = delegate(ref string targetString)
			{
				targetString = targetString.Replace("___Value1___", 3.ToString());
			};
			_skllLevelTexts[3].replaceMethod = delegate(ref string targetString)
			{
				targetString = targetString.Replace("___Value1___", 4.ToString());
			};
			for (int i = 0; i < _skllLevelTexts.Length; i++)
			{
				_skllLevelTexts[i].InitializeText();
				_skllLevelTexts[i].TmpTextObject.color = Color.clear;
			}
			_guideImage.color = Color.clear;
		}

		public void Show()
		{
			_alphaTweener.InitTweener(1.5f, currentDispalyingRate, 1f);
		}

		public void Hide()
		{
			_alphaTweener.InitTweener(1.5f, currentDispalyingRate, 0f);
		}

		private void Update()
		{
			if (!_alphaTweener.IsEnd())
			{
				currentDispalyingRate = _alphaTweener.UpdateParameterFloat(Time.deltaTime);
				for (int i = 0; i < _skllLevelTexts.Length; i++)
				{
					_skllLevelTexts[i].TmpTextObject.color = guideTextColor * currentDispalyingRate;
				}
				_guideImage.color = guideImageColor * currentDispalyingRate;
				Vector3 localPosition = initializedLocalPosition;
				localPosition.y = initializedLocalPosition.y + 50f * (1f - currentDispalyingRate);
				base.transform.localPosition = localPosition;
			}
		}
	}
}
