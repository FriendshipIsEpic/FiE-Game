using Fie.Object;
using System;
using System.Collections.Generic;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightAttackIdle : FieStateMachineGameCharacterBase
	{
		private Type _nextState;

		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd)
			{
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				if (!(fieTwilight == null) && fieTwilight.baseAttackChargedForce > 0f)
				{
					_nextState = typeof(FieStateMachineTwilightBaseShotActivator);
					_isEnd = true;
				}
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
			list.Add(typeof(FieStateMachineTwilightBaseShotCharging));
			list.Add(typeof(FieStateMachineTwilightForceField));
			list.Add(typeof(FieStateMachineTwilightSparklyCannon));
			list.Add(typeof(FieStateMachineTwilightSummonArrow));
			return list;
		}

		public override bool isNotNetworkSync()
		{
			return true;
		}
	}
}
