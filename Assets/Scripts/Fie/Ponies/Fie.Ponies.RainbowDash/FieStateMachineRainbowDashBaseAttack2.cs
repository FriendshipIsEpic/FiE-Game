using Fie.Manager;
using Fie.Object;
using GameDataEditor;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	public class FieStateMachineRainbowDashBaseAttack2 : FieStateMachineGameCharacterBase
	{
		private enum FireState
		{
			PUNCH_START,
			PUNCHING
		}

		private const float PUNCH_DELAY = 0.5f;

		private const float PUNCH_HORMING_DISTANCE = 1f;

		private const float PUNCH_HORMING_DEFAULT_RATE = 0.5f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private FireState _fireState;

		private bool _isEnd;

		private float _endTime = 3.40282347E+38f;

		private float _timeCount;

		private bool _isSetEndAnim;

		private bool _isFinished;

		private bool _isCancellable;

		private int _punchCount;

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
				case FireState.PUNCH_START:
				{
					TrackEntry trackEntry = rainbowDash.animationManager.SetAnimation(27, isLoop: false, isForceSet: true);
					if (trackEntry != null)
					{
						trackEntry.Event += delegate(Spine.AnimationState state, int trackIndex, Spine.Event e)
						{
							if (e.Data.Name == "fire")
							{
								switch (_punchCount)
								{
								case 0:
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashBaseAttack2>(rainbowDash.leftFrontHoofTransform, rainbowDash.flipDirectionVector, null, rainbowDash);
									break;
								case 1:
									FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashBaseAttack2>(rainbowDash.rightFrontHoofTransform, rainbowDash.flipDirectionVector, null, rainbowDash);
									break;
								}
								_punchCount++;
							}
							if (e.Data.Name == "move")
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
								Vector3 vector2 = a * (e.Float * num);
								rainbowDash.resetMoveForce();
								rainbowDash.setMoveForce(vector2, 0f, useRound: false);
								rainbowDash.setFlipByVector(vector2);
							}
							if (e.Data.Name == "finished")
							{
								_isFinished = true;
							}
							if (e.Data.name == "cancellable")
							{
								_isCancellable = true;
							}
						};
						_endTime = trackEntry.endTime;
					}
					else
					{
						_endTime = 0f;
					}
					_fireState = FireState.PUNCHING;
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
				list.Add(typeof(FieStateMachineRainbowDashBaseAttack3));
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
			return 0.5f;
		}

		public override bool isFinished()
		{
			return _isFinished;
		}
	}
}
