using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies
{
	public class FieStateMachinePoniesArrival : FieStateMachineGameCharacterBase
	{
		private enum ArrivalfState
		{
			STATE_START,
			STATE_PREPARE,
			STATE_END
		}

		private const float ARRIVAL_INVISIBLE_DURATION = 1.9f;

		private ArrivalfState _arrivalState;

		private bool _isEnd;

		private Type _nextState;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FiePonies)
			{
				FiePonies ponies = gameCharacter as FiePonies;
				if (_arrivalState == ArrivalfState.STATE_START)
				{
					FieEmitObjectPoniesArrival fieEmitObjectPoniesArrival = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectPoniesArrival>(ponies.centerTransform, Vector3.zero);
					if (fieEmitObjectPoniesArrival == null)
					{
						_nextState = typeof(FieStateMachineCommonIdle);
						_isEnd = true;
					}
					fieEmitObjectPoniesArrival.SetSubMeshObject(ponies.gameObject);
					TrackEntry trackEntry = ponies.animationManager.SetAnimation(10, isLoop: false, isForceSet: true);
					if (trackEntry == null)
					{
						_nextState = typeof(FieStateMachineCommonIdle);
						_isEnd = true;
					}
					trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
					{
						if (trackIndex.Data.Name == "finished")
						{
							ponies.SetDialog(FieMasterData<GDEWordScriptTriggerTypeData>.I.GetMasterData(GDEItemKeys.WordScriptTriggerType_WS_TRIGGER_TYPE_ARRIVAL));
							ponies.submeshObject.enabled = true;
						}
					};
					trackEntry.End += delegate
					{
						ponies.submeshObject.enabled = true;
						_nextState = typeof(FieStateMachineCommonIdle);
						_isEnd = true;
					};
					ponies.submeshObject.enabled = false;
					_arrivalState = ArrivalfState.STATE_PREPARE;
					gameCharacter.setFlip(FieObjectFlipState.Right);
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.isEnableAutoFlip = false;
				fiePonies.emotionController.StopAutoAnimation();
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FiePonies fiePonies = gameCharacter as FiePonies;
			if (!(fiePonies == null))
			{
				fiePonies.emotionController.RestoreEmotionFromDefaultData();
				fiePonies.emotionController.RestartAutoAnimation();
				fiePonies.setGravityRate(1f);
				fiePonies.isEnableHeadTracking = true;
				fiePonies.isEnableAutoFlip = true;
				fiePonies.isEnableGravity = true;
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
