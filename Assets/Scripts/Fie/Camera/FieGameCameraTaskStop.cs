using Fie.Manager;
using UnityEngine;

namespace Fie.Camera
{
	public class FieGameCameraTaskStop : FieGameCameraTaskBase
	{
		private const float CAMERA_MAX_HORIZONTAL_DISTANCE = 3.5f;

		private const float CAMERA_MAX_FORWARD_DISTANCE = 7f;

		private const float CAMERA_MAX_VIRTICAL_DISTANCE = 1.5f;

		private const float CAMERA_UPPER_DIRECTION_OFFSET_ANGLE_X = -4.5f;

		private Vector3 initedPos = Vector3.zero;

		private Quaternion initedRotation = Quaternion.identity;

		private float targetHightOffset;

		public override void Initialize(FieGameCamera gameCamera)
		{
			Vector3 defaultCameraOffset = FieManagerBehaviour<FieGameCameraManager>.I.getDefaultCameraOffset();
			Vector3 defaultCameraRotation = FieManagerBehaviour<FieGameCameraManager>.I.getDefaultCameraRotation();
			if (gameCamera.cameraOwner != null)
			{
				Vector3 groundPosition = gameCamera.cameraOwner.groundPosition;
				float x = groundPosition.x + defaultCameraOffset.x;
				Vector3 groundPosition2 = gameCamera.cameraOwner.groundPosition;
				float y = groundPosition2.y + defaultCameraOffset.y;
				Vector3 groundPosition3 = gameCamera.cameraOwner.groundPosition;
				gameCamera.nowCameraTargetPos = new Vector3(x, y, groundPosition3.z + defaultCameraOffset.z);
				gameCamera.nowCameraTargetRotation = Quaternion.Euler(defaultCameraRotation);
			}
			initedPos = gameCamera.nowCameraTargetPos;
			initedRotation = gameCamera.nowCameraTargetRotation;
		}

		public override void TargetChanged(FieGameCamera gameCamera, FieGameCharacter fromCharacter, FieGameCharacter toCharacter)
		{
			gameCamera.SetCameraTask<FieGameCameraTaskLockOn>(0.75f);
		}

		public override void CameraUpdate(FieGameCamera gameCamera)
		{
			if (!(gameCamera.cameraOwner == null))
			{
				Vector3 a = new Vector3(gameCamera.nowCameraTargetPos.x, 0f, 0f);
				Vector3 groundPosition = gameCamera.cameraOwner.groundPosition;
				float num = Vector3.Distance(a, new Vector3(groundPosition.x, 0f, 0f));
				Vector3 a2 = new Vector3(0f, 0f, gameCamera.nowCameraTargetPos.z);
				Vector3 groundPosition2 = gameCamera.cameraOwner.groundPosition;
				float num2 = Vector3.Distance(a2, new Vector3(0f, 0f, groundPosition2.z));
				if (num > 3.5f || num2 > 7f)
				{
					gameCamera.SetCameraTask<FieGameCameraTaskHorming>(1f);
				}
				else
				{
					Vector3 vector = gameCamera.camera.WorldToScreenPoint(gameCamera.cameraOwner.guiPointTransform.position);
					if (vector.x <= 0f || vector.y <= 0f || vector.x > (float)Screen.width || vector.y > (float)Screen.height)
					{
						gameCamera.SetCameraTask<FieGameCameraTaskHorming>(1f);
					}
					else
					{
						Vector3 defaultCameraOffset = FieManagerBehaviour<FieGameCameraManager>.I.getDefaultCameraOffset();
						Vector3 nowCameraTargetPos = gameCamera.nowCameraTargetPos;
						float x = initedPos.x;
						Vector3 groundPosition3 = gameCamera.cameraOwner.groundPosition;
						gameCamera.nowCameraTargetPos = Vector3.Lerp(nowCameraTargetPos, new Vector3(x, groundPosition3.y + defaultCameraOffset.y, initedPos.z), 0.5f * Time.deltaTime);
						Vector3 groundPosition4 = gameCamera.cameraOwner.groundPosition;
						Vector3 a3 = new Vector3(0f, groundPosition4.y, 0f);
						Vector3 position = gameCamera.cameraOwner.transform.position;
						float b = -4.5f * Vector3.Distance(a3, new Vector3(0f, position.y, 0f));
						targetHightOffset = Mathf.Lerp(targetHightOffset, b, 1f * Time.deltaTime);
						gameCamera.nowCameraTargetRotation = Quaternion.Euler(FieManagerBehaviour<FieGameCameraManager>.I.getDefaultCameraRotation() + new Vector3(targetHightOffset, 0f, 0f));
					}
				}
			}
		}
	}
}
