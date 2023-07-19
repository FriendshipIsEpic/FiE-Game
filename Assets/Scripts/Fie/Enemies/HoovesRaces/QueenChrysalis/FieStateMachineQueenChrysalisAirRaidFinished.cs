using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisAirRaidFinished : FieStateMachineGameCharacterBase
	{
		private enum FinishedState
		{
			FINISHED_START,
			FINISHED_END
		}

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FinishedState _fireState;

		private bool _isEnd;

		private bool _isFinished;

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
			if (!(chrysalis == null))
			{
				switch (_fireState)
				{
				case FinishedState.FINISHED_START:
				{
					TrackEntry trackEntry = chrysalis.animationManager.SetAnimation(10, isLoop: false, isForceSet: true);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisAirRaidPreHit>(chrysalis.transform, Vector3.up, null, chrysalis);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisAirRiad>(chrysalis.transform, Vector3.up, null, chrysalis);
					if (trackEntry != null)
					{
						trackEntry.Complete += delegate
						{
							TrackEntry trackEntry2 = chrysalis.animationManager.SetAnimation(12);
							if (trackEntry2 != null)
							{
								trackEntry2.Complete += delegate
								{
									_nextState = typeof(FieStateMachineCommonIdle);
									_isEnd = true;
								};
							}
						};
					}
					else
					{
						_isEnd = true;
					}
					_fireState = FinishedState.FINISHED_END;
					break;
				}
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.emotionController.StopAutoAnimation();
				gameCharacter.isEnableCollider = true;
				gameCharacter.isEnableAutoFlip = false;
				gameCharacter.isEnableGravity = true;
				gameCharacter.resetMoveForce();
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.isEnableCollider = true;
				gameCharacter.rootBone.transform.localRotation = Quaternion.identity;
				gameCharacter.resetMoveForce();
				gameCharacter.submeshObject.enabled = true;
			}
		}

		public override List<Type> getAllowedStateList()
		{
			return new List<Type>();
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return _nextState;
		}

		public override bool isFinished()
		{
			return _isFinished;
		}
	}
}
