using CinemaDirector;
using Colorful;
using Fie.Manager;
using Fie.Utility;
using UnityEngine;

namespace Fie.Camera
{
	public class FieGameCamera : FieCameraBase
	{
		private const float CALC_MAXHP_RATE = 0.6f;

		private const float MAX_GRAYSCALE_AMOUNT = 0.4f;

		private const float MAX_VIGNETTE = 20f;

		private const float MAX_CHROMATIC = -20f;

		private const float MAX_DRY_LEVEL = -1000f;

		private const float MAX_DECAY_TIME = 2f;

		private const float MAX_DECAY_RATIO = 1f;

		private const float MAX_ROOM_HF = -1000f;

		private const float MAX_ROOM_LF = -500f;

		private const float MAX_REVERB_LEVEL = 100f;

		private const float MAX_REVERB_DELAY = 0.1f;

		private float _maxHp;

		private float _currentHp;

		private float _currentAnimatinoHp;

		private Tweener<TweenTypesInOutSine> _transitionAlphaTweener = new Tweener<TweenTypesInOutSine>();

		private Tweener<TweenTypesOutSine> _damageTweener = new Tweener<TweenTypesOutSine>();

		public Quaternion nowCameraTargetRotation = Quaternion.identity;

		public Quaternion lastCameraTargetRotation = Quaternion.identity;

		public Quaternion beforeTaskChangedTargetRotation = Quaternion.identity;

		public Vector3 nowCameraTargetPos = Vector3.zero;

		public Vector3 lastCameraTargetPos = Vector3.zero;

		public Vector3 beforeTaskChangedTargetPos = Vector3.zero;

		public Vector3 tagetMakerScreenPos = Vector3.zero;

		private float _initCameraFov;

		private float _lastCameraFov;

		private float _transitionAlpha = 1f;

		private Vector3 _wiggleRange = Vector3.zero;

		private Wiggler _cameraWiggler;

		private FieCameraOffset _cameraOffset;

		private FieCameraOffset.FieCameraOffsetParam currentOffsetParam = default(FieCameraOffset.FieCameraOffsetParam);

		private FieGameCameraTaskBase nowTaskObject;

		[SerializeField]
		private Grayscale _colorCorrectionEffect;

		[SerializeField]
		private ContrastVignette _vignetteEffect;

		[SerializeField]
		private AudioReverbFilter _reverbFilter;

		[SerializeField]
		private bool _isEnableDOF = true;

		public Cutscene _mainCameraCutScene;

		public float transitionAlpha
		{
			get
			{
				if (_transitionAlphaTweener == null || _transitionAlphaTweener.IsEnd())
				{
					return 1f;
				}
				return _transitionAlpha;
			}
			private set
			{
				_transitionAlpha = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			base.camera.transparencySortMode = TransparencySortMode.Orthographic;
			_cameraWiggler = new Wiggler(Vector3.zero, 0f, 1, Vector3.zero);
			_transitionAlphaTweener.InitTweener(0.1f, 0f, 1f);
			_lastCameraFov = (_initCameraFov = base.camera.fieldOfView);
			InitTargetMakerScreenPos();
		}

		protected override void Start()
		{
			FiePostProcessContainer[] array = UnityEngine.Object.FindObjectsOfType<FiePostProcessContainer>();
			if (array != null || array.Length > 0)
			{
				FiePostProcessContainer[] array2 = array;
				foreach (FiePostProcessContainer fiePostProcessContainer in array2)
				{
					fiePostProcessContainer.AttachPostProcessEffect(base.gameObject);
				}
			}
			base.Start();
			if (array != null || array.Length > 0)
			{
				FiePostProcessContainer[] array3 = array;
				foreach (FiePostProcessContainer fiePostProcessContainer2 in array3)
				{
					fiePostProcessContainer2.PostHook(base.gameObject);
				}
			}
			FieManagerBehaviour<FieInGameStateManager>.I.RetryEvent += I_RetryEvent;
		}

		private void I_RetryEvent()
		{
			SetCameraTask<FieGameCameraTaskStop>();
		}

		public void InitTargetMakerScreenPos()
		{
			tagetMakerScreenPos = new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f);
		}

		public void AddTargetMakerScreenPos(Vector3 additionalScreenVector)
		{
			SetTargetMakerScreenPos(tagetMakerScreenPos + additionalScreenVector);
		}

		public void SetTargetMakerScreenPos(Vector3 newScreenPosition)
		{
			tagetMakerScreenPos.x = Mathf.Clamp(newScreenPosition.x, (float)Screen.width * 0.05f, (float)Screen.width * 0.95f);
			tagetMakerScreenPos.y = Mathf.Clamp(newScreenPosition.y, (float)Screen.height * 0.1f, (float)Screen.height * 0.9f);
		}

		public override void setCameraOwner(FieGameCharacter owner)
		{
			base.setCameraOwner(owner);
			if (base.cameraOwner != null)
			{
				base.cameraOwner.detector.targetChangedEvent += Detector_targetChangeEvent;
				base.cameraOwner.detector.completelyMissedEvent += Detector_completelyMissedEvent;
				_currentHp = (_currentAnimatinoHp = base.cameraOwner.healthStats.hitPoint);
				_maxHp = base.cameraOwner.healthStats.maxHitPoint;
			}
		}

		private void OnDestroy()
		{
			if (base.cameraOwner != null)
			{
				base.cameraOwner.detector.targetChangedEvent -= Detector_targetChangeEvent;
				base.cameraOwner.detector.completelyMissedEvent -= Detector_completelyMissedEvent;
			}
		}

		public void SetCameraTask<T>(float transitionTime = 0f) where T : FieGameCameraTaskBase, new()
		{
			if (nowTaskObject == null)
			{
				lastCameraTargetPos = (nowCameraTargetPos = base.transform.position);
				lastCameraTargetRotation = (nowCameraTargetRotation = base.transform.rotation);
			}
			else
			{
				nowTaskObject.Terminate(this);
			}
			nowTaskObject = new T();
			nowTaskObject.Initialize(this);
			if (transitionTime > 0f)
			{
				_transitionAlphaTweener.InitTweener(transitionTime, 0f, 1f);
			}
			beforeTaskChangedTargetPos = lastCameraTargetPos;
			beforeTaskChangedTargetRotation = lastCameraTargetRotation;
		}

		public FieGameCameraTaskBase GetCameraTask()
		{
			return nowTaskObject;
		}

		private void Detector_targetChangeEvent(FieGameCharacter fromCharacter, FieGameCharacter toCharacter)
		{
			if (nowTaskObject != null)
			{
				nowTaskObject.TargetChanged(this, fromCharacter, toCharacter);
			}
		}

		private void Detector_completelyMissedEvent(FieGameCharacter targetCharacter)
		{
			if (nowTaskObject != null)
			{
				nowTaskObject.TargetChanged(this, null, null);
			}
		}

		private void LateUpdate()
		{
			if (nowTaskObject != null)
			{
				nowTaskObject.CameraUpdate(this);
				if (_currentHp != base.cameraOwner.healthStats.hitPoint)
				{
					_damageTweener.InitTweener(0.5f, _currentHp, base.cameraOwner.healthStats.hitPoint);
					_currentHp = base.cameraOwner.healthStats.hitPoint;
				}
				if (!_damageTweener.IsEnd())
				{
					setDamageEffect(_damageTweener.UpdateParameterFloat(Time.deltaTime), _maxHp);
				}
				if (_cameraWiggler != null)
				{
					_wiggleRange = _cameraWiggler.UpdateWiggler(Time.deltaTime);
				}
				if (!_transitionAlphaTweener.IsEnd())
				{
					transitionAlpha = _transitionAlphaTweener.UpdateParameterFloat(Time.deltaTime);
				}
				lastCameraTargetPos = Vector3.Lerp(beforeTaskChangedTargetPos, nowCameraTargetPos, transitionAlpha);
				lastCameraTargetRotation = Quaternion.Lerp(beforeTaskChangedTargetRotation, nowCameraTargetRotation, transitionAlpha);
				if (_cameraOffset != null)
				{
					updateCameraOffset(_cameraOffset);
				}
				base.transform.position = lastCameraTargetPos + _wiggleRange + currentOffsetParam.position;
				base.transform.rotation = lastCameraTargetRotation * Quaternion.Euler(currentOffsetParam.angle);
				base.camera.fieldOfView = _lastCameraFov + currentOffsetParam.fov;
			}
		}

		private void updateCameraOffset(FieCameraOffset offset)
		{
			if (!offset.isEnd())
			{
				currentOffsetParam = offset.updateParams(Time.deltaTime);
			}
		}

		private void setDamageEffect(float nowHp, float maxHp)
		{
			if (!(maxHp <= 0f))
			{
				float num = Mathf.Max(0f, nowHp - (maxHp - nowHp) * 1f);
				float num2 = Mathf.Min(num / maxHp, 1f);
				if (_colorCorrectionEffect != null)
				{
					_colorCorrectionEffect.Amount = 0.4f - 0.4f * num2;
				}
				if (_vignetteEffect != null)
				{
					_vignetteEffect.Darkness = 20f - 20f * num2;
				}
				if (_reverbFilter != null)
				{
					_reverbFilter.dryLevel = -1000f - -1000f * num2;
					_reverbFilter.decayTime = 2f - 2f * num2;
					_reverbFilter.decayHFRatio = 1f - 1f * num2;
					_reverbFilter.roomHF = 1000f * num2;
					_reverbFilter.roomLF = 500f * num2;
					_reverbFilter.reverbLevel = 100f - 100f * num2;
					_reverbFilter.reverbDelay = 0.1f - 0.1f * num2;
				}
			}
		}

		public void setWiggler(Wiggler.WiggleTemplate template)
		{
			_cameraWiggler = new Wiggler(base.transform.rotation * Vector3.forward, template);
		}

		public void setWiggler(float totalTime, int wiggleCount, Vector3 wiggleScale)
		{
			_cameraWiggler = new Wiggler(base.transform.rotation * Vector3.forward, totalTime, wiggleCount, wiggleScale);
		}

		public void setOffsetTransition(FieGameCharacter callCharacter, FieCameraOffset offset, bool isOwnerOnly = true)
		{
			if (!(callCharacter == null) && offset != null && (!isOwnerOnly || isCameraOwner(callCharacter)))
			{
				_cameraOffset = offset;
			}
		}

		public bool existsTargetEnemy()
		{
			if (base.cameraOwner.detector.lockonTargetObject != null)
			{
				return true;
			}
			return false;
		}
	}
}
