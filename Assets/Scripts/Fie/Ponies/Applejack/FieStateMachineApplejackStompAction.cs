using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackStompAction : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			STOMP_ACTION_START,
			STOMP_ACTION_STOMPED,
			STOMP_ACTION_END
		}

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private bool _isFinished;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (!_isEnd && gameCharacter is FieApplejack)
			{
				FieApplejack applejack = gameCharacter as FieApplejack;
				switch (_fireState)
				{
				case FireState.STOMP_ACTION_START:
				{
					TrackEntry trackEntry2 = applejack.animationManager.SetAnimation(39, isLoop: false, isForceSet: true);
					applejack.setMoveForce(Vector3.up * 2f, 1f, useRound: false);
					if (trackEntry2 != null)
					{
						trackEntry2.Complete += delegate
						{
							applejack.isEnableGravity = true;
							applejack.setGravityRate(3f);
						};
					}
					else
					{
						_isEnd = true;
					}
					GDESkillTreeData skill = applejack.GetSkill(FieConstValues.FieSkill.HONESTY_STOMP_LV3_2);
					if (skill != null)
					{
						applejack.isEnableCollider = false;
					}
					applejack.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_USED_ABILITY));
					_fireState = FireState.STOMP_ACTION_STOMPED;
					break;
				}
				case FireState.STOMP_ACTION_STOMPED:
					if (applejack.groundState == FieObjectGroundState.Grounding)
					{
						TrackEntry trackEntry = applejack.animationManager.SetAnimation(4, isLoop: false, isForceSet: true);
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackStomp>(applejack.transform, Vector3.zero, null, applejack);
						FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(0.4f, 10, new Vector3(0.05f, 0.3f));
						if (trackEntry != null)
						{
							trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
							{
								if (trackIndex.Data.Name == "finished")
								{
									_isEnd = true;
								}
							};
						}
						applejack.isEnableCollider = true;
						applejack.abilitiesContainer.SetCooldown<FieStateMachineApplejackStomp>(7f);
						_fireState = FireState.STOMP_ACTION_END;
					}
					break;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieApplejack fieApplejack = gameCharacter as FieApplejack;
			if (!(fieApplejack == null))
			{
				fieApplejack.isEnableGravity = false;
				fieApplejack.isEnableAutoFlip = false;
				fieApplejack.isEnableHeadTracking = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieApplejack fieApplejack = gameCharacter as FieApplejack;
			if (!(fieApplejack == null))
			{
				fieApplejack.isEnableCollider = true;
				fieApplejack.isEnableGravity = true;
				fieApplejack.setGravityRate(1f);
				fieApplejack.isEnableAutoFlip = true;
				fieApplejack.isEnableHeadTracking = true;
			}
		}

		public override List<Type> getAllowedStateList()
		{
			List<Type> list = new List<Type>();
			if (_isFinished)
			{
				list.Add(typeof(FieStateMachineApplejackFirePunch));
				list.Add(typeof(FieStateMachineApplejackEvasion));
			}
			return list;
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
