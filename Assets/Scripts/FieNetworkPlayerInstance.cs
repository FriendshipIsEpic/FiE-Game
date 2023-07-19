using UnityEngine.Networking;

public class FieNetworkPlayerInstance : NetworkBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result = default(bool);
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}
}
