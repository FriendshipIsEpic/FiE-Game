using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisHormingShot : FieStateMachineGameCharacterBase
	{
		private enum ShootState
		{
			STATE_PREPARE,
			STATE_SHOOT
		}

		private ShootState _shootState;

		private bool _isEnd;

		private FieEmitObjectQueenChrysalisHornEffect _hornEffect;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			gameCharacter.isEnableAutoFlip = false;
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieQueenChrysalis fieQueenChrysalis = gameCharacter as FieQueenChrysalis;
			if (!(fieQueenChrysalis == null))
			{
				if ((bool)_hornEffect)
				{
					_hornEffect.Kill();
				}
				fieQueenChrysalis._isEffectivePull = false;
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
			if (!(chrysalis == null))
			{
				if (chrysalis.detector.getLockonEnemyTransform() == null)
				{
					_isEnd = true;
				}
				else
				{
					ShootState shootState = _shootState;
					if (shootState == ShootState.STATE_PREPARE)
					{
						TrackEntry trackEntry = chrysalis.animationManager.SetAnimation(28, isLoop: false, isForceSet: true);
						FieEmitObjectQueenChrysalisShootingConcentration concentRation = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisShootingConcentration>(chrysalis.hornTransform, chrysalis.flipDirectionVector, chrysalis.detector.getLockonEnemyTransform(isCenter: true));
						_hornEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHornEffect>(chrysalis.hornTransform, Vector3.zero, null, chrysalis);
						chrysalis._isEffectivePull = true;
						if (trackEntry != null)
						{
							autoFlipToEnemy(chrysalis);
							trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
							{
								if (e.Data.Name == "fire")
								{
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHormingShot>(chrysalis.hornTransform, chrysalis.flipDirectionVector, chrysalis.detector.getLockonEnemyTransform(isCenter: true), chrysalis);
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHormingShotActivateEffect>(chrysalis.hornTransform, Vector3.zero, null);
									chrysalis._isEffectivePull = false;
								}
								if (e.Data.Name == "move")
								{
									Vector3 flipDirectionVector = chrysalis.flipDirectionVector;
									Vector3 moveForce = flipDirectionVector * e.Float;
									chrysalis.resetMoveForce();
									chrysalis.setMoveForce(moveForce, 0f, useRound: false);
								}
								if (e.Data.Name == "finished")
								{
									if ((bool)_hornEffect)
									{
										_hornEffect.Kill();
									}
									concentRation.StopEffect();
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
						_shootState = ShootState.STATE_SHOOT;
					}
				}
			}
		}

		public override bool isEnd()
		{
			return _isEnd;
		}

		public override Type getNextState()
		{
			return typeof(FieStateMachineQueenChrysalisPenetrateShot);
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list = new List<Type>();
			list.Add(typeof(FieStateMachineQueenChrysalisPenetrateShot));
			return list;
		}
	}
}
