using Fie.Title;
using HutongGames.PlayMaker;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieTitleLoginActionForPlayMaker : FsmStateAction
	{
		[RequiredField]
		public FieTitleAuthController _titleAuthActionComponent;

		[RequiredField]
		public FsmEvent EventToSend;

		public override void OnEnter()
		{
			if (!base.Finished)
			{
				if (_titleAuthActionComponent != null)
				{
					_titleAuthActionComponent.finishedEvent += loginCallback;
				}
				FieTitleAuthController.RecomendedLoginState recomendedLoginState = _titleAuthActionComponent.GetRecomendedLoginState();
				_titleAuthActionComponent.SetupLoginComponents(recomendedLoginState);
			}
		}

		private void loginCallback()
		{
			base.Fsm.Event(EventToSend);
			Finish();
		}

		public override void OnExit()
		{
			_titleAuthActionComponent.finishedEvent -= loginCallback;
			base.OnExit();
		}
	}
}
