using Colorful;
using Fie.LevelObject;
using UnityEngine;

namespace Fie.Camera
{
	public class FieGameCameraEverfreeVignette : MonoBehaviour
	{
		[SerializeField]
		private float maximumVignetteDistance = 20f;

		[SerializeField]
		private float minimumVignetteDistance = 10f;

		private FastVignette _vignette;

		private FieGameCamera _gameCmaera;

		private FieLevelObjectGlowInsect _insect;

		private void Start()
		{
			_vignette = base.gameObject.GetComponent<FastVignette>();
			_gameCmaera = base.gameObject.GetComponent<FieGameCamera>();
			_insect = UnityEngine.Object.FindObjectOfType<FieLevelObjectGlowInsect>();
		}

		private void Update()
		{
			if (!(_vignette == null) && !(_gameCmaera == null) && !(_gameCmaera.cameraOwner == null))
			{
				Vector3 vector = _gameCmaera.camera.WorldToScreenPoint(_gameCmaera.cameraOwner.centerTransform.position);
				_vignette.Center = new Vector2(vector.x / (float)Screen.width, vector.y / (float)Screen.height);
				_vignette.Darkness = 100f * Mathf.Clamp((Vector3.Distance(_gameCmaera.cameraOwner.centerTransform.position, _insect.transform.position) - minimumVignetteDistance) / maximumVignetteDistance, 0f, 1f);
			}
		}
	}
}
