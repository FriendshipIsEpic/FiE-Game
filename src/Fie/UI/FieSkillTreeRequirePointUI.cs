using Fie.Utility;
using GameDataEditor;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieSkillTreeRequirePointUI : MonoBehaviour
	{
		public enum RequireWindowState
		{
			UNLOKED,
			CAN_OPEN,
			NOT_ENOUGH_COSTS,
			REQUIRE_UNLOCK_LOW_LEVEL
		}

		public RequireWindowState state;

		[SerializeField]
		private Color _canNotOpenColor = Color.white;

		[SerializeField]
		private Color _canOpenColor = Color.white;

		[SerializeField]
		private FieUIConstant2DText _requirePointText;

		[SerializeField]
		private Image _backGround;

		private Tweener<TweenTypesOutSine> _transitionTweener = new Tweener<TweenTypesOutSine>();

		private Coroutine _transitionCoroutine;

		private IEnumerator ShowCoroutine(Color targetColor)
		{
			yield return (object)new WaitForSeconds(0.3f);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		private void Start()
		{
			state = RequireWindowState.UNLOKED;
			_requirePointText.TmpTextObject.color = Color.clear;
			_backGround.color = Color.clear;
		}

		public void Hide()
		{
			if (_transitionCoroutine != null)
			{
				StopCoroutine(_transitionCoroutine);
			}
			_requirePointText.TmpTextObject.color = Color.clear;
			_backGround.color = Color.clear;
		}

		public void ChangeState(RequireWindowState state, FieSkillTree skillTree, GDESkillTreeData skillData)
		{
			if (_transitionCoroutine != null)
			{
				StopCoroutine(_transitionCoroutine);
			}
			_requirePointText.TmpTextObject.color = Color.clear;
			_backGround.color = Color.clear;
			switch (state)
			{
			case RequireWindowState.CAN_OPEN:
			case RequireWindowState.NOT_ENOUGH_COSTS:
			{
				string constantText2 = FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_SKILL_TREE_REQUIRE_COST_LABLEL);
				constantText2 = constantText2.Replace("___Value1___", skillData.Cost.ToString());
				_requirePointText.TmpTextObject.text = constantText2;
				_transitionCoroutine = StartCoroutine(ShowCoroutine((state != RequireWindowState.CAN_OPEN) ? _canNotOpenColor : _canOpenColor));
				break;
			}
			case RequireWindowState.REQUIRE_UNLOCK_LOW_LEVEL:
			{
				string constantText = FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_SKILL_TREE_REQUIRE_UNLOCK_LABLEL);
				constantText = constantText.Replace("___Value1___", (skillData.SkillLevel - 1).ToString());
				_requirePointText.TmpTextObject.text = constantText;
				_transitionCoroutine = StartCoroutine(ShowCoroutine(_canNotOpenColor));
				break;
			}
			}
		}
	}
}
