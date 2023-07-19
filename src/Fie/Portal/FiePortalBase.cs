using Fie.Manager;
using Fie.User;
using Photon;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Portal
{
	public abstract class FiePortalBase : Photon.MonoBehaviour
	{
		public enum PortalTriggerType
		{
			SINGLE,
			COOP
		}

		private const float DEFAULT_TRIGGERING_SEC = 10f;

		[SerializeField]
		private float _triggeringSec = 10f;

		[SerializeField]
		protected PortalTriggerType _portalType;

		protected float _localProgress;

		protected float _globalProgress;

		private int _hittedFlags;

		private int _currentHittedCount;

		protected bool _isTriggerd;

		private bool _hittedMySelf;

		private Dictionary<int, float> _hittedCharacterList = new Dictionary<int, float>();

		public float progress
		{
			get
			{
				return (_portalType != 0) ? _globalProgress : _localProgress;
			}
			set
			{
				if (_portalType == PortalTriggerType.SINGLE)
				{
					_localProgress = value;
				}
				else
				{
					_globalProgress = value;
				}
			}
		}

		public float triggeringSec => _triggeringSec;

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.isWriting)
			{
				stream.SendNext(_globalProgress);
			}
			else
			{
				_globalProgress = (float)stream.ReceiveNext();
			}
		}

		public void Update()
		{
			bool flag = false;
			switch (_portalType)
			{
			case PortalTriggerType.SINGLE:
				if (_hittedMySelf)
				{
					flag = true;
					_hittedMySelf = false;
				}
				break;
			case PortalTriggerType.COOP:
				if (FieManagerBehaviour<FieUserManager>.I.nowPlayerNum > 0)
				{
					int num = 0;
					for (int i = 0; i < FieManagerBehaviour<FieUserManager>.I.nowPlayerNum; i++)
					{
						if ((_hittedFlags & (1 << i)) != 0)
						{
							num++;
						}
					}
					if (num >= FieManagerBehaviour<FieUserManager>.I.nowPlayerNum)
					{
						flag = true;
					}
				}
				break;
			}
			if (flag)
			{
				progress = Mathf.Min(_triggeringSec, progress + Time.deltaTime);
			}
			else if (!_isTriggerd)
			{
				progress = Mathf.Max(0f, progress - Time.deltaTime);
			}
			if (progress >= _triggeringSec && !_isTriggerd)
			{
				if (_portalType == PortalTriggerType.SINGLE)
				{
					Trigger();
				}
				else if (_portalType == PortalTriggerType.COOP && (PhotonNetwork.offlineMode || base.photonView.isMine))
				{
					Trigger();
					base.photonView.RPC("TriggerRPC", PhotonTargets.Others, null);
				}
				_isTriggerd = true;
			}
			_hittedFlags = 0;
		}

		[PunRPC]
		public void TriggerRPC()
		{
			Trigger();
		}

		private void OnTriggerStay(Collider other)
		{
			if (!(other == null) && !(other.gameObject == null))
			{
				FieCollider component = other.gameObject.GetComponent<FieCollider>();
				if (!(component == null))
				{
					FieGameCharacter parentGameCharacter = component.getParentGameCharacter();
					if (!(parentGameCharacter == null))
					{
						int num = 0;
						_hittedMySelf = false;
						FieUser[] allUserData = FieManagerBehaviour<FieUserManager>.I.getAllUserData();
						foreach (FieUser fieUser in allUserData)
						{
							if (fieUser != null && !(fieUser.usersCharacter == null))
							{
								if (PhotonNetwork.offlineMode)
								{
									if (parentGameCharacter.GetInstanceID() == fieUser.usersCharacter.GetInstanceID())
									{
										_hittedFlags |= 1 << num;
									}
								}
								else
								{
									if (parentGameCharacter.photonView.isMine)
									{
										_hittedMySelf = true;
									}
									if (parentGameCharacter.photonView.viewID == fieUser.usersCharacter.photonView.viewID)
									{
										_hittedFlags |= 1 << num;
									}
								}
								num++;
							}
						}
					}
				}
			}
		}

		protected abstract void Trigger();
	}
}
