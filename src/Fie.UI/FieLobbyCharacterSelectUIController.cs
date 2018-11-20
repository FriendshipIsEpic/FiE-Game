using Fie.Camera;
using Fie.Manager;
using Fie.Ponies.Applejack;
using Fie.Ponies.RainbowDash;
using Fie.Ponies.RisingSun;
using Fie.Ponies.Twilight;
using Fie.User;
using Fie.Utility;
using GameDataEditor;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Fie.UI
{
	public class FieLobbyCharacterSelectUIController : MonoBehaviour
	{
		public enum SelectableElementType
		{
			Magic,
			Kindness,
			Laghter,
			Loyalty,
			Generosity,
			Honestly,
			Maximum_Element,
			PrincessRisingSun
		}

		private delegate void ResouceLoadedCallback<T>(T gameCharacter) where T : FieGameCharacter;

		public bool isEnable;

		public float lookingEaseTime = 0.5f;

		public float enableGemLightIntensity = 3f;

		public float enableGemLightBlinkInterval = 1f;

		private bool _gemLightAnimationInverseFlag;

		private bool _initializedFirstTime;

		[SerializeField]
		private bool[] existFlagList = new bool[6];

		[SerializeField]
		private Transform[] gemTransformList = new Transform[6];

		[SerializeField]
		private Light[] gemLightList = new Light[6];

		[SerializeField]
		private TextMeshPro elementNameUITextMesh;

		private SelectableElementType _beforeElement;

		private SelectableElementType _currentElement;

		private Tweener<TweenTypesOutSine> _lookAtElementTweener = new Tweener<TweenTypesOutSine>();

		private Tweener<TweenTypesInOutSine> _lightIntesityTweener = new Tweener<TweenTypesInOutSine>();

		private Vector3 _beforeLookAtPosition = Vector3.zero;

		private Vector3 _nowLookAtPosition = Vector3.zero;

		private IEnumerator AsyncLoadCharacterResouce<T>(ResouceLoadedCallback<T> callback) where T : FieGameCharacter
		{
			FiePrefabInfo existsAttribute = (FiePrefabInfo)Attribute.GetCustomAttribute(typeof(T), typeof(FiePrefabInfo));
			if (existsAttribute == null)
			{
				callback((T)null);
			}
			else
			{
				ResourceRequest loadRequest = Resources.LoadAsync(existsAttribute.path);
				float time = 0f;
				if (time < 3f && !loadRequest.isDone)
				{
					float num = time + Time.deltaTime;
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
				GameObject loadObject = loadRequest.asset as GameObject;
				if (loadObject == null)
				{
					callback((T)null);
				}
				else
				{
					T gameCharacter = loadObject.GetComponent<T>();
					if ((UnityEngine.Object)gameCharacter == (UnityEngine.Object)null)
					{
						callback((T)null);
					}
					else
					{
						callback(gameCharacter);
					}
				}
			}
		}

		private void Start()
		{
			UpdateTextMesh();
		}

		private void Update()
		{
		}

		private void UpdateElementActivity()
		{
			switch (_currentElement)
			{
			case SelectableElementType.Maximum_Element:
				break;
			case SelectableElementType.Magic:
				FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_MAGIC_DESCRIPTION), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_MAGIC_DESCRIPTION), 30f);
				break;
			case SelectableElementType.Kindness:
				FieManagerBehaviour<FieActivityManager>.I.RequestToHideActivity();
				break;
			case SelectableElementType.Laghter:
				FieManagerBehaviour<FieActivityManager>.I.RequestToHideActivity();
				break;
			case SelectableElementType.Loyalty:
				FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_LOYALTY_DESCRIPTION), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_LOYALTY_DESCRIPTION), 30f);
				break;
			case SelectableElementType.Generosity:
				FieManagerBehaviour<FieActivityManager>.I.RequestToHideActivity();
				break;
			case SelectableElementType.Honestly:
				FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ELE_HONESTY_DESCRIPTION), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ELE_HONESTY_DESCRIPTION), 30f);
				break;
			case SelectableElementType.PrincessRisingSun:
				FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_RISING_SIUN_DESCRIPTION), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_RISING_SUN_DESCRIPTION), 30f);
				break;
			}
		}

		private void UpdateTextMesh()
		{
			if (!(elementNameUITextMesh == null))
			{
				string key = null;
				switch (_currentElement)
				{
				case SelectableElementType.Magic:
					key = GDEItemKeys.ConstantTextList_ELEMENT_NAME_MAGIC;
					break;
				case SelectableElementType.Kindness:
					key = GDEItemKeys.ConstantTextList_ELEMENT_NAME_KINDNESS;
					break;
				case SelectableElementType.Laghter:
					key = GDEItemKeys.ConstantTextList_ELEMENT_NAME_LAUGHTER;
					break;
				case SelectableElementType.Loyalty:
					key = GDEItemKeys.ConstantTextList_ELEMENT_NAME_LOYALTY;
					break;
				case SelectableElementType.Generosity:
					key = GDEItemKeys.ConstantTextList_ELEMENT_NAME_GENEROSITY;
					break;
				case SelectableElementType.Honestly:
					key = GDEItemKeys.ConstantTextList_ELEMENT_NAME_HONESTY;
					break;
				case SelectableElementType.PrincessRisingSun:
					key = GDEItemKeys.ConstantTextList_ELEMENT_NAME_PONICO;
					break;
				}
				GDEConstantTextListData constantTextData;
				string constantText = FieLocalizeUtility.GetConstantText(key, out constantTextData);
				elementNameUITextMesh.font = ((!constantTextData.ForceEnableToUseEnglishFont) ? FieManagerBehaviour<FieEnvironmentManager>.I.currentFont : FieManagerBehaviour<FieEnvironmentManager>.I.englishFont);
				elementNameUITextMesh.text = constantText;
				elementNameUITextMesh.ForceMeshUpdate();
			}
		}

		public void IncreaseElementIndex()
		{
			int num = (int)(_currentElement + 1);
			if (num >= 6)
			{
				num = 0;
			}
			_currentElement = (SelectableElementType)num;
		}

		public void DecreaseElementIndex()
		{
			int num = (int)(_currentElement - 1);
			if (num < 0)
			{
				num = 5;
			}
			_currentElement = (SelectableElementType)num;
		}

		public bool isDecidable()
		{
			return existFlagList[(int)_currentElement];
		}

		public void Decide()
		{
			FieLobbyGameCharacterGenerateManager.LobbyGameCharacterCreatedCallback lobbyCallback = delegate
			{
				isEnable = false;
				FieManagerBehaviour<FieGameCameraManager>.I.setDefaultCameraOffset(new Vector3(0f, 0.75f, -5f));
				FieManagerBehaviour<FieGameCameraManager>.I.setDefaultCameraRotation(new Vector3(354f, 0f, 0f));
				FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.SetCameraTask<FieGameCameraTaskStop>(1f);
			};
			int userNumberByHash = FieManagerBehaviour<FieUserManager>.I.getUserNumberByHash(FieManagerBehaviour<FieUserManager>.I.myHash);
			FieUser userData = FieManagerBehaviour<FieUserManager>.I.GetUserData(userNumberByHash);
			FieManagerBehaviour<FieActivityManager>.I.RequestToHideActivity();
			switch (_currentElement)
			{
			case SelectableElementType.Kindness:
				break;
			case SelectableElementType.Laghter:
				break;
			case SelectableElementType.Generosity:
				break;
			case SelectableElementType.Maximum_Element:
				break;
			case SelectableElementType.Magic:
			{
				ResouceLoadedCallback<FieTwilight> callback4 = delegate
				{
					FieManagerBehaviour<FieLobbyGameCharacterGenerateManager>.I.RequestToCreateGameCharacter<FieTwilight>(userData, lobbyCallback);
				};
				StartCoroutine(AsyncLoadCharacterResouce(callback4));
				break;
			}
			case SelectableElementType.Loyalty:
			{
				ResouceLoadedCallback<FieRainbowDash> callback3 = delegate
				{
					FieManagerBehaviour<FieLobbyGameCharacterGenerateManager>.I.RequestToCreateGameCharacter<FieRainbowDash>(userData, lobbyCallback);
				};
				StartCoroutine(AsyncLoadCharacterResouce(callback3));
				break;
			}
			case SelectableElementType.Honestly:
			{
				ResouceLoadedCallback<FieApplejack> callback2 = delegate
				{
					FieManagerBehaviour<FieLobbyGameCharacterGenerateManager>.I.RequestToCreateGameCharacter<FieApplejack>(userData, lobbyCallback);
				};
				StartCoroutine(AsyncLoadCharacterResouce(callback2));
				break;
			}
			case SelectableElementType.PrincessRisingSun:
			{
				ResouceLoadedCallback<FieRisingSun> callback = delegate
				{
					FieManagerBehaviour<FieLobbyGameCharacterGenerateManager>.I.RequestToCreateGameCharacter<FieRisingSun>(userData, lobbyCallback);
				};
				StartCoroutine(AsyncLoadCharacterResouce(callback));
				break;
			}
			}
		}
	}
}
