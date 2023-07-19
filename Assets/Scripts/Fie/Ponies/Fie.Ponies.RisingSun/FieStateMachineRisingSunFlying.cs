using Fie.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	public class FieStateMachineRisingSunFlying : FieStateMachineRisingSunInterface
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
			if (gameCharacter is FieRisingSun)
			{
				FieRisingSun fieRisingSun = gameCharacter as FieRisingSun;
				if (fieRisingSun.groundState == FieObjectGroundState.Grounding)
				{
					_nextState = typeof(FieStateMachinePoniesLanding);
					_isEnd = true;
				}
				else
				{
					if (fieRisingSun.animationManager.IsEndAnimation(0))
					{
						fieRisingSun.animationManager.SetAnimation(20, isLoop: true);
					}
					Vector3 vector = fieRisingSun.externalInputVector;
					_flyingTime -= Time.deltaTime;
					_flyingTime = Mathf.Max(_flyingTime, 0f);
					if (vector.y > 0f)
					{
						vector.y *= 1f * (_flyingTime / 3f);
					}
					vector *= 10f * fieRisingSun.externalInputForce;
					vector.y *= 0.2f;
					vector.y += 11f;
					fieRisingSun.addMoveForce(vector, 1f);
					fieRisingSun.physicalForce.SetPhysicalForce(gameCharacter.getNowMoveForce() * -1f, 1000f);
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
			list.Add(typeof(FieStateMachineRisingSunTeleportation));
			return list;
		}
	}
}
