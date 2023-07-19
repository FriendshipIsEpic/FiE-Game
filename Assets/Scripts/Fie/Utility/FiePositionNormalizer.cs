using UnityEngine;

namespace Fie.Utility
{
	public class FiePositionNormalizer : MonoBehaviour
	{
		private const float ROTATION_LOCK_DELAY = 0.1f;

		private Vector3 defaultPosition = Vector3.zero;

		private Quaternion defaultRotation = Quaternion.identity;

		private float rotationLockTimer;

		private void Start()
		{
			defaultPosition = base.transform.position;
		}

		private void Update()
		{
			if (rotationLockTimer < 0.1f)
			{
				rotationLockTimer += Time.deltaTime;
				defaultRotation = base.transform.rotation;
			}
			base.transform.position = defaultPosition;
			base.transform.rotation = defaultRotation;
		}
	}
}
