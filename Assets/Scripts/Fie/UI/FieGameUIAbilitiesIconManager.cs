using Fie.Manager;
using Fie.Object;
using Fie.Object.Abilities;
using Fie.Ponies;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Fie.UI
{
	public class FieGameUIAbilitiesIconManager : FieGameUIComponentManagerBase
	{
		private Dictionary<int, FieGameUIAbilitiesIconBase> _abilityIconList = new Dictionary<int, FieGameUIAbilitiesIconBase>();

		public override void StartUp()
		{
			FiePonies fiePonies = base.componentManagerOwner as FiePonies;
			if (!(fiePonies == null))
			{
				KeyValuePair<Type, string> abilitiesIconInfo = fiePonies.getAbilitiesIconInfo();
				MethodInfo method = GetType().GetMethod("CreateAbilitiesIcons");
				if (method != null)
				{
					MethodInfo methodInfo = method.MakeGenericMethod(abilitiesIconInfo.Key);
					if (methodInfo != null)
					{
						object[] parameters = new object[2]
						{
							fiePonies,
							abilitiesIconInfo.Value
						};
						methodInfo.Invoke(this, parameters);
						Relocate();
					}
				}
			}
		}

		public void CreateAbilitiesIcons<T>(FiePonies ownerPony, string prefabPath) where T : FieGameUIAbilitiesIconBase
		{
			for (int i = 0; i < 3; i++)
			{
				FieManagerBehaviour<FieGUIManager>.I.AssignGameUIObject<T>(prefabPath);
				FieStateMachineAbilityInterface abilityInstance = ownerPony.getAbilityInstance((FieAbilitiesSlot.SlotType)i);
				if (abilityInstance != null)
				{
					FieGameUIAbilitiesIconBase fieGameUIAbilitiesIconBase = FieManagerBehaviour<FieGUIManager>.I.CreateGui<T>(ownerPony);
					fieGameUIAbilitiesIconBase.SetSlot((FieAbilitiesSlot.SlotType)i);
					fieGameUIAbilitiesIconBase.uiCamera = FieManagerBehaviour<FieGUIManager>.I.uiCamera;
					fieGameUIAbilitiesIconBase.abilityInstance = abilityInstance;
					_abilityIconList[i] = fieGameUIAbilitiesIconBase;
				}
			}
		}

		public void Relocate()
		{
			for (int i = 0; i < 3; i++)
			{
				if (_abilityIconList.ContainsKey(i))
				{
					FieGUIManager.FieUIPositionTag key = FieGUIManager.FieUIPositionTag.ABILITY_ICON_1;
					switch (i)
					{
					case 0:
						key = FieGUIManager.FieUIPositionTag.ABILITY_ICON_1;
						break;
					case 1:
						key = FieGUIManager.FieUIPositionTag.ABILITY_ICON_2;
						break;
					case 2:
						key = FieGUIManager.FieUIPositionTag.ABILITY_ICON_3;
						break;
					}
					if (FieManagerBehaviour<FieGUIManager>.I.uiPositionList[key] != null)
					{
						_abilityIconList[i].transform.position = FieManagerBehaviour<FieGUIManager>.I.uiPositionList[key].position;
						_abilityIconList[i].transform.parent = FieManagerBehaviour<FieGUIManager>.I.uiPositionList[key];
					}
				}
			}
		}

		public override void setComponentManagerOwner(FieGameCharacter owner)
		{
			base.setComponentManagerOwner(owner);
		}
	}
}
