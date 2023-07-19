using UnityEngine;

public class FiePhysicalForceDynamic : MonoBehaviour
{
	public float coefficient;

	private Vector3 _nowNormalVec = Vector3.zero;

	private float _nowForce;

	public void Awake()
	{
	}

	private void OnTriggerStay(Collider col)
	{
		if (!(col.GetComponent<Rigidbody>() == null))
		{
			col.GetComponent<Rigidbody>().AddForce(coefficient * _nowNormalVec * _nowForce);
		}
	}

	public void SetPhysicalForce(Vector3 normalVec, float force)
	{
		_nowNormalVec = normalVec;
		_nowForce = force;
	}
}
