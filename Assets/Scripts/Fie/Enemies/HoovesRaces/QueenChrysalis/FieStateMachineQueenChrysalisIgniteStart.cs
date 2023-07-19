using Fie.Manager;
using Fie.Object;
using Fie.Ponies;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisIgniteStart : FieStateMachineGameCharacterBase
	{
		private enum GrabState
		{
			START,
			GRABBING,
			FINISHED
		}

		private const float HORMING_DISTANCE = 1f;

		private const float HORMING_DEFAULT_RATE = 0.5f;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineQueenChrysalisIdle);

		private GrabState _state;

		private bool _succeedGrab;

		private FieGameCharacter _grabbedCharacter;

		private FieEmitObjectQueenChrysalisIgniteCollision _grabbingCollisionObject;

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
			if (chrysalis == null)
			{
				_isEnd = true;
			}
			else
			{
				switch (_state)
				{
				case GrabState.START:
					if (chrysalis.groundState != 0)
					{
						_nextState = typeof(FieStateMachineQueenChrysalisIdle);
						_isEnd = true;
					}
					else
					{
						TrackEntry trackEntry = chrysalis.animationManager.SetAnimation(22, isLoop: false, isForceSet: true);
						chrysalis.SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_3), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_4));
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
							{
								if (e.Data.Name == "move")
								{
									Vector3 a = chrysalis.flipDirectionVector;
									Transform lockonEnemyTransform = chrysalis.detector.getLockonEnemyTransform();
									float num = 0.5f;
									if (lockonEnemyTransform != null)
									{
										Vector3 vector = lockonEnemyTransform.position - chrysalis.transform.position;
										num = Mathf.Min(Mathf.Abs(vector.x) / 1f, 1f);
										vector.Normalize();
										a = vector;
										vector.y = 0f;
									}
									Vector3 vector2 = a * (e.Float * num);
									chrysalis.resetMoveForce();
									chrysalis.setMoveForce(vector2, 0f, useRound: false);
									chrysalis.setFlipByVector(vector2);
								}
								if (e.Data.Name == "fire")
								{
									_grabbingCollisionObject = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisIgniteCollision>(chrysalis.centerTransform, chrysalis.flipDirectionVector, null, chrysalis);
									if (_grabbingCollisionObject != null)
									{
										_grabbingCollisionObject.grabbedEvent += delegate(FieGameCharacter grabbedCharacter)
										{
											_grabbedCharacter = grabbedCharacter;
										};
									}
								}
							};
							trackEntry.Complete += delegate
							{
								if (!_succeedGrab)
								{
									_nextState = typeof(FieStateMachineQueenChrysalisIgniteFailed);
									_isEnd = true;
								}
							};
							_state = GrabState.GRABBING;
						}
						else
						{
							_nextState = typeof(FieStateMachineQueenChrysalisIgniteFailed);
							_isEnd = true;
						}
					}
					break;
				case GrabState.GRABBING:
					if (_grabbedCharacter != null)
					{
						FieStateMachinePoniesGrabbed fieStateMachinePoniesGrabbed = _grabbedCharacter.getStateMachine().getCurrentStateMachine() as FieStateMachinePoniesGrabbed;
						if (fieStateMachinePoniesGrabbed != null)
						{
							fieStateMachinePoniesGrabbed.SetAnchorTransform(chrysalis.rightFrontHoofTransform, chrysalis.torsoTransform);
							chrysalis.grabbingGameCharacter = _grabbedCharacter;
							_nextState = typeof(FieStateMachineQueenChrysalisIgnitePrepare);
							_isEnd = true;
						}
					}
					break;
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
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = false;
				gameCharacter.emotionController.RestartAutoAnimation();
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
