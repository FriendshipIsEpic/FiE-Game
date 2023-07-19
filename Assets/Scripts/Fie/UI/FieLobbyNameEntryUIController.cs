using Fie.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieLobbyNameEntryUIController : MonoBehaviour
	{
		[SerializeField]
		private Text _nameEntryField;

		public void Decide()
		{
			string myName = string.Empty;
			if (_nameEntryField != null && _nameEntryField.text != string.Empty)
			{
				myName = _nameEntryField.text;
			}
			FieManagerBehaviour<FieUserManager>.I.SetMyName(myName);
		}
	}
}
