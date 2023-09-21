using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using Event = Spine.Event;

namespace Fie.Ponies.RainbowDash
{
	public class FieStateMachineRainbowDashBaseAttack1 : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			KICK_START,
			KICKING
		}

		private const float KICK_DELAY = 0.2f;

		private const float KICK_HORMING_DISTANCE = 1f;

		private const float KICK_HORMING_DEFAULT_RATE = 0.5f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private float _timeCount;

		private bool _isSetEndAnim;

		private bool _isFinished;

		private bool _isCancellable;

		private float _initializedDrag;

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				autoFlipToEnemy(fieRainbowDash);
				fieRainbowDash.Decloack();
				fieRainbowDash.isEnableHeadTracking = false;
				fieRainbowDash.isEnableAutoFlip = false;
				fieRainbowDash.isEnableGravity = false;
				fieRainbowDash.resetMoveForce();
				Rigidbody component = fieRainbowDash.GetComponent<Rigidbody>();
				if (component != null)
				{
					_initializedDrag = component.drag;
					if (fieRainbowDash.healthStats.shield >= 0f)
					{
						component.drag = 5f;
					}
				}
				GDESkillTreeData skill = fieRainbowDash.GetSkill(FieConstValues.FieSkill.LOYALTY_ATTACK_PASSIVE_LV4_SO_FAST_SO_COOL);
				if (skill != null)
				{
					fieRainbowDash.baseTimeScale = skill.Value1;
				}
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				fieRainbowDash.isEnableHeadTracking = true;
				fieRainbowDash.isEnableGravity = true;
				fieRainbowDash.isEnableAutoFlip = true;
				fieRainbowDash.isEnableCollider = true;
				fieRainbowDash.isEnableAutoFlip = true;
				fieRainbowDash.isEnableHeadTracking = true;
				fieRainbowDash.baseTimeScale = 1f;
				Rigidbody component = fieRainbowDash.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.drag = _initializedDrag;
				}
			}
		}

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieRainbowDash)
			{
				_timeCount += Time.deltaTime;
				FieRainbowDash rainbowDash = gameCharacter as FieRainbowDash;
				rainbowDash.damageSystem.setRegenerateDelay(Mathf.Max(1.5f, rainbowDash.healthStats.regenerateDelay));
				_nextState = ((rainbowDash.groundState != 0) ? typeof(FieStateMachineRainbowDashFlying) : typeof(FieStateMachineCommonIdle));
				switch (_fireState)
				{
				case FireState.KICK_START:
				{
					TrackEntry trackEntry = rainbowDash.animationManager.SetAnimation(26, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(TrackEntry state, Event trackIndex)
						{
							if (trackIndex.Data.Name == "fire")
							{
								FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashBaseAttack1>(rainbowDash.leftBackHoofTransform, Vector3.zero, null, rainbowDash);
							}
							if (trackIndex.Data.Name == "move")
							{
								Vector3 a = rainbowDash.flipDirectionVector;
								Transform lockonEnemyTransform = rainbowDash.detector.getLockonEnemyTransform(isCenter: true);
								float num = 0.5f;
								if (lockonEnemyTransform != null)
								{
									Vector3 vector = lockonEnemyTransform.position - rainbowDash.transform.position;
									num = Mathf.Min(Vector3.Distance(lockonEnemyTransform.position, rainbowDash.transform.position) / 1f, 1f);
									vector.Normalize();
									a = vector;
								}
								Vector3 vector2 = a * (trackIndex.Float * num);
								rainbowDash.resetMoveForce();
								rainbowDash.setMoveForce(vector2, 0f, useRound: false);
								if (rainbowDash.groundState == FieObjectGroundState.Grounding)
								{
									rainbowDash.setMoveForce(Vector3.up * trackIndex.Float * 0.2f, 0f, useRound: false);
								}
								rainbowDash.setFlipByVector(vector2);
							}
							if (trackIndex.Data.Name == "finished")
							{
								_isFinished = true;
							}
							if (trackIndex.Data.name == "cancellable")
							{
								_isCancellable = true;
							}
						};
						_endTime = trackEntry.animationEnd;
					}
					else
					{
						_endTime = 0f;
					}
					_fireState = FireState.KICKING;
					break;
				}
				}
				if (_timeCount > _endTime)
				{
					_isEnd = true;
				}
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
			if (_isFinished)
			{
				list.Add(typeof(FieStateMachineRainbowDashBaseAttack2));
			}
			if (_isCancellable)
			{
				list.Add(typeof(FieStateMachineRainbowDashEvasion));
				list.Add(typeof(FieStateMachineRainbowDashOmniSmashStart));
				list.Add(typeof(FieStateMachineRainbowDashDoublePaybackPrepareMidAir));
				list.Add(typeof(FieStateMachineRainbowDashDoublePaybackPrepareOnGround));
				list.Add(typeof(FieStateMachineRainbowDashRainblowCloack));
			}
			return list;
		}

		public override float getDelay()
		{
			return 0.2f;
		}

		public override bool isFinished()
		{
			return _isFinished;
		}
	}
}
