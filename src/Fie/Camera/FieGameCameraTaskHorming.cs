using Fie.Manager;
using UnityEngine;

namespace Fie.Camera
{
	public class FieGameCameraTaskHorming : FieGameCameraTaskBase
	{
		private Vector3 _hormingAncher = Vector3.zero;

		private float latestHormingDistance;

		private float targetHightOffset;

		private const float CAMERA_UPPER_DIRECTION_OFFSET_ANGLE_X = -4.5f;

		public override void Initialize(FieGameCamera gameCamera)
		{
			_hormingAncher = gameCamera.nowCameraTargetPos;
		}

		public override void TargetChanged(FieGameCamera gameCamera, FieGameCharacter fromCharacter, FieGameCharacter toCharacter)
		{
			gameCamera.SetCameraTask<FieGameCameraTaskLockOn>(0.75f);
		}

		public override void CameraUpdate(FieGameCamera gameCamera)
		{
			if (!(gameCamera.cameraOwner == null))
			{
				Vector3 defaultCameraOffset = FieManagerBehaviour<FieGameCameraManager>.I.getDefaultCameraOffset();
				Vector3 groundPosition = gameCamera.cameraOwner.groundPosition;
				float x = groundPosition.x + defaultCameraOffset.x;
				Vector3 groundPosition2 = gameCamera.cameraOwner.groundPosition;
				float y = groundPosition2.y + defaultCameraOffset.y;
				Vector3 groundPosition3 = gameCamera.cameraOwner.groundPosition;
				gameCamera.nowCameraTargetPos = new Vector3(x, y, groundPosition3.z + defaultCameraOffset.z);
				Vector3 groundPosition4 = gameCamera.cameraOwner.groundPosition;
				Vector3 a = new Vector3(0f, groundPosition4.y, 0f);
				Vector3 position = gameCamera.cameraOwner.transform.position;
				float b = -4.5f * Vector3.Distance(a, new Vector3(0f, position.y, 0f));
				targetHightOffset = Mathf.Lerp(targetHightOffset, b, 1f * Time.deltaTime);
				gameCamera.nowCameraTargetRotation = Quaternion.Euler(FieManagerBehaviour<FieGameCameraManager>.I.getDefaultCameraRotation() + new Vector3(targetHightOffset, 0f, 0f));
				float num = Vector3.Distance(new Vector3(_hormingAncher.x, gameCamera.nowCameraTargetPos.y, gameCamera.nowCameraTargetPos.z), gameCamera.nowCameraTargetPos);
				if (num < latestHormingDistance)
				{
					gameCamera.SetCameraTask<FieGameCameraTaskStop>(1f);
				}
				else
				{
					latestHormingDistance = num;
				}
			}
		}
	}
}
