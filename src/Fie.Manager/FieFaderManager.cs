using Fie.Fader;
using UnityEngine;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.NEVER_DESTROY)]
	public sealed class FieFaderManager : FieManagerBehaviour<FieFaderManager>
	{
		public enum FadeType
		{
			NONE,
			OUT_TO_BLACK,
			OUT_TO_WHITE,
			IN_FROM_AUTOMATIC,
			IN_FROM_BLACK,
			IN_FROM_WHITE
		}

		public enum FadeState
		{
			NOT_INITIALIZED,
			FADE_OUTING,
			FADE_OUT_DONE,
			FADE_INING,
			FADE_IN_DONE,
			DONE
		}

		public const float DEFAULT_FADE_TIME = 0.5f;

		private FadeType latestFaderType;

		private FieFader fader;

		private readonly Vector4 fadeOutToBlackColor = new Vector4(0f, 0f, 0f, 1f);

		private readonly Vector4 fadeOutToWhiteColor = new Vector4(1f, 1f, 1f, 1f);

		private readonly Vector4 fadeInFromBlackColor = new Vector4(0f, 0f, 0f, 0f);

		private readonly Vector4 fadeInFromWhiteColor = new Vector4(1f, 1f, 1f, 0f);

		private float showLoadScreenCount;

		private Coroutine faderCoroutine;

		protected override void StartUpEntity()
		{
			GameObject original = Resources.Load("Prefabs/LoadScreen/FieFaderObjects") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original);
			gameObject.transform.parent = base.transform;
			fader = gameObject.GetComponent<FieFader>();
			fader.Initialize();
		}

		public void InitializeFader(FadeType type, float fadeTime = 0.5f)
		{
			FadeType fadeType = type;
			switch (type)
			{
			case FadeType.NONE:
				fader.InitFader(Vector4.zero, Vector4.zero, 0f);
				break;
			case FadeType.IN_FROM_AUTOMATIC:
				if (latestFaderType == FadeType.OUT_TO_BLACK)
				{
					fader.InitFader(fadeOutToBlackColor, fadeInFromBlackColor, fadeTime);
					fadeType = FadeType.IN_FROM_BLACK;
				}
				else if (latestFaderType == FadeType.OUT_TO_WHITE)
				{
					fader.InitFader(fadeOutToWhiteColor, fadeInFromWhiteColor, fadeTime);
					fadeType = FadeType.IN_FROM_WHITE;
				}
				else
				{
					fader.InitFader(Vector4.zero, Vector4.zero, 0f);
				}
				break;
			case FadeType.OUT_TO_BLACK:
				fader.InitFader(fadeInFromBlackColor, fadeOutToBlackColor, fadeTime);
				break;
			case FadeType.OUT_TO_WHITE:
				fader.InitFader(fadeInFromWhiteColor, fadeOutToWhiteColor, fadeTime);
				break;
			case FadeType.IN_FROM_BLACK:
				fader.InitFader(fadeOutToBlackColor, fadeInFromBlackColor, fadeTime);
				break;
			case FadeType.IN_FROM_WHITE:
				fader.InitFader(fadeOutToWhiteColor, fadeInFromWhiteColor, fadeTime);
				break;
			}
			latestFaderType = fadeType;
		}

		public void HideFader()
		{
			fader.HideFader();
		}

		public bool isEndUpdateFader()
		{
			return fader.IsEndUpdateFader();
		}

		public void ShowLoadScreen(float showSec)
		{
			fader.ShowLoadScreen();
			showLoadScreenCount = showSec;
		}

		public void HideLoadScreen()
		{
			fader.HideLoadScreen();
		}

		public bool UpdateLoadScreen(float time)
		{
			showLoadScreenCount -= time;
			if (showLoadScreenCount <= 0f)
			{
				return true;
			}
			return false;
		}

		public void LaunchFader(FadeType fadeType, float fadeTime = 0.5f)
		{
			if (!FieManagerBehaviour<FieSceneManager>.I.isNowLoading)
			{
				FieManagerBehaviour<FieFaderManager>.I.InitializeFader(fadeType, fadeTime);
			}
		}

		public bool IsEndFade()
		{
			return fader.IsEndUpdateFader();
		}
	}
}
