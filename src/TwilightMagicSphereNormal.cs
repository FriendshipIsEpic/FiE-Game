using UnityEngine;

public class TwilightMagicSphereNormal : MonoBehaviour
{
	private float angle;

	private void Start()
	{
	}

	private void Update()
	{
		angle += 0.9f * Time.deltaTime;
		angle = Mathf.Repeat(angle, 1f);
		base.gameObject.GetComponent<Renderer>().material.SetTextureOffset("_BumpMap", new Vector2(0f - angle, 1f));
	}
}
