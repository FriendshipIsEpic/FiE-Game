using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesRevive : FieStateMachineGameCharacterBase
	{
		private enum RevivingState
		{
			STATE_START,
			STATE_REVIVING
		}

		private RevivingState _revivingState;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private List<Type> _allowedStateList = new List<Type>();

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FiePonies)
			{
				FiePonies fiePonies = gameCharacter as FiePonies;
				if (_revivingState == RevivingState.STATE_START)
				{
					fiePonies.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_RESURRECTED));
					fiePonies.emotionController.SetDefaultEmoteAnimationID(16);
					TrackEntry trackEntry = fiePonies.animationManager.SetAnimation(13, isLoop: false, isForceSet: true);
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectPoniesRevive>(fiePonies.transform, Vector3.up);
					trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
					{
						if (e.Data.Name == "finished")
						{
							_isEnd = true;
						}
					};
					trackEntry.Complete += delegate
					{
						_isEnd = true;
					};
					_revivingState = RevivingState.STATE_REVIVING;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.damageSystem.resetHealthSystem();
				fiePonies.isEnableCollider = false;
				fiePonies.isEnableHeadTracking = false;
				fiePonies.isEnableGravity = true;
				fiePonies.isEnableAutoFlip = false;
				fiePonies.isSpeakable = true;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.emotionController.RestoreEmotionFromDefaultData();
				fiePonies.isSpeakable = true;
				fiePonies.isEnableHeadTracking = true;
				fiePonies.isEnableAutoFlip = true;
				fiePonies.isEnableGravity = true;
				fiePonies.isEnableCollider = true;
				fiePonies.setGravityRate(1f);
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
