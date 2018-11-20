using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	public class FieRainbowDashTrailRotator : MonoBehaviour
	{
		[SerializeField]
		private float axisZWiggleWidth = 25f;

		[SerializeField]
		private float axisZSinSpeedPerSec = 30f;

		[SerializeField]
		private float axisYRotateSpeedPerSec = 120f;

		private Vector3 trailVecotrNormal = new Vector3(0f, 1f, 0f);

		private float axisZRotater;

		private float axisYRotater;

		private void OnEnable()
		{
			axisZRotater = Random.Range(0f, 180f);
			axisYRotater = Random.Range(0f, 360f);
		}

		private void Update()
		{
			axisZRotater = Mathf.Repeat(axisZRotater + axisZSinSpeedPerSec * Time.deltaTime, 180f);
			axisYRotater = Mathf.Repeat(axisYRotater + axisYRotateSpeedPerSec * Time.deltaTime, 360f);
			base.transform.rotation = Quaternion.AngleAxis(axisYRotater, Quaternion.AngleAxis(Mathf.Sin(axisZRotater) * axisZWiggleWidth, Vector3.forward) * trailVecotrNormal);
		}
	}
}
