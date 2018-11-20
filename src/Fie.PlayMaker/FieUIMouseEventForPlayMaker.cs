using Fie.UI;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieUIMouseEventForPlayMaker : FsmStateAction
	{
		public enum FieUIEventTypeForPlayMaker
		{
			OVER,
			EXIT,
			CLICK,
			SHOW,
			HIDE
		}

		public FieUIEventTypeForPlayMaker eventType;

		[RequiredField]
		[CheckForComponent(typeof(FieUIMouseEvent))]
		public FsmOwnerDefault mouseEventObject;

		[RequiredField]
		public FsmEvent eventToSend;

		private FieUIMouseEvent _mouseEventComponent;

		public override void Reset()
		{
			mouseEventObject = null;
			eventToSend = null;
		}

		public override void Awake()
		{
			if (mouseEventObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(mouseEventObject);
				if (!(ownerDefaultTarget == null))
				{
					_mouseEventComponent = ownerDefaultTarget.GetComponent<FieUIMouseEvent>();
					if (_mouseEventComponent != null)
					{
						_mouseEventComponent.mouseOverEvent -= MouseEventCallback;
						_mouseEventComponent.mouseExitEvent -= MouseEventCallback;
						_mouseEventComponent.mouseClickEvent -= MouseEventCallback;
						switch (eventType)
						{
						case FieUIEventTypeForPlayMaker.OVER:
							_mouseEventComponent.mouseOverEvent += MouseEventCallback;
							break;
						case FieUIEventTypeForPlayMaker.EXIT:
							_mouseEventComponent.mouseExitEvent += MouseEventCallback;
							break;
						case FieUIEventTypeForPlayMaker.CLICK:
							_mouseEventComponent.mouseClickEvent += MouseEventCallback;
							break;
						}
					}
				}
			}
		}

		public override void OnEnter()
		{
			if (eventType == FieUIEventTypeForPlayMaker.SHOW || eventType == FieUIEventTypeForPlayMaker.HIDE)
			{
				_mouseEventComponent._isEnable = (eventType == FieUIEventTypeForPlayMaker.SHOW);
				base.Fsm.Event(eventToSend);
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!(_mouseEventComponent == null) && _mouseEventComponent._isEnable)
			{
				if (_mouseEventComponent.mouseOverState == FieUIMouseEvent.FieUIMouseOverState.MOUSE_OVER && eventType == FieUIEventTypeForPlayMaker.OVER)
				{
					base.Fsm.Event(eventToSend);
					Finish();
				}
				if (_mouseEventComponent.mouseOverState == FieUIMouseEvent.FieUIMouseOverState.MOUSE_IDLE && eventType == FieUIEventTypeForPlayMaker.EXIT)
				{
					base.Fsm.Event(eventToSend);
					Finish();
				}
			}
		}

		private void MouseEventCallback(bool isHit, Vector3 point)
		{
			base.Fsm.Event(eventToSend);
			Finish();
		}
	}
}
