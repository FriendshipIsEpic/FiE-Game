using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisStaggerFall : FieStateMachineGameCharacterBase
	{
		private enum StaggerState
		{
			STATE_PREPARE,
			STATE_STAGGER,
			STATE_STAGGER_END
		}

		private StaggerState _staggerState;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private List<Type> _allowedStateList = new List<Type>();

		private FieEmitObjectQueenChrysalisHornEffect _hornEffect;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieEnemiesHoovesRaces)
			{
				FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
				switch (_staggerState)
				{
				case StaggerState.STATE_PREPARE:
				{
					int animationId = 5;
					TrackEntry trackEntry2 = chrysalis.animationManager.SetAnimation(animationId, isLoop: false, isForceSet: true);
					trackEntry2.mixDuration = 0f;
					chrysalis.emotionController.SetDefaultEmoteAnimationID(31);
					chrysalis.emotionController.SetEmoteAnimation(31, isForceSet: true);
					chrysalis.isSpeakable = false;
					chrysalis._isImmuniteStaggerDamage = true;
					_staggerState = StaggerState.STATE_STAGGER;
					break;
				}
				case StaggerState.STATE_STAGGER:
					if (chrysalis.groundState == FieObjectGroundState.Grounding)
					{
						TrackEntry trackEntry = chrysalis.animationManager.SetAnimation(6, isLoop: false, isForceSet: true);
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "activate")
							{
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisCommonActivationEffect>(chrysalis.hornTransform, chrysalis.flipDirectionVector, null, chrysalis);
								_hornEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHornEffect>(chrysalis.hornTransform, Vector3.zero, null);
								chrysalis.SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_RANDOM_4), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_RANDOM_5));
								chrysalis.emotionController.SetDefaultEmoteAnimationID(30);
								chrysalis.emotionController.SetEmoteAnimation(30, isForceSet: true);
								chrysalis.isSpeakable = true;
							}
							if (e.Data.Name == "fire")
							{
								FieEmitObjectQueenChrysalisStaggerRecoveringBurst fieEmitObjectQueenChrysalisStaggerRecoveringBurst = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisStaggerRecoveringBurst>(chrysalis.centerTransform, chrysalis.flipDirectionVector, null, chrysalis);
								FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(0.6f, 15, new Vector3(0.2f, 0.5f));
								fieEmitObjectQueenChrysalisStaggerRecoveringBurst.transform.rotation = Quaternion.identity;
								if ((bool)_hornEffect)
								{
									_hornEffect.Kill();
								}
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
						_staggerState = StaggerState.STATE_STAGGER_END;
					}
					break;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieQueenChrysalis fieQueenChrysalis = gameCharacter as FieQueenChrysalis;
			if (!(fieQueenChrysalis == null))
			{
				fieQueenChrysalis.isEnableGravity = true;
				fieQueenChrysalis.setGravityRate(0.5f);
				fieQueenChrysalis.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieQueenChrysalis fieQueenChrysalis = gameCharacter as FieQueenChrysalis;
			if (!(fieQueenChrysalis == null))
			{
				fieQueenChrysalis.setGravityRate(1f);
				fieQueenChrysalis.isEnableAutoFlip = true;
				fieQueenChrysalis.isEnableGravity = true;
				if ((bool)_hornEffect)
				{
					_hornEffect.Kill();
				}
				fieQueenChrysalis.emotionController.SetDefaultEmoteAnimationID(30);
				fieQueenChrysalis.emotionController.SetEmoteAnimation(30, isForceSet: true);
				fieQueenChrysalis.isSpeakable = true;
				fieQueenChrysalis._isImmuniteStaggerDamage = false;
				fieQueenChrysalis.SetStaggerImmuniteDelay();
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
			return _allowedStateList;
		}
	}
}
