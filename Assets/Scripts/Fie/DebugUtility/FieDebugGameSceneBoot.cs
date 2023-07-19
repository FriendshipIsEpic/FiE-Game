using Fie.Core;
using Fie.Manager;
using Fie.Scene;
using GameDataEditor;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.DebugUtility {
    public class FieDebugGameSceneBoot : MonoBehaviour {
        [Range(1f, 3f)]
        [SerializeField]
        private int playerNum = 1;

        [SerializeField]
        private List<FieGameCharacter> _debugCharacters = new List<FieGameCharacter>();

        [SerializeField]
        private List<FieGameCharacter.IntelligenceType> _debugCharactersIntelligenceTypes = new List<FieGameCharacter.IntelligenceType>();

        [SerializeField]
        private List<string> _debugCharactersNames = new List<string>();

        [SerializeField]
        private List<FieConstValues.FieSkill> _debugSkills = new List<FieConstValues.FieSkill>();

        private void Awake() {
            if (!FieBootstrap.isBootedFromBootStrap
                && _debugCharacters.Count == _debugCharactersIntelligenceTypes.Count
                && _debugCharactersIntelligenceTypes.Count == _debugCharactersNames.Count
                && _debugCharacters.Count == _debugCharactersNames.Count) {

                PhotonNetwork.offlineMode = true;
                PhotonNetwork.InstantiateInRoomOnly = false;

                FieManagerFactory.I.currentSceneType = FieSceneType.INGAME;
                FieManagerBehaviour<FieSceneManager>.I.StartUp();
                FieManagerBehaviour<FieSaveManager>.I.StartUp();
                FieManagerBehaviour<FieFaderManager>.I.StartUp();
                FieManagerBehaviour<FieEnvironmentManager>.I.StartUp();
                FieManagerBehaviour<FieUserManager>.I.nowPlayerNum = playerNum;
                FieManagerBehaviour<FieUserManager>.I.StartUp();

                List<GDESkillTreeData> list = FieMasterData<GDESkillTreeData>.FindMasterDataList(delegate (GDESkillTreeData data) {
                    return _debugSkills.Contains((FieConstValues.FieSkill)data.ID);
                });

                for (int i = 0; i < playerNum; i++) {
                    _debugCharacters[i].intelligenceType = _debugCharactersIntelligenceTypes[i];

                    if (list != null && list.Count > 0) {
                        FieSaveManager.debugSkills = list;
                    }

                    FieManagerBehaviour<FieUserManager>.I.SetUserName(i, _debugCharactersNames[i]);
                    FieManagerBehaviour<FieUserManager>.I.SetUserCharacterPrefab(i, _debugCharacters[i]);
                }
            }
        }

        private void Start() {
            FieManagerBehaviour<FieInGameStateManager>.I.StartUp();
        }
    }
}
