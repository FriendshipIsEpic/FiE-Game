using Fie.Object;
using System;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesBaseAttack : FieStateMachineGameCharacterBase
	{
		private bool _isEnd;

		private Type _nextState;

		public override void updateState<T>(ref T gameCharacter)
		{
			_nextState = gameCharacter.getDefaultAttackState();
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
	}
}
