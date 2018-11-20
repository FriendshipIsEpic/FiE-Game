using Fie.Utility;
using System.Collections;
using UnityEngine;

namespace Fie.Camera
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class FieSkillTreeCamera : MonoBehaviour
	{
		private Matrix4x4 ortho;

		private Matrix4x4 perspective;

		public float fov = 60f;

		public float near = 0.3f;

		public float far = 1000f;

		public float orthographicSize = 50f;

		private float aspect;

		private bool orthoOn;

		private Tweener<TweenTypesInOutSine> _transitionTweener = new Tweener<TweenTypesInOutSine>();

		private UnityEngine.Camera _camera;

		public UnityEngine.Camera camera => _camera;

		public void Awake()
		{
			_camera = base.gameObject.GetComponent<UnityEngine.Camera>();
			aspect = (float)Screen.width / (float)Screen.height;
			perspective = _camera.projectionMatrix;
			ortho = Matrix4x4.Ortho((0f - orthographicSize) * aspect, orthographicSize * aspect, 0f - orthographicSize, orthographicSize, near, far);
			orthoOn = false;
		}

		public void SetViewMode(bool isOrtho)
		{
			if (isOrtho)
			{
				BlendToMatrix(ortho, 0.5f);
			}
			else
			{
				BlendToMatrix(perspective, 1f);
			}
		}

		public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
		{
			Matrix4x4 result = default(Matrix4x4);
			for (int i = 0; i < 16; i++)
			{
				result[i] = Mathf.Lerp(from[i], to[i], time);
			}
			return result;
		}

		private IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float duration)
		{
			_transitionTweener.InitTweener(duration, 0f, 1f);
			if (!_transitionTweener.IsEnd())
			{
				_camera.projectionMatrix = MatrixLerp(src, dest, _transitionTweener.UpdateParameterFloat(Time.deltaTime));
				yield return (object)1;
				/*Error: Unable to find new state assignment for yield return*/;
			}
			_camera.projectionMatrix = dest;
		}

		public Coroutine BlendToMatrix(Matrix4x4 targetMatrix, float duration)
		{
			StopAllCoroutines();
			return StartCoroutine(LerpFromTo(_camera.projectionMatrix, targetMatrix, duration));
		}
	}
}
