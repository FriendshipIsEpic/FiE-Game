using Fie.Scene;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.NEVER_DESTROY)]
	public sealed class FieSceneManager : FieManagerBehaviour<FieSceneManager>
	{
		public class FieLoadingSceneJob
		{
			public bool allowSceneActivation;

			public bool isDoneLoading;
		}

		public delegate void FiePreparedForLoadSceneDelegate(FieSceneBase targetScene);

		public delegate void FieSceneEventDelegate();

		public FieSceneBase latestScene;

		private bool _isNowLoading;

		private bool isForceWaitingInLoadScreen;

		private FieLoadingSceneJob _currentLoadingJob;

		public bool isNowLoading => _isNowLoading;

		public FieLoadingSceneJob currentLoadingJob => _currentLoadingJob;

		public event FiePreparedForLoadSceneDelegate FiePreparedForLoadSceneEvent;

		public event FiePreparedForLoadSceneDelegate FieSceneWasLoadedEvent;

		protected override void StartUpEntity()
		{
		}

		private IEnumerator LoadSceneTask(FieSceneBase targetScene, FieFaderManager.FadeType fadeOutType, FieFaderManager.FadeType fadeInType, float fadeTime = 0.5f, bool useLoadScreen = false, float minFadeScreenViewSec = 0f)
		{
			if (targetScene != null && _currentLoadingJob != null)
			{
				_isNowLoading = true;
				FieManagerBehaviour<FieInputManager>.I.isEnableControll = false;
				FieSceneLink sceneInfo = targetScene.GetSceneLinkInfo();
				Application.backgroundLoadingPriority = ThreadPriority.Low;
				AsyncOperation async = SceneManager.LoadSceneAsync((int)sceneInfo.linkedScene, LoadSceneMode.Single);
				if (async != null)
				{
					async.allowSceneActivation = false;
					if (async.progress < 0.9f)
					{
						yield return (object)null;
						/*Error: Unable to find new state assignment for yield return*/;
					}
					_currentLoadingJob.isDoneLoading = true;
					if (!_currentLoadingJob.allowSceneActivation)
					{
						yield return (object)null;
						/*Error: Unable to find new state assignment for yield return*/;
					}
					if (fadeOutType != 0)
					{
						FieManagerBehaviour<FieFaderManager>.I.InitializeFader(fadeOutType, fadeTime);
						if (!FieManagerBehaviour<FieFaderManager>.I.isEndUpdateFader())
						{
							yield return (object)null;
							/*Error: Unable to find new state assignment for yield return*/;
						}
					}
					if (useLoadScreen)
					{
						FieManagerBehaviour<FieFaderManager>.I.ShowLoadScreen(minFadeScreenViewSec);
						FieManagerBehaviour<FieFaderManager>.I.InitializeFader(FieFaderManager.FadeType.IN_FROM_AUTOMATIC, fadeTime);
						if (!FieManagerBehaviour<FieFaderManager>.I.isEndUpdateFader())
						{
							yield return (object)null;
							/*Error: Unable to find new state assignment for yield return*/;
						}
					}
					if (!PhotonNetwork.offlineMode && PhotonNetwork.inRoom)
					{
						FieManagerBehaviour<FieNetworkManager>.I.SetMySceneId((int)sceneInfo.linkedScene);
						bool isSameScene = false;
						float waitingTime2 = 0f;
						if (!isSameScene)
						{
							FieManagerBehaviour<FieNetworkManager>.I.CheckPlayerScenesIsSame();
							waitingTime2 += Time.deltaTime;
							if (!(waitingTime2 > 60f))
							{
								yield return (object)new WaitForSeconds(1f);
								/*Error: Unable to find new state assignment for yield return*/;
							}
						}
					}
					FieManagerFactory.I.KillPopcornFxAll();
					if (this.FiePreparedForLoadSceneEvent == null)
					{
						async.allowSceneActivation = true;
						latestScene = targetScene;
						if (!async.isDone)
						{
							yield return (object)null;
							/*Error: Unable to find new state assignment for yield return*/;
						}
						yield return (object)new WaitForSeconds(0.5f);
						/*Error: Unable to find new state assignment for yield return*/;
					}
					this.FiePreparedForLoadSceneEvent(targetScene);
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
		}

		public bool isLobby()
		{
			if (latestScene == null)
			{
				return false;
			}
			FieSceneLink sceneLinkInfo = latestScene.GetSceneLinkInfo();
			return sceneLinkInfo.linkedScene == FieConstValues.DefinedScenes.FIE_LOBBY;
		}

		public bool isTitle()
		{
			if (latestScene == null)
			{
				return false;
			}
			FieSceneLink sceneLinkInfo = latestScene.GetSceneLinkInfo();
			return sceneLinkInfo.linkedScene == FieConstValues.DefinedScenes.FIE_TITLE;
		}

		public FieLoadingSceneJob LoadScene(FieSceneBase loadScene, bool allowSceneActivation = true)
		{
			return LoadScene(loadScene, allowSceneActivation, FieFaderManager.FadeType.NONE, FieFaderManager.FadeType.NONE);
		}

		public FieLoadingSceneJob LoadScene(FieSceneBase loadScene, bool allowSceneActivation, FieFaderManager.FadeType fadeOutType, float fadeTime = 0.5f)
		{
			return LoadScene(loadScene, allowSceneActivation, fadeOutType, FieFaderManager.FadeType.IN_FROM_WHITE, fadeTime);
		}

		public FieLoadingSceneJob LoadScene(FieSceneBase loadScene, bool allowSceneActivation, FieFaderManager.FadeType fadeOutType, FieFaderManager.FadeType fadeInType, float fadeTime = 0.5f, bool useFadeScreen = false, float minFadeScreenViewSec = 0f)
		{
			if (_isNowLoading)
			{
				return currentLoadingJob;
			}
			try
			{
				_currentLoadingJob = new FieLoadingSceneJob();
				_currentLoadingJob.allowSceneActivation = allowSceneActivation;
				StartCoroutine(LoadSceneTask(loadScene, fadeOutType, fadeInType, fadeTime, useFadeScreen, minFadeScreenViewSec));
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
			return currentLoadingJob;
		}

		private void SetForceWaitInLoadScreenFlag(bool forceWait)
		{
			isForceWaitingInLoadScreen = forceWait;
		}
	}
}
