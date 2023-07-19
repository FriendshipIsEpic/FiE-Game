using Fie.UI;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieLobbyCharacterSelectUIControllerForPlayMaker : FsmStateAction
	{
		public enum FieLobbyElementSelectUIActionType
		{
			ENABLE,
			DISABLE,
			INCREASE,
			DECREASE,
			DECIDE
		}

		public FieLobbyElementSelectUIActionType eventType;

		[RequiredField]
		[CheckForComponent(typeof(FieLobbyCharacterSelectUIController))]
		public FsmOwnerDefault controllerObject;

		private FieLobbyCharacterSelectUIController _controllerComponent;

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
					_controllerComponent = ownerDefaultTarget.GetComponent<FieLobbyCharacterSelectUIController>();
					if (_controllerComponent != null)
					{
						switch (eventType)
						{
						case FieLobbyElementSelectUIActionType.ENABLE:
							_controllerComponent.isEnable = true;
							break;
						case FieLobbyElementSelectUIActionType.DISABLE:
							_controllerComponent.isEnable = false;
							break;
						case FieLobbyElementSelectUIActionType.INCREASE:
							_controllerComponent.IncreaseElementIndex();
							break;
						case FieLobbyElementSelectUIActionType.DECREASE:
							_controllerComponent.DecreaseElementIndex();
							break;
						case FieLobbyElementSelectUIActionType.DECIDE:
							_controllerComponent.Decide();
							break;
						}
					}
					Finish();
				}
			}
		}
	}
}
