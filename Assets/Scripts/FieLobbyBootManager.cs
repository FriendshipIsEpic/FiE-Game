using Fie.Manager;

public class FieLobbyBootManager : FieManagerBehaviour<FieLobbyBootManager>
{
	private bool _isInitialized;

	private void Start()
	{
		FieManagerBehaviour<FieLobbyBootManager>.I.StartUp();
		if (!_isInitialized)
		{
			FieManagerBehaviour<FieGameCameraManager>.I.gameObject.SetActive(value: true);
			FieManagerBehaviour<FieGameCharacterManager>.I.gameObject.SetActive(value: true);
			FieManagerBehaviour<FieGameCameraManager>.I.StartUp();
			FieManagerBehaviour<FieLobbyGameCharacterGenerateManager>.I.StartUp();
			FieManagerBehaviour<FieGUIManager>.I.StartUp();
			_isInitialized = true;
		}
	}
}
