using UnityEngine;

namespace Fie.Title
{
	public class FieTitleCameraScaler : MonoBehaviour
	{
		public Transform syncScaleTransform;

		private void Start()
		{
		}

		private void LateUpdate()
		{
			if (syncScaleTransform != null)
			{
				base.transform.localScale = syncScaleTransform.transform.localScale;
			}
		}
	}
}
