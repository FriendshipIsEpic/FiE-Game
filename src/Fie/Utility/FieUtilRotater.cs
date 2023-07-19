using UnityEngine;

namespace Fie.Utility
{
	[ExecuteInEditMode]
	public class FieUtilRotater : MonoBehaviour
	{
		[SerializeField]
		private Vector3 rotateAngleInSec = Vector3.zero;

		private void Update()
		{
			base.transform.Rotate(rotateAngleInSec * Time.deltaTime);
		}
	}
}
