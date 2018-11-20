using Fie.Object;
using System;
using System.Collections.Generic;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisIdle : FieStateMachineGameCharacterBase
	{
		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineQueenChrysalisIdle);

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieQueenChrysalis)
			{
				if (gameCharacter.groundState == FieObjectGroundState.Flying)
				{
					_nextState = typeof(FieStateMachineQueenChrysalisJumpIdle);
					_isEnd = true;
				}
				gameCharacter.animationManager.SetAnimation(0, isLoop: true);
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
			list.Add(typeof(FieStateMachineAnyConsider));
			return list;
		}
	}
}
