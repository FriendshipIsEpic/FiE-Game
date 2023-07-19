using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisAirRiadPrepare : FieStateMachineGameCharacterBase
	{
		private enum PreparingState
		{
			AIR_RAID_PREPARING_START,
			AIR_RAID_PREPARING_END
		}

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private PreparingState _fireState;

		private bool _isEnd;

		private bool _isFinished;

		private FieEmitObjectQueenChrysalisAirRaidConcentration _concentrationObject;

		private FieEmitObjectQueenChrysalisHornEffect _hornEffect;

		private float _initializedDrag;

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
			if (!(chrysalis == null))
			{
				if (chrysalis.groundState != FieObjectGroundState.Flying)
				{
					_isEnd = true;
				}
				else
				{
					switch (_fireState)
					{
					case PreparingState.AIR_RAID_PREPARING_START:
					{
						TrackEntry trackEntry = chrysalis.animationManager.SetAnimation(11, isLoop: false, isForceSet: true);
						_hornEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHornEffect>(chrysalis.hornTransform, Vector3.zero, null);
						chrysalis._isEffectivePull = true;
						if (trackEntry != null)
						{
							trackEntry.Complete += delegate
							{
								chrysalis.animationManager.SetAnimation(0, isLoop: true);
								_nextState = typeof(FieStateMachineQueenChrysalisAirRiadAttacking);
								_isEnd = true;
							};
							trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
							{
								if (e.Data.Name == "activate")
								{
									_concentrationObject = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisAirRaidConcentration>(chrysalis.leftFrontHoofTransform, chrysalis.flipDirectionVector);
								}
								if (e.Data.Name == "finished")
								{
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisCommonActivationEffect>(chrysalis.leftFrontHoofTransform, chrysalis.flipDirectionVector);
									if (_concentrationObject != null)
									{
										_concentrationObject.StopEffect();
									}
								}
							};
						}
						else
						{
							_isEnd = true;
						}
						_fireState = PreparingState.AIR_RAID_PREPARING_END;
						break;
					}
					}
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.isEnableGravity = false;
				gameCharacter.resetMoveForce();
				autoFlipToEnemy(gameCharacter);
				Rigidbody component = gameCharacter.GetComponent<Rigidbody>();
				if (component != null)
				{
					_initializedDrag = component.drag;
					component.drag = 500f;
				}
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieQueenChrysalis fieQueenChrysalis = gameCharacter as FieQueenChrysalis;
			if (!(fieQueenChrysalis == null))
			{
				fieQueenChrysalis.isEnableGravity = true;
				fieQueenChrysalis.isEnableAutoFlip = true;
				fieQueenChrysalis.resetMoveForce();
				if (_concentrationObject != null)
				{
					_concentrationObject.StopEffect();
				}
				if ((bool)_hornEffect)
				{
					_hornEffect.Kill();
				}
				Rigidbody component = fieQueenChrysalis.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.drag = _initializedDrag;
				}
				fieQueenChrysalis._isEffectivePull = false;
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
