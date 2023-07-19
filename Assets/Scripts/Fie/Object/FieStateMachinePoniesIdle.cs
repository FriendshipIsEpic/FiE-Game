using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Object
{
	public class FieStateMachinePoniesIdle : FieStateMachineGameCharacterBase
	{
		public const float IDLING_THLESHOLD_SEC = 5f;

		private float idleCount;

		public override void updateState<T>(ref T gameCharacter)
		{
			idleCount = Mathf.Min(idleCount + Time.deltaTime, 5f);
			if (idleCount >= 5f)
			{
				gameCharacter.animationManager.SetAnimation(1, isLoop: true);
			}
			else
			{
				gameCharacter.animationManager.SetAnimation(0, isLoop: true);
			}
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
