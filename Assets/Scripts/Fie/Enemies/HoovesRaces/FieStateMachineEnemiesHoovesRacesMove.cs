using Fie.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces
{
	public class FieStateMachineEnemiesHoovesRacesMove : FieStateMachineGameCharacterBase
	{
		private const float MOVE_THRESHOLD = 0.02f;

		private const float WALK_FORCE_THRESHOLD = 0.3f;

		private const float WALK_MOVESPEED_MAGNI = 0.5f;

		private bool _isEnd;

		private Type _nextState;

		private float _physicalSinWave;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieEnemiesHoovesRaces)
			{
				Vector3 externalInputVector = gameCharacter.externalInputVector;
				externalInputVector.z = externalInputVector.y * 0.5f;
				externalInputVector.y = 0f;
				float externalInputForce = gameCharacter.externalInputForce;
				Vector3 velocity = gameCharacter.GetComponent<Rigidbody>().velocity;
				float num = Math.Abs(velocity.x);
				float num2 = num / 5f;
				float num3 = gameCharacter.getDefaultMoveSpeed();
				_physicalSinWave += 10f * Time.deltaTime;
				if (num2 <= 0.02f)
				{
					_physicalSinWave = 0f;
				}
				else if (num2 <= 0.3f)
				{
					float timescale = Mathf.Min(Mathf.Max(num2 / 0.1f, 0.25f), 1.5f);
					gameCharacter.animationManager.SetAnimation(1, isLoop: true);
					gameCharacter.animationManager.SetAnimationTimescale(1, timescale);
					if (externalInputForce < 0.6f)
					{
						num3 *= 0.5f;
					}
				}
				else
				{
					float timescale2 = Mathf.Min(Mathf.Max(num2, 1f) / 0.5f, 1f);
					gameCharacter.animationManager.SetAnimation(2, isLoop: true);
					gameCharacter.animationManager.SetAnimationTimescale(2, timescale2);
				}
				gameCharacter.addMoveForce(externalInputVector * (num3 * gameCharacter.externalInputForce), 0.4f);
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
