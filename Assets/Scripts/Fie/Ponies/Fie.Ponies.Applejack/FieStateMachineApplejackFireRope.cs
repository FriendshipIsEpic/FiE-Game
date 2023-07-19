using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	public class FieStateMachineApplejackFireRope : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			ROPE_READY,
			ROPE_FIRE,
			ROPE_WAITING,
			ROPE_PULL,
			ROPE_END
		}

		public float pullDuration = 0.75f;

		private const float STAGGER = 100f;

		private const float HATE = 5f;

		private const float PUNCH_DELAY = 0.5f;

		private const float GROUND_FORCE_TIME = 1.5f;

		private const float GROUND_FORCE = 15f;

		private const float AIR_ROPE_FLYING_FORCE = 4f;

		private const float AIR_ROPE_PAST_TIME = 0.3f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private float _timeCount;

		private bool _isSetEndAnim;

		private bool _isFinished;

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieApplejack)
			{
				_timeCount += Time.deltaTime;
				FieApplejack applejack = gameCharacter as FieApplejack;
				FireState fireState = _fireState;
				switch (fireState)
				{
				case FireState.ROPE_READY:
				{
					bool isFlyingAction = false;
					int animationId = (applejack.groundState != 0) ? 32 : 31;
					if (applejack.groundState == FieObjectGroundState.Flying)
					{
						isFlyingAction = true;
					}
					applejack.resetMoveForce();
					applejack.setGravityRate(0.2f);
					autoFlipToEnemy(applejack);
					TrackEntry trackEntry = applejack.animationManager.SetAnimation(animationId, isLoop: false, isForceSet: true);
					applejack.emotionController.SetEmoteAnimation(41, isForceSet: true);
					applejack.SetDialog(30, FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_P_HONESTY_ABILITY_ROPE_1), FieMasterData<GDEWordScriptsListData>.I.GetMasterData(GDEItemKeys.WordScriptsList_P_HONESTY_ABILITY_ROPE_2));
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "fire")
							{
								Vector3 directionalVec = (applejack.flipState != 0) ? Vector3.right : Vector3.left;
								FieEmitObjectApplejackRope fieEmitObjectApplejackRope = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectApplejackRope>(applejack.mouthTransform, directionalVec, applejack.detector.getLockonEnemyTransform(), applejack);
								if (!(fieEmitObjectApplejackRope == null))
								{
									GDESkillTreeData skill = applejack.GetSkill(FieConstValues.FieSkill.HONESTY_ROPE_LV3_1);
									if (skill != null)
									{
										applejack.damageSystem.AddDefenceMagni(1005, 1f, skill.Value1);
									}
									GDESkillTreeData skill2 = applejack.GetSkill(FieConstValues.FieSkill.HONESTY_ROPE_LV4_SYNERGY_ROPING);
									if (skill2 != null)
									{
										applejack.abilitiesContainer.IncreaseOrReduceCooldownAll(0f - skill2.Value1);
									}
									fieEmitObjectApplejackRope.setPullPosition(applejack.transform.position);
									fieEmitObjectApplejackRope.emitObjectHitEvent += delegate(FieEmittableObjectBase emitObject, FieGameCharacter hitCharacter)
									{
										if (isFlyingAction)
										{
											(applejack.getStateMachine().setState(typeof(FieStateMachineApplejackRopeActionAir), isForceSet: false) as FieStateMachineGameCharacterBase)?.addTargetGameCharacter(hitCharacter);
											_isEnd = true;
										}
										else
										{
											FieStateMachineGameCharacterBase fieStateMachineGameCharacterBase = applejack.getStateMachine().setState(typeof(FieStateMachineApplejackRopeAction), isForceSet: false) as FieStateMachineGameCharacterBase;
											if (fieStateMachineGameCharacterBase != null)
											{
												FieDamage fieDamage = new FieDamage(100f, 5f);
												FieStatusEffectsPullEntity item = new FieStatusEffectsPullEntity
												{
													pullPosition = applejack.transform.position,
													pullDuration = pullDuration
												};
												fieDamage.statusEffects.Add(item);
												hitCharacter.AddDamage(applejack, fieDamage);
												fieStateMachineGameCharacterBase.addTargetGameCharacter(hitCharacter);
											}
											_isEnd = true;
										}
									};
								}
							}
							else if (e.Data.Name == "takeOff")
							{
								Vector3 up = Vector3.up;
								up.Normalize();
								up *= 4f;
								applejack.setMoveForce(up, 0.3f);
							}
							else if (e.Data.Name == "finished")
							{
								_nextState = typeof(FieStateMachineApplejackFlying);
								_isEnd = true;
							}
						};
						trackEntry.Complete += delegate
						{
							_isEnd = true;
						};
						_endTime = trackEntry.endTime;
					}
					else
					{
						applejack.setGravityRate(1f);
						applejack.isEnableGravity = true;
						_endTime = 0f;
					}
					_fireState = FireState.ROPE_FIRE;
					break;
				}
				}
				if (_timeCount > _endTime)
				{
					_isEnd = true;
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieApplejack fieApplejack = gameCharacter as FieApplejack;
			if (!(fieApplejack == null))
			{
				fieApplejack.isEnableHeadTracking = false;
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieApplejack fieApplejack = gameCharacter as FieApplejack;
			if (!(fieApplejack == null))
			{
				fieApplejack.setGravityRate(1f);
				fieApplejack.isEnableGravity = true;
				fieApplejack.isEnableHeadTracking = true;
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
			list.Add(typeof(FieStateMachineApplejackRopeActionAir));
			list.Add(typeof(FieStateMachineApplejackRopeAction));
			return list;
		}

		public override float getDelay()
		{
			return 0.5f;
		}

		public override bool isFinished()
		{
			return _isFinished;
		}
	}
}
