using Fie.Manager;
using Spine.Unity;
using UnityEngine;

namespace Fie.UI
{
	[RequireComponent(typeof(SpineAnimation))]
	public class FieUIMouseEvent : MonoBehaviour
	{
		public enum FieUIMouseOverState
		{
			MOUSE_IDLE,
			MOUSE_OVER
		}

		public delegate void FieUIMouseEventCallback(bool isHit, Vector3 point);

		[SerializeField]
		private Collider _collider;

		private FieUIMouseOverState _mouseOverState;

		public bool _isEnable;

		private float _gamepadDecideDelay;

		public FieUIMouseOverState mouseOverState => _mouseOverState;

		public event FieUIMouseEventCallback mouseOverEvent;

		public event FieUIMouseEventCallback mouseExitEvent;

		public event FieUIMouseEventCallback mouseClickEvent;

		public void ExecuteGamepadDecide()
		{
			if (_isEnable)
			{
				if (this.mouseOverEvent != null)
				{
					this.mouseOverEvent(isHit: true, Vector3.zero);
				}
				_mouseOverState = FieUIMouseOverState.MOUSE_OVER;
				_gamepadDecideDelay = 0.1f;
			}
		}

		public void ExecuteOverEvent()
		{
			if (_isEnable)
			{
				if (this.mouseOverEvent != null)
				{
					this.mouseOverEvent(isHit: true, Vector3.zero);
				}
				_mouseOverState = FieUIMouseOverState.MOUSE_OVER;
			}
		}

		public void ExecuteExitEvent()
		{
			if (_isEnable)
			{
				if (this.mouseExitEvent != null)
				{
					this.mouseExitEvent(isHit: false, Vector3.zero);
				}
				_mouseOverState = FieUIMouseOverState.MOUSE_IDLE;
			}
		}

		public void ExecuteClickEvent()
		{
			if (_isEnable && this.mouseClickEvent != null)
			{
				this.mouseClickEvent(isHit: true, Vector3.zero);
			}
		}

		public void Start()
		{
			if (_collider == null)
			{
				_collider = base.gameObject.GetComponent<Collider>();
			}
		}

		public void Update()
		{
			if (_isEnable && !(FieManagerBehaviour<FieGUIManager>.I.uiCamera == null) && !(_collider == null))
			{
				if (_gamepadDecideDelay > 0f)
				{
					_gamepadDecideDelay -= Time.deltaTime;
					if (_gamepadDecideDelay <= 0f)
					{
						if (this.mouseClickEvent != null)
						{
							this.mouseClickEvent(isHit: true, Vector3.zero);
						}
						_mouseOverState = FieUIMouseOverState.MOUSE_IDLE;
					}
				}
				else
				{
					Ray ray = FieManagerBehaviour<FieGUIManager>.I.uiCamera.camera.ScreenPointToRay(Input.mousePosition);
					ray.origin -= FieManagerBehaviour<FieGUIManager>.I.uiCamera.transform.rotation * Vector3.forward * 10f;
					if (_collider.Raycast(ray, out RaycastHit hitInfo, 100f))
					{
						if (_mouseOverState == FieUIMouseOverState.MOUSE_IDLE && this.mouseOverEvent != null)
						{
							this.mouseOverEvent(isHit: true, hitInfo.point);
						}
						if (Input.GetMouseButtonDown(0) && this.mouseClickEvent != null)
						{
							this.mouseClickEvent(isHit: true, hitInfo.point);
						}
						FieManagerBehaviour<FieInputManager>.I.SetUIControlMode(FieInputManager.FieInputUIControlMode.KEYBOARD);
						_mouseOverState = FieUIMouseOverState.MOUSE_OVER;
					}
					else if (FieManagerBehaviour<FieInputManager>.I.GetUIControlMode() != FieInputManager.FieInputUIControlMode.GAME_PAD)
					{
						if (_mouseOverState == FieUIMouseOverState.MOUSE_OVER && this.mouseOverEvent != null)
						{
							this.mouseExitEvent(isHit: false, hitInfo.point);
						}
						_mouseOverState = FieUIMouseOverState.MOUSE_IDLE;
					}
				}
			}
		}
	}
}
