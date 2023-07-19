using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineEnemiesQueenChrysalisStagger : FieStateMachineGameCharacterBase
	{
		private enum StaggerState
		{
			STATE_PREPARE,
			STATE_STAGGER
		}

		private StaggerState _staggerState;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieEnemiesHoovesRaces)
			{
				FieEnemiesHoovesRaces fieEnemiesHoovesRaces = gameCharacter as FieEnemiesHoovesRaces;
				if (_staggerState == StaggerState.STATE_PREPARE)
				{
					int num = 3;
					_nextState = typeof(FieStateMachineQueenChrysalisStaggerFall);
					fieEnemiesHoovesRaces.isEnableGravity = false;
					fieEnemiesHoovesRaces.isEnableGravity = true;
					gameCharacter.setGravityRate(0.2f);
					Vector3 vector = Vector3.Normalize(fieEnemiesHoovesRaces.centerTransform.position - fieEnemiesHoovesRaces.latestDamageWorldPoint);
					vector.y *= 0.1f;
					vector.z = 0f;
					vector.Normalize();
					fieEnemiesHoovesRaces.setFlipByVector(vector * -1f);
					fieEnemiesHoovesRaces.setMoveForce((Vector3.up + vector).normalized * 8f, 1f, useRound: false);
					num = 4;
					fieEnemiesHoovesRaces.SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_8), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_9));
					TrackEntry trackEntry = fieEnemiesHoovesRaces.animationManager.SetAnimation(num, isLoop: false, isForceSet: true);
					trackEntry.mixDuration = 0f;
					if (trackEntry != null)
					{
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
					}
					else
					{
						_isEnd = true;
					}
					_staggerState = StaggerState.STATE_STAGGER;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieEnemiesHoovesRaces fieEnemiesHoovesRaces = gameCharacter as FieEnemiesHoovesRaces;
			if (!(fieEnemiesHoovesRaces == null))
			{
				fieEnemiesHoovesRaces.isEnableAutoFlip = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieEnemiesHoovesRaces fieEnemiesHoovesRaces = gameCharacter as FieEnemiesHoovesRaces;
			if (!(fieEnemiesHoovesRaces == null))
			{
				fieEnemiesHoovesRaces.setGravityRate(1f);
				fieEnemiesHoovesRaces.isEnableAutoFlip = true;
				fieEnemiesHoovesRaces.isEnableGravity = true;
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
			list.Add(typeof(FieStateMachineEnemiesQueenChrysalisStagger));
			return list;
		}
	}
}
