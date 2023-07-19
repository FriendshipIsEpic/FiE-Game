using Fie.Manager;
using UnityEngine;

namespace Fie.Camera
{
	public class FieGameCameraTaskLockOn : FieGameCameraTaskBase
	{
		public const float DEFAULT_LOCKON_TRASITION_TIME = 0.75f;

		private const float MAXIMUM_LOCKON_DISTANCE = 6f;

		private Vector3 _hormingAncher = Vector3.zero;

		private float latestHormingDistance;

		private float targetHightOffset;

		public bool isCameraHorming = true;

		private const float CAMERA_UPPER_DIRECTION_OFFSET_ANGLE_X = -4.5f;

		public override void Initialize(FieGameCamera gameCamera)
		{
			_hormingAncher = gameCamera.nowCameraTargetPos;
		}

		public override void Terminate(FieGameCamera gameCamera)
		{
			gameCamera.InitTargetMakerScreenPos();
		}

		public override void TargetChanged(FieGameCamera gameCamera, FieGameCharacter fromCharacter, FieGameCharacter toCharacter)
		{
			if (isCameraHorming)
			{
				gameCamera.SetCameraTask<FieGameCameraTaskLockOn>(0.75f);
			}
		}

		public override void TargetMissed(FieGameCamera gameCamera)
		{
			gameCamera.SetCameraTask<FieGameCameraTaskStop>(1f);
		}

		public override void CameraUpdate(FieGameCamera gameCamera)
		{
			if (!(gameCamera.cameraOwner == null))
			{
				if (isCameraHorming)
				{
					if (gameCamera.cameraOwner.detector.lockonTargetObject == null || gameCamera.cameraOwner.detector.lockonTargetObject.transform == null)
					{
						gameCamera.SetCameraTask<FieGameCameraTaskStop>(1f);
					}
					else
					{
						float num = 0f;
						float num2 = Vector3.Distance(gameCamera.cameraOwner.detector.lockonTargetObject.centerTransform.position, gameCamera.cameraOwner.centerTransform.position);
						if (num2 > 6f)
						{
							num = Mathf.Min((num2 - 6f) * 1f, 2f);
						}
						Vector3 vector = Vector3.Lerp(gameCamera.cameraOwner.transform.position, gameCamera.cameraOwner.detector.lockonTargetObject.position, 0.5f);
						Vector3 defaultCameraOffset = FieManagerBehaviour<FieGameCameraManager>.I.getDefaultCameraOffset();
						Vector3 nowCameraTargetPos = gameCamera.nowCameraTargetPos;
						float x = vector.x + defaultCameraOffset.x;
						Vector3 groundPosition = gameCamera.cameraOwner.groundPosition;
						float y = groundPosition.y + defaultCameraOffset.y;
						Vector3 groundPosition2 = gameCamera.cameraOwner.groundPosition;
						gameCamera.nowCameraTargetPos = Vector3.Lerp(nowCameraTargetPos, new Vector3(x, y, groundPosition2.z + defaultCameraOffset.z - num), 3f * Time.deltaTime);
						gameCamera.tagetMakerScreenPos = gameCamera.camera.WorldToScreenPoint(gameCamera.cameraOwner.detector.lockonTargetObject.centerTransform.position);
						Vector3 groundPosition3 = gameCamera.cameraOwner.groundPosition;
						Vector3 a = new Vector3(0f, groundPosition3.y, 0f);
						Vector3 position = gameCamera.cameraOwner.transform.position;
						float b = -4.5f * Vector3.Distance(a, new Vector3(0f, position.y, 0f));
						targetHightOffset = Mathf.Lerp(targetHightOffset, b, 1f * Time.deltaTime);
						gameCamera.nowCameraTargetRotation = Quaternion.Euler(FieManagerBehaviour<FieGameCameraManager>.I.getDefaultCameraRotation() + new Vector3(targetHightOffset, 0f, 0f));
					}
				}
				else
				{
					Vector3 vector2 = gameCamera.camera.ScreenToWorldPoint(gameCamera.tagetMakerScreenPos);
					Vector3 position2 = gameCamera.cameraOwner.transform.position;
					vector2.z = position2.z;
					float num3 = 0f;
					float num4 = Vector3.Distance(vector2, gameCamera.cameraOwner.centerTransform.position);
					if (num4 > 6f)
					{
						num3 = Mathf.Min((num4 - 6f) * 1f, 4f);
					}
					Vector3 vector3 = Vector3.Lerp(gameCamera.cameraOwner.transform.position, vector2, 0.5f);
					Vector3 defaultCameraOffset2 = FieManagerBehaviour<FieGameCameraManager>.I.getDefaultCameraOffset();
					Vector3 nowCameraTargetPos2 = gameCamera.nowCameraTargetPos;
					float x2 = vector3.x + defaultCameraOffset2.x;
					Vector3 groundPosition4 = gameCamera.cameraOwner.groundPosition;
					float y2 = groundPosition4.y + defaultCameraOffset2.y;
					Vector3 groundPosition5 = gameCamera.cameraOwner.groundPosition;
					gameCamera.nowCameraTargetPos = Vector3.Lerp(nowCameraTargetPos2, new Vector3(x2, y2, groundPosition5.z + defaultCameraOffset2.z - num3), 4f * Time.deltaTime);
					Vector3 groundPosition6 = gameCamera.cameraOwner.groundPosition;
					float b2 = -4.5f * Vector3.Distance(new Vector3(0f, groundPosition6.y, 0f), new Vector3(0f, vector2.y, 0f));
					targetHightOffset = Mathf.Lerp(targetHightOffset, b2, 2f * Time.deltaTime);
					gameCamera.nowCameraTargetRotation = Quaternion.Euler(FieManagerBehaviour<FieGameCameraManager>.I.getDefaultCameraRotation() + new Vector3(targetHightOffset * 0.5f, 0f, 0f));
					float num5 = latestHormingDistance = Vector3.Distance(new Vector3(_hormingAncher.x, gameCamera.nowCameraTargetPos.y, gameCamera.nowCameraTargetPos.z), gameCamera.nowCameraTargetPos);
				}
			}
		}
	}
}
