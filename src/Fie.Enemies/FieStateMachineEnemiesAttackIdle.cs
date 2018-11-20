using Fie.Object;
using System;
using System.Collections.Generic;

namespace Fie.Enemies
{
	public class FieStateMachineEnemiesAttackIdle : FieStateMachineGameCharacterBase
	{
		public override void updateState<T>(ref T gameCharacter)
		{
		}

		public override bool isEnd()
		{
			return false;
		}

		public override Type getNextState()
		{
			return null;
		}

		public override List<Type> getAllowedStateList()
		{
			return new List<Type>();
		}
	}
}
