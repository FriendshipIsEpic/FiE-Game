using Fie.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.AI {
    public class FieAIHateController : FieAIControllerBase {

        private float MAXIMUM_STACKED_HATE = 6;

        private Dictionary<int, IEnumerator> _hateCorutine = new Dictionary<int, IEnumerator>();

        private Dictionary<int, float> _hateList = new Dictionary<int, float>();

        private void Start() {
            _ownerCharacter.damageSystem.damagedEvent += delegate (FieGameCharacter attacker, FieDamage damage) {
                if (attacker != null && damage != null && PhotonNetwork.isMasterClient) {
                    AddHate(attacker, damage.hate);
                }
            };
        }

        private void AddHate(FieGameCharacter attaker, float hate) {
            if (!(attaker == null)) {
                int instanceID = attaker.GetInstanceID();

                if (_hateCorutine.ContainsKey(instanceID)) {
                    StopCoroutine(_hateCorutine[instanceID]);
                }

                if (!_hateList.ContainsKey(instanceID)) {
                    _hateList[instanceID] = hate;
                } else {
                    _hateList[instanceID] = Mathf.Min(_hateList[instanceID] + hate, MAXIMUM_STACKED_HATE);
                }

                _hateCorutine[instanceID] = CalculateHateCoroutine(instanceID);

                StartCoroutine(_hateCorutine[instanceID]);
            }
        }

        private void Update() {
            updateLockonTargetByHate();
        }

        private void updateLockonTargetByHate() {
            if (_hateList.Count > 0) {
                bool flag = false;
                int instanceId = 0;
                float num = 0;

                foreach (KeyValuePair<int, float> hate in _hateList) {
                    if (num < hate.Value) {
                        instanceId = hate.Key;
                        flag = true;
                    }
                    num = Mathf.Max(num, hate.Value);
                }

                if (flag) {
                    _ownerCharacter.detector.ChangeLockonTargetByInstanceID(instanceId);
                }
            }
        }

        private IEnumerator CalculateHateCoroutine(int attackerInstanceId) {
            if (_hateList.ContainsKey(attackerInstanceId)) {
                
                _hateList[attackerInstanceId] -= Time.deltaTime;

                if (_hateList[attackerInstanceId] > 0) {
                    yield return 0;
                }

                _hateList.Remove(attackerInstanceId);
            }

            if (_hateCorutine.ContainsKey(attackerInstanceId)) {
                _hateCorutine.Remove(attackerInstanceId);
            }

            yield return null;
        }
    }
}
