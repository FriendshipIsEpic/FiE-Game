using Fie.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesMove : FieStateMachineGameCharacterBase
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
					float num = 0.38f;
					Vector3 velocity = gameCharacter.GetComponent<Rigidbody>().velocity;
					float num2 = Math.Abs(velocity.x);
					float num3 = num2 / 5f;
					float defaultMoveSpeed = gameCharacter.getDefaultMoveSpeed();
					_physicalSinWave += 3f * Time.deltaTime;
					float timescale = Mathf.Min(Mathf.Max(num3 / 0.1f, 0.25f), 1f);
					gameCharacter.animationManager.SetAnimation(2, isLoop: true);
					gameCharacter.animationManager.SetAnimationTimescale(2, timescale);
					_nowMoving = true;
					gameCharacter.addMoveForce(externalInputVector * (defaultMoveSpeed * num), 0.1f);
					Vector3 normalVec = (gameCharacter.getNowMoveForce().normalized + Vector3.down * (0.75f * Mathf.Sin(_physicalSinWave))) * -1f;
					fiePonies.physicalForce.SetPhysicalForce(normalVec, 1000f, 0.5f);
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
