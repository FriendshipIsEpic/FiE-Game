using Fie.Manager;
using Fie.Object;
using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	public class FieStateMachineQueenChrysalisMeteorShowerAttacking : FieStateMachineGameCharacterBase
	{
		private enum AttackingState
		{
			METEOR_SHOWER_ATTACKING_START,
			METEOR_SHOWER_ATTACKING,
			METEOR_SHOWER_ATTACKING_END
		}

		private const float METEOR_EMITTING_DURATION = 6f;

		private const float METEOR_EMITTING_INTERVAL = 0.5f;

		private const float HORMING_SHOT_EMITTING_INTERVAL = 1f;

		private const float METEOR_EMITTING_DISTANCE_MAX = 8f;

		private Type _nextState = typeof(FieStateMachineCommonIdle);

		private AttackingState _fireState;

		private bool _isEnd;

		private bool _isFinished;

		private FieEmitObjectQueenChrysalisMeteorShowerEmittingEffect _emittingEffect;

		private FieEmitObjectQueenChrysalisHornEffect _hornEffect;

		private float _lifeTimeCount;

		private float _meteorEmittingDelay;

		private float _hormingShotEmittingDelay;

		private int _meteorEmittedCount;

		private float _initializedDrag;

		public override void updateState<T>(ref T gameCharacter)
		{
			FieQueenChrysalis fieQueenChrysalis = gameCharacter as FieQueenChrysalis;
			if (!(fieQueenChrysalis == null))
			{
				switch (_fireState)
				{
				case AttackingState.METEOR_SHOWER_ATTACKING_START:
				{
					TrackEntry trackEntry = fieQueenChrysalis.animationManager.SetAnimation(25, isLoop: true, isForceSet: true);
					if (trackEntry == null)
					{
						_isEnd = true;
					}
					else
					{
						_hornEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHornEffect>(fieQueenChrysalis.hornTransform, Vector3.zero, null);
						_emittingEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisMeteorShowerEmittingEffect>(fieQueenChrysalis.leftFrontHoofTransform, Vector3.zero, null);
						_fireState = AttackingState.METEOR_SHOWER_ATTACKING;
					}
					break;
				}
				case AttackingState.METEOR_SHOWER_ATTACKING:
					_lifeTimeCount += Time.deltaTime;
					_meteorEmittingDelay -= Time.deltaTime;
					if (_meteorEmittingDelay <= 0f)
					{
						EmitMeteor(fieQueenChrysalis);
						_meteorEmittingDelay = 0.5f;
					}
					_hormingShotEmittingDelay -= Time.deltaTime;
					if (_hormingShotEmittingDelay <= 0f)
					{
						FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHormingShot>(fieQueenChrysalis.leftFrontHoofTransform, fieQueenChrysalis.flipDirectionVector, fieQueenChrysalis.detector.getLockonEnemyTransform(isCenter: true), fieQueenChrysalis);
						_hormingShotEmittingDelay = 1f;
					}
					if (_lifeTimeCount > 6f)
					{
						_fireState = AttackingState.METEOR_SHOWER_ATTACKING_END;
					}
					break;
				case AttackingState.METEOR_SHOWER_ATTACKING_END:
					_nextState = typeof(FieStateMachineQueenChrysalisMeteorShowerFinished);
					_isEnd = true;
					break;
				}
			}
		}

		private void EmitMeteor(FieQueenChrysalis chrysalis)
		{
			if (!(chrysalis == null))
			{
				Vector3 position = chrysalis.transform.position;
				int num = Mathf.FloorToInt(position.x * 10f);
				Vector3 position2 = chrysalis.transform.position;
				int num2 = num + Mathf.FloorToInt(position2.y * 10f);
				Vector3 position3 = chrysalis.transform.position;
				int seed = num2 + Mathf.FloorToInt(position3.z * 10f) + _meteorEmittedCount;
				UnityEngine.Random.InitState(seed);
				float z = UnityEngine.Random.Range(-0.5f, 0.5f);
				UnityEngine.Random.InitState(seed);
				float num3 = UnityEngine.Random.Range(-10f, 10f);
				Vector3 vector = -chrysalis.flipDirectionVector;
				vector.z = z;
				vector.Normalize();
				Vector3 vector2 = chrysalis.centerTransform.position + vector * num3;
				int layerMask = 1049088;
				if (Physics.Raycast(chrysalis.centerTransform.position, vector, out RaycastHit hitInfo, num3, layerMask))
				{
					vector2 = hitInfo.point;
					vector2 -= vector;
				}
				vector2.y += 4f;
				FieEmitObjectQueenChrysalisMeteorShowerMeteor fieEmitObjectQueenChrysalisMeteorShowerMeteor = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisMeteorShowerMeteor>(chrysalis.centerTransform, chrysalis.flipDirectionVector, null, chrysalis);
				if (fieEmitObjectQueenChrysalisMeteorShowerMeteor != null)
				{
					fieEmitObjectQueenChrysalisMeteorShowerMeteor.transform.position = vector2;
				}
				_meteorEmittedCount++;
			}
		}

		public override void initialize(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableAutoFlip = false;
				gameCharacter.isEnableGravity = false;
				gameCharacter.resetMoveForce();
				Rigidbody component = gameCharacter.GetComponent<Rigidbody>();
				if (component != null)
				{
					_initializedDrag = component.drag;
					component.drag = 500f;
				}
			}
		}

		public override void terminate(FieGameCharacter gameCharacter)
		{
			if (!(gameCharacter == null))
			{
				gameCharacter.isEnableGravity = true;
				gameCharacter.isEnableAutoFlip = true;
				gameCharacter.resetMoveForce();
				if (_emittingEffect != null)
				{
					_emittingEffect.StopEffect();
				}
				if (_hornEffect != null)
				{
					_hornEffect.Kill();
				}
				Rigidbody component = gameCharacter.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.drag = _initializedDrag;
				}
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
