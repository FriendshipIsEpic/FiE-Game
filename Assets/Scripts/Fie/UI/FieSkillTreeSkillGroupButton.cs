using UnityEngine;
using UnityEngine.UI;

namespace Fie.UI
{
	public class FieSkillTreeSkillGroupButton : MonoBehaviour
	{
		public delegate void FieSkillTreeSkillGroupButtonClickCallback(FieSkillTreeSkillGroupButton clickedButton);

		[SerializeField]
		private FieUIConstant2DText _SkillGroupNameText;

		private FieSkillTreeRootObject _relatedSkillGroupObject;

		public Button buttonEntity;

		public FieSkillTreeRootObject relatedSkillGroupObject => _relatedSkillGroupObject;

		public event FieSkillTreeSkillGroupButtonClickCallback clickedEvent;

		public void InitBySkillGroupObject(FieSkillTreeRootObject skillGroupObject)
		{
			_relatedSkillGroupObject = skillGroupObject;
			if (_relatedSkillGroupObject != null && _relatedSkillGroupObject.relatedSkillGroupData != null)
			{
				_SkillGroupNameText.replaceMethod = delegate(ref string targetText)
				{
					string constantText = FieLocalizeUtility.GetConstantText(_relatedSkillGroupObject.relatedSkillGroupData.SkillGroupName.Key);
					targetText = targetText.Replace("___Value1___", constantText);
				};
				_SkillGroupNameText.InitializeText();
			}
		}

		public void OnClickedCallback()
		{
			if (this.clickedEvent != null)
			{
				this.clickedEvent(this);
			}
		}
	}
}
