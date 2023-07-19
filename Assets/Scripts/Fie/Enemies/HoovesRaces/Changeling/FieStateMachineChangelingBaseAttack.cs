using Fie.Object;
using System;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	public class FieStateMachineChangelingBaseAttack : FieStateMachineGameCharacterBase
	{
		private bool _isEnd;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieChangeling)
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
