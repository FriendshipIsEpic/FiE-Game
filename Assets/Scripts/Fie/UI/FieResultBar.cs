using Fie.Manager;
using Fie.Utility;
using GameDataEditor;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieResultBar : MonoBehaviour
	{
		[SerializeField]
		private FieUIConstant2DText _playerName;

		[SerializeField]
		private FieUGUIUtilityGauge _levelGauge;

		[SerializeField]
		private FieUIConstant2DText _levelText;

		[SerializeField]
		private TMP_Text _expText;

		[SerializeField]
		private FieUIConstant2DText _scoreText;

		[SerializeField]
		private FieResultLevelEffect _levelUpEffect;

		[SerializeField]
		private Image _rankImage;

		[SerializeField]
		private Image _bgImage;

		[SerializeField]
		private AudioSource _levelUpSound;

		private FieResult.FieResultParameter _resultParam;

		[SerializeField]
		private Sprite[] _rankSprites = new Sprite[3];

		private bool _isEndTransition;

		public bool isEndTransitin => _isEndTransition;

		public IEnumerator ScoreTransition()
		{
			//_003CScoreTransition_003Ec__Iterator0 _003CScoreTransition_003Ec__Iterator = (_003CScoreTransition_003Ec__Iterator0)/*Error near IL_0032: stateMachine*/;
			FieLevelInfo expInfo = FieManagerBehaviour<FieSaveManager>.I.GetLevelInfoByTotalExp(_resultParam.beforeExp);
			_levelGauge.Initialize(0f, (float)expInfo.requiredExpToNextLevel, (float)expInfo.currentExpToNextLevel);
			string levelLabel = FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_SKILL_TREE_LEVEL_LABEL);
			string scoreLabel = FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_RESULT_SCORE);
			levelLabel = levelLabel.Replace("___Value2___", expInfo.levelCap.ToString());
			_levelText.replaceMethod = delegate(ref string text)
			{
				text = levelLabel.Replace("___Value1___", expInfo.level.ToString());
			};
			_expText.text = expInfo.currentExpToNextLevel.ToString() + " / " + expInfo.requiredExpToNextLevel;
			_scoreText.replaceMethod = delegate(ref string text)
			{
				text = scoreLabel.Replace("___Value1___", "0");
			};
			if (_resultParam.gainedExp == 0)
			{
				_isEndTransition = true;
			}
			else
			{
				Tweener<TweenTypesInOutSine> gainedExptweener = new Tweener<TweenTypesInOutSine>();
				gainedExptweener.InitTweener(3f, 0f, (float)_resultParam.gainedExp);
				if (!gainedExptweener.IsEnd())
				{
					int gainedExp = (int)gainedExptweener.UpdateParameterFloat(Time.deltaTime);
					_scoreText.TmpTextObject.text = scoreLabel.Replace("___Value1___", gainedExp.ToString());
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				_scoreText.TmpTextObject.text = scoreLabel.Replace("___Value1___", _resultParam.gainedExp.ToString());
				_isEndTransition = true;
			}
		}

		public IEnumerator GaugeTransition()
		{
			FieLevelInfo beforeLevelInfo = FieManagerBehaviour<FieSaveManager>.I.GetLevelInfoByTotalExp(_resultParam.beforeExp);
			FieLevelInfo afterLevelInfo = FieManagerBehaviour<FieSaveManager>.I.GetLevelInfoByTotalExp(_resultParam.beforeExp + _resultParam.teamGainedExp);
			int levelDiff = afterLevelInfo.level - beforeLevelInfo.level;
			string levelLabel2 = FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_SKILL_TREE_LEVEL_LABEL);
			levelLabel2 = levelLabel2.Replace("___Value2___", beforeLevelInfo.levelCap.ToString());
			_levelText.TmpTextObject.text = levelLabel2.Replace("___Value1___", beforeLevelInfo.level.ToString());
			_expText.text = beforeLevelInfo.currentExpToNextLevel.ToString() + " / " + beforeLevelInfo.requiredExpToNextLevel;
			if (_resultParam.teamGainedExp == 0 || beforeLevelInfo.totalExp == afterLevelInfo.totalExp)
			{
				_isEndTransition = true;
			}
			else
			{
				Tweener<TweenTypesInOutSine> totalExptweener = new Tweener<TweenTypesInOutSine>();
				totalExptweener.InitTweener(3f + 0.8f * (float)levelDiff, (float)beforeLevelInfo.totalExp, (float)afterLevelInfo.totalExp);
				FieLevelInfo latestInfo = beforeLevelInfo;
				if (!totalExptweener.IsEnd())
				{
					int currentExp = (int)totalExptweener.UpdateParameterFloat(Time.deltaTime);
					FieLevelInfo currentLevelInfo = FieManagerBehaviour<FieSaveManager>.I.GetLevelInfoByTotalExp(currentExp);
					if (latestInfo.level != currentLevelInfo.level)
					{
						_levelGauge.Initialize(0f, (float)currentLevelInfo.requiredExpToNextLevel, 0f);
						_levelUpEffect.PlayLevelupAnimation();
						_levelUpSound.Stop();
						_levelUpSound.Play();
						_levelText.TmpTextObject.text = levelLabel2.Replace("___Value1___", currentLevelInfo.level.ToString());
					}
					_levelGauge.SetValue((float)currentLevelInfo.currentExpToNextLevel, 0f);
					if (currentLevelInfo.level != currentLevelInfo.levelCap)
					{
						_expText.text = currentLevelInfo.currentExpToNextLevel + " / " + currentLevelInfo.requiredExpToNextLevel;
						yield return (object)null;
						/*Error: Unable to find new state assignment for yield return*/;
					}
					_expText.text = "MAX";
					latestInfo = currentLevelInfo;
				}
				if (latestInfo.level != afterLevelInfo.level)
				{
					_levelGauge.Initialize(0f, (float)afterLevelInfo.requiredExpToNextLevel, 0f);
					_levelUpEffect.PlayLevelupAnimation();
					_levelUpSound.Stop();
					_levelUpSound.Play();
					_levelText.TmpTextObject.text = levelLabel2.Replace("___Value1___", afterLevelInfo.level.ToString());
				}
				_levelGauge.SetValue((float)afterLevelInfo.currentExpToNextLevel, 0f);
				if (afterLevelInfo.level == afterLevelInfo.levelCap)
				{
					_expText.text = "MAX";
				}
				else
				{
					_expText.text = afterLevelInfo.currentExpToNextLevel + " / " + afterLevelInfo.requiredExpToNextLevel;
				}
				_isEndTransition = true;
			}
		}

		public void Initialize(FieResult.FieResultParameter resultParam, int rank)
		{
			switch (rank)
			{
			case 1:
				_rankImage.sprite = _rankSprites[0];
				break;
			case 2:
				_rankImage.sprite = _rankSprites[1];
				break;
			case 3:
				_rankImage.sprite = _rankSprites[2];
				break;
			default:
				_rankImage.sprite = null;
				break;
			}
			GDEGameCharacterTypeData gDEGameCharacterTypeData = FieMasterData<GDEGameCharacterTypeData>.FindMasterData(delegate(GDEGameCharacterTypeData data)
			{
				if (data.ID == (int)resultParam.gameCharacterID)
				{
					return true;
				}
				return false;
			});
			if (gDEGameCharacterTypeData != null)
			{
				_bgImage.color = new Color(gDEGameCharacterTypeData.SymbolColor.R, gDEGameCharacterTypeData.SymbolColor.G, gDEGameCharacterTypeData.SymbolColor.B, 1f);
			}
			_resultParam = resultParam;
			_playerName.replaceMethod = delegate(ref string text)
			{
				text = resultParam.userName;
			};
			_isEndTransition = false;
			StartCoroutine(ScoreTransition());
		}

		public void StartExpTransition()
		{
			_isEndTransition = false;
			StartCoroutine(GaugeTransition());
		}
	}
}
