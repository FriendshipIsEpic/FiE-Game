using UnityEngine;

namespace Fie.UI
{
	public class FieLobbySelectableUICharacterSelectButton : MonoBehaviour
	{
		public delegate void ButtonCallback(FieLobbySelectableUICharacterSelectButton buttonEntity);

		[SerializeField]
		public FieGameCharacter relatedCharacter;

		[SerializeField]
		private FieConstValues.FieGameCharacter _gameCharacterType;

		[SerializeField]
		private Sprite _relatedSprite;

		public FieConstValues.FieGameCharacter gameCharacterType => _gameCharacterType;

		public Sprite relatedSprite => _relatedSprite;

		public event ButtonCallback clickedEvent;

		public void Clicked()
		{
			if (this.clickedEvent != null)
			{
				this.clickedEvent(this);
			}
		}
	}
}
