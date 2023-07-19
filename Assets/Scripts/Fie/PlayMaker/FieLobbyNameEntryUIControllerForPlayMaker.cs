using Fie.UI;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieLobbyNameEntryUIControllerForPlayMaker : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(FieLobbyNameEntryUIController))]
		public FsmOwnerDefault controllerObject;

		private FieLobbyNameEntryUIController _controllerComponent;

		public override void Reset()
		{
			controllerObject = null;
		}

		public override void OnEnter()
		{
			if (!base.Finished && controllerObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(controllerObject);
				if (!(ownerDefaultTarget == null))
				{
					_controllerComponent = ownerDefaultTarget.GetComponent<FieLobbyNameEntryUIController>();
					if (_controllerComponent != null)
					{
						_controllerComponent.Decide();
					}
					Finish();
				}
			}
		}
	}
}
