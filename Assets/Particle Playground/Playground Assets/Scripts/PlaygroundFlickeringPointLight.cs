using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]
public class PlaygroundFlickeringPointLight : MonoBehaviour {

	public float flickerSpeed = 10f;
	public float minimumLight = .3f;
	public float maximumLight = 1f;

	private Light _thisLight;
	private float _targetLightValue;

	void Start () 
	{
		_thisLight = GetComponent<Light>();
		SetTargetLightValue();
	}
	
	void Update () 
	{

		if (_thisLight.intensity<_targetLightValue-.1||_thisLight.intensity>_targetLightValue+.1)
			_thisLight.intensity = Mathf.Lerp(_thisLight.intensity, _targetLightValue, flickerSpeed * Time.deltaTime);
		else
			SetTargetLightValue();
	}

	void SetTargetLightValue ()
	{
		_targetLightValue = Random.Range (minimumLight, maximumLight);
	}
}
