using UnityEngine;

namespace Fie.Object
{
	public class FieInputControllerBase : MonoBehaviour
	{
		public FieGameCharacter _ownerCharacter;

		public FieGameCharacter ownerCharacter => _ownerCharacter;

		public void SetOwner(FieGameCharacter character)
		{
			_ownerCharacter = character;
		}
	}
}
