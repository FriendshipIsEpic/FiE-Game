using Fie.Utility;
using GameDataEditor;
using UnityEngine;

namespace Fie.UI
{
	public class FieSkillTreeHeaderUI : MonoBehaviour
	{
		[SerializeField]
		private FieUIConstant2DText _headerText;

		private Tweener<TweenTypesInOutSine> _alphaTweener = new Tweener<TweenTypesInOutSine>();

		private float currentDispalyingRate;

		private Vector3 initializedLocalPosition = Vector3.zero;

		private void Awake()
		{
			initializedLocalPosition = base.transform.localPosition;
		}

		public void InitWithGameCharacterData(GDEGameCharacterTypeData gameCharacterData)
		{
			_headerText.replaceMethod = delegate(ref string targetString)
			{
				string constantText = FieLocalizeUtility.GetConstantText(gameCharacterData.CharacterName.Key);
				targetString = targetString.Replace("___Value1___", constantText);
			};
			_headerText.InitializeText();
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
				Vector3 localPosition = initializedLocalPosition;
				localPosition.y = initializedLocalPosition.y + 150f * (1f - currentDispalyingRate);
				base.transform.localPosition = localPosition;
			}
		}
	}
}
