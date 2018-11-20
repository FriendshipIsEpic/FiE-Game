using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisCrucible : FieStateMachineGameCharacterBase
	{
		private enum AttackState
		{
			KICK_START,
			KICK,
			FLYBY,
			FINISHED
		}

		private Type _nextState = typeof(FieStateMachineQueenChrysalisIdle);

		private AttackState _attackingState;

		private bool _isEnd;

		private bool _isMidAirAttack;

		private int _attackCount;

		private FieEmitObjectQueenChrysalisHornEffect _hornEffect;

		private List<Vector3> _hittedPosition = new List<Vector3>();

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieQueenChrysalis fieQueenChrysalis = gameCharacter as FieQueenChrysalis;
			if (!(fieQueenChrysalis == null))
			{
				autoFlipToEnemy(fieQueenChrysalis);
				fieQueenChrysalis.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableCollider = true;
				if ((bool)_hornEffect)
				{
					_hornEffect.Kill();
				}
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieQueenChrysalis)
			{
				FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
				switch (_attackingState)
				{
				case AttackState.KICK_START:
					if (chrysalis.groundState != 0)
					{
						_isEnd = true;
					}
					else
					{
						TrackEntry trackEntry2 = chrysalis.animationManager.SetAnimation(16, isLoop: false, isForceSet: true);
						_hornEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHornEffect>(chrysalis.hornTransform, Vector3.zero, null);
						if (trackEntry2 != null)
						{
							trackEntry2.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
							{
								if (e.Data.Name == "fire")
								{
									FieEmitObjectQueenChrysalisCrucibleCircle fieEmitObjectQueenChrysalisCrucibleCircle = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisCrucibleCircle>(chrysalis.leftFrontHoofTransform, Vector3.forward, null, chrysalis);
									if (fieEmitObjectQueenChrysalisCrucibleCircle != null)
									{
										fieEmitObjectQueenChrysalisCrucibleCircle.transform.rotation = Quaternion.identity;
										fieEmitObjectQueenChrysalisCrucibleCircle.hitEvent += CrucibleCircle_hitEvent;
									}
									if ((bool)_hornEffect)
									{
										_hornEffect.Kill();
									}
								}
								if (e.Data.Name == "finished")
								{
									_attackingState = AttackState.FLYBY;
								}
							};
							trackEntry2.Complete += delegate
							{
								_attackingState = AttackState.FLYBY;
							};
						}
						else
						{
							_isEnd = true;
						}
						_attackingState = AttackState.KICK;
					}
					break;
				case AttackState.FLYBY:
				{
					if ((bool)_hornEffect)
					{
						_hornEffect.Kill();
					}
					TrackEntry trackEntry = chrysalis.animationManager.SetAnimation(15, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "fire")
							{
								foreach (Vector3 item in _hittedPosition)
								{
									FieEmitObjectQueenChrysalisCrucibleBurst fieEmitObjectQueenChrysalisCrucibleBurst = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisCrucibleBurst>(chrysalis.transform, Vector3.zero, null, chrysalis);
									fieEmitObjectQueenChrysalisCrucibleBurst.transform.position = item;
								}
							}
							if (e.Data.Name == "activate")
							{
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisCommonActivationEffect>(chrysalis.leftFrontHoofTransform, Vector3.zero, null, chrysalis);
							}
							if (e.Data.Name == "move")
							{
								chrysalis.isEnableGravity = false;
								Vector3 moveForce = Vector3.up * e.Float;
								chrysalis.setMoveForce(moveForce, 0.5f);
							}
						};
						trackEntry.Complete += delegate
						{
							_nextState = typeof(FieStateMachineQueenChrysalisJumpIdle);
							_isEnd = true;
						};
					}
					_attackingState = AttackState.FINISHED;
					break;
				}
				}
			}
		}

		private void CrucibleCircle_hitEvent(Vector3 point)
		{
			_hittedPosition.Add(point);
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
