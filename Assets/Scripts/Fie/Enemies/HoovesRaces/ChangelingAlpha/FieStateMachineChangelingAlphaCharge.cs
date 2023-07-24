using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	public class FieStateMachineChangelingAlphaCharge : FieStateMachineGameCharacterBase
	{
		private const float MOVE_THRESHOLD = 0.02f;

		private const float WALK_FORCE_THRESHOLD = 0.3f;

		private const float WALK_MOVESPEED_MAGNI = 0.25f;

		private bool _isEnd;

		private Type _nextState;

		public override void updateState<T>(ref T gameCharacter)
		{
			FieChangelingAlpha changelingAlpha = gameCharacter as FieChangelingAlpha;
			if ((object)changelingAlpha != null)
			{
				Vector3 externalInputVector = changelingAlpha.externalInputVector;
				externalInputVector.z = externalInputVector.y * 0.5f;
				externalInputVector.y = 0f;
				float defaultMoveSpeed = changelingAlpha.getDefaultMoveSpeed();
				TrackEntry trackEntry = changelingAlpha.animationManager.SetAnimation(13, isLoop: true);
				if (trackEntry != null)
				{
					trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
					{
						if (trackIndex.Data.Name == "fire")
						{
							FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingAlphaChargeEffect>(changelingAlpha.centerTransform, changelingAlpha.flipDirectionVector);
							FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingAlphaCharge>(changelingAlpha.centerTransform, changelingAlpha.flipDirectionVector, null, changelingAlpha);
						}
					};
				}
				changelingAlpha.addMoveForce(externalInputVector * (defaultMoveSpeed * changelingAlpha.externalInputForce), 0.4f);
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = false;
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
