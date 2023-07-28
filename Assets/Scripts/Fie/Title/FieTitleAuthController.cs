using Fie.Manager;
//using MBS;
using UnityEngine;

namespace Fie.Title
{
	public class FieTitleAuthController : MonoBehaviour
	{
		public delegate void FinishedLoginProcessCallback();

		public enum RecomendedLoginState
		{
			AUTH,
			CONNECT_TO_MASTER,
			UNNECESSARY
		}

		//[SerializeField]
		//private WUUGLoginGUI _loginGUIComponent;

		public event FinishedLoginProcessCallback finishedEvent;

		public RecomendedLoginState GetRecomendedLoginState()
		{
			if (PhotonNetwork.offlineMode)
			{
				return RecomendedLoginState.UNNECESSARY;
			}
			// if (WULogin.logged_in)
			// {
			// 	if (!PhotonNetwork.connected)
			// 	{
			// 		return RecomendedLoginState.CONNECT_TO_MASTER;
			// 	}
			// 	return RecomendedLoginState.UNNECESSARY;
			// }
			return RecomendedLoginState.AUTH;
		}

		public void SetupLoginComponents(RecomendedLoginState recomendedState)
		{
			switch (recomendedState)
			{
			case RecomendedLoginState.AUTH:
				// _loginGUIComponent.gameObject.SetActive(value: true);
				// _loginGUIComponent.InitWULoginGUI();
				// _loginGUIComponent.loginEvent += _loginGUIComponent_loginCallback;
				break;
			case RecomendedLoginState.CONNECT_TO_MASTER:
				FieManagerBehaviour<FieNetworkManager>.I.connectedToMasterServerEvent += _connectedToMasterServerCallback;
				FieManagerBehaviour<FieNetworkManager>.I.feiledToConnectToMasterServerEvent += _connectedToMasterServerCallback;
				FieManagerBehaviour<FieNetworkManager>.I.ConnectToMasterServer();
				break;
			case RecomendedLoginState.UNNECESSARY:
				if (this.finishedEvent != null)
				{
					this.finishedEvent();
				}
				break;
			}
		}

		private void _connectedToMasterServerCallback(FieNetworkManager.FieNetowrkErrorCode errorCode)
		{
			if (this.finishedEvent != null)
			{
				this.finishedEvent();
			}
		}

		private void _loginGUIComponent_loginCallback()
		{
			if (this.finishedEvent != null)
			{
				this.finishedEvent();
			}
		}
	}
}
