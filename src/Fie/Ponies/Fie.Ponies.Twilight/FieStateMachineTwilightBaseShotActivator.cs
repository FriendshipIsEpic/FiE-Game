using Fie.Object;
using System;
using System.Collections.Generic;

namespace Fie.Ponies.Twilight
{
	public class FieStateMachineTwilightBaseShotActivator : FieStateMachineGameCharacterBase
	{
		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineTwilightAttackIdle);

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd)
			{
				FieTwilight fieTwilight = gameCharacter as FieTwilight;
				if (!(fieTwilight == null))
				{
					if (fieTwilight.baseAttackChargedForce <= 0f)
					{
						_isEnd = true;
					}
					else if (FieTwilight.ignoreAttackState.Contains(fieTwilight.getStateMachine().nowStateType()))
					{
						_isEnd = true;
					}
					else
					{
						_nextState = typeof(FieStateMachineTwilightBaseShotLevel1);
						int chargedCount = fieTwilight.chargedCount;
						if (chargedCount >= 2)
						{
							_nextState = typeof(FieStateMachineTwilightBaseShotLevel3);
						}
						else if (chargedCount == 1)
						{
							_nextState = typeof(FieStateMachineTwilightBaseShotLevel2);
						}
						_isEnd = true;
						fieTwilight.baseAttackChargedForce = 0f;
					}
				}
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieTwilight x = gameCharacter as FieTwilight;
			if (!(x == null))
			{
			}
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineTwilightAttackIdle));
			list.Add(typeof(FieStateMachineTwilightBaseShotLevel1));
			list.Add(typeof(FieStateMachineTwilightBaseShotLevel2));
			list.Add(typeof(FieStateMachineTwilightBaseShotLevel3));
			return list;
		}

		public override Type getNextState()
		{
			return _nextState;
		}

		public override bool isNotNetworkSync()
		{
			return true;
		}
	}
}
