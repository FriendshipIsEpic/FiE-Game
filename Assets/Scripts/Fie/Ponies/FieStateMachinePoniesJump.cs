using Fie.Object;
using System;
using System.Collections.Generic;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesJump : FieStateMachineGameCharacterBase
	{
		private bool _isEnd;

		private Type _nextState;

		public override void updateState<T>(ref T gameCharacter)
		{
			_isEnd = true;
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
			return new List<Type>();
		}
	}
}
