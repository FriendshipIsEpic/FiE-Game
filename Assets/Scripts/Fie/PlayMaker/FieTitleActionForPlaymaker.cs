using Fie.Manager;
using Fie.Scene;
using HutongGames.PlayMaker;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieTitleActionForPlaymaker : FsmStateAction
	{
		public float fadeTime;

		[RequiredField]
		public FsmEvent EventToSend;

		private FieSceneManager.FieLoadingSceneJob _loadSceneJob;

		public override void OnEnter()
		{
			_loadSceneJob = FieManagerBehaviour<FieSceneManager>.I.LoadScene(new FieSceneLobby(), allowSceneActivation: false, FieFaderManager.FadeType.OUT_TO_WHITE, FieFaderManager.FadeType.IN_FROM_WHITE, fadeTime);
		}

		public override void OnUpdate()
		{
			if (_loadSceneJob.isDoneLoading)
			{
				_loadSceneJob.allowSceneActivation = true;
				base.Fsm.Event(EventToSend);
				Finish();
			}
		}
	}
}
