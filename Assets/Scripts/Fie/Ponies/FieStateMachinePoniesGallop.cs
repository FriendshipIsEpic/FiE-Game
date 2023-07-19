using Fie.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesGallop : FieStateMachineGameCharacterBase
	{
		private const float MOVE_THRESHOLD = 0.05f;

		private const float WALK_FORCE_THRESHOLD = 0.2f;

		private const float WALK_MOVESPEED_MAGNI = 0.5f;

		private bool _isEnd;

		private Type _nextState;

		private float _physicalSinWave;

		private bool _nowMoving;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FiePonies)
			{
				if (gameCharacter.externalInputForce <= 0.3f)
				{
					_isEnd = true;
					_nextState = typeof(FieStateMachinePoniesIdle);
				}
				else
				{
					FiePonies fiePonies = gameCharacter as FiePonies;
					Vector3 externalInputVector = gameCharacter.externalInputVector;
					externalInputVector.z = externalInputVector.y * 0.5f;
					externalInputVector.y = 0f;
					Vector3 velocity = gameCharacter.GetComponent<Rigidbody>().velocity;
					float num = Math.Abs(velocity.x);
					float num2 = num / 5f;
					float defaultMoveSpeed = gameCharacter.getDefaultMoveSpeed();
					_physicalSinWave += 12.5f * Time.deltaTime;
					gameCharacter.animationManager.SetAnimation(3, isLoop: true);
					gameCharacter.addMoveForce(externalInputVector * (defaultMoveSpeed * gameCharacter.externalInputForce), 0.4f);
					Vector3 normalVec = (gameCharacter.getNowMoveForce().normalized + Vector3.down * (0.65f * Mathf.Sin(_physicalSinWave))) * -1f;
					fiePonies.physicalForce.SetPhysicalForce(normalVec, 2000f * num2, 0.5f);
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.animationManager.ResetAllAnimationTimescale();
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
