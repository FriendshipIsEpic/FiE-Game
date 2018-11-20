using UnityEngine;

public class FieBrobShadow : MonoBehaviour
{
	private float _initHight;

	private void Start()
	{
		Vector3 position = base.transform.parent.position;
		float y = position.y;
		Vector3 position2 = base.transform.position;
		_initHight = y - position2.y;
	}

	private void LateUpdate()
	{
		base.transform.position = base.transform.parent.position + Vector3.down * _initHight;
		base.transform.rotation = Quaternion.LookRotation(Vector3.up, base.transform.parent.forward);
	}
}
