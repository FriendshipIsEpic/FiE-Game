using Fie.Object;
using System;
using System.Collections.Generic;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesAttackIdle : FieStateMachineGameCharacterBase
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
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineAnyConsider));
			return list;
		}

		public override bool isNotNetworkSync()
		{
			return true;
		}
	}
}
