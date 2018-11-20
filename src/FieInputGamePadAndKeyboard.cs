using Fie.Camera;
using Fie.Manager;
using Fie.Object;
using Fie.Ponies;
using UnityEngine;

public class FieInputGamePadAndKeyboard : FieInputControllerBase
{
	private enum KeyTypes
	{
		NONE = 0,
		EVASION = 1,
		BASE_ATTACK = 2,
		JUMP = 4,
		ABILITIES_1 = 8,
		ABILITIES_2 = 0x10,
		ABILITIES_3 = 0x20,
		INTERACT = 0x40,
		SWITCHED_LOCKON_MODE = 0x80,
		MENU = 0x100,
		RESTART = 0x200
	}

	private struct KeyDownInfo
	{
		public KeyTypes keyType;

		public float downCount;

		public KeyDownInfo(KeyTypes keyType, float downCount)
		{
			this.keyType = keyType;
			this.downCount = downCount;
		}
	}

	private const float TARGET_CHANGE_AXIS_THRESHOLD = 0.5f;

	private const float HORIZONTAL_MOVE_AXIS_THRESHOLD = 0.3f;

	private const float HORIZONTAL_GALLOP_AXIS_THRESHOLD = 0.7f;

	private const float FIRE_TRIGGER_AXIS_THRESHOLD = 0.3f;

	private const float TARGET_MAKER_MOVE_AXIS_THRESHOLD = 0.05f;

	private const float TARGET_MAKER_MOVE_SPEED_PER_SEC = 6f;

	private KeyTypes _keyTypes;

	private KeyDownInfo[] _keyDownCounts = new KeyDownInfo[11]
	{
		new KeyDownInfo(KeyTypes.NONE, 0f),
		new KeyDownInfo(KeyTypes.EVASION, 0f),
		new KeyDownInfo(KeyTypes.BASE_ATTACK, 0f),
		new KeyDownInfo(KeyTypes.JUMP, 0f),
		new KeyDownInfo(KeyTypes.ABILITIES_1, 0f),
		new KeyDownInfo(KeyTypes.ABILITIES_2, 0f),
		new KeyDownInfo(KeyTypes.ABILITIES_3, 0f),
		new KeyDownInfo(KeyTypes.INTERACT, 0f),
		new KeyDownInfo(KeyTypes.SWITCHED_LOCKON_MODE, 0f),
		new KeyDownInfo(KeyTypes.MENU, 0f),
		new KeyDownInfo(KeyTypes.RESTART, 0f)
	};

	private KeyTypes[] _keyTypeList = new KeyTypes[11]
	{
		KeyTypes.NONE,
		KeyTypes.EVASION,
		KeyTypes.BASE_ATTACK,
		KeyTypes.JUMP,
		KeyTypes.ABILITIES_1,
		KeyTypes.ABILITIES_2,
		KeyTypes.ABILITIES_3,
		KeyTypes.INTERACT,
		KeyTypes.SWITCHED_LOCKON_MODE,
		KeyTypes.MENU,
		KeyTypes.RESTART
	};

	private float _rightHorizontalAxisPower;

	private float _rightVerticalAxisPower;

	private Vector3 _leftAxisVector = Vector3.zero;

	private Vector3 _rightAxisVector = Vector3.zero;

	private float _rightAxisCount;

	private float _leftAxisInputForce;

	private float _rightAxisInputForce;

	private bool _isMouseViewMode;

	private float _timeScale = 1f;

	private void Start()
	{
	}

	private Vector3 getMovemenetAxis()
	{
		Vector3 zero = Vector3.zero;
		zero.x = FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetAxis("Move Horizontal");
		zero.y = FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetAxis("Move Vertical");
		zero.x = Mathf.Clamp(zero.x, -1f, 1f);
		zero.y = Mathf.Clamp(zero.y, -1f, 1f);
		return zero;
	}

	private void Update()
	{
		if (!(base.ownerCharacter == null))
		{
			if (!FieManagerBehaviour<FieInputManager>.I.isEnableControll)
			{
				base.ownerCharacter.SetExternalForces(Vector3.zero, 0f);
			}
			else
			{
				updateKeyFlags();
				_rightHorizontalAxisPower = FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetAxis("Camera Horizontal");
				_rightVerticalAxisPower = FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetAxis("Camera Vertical");
				_leftAxisVector = getMovemenetAxis();
				_leftAxisInputForce = Mathf.Clamp(Vector3.Distance(b: new Vector3(_leftAxisVector.x, _leftAxisVector.y), a: Vector3.zero), 0f, 1f);
				_leftAxisVector.Normalize();
				_rightAxisVector = new Vector3(_rightHorizontalAxisPower, _rightVerticalAxisPower, 0f);
				Vector3 b2 = new Vector3(_rightAxisVector.x, _rightAxisVector.y);
				float x = b2.x;
				Vector3 normalized = _rightAxisVector.normalized;
				float min = 0f - Mathf.Abs(normalized.x);
				Vector3 normalized2 = b2.normalized;
				b2.x = Mathf.Clamp(x, min, Mathf.Abs(normalized2.x));
				float y = b2.y;
				Vector3 normalized3 = _rightAxisVector.normalized;
				float min2 = 0f - Mathf.Abs(normalized3.y);
				Vector3 normalized4 = b2.normalized;
				b2.y = Mathf.Clamp(y, min2, Mathf.Abs(normalized4.y));
				_rightAxisInputForce = Mathf.Clamp(Vector3.Distance(Vector3.zero, b2), 0f, 1f);
				_rightAxisVector.Normalize();
				if (_leftAxisInputForce > 0.7f)
				{
					base.ownerCharacter.RequestToChangeState<FieStateMachinePoniesGallop>(_leftAxisVector, _leftAxisInputForce, FieGameCharacter.StateMachineType.Base);
				}
				else if (_leftAxisInputForce > 0.3f)
				{
					base.ownerCharacter.RequestToChangeState<FieStateMachinePoniesMove>(_leftAxisVector, _leftAxisInputForce, FieGameCharacter.StateMachineType.Base);
				}
				if (GetKeyDownCount(KeyTypes.EVASION) > 0f)
				{
					base.ownerCharacter.RequestToChangeState<FieStateMachinePoniesEvasion>(_leftAxisVector, _leftAxisInputForce, FieGameCharacter.StateMachineType.Base);
				}
				if (GetKeyDownCount(KeyTypes.JUMP) > 0f)
				{
					base.ownerCharacter.RequestToChangeState<FieStateMachinePoniesJump>(_leftAxisVector, _leftAxisInputForce, FieGameCharacter.StateMachineType.Base);
				}
				if (FieManagerBehaviour<FieGameCameraManager>.I.gameCamera != null)
				{
					FieGameCameraTaskLockOn fieGameCameraTaskLockOn = FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.GetCameraTask() as FieGameCameraTaskLockOn;
					if (fieGameCameraTaskLockOn != null)
					{
						if (_isMouseViewMode)
						{
							ControllTargetCursorByMouse(fieGameCameraTaskLockOn);
						}
						else if (Input.GetAxis("Mouse X") > 50f * Time.deltaTime || Input.GetAxis("Mouse Y") > 50f * Time.deltaTime)
						{
							_isMouseViewMode = true;
						}
						else
						{
							ControllTargetCursorByPad(fieGameCameraTaskLockOn);
						}
					}
				}
				bool flag = false;
				if (GetKeyDownCount(KeyTypes.ABILITIES_1) > 0f)
				{
					base.ownerCharacter.RequestToChangeState<FieStateMachinePoniesAbilitySlot1>(_leftAxisVector, _leftAxisInputForce, FieGameCharacter.StateMachineType.Attack);
					flag = ((byte)((flag ? 1 : 0) | 1) != 0);
				}
				if (GetKeyDownCount(KeyTypes.ABILITIES_2) > 0f)
				{
					base.ownerCharacter.RequestToChangeState<FieStateMachinePoniesAbilitySlot2>(_leftAxisVector, _leftAxisInputForce, FieGameCharacter.StateMachineType.Attack);
					flag = ((byte)((flag ? 1 : 0) | 1) != 0);
				}
				if (GetKeyDownCount(KeyTypes.ABILITIES_3) > 0f)
				{
					base.ownerCharacter.RequestToChangeState<FieStateMachinePoniesAbilitySlot3>(_leftAxisVector, _leftAxisInputForce, FieGameCharacter.StateMachineType.Attack);
					flag = ((byte)((flag ? 1 : 0) | 1) != 0);
				}
				if (GetKeyDownCount(KeyTypes.BASE_ATTACK) > 0f)
				{
					base.ownerCharacter.RequestToChangeState<FieStateMachinePoniesBaseAttack>(Vector3.zero, 0f, FieGameCharacter.StateMachineType.Attack);
					flag = ((byte)((flag ? 1 : 0) | 1) != 0);
				}
				if (!flag)
				{
					base.ownerCharacter.RequestToChangeState<FieStateMachinePoniesAttackIdle>(Vector3.zero, 0f, FieGameCharacter.StateMachineType.Attack);
				}
				if (GetKeyDownCount(KeyTypes.INTERACT) > 0f)
				{
					FiePonies fiePonies = base.ownerCharacter as FiePonies;
					if (fiePonies != null)
					{
						fiePonies.TryToRevive();
					}
				}
				if (GetKeyDownCount(KeyTypes.MENU) > 3f)
				{
					Application.Quit();
				}
				if (GetKeyDownCount(KeyTypes.RESTART) > 3f)
				{
					FieManagerFactory.I.Restart();
				}
				base.ownerCharacter.SetExternalForces(_leftAxisVector, _leftAxisInputForce);
			}
		}
	}

	private void ControllTargetCursorByMouse(FieGameCameraTaskLockOn lockonTask)
	{
		if (_rightAxisInputForce > 0.05f || GetKeyDownCount(KeyTypes.SWITCHED_LOCKON_MODE) > 0f)
		{
			_isMouseViewMode = false;
		}
		else
		{
			lockonTask.isCameraHorming = false;
			FieGameCamera gameCamera = FieManagerBehaviour<FieGameCameraManager>.I.gameCamera;
			Vector3 mousePosition = Input.mousePosition;
			float x = mousePosition.x;
			Vector3 mousePosition2 = Input.mousePosition;
			gameCamera.SetTargetMakerScreenPos(new Vector3(x, mousePosition2.y, 0f));
			base.ownerCharacter.detector.ChangeLockonTargetFromNearestWorldPosition(FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.camera.ScreenToWorldPoint(FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.tagetMakerScreenPos), 0.5f);
		}
	}

	private void ControllTargetCursorByPad(FieGameCameraTaskLockOn lockonTask)
	{
		if (GetKeyDownCount(KeyTypes.SWITCHED_LOCKON_MODE) > 0f)
		{
			lockonTask.isCameraHorming = !lockonTask.isCameraHorming;
		}
		if (!lockonTask.isCameraHorming)
		{
			if (_rightAxisInputForce > 0.05f)
			{
				FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.AddTargetMakerScreenPos(_rightAxisVector * _rightAxisInputForce * (float)(Screen.height + Screen.width) * GetTargetMakerSpeedMagnify() * Time.deltaTime);
			}
			base.ownerCharacter.detector.ChangeLockonTargetFromNearestWorldPosition(FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.camera.ScreenToWorldPoint(FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.tagetMakerScreenPos), 0.5f);
		}
		else
		{
			if (_rightAxisInputForce > 0.5f && _rightAxisCount <= 0f)
			{
				base.ownerCharacter.detector.ChangeLockonTargetByScreenDirection(FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.camera, _rightAxisVector);
				_rightAxisCount = 0.2f;
			}
			if (_rightAxisCount > 0f)
			{
				_rightAxisCount -= Time.deltaTime;
			}
		}
	}

	private float GetTargetMakerSpeedMagnify()
	{
		return 1f;
	}

	private void LateUpdate()
	{
	}

	private float GetKeyDownCount(KeyTypes _keyType)
	{
		for (int i = 0; i < _keyTypeList.Length; i++)
		{
			if (_keyDownCounts[i].keyType == _keyType)
			{
				return _keyDownCounts[i].downCount;
			}
		}
		return 0f;
	}

	private void updateKeyFlags()
	{
		_keyTypes = KeyTypes.NONE;
		if (FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetButtonDown("Jump"))
		{
			_keyTypes |= KeyTypes.JUMP;
		}
		if (FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetButton("Evasion"))
		{
			_keyTypes |= KeyTypes.EVASION;
		}
		if (FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetButton("Ability1"))
		{
			_keyTypes |= KeyTypes.ABILITIES_1;
		}
		if (FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetButton("Ability2"))
		{
			_keyTypes |= KeyTypes.ABILITIES_2;
		}
		if (FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetButton("Ability3"))
		{
			_keyTypes |= KeyTypes.ABILITIES_3;
		}
		float axis = FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetAxis("Base Attack");
		if (axis > 0.3f)
		{
			_keyTypes |= KeyTypes.BASE_ATTACK;
		}
		if (FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetButtonDown("Switched Lockon Mode"))
		{
			_keyTypes |= KeyTypes.SWITCHED_LOCKON_MODE;
		}
		if (FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetButtonDown("Interact"))
		{
			_keyTypes |= KeyTypes.INTERACT;
		}
		if (FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetButton("Menu"))
		{
			_keyTypes |= KeyTypes.MENU;
		}
		if (FieManagerBehaviour<FieInputManager>.I.GetPlayer().GetButton("Restart"))
		{
			_keyTypes |= KeyTypes.RESTART;
		}
		for (int i = 0; i < _keyTypeList.Length; i++)
		{
			if ((_keyTypes & _keyTypeList[i]) == _keyTypeList[i])
			{
				_keyDownCounts[i].downCount += Time.deltaTime;
			}
			else
			{
				_keyDownCounts[i].downCount = 0f;
			}
		}
	}
}
