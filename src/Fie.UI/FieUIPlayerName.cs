using Fie.Manager;
using Fie.User;
using TMPro;
using UnityEngine;

namespace Fie.UI
{
	public class FieUIPlayerName : MonoBehaviour
	{
		[SerializeField]
		private FieGameCharacter _gameCharacter;

		[SerializeField]
		private TextMeshPro _textMesh;

		private string _currentPlayerName = string.Empty;

		private void Update()
		{
			if (_currentPlayerName == string.Empty)
			{
				SetPlayerName();
			}
			base.transform.rotation = Quaternion.identity;
		}

		private void SetPlayerName()
		{
			if (!(_gameCharacter == null) && !(_gameCharacter.photonView == null) && _gameCharacter.photonView.owner != null)
			{
				FieUser userData = FieManagerBehaviour<FieUserManager>.I.GetUserData(_gameCharacter.photonView.owner);
				if (userData != null && !(userData.userName == string.Empty))
				{
					_currentPlayerName = userData.userName;
					_textMesh.text = _currentPlayerName;
				}
			}
		}
	}
}
