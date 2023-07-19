using Fie.Manager;
using GameDataEditor;
using UnityEngine;

namespace Fie.Portal
{
	public sealed class FieReturnPortal : FieVisualizedPortal
	{
		public new void Update()
		{
			base.Update();
		}

		protected override void Trigger()
		{
			FieManagerFactory.I.Restart();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!(other == null) && !(other.gameObject == null))
			{
				FieCollider component = other.gameObject.GetComponent<FieCollider>();
				if (!(component == null))
				{
					FieGameCharacter parentGameCharacter = component.getParentGameCharacter();
					if (!(parentGameCharacter == null))
					{
						FieManagerBehaviour<FieActivityManager>.I.RequestGameOwnerOnlyActivity(parentGameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_RETURNING_PORTAL_INFO), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_RETURNING_PORTAL_INFO));
					}
				}
			}
		}
	}
}
