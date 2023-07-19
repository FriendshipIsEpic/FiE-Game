using Fie.Utility;
using UnityEngine;

namespace Fie.UI
{
	public class FieSkillTreeSkillGroupButtonRoot : MonoBehaviour
	{
		private Tweener<TweenTypesInOutSine> _alphaTweener = new Tweener<TweenTypesInOutSine>();

		private float currentDispalyingRate;

		private Vector3 initializedLocalPosition = Vector3.zero;

		private void Awake()
		{
			initializedLocalPosition = base.transform.localPosition;
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
				localPosition.x = initializedLocalPosition.x - (float)Screen.width * 0.5f * (1f - currentDispalyingRate);
				base.transform.localPosition = localPosition;
			}
		}
	}
}
