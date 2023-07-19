using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.NEVER_DESTROY)]
	public sealed class FieInputManager : FieManagerBehaviour<FieInputManager>
	{
		public enum FieInputUIControlMode
		{
			KEYBOARD,
			GAME_PAD
		}

		public enum FieInputUIKeyType
		{
			NONE = 0,
			KEY_TYPE_UP = 1,
			KEY_TYPE_DOWN = 2,
			KEY_TYPE_LEFT = 4,
			KEY_TYPE_RIGHT = 8,
			KEY_TYPE_CANCEL = 0x10,
			KEY_TYPE_DECIDE = 0x20,
			KEY_TYPE_INTERACT = 0x40
		}

		public delegate void FieUIInputDelegate(FieInputUIKeyType keyType);

		private const string fieInputPlayerPrefsBaseKey = "FieInputMap";

		public const int MAXIMUM_LOCAL_PLAYERS = 1;

		private FieInputUIKeyType _currentKeyTypes;

		private FieInputUIControlMode _currentInputUIMode;

		private InputManager _inputManager;

		private Dictionary<int, Player> _players = new Dictionary<int, Player>();

		public bool isEnableControll = true;

		public FieInputUIKeyType currentKeyTypes => _currentKeyTypes;

		public event FieUIInputDelegate uiInputEvent;

		protected override void StartUpEntity()
		{
			if (_inputManager == null)
			{
				_inputManager = UnityEngine.Object.FindObjectOfType<InputManager>();
				if (_inputManager == null)
				{
					GameObject gameObject = Resources.Load("Prefabs/Manager/FieInputMapper") as GameObject;
					if (gameObject == null)
					{
						throw new Exception("Missing the input manager prefab.");
					}
					GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
					if (gameObject2 == null)
					{
						throw new Exception("Fiald to instantiate the input manager object.");
					}
					_inputManager = gameObject2.GetComponent<InputManager>();
					if (_inputManager == null)
					{
						throw new Exception("InputManager component dosen't exists in the imputmanager prefab.");
					}
				}
			}
			_inputManager.transform.parent = base.transform;
			LoadAllMaps();
			for (int i = 0; i < 1; i++)
			{
				_players[i] = ReInput.players.GetPlayer(i);
			}
		}

		public void Update()
		{
			_currentKeyTypes = FieInputUIKeyType.NONE;
			if (GetPlayer().GetButtonDown("Up"))
			{
				_currentKeyTypes |= FieInputUIKeyType.KEY_TYPE_UP;
			}
			else if (GetPlayer().GetButtonDown("Down"))
			{
				_currentKeyTypes |= FieInputUIKeyType.KEY_TYPE_DOWN;
			}
			if (GetPlayer().GetButtonDown("Left"))
			{
				_currentKeyTypes |= FieInputUIKeyType.KEY_TYPE_LEFT;
			}
			else if (GetPlayer().GetButtonDown("Right"))
			{
				_currentKeyTypes |= FieInputUIKeyType.KEY_TYPE_RIGHT;
			}
			if (GetPlayer().GetButtonDown("Cancel"))
			{
				_currentKeyTypes |= FieInputUIKeyType.KEY_TYPE_CANCEL;
			}
			if (GetPlayer().GetButtonDown("Decide"))
			{
				_currentKeyTypes |= FieInputUIKeyType.KEY_TYPE_DECIDE;
			}
			if (_currentKeyTypes != 0 && this.uiInputEvent != null)
			{
				this.uiInputEvent(_currentKeyTypes);
			}
			if (GetPlayer().GetButtonTimePressed("Menu") > 3f)
			{
				Application.Quit();
			}
			if (GetPlayer().GetButtonTimePressed("Restart") > 3f)
			{
				FieManagerFactory.I.Restart();
			}
		}

		public Player GetPlayer(int playerId = 0)
		{
			return _players[playerId];
		}

		public void SetUIControlMode(FieInputUIControlMode mode)
		{
			_currentInputUIMode = mode;
		}

		public FieInputUIControlMode GetUIControlMode()
		{
			return _currentInputUIMode;
		}

		private string GetBasePlayerPrefsKey(Player player)
		{
			string str = "FieInputMap";
			return str + "|playerName=" + player.name;
		}

		private string GetControllerMapPlayerPrefsKey(Player player, ControllerMapSaveData saveData)
		{
			string basePlayerPrefsKey = GetBasePlayerPrefsKey(player);
			basePlayerPrefsKey += "|dataType=ControllerMap";
			basePlayerPrefsKey = basePlayerPrefsKey + "|controllerMapType=" + saveData.mapTypeString;
			string text = basePlayerPrefsKey;
			basePlayerPrefsKey = text + "|categoryId=" + saveData.map.categoryId + "|layoutId=" + saveData.map.layoutId;
			basePlayerPrefsKey = basePlayerPrefsKey + "|hardwareIdentifier=" + saveData.controllerHardwareIdentifier;
			if (saveData.mapType == typeof(JoystickMap))
			{
				basePlayerPrefsKey = basePlayerPrefsKey + "|hardwareGuid=" + ((JoystickMapSaveData)saveData).joystickHardwareTypeGuid.ToString();
			}
			return basePlayerPrefsKey;
		}

		private string GetControllerMapXml(Player player, ControllerType controllerType, int categoryId, int layoutId, Controller controller)
		{
			string basePlayerPrefsKey = GetBasePlayerPrefsKey(player);
			basePlayerPrefsKey += "|dataType=ControllerMap";
			basePlayerPrefsKey = basePlayerPrefsKey + "|controllerMapType=" + controller.mapTypeString;
			string text = basePlayerPrefsKey;
			basePlayerPrefsKey = text + "|categoryId=" + categoryId + "|layoutId=" + layoutId;
			basePlayerPrefsKey = basePlayerPrefsKey + "|hardwareIdentifier=" + controller.hardwareIdentifier;
			if (controllerType == ControllerType.Joystick)
			{
				Joystick joystick = (Joystick)controller;
				basePlayerPrefsKey = basePlayerPrefsKey + "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString();
			}
			if (!PlayerPrefs.HasKey(basePlayerPrefsKey))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(basePlayerPrefsKey);
		}

		private List<string> GetAllControllerMapsXml(Player player, bool userAssignableMapsOnly, ControllerType controllerType, Controller controller)
		{
			List<string> list = new List<string>();
			IList<InputMapCategory> mapCategories = ReInput.mapping.MapCategories;
			for (int i = 0; i < mapCategories.Count; i++)
			{
				InputMapCategory inputMapCategory = mapCategories[i];
				if (!userAssignableMapsOnly || inputMapCategory.userAssignable)
				{
					IList<InputLayout> list2 = ReInput.mapping.MapLayouts(controllerType);
					for (int j = 0; j < list2.Count; j++)
					{
						InputLayout inputLayout = list2[j];
						string controllerMapXml = GetControllerMapXml(player, controllerType, inputMapCategory.id, inputLayout.id, controller);
						if (!(controllerMapXml == string.Empty))
						{
							list.Add(controllerMapXml);
						}
					}
				}
			}
			return list;
		}

		private string GetJoystickCalibrationMapPlayerPrefsKey(JoystickCalibrationMapSaveData saveData)
		{
			string str = "FieInputMap";
			str += "|dataType=CalibrationMap";
			str = str + "|controllerType=" + saveData.controllerType.ToString();
			str = str + "|hardwareIdentifier=" + saveData.hardwareIdentifier;
			return str + "|hardwareGuid=" + saveData.joystickHardwareTypeGuid.ToString();
		}

		private string GetJoystickCalibrationMapXml(Joystick joystick)
		{
			string str = "FieInputMap";
			str += "|dataType=CalibrationMap";
			str = str + "|controllerType=" + joystick.type.ToString();
			str = str + "|hardwareIdentifier=" + joystick.hardwareIdentifier;
			str = str + "|hardwareGuid=" + joystick.hardwareTypeGuid.ToString();
			if (!PlayerPrefs.HasKey(str))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(str);
		}

		private string GetInputBehaviorPlayerPrefsKey(Player player, InputBehavior saveData)
		{
			string basePlayerPrefsKey = GetBasePlayerPrefsKey(player);
			basePlayerPrefsKey += "|dataType=InputBehavior";
			return basePlayerPrefsKey + "|id=" + saveData.id;
		}

		private string GetInputBehaviorXml(Player player, int id)
		{
			string basePlayerPrefsKey = GetBasePlayerPrefsKey(player);
			basePlayerPrefsKey += "|dataType=InputBehavior";
			basePlayerPrefsKey = basePlayerPrefsKey + "|id=" + id;
			if (!PlayerPrefs.HasKey(basePlayerPrefsKey))
			{
				return string.Empty;
			}
			return PlayerPrefs.GetString(basePlayerPrefsKey);
		}

		public void SaveAllMaps()
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				PlayerSaveData saveData = player.GetSaveData(userAssignableMapsOnly: true);
				InputBehavior[] inputBehaviors = saveData.inputBehaviors;
				foreach (InputBehavior inputBehavior in inputBehaviors)
				{
					string inputBehaviorPlayerPrefsKey = GetInputBehaviorPlayerPrefsKey(player, inputBehavior);
					PlayerPrefs.SetString(inputBehaviorPlayerPrefsKey, inputBehavior.ToXmlString());
				}
				foreach (ControllerMapSaveData allControllerMapSaveDatum in saveData.AllControllerMapSaveData)
				{
					string controllerMapPlayerPrefsKey = GetControllerMapPlayerPrefsKey(player, allControllerMapSaveDatum);
					PlayerPrefs.SetString(controllerMapPlayerPrefsKey, allControllerMapSaveDatum.map.ToXmlString());
				}
			}
			foreach (Joystick joystick in ReInput.controllers.Joysticks)
			{
				JoystickCalibrationMapSaveData calibrationMapSaveData = joystick.GetCalibrationMapSaveData();
				string joystickCalibrationMapPlayerPrefsKey = GetJoystickCalibrationMapPlayerPrefsKey(calibrationMapSaveData);
				PlayerPrefs.SetString(joystickCalibrationMapPlayerPrefsKey, calibrationMapSaveData.map.ToXmlString());
			}
			PlayerPrefs.Save();
		}

		public void LoadAllMaps()
		{
			IList<Player> allPlayers = ReInput.players.AllPlayers;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				Player player = allPlayers[i];
				IList<InputBehavior> inputBehaviors = ReInput.mapping.GetInputBehaviors(player.id);
				for (int j = 0; j < inputBehaviors.Count; j++)
				{
					string inputBehaviorXml = GetInputBehaviorXml(player, inputBehaviors[j].id);
					if (inputBehaviorXml != null && !(inputBehaviorXml == string.Empty))
					{
						inputBehaviors[j].ImportXmlString(inputBehaviorXml);
					}
				}
				List<string> allControllerMapsXml = GetAllControllerMapsXml(player, userAssignableMapsOnly: true, ControllerType.Keyboard, ReInput.controllers.Keyboard);
				List<string> allControllerMapsXml2 = GetAllControllerMapsXml(player, userAssignableMapsOnly: true, ControllerType.Mouse, ReInput.controllers.Mouse);
				bool flag = false;
				List<List<string>> list = new List<List<string>>();
				foreach (Joystick joystick in player.controllers.Joysticks)
				{
					List<string> allControllerMapsXml3 = GetAllControllerMapsXml(player, userAssignableMapsOnly: true, ControllerType.Joystick, joystick);
					list.Add(allControllerMapsXml3);
					if (allControllerMapsXml3.Count > 0)
					{
						flag = true;
					}
				}
				if (allControllerMapsXml.Count > 0)
				{
					player.controllers.maps.ClearMaps(ControllerType.Keyboard, userAssignableOnly: true);
				}
				player.controllers.maps.AddMapsFromXml(ControllerType.Keyboard, 0, allControllerMapsXml);
				if (flag)
				{
					player.controllers.maps.ClearMaps(ControllerType.Joystick, userAssignableOnly: true);
				}
				int num = 0;
				foreach (Joystick joystick2 in player.controllers.Joysticks)
				{
					player.controllers.maps.AddMapsFromXml(ControllerType.Joystick, joystick2.id, list[num]);
					num++;
				}
				if (allControllerMapsXml2.Count > 0)
				{
					player.controllers.maps.ClearMaps(ControllerType.Mouse, userAssignableOnly: true);
				}
				player.controllers.maps.AddMapsFromXml(ControllerType.Mouse, 0, allControllerMapsXml2);
			}
			foreach (Joystick joystick3 in ReInput.controllers.Joysticks)
			{
				joystick3.ImportCalibrationMapFromXmlString(GetJoystickCalibrationMapXml(joystick3));
			}
		}
	}
}
