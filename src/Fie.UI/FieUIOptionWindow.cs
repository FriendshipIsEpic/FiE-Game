using Fie.Manager;
using Fie.Utility;
using Rewired.UI.ControlMapper;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieUIOptionWindow : MonoBehaviour
	{
		public enum OptionMode
		{
			OPTION,
			KEY_CONFIG
		}

		public delegate void OptionScreenCloseCallback();

		[SerializeField]
		private RectTransform _optionWindowRectTransform;

		[SerializeField]
		private CanvasGroup _optionMenuCanvas;

		[SerializeField]
		private CanvasGroup _keyConfigCanvas;

		[SerializeField]
		private TMP_Dropdown _qualityDropdown;

		[SerializeField]
		private TMP_Dropdown _resolutionDropDown;

		[SerializeField]
		private TMP_Dropdown _languageDropDown;

		[SerializeField]
		private Toggle _fullScreenToggle;

		[SerializeField]
		private Button _decideButton;

		[SerializeField]
		private Button _keyConfigButton;

		[SerializeField]
		private Button _KeyConfigDecideButton;

		[SerializeField]
		private ControlMapper _controllMapper;

		[SerializeField]
		private LanguageData _languageEnglish;

		[SerializeField]
		private LanguageData _languageJapanese;

		[SerializeField]
		private LanguageData _languageFrench;

		private readonly List<Vector2> _resolutionList = new List<Vector2>
		{
			new Vector2(480f, 270f),
			new Vector2(640f, 360f),
			new Vector2(800f, 450f),
			new Vector2(960f, 540f),
			new Vector2(1280f, 720f),
			new Vector2(1600f, 900f),
			new Vector2(1920f, 1080f),
			new Vector2(2560f, 1440f),
			new Vector2(3840f, 2160f),
			new Vector2(1280f, 800f),
			new Vector2(1440f, 900f),
			new Vector2(1920f, 1200f),
			new Vector2(2560f, 1600f),
			new Vector2(3840f, 2400f)
		};

		private Vector3 _defaultPosition = Vector3.zero;

		private Vector3 _latestPosition = Vector3.zero;

		private Tweener<TweenTypesInOutSine> _scaleTweener = new Tweener<TweenTypesInOutSine>();

		private Tweener<TweenTypesInOutSine> _transitionTweener = new Tweener<TweenTypesInOutSine>();

		private bool _isEnableOptionScreen;

		private bool _isLockCallback;

		private bool _isTransition;

		private Coroutine windowTransitionCoroutine;

		public event OptionScreenCloseCallback optionScreenCloseEvent;

		private IEnumerator ShowOptionScreenCoroutine()
		{
			if (!_isTransition)
			{
				_isTransition = true;
				_optionWindowRectTransform.gameObject.SetActive(value: true);
				_scaleTweener.InitTweener(0.8f, new Vector3(0f, 800f, 0f), Vector3.zero);
				if (!_scaleTweener.IsEnd())
				{
					_optionWindowRectTransform.localPosition = _scaleTweener.UpdateParameterVec3(Time.deltaTime);
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				_optionWindowRectTransform.localPosition = _scaleTweener.getEndParamVec3();
				_isTransition = false;
				EventSystem.current.SetSelectedGameObject(_decideButton.gameObject);
			}
		}

		private IEnumerator HideOptionScreenCoroutine()
		{
			if (!_isTransition)
			{
				_isTransition = true;
				_scaleTweener.InitTweener(0.8f, Vector3.zero, new Vector3(0f, 800f, 0f));
				if (!_scaleTweener.IsEnd())
				{
					_optionWindowRectTransform.localPosition = _scaleTweener.UpdateParameterVec3(Time.deltaTime);
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				_optionWindowRectTransform.localPosition = _scaleTweener.getEndParamVec3();
				_optionWindowRectTransform.gameObject.SetActive(value: false);
				_isTransition = false;
			}
		}

		private IEnumerator ShowKeyConfigCoroutine()
		{
			if (!_isTransition)
			{
				_isTransition = true;
				_transitionTweener.InitTweener(0.5f, 1f, 0f);
				if (!_transitionTweener.IsEnd())
				{
					float rate = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
					_optionMenuCanvas.alpha = rate;
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				_optionMenuCanvas.alpha = 0f;
				_keyConfigCanvas.alpha = 1f;
				_keyConfigCanvas.gameObject.SetActive(value: true);
				_optionMenuCanvas.gameObject.SetActive(value: false);
				switch (FieManagerBehaviour<FieEnvironmentManager>.I.currentLanguage)
				{
				case FieEnvironmentManager.Language.Japanese:
					_controllMapper.language = _languageJapanese;
					break;
				case FieEnvironmentManager.Language.French:
					_controllMapper.language = _languageFrench;
					break;
				default:
					_controllMapper.language = _languageEnglish;
					break;
				}
				_controllMapper.Open();
				_transitionTweener.InitTweener(0.5f, 0f, 1f);
				if (!_transitionTweener.IsEnd())
				{
					float rate2 = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
					_keyConfigCanvas.alpha = rate2;
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				EventSystem.current.SetSelectedGameObject(_KeyConfigDecideButton.gameObject);
				_isTransition = false;
			}
		}

		private IEnumerator ShowOptionCoroutine()
		{
			if (!_isTransition)
			{
				_isTransition = true;
				_transitionTweener.InitTweener(0.5f, 1f, 0f);
				if (!_transitionTweener.IsEnd())
				{
					float rate = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
					_keyConfigCanvas.alpha = rate;
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				_keyConfigCanvas.alpha = 0f;
				_keyConfigCanvas.gameObject.SetActive(value: false);
				_optionMenuCanvas.gameObject.SetActive(value: true);
				_transitionTweener.InitTweener(0.5f, 0f, 1f);
				if (!_transitionTweener.IsEnd())
				{
					float rate2 = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
					_optionMenuCanvas.alpha = rate2;
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				_optionMenuCanvas.alpha = 1f;
				_isTransition = false;
			}
		}

		private void Awake()
		{
			_isLockCallback = true;
			if ((bool)_qualityDropdown)
			{
				List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
				int qualityLevel = QualitySettings.GetQualityLevel();
				int num = -1;
				for (int i = 0; i < 5; i++)
				{
					list.Add(new TMP_Dropdown.OptionData((-1 + i + 1).ToString().Replace("_", " ")));
					if (i == qualityLevel)
					{
						num = i;
					}
				}
				_qualityDropdown.AddOptions(list);
				if (num != -1)
				{
					_qualityDropdown.value = num;
				}
			}
			if (_languageDropDown != null)
			{
				List<TMP_Dropdown.OptionData> list2 = new List<TMP_Dropdown.OptionData>();
				int num2 = -1;
				for (int j = 0; j < 3; j++)
				{
					FieEnvironmentManager.Language language = (FieEnvironmentManager.Language)(-1 + j + 1);
					list2.Add(new TMP_Dropdown.OptionData(language.ToString().Replace("_", " ")));
					if (language == FieManagerBehaviour<FieEnvironmentManager>.I.currentLanguage)
					{
						num2 = (int)language;
					}
				}
				_languageDropDown.AddOptions(list2);
				if (num2 != -1)
				{
					_languageDropDown.value = num2;
				}
			}
			if (_fullScreenToggle != null)
			{
				_fullScreenToggle.isOn = Screen.fullScreen;
			}
			if (_resolutionDropDown != null && _resolutionDropDown.options.Count <= _resolutionList.Count)
			{
				int num3 = 0;
				foreach (Vector2 resolution in _resolutionList)
				{
					Vector2 current = resolution;
					if (Mathf.RoundToInt(current.x) == Screen.currentResolution.width && Mathf.RoundToInt(current.y) == Screen.currentResolution.height)
					{
						break;
					}
					num3++;
				}
				_resolutionDropDown.value = num3;
			}
			_optionWindowRectTransform.position = new Vector3(0f, 800f, 0f);
			_defaultPosition = _optionWindowRectTransform.position;
			_isLockCallback = false;
			_keyConfigCanvas.gameObject.SetActive(value: false);
			_optionMenuCanvas.gameObject.SetActive(value: true);
			base.gameObject.SetActive(value: false);
		}

		public void ShowOptionalScreen()
		{
			_keyConfigCanvas.gameObject.SetActive(value: false);
			_optionMenuCanvas.gameObject.SetActive(value: true);
			_isEnableOptionScreen = true;
			if (windowTransitionCoroutine != null)
			{
				StopCoroutine(windowTransitionCoroutine);
			}
			windowTransitionCoroutine = StartCoroutine(ShowOptionScreenCoroutine());
		}

		public void HideOptionalScreen()
		{
			if (windowTransitionCoroutine != null)
			{
				StopCoroutine(windowTransitionCoroutine);
			}
			windowTransitionCoroutine = StartCoroutine(HideOptionScreenCoroutine());
			_isEnableOptionScreen = false;
		}

		public void ChangeResolutionValue(int value)
		{
			if (!_isLockCallback && value >= 0 && value < _resolutionList.Count)
			{
				Vector2 vector = _resolutionList[value];
				Screen.SetResolution(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Screen.fullScreen);
			}
		}

		public void ChangedFullScreenToggle(bool value)
		{
			if (!_isLockCallback && _fullScreenToggle != null)
			{
				Screen.fullScreen = _fullScreenToggle.isOn;
			}
		}

		public void ChangeLanguageValue(int value)
		{
			if (!_isLockCallback && value > -1 && value < 3)
			{
				FieManagerBehaviour<FieEnvironmentManager>.I.SetLanguageSetting((FieEnvironmentManager.Language)value);
			}
		}

		public void ChangeQualityLevel(int value)
		{
			if (!_isLockCallback && value > -1 && value < 5)
			{
				QualitySettings.SetQualityLevel(value);
				if (FieManagerBehaviour<FieGameCameraManager>.I.gameCamera != null)
				{
					FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.CalibratePostEffects();
				}
				if (value == 0)
				{
					FieManagerBehaviour<FieEnvironmentManager>.I.CalibrateTimeEnviroment(1f, 30);
				}
				else
				{
					FieManagerBehaviour<FieEnvironmentManager>.I.CalibrateTimeEnviroment();
				}
			}
		}

		public void OnClickDecide()
		{
			HideOptionalScreen();
			if (this.optionScreenCloseEvent != null)
			{
				this.optionScreenCloseEvent();
			}
		}

		public void OnClickCancel()
		{
		}

		public void OnClickGoingToKeyConfig()
		{
			if (!_isTransition)
			{
				StartCoroutine(ShowKeyConfigCoroutine());
			}
		}

		public void OnClickGoingToOption()
		{
			if (!_isTransition)
			{
				FieManagerBehaviour<FieInputManager>.I.SaveAllMaps();
				StartCoroutine(ShowOptionCoroutine());
			}
		}
	}
}
