using AmplifyBloom;
using Fie.Manager;
using Fie.PostEffect;
using System;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;

namespace Fie.Camera
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	public abstract class FieCameraBase : MonoBehaviour
	{
		public PostProcessingProfile PostProcessingProfileForLev2;

		public PostProcessingProfile PostProcessingProfileForLev3;

		public PostProcessingProfile PostProcessingProfileForLev4;

		public PostProcessingProfile PostProcessingProfileForLev5;

		private FieGameCharacter _cameraOwner;

		private UnityEngine.Camera _camera;

		public UnityEngine.Camera camera => _camera;

		public FieGameCharacter cameraOwner
		{
			get
			{
				if (_cameraOwner == null)
				{
					_cameraOwner = FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter;
				}
				return _cameraOwner;
			}
		}

		protected virtual void Awake()
		{
			_camera = GetComponent<UnityEngine.Camera>();
			if (_camera == null)
			{
				throw new Exception("this component require UnityEngine.Camera. but didn't.");
			}
		}

		protected virtual void Start()
		{
			CalibratePostEffects();
		}

		public void CalibratePostEffects()
		{
			int qualityLevel = QualitySettings.GetQualityLevel();
			PostProcessingBehaviour component = GetComponent<PostProcessingBehaviour>();
			AmplifyBloomEffect component2 = GetComponent<AmplifyBloomEffect>();
			UltimateBloom component3 = GetComponent<UltimateBloom>();
			SSREffect component4 = GetComponent<SSREffect>();
			PKFxRenderingPlugin component5 = GetComponent<PKFxRenderingPlugin>();
			SunShafts component6 = GetComponent<SunShafts>();
			SEGI component7 = GetComponent<SEGI>();
			VolumetricFog component8 = GetComponent<VolumetricFog>();
			FieCommandBufferReflection component9 = GetComponent<FieCommandBufferReflection>();
			switch (qualityLevel)
			{
			default:
				if (component != null)
				{
					component.enabled = false;
				}
				if (component2 != null)
				{
					component2.enabled = false;
				}
				if (component3 != null)
				{
					component3.enabled = false;
				}
				if (component4 != null)
				{
					component4.enabled = false;
				}
				if (component5 != null)
				{
					component5.m_EnableDistortion = false;
				}
				if (component5 != null)
				{
					component5.m_EnableSoftParticles = false;
				}
				if (component6 != null)
				{
					component6.enabled = false;
				}
				if (component7 != null)
				{
					component7.enabled = false;
				}
				if (component8 != null)
				{
					component8.enabled = false;
				}
				if (component9 != null)
				{
					component9.enabled = false;
				}
				Shader.globalMaximumLOD = 800;
				break;
			case 1:
				if (component2 != null)
				{
					component2.enabled = false;
				}
				if (component3 != null)
				{
					component3.enabled = false;
				}
				if (component4 != null)
				{
					component4.enabled = false;
				}
				if (component5 != null)
				{
					component5.m_EnableDistortion = false;
				}
				if (component5 != null)
				{
					component5.m_EnableSoftParticles = false;
				}
				if (component6 != null)
				{
					component6.enabled = true;
				}
				if (component7 != null)
				{
					component7.enabled = false;
				}
				if (component8 != null)
				{
					component8.enabled = false;
				}
				if (component9 != null)
				{
					component9.enabled = false;
				}
				if (component != null)
				{
					component.profile = PostProcessingProfileForLev2;
				}
				Shader.globalMaximumLOD = 800;
				break;
			case 2:
				if (component2 != null)
				{
					component2.enabled = true;
				}
				if (component3 != null)
				{
					component3.enabled = true;
				}
				if (component4 != null)
				{
					component4.enabled = false;
				}
				if (component5 != null)
				{
					component5.m_EnableDistortion = true;
				}
				if (component5 != null)
				{
					component5.m_EnableSoftParticles = true;
				}
				if (component6 != null)
				{
					component6.enabled = true;
				}
				if (component7 != null)
				{
					component7.enabled = false;
				}
				if (component != null)
				{
					component.profile = PostProcessingProfileForLev3;
				}
				if (component8 != null)
				{
					component8.enabled = false;
				}
				if (component9 != null)
				{
					component9.enabled = true;
				}
				if (component2 != null)
				{
					component2.BloomDownsampleCount = 3;
				}
				Shader.globalMaximumLOD = 1500;
				break;
			case 3:
				if (component2 != null)
				{
					component2.enabled = true;
				}
				if (component3 != null)
				{
					component3.enabled = true;
				}
				if (component4 != null)
				{
					component4.enabled = true;
				}
				if (component5 != null)
				{
					component5.m_EnableDistortion = true;
				}
				if (component5 != null)
				{
					component5.m_EnableSoftParticles = true;
				}
				if (component6 != null)
				{
					component6.enabled = true;
				}
				if (component7 != null)
				{
					component7.enabled = false;
				}
				if (component != null)
				{
					component.profile = PostProcessingProfileForLev4;
				}
				if (component8 != null)
				{
					component8.enabled = true;
				}
				if (component9 != null)
				{
					component9.enabled = true;
				}
				if (component2 != null)
				{
					component2.BloomDownsampleCount = 5;
				}
				Shader.globalMaximumLOD = 1500;
				break;
			case 4:
				if (component2 != null)
				{
					component2.enabled = true;
				}
				if (component3 != null)
				{
					component3.enabled = true;
				}
				if (component4 != null)
				{
					component4.enabled = true;
				}
				if (component5 != null)
				{
					component5.m_EnableDistortion = true;
				}
				if (component5 != null)
				{
					component5.m_EnableSoftParticles = true;
				}
				if (component6 != null)
				{
					component6.enabled = true;
				}
				if (component7 != null)
				{
					component7.enabled = true;
				}
				if (component != null)
				{
					component.profile = PostProcessingProfileForLev5;
				}
				if (component8 != null)
				{
					component8.enabled = true;
				}
				if (component9 != null)
				{
					component9.enabled = true;
				}
				if (component2 != null)
				{
					component2.BloomDownsampleCount = 5;
				}
				Shader.globalMaximumLOD = 1500;
				break;
			}
		}

		public virtual void setCameraOwner(FieGameCharacter owner)
		{
			if (!(owner == null))
			{
				_cameraOwner = owner;
			}
		}

		public bool isCameraOwner(FieGameCharacter character)
		{
			return character.GetInstanceID() == _cameraOwner.GetInstanceID();
		}
	}
}
