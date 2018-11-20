using Fie.Manager;
using Fie.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	public class FieStateMachineChangelingAlphaConcentration : FieStateMachineGameCharacterBase
	{
		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieChangelingAlpha)
			{
				gameCharacter.animationManager.SetAnimation(9, isLoop: true);
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieChangelingAlpha fieChangelingAlpha = gameCharacter as FieChangelingAlpha;
			if (!(fieChangelingAlpha == null))
			{
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingAlphaConcentration>(fieChangelingAlpha.hornTransform, Vector3.zero, null, gameCharacter);
				autoFlipToEnemy(fieChangelingAlpha);
				fieChangelingAlpha.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
			}
		}

		public override bool isEnd()
		{
			return false;
		}

		public override Type getNextState()
		{
			return typeof(FieStateMachineCommonIdle);
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineAnyConsider));
			return list;
		}
	}
}
