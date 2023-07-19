using Fie.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightFlying : FieStateMachineGameCharacterBase
	{
		private const float DEFAULT_FLYING_POWER = 11f;

		private const float MAX_FLYING_SPEED = 10f;

		private const float FLYING_UPPER_FORCE_DURATION = 3f;

		private const float LANDING_DELAY = 0.2f;

		private float _flyingTime = 3f;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieTwilight)
			{
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				if (fieTwilight.groundState == FieObjectGroundState.Grounding)
				{
					_nextState = typeof(FieStateMachinePoniesLanding);
					_isEnd = true;
				}
				else
				{
					if (fieTwilight.animationManager.IsEndAnimation(0))
					{
						fieTwilight.animationManager.SetAnimation(20, isLoop: true);
					}
					Vector3 vector = fieTwilight.externalInputVector;
					_flyingTime -= Time.deltaTime;
					_flyingTime = Mathf.Max(_flyingTime, 0f);
					if (vector.y > 0f)
					{
						vector.y *= 1f * (_flyingTime / 3f);
					}
					vector *= 10f * fieTwilight.externalInputForce;
					vector.y *= 0.2f;
					vector.y += 11f;
					fieTwilight.addMoveForce(vector, 1f);
					fieTwilight.physicalForce.SetPhysicalForce(gameCharacter.getNowMoveForce() * -1f, 1000f);
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.setGravityRate(0.5f);
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
				gameCharacter.setGravityRate(1f);
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
			list.Add(typeof(FieStateMachinePoniesEvasion));
			list.Add(typeof(FieStateMachineTwilightTeleportation));
			list.Add(typeof(FieStateMachineTwilightSparklyCannonShooting));
			return list;
		}
	}
}
