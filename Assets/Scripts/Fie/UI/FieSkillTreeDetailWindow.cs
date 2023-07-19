using Fie.Utility;
using GameDataEditor;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Fie.UI
{
	public class FieSkillTreeDetailWindow : MonoBehaviour
	{
		public enum SkillTreeDetailWindowState
		{
			IDLE,
			SHOWING,
			HIDING,
			BUSY
		}

		[SerializeField]
		private RectTransform windowRectTransform;

		[SerializeField]
		private float tweenTime = 0.3f;

		[SerializeField]
		private float minWindowSizeWidth = 96f;

		[SerializeField]
		private float minWindowSizeHeight = 96f;

		[SerializeField]
		private float windowSizeMarginWidth;

		[SerializeField]
		private float windowSizeMarginHeight;

		[SerializeField]
		private FieGameUIActivityWindowText titleTextComponent;

		[SerializeField]
		private FieGameUIActivityWindowText noteTextComponent;

		private Tweener<TweenTypesOutSine> _transitionTweener = new Tweener<TweenTypesOutSine>();

		private Vector2 _currentWindowTargetSize = Vector2.zero;

		private float _currentSizeRate;

		private Coroutine _transitionCoroutine;

		private IEnumerator ShowCoroutine(float targetRate)
		{
			_transitionTweener.InitTweener(tweenTime, _currentSizeRate, targetRate);
			if (!_transitionTweener.IsEnd())
			{
				_currentSizeRate = _transitionTweener.UpdateParameterFloat(Time.deltaTime);
				windowRectTransform.sizeDelta = _currentWindowTargetSize * _currentSizeRate;
				yield return (object)null;
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		private void Start()
		{
			Initialize();
		}

		public void Initialize()
		{
			if (_transitionCoroutine != null)
			{
				StopCoroutine(_transitionCoroutine);
			}
			_currentSizeRate = 0f;
			titleTextComponent.SetText(string.Empty);
			titleTextComponent.SetText(string.Empty);
			_currentWindowTargetSize = Vector2.zero;
			windowRectTransform.sizeDelta = _currentWindowTargetSize;
		}

		public void HideText()
		{
			if (_transitionCoroutine != null)
			{
				StopCoroutine(_transitionCoroutine);
			}
			titleTextComponent.HideText(0f);
			noteTextComponent.HideText(0f);
			StartCoroutine(ShowCoroutine(0f));
		}

		public void ShowText(Vector3 position, FieSkillTreeEndPoint endPoint, GDEGameCharacterTypeData gameCharacterTypeData)
		{
			if (_transitionCoroutine != null)
			{
				StopCoroutine(_transitionCoroutine);
			}
			_currentSizeRate = 0f;
			RectTransform rectTransform = base.transform as RectTransform;
			if (rectTransform != null)
			{
				windowRectTransform.pivot = ((!(position.x < 0f)) ? new Vector2(1f, 0.5f) : new Vector2(0f, 0.5f));
				position.x += ((!(position.x < 0f)) ? (-64f) : 64f);
				rectTransform.localPosition = position;
			}
			titleTextComponent.HideText(0f);
			noteTextComponent.HideText(0f);
			string title = string.Empty;
			string desc = string.Empty;
			PerseSkillText(endPoint.assigendSkillData, gameCharacterTypeData, ref title, ref desc);
			Vector2 vector = titleTextComponent.SetText(title);
			Vector2 vector2 = noteTextComponent.SetText(desc);
			_currentWindowTargetSize.x = Mathf.Max(Mathf.Max(vector.x, vector2.x) + windowSizeMarginWidth, minWindowSizeWidth);
			_currentWindowTargetSize.y = Mathf.Max(Mathf.Max(vector.y, vector2.y) + windowSizeMarginHeight, minWindowSizeHeight);
			titleTextComponent.ShowText();
			noteTextComponent.ShowText();
			StartCoroutine(ShowCoroutine(1f));
		}

		private void PerseSkillText(GDESkillTreeData skillData, GDEGameCharacterTypeData gameCharacterTypeData, ref string title, ref string desc)
		{
			string empty = string.Empty;
			empty = ((skillData.SpecificDescriptionText == null || !(skillData.SpecificDescriptionText.Key != string.Empty)) ? FieLocalizeUtility.GetConstantText(skillData.SkillType.DescriptionText.Key) : FieLocalizeUtility.GetConstantText(skillData.SpecificDescriptionText.Key));
			empty = empty.Replace("\n", "[break]");
			Match match = Regex.Match(empty, "<title>(.*\\r\\n){0,10000}.*?<\\/title>");
			Match match2 = Regex.Match(empty, "<desc>(.*\\r\\n){0,10000}.*?<\\/desc>");
			if (match != null && match.Value != null)
			{
				title = match.Value;
			}
			if (match2 != null && match2.Value != null)
			{
				desc = match2.Value;
			}
			title = title.Replace("<title>", string.Empty);
			title = title.Replace("</title>", string.Empty);
			desc = desc.Replace("<desc>", string.Empty);
			desc = desc.Replace("</desc>", string.Empty);
			ReplaceMetaData(skillData, gameCharacterTypeData, ref title);
			ReplaceMetaData(skillData, gameCharacterTypeData, ref desc);
		}

		private void ReplaceMetaData(GDESkillTreeData skillData, GDEGameCharacterTypeData gameCharacterTypeData, ref string text)
		{
			text = text.Replace("[break]", "\n");
			if (skillData.Ability != null && skillData.Ability.Key != string.Empty)
			{
				text = text.Replace("___ABILITY_NAME___", FieLocalizeUtility.GetConstantText(skillData.Ability.AbilityName.Key));
			}
			if (gameCharacterTypeData != null && gameCharacterTypeData.Key != string.Empty && skillData.SkillType.ID == 6)
			{
				if (skillData.Value1 > 0f)
				{
					string empty = string.Empty;
					empty = ((!(skillData.Value2 > 0f)) ? FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_HEALTH_ATTRIBUTE_HEALTH) : FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_HEALTH_AND_SHIELD));
					text = text.Replace("___HEALTH_ATTRIBUTE_NAME___", empty);
				}
				string newValue = string.Empty;
				if (skillData.Value2 > 0f)
				{
					if (gameCharacterTypeData.ShieldType == "magic")
					{
						newValue = FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_HEALTH_ATTRIBUTE_MAGIC_POWER);
					}
					else if (gameCharacterTypeData.ShieldType == "wing")
					{
						newValue = FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_HEALTH_ATTRIBUTE_WING_POWER);
					}
					else if (gameCharacterTypeData.ShieldType == "earth")
					{
						newValue = FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_HEALTH_ATTRIBUTE_EARTH_POWER);
					}
					text = text.Replace("___Value1___", Mathf.Abs(Mathf.RoundToInt(skillData.Value2 * 100f)).ToString());
				}
				text = text.Replace("___HEALTH_ATTRIBUTE_NAME___", newValue);
			}
			text = text.Replace("___Value1___", Mathf.Abs(Mathf.RoundToInt(skillData.Value1 * 100f)).ToString());
			text = text.Replace("___Value1_Raw___", Mathf.Abs(Mathf.RoundToInt(skillData.Value1)).ToString());
		}
	}
}
