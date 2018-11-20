using UnityEngine;

namespace Fie.UI
{
	public class FieSkillTreeSelectablePoint : MonoBehaviour
	{
		[SerializeField]
		private FieSkillTreeEndPoint _option1;

		[SerializeField]
		private FieSkillTreeEndPoint _option2;

		[SerializeField]
		private FieSkillTreeSelectablePoint _linkedSelectablePoint;

		private int _unlockedSkillIndex;

		public FieSkillTreeEndPoint option1 => _option1;

		public FieSkillTreeEndPoint option2 => _option2;

		public int unlockedSkillIndex
		{
			get
			{
				return _unlockedSkillIndex;
			}
			set
			{
				_unlockedSkillIndex = Mathf.Clamp(value, 0, 2);
			}
		}

		public FieSkillTreeSelectablePoint linkedSelectablePoint => _linkedSelectablePoint;

		public void SetLinkedSelectablePoint(FieSkillTreeSelectablePoint _linkedPoint)
		{
			if (!(_linkedPoint == null))
			{
				_linkedSelectablePoint = _linkedPoint;
				UpdateLineEffect();
			}
		}

		public void UpdateLineEffect()
		{
			if (unlockedSkillIndex != 0 && !(_linkedSelectablePoint == null))
			{
				FieSkillTreeEndPoint fieSkillTreeEndPoint = (unlockedSkillIndex != 1) ? option2 : option1;
				switch (_linkedSelectablePoint.unlockedSkillIndex)
				{
				case 0:
					_linkedSelectablePoint.option1.ConnectLineToEndPoint(fieSkillTreeEndPoint);
					_linkedSelectablePoint.option2.ConnectLineToEndPoint(fieSkillTreeEndPoint);
					break;
				case 1:
					_linkedSelectablePoint.option1.ConnectLineToEndPoint(fieSkillTreeEndPoint);
					_linkedSelectablePoint.option2.DisableLineEffct();
					break;
				case 2:
					_linkedSelectablePoint.option1.DisableLineEffct();
					_linkedSelectablePoint.option2.ConnectLineToEndPoint(fieSkillTreeEndPoint);
					break;
				}
			}
		}
	}
}
