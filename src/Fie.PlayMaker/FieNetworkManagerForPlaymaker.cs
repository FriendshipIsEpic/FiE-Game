using Fie.Manager;
using HutongGames.PlayMaker;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieNetworkManagerForPlaymaker : FsmStateAction
	{
		public enum FieNetworkActionType
		{
			CREATE_GLOBAL_MATCH,
			CREATE_LOCAL_MATCH,
			JOIN_MATCH,
			DISCONNECT
		}

		[RequiredField]
		public FsmEvent SucseedEvent;

		[RequiredField]
		public FsmEvent FeildEvent;

		public FieNetworkActionType actionType;

		public override void OnEnter()
		{
			switch (actionType)
			{
			case FieNetworkActionType.CREATE_GLOBAL_MATCH:
				if (!PhotonNetwork.offlineMode)
				{
					FieManagerBehaviour<FieNetworkManager>.I.CreateNetowkRoom(delegate(FieNetworkManager.FieNetowrkErrorCode errorCode)
					{
						ClacNetworkResponse(errorCode == FieNetworkManager.FieNetowrkErrorCode.SUCCEED);
					}, string.Empty);
				}
				else
				{
					PhotonPlayer player = PhotonNetwork.player;
					player.name = "Player 1";
					FieManagerBehaviour<FieUserManager>.I.RegistUser(player);
					ClacNetworkResponse();
				}
				break;
			case FieNetworkActionType.JOIN_MATCH:
				if (PhotonNetwork.offlineMode)
				{
					ClacNetworkResponse(response: false);
				}
				else
				{
					FieManagerBehaviour<FieNetworkManager>.I.JoinNetowkRoom(delegate(FieNetworkManager.FieNetowrkErrorCode errorCode)
					{
						ClacNetworkResponse(errorCode == FieNetworkManager.FieNetowrkErrorCode.SUCCEED);
					}, null);
				}
				break;
			}
		}

		private void ClacNetworkResponse(bool response = true)
		{
			if (response)
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
