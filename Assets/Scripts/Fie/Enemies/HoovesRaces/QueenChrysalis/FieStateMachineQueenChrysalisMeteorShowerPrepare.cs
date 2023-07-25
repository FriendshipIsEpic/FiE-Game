using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisMeteorShowerPrepare : FieStateMachineGameCharacterBase
	{
		private enum PreparingState
		{
			METEOR_SHOWER_PREPARE_START,
			METEOR_SHOWER_PREPARE_END
		}

		public const float METEOR_SHOWER_AIR_DRAG = 500f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private PreparingState _fireState;

		private bool _isEnd;

		private bool _isFinished;

		private FieEmitObjectQueenChrysalisMeteorShowerConcentration _concentrationObject;

		private FieEmitObjectQueenChrysalisHornEffect _hornEffect;

		private float _initializedDrag;

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
			if (!(chrysalis == null))
			{
				switch (_fireState)
				{
				case PreparingState.METEOR_SHOWER_PREPARE_START:
				{
					chrysalis.SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_5));
					TrackEntry trackEntry = chrysalis.animationManager.SetAnimation(26, isLoop: false, isForceSet: true);
					_hornEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHornEffect>(chrysalis.hornTransform, Vector3.zero, null);
					_concentrationObject = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisMeteorShowerConcentration>(chrysalis.concentrationTransform, chrysalis.flipDirectionVector);
					chrysalis.emotionController.SetDefaultEmoteAnimationID(31);
					chrysalis.emotionController.SetEmoteAnimation(31, isForceSet: true);
					chrysalis._isEffectivePull = true;
					if (trackEntry != null)
					{
						trackEntry.Complete += delegate
						{
							_nextState = typeof(FieStateMachineQueenChrysalisMeteorShowerAttacking);
							_isEnd = true;
						};
						trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
						{
							if (trackIndex.Data.Name == "move")
							{
								Vector3 moveForce = Vector3.up * trackIndex.Float;
								chrysalis.isEnableGravity = false;
								chrysalis.resetMoveForce();
								chrysalis.setMoveForce(moveForce, 0.5f);
							}
							if (trackIndex.Data.Name == "activate")
							{
								if (_concentrationObject != null)
								{
									_concentrationObject.StopEffect();
								}
								chrysalis.emotionController.SetDefaultEmoteAnimationID(30);
								chrysalis.emotionController.SetEmoteAnimation(30, isForceSet: true);
								chrysalis._isEffectivePull = false;
							}
							if (trackIndex.Data.Name == "fire")
							{
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisCommonActivationEffect>(chrysalis.hornTransform, chrysalis.flipDirectionVector);
								chrysalis.SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_1), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_2));
							}
						};
					}
					else
					{
						_isEnd = true;
					}
					_fireState = PreparingState.METEOR_SHOWER_PREPARE_END;
					break;
				}
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = false;
				gameCharacter.isEnableGravity = true;
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
				fieQueenChrysalis.emotionController.SetDefaultEmoteAnimationID(30);
				fieQueenChrysalis.emotionController.SetEmoteAnimation(30, isForceSet: true);
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
