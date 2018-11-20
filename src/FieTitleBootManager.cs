using Fie.Manager;

public class FieTitleBootManager : FieManagerBehaviour<FieTitleBootManager>
{
	private bool _isInitialized;

	private void Start()
	{
		FieManagerBehaviour<FieTitleBootManager>.I.StartUp();
		if (!_isInitialized)
		{
			FieManagerBehaviour<FieGUIManager>.I.StartUp();
			_isInitialized = true;
		}
	}
}
