using UnityEngine;

namespace Fie.Ponies.Twilight
{
	public class FieTwilightBigLaserUvOffset : MonoBehaviour
	{
		private float _offsetX;

		private void Update()
		{
			_offsetX -= 1.5f * Time.deltaTime;
			GetComponent<Renderer>().material.SetVector("_texUV", new Vector4(_offsetX, 0f, 0f, 0f));
			base.transform.Rotate(new Vector3(1f, 0f, 0f));
		}
	}
}
