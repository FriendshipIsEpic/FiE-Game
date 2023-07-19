using UnityEngine;

namespace Fie.UI
{
	public abstract class FieGameUIComponentManagerBase : MonoBehaviour
	{
		private FieGameCharacter _componentManagerOwner;

		public FieGameCharacter componentManagerOwner => _componentManagerOwner;

		public virtual void setComponentManagerOwner(FieGameCharacter owner)
		{
			_componentManagerOwner = owner;
		}

		public virtual void StartUp()
		{
		}
	}
}
