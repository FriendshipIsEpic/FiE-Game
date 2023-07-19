using Fie.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieUGUIUtilityGauge : MonoBehaviour
	{
		public enum GaugeDirection
		{
			HORIZONTAL,
			VIRTICAL
		}

		private const float DEFAULT_TRANSITION_TIME = 0.5f;

		[SerializeField]
		private Image _gaugeImageObject;

		[SerializeField]
		private GaugeDirection _gaugeDirection;

		[SerializeField]
		private float _maxValue = 1f;

		[SerializeField]
		public float _minValue;

		[SerializeField]
		public float _currentValue;

		private float _inherentCurrentValue;

		private Vector3 _currentGaugePosition;

		private Tweener<TweenTypesOutSine> _gaugeAnimationTweener = new Tweener<TweenTypesOutSine>();

		public float maxValue
		{
			get
			{
				return _maxValue;
			}
			set
			{
				_maxValue = value;
			}
		}

		public float minValue
		{
			get
			{
				return _minValue;
			}
			set
			{
				_minValue = value;
			}
		}

		public float currentValue
		{
			get
			{
				return _currentValue;
			}
			set
			{
				_currentValue = value;
			}
		}

		public void Initialize(float minValue, float maxValue, float initValue, float transitionTime = 0.5f)
		{
			_minValue = minValue;
			_maxValue = maxValue;
			SetValue(initValue, transitionTime);
		}

		public void SetValue(float value, float transitionTime = 0.5f)
		{
			currentValue = Mathf.Clamp(value, minValue, maxValue);
			if (transitionTime > 0f)
			{
				_gaugeAnimationTweener.InitTweener(transitionTime, _inherentCurrentValue, currentValue);
			}
			else
			{
				_inherentCurrentValue = currentValue;
				GaugeUpdate();
			}
		}

		private void Awake()
		{
			if (_gaugeImageObject != null)
			{
				_currentGaugePosition = _gaugeImageObject.rectTransform.anchoredPosition;
				GaugeUpdate();
			}
		}

		public void Update()
		{
			if (!_gaugeAnimationTweener.IsEnd())
			{
				_inherentCurrentValue = _gaugeAnimationTweener.UpdateParameterFloat(Time.deltaTime);
				GaugeUpdate();
			}
		}

		private void GaugeUpdate()
		{
			if (maxValue <= 0f)
			{
				_currentGaugePosition.x = 0f;
				_gaugeImageObject.rectTransform.anchoredPosition = _currentGaugePosition;
			}
			else
			{
				if (_gaugeDirection == GaugeDirection.HORIZONTAL)
				{
					ref Vector3 currentGaugePosition = ref _currentGaugePosition;
					Vector2 sizeDelta = _gaugeImageObject.rectTransform.sizeDelta;
					currentGaugePosition.x = (0f - sizeDelta.x) * Mathf.Clamp(1f - _inherentCurrentValue / maxValue, 0f, 1f);
				}
				else
				{
					ref Vector3 currentGaugePosition2 = ref _currentGaugePosition;
					Vector2 sizeDelta2 = _gaugeImageObject.rectTransform.sizeDelta;
					currentGaugePosition2.y = (0f - sizeDelta2.y) * Mathf.Clamp(1f - _inherentCurrentValue / maxValue, 0f, 1f);
				}
				_gaugeImageObject.rectTransform.anchoredPosition = _currentGaugePosition;
			}
		}
	}
}
