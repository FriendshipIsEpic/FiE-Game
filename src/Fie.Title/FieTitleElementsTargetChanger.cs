using Fie.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Title
{
	public class FieTitleElementsTargetChanger : MonoBehaviour
	{
		public List<GameObject> objectList = new List<GameObject>();

		public float tweenTime = 1f;

		public float tweenInterval = 10f;

		public float firstInterval = 10f;

		private float _currentTweenInterval;

		private Tweener<TweenTypesInOutSine> positionTweener = new Tweener<TweenTypesInOutSine>();

		private int objectIndex;

		private Transform currentTransform;

		private Transform nextTransform;

		private void Start()
		{
			_currentTweenInterval = tweenInterval;
			positionTweener.InitTweener(tweenTime, 0f, 1f);
			currentTransform = objectList[objectIndex].transform;
			objectIndex = getNextPathPoint(objectIndex);
			nextTransform = objectList[objectIndex].transform;
		}

		private int getNextPathPoint(int currentIndex, int incrementCount = 1)
		{
			return Mathf.RoundToInt(Mathf.Repeat((float)(currentIndex + incrementCount), (float)objectList.Count));
		}

		private void Update()
		{
			if (firstInterval > 0f)
			{
				firstInterval -= Time.deltaTime;
			}
			else
			{
				float t = positionTweener.UpdateParameterFloat(Time.deltaTime);
				base.transform.position = Vector3.Slerp(currentTransform.position, nextTransform.position, t);
				if (positionTweener.IsEnd())
				{
					_currentTweenInterval -= Time.deltaTime;
					if (_currentTweenInterval <= 0f)
					{
						currentTransform = objectList[objectIndex].transform;
						objectIndex = getNextPathPoint(objectIndex);
						nextTransform = objectList[objectIndex].transform;
						positionTweener.InitTweener(tweenTime, 0f, 1f);
						_currentTweenInterval = tweenInterval;
					}
				}
			}
		}
	}
}
