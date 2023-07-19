using System;
using UnityEngine;

namespace Fie.Voice
{
	[Serializable]
	public class FiePhonemeAnimationData
	{
		[SerializeField]
		public Phoneme phoneme;

		[SerializeField]
		public string animationName;

		public FiePhonemeAnimationData(Phoneme phoneme, string animationName)
		{
			this.phoneme = phoneme;
			this.animationName = animationName;
		}
	}
}
