using UnityEngine;

namespace Fie.Camera
{
	public class FieUICamera : FieCameraBase
	{
		public delegate void ScreenResizeEvent();

		private int _latestWidth;

		private int _latestHeight;

		private FieGameCamera _gameCamera;

		private Vector3 leftTopInWorldPosition;

		private Vector3 leftBottomInWorldPosition;

		private Vector3 rightTopInWorldPosition;

		private Vector3 rightBottomInWorldPosition;

		private float _screenHeight;

		private float _screenWidth;

		private float _uiWorldHeight;

		private float _uiWorldWidth;

		public event ScreenResizeEvent screenResizeEvent;

		protected override void Start()
		{
			base.Start();
			InitializeUICameraResolution();
		}

		private void LateUpdate()
		{
			if (Screen.height != _latestHeight || Screen.width != _latestWidth)
			{
				InitializeUICameraResolution();
			}
		}

		private void InitializeUICameraResolution()
		{
			_screenHeight = (float)Screen.height;
			_screenWidth = (float)Screen.width;
			leftBottomInWorldPosition = base.camera.ScreenToWorldPoint(Vector3.zero);
			leftTopInWorldPosition = base.camera.ScreenToWorldPoint(new Vector3(0f, _screenHeight));
			rightBottomInWorldPosition = base.camera.ScreenToWorldPoint(new Vector3(_screenWidth, 0f));
			rightTopInWorldPosition = base.camera.ScreenToWorldPoint(new Vector3(_screenWidth, _screenHeight));
			_uiWorldHeight = Vector3.Distance(leftBottomInWorldPosition, leftTopInWorldPosition);
			_uiWorldWidth = Vector3.Distance(leftBottomInWorldPosition, rightBottomInWorldPosition);
			_latestHeight = Screen.height;
			_latestWidth = Screen.width;
			if (this.screenResizeEvent != null)
			{
				this.screenResizeEvent();
			}
		}

		public void setGameCamera(FieGameCamera gameChamera)
		{
			_gameCamera = gameChamera;
		}

		public Vector3 getPositionInUICameraWorld(Vector3 inGamePosition)
		{
			if (_gameCamera == null)
			{
				Debug.Log("game camera didn't assign to ui camera.");
				return Vector3.zero;
			}
			Vector3 vector = _gameCamera.camera.WorldToScreenPoint(inGamePosition);
			return new Vector3(leftBottomInWorldPosition.x + _uiWorldWidth * (vector.x / _screenWidth), leftBottomInWorldPosition.y + _uiWorldHeight * (vector.y / _screenHeight));
		}

		public bool isOnScreen(Vector3 worldPosition, ref Vector3 screenPosition)
		{
			screenPosition = _gameCamera.camera.WorldToScreenPoint(worldPosition);
			if (screenPosition.x < 0f || screenPosition.x > (float)Screen.width || screenPosition.y < 0f || screenPosition.y > (float)Screen.height)
			{
				return false;
			}
			return true;
		}
	}
}
