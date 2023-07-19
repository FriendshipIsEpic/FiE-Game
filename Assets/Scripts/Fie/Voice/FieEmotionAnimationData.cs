using System;
using UnityEngine;

namespace Fie.Voice
{
	[Serializable]
	public class FieEmotionAnimationData
	{
		[SerializeField]
		public string emotion;

		[SerializeField]
		public string animationName;

		public FieEmotionAnimationData(string emotion, string animationName)
		{
			this.emotion = emotion;
			this.animationName = animationName;
		}
	}
}
