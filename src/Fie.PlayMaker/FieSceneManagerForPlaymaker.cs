using Fie.Manager;
using Fie.Scene;
using HutongGames.PlayMaker;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.PlayMaker
{
	[ActionCategory("Friendship is Epic")]
	public class FieSceneManagerForPlaymaker : FsmStateAction
	{
		public FieFaderManager.FadeType outFadeType;

		public FieFaderManager.FadeType inFadeType;

		public float fadeTime;

		public bool useLoadScreen;

		public float minLoadScreenViewSec;

		public FieConstValues.DefinedScenes nextScene;

		private readonly Dictionary<FieConstValues.DefinedScenes, FieSceneBase> _sceneList = new Dictionary<FieConstValues.DefinedScenes, FieSceneBase>
		{
			{
				FieConstValues.DefinedScenes.FIE_TITLE,
				new FieSceneTitle()
			},
			{
				FieConstValues.DefinedScenes.FIE_LOBBY,
				new FieSceneLobby()
			}
		};

		public override void OnEnter()
		{
			if (nextScene != FieConstValues.DefinedScenes.UNDEFINED)
			{
				if (!_sceneList.ContainsKey(nextScene))
				{
					Debug.LogError("A requested scene class dosen't exist. Name : " + nextScene.ToString());
				}
				else if (inFadeType != 0)
				{
					FieManagerBehaviour<FieSceneManager>.I.LoadScene(_sceneList[nextScene], allowSceneActivation: true, outFadeType, inFadeType, fadeTime, useLoadScreen, minLoadScreenViewSec);
				}
				else if (outFadeType != 0)
				{
					FieManagerBehaviour<FieSceneManager>.I.LoadScene(_sceneList[nextScene], allowSceneActivation: true, outFadeType, fadeTime);
				}
				else
				{
					FieManagerBehaviour<FieSceneManager>.I.LoadScene(_sceneList[nextScene]);
				}
			}
		}
	}
}
