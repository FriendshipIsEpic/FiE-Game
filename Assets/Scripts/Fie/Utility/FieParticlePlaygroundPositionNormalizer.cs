using UnityEngine;

namespace Fie.Utility
{
	public class FieParticlePlaygroundPositionNormalizer : MonoBehaviour
	{
		[SerializeField]
		private Transform sourceTransform;

		private const float ROTATION_LOCK_DELAY = 0.1f;

		private Vector3 defaultPosition = Vector3.zero;

		private Quaternion defaultRotation = Quaternion.identity;

		private float rotationLockTimer;

		private bool isEnableSync;

		private void OnEnable()
		{
			if (sourceTransform != null)
			{
				defaultPosition = sourceTransform.position;
				defaultRotation = sourceTransform.rotation;
			}
			rotationLockTimer = 0f;
		}

		private void Update()
		{
			if (!(sourceTransform == null))
			{
				if (rotationLockTimer < 0.1f)
				{
					rotationLockTimer += Time.deltaTime;
					defaultRotation = sourceTransform.rotation;
				}
				base.transform.position = defaultPosition;
				base.transform.rotation = defaultRotation;
			}
		}
	}
}
