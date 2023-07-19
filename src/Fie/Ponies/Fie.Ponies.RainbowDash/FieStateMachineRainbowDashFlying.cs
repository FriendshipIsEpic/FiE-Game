using Fie.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	public class FieStateMachineRainbowDashFlying : FieStateMachineGameCharacterBase
	{
		public const float FLYING_DRAG = 5f;

		private const float FLYING_INPUT_THLESHOLD = 0.3f;

		private const float FLYING_VELOCITY_PER_SEC = 25f;

		private const float FLYING_SHIELD_COST_PER_SEC = 0.02f;

		private const float FLYING_SHIELD_REGEN_DELAY = 1.5f;

		private float _speedAngle;

		private float _initializedDrag;

		private bool _isEnd;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		public override void updateState<T>(ref T gameCharacter)
		{
			if (gameCharacter is FieRainbowDash)
			{
				FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
				Vector3 externalInputVector = fieRainbowDash.externalInputVector;
				float externalInputForce = fieRainbowDash.externalInputForce;
				externalInputVector.z = 0f;
				if (fieRainbowDash.animationManager.IsEndAnimation(0))
				{
					fieRainbowDash.animationManager.SetAnimation(20, isLoop: true);
				}
				if (fieRainbowDash.groundState == FieObjectGroundState.Grounding)
				{
					_nextState = typeof(FieStateMachinePoniesLanding);
					_isEnd = true;
				}
				else
				{
					if (fieRainbowDash.healthStats.shield <= 0f)
					{
						externalInputVector.y = 0f;
						fieRainbowDash.isEnableGravity = true;
					}
					else
					{
						fieRainbowDash.damageSystem.setRegenerateDelay(Mathf.Max(1.5f, fieRainbowDash.healthStats.regenerateDelay));
						fieRainbowDash.isEnableGravity = false;
					}
					if (externalInputForce > 0.3f)
					{
						Vector3 moveForce = externalInputVector * 25f * externalInputForce;
						fieRainbowDash.addMoveForce(moveForce, 1f);
						fieRainbowDash.physicalForce.SetPhysicalForce(gameCharacter.getNowMoveForce() * -1f, 2500f);
					}
					if (externalInputForce > 0.5f && Mathf.Abs(externalInputVector.y) < 0.5f)
					{
						_speedAngle += 90f * Time.deltaTime * externalInputForce;
					}
					else
					{
						_speedAngle -= 90f * Time.deltaTime;
					}
					_speedAngle = Mathf.Clamp(_speedAngle, 0f, 20f);
					fieRainbowDash.rootBone.transform.localRotation = Quaternion.AngleAxis(_speedAngle, Vector3.forward);
					fieRainbowDash.damageSystem.calcShieldDirect((0f - fieRainbowDash.healthStats.maxShield) * 0.02f * Time.deltaTime);
				}
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				fieRainbowDash.isEnableAutoFlip = true;
				Rigidbody component = fieRainbowDash.GetComponent<Rigidbody>();
				if (component != null)
				{
					_initializedDrag = component.drag;
					if (fieRainbowDash.healthStats.shield >= 0f)
					{
						component.drag = 5f;
					}
				}
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			FieRainbowDash fieRainbowDash = gameCharacter as FieRainbowDash;
			if (!(fieRainbowDash == null))
			{
				fieRainbowDash.setGravityRate(1f);
				fieRainbowDash.isEnableGravity = true;
				fieRainbowDash.isEnableAutoFlip = true;
				Rigidbody component = fieRainbowDash.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.drag = _initializedDrag;
				}
				fieRainbowDash.rootBone.transform.localRotation = Quaternion.AngleAxis(0f, Vector3.forward);
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
			list.Add(typeof(FieStateMachineRainbowDashBaseAttack1));
			list.Add(typeof(FieStateMachineRainbowDashBaseAttack3));
			list.Add(typeof(FieStateMachineRainbowDashStabAttack));
			list.Add(typeof(FieStateMachineRainbowDashEvasion));
			list.Add(typeof(FieStateMachineRainbowDashRainblowCloack));
			list.Add(typeof(FieStateMachineRainbowDashDoublePaybackPrepareMidAir));
			list.Add(typeof(FieStateMachineRainbowDashOmniSmashStart));
			return list;
		}
	}
}
