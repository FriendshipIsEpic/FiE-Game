using Fie.Utility;
using UnityEngine;

namespace Fie.Title
{
	public class FieTitleElementsRotater : MonoBehaviour
	{
		public enum AxisType
		{
			XYZ,
			YXZ
		}

		public AxisType axisType;

		public Vector3 clickRotForce = Vector3.zero;

		public float clickRotDuration;

		public Vector3 rotatePerSec = Vector3.zero;

		public Vector3 currentRotateion = Vector3.zero;

		private Tweener<TweenTypesOutSine> clickRotTweener = new Tweener<TweenTypesOutSine>();

		private Vector3 currentClickForce = Vector3.zero;

		private void Start()
		{
		}

		private void Update()
		{
			if (Input.anyKeyDown)
			{
				currentClickForce += clickRotForce * 20f;
				currentClickForce.x = Mathf.Min(currentClickForce.x, clickRotForce.x * 60f);
				currentClickForce.y = Mathf.Min(currentClickForce.y, clickRotForce.y * 60f);
				currentClickForce.z = Mathf.Min(currentClickForce.z, clickRotForce.z * 60f);
			}
			currentClickForce = Vector3.Lerp(currentClickForce, Vector3.zero, 0.95f * Time.deltaTime);
			currentRotateion.x += rotatePerSec.x * Time.deltaTime;
			currentRotateion.y += rotatePerSec.y * Time.deltaTime;
			currentRotateion.z += rotatePerSec.z * Time.deltaTime;
			currentRotateion += currentClickForce * Time.deltaTime;
			currentRotateion.x = Mathf.Repeat(currentRotateion.x, 360f);
			currentRotateion.y = Mathf.Repeat(currentRotateion.y, 360f);
			currentRotateion.z = Mathf.Repeat(currentRotateion.z, 360f);
			Quaternion quaternion = Quaternion.AngleAxis(currentRotateion.x, Vector3.right);
			Quaternion quaternion2 = Quaternion.AngleAxis(currentRotateion.y, Vector3.up);
			Quaternion rhs = Quaternion.AngleAxis(currentRotateion.z, Vector3.forward);
			switch (axisType)
			{
			case AxisType.XYZ:
				base.transform.rotation = quaternion * quaternion2 * rhs;
				break;
			case AxisType.YXZ:
				base.transform.rotation = quaternion2 * quaternion * rhs;
				break;
			}
		}
	}
}
