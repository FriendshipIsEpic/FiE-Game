using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;

namespace Fie.Enemies.HoovesRaces.Changeling
{
	public class FieStateMachineChangelingVortex : FieStateMachineGameCharacterBase
	{
		private enum MeleeState
		{
			MELEE_START,
			MELEE
		}

		private MeleeState _meleeState;

		private bool _isEnd;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieChangeling fieChangeling = gameCharacter as FieChangeling;
			if (!(fieChangeling == null))
			{
				autoFlipToEnemy(fieChangeling);
				fieChangeling.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieChangeling)
			{
				FieChangeling changeling = gameCharacter as FieChangeling;
				if (_meleeState == MeleeState.MELEE_START)
				{
					if (changeling.groundState == FieObjectGroundState.Grounding)
					{
						TrackEntry trackEntry = changeling.animationManager.SetAnimation(14, isLoop: false, isForceSet: true);
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(AnimationState state, int trackIndex, Event e)
							{
								if (e.Data.Name == "fire")
								{
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingVortex>(changeling.transform, changeling.flipDirectionVector, null, changeling);
								}
								if (e.Data.Name == "finished")
								{
									_isEnd = true;
								}
							};
							trackEntry.Complete += delegate
							{
								_isEnd = true;
							};
						}
						else
						{
							_isEnd = true;
						}
					}
					else
					{
						_isEnd = true;
					}
					_meleeState = MeleeState.MELEE;
				}
			}
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return typeof(FieStateMachineCommonIdle);
		}

		public override List<Type> getAllowedStateList()
		{
			return new List<Type>();
		}
	}
}
