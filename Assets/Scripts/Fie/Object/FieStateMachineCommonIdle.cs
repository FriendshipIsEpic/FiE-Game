using System;
using System.Collections.Generic;

namespace Fie.Object
{
	public class FieStateMachineCommonIdle : FieStateMachineGameCharacterBase
	{
		public override void updateState<T>(ref T gameCharacter)
		{
			gameCharacter.animationManager.SetAnimation(0, isLoop: true);
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
	}
}
