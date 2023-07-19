using HutongGames.PlayMaker;
using UnityEngine.SceneManagement;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieResultActionForPlaymaker : FsmStateAction
	{
		public override void OnEnter()
		{
			SceneManager.LoadScene(0, LoadSceneMode.Single);
			Finish();
		}
	}
}
