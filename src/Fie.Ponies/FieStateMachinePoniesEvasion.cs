using Fie.Object;
using System;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesEvasion : FieStateMachineGameCharacterBase
	{
		private bool _isEnd;

		private Type _nextState;

		public override void updateState<T>(ref T gameCharacter)
		{
		}

		public override bool isEnd()
		{
			return true;
		}

		public override Type getNextState()
		{
			return _nextState;
		}
	}
}
