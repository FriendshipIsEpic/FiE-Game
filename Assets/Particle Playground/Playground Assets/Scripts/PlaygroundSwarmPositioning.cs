using UnityEngine;
using System.Collections;

public class PlaygroundSwarmPositioning : MonoBehaviour {

	public float swarmStrength = .1f;
	public float swarmSpeed = 6f;
	public Transform swarmTransform;

	private Vector3 _velocity;
	private Transform _thisTransform;

	void Start () {
		_thisTransform = transform;
	}
	
	void Update () 
	{
		float t = Time.time * swarmSpeed;
		Vector3 tPos = _thisTransform.position;
		
		_velocity.x += Mathf.PerlinNoise(t, tPos.z)-.5f;
		_velocity.y += Mathf.PerlinNoise(t+1f, tPos.x)-.5f;
		_velocity.z += Mathf.PerlinNoise(t+2f, tPos.y)-.5f;
		
		_velocity = Vector3.Lerp (_velocity, Vector3.zero, Time.deltaTime * swarmSpeed);
		_thisTransform.position = Vector3.Lerp (tPos, swarmTransform.position + (_velocity*swarmStrength), Time.deltaTime * swarmSpeed);
	}
}
