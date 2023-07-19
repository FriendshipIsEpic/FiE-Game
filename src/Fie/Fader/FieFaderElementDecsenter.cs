using Fie.Utility;
using UnityEngine;

namespace Fie.Fader
{
	internal class FieFaderElementDecsenter : MonoBehaviour
	{
		public Vector3 moving = Vector3.zero;

		public float movingCycleSec = 1f;

		private bool isRepeat;

		private Vector3 currentPosition = Vector3.zero;

		private Tweener<TweenTypesInOutSine> positionTweener = new Tweener<TweenTypesInOutSine>();

		private void Start()
		{
			currentPosition = base.transform.position;
		}

		private void Update()
		{
			if (positionTweener.IsEnd())
			{
				positionTweener.InitTweener(movingCycleSec, (!isRepeat) ? moving : (moving * -1f), (!isRepeat) ? (moving * -1f) : moving);
				isRepeat = !isRepeat;
			}
			base.transform.position = currentPosition + positionTweener.UpdateParameterVec3(Time.deltaTime);
		}
	}
}
