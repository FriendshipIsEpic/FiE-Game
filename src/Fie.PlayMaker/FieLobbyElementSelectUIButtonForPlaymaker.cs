using Fie.UI;
using HutongGames.PlayMaker;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieLobbyElementSelectUIButtonForPlaymaker : FsmStateAction
	{
		[RequiredField]
		public FieLobbyCharacterSelectUIController controller;

		[RequiredField]
		public FsmEvent SucseedEvent;

		[RequiredField]
		public FsmEvent FeildEvent;

		public override void OnEnter()
		{
			if (controller.isDecidable())
			{
				base.Fsm.Event(SucseedEvent);
			}
			else
			{
				base.Fsm.Event(FeildEvent);
			}
			Finish();
		}
	}
}
