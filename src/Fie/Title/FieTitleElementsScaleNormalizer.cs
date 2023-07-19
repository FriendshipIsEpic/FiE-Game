using UnityEngine;

namespace Fie.Title
{
	public class FieTitleElementsScaleNormalizer : MonoBehaviour
	{
		private Vector3 defaultScale = Vector3.zero;

		private void Start()
		{
			defaultScale = base.transform.lossyScale;
		}

		private void LateUpdate()
		{
			Vector3 lossyScale = base.transform.lossyScale;
			Vector3 localScale = base.transform.localScale;
			base.transform.localScale = new Vector3(localScale.x / lossyScale.x * defaultScale.x, localScale.y / lossyScale.y * defaultScale.y, localScale.z / lossyScale.z * defaultScale.z);
		}
	}
}
