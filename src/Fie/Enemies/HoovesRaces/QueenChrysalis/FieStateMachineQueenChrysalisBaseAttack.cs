using Fie.Object;
using System;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisBaseAttack : FieStateMachineGameCharacterBase
	{
		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieQueenChrysalis)
			{
			}
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return typeof(FieStateMachineEnemiesAttackIdle);
		}
	}
}
