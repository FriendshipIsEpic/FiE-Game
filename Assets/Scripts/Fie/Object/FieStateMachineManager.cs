using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Object
{
	public class FieStateMachineManager
	{
		public delegate bool FieStateMachineActivateCheckDelegate();

		private class TypeByParams
		{
			public Type type;

			public float delay;

			public FieStateMachineActivateCheckDelegate checkDelegate;
		}

		private const int MAXIMUM_STATE_MACHINE_PER_MANAGER = 64;

		private FieGameCharacter _fieGameCharacter;

		private FieGameCharacter.StateMachineType _stateMachineType;

		private FieStateMachineInterface _currentStateMachine;

		private TypeByParams[] _typeByParams = new TypeByParams[64];

		public FieGameCharacter.StateMachineType stateMachineType => _stateMachineType;

		public FieStateMachineManager(FieGameCharacter gameCharacter, FieGameCharacter.StateMachineType type)
		{
			for (int i = 0; i < 64; i++)
			{
				_typeByParams[i] = new TypeByParams();
			}
			_fieGameCharacter = gameCharacter;
			_stateMachineType = type;
			_currentStateMachine = _fieGameCharacter.getDefaultState(_stateMachineType);
		}

		private TypeByParams GetTypeByParam(Type type)
		{
			for (int i = 0; i < 64; i++)
			{
				if (_typeByParams[i].type == type)
				{
					return _typeByParams[i];
				}
			}
			TypeByParams typeByParams = null;
			for (int j = 0; j < 64; j++)
			{
				if (_typeByParams[j].type == null)
				{
					typeByParams = _typeByParams[j];
					typeByParams.type = type;
					break;
				}
			}
			return typeByParams;
		}

		private void SetTypeByDelay(Type type, float delay = 0f)
		{
			TypeByParams typeByParam = GetTypeByParam(type);
			if (typeByParam != null)
			{
				typeByParam.delay = delay;
			}
		}

		private void SetTypeByActivateCheckDelegate(Type type, FieStateMachineActivateCheckDelegate callback)
		{
			TypeByParams typeByParam = GetTypeByParam(type);
			if (typeByParam != null)
			{
				typeByParam.checkDelegate = callback;
			}
		}

		public void SetActivateCheckEvent<T>(FieStateMachineActivateCheckDelegate activateCheckCallback) where T : FieStateMachineInterface
		{
			SetTypeByActivateCheckDelegate(typeof(T), activateCheckCallback);
		}

		public FieStateMachineInterface setState(Type state, bool isForceSet, bool isDupulicate = false)
		{
			Type entityState = _fieGameCharacter.getEntityState(state);
			if (entityState == null)
			{
				return null;
			}
			if (!isDupulicate && entityState == _currentStateMachine.GetType())
			{
				return null;
			}
			TypeByParams typeByParam = GetTypeByParam(entityState);
			if (typeByParam != null && typeByParam.checkDelegate != null && !typeByParam.checkDelegate())
			{
				return null;
			}
			if (!isForceSet)
			{
				List<Type> allowedStateList = _currentStateMachine.getAllowedStateList();
				if (!allowedStateList.Contains(typeof(FieStateMachineAnyConsider)) && !allowedStateList.Contains(entityState))
				{
					return null;
				}
				if (getStateDelay(entityState) > 0f)
				{
					return null;
				}
			}
			return _fieGameCharacter.sendStateChangeCommand(this, entityState);
		}

		public FieStateMachineInterface SetStateDynamic(Type nextStateType)
		{
			FieStateMachineInterface fieStateMachineInterface = Activator.CreateInstance(nextStateType) as FieStateMachineInterface;
			if (fieStateMachineInterface == null)
			{
				return null;
			}
			SetTypeByDelay(_currentStateMachine.GetType(), _currentStateMachine.getDelay());
			_currentStateMachine.terminateAndCallStateChangeEvent(_fieGameCharacter, _currentStateMachine.GetType(), fieStateMachineInterface.GetType());
			_currentStateMachine = fieStateMachineInterface;
			_currentStateMachine.initialize(_fieGameCharacter);
			_currentStateMachine.updateState(ref _fieGameCharacter);
			return _currentStateMachine;
		}

		public void updateState()
		{
			if (_currentStateMachine.isEnd())
			{
				Type nextState = _currentStateMachine.getNextState();
				if (nextState != null)
				{
					setState(nextState, isForceSet: true);
				}
				else
				{
					SetTypeByDelay(_currentStateMachine.GetType(), _currentStateMachine.getDelay());
					_currentStateMachine.terminateAndCallStateChangeEvent(_fieGameCharacter, _currentStateMachine.GetType(), _fieGameCharacter.getDefaultState(_stateMachineType).GetType());
					_currentStateMachine = _fieGameCharacter.getDefaultState(_stateMachineType);
					_currentStateMachine.initialize(_fieGameCharacter);
				}
			}
			_currentStateMachine.updateState(ref _fieGameCharacter);
			for (int i = 0; i < _typeByParams.Length; i++)
			{
				_typeByParams[i].delay = Mathf.Max(0f, _typeByParams[i].delay - Time.deltaTime);
			}
		}

		public float getStateDelay(Type state)
		{
			if (state == null)
			{
				return 0f;
			}
			return GetTypeByParam(state)?.delay ?? 0f;
		}

		public FieStateMachineInterface getCurrentStateMachine()
		{
			return _currentStateMachine;
		}

		public Type nowStateType()
		{
			return _currentStateMachine.GetType();
		}
	}
}
