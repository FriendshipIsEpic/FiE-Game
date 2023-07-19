using UnityEngine;

namespace Fie.UI
{
	public class FieResultLevelEffect : MonoBehaviour
	{
		[SerializeField]
		private Animation levelUpAnimation;

		public void PlayLevelupAnimation()
		{
			if (levelUpAnimation.isPlaying)
			{
				levelUpAnimation.Stop();
			}
			levelUpAnimation.Play();
		}
	}
}
