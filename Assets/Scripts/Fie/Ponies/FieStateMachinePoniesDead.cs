using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesDead : FieStateMachineGameCharacterBase
	{
		private enum ArrivalfState
		{
			STATE_START,
			STATE_DEAD_PREPARE,
			STATE_DEAD
		}

		private const float STAGGER_MOVE_FORCE = 8f;

		private const float ARRIVAL_INVISIBLE_DURATION = 1.9f;

		private ArrivalfState _arrivalState;

		private bool _isEnd;

		private Type _nextState;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FiePonies)
			{
				FiePonies ponies = gameCharacter as FiePonies;
				switch (_arrivalState)
				{
				case ArrivalfState.STATE_START:
					if (ponies.groundState != 0)
					{
						ponies.isEnableGravity = false;
						ponies.isEnableGravity = true;
						gameCharacter.setGravityRate(0.2f);
						Vector3 vector = Vector3.Normalize(ponies.centerTransform.position - ponies.latestDamageWorldPoint);
						vector.y *= 0.1f;
						vector.z = 0f;
						vector.Normalize();
						ponies.setFlipByVector(vector * -1f);
						ponies.setMoveForce((Vector3.up + vector).normalized * 8f, 1f, useRound: false);
						ponies.emotionController.SetDefaultEmoteAnimationID(18);
						TrackEntry trackEntry = ponies.animationManager.SetAnimation(8, isLoop: false, isForceSet: true);
						trackEntry.mixDuration = 0f;
						_arrivalState = ArrivalfState.STATE_DEAD_PREPARE;
					}
					else
					{
						TrackEntry trackEntry2 = ponies.animationManager.SetAnimation(18, isLoop: false, isForceSet: true);
						if (trackEntry2 == null)
						{
							_nextState = typeof(FieStateMachineCommonIdle);
							_isEnd = true;
						}
						trackEntry2.End += delegate
						{
							ponies.animationManager.SetAnimation(11, isLoop: false, isForceSet: true);
							ponies.isSpeakable = false;
							ponies.damageSystem.revivable = true;
						};
						ponies.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_DEFEATED));
						ponies.emotionController.SetDefaultEmoteAnimationID(18);
						if (gameCharacter.GetInstanceID() != FieManagerBehaviour<FieUserManager>.I.gameOwnerCharacter.GetInstanceID())
						{
							FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_ANYONE_DEFEATED), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_ANYONE_DEFEATED));
						}
						else
						{
							FieManagerBehaviour<FieActivityManager>.I.RequestActivity(FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_TITLE_MYSELF_DEFEATED), FieMasterData<GDEConstantTextListData>.I.GetMasterData(GDEItemKeys.ConstantTextList_ACTIVITY_NOTE_MYSELF_DEFEATED));
						}
						_arrivalState = ArrivalfState.STATE_DEAD;
					}
					break;
				case ArrivalfState.STATE_DEAD_PREPARE:
					if (ponies.groundState == FieObjectGroundState.Grounding)
					{
						_arrivalState = ArrivalfState.STATE_START;
					}
					break;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.isEnableAutoFlip = false;
				fiePonies.isEnableCollider = false;
				fiePonies.isEnableHeadTracking = false;
				fiePonies.UnbindFromDetecter();
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.isSpeakable = true;
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
			list.Add(typeof(FieStateMachinePoniesArrival));
			return list;
		}
	}
}
