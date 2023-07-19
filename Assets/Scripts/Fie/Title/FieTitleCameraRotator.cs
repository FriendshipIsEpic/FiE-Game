using UnityEngine;

namespace Fie.Title
{
	public class FieTitleCameraRotator : MonoBehaviour
	{
		public Transform lookTransform;

		public Vector3 tiltRange = Vector3.zero;

		public float tiltTime = 4f;

		private float tiltSin;

		private Vector3 initPos = Vector3.zero;

		private void Start()
		{
			initPos = base.transform.position;
		}

		private void Update()
		{
			tiltSin += 90f / tiltTime * Time.deltaTime;
			tiltSin = Mathf.Repeat(tiltSin, 360f);
			base.transform.rotation = Quaternion.AngleAxis(tiltRange.x * Mathf.Sin(tiltSin), Vector3.right) * Quaternion.AngleAxis(tiltRange.y * Mathf.Sin(tiltSin), Vector3.up);
		}
	}
}
