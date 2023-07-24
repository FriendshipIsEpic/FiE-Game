using Fie.Camera;
using Fie.Manager;
using Fie.Object;
using Fie.Ponies;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisIgnitePrepare : FieStateMachineGameCharacterBase
	{
		private enum IgniteState
		{
			START,
			PREPARE,
			FINISHED
		}

		private const float HORMING_DISTANCE = 1f;

		private const float HORMING_DEFAULT_RATE = 0.5f;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineQueenChrysalisIdle);

		private IgniteState _state;

		private bool _succeedGrab;

		private FieEmitObjectQueenChrysalisHornEffect _hornEffect;

		private FieEmitObjectQueenChrysalisIgniteConcentration _concentrationEffect;

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
			if (chrysalis == null)
			{
				_isEnd = true;
			}
			else if (chrysalis.grabbingGameCharacter == null)
			{
				_nextState = typeof(FieStateMachineQueenChrysalisIgniteFailed);
				_isEnd = true;
			}
			else
			{
				switch (_state)
				{
				case IgniteState.START:
				{
					TrackEntry trackEntry = chrysalis.animationManager.SetAnimation(21, isLoop: false, isForceSet: true);
					_concentrationEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisIgniteConcentration>(chrysalis.hornTransform, chrysalis.flipDirectionVector);
					_hornEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHornEffect>(chrysalis.hornTransform, Vector3.zero, null);
					chrysalis._isEffectivePull = true;
					if (FieManagerBehaviour<FieGameCameraManager>.I.gameCamera != null)
					{
						FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setOffsetTransition(chrysalis.grabbingGameCharacter, new FieCameraOffset(new FieCameraOffset.FieCameraOffsetParam(new Vector3(0f, -0.5f, 2.2f), new Vector3(0f, 0f, 0f), -10f), 0.5f, 3.5f, 0.3f));
					}
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
						{
							if (trackIndex.Data.Name == "activate")
							{
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisCommonActivationEffect>(chrysalis.hornTransform, chrysalis.flipDirectionVector);
								if (_concentrationEffect != null)
								{
									_concentrationEffect.StopEffect();
								}
								chrysalis._isEffectivePull = true;
							}
						};
						trackEntry.Complete += delegate
						{
							FieStateMachinePoniesGrabbed fieStateMachinePoniesGrabbed = chrysalis.grabbingGameCharacter.getStateMachine().getCurrentStateMachine() as FieStateMachinePoniesGrabbed;
							if (fieStateMachinePoniesGrabbed != null)
							{
								FieEmitObjectQueenChrysalisIgniteBurst fieEmitObjectQueenChrysalisIgniteBurst = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisIgniteBurst>(chrysalis.hornTransform, chrysalis.flipDirectionVector, null, chrysalis);
								if (fieEmitObjectQueenChrysalisIgniteBurst != null)
								{
									fieEmitObjectQueenChrysalisIgniteBurst.AddDamageForGameCharacter(chrysalis.grabbingGameCharacter);
								}
								fieStateMachinePoniesGrabbed.SetReleaseState(isReleased: true);
								chrysalis.grabbingGameCharacter = null;
								_nextState = typeof(FieStateMachineQueenChrysalisIgniteSucceed);
								_isEnd = true;
							}
						};
						_state = IgniteState.PREPARE;
					}
					else
					{
						_nextState = typeof(FieStateMachineQueenChrysalisIgniteFailed);
						_isEnd = true;
					}
					break;
				}
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
				gameCharacter.emotionController.StopAutoAnimation();
				gameCharacter.isEnableAutoFlip = false;
				autoFlipToEnemy(gameCharacter);
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			base.terminate(gameCharacter);
			FieQueenChrysalis fieQueenChrysalis = gameCharacter as FieQueenChrysalis;
			if (!(fieQueenChrysalis == null))
			{
				fieQueenChrysalis.isEnableGravity = true;
				fieQueenChrysalis.isEnableAutoFlip = false;
				fieQueenChrysalis._isEffectivePull = false;
				fieQueenChrysalis.emotionController.RestartAutoAnimation();
				if (_hornEffect != null)
				{
					_hornEffect.Kill();
				}
				if (_concentrationEffect != null)
				{
					_concentrationEffect.StopEffect();
				}
				if (fieQueenChrysalis.grabbingGameCharacter != null)
				{
					(fieQueenChrysalis.grabbingGameCharacter.getStateMachine().getCurrentStateMachine() as FieStateMachinePoniesGrabbed)?.SetReleaseState(isReleased: true);
				}
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
