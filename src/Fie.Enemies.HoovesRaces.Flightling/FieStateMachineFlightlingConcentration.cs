using Fie.Manager;
using Fie.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.Flightling
{
	public class FieStateMachineFlightlingConcentration : FieStateMachineGameCharacterBase
	{
		public override void updateState<T>(ref T flightling)
		{
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieFlightling fieFlightling = gameCharacter as FieFlightling;
			if (!(fieFlightling == null))
			{
				FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectFlightlingConcentration>(fieFlightling.hornTransform, Vector3.zero, null, gameCharacter);
				autoFlipToEnemy(fieFlightling);
				fieFlightling.isEnableGravity = false;
				fieFlightling.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
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
