using Fie.Object;
using System;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesAbilitySlot1 : FieStateMachineGameCharacterBase
	{
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
