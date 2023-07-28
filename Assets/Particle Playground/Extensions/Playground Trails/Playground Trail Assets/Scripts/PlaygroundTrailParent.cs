using UnityEngine;
using System.Collections;
using ParticlePlayground;

[ExecuteInEditMode()]
public class PlaygroundTrailParent : MonoBehaviour {

	public PlaygroundTrails trailsReference;

	private GameObject _gameObject;

	void Awake () {
		_gameObject = gameObject;
	}
	
	void Update () {
		if (_gameObject != trailsReference.GetParentGameObject())
		{
			if (Application.isPlaying)
				Destroy (_gameObject);
			else
				DestroyImmediate(_gameObject);
		}
	}
}
