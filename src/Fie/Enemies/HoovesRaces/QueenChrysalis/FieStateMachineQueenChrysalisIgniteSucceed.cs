using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisIgniteSucceed : FieStateMachineGameCharacterBase
	{
		private enum SucceedState
		{
			SUCCEED_START,
			SUCCEED_END
		}

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private SucceedState _fireState;

		private bool _isEnd;

		private bool _isFinished;

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
			if (!(chrysalis == null))
			{
				switch (_fireState)
				{
				case SucceedState.SUCCEED_START:
				{
					TrackEntry trackEntry = chrysalis.animationManager.SetAnimation(19, isLoop: false, isForceSet: true);
					chrysalis.SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_1), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_2));
					if (trackEntry != null)
					{
						trackEntry.Complete += delegate
						{
							chrysalis.animationManager.SetAnimation(0, isLoop: true);
							_nextState = typeof(FieStateMachineCommonIdle);
							_isEnd = true;
						};
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "move")
							{
								Vector3 moveForce = chrysalis.flipDirectionVector * e.Float;
								chrysalis.setMoveForce(moveForce, 0f, useRound: false);
							}
							if (e.Data.Name == "finished")
							{
								_nextState = typeof(FieStateMachineCommonIdle);
								_isEnd = true;
							}
						};
					}
					else
					{
						_isEnd = true;
					}
					_fireState = SucceedState.SUCCEED_END;
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
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = true;
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
