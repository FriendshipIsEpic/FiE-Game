using UnityEngine;

namespace Fie.Utility
{
	[ExecuteInEditMode]
	public class FieUtilRotationLocker : MonoBehaviour
	{
		private void Update()
		{
			base.transform.rotation = Quaternion.identity;
		}
	}
}
