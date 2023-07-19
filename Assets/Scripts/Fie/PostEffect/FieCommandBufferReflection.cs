using UnityEngine;
using UnityEngine.Rendering;

namespace Fie.PostEffect
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class FieCommandBufferReflection : MonoBehaviour
	{
		[Range(0f, 10f)]
		public float m_BlurScale = 1f;

		public Shader m_BlurShader;

		private UnityEngine.Camera m_Cam;

		private Material m_Material;

		private CommandBuffer m_CommandBuffer;

		private void Awake()
		{
			m_Cam = GetComponent<UnityEngine.Camera>();
			if (m_BlurShader != null)
			{
				m_Material = new Material(m_BlurShader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
		}

		private void OnEnable()
		{
			int qualityLevel = QualitySettings.GetQualityLevel();
			if (qualityLevel > 1 && !(m_Cam == null) && m_CommandBuffer == null)
			{
				m_CommandBuffer = new CommandBuffer();
				m_CommandBuffer.name = "Grab screen and blur";
				int nameID = Shader.PropertyToID("_ScreenCopyTexture");
				m_CommandBuffer.GetTemporaryRT(nameID, -1, -1, 0, FilterMode.Bilinear);
				m_CommandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, nameID);
				m_CommandBuffer.SetGlobalTexture("_GrabRawTexture", nameID);
				int nameID2 = Shader.PropertyToID("_Temp1");
				int nameID3 = Shader.PropertyToID("_Temp2");
				m_CommandBuffer.GetTemporaryRT(nameID2, -2, -2, 0, FilterMode.Bilinear);
				m_CommandBuffer.GetTemporaryRT(nameID3, -2, -2, 0, FilterMode.Bilinear);
				m_CommandBuffer.Blit(nameID, nameID2);
				m_CommandBuffer.ReleaseTemporaryRT(nameID);
				m_CommandBuffer.SetGlobalVector("offsets", new Vector4(1f * m_BlurScale / (float)Screen.width, 0f, 0f, 0f));
				m_CommandBuffer.Blit(nameID2, nameID3, m_Material);
				m_CommandBuffer.SetGlobalVector("offsets", new Vector4(0f, 2f * m_BlurScale / (float)Screen.height, 0f, 0f));
				m_CommandBuffer.Blit(nameID3, nameID2, m_Material);
				m_CommandBuffer.SetGlobalVector("offsets", new Vector4(4f * m_BlurScale / (float)Screen.width, 0f, 0f, 0f));
				m_CommandBuffer.Blit(nameID2, nameID3, m_Material);
				m_CommandBuffer.SetGlobalVector("offsets", new Vector4(0f, 4f * m_BlurScale / (float)Screen.height, 0f, 0f));
				m_CommandBuffer.Blit(nameID3, nameID2, m_Material);
				m_CommandBuffer.SetGlobalTexture("_GrabBlurTexture", nameID2);
				m_Cam.AddCommandBuffer(CameraEvent.AfterEverything, m_CommandBuffer);
			}
		}

		private void OnDisable()
		{
			if (!(m_Cam == null) && m_CommandBuffer != null)
			{
				m_Cam.RemoveCommandBuffer(CameraEvent.AfterEverything, m_CommandBuffer);
				m_CommandBuffer = null;
			}
		}
	}
}
