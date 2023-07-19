using Fie.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Title
{
	public class FieTitleElementsCicleScaleTilter : MonoBehaviour
	{
		public List<float> tweenScaleList = new List<float>
		{
			1f,
			1.5f,
			0.5f
		};

		public float tweenTime = 1f;

		public float tweenInterval = 10f;

		private float _currentTweenInterval;

		private Tweener<TweenTypesInOutSine> scaleTweener = new Tweener<TweenTypesInOutSine>();

		private int tweenScaleIndex;

		private void Start()
		{
			_currentTweenInterval = tweenInterval;
			scaleTweener.InitTweener(1f, tweenScaleList[0], tweenScaleList[0]);
		}

		private void Update()
		{
			float num = scaleTweener.UpdateParameterFloat(Time.deltaTime);
			base.transform.localScale = new Vector3(num, num, num);
			if (scaleTweener.IsEnd())
			{
				_currentTweenInterval -= Time.deltaTime;
				if (_currentTweenInterval <= 0f)
				{
					tweenScaleIndex++;
					if (tweenScaleIndex >= tweenScaleList.Count)
					{
						tweenScaleIndex = 0;
					}
					scaleTweener.InitTweener(tweenTime, num, tweenScaleList[tweenScaleIndex]);
					_currentTweenInterval = tweenInterval;
				}
			}
		}
	}
}
