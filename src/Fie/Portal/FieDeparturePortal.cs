using Fie.Manager;
using Fie.Scene;
using GameDataEditor;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Portal
{
	public sealed class FieDeparturePortal : FieVisualizedPortal
	{
		public FieConstValues.DefinedScenes nextScene;

		public bool isEnableActivity;

		private readonly Dictionary<FieConstValues.DefinedScenes, FieSceneBase> _sceneList = new Dictionary<FieConstValues.DefinedScenes, FieSceneBase>
		{
			{
				FieConstValues.DefinedScenes.FIE_TITLE,
				new FieSceneTitle()
			},
			{
				FieConstValues.DefinedScenes.FIE_LOBBY,
				new FieSceneLobby()
			},
			{
				FieConstValues.DefinedScenes.FIE_MAP1_1,
				new FieSceneMap1_1()
			},
			{
				FieConstValues.DefinedScenes.FIE_MAP1_2,
				new FieSceneMap1_2()
			},
			{
				FieConstValues.DefinedScenes.FIE_MAP1_3,
				new FieSceneMap1_3()
			},
			{
				FieConstValues.DefinedScenes.FIE_MAP1_4,
				new FieSceneMap1_4()
			},
			{
				FieConstValues.DefinedScenes.FIE_RESULT,
				new FieSceneResult()
			}
		};

		public new void Update()
		{
			base.Update();
		}

		protected override void Trigger()
		{
			if (PhotonNetwork.isMasterClient && !PhotonNetwork.offlineMode)
			{
				PhotonNetwork.room.IsOpen = false;
			}
			FieManagerBehaviour<FieSceneManager>.I.LoadScene(_sceneList[nextScene], allowSceneActivation: true, FieFaderManager.FadeType.OUT_TO_WHITE, FieFaderManager.FadeType.IN_FROM_WHITE, 1f);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!(other == null) && !(other.gameObject == null))
			{
				FieCollider component = other.gameObject.GetComponent<FieCollider>();
				if (!(component == null))
				{
					FieGameCharacter parentGameCharacter = component.getParentGameCharacter();
					if (!(parentGameCharacter == null) && isEnableActivity)
					{
						FieManagerBehaviour<FieActivityManager>.I.RequestGameOwnerOnlyActivity(parentGameCharacter, FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_DEPATURE_PORTAL_INFO), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_DEPATURE_PORTAL_INFO));
					}
				}
			}
		}
	}
}
