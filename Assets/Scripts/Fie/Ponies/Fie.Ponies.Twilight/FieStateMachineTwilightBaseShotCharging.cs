using Fie.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightBaseShotCharging : FieStateMachineGameCharacterBase
	{
		private const float BASE_SHOT_DELAY = 0.25f;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd)
			{
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				if (!(fieTwilight == null))
				{
					float num = fieTwilight.healthStats.maxShield * (0.1f * fieTwilight.baseAttackConsumeShieldRate);
					fieTwilight.baseAttackChargedForce += Time.deltaTime * fieTwilight.baseAttackChargingTimeRate;
					fieTwilight.damageSystem.calcShieldDirect((0f - num) * Time.deltaTime);
					fieTwilight.damageSystem.setRegenerateDelay(fieTwilight.healthStats.regenerateDelay * 0.2f, roundToBigger: true);
					if (fieTwilight.healthStats.shield <= 0f)
					{
						_isEnd = true;
					}
				}
			}
		}

		public override float getDelay()
		{
			return 0.25f;
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachinePoniesAttackIdle));
			list.Add(typeof(FieStateMachineTwilightBaseShotActivator));
			list.Add(typeof(FieStateMachineTwilightForceField));
			list.Add(typeof(FieStateMachineTwilightSparklyCannon));
			list.Add(typeof(FieStateMachineTwilightSummonArrow));
			return list;
		}

		public override Type getNextState()
		{
			return typeof(FieStateMachineTwilightBaseShotActivator);
		}
	}
}
