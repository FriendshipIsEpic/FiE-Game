using Fie.Utility;
using System.Collections;
using UnityEngine;

public class FieOnetimeLight : MonoBehaviour
{
	public float startIntensity = 1f;

	public float endIntensity;

	public float startRange = 5f;

	public float endRange = 10f;

	public float duration = 0.5f;

	public bool isEnd;

	private Coroutine calcLightCoroutine;

	[SerializeField]
	private Light _light;

	private Tweener<TweenTypesOutSine> _intensityTweener = new Tweener<TweenTypesOutSine>();

	private Tweener<TweenTypesOutSine> _rangeTweener = new Tweener<TweenTypesOutSine>();

	public void OnEnable()
	{
		_intensityTweener.InitTweener(duration, startIntensity, endIntensity);
		_rangeTweener.InitTweener(duration, startRange, endRange);
		calcLightCoroutine = StartCoroutine(calcLight());
		isEnd = false;
	}

	private IEnumerator calcLight()
	{
		if (_intensityTweener.IsEnd() && _rangeTweener.IsEnd())
		{
			yield return (object)null;
			/*Error: Unable to find new state assignment for yield return*/;
		}
		_light.intensity = _intensityTweener.UpdateParameterFloat(Time.deltaTime);
		_light.range = _rangeTweener.UpdateParameterFloat(Time.deltaTime);
		yield return (object)null;
		/*Error: Unable to find new state assignment for yield return*/;
	}

	public void Kill()
	{
		if (calcLightCoroutine != null)
		{
			StopCoroutine(calcLightCoroutine);
		}
		_light.intensity = 0f;
		_light.range = 0f;
		isEnd = true;
	}
}
