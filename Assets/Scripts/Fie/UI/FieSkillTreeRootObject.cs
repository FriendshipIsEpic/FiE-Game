using GameDataEditor;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.UI
{
	public class FieSkillTreeRootObject : MonoBehaviour
	{
		public Transform cameraPositionTransform;

		[SerializeField]
		private FieSkillTreeSelectablePoint[] _skillTreeBranches = new FieSkillTreeSelectablePoint[4];

		private GDESkillGroupData _relatedSkillGroupData;

		public FieSkillTreeSelectablePoint[] skillTreeBranches => _skillTreeBranches;

		public GDESkillGroupData relatedSkillGroupData => _relatedSkillGroupData;

		public void SetAllEndPointInteractiveState(bool isInteractable)
		{
			if (_skillTreeBranches != null && _skillTreeBranches.Length > 0)
			{
				for (int i = 0; i < _skillTreeBranches.Length; i++)
				{
					_skillTreeBranches[i].option1.interactable = isInteractable;
					_skillTreeBranches[i].option2.interactable = isInteractable;
				}
			}
		}

		public void InitializeSkillTreeObjects(FieSkillTree parent, GDESkillGroupData skillGroup)
		{
			if (_skillTreeBranches != null && _skillTreeBranches.Length > 0 && !(parent == null) && skillGroup != null)
			{
				_relatedSkillGroupData = skillGroup;
				List<GDESkillTreeData> list = FieMasterData<GDESkillTreeData>.FindMasterDataList(delegate(GDESkillTreeData data)
				{
					if (data.SkillGroup.ID == skillGroup.ID)
					{
						return true;
					}
					return false;
				});
				if (list != null && list.Count > 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						int num = list[i].SkillLevel - 1;
						int num2 = list[i].OptionID - 1;
						if ((num2 == 0 || num2 == 1) && _skillTreeBranches.IsValidIndex(num))
						{
							((num2 != 0) ? _skillTreeBranches[num].option2 : _skillTreeBranches[num].option1).AssignSkillData(list[i]);
						}
					}
					Reflesh3DUIComponents(parent);
				}
			}
		}

		public void Reflesh3DUIComponents(FieSkillTree parent)
		{
			if (_skillTreeBranches != null && _skillTreeBranches.Length > 0 && !(parent == null) && parent.saveData != null)
			{
				for (int i = 0; i < _skillTreeBranches.Length; i++)
				{
					if (_skillTreeBranches[i].option1.assigendSkillData != null)
					{
						int unlockedSkillIndex = 0;
						if (parent.saveData.unlockedSkills != null && parent.saveData.unlockedSkills.Count > 0)
						{
							if (parent.saveData.unlockedSkills.Contains(_skillTreeBranches[i].option1.assigendSkillData.ID))
							{
								_skillTreeBranches[i].option1.state = FieSkillTreeEndPoint.EndPointState.UNLOCKED;
								_skillTreeBranches[i].option2.state = FieSkillTreeEndPoint.EndPointState.LOCKED;
								unlockedSkillIndex = 1;
							}
							else if (parent.saveData.unlockedSkills.Contains(_skillTreeBranches[i].option2.assigendSkillData.ID))
							{
								_skillTreeBranches[i].option2.state = FieSkillTreeEndPoint.EndPointState.UNLOCKED;
								_skillTreeBranches[i].option1.state = FieSkillTreeEndPoint.EndPointState.LOCKED;
								unlockedSkillIndex = 2;
							}
						}
						_skillTreeBranches[i].unlockedSkillIndex = unlockedSkillIndex;
					}
				}
				for (int j = 0; j < _skillTreeBranches.Length; j++)
				{
					_skillTreeBranches[j].UpdateLineEffect();
				}
			}
		}

		internal int GetUnlockedLevel()
		{
			int num = 0;
			if (_skillTreeBranches == null || _skillTreeBranches.Length <= 0)
			{
				return 0;
			}
			for (int i = 0; i < _skillTreeBranches.Length; i++)
			{
				if (_skillTreeBranches[i].option1.state == FieSkillTreeEndPoint.EndPointState.UNLOCKED)
				{
					num = Mathf.Max(_skillTreeBranches[i].option1.assigendSkillData.SkillLevel, num);
				}
				if (_skillTreeBranches[i].option2.state == FieSkillTreeEndPoint.EndPointState.UNLOCKED)
				{
					num = Mathf.Max(_skillTreeBranches[i].option2.assigendSkillData.SkillLevel, num);
				}
			}
			return num;
		}
	}
}
