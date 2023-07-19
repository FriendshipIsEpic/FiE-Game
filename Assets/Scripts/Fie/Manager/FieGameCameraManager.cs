using Fie.Camera;
using Fie.Utility;
using System;
using UnityEngine;

namespace Fie.Manager
{
	public class FieGameCameraManager : FieManagerBehaviour<FieGameCameraManager>
	{
		private static readonly Vector3 defaultCameraOffset = new Vector3(0f, 1.5f, -5.5f);

		private static readonly Vector3 defaultCameraRotation = new Vector3(2f, 0f, 0f);

		private Vector3 _sceneCameraOffset = defaultCameraOffset;

		private Vector3 _sceneCameraRotation = defaultCameraRotation;

		private FieGameCamera _gameCamera;

		public FieGameCamera gameCamera => _gameCamera;

		protected override void StartUpEntity()
		{
			if (_gameCamera == null)
			{
				_gameCamera = UnityEngine.Object.FindObjectOfType<FieGameCamera>();
				if (_gameCamera == null)
				{
					GameObject gameObject = Resources.Load("Prefabs/Manager/FieGameCamera") as GameObject;
					if (gameObject == null)
					{
						throw new Exception("Missing the game camera prefab.");
					}
					GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
					if (gameObject2 == null)
					{
						throw new Exception("Fiald to instantiate the game camera object.");
					}
					_gameCamera = gameObject2.GetComponent<FieGameCamera>();
					if (_gameCamera == null)
					{
						throw new Exception("Game camera component dosen't exists in game camera prefab.");
					}
				}
			}
			_gameCamera.transform.parent = base.transform;
			_gameCamera.setCameraOwner(FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter);
		}

		public void setWiggler(Wiggler.WiggleTemplate template)
		{
			gameCamera.setWiggler(template);
		}

		public void setWiggler(float totalTime, int wiggleCount, Vector3 wiggleScale)
		{
			gameCamera.setWiggler(totalTime, wiggleCount, wiggleScale);
		}

		public void setDefaultCameraOffset(Vector3 newDefaultOffset)
		{
			_sceneCameraOffset = newDefaultOffset;
		}

		public void setDefaultCameraRotation(Vector3 newDefaultRotation)
		{
			_sceneCameraRotation = newDefaultRotation;
		}

		public Vector3 getDefaultCameraOffset()
		{
			return _sceneCameraOffset;
		}

		public Vector3 getDefaultCameraRotation()
		{
			return _sceneCameraRotation;
		}
	}
}
