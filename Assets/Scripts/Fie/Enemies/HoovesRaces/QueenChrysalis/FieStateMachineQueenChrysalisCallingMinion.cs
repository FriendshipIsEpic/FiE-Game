using Fie.Enemies.HoovesRaces.Changeling;
using Fie.Enemies.HoovesRaces.ChangelingAlpha;
using Fie.Enemies.HoovesRaces.Flightling;
using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisCallingMinion : FieStateMachineGameCharacterBase
	{
		private enum CallingState
		{
			CALLING_START,
			CALLING
		}

		private const float MELEE_HORMING_DISTANCE = 1f;

		private const float MELEE_HORMING_DEFAULT_RATE = 0.5f;

		private CallingState _meleeState;

		private bool _isEnd;

		private bool _isMidAirAttack;

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
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis chrysalis = gameCharacter as FieQueenChrysalis;
			if (chrysalis == null)
			{
				_isEnd = true;
			}
			else if (_meleeState == CallingState.CALLING_START)
			{
				if (!chrysalis.canAbleToCallMinion)
				{
					_isEnd = true;
				}
				else
				{
					_isMidAirAttack |= (chrysalis.groundState == FieObjectGroundState.Flying);
					TrackEntry trackEntry = null;
					if (!_isMidAirAttack)
					{
						trackEntry = chrysalis.animationManager.SetAnimation(14, isLoop: false, isForceSet: true);
					}
					else
					{
						trackEntry = chrysalis.animationManager.SetAnimation(13, isLoop: false, isForceSet: true);
						chrysalis.isEnableGravity = false;
					}
					if (trackEntry != null)
					{
						chrysalis.SetDialog(100, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_6), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_E_THE_INSECT_QUEEN_USING_ABILITY_7));
						trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
						{
							if (trackIndex.Data.Name == "fire")
							{
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisCommonActivationEffect>(chrysalis.leftFrontHoofTransform, Vector3.zero, null);
								FieGameCharacter[] currentMinions = chrysalis.currentMinions;
								for (int i = 0; i < chrysalis.maximumCallingMinionCount; i++)
								{
									currentMinions[i] = CreateEnemy(chrysalis);
									if (currentMinions[i] != null)
									{
										currentMinions[i].healthStats.maxShield *= 0.5f;
										currentMinions[i].healthStats.shield *= 0.5f;
										currentMinions[i].healthStats.maxHitPoint *= 0.5f;
										currentMinions[i].healthStats.hitPoint *= 0.5f;
										currentMinions[i].expRate = 0.1f;
									}
								}
								chrysalis.SetCalledMinionDelay();
							}
						};
						trackEntry.Complete += delegate
						{
							_isEnd = true;
						};
					}
					_meleeState = CallingState.CALLING;
				}
			}
		}

		private FieGameCharacter CreateEnemy(FieQueenChrysalis chrysalis)
		{
			Type enemyType = typeof(FieChangeling);
			Lottery<Type> lottery = new Lottery<Type>();
			FieEnvironmentManager.Difficulty currentDifficulty = FieManagerBehaviour<FieEnvironmentManager>.I.currentDifficulty;
			if (chrysalis.healthStats.shield > 0f)
			{
				if (currentDifficulty >= FieEnvironmentManager.Difficulty.NIGHTMARE)
				{
					lottery.AddItem(typeof(FieChangeling), 50);
					lottery.AddItem(typeof(FieFlightling), 50);
					if (chrysalis.currentMinionCount <= 0 && currentDifficulty >= FieEnvironmentManager.Difficulty.NORMAL)
					{
						lottery.AddItem(typeof(FieChangelingAlpha), 50 * (int)currentDifficulty);
					}
				}
				else
				{
					lottery.AddItem(typeof(FieChangeling), 100);
					lottery.AddItem(typeof(FieFlightling), 100);
				}
			}
			else
			{
				float num = chrysalis.healthStats.hitPoint / chrysalis.healthStats.maxHitPoint;
				if (num > 0.5f)
				{
					lottery.AddItem(typeof(FieChangeling), 200);
					lottery.AddItem(typeof(FieFlightling), 100);
					if (chrysalis.currentMinionCount <= 0 && currentDifficulty >= FieEnvironmentManager.Difficulty.NORMAL)
					{
						lottery.AddItem(typeof(FieChangelingAlpha), 50 * (int)currentDifficulty);
					}
				}
				else if (currentDifficulty >= FieEnvironmentManager.Difficulty.CHAOS)
				{
					lottery.AddItem(typeof(FieChangelingAlpha), 100);
				}
				else
				{
					lottery.AddItem(typeof(FieChangeling), 100);
					lottery.AddItem(typeof(FieFlightling), 100);
					if (chrysalis.currentMinionCount <= 0 && currentDifficulty >= FieEnvironmentManager.Difficulty.NORMAL)
					{
						lottery.AddItem(typeof(FieChangelingAlpha), 200 * (int)currentDifficulty);
					}
				}
			}
			if (lottery.IsExecutable())
			{
				enemyType = lottery.Lot();
			}
			Vector3 normalized = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f)).normalized;
			Vector3 vector = chrysalis.position + normalized * 1f;
			int layerMask = 1049088;
			if (Physics.Raycast(chrysalis.centerTransform.position, normalized, out RaycastHit hitInfo, 1f, layerMask))
			{
				vector = hitInfo.point;
			}
			layerMask = 512;
			if (Physics.Raycast(vector, Vector3.down, out hitInfo, 20f, layerMask) && hitInfo.collider.tag == "Floor")
			{
				vector = hitInfo.point;
			}
			return FieManagerBehaviour<FieInGameEnemyManager>.I.CreateEnemyOnlyMasterClienty(enemyType, vector);
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
