using Fie.Manager;
using TMPro;
using UnityEngine;

namespace Fie.Title
{
	public class FieTitleVersionNumberDrawer : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _textField;

		private void Start()
		{
			if (!(_textField == null))
			{
				_textField.text = FieManagerBehaviour<FieEnvironmentManager>.I.getVersionString();
			}
		}
	}
}
