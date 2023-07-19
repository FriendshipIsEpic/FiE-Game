using UnityEngine;

namespace Fie.Ponies.Twilight
{
	public class FieTwilightForceFieldUvOffset : MonoBehaviour
	{
		private float _offsetX;

		private void Update()
		{
			_offsetX += 0.4f * Time.deltaTime;
			GetComponent<Renderer>().material.SetTextureOffset("_Gradient_Edge_Fake", new Vector2(_offsetX, 0.65f));
		}
	}
}
