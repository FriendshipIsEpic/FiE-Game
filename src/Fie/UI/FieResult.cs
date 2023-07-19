using Fie.Manager;
using Fie.Scene;
using Fie.User;
using Fie.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieResult : MonoBehaviour
	{
		public enum ResultState
		{
			STATE_INIT,
			STATE_ANIMATING_SCORE,
			STATE_SHOWING_TOTAL_SCORE,
			STATE_ANIMATING_EXP,
			STATE_SHOWING_FOOTER,
			STATE_WAITING_OK_AT_SCORE,
			STATE_SHOWING_TRESURE,
			STATE_WAITING_OK_AT_FINALLY,
			STATE_LOADING_LOBBY
		}

		[Serializable]
		public struct FieResultParameter
		{
			public FieConstValues.FieGameCharacter gameCharacterID;

			public string userName;

			public int beforeExp;

			public int gainedExp;

			public int teamGainedExp;
		}

		private bool _isNowTransition;

		[SerializeField]
		private GameObject resultBarPrefab;

		[SerializeField]
		private RectTransform resultBarRoot;

		[SerializeField]
		private AudioSource scoreGainingSound;

		[SerializeField]
		private Animation totalScoreAnimation;

		[SerializeField]
		private TextMeshProUGUI totalScoreText;

		[SerializeField]
		private CanvasGroup hooterCanvas;

		[SerializeField]
		private FieSkillTreeExpAndLevelUI hooterExpComponent;

		[SerializeField]
		private FieSkillTreeReminingSkillPointsUI hooterSkillPointComponent;

		[SerializeField]
		private Button _okButton;

		public ResultState resultState;

		private List<FieResultBar> resultBars = new List<FieResultBar>();

		private FieResultParameter[] _resultParamters;

		private FieLevelInfo afterGameLevelInfo;

		private int afterGameSkillPoint;

		private void Start()
		{
			FieManagerBehaviour<FieInputManager>.I.isEnableControll = false;
			FieManagerBehaviour<FieGameCameraManager>.I.gameObject.SetActive(value: false);
			FieManagerBehaviour<FieGameCharacterManager>.I.gameObject.SetActive(value: false);
			hooterCanvas.gameObject.SetActive(value: false);
			FieGameCharacter gameOwnerCharacter = FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter;
			if (gameOwnerCharacter == null)
			{
				GotoLobby();
			}
			else
			{
				Dictionary<string, int> snapShottedExp = FieManagerBehaviour<FieSaveManager>.I.GetSnapShottedExp();
				Dictionary<string, int> currentGameExp = FieManagerBehaviour<FieSaveManager>.I.GetCurrentGameExp();
				if (snapShottedExp == null || snapShottedExp.Count <= 0)
				{
					GotoLobby();
				}
				else
				{
					_resultParamters = CreateResultParam(snapShottedExp, currentGameExp).ToArray();
					if (_resultParamters == null || _resultParamters.Length <= 0)
					{
						GotoLobby();
					}
					else
					{
						FieManagerBehaviour<FieSaveManager>.I.FlushExpToSaveData(gameOwnerCharacter);
						FieManagerBehaviour<FieSaveManager>.I.ResetCurrentGameData();
						afterGameLevelInfo = FieManagerBehaviour<FieSaveManager>.I.GetCharacterLevelInfo(gameOwnerCharacter.getGameCharacterID());
						afterGameSkillPoint = FieManagerBehaviour<FieSaveManager>.I.onMemorySaveData.CharacterSkillPoint[(int)gameOwnerCharacter.getGameCharacterID()];
						resultState = ResultState.STATE_INIT;
						InitWithResultParam(_resultParamters.ToList());
					}
				}
			}
		}

		private List<FieResultParameter> CreateResultParam(Dictionary<string, int> beforeGameExp, Dictionary<string, int> currentGameExp)
		{
			if (beforeGameExp == null || beforeGameExp.Count <= 0)
			{
				return null;
			}
			List<FieResultParameter> list = new List<FieResultParameter>();
			int num = 0;
			foreach (KeyValuePair<string, int> item2 in currentGameExp)
			{
				num += item2.Value;
			}
			foreach (KeyValuePair<string, int> item3 in beforeGameExp)
			{
				FieConstValues.FieGameCharacter gameCharacterID = FieConstValues.FieGameCharacter.NONE;
				FieResultParameter item = default(FieResultParameter);
				string userName = string.Empty;
				int gainedExp = 0;
				int userNumberByHash = FieManagerBehaviour<FieUserManager>.I.getUserNumberByHash(item3.Key);
				FieUser userData = FieManagerBehaviour<FieUserManager>.I.GetUserData(userNumberByHash);
				if (userData != null)
				{
					gameCharacterID = userData.usersCharacter.getGameCharacterID();
					userName = userData.userName;
				}
				if (currentGameExp.ContainsKey(item3.Key))
				{
					gainedExp = currentGameExp[item3.Key];
				}
				item.gameCharacterID = gameCharacterID;
				item.beforeExp = item3.Value;
				item.gainedExp = gainedExp;
				item.teamGainedExp = num;
				item.userName = userName;
				list.Add(item);
			}
			if (list == null || list.Count <= 0)
			{
				return null;
			}
			IOrderedEnumerable<FieResultParameter> source = from data in list
			orderby data.gainedExp descending
			select data;
			return source.ToList();
		}

		private void InitWithResultParam(List<FieResultParameter> resultParamters)
		{
			Vector2 sizeDelta = resultBarRoot.sizeDelta;
			float num = sizeDelta.y * 0.5f;
			float num2 = 0f - num + num * 0.5f * (float)(resultParamters.Count - 1);
			int num3 = 0;
			int num4 = 0;
			foreach (FieResultParameter resultParamter in resultParamters)
			{
				FieResultParameter current = resultParamter;
				num4 += current.gainedExp;
				GameObject gameObject = UnityEngine.Object.Instantiate(resultBarPrefab, Vector3.zero, Quaternion.identity);
				if (!(gameObject == null))
				{
					gameObject.transform.parent = resultBarRoot;
					gameObject.transform.localPosition = new Vector3(0f, num2 + (0f - num) * (float)num3, -1f);
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.localRotation = Quaternion.identity;
					FieResultBar component = gameObject.gameObject.GetComponent<FieResultBar>();
					if (!(component == null))
					{
						resultBars.Add(component);
						component.Initialize(current, num3 + 1);
						num3++;
					}
				}
			}
			totalScoreText.text = num4.ToString();
			resultState = ResultState.STATE_ANIMATING_SCORE;
		}

		private void Update()
		{
			switch (resultState)
			{
			case ResultState.STATE_SHOWING_FOOTER:
			case ResultState.STATE_WAITING_OK_AT_SCORE:
				break;
			case ResultState.STATE_ANIMATING_SCORE:
			{
				if (!scoreGainingSound.isPlaying)
				{
					scoreGainingSound.loop = true;
					scoreGainingSound.Play();
				}
				bool flag2 = true;
				for (int k = 0; k < resultBars.Count; k++)
				{
					flag2 &= resultBars[k].isEndTransitin;
				}
				if (flag2)
				{
					scoreGainingSound.Stop();
					totalScoreAnimation.Stop();
					totalScoreAnimation.Play();
					resultState = ResultState.STATE_SHOWING_TOTAL_SCORE;
				}
				break;
			}
			case ResultState.STATE_SHOWING_TOTAL_SCORE:
				if (!totalScoreAnimation.isPlaying)
				{
					for (int j = 0; j < resultBars.Count; j++)
					{
						resultBars[j].StartExpTransition();
					}
					resultState = ResultState.STATE_ANIMATING_EXP;
				}
				break;
			case ResultState.STATE_ANIMATING_EXP:
			{
				if (!scoreGainingSound.isPlaying)
				{
					scoreGainingSound.loop = true;
					scoreGainingSound.Play();
				}
				bool flag = true;
				for (int i = 0; i < resultBars.Count; i++)
				{
					flag &= resultBars[i].isEndTransitin;
				}
				if (flag)
				{
					scoreGainingSound.Stop();
					StartCoroutine(ShowingFooterCoroutine());
					resultState = ResultState.STATE_SHOWING_FOOTER;
				}
				break;
			}
			case ResultState.STATE_SHOWING_TRESURE:
				resultState = ResultState.STATE_WAITING_OK_AT_FINALLY;
				break;
			}
		}

		public void GotoLobby()
		{
			if (resultState != ResultState.STATE_LOADING_LOBBY)
			{
				if (PhotonNetwork.inRoom)
				{
					PhotonNetwork.LeaveRoom();
				}
				FieManagerBehaviour<FieGameCameraManager>.I.gameObject.SetActive(value: true);
				FieManagerBehaviour<FieGameCharacterManager>.I.gameObject.SetActive(value: true);
				FieManagerBehaviour<FieUserManager>.I.SetUserCharacter(FieManagerBehaviour<FieUserManager>.I.getUserNumberByHash(FieManagerBehaviour<FieUserManager>.I.myHash), null);
				FieManagerBehaviour<FieGameCharacterManager>.I.DestroyAllCharacters();
				FieManagerBehaviour<FieSceneManager>.I.LoadScene(new FieSceneLobby(), allowSceneActivation: true, FieFaderManager.FadeType.OUT_TO_WHITE, 1f);
				resultState = ResultState.STATE_LOADING_LOBBY;
			}
		}

		private IEnumerator FirstTransitionCoroutine()
		{
			yield break;
		}

		private IEnumerator ShowingFooterCoroutine()
		{
			hooterCanvas.gameObject.SetActive(value: true);
			hooterExpComponent.InithWithLevelInfo(afterGameLevelInfo);
			hooterSkillPointComponent.InithWithSkillPoint(afterGameSkillPoint);
			Tweener<TweenTypesInOutSine> canvasAlphaTweener = new Tweener<TweenTypesInOutSine>();
			canvasAlphaTweener.InitTweener(1f, 0f, 1f);
			if (!canvasAlphaTweener.IsEnd())
			{
				hooterCanvas.alpha = 1f * canvasAlphaTweener.UpdateParameterFloat(Time.deltaTime);
				yield return (object)null;
				/*Error: Unable to find new state assignment for yield return*/;
			}
			hooterCanvas.alpha = 1f;
			resultState = ResultState.STATE_SHOWING_TRESURE;
			EventSystem.current.SetSelectedGameObject(_okButton.gameObject);
		}
	}
}
