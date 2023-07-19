using Fie.UI;
using HutongGames.PlayMaker;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieUIOptionWindowForPlayMaker : FsmStateAction
	{
		public enum OptionScreenActionType
		{
			OPEN,
			WAIT_FOR_CLOSE
		}

		[RequiredField]
		public OptionScreenActionType _optionScreenActionType;

		[RequiredField]
		public FieUIOptionWindow _optionScreenComponent;

		[RequiredField]
		public FsmEvent EventToSend;

		public override void OnEnter()
		{
			if (!base.Finished)
			{
				if (_optionScreenComponent == null)
				{
					base.Fsm.Event(EventToSend);
					Finish();
				}
				else
				{
					switch (_optionScreenActionType)
					{
					case OptionScreenActionType.OPEN:
						_optionScreenComponent.gameObject.SetActive(value: true);
						_optionScreenComponent.ShowOptionalScreen();
						base.Fsm.Event(EventToSend);
						Finish();
						break;
					case OptionScreenActionType.WAIT_FOR_CLOSE:
						_optionScreenComponent.gameObject.SetActive(value: true);
						_optionScreenComponent.optionScreenCloseEvent += closedCallback;
						break;
					}
				}
			}
		}

		private void closedCallback()
		{
			base.Fsm.Event(EventToSend);
			Finish();
		}

		public override void OnExit()
		{
			_optionScreenComponent.optionScreenCloseEvent -= closedCallback;
			base.OnExit();
		}
	}
}
