using UnityEngine;

namespace Fie.Utility
{
	public class FieUtilPopcornFxForAnimation : MonoBehaviour
	{
		[SerializeField]
		private PKFxFX fx;

		public void PlayPopcornFX()
		{
			if (fx != null)
			{
				fx.StopEffect();
				fx.StartEffect();
			}
		}
	}
}
