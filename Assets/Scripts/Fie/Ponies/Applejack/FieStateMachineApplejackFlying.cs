using Fie.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackFlying : FieStateMachineGameCharacterBase
	{
		private const float FLYING_MOVE_FORCE_ROUND = 0.5f;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieApplejack)
			{
				FieApplejack fieApplejack = gameCharacter as FieApplejack;
				Vector3 externalInputVector = fieApplejack.externalInputVector;
				float externalInputForce = fieApplejack.externalInputForce;
				externalInputVector.y = (externalInputVector.z = 0f);
				if (fieApplejack.animationManager.IsEndAnimation(0))
				{
					fieApplejack.animationManager.SetAnimation(20, isLoop: true);
				}
				if (fieApplejack.groundState == FieObjectGroundState.Grounding)
				{
					_nextState = typeof(FieStateMachinePoniesLanding);
					_isEnd = true;
				}
				else
				{
					gameCharacter.addMoveForce(externalInputVector * (fieApplejack.getDefaultMoveSpeed() * fieApplejack.externalInputForce) * 0.5f, 0.4f);
					fieApplejack.physicalForce.SetPhysicalForce(gameCharacter.getNowMoveForce() * -1f, 1500f);
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.setGravityRate(1f);
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = true;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.setGravityRate(1f);
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = true;
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
			list.Add(typeof(FieStateMachineApplejackFireAir));
			list.Add(typeof(FieStateMachineApplejackFireRope));
			list.Add(typeof(FieStateMachineApplejackRopeActionAir));
			list.Add(typeof(FieStateMachineApplejackYeehawActionMidAir));
			list.Add(typeof(FieStateMachineApplejackStompAction));
			return list;
		}
	}
}
