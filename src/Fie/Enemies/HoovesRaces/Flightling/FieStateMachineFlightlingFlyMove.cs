using Fie.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.Flightling
{
	public class FieStateMachineFlightlingFlyMove : FieStateMachineGameCharacterBase
	{
		private enum FlightState
		{
			PREPARE,
			DO
		}

		private const float MOVE_THRESHOLD = 0.02f;

		private const float WALK_FORCE_THRESHOLD = 0.3f;

		private const float WALK_MOVESPEED_MAGNI = 0.5f;

		private bool _isEnd;

		private Type _nextState;

		private FlightState state;

		public override void updateState<T>(ref T flightling)
		{
			if (flightling is FieFlightling)
			{
				switch (state)
				{
				default:
					return;
				case FlightState.PREPARE:
					flightling.animationManager.SetAnimation(8, isLoop: true);
					state = FlightState.DO;
					break;
				case FlightState.DO:
					break;
				}
				autoFlipToEnemy(flightling);
				Vector3 externalInputVector = flightling.externalInputVector;
				externalInputVector.z = externalInputVector.y * 0.5f;
				float defaultMoveSpeed = flightling.getDefaultMoveSpeed();
				flightling.addMoveForce(externalInputVector * (defaultMoveSpeed * flightling.externalInputForce), 0.4f);
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = false;
				gameCharacter.isEnableAutoFlip = true;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = false;
			}
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return _nextState;
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineAnyConsider));
			return list;
		}
	}
}
