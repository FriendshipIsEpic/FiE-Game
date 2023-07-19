using Fie.UI;
using HutongGames.PlayMaker;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieLobbySelectableUIForPlaymaker : FsmStateAction
	{
		public bool waitingMode;

		public FsmEvent transitionEvent;

		[RequiredField]
		public FieLobbySelectableUIController controller;

		[RequiredField]
		public FieLobbySelectableUIController.SelectableWindowState _targetState;

		public override void OnEnter()
		{
			if (!waitingMode)
			{
				controller.ChangeState(_targetState);
				Finish();
			}
		}

		public override void OnUpdate()
		{
			if (waitingMode)
			{
				if (controller.currentState == _targetState)
				{
					base.Fsm.Event(transitionEvent);
				}
				Finish();
			}
		}
	}
}
