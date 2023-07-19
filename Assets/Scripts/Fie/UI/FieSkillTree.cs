using Fie.Camera;
using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fie.UI
{
	public class FieSkillTree : MonoBehaviour
	{
		public enum SkillTreeMode
		{
			OVERALL,
			DETAIL
		}

		public bool isEnd;

		public SkillTreeMode skillTreeMode;

		private bool _isNowTransition;

		[SerializeField]
		private FieSkillTreeRootObject[] skillTrees = new FieSkillTreeRootObject[5];

		[SerializeField]
		private FieSkillTreeSkillGroupButton[] skillGroupButtons = new FieSkillTreeSkillGroupButton[5];

		[SerializeField]
		private RectTransform canvasTransform;

		[SerializeField]
		private FieSkillTreeExpAndLevelUI expAndLevelUI;

		[SerializeField]
		private FieSkillTreeReminingSkillPointsUI reminingSkillPointUI;

		[SerializeField]
		private FieSkillTreeDetailGuideUI levelGuideUI;

		[SerializeField]
		private FieSkillTreeConfirmationUI confirmationUI;

		[SerializeField]
		private FieSkillTreeDetailWindow detailWindow;

		[SerializeField]
		private FieSkillTreeRequirePointUI requirePointUI;

		[SerializeField]
		private FieSkillTreeDetailCursorUI detailCursorUI;

		[SerializeField]
		private FieSkillTreeHeaderUI headerUI;

		[SerializeField]
		private FieSkillTreeSkillGroupButtonRoot skillGroupButtonRoot;

		[SerializeField]
		private FieSkillTreeBackButton backButton;

		[SerializeField]
		private FieSkillTreeCamera skillTreeCamera;

		[SerializeField]
		private FieSkillTreeActivationEffect activationEffect;

		[SerializeField]
		private Transform defaultCameraTransform;

		[SerializeField]
		private AudioClip goToDetailSound;

		[SerializeField]
		private AudioClip goToOverallSound;

		[SerializeField]
		private AudioSource audioSource;

		[SerializeField]
		private PKFxFX _atomosphicParticle;

		private Transform currentCameraTransform;

		private FieSkillTreeRootObject _latestSelectedTree;

		private GameObject _latestSelectedEndPointGameObject;

		private FieSkillTreeEndPoint _latestSelectedEndPoint;

		private Vector2 _latestSelectedEndPointScreenPosition;

		private FieGameCharacter _currentOwnerGameCharacter;

		private GDEGameCharacterTypeData _currentGameCharacterTypeData;

		private FieGameCharacterBuildData _buildData;

		private GameObject _latestSelectedGroupButton;

		private bool _isUnlockableLatestEndPoint;

		private Tweener<TweenTypesInOutSine> _modeSelectionTransitionTweener = new Tweener<TweenTypesInOutSine>();

		public FieSaveData saveData => FieManagerBehaviour<FieSaveManager>.I.onMemorySaveData;

		public int getExp(FieGameCharacter gameCharacter)
		{
			if (gameCharacter != null && FieManagerBehaviour<FieSaveManager>.I.onMemorySaveData.CharacterExp.ContainsKey((int)gameCharacter.getGameCharacterID()))
			{
				return FieManagerBehaviour<FieSaveManager>.I.onMemorySaveData.CharacterExp[(int)gameCharacter.getGameCharacterID()];
			}
			return 0;
		}

		private IEnumerator FirstTransitionCoroutine()
		{
			_isNowTransition = true;
			yield return (object)new WaitForSeconds(0.75f);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		private IEnumerator ToDetailTransitionCoroutine(FieSkillTreeRootObject targetSkillTreeRootObject)
		{
			if (!(targetSkillTreeRootObject == null) && !_isNowTransition)
			{
				_isNowTransition = true;
				_latestSelectedGroupButton = EventSystem.current.currentSelectedGameObject;
				EventSystem.current.SetSelectedGameObject(null);
				for (int i = 0; i < skillGroupButtons.Length; i++)
				{
					if (skillGroupButtons.IsValidIndex(i))
					{
						skillGroupButtons[i].buttonEntity.interactable = false;
					}
				}
				skillGroupButtonRoot.Hide();
				audioSource.PlayOneShot(goToOverallSound);
				headerUI.Hide();
				levelGuideUI.Show();
				Vector3 position = skillTreeCamera.transform.position;
				Quaternion rotation = skillTreeCamera.transform.rotation;
				Vector3 position2 = targetSkillTreeRootObject.cameraPositionTransform.position;
				Quaternion rotation2 = targetSkillTreeRootObject.cameraPositionTransform.rotation;
				skillTreeCamera.SetViewMode(isOrtho: false);
				yield return (object)new WaitForSeconds(0.3f);
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		private IEnumerator ToOverallTransitionCoroutine()
		{
			if (!_isNowTransition)
			{
				_isNowTransition = true;
				_latestSelectedEndPointGameObject = null;
				EventSystem.current.SetSelectedGameObject(null);
				if (_latestSelectedTree != null)
				{
					_latestSelectedTree.SetAllEndPointInteractiveState(isInteractable: false);
				}
				_isUnlockableLatestEndPoint = false;
				skillGroupButtonRoot.Show();
				audioSource.PlayOneShot(goToDetailSound);
				headerUI.Show();
				levelGuideUI.Hide();
				confirmationUI.Hide();
				detailWindow.HideText();
				requirePointUI.Hide();
				detailCursorUI.Hide();
				Vector3 position = skillTreeCamera.transform.position;
				Quaternion rotation = skillTreeCamera.transform.rotation;
				Vector3 position2 = defaultCameraTransform.position;
				Quaternion rotation2 = defaultCameraTransform.rotation;
				skillTreeCamera.SetViewMode(isOrtho: true);
				yield return (object)new WaitForSeconds(0.3f);
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public void Initialize(FieGameCharacter ownerCharacter, FieGameCharacterBuildData buildData)
		{
			isEnd = false;
			InitializeSkillTree(ownerCharacter, buildData);
			_currentOwnerGameCharacter = ownerCharacter;
			_currentGameCharacterTypeData = ownerCharacter.getGameCharacterTypeData();
			_buildData = buildData;
			_atomosphicParticle.StopEffect();
			_atomosphicParticle.StartEffect();
		}

		private void OnDisable()
		{
			_atomosphicParticle.StopEffect();
		}

		private void InitializeSkillTree(FieGameCharacter ownerCharacter, FieGameCharacterBuildData buildData)
		{
			if (!(ownerCharacter == null))
			{
				Init3DComponent(ownerCharacter);
				Init2DComponent(ownerCharacter, buildData);
				skillTreeMode = SkillTreeMode.OVERALL;
				currentCameraTransform = defaultCameraTransform;
				skillTreeCamera.transform.position = currentCameraTransform.position;
				skillTreeCamera.transform.rotation = currentCameraTransform.rotation;
				StartCoroutine(FirstTransitionCoroutine());
			}
		}

		private void Init3DComponent(FieGameCharacter ownerCharacter)
		{
			if (skillTrees != null && skillTrees.Length > 0)
			{
				int i;
				for (i = 0; i < skillTrees.Length; i++)
				{
					if (ownerCharacter.skillTreeSlot.IsValidIndex(i))
					{
						GDESkillGroupData gDESkillGroupData = FieMasterData<GDESkillGroupData>.FindMasterData(delegate(GDESkillGroupData data)
						{
							if (data.ID == ownerCharacter.skillTreeSlot[i])
							{
								return true;
							}
							return false;
						});
						if (gDESkillGroupData != null)
						{
							skillTrees[i].gameObject.SetActive(value: true);
							skillTrees[i].InitializeSkillTreeObjects(this, gDESkillGroupData);
						}
						else
						{
							skillTrees[i].gameObject.SetActive(value: false);
						}
						skillTrees[i].SetAllEndPointInteractiveState(isInteractable: false);
					}
				}
				if (skillTreeCamera != null)
				{
					skillTreeCamera.SetViewMode(isOrtho: true);
				}
			}
		}

		private void Init2DComponent(FieGameCharacter ownerCharacter, FieGameCharacterBuildData buildData)
		{
			if (skillGroupButtons != null || skillGroupButtons.Length > 0)
			{
				for (int i = 0; i < skillGroupButtons.Length; i++)
				{
					if (skillGroupButtons.IsValidIndex(i))
					{
						skillGroupButtons[i].InitBySkillGroupObject(skillTrees[i]);
						skillGroupButtons[i].clickedEvent -= GroupButtonClickedCallback;
						skillGroupButtons[i].clickedEvent += GroupButtonClickedCallback;
						skillGroupButtons[i].buttonEntity.interactable = false;
					}
				}
			}
			if (skillGroupButtonRoot != null)
			{
				skillGroupButtonRoot.Show();
			}
			if (expAndLevelUI != null)
			{
				expAndLevelUI.InithWithLevelInfo(buildData.levelInfo);
			}
			if (reminingSkillPointUI != null)
			{
				reminingSkillPointUI.InithWithSkillPoint(buildData.skillPoint);
			}
			if (headerUI != null)
			{
				headerUI.InitWithGameCharacterData(ownerCharacter.getCharacterTypeData());
				headerUI.Show();
			}
			if (backButton != null)
			{
				backButton.clickedEvent -= BackButtonClickedCallback;
				backButton.clickedEvent += BackButtonClickedCallback;
			}
			if (confirmationUI != null)
			{
				confirmationUI.Initialize();
			}
		}

		private void Update()
		{
			SkillTreeMode skillTreeMode = this.skillTreeMode;
			if (skillTreeMode == SkillTreeMode.DETAIL)
			{
				UpdateDetailMode();
			}
		}

		private void UpdateDetailMode()
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject != _latestSelectedEndPointGameObject && currentSelectedGameObject != null)
			{
				FieSkillTreeEndPoint component = currentSelectedGameObject.GetComponent<FieSkillTreeEndPoint>();
				_isUnlockableLatestEndPoint = false;
				if (component != null)
				{
					_latestSelectedEndPoint = component;
					RectTransform rectTransform = canvasTransform;
					Vector2 vector = skillTreeCamera.camera.WorldToViewportPoint(component.transform.position);
					float x = vector.x;
					Vector2 sizeDelta = canvasTransform.sizeDelta;
					float num = x * sizeDelta.x;
					Vector2 sizeDelta2 = canvasTransform.sizeDelta;
					float x2 = num - sizeDelta2.x * 0.5f;
					float y = vector.y;
					Vector2 sizeDelta3 = canvasTransform.sizeDelta;
					float num2 = y * sizeDelta3.y;
					Vector2 sizeDelta4 = canvasTransform.sizeDelta;
					_latestSelectedEndPointScreenPosition = new Vector2(x2, num2 - sizeDelta4.y * 0.5f);
					detailWindow.ShowText(_latestSelectedEndPointScreenPosition, component, _currentGameCharacterTypeData);
					FieSkillTreeRequirePointUI.RequireWindowState state = FieSkillTreeRequirePointUI.RequireWindowState.UNLOKED;
					if (component.state == FieSkillTreeEndPoint.EndPointState.LOCKED)
					{
						int unlockedLevel = _latestSelectedTree.GetUnlockedLevel();
						if (component.assigendSkillData.SkillLevel > unlockedLevel + 1)
						{
							state = FieSkillTreeRequirePointUI.RequireWindowState.REQUIRE_UNLOCK_LOW_LEVEL;
						}
						else if (_buildData.skillPoint >= component.assigendSkillData.Cost)
						{
							state = FieSkillTreeRequirePointUI.RequireWindowState.CAN_OPEN;
							_isUnlockableLatestEndPoint = true;
						}
						else if (_buildData.skillPoint < component.assigendSkillData.Cost)
						{
							state = FieSkillTreeRequirePointUI.RequireWindowState.NOT_ENOUGH_COSTS;
						}
						if (component.parent != null)
						{
							bool flag = false;
							flag |= (component.parent.option1.state == FieSkillTreeEndPoint.EndPointState.UNLOCKED);
							if (flag | (component.parent.option2.state == FieSkillTreeEndPoint.EndPointState.UNLOCKED))
							{
								state = FieSkillTreeRequirePointUI.RequireWindowState.UNLOKED;
								_isUnlockableLatestEndPoint = false;
							}
						}
					}
					requirePointUI.ChangeState(state, this, component.assigendSkillData);
					detailCursorUI.Show();
				}
				else
				{
					detailWindow.HideText();
					requirePointUI.Hide();
					detailCursorUI.Hide();
				}
				_latestSelectedEndPointGameObject = currentSelectedGameObject;
			}
			if (_latestSelectedEndPointGameObject != null)
			{
				detailCursorUI.rectTransform.localPosition = Vector3.Lerp(detailCursorUI.rectTransform.localPosition, _latestSelectedEndPointScreenPosition, Mathf.Min(Time.deltaTime * 10f, 1f));
			}
		}

		private void BackButtonClickedCallback(FieSkillTreeBackButton clickedButton)
		{
			if (!_isNowTransition)
			{
				switch (skillTreeMode)
				{
				case SkillTreeMode.DETAIL:
					StartCoroutine(ToOverallTransitionCoroutine());
					break;
				case SkillTreeMode.OVERALL:
					isEnd = true;
					break;
				}
			}
		}

		private void ActivateSkill(FieSkillTreeEndPoint endPoint, FieSkillTreeRootObject selectedTree)
		{
			if (!(endPoint == null) && !(selectedTree == null) && _buildData.skillPoint >= endPoint.assigendSkillData.Cost)
			{
				saveData.unlockedSkills.Add(endPoint.assigendSkillData.ID);
				FieManagerBehaviour<FieSaveManager>.I.UnlockSkills(endPoint.assigendSkillData);
				if (!saveData.unlockedSkills.Contains(endPoint.assigendSkillData.ID))
				{
					Debug.LogError("Faild to unlock the skill! : " + endPoint.assigendSkillData.Key);
				}
				FieManagerBehaviour<FieSaveManager>.I.IncreaseOrReduceSkillPoints(_currentGameCharacterTypeData, -endPoint.assigendSkillData.Cost);
				_buildData.skillPoint = saveData.CharacterSkillPoint[_currentGameCharacterTypeData.ID];
				activationEffect.Activate(endPoint);
				_latestSelectedTree.Reflesh3DUIComponents(this);
				reminingSkillPointUI.InithWithSkillPoint(_buildData.skillPoint);
			}
		}

		private void GroupButtonClickedCallback(FieSkillTreeSkillGroupButton clickedButton)
		{
			if (!_isNowTransition)
			{
				StartCoroutine(ToDetailTransitionCoroutine(clickedButton.relatedSkillGroupObject));
			}
		}

		private void SkillClickedCallback(FieSkillTreeSkillGroupButton clickedButton)
		{
			if (!_isNowTransition)
			{
				StartCoroutine(ToDetailTransitionCoroutine(clickedButton.relatedSkillGroupObject));
			}
		}

		public void OnClickedEndPoint()
		{
			if (skillTreeMode == SkillTreeMode.DETAIL && !_isNowTransition && _isUnlockableLatestEndPoint)
			{
				FieSkillTreeEndPoint component = _latestSelectedEndPointGameObject.GetComponent<FieSkillTreeEndPoint>();
				if (component != null)
				{
					string constantText = FieLocalizeUtility.GetConstantText(GDEItemKeys.ConstantTextList_SKILL_TREE_REQUIRE_COST_LABLEL);
					confirmationUI.costText.TmpTextObject.text = constantText.Replace("___Value1___", component.assigendSkillData.Cost.ToString());
				}
				_latestSelectedTree.SetAllEndPointInteractiveState(isInteractable: false);
				confirmationUI.Show();
			}
		}

		public void OnClickedConfirmationOK()
		{
			confirmationUI.Hide();
			if (skillTreeMode == SkillTreeMode.DETAIL)
			{
				ActivateSkill(_latestSelectedEndPoint, _latestSelectedTree);
				_latestSelectedTree.SetAllEndPointInteractiveState(isInteractable: true);
				EventSystem.current.SetSelectedGameObject(_latestSelectedEndPoint.gameObject);
			}
		}

		public void OnClickedConfirmationCancel()
		{
			confirmationUI.Hide();
			if (skillTreeMode == SkillTreeMode.DETAIL)
			{
				_latestSelectedTree.SetAllEndPointInteractiveState(isInteractable: true);
				EventSystem.current.SetSelectedGameObject(_latestSelectedEndPoint.gameObject);
			}
		}
	}
}
