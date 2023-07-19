using Fie.Object;
using System;
using System.Collections.Generic;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisJumpIdle : FieStateMachineGameCharacterBase
	{
		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineQueenChrysalisIdle);

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieQueenChrysalis && gameCharacter.groundState == FieObjectGroundState.Grounding)
			{
				gameCharacter.animationManager.SetAnimation(24, isLoop: false, isForceSet: true);
				_nextState = typeof(FieStateMachineQueenChrysalisIdle);
				_isEnd = true;
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			base.terminate(gameCharacter);
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
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
