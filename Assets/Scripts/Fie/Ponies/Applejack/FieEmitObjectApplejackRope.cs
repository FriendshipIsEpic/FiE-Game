using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using UnityEngine;

namespace Fie.Ponies.Applejack
{
	[FiePrefabInfo("Prefabs/Applejack/Power/ApplejackRope")]
	public class FieEmitObjectApplejackRope : FieEmittableObjectBase
	{
		private enum RopeState
		{
			ROPE_READY,
			ROPE_SHOOTING,
			ROPE_HITTING,
			ROPE_END
		}

		public float pullDuration = 0.5f;

		private const float ROPE_CURVE_MINIMUM = 0.0001f;

		private const float ROPE_CURVE_INIT = 0.001f;

		private const float ROPE_SCALE = 0.025f;

		private const float ROPE_END_SCALE = 0f;

		private float ROPE_MAX_RANGE = 10f;

		private RopeState _ropeState;

		[SerializeField]
		private float RopeShotTime = 0.2f;

		[SerializeField]
		private float RopeDuration = 1f;

		[SerializeField]
		private float RopeDestroyPastTime = 0.8f;

		[SerializeField]
		private float RopeStabilityHeight = 4f;

		[SerializeField]
		private PhysicMaterial _ropePhysicalMaterial;

		[SerializeField]
		private GameObject _ropeOrigin;

		[SerializeField]
		private GameObject _ropeTop;

		[SerializeField]
		private GameObject _ropeBackOrigin;

		[SerializeField]
		private GameObject _ropeBackTop;

		private Vector3 _velocityVec = Vector3.zero;

		private Vector3 _lastDirectionalVec = Vector3.zero;

		private float _lifeTimeCount;

		private float _minDistance = 3.40282347E+38f;

		private bool _isEndUpdate;

		private Vector3 _startPosition = Vector3.zero;

		private Vector3 _targetPosition = Vector3.zero;

		private Tweener<TweenTypesOutSine> _ropeDistanceTweener = new Tweener<TweenTypesOutSine>();

		private Tweener<TweenTypesInSine> _ropeScaleTweener = new Tweener<TweenTypesInSine>();

		private Tweener<TweenTypesOutSine> _ropeShotStabirityTweener = new Tweener<TweenTypesOutSine>();

		private QuickRope2 _ropeObject;

		private QuickRope2 _backRopeObject;

		private QuickRope2Mesh _ropeMesh;

		private QuickRope2Mesh _backRopeMesh;

		private float _currentRopeCurve = 0.001f;

		private float _lastRopeCurve;

		private float _lastLopeScale;

		private Vector3 _targetPositionScatter = Vector3.zero;

		private Vector3 _pullPosition = Vector3.zero;

		private FieGameCharacter _hitGameCharacter;

		private FieStatusEffectsTimeScaler _timeScalerStatusEffectEntity;

		public void setPullPosition(Vector3 pullPosition)
		{
			_pullPosition = pullPosition;
		}

		private void Awake()
		{
			_ropeObject = QuickRope2.Create(_ropeOrigin, _ropeTop, BasicRopeTypes.Mesh);
			_backRopeObject = QuickRope2.Create(_ropeBackOrigin, _ropeBackTop, BasicRopeTypes.Mesh);
			_ropeObject.mass = 1f;
			_ropeObject.angDrag = 1f;
			_ropeObject.drag = 1f;
			_backRopeObject.mass = 1f;
			_backRopeObject.AngZLimit = (_ropeObject.AngZLimit = 45f);
			_backRopeObject.LowAngXLimit = (_ropeObject.LowAngXLimit = 45f);
			_backRopeObject.AngYLimit = (_ropeObject.AngYLimit = 45f);
			_backRopeObject.LTLDamper = (_ropeObject.LTLDamper = 50f);
			_backRopeObject.S1LDamper = (_ropeObject.S1LDamper = 50f);
			_ropeObject.jointSpacing = 0.2f;
			_ropeObject.enablePhysics = true;
			_ropeObject.colliderRadius = 0.02f;
			_ropeObject.solverOverride = 20;
			_ropeObject.colliderType = RopeColliderType.Capsule;
			_ropeObject.physicsMaterial = _ropePhysicalMaterial;
			_ropeObject.ApplyRopeSettings();
			_backRopeObject.jointSpacing = 0.1f;
			_backRopeObject.useGravity = true;
			_backRopeObject.enablePhysics = true;
			_backRopeObject.solverOverride = 20;
			_backRopeObject.physicsMaterial = _ropePhysicalMaterial;
			_backRopeObject.ApplyRopeSettings();
			_ropeMesh = _ropeObject.GetComponent<QuickRope2Mesh>();
			_backRopeMesh = _backRopeObject.GetComponent<QuickRope2Mesh>();
		}

		public override void awakeEmitObject()
		{
			Transform transform = base.transform;
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = base.transform.position;
			float y = position2.y;
			Vector3 position3 = base.transform.position;
			transform.position = new Vector3(x, y, position3.z - 0.1f);
			float ropeStabilityHeight = RopeStabilityHeight;
			if (targetTransform != null)
			{
				_targetPosition = targetTransform.position;
				FieGameCharacter component = targetTransform.GetComponent<FieGameCharacter>();
				if (component != null)
				{
					_targetPositionScatter = component.centerTransform.position - _targetPosition;
				}
				ropeStabilityHeight = RopeStabilityHeight * Vector3.Distance(targetTransform.position, initTransform.position) / ROPE_MAX_RANGE;
			}
			else
			{
				_targetPosition = initTransform.position + directionalVec.normalized * ROPE_MAX_RANGE;
				ropeStabilityHeight = 0f;
			}
			_ropeShotStabirityTweener.InitTweener(RopeShotTime, ropeStabilityHeight, 0f);
			_targetPosition.y += ropeStabilityHeight;
			_ropeOrigin.transform.position = initTransform.position;
			_ropeTop.transform.position = _targetPosition;
			_ropeBackOrigin.transform.position = initTransform.position;
			_ropeBackTop.transform.position = initTransform.position;
			updateRopeRange(_ropeMesh, 0.001f);
			updateRopeRange(_backRopeMesh, 1f);
			_ropeObject.ApplyRopeSettings();
			_backRopeObject.ApplyRopeSettings();
			_ropeDistanceTweener.InitTweener(RopeShotTime, 0.001f, 1f);
			_ropeScaleTweener.InitTweener(1f, 0.025f, 0.025f);
			_ropeState = RopeState.ROPE_READY;
			_hitGameCharacter = null;
			if (base.ownerCharacter != null)
			{
				GDESkillTreeData skill = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.HONESTY_ROPE_LV1_2);
				if (skill != null)
				{
					defaultDamage += skill.Value1;
				}
				GDESkillTreeData skill2 = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.HONESTY_ROPE_LV2_2);
				if (skill2 != null)
				{
					defaultDamage += skill2.Value1;
				}
				_timeScalerStatusEffectEntity = base.gameObject.GetComponent<FieStatusEffectsTimeScaler>();
				if (_timeScalerStatusEffectEntity != null)
				{
					_timeScalerStatusEffectEntity.isActive = false;
				}
				if (_timeScalerStatusEffectEntity != null)
				{
					_timeScalerStatusEffectEntity.isActive = false;
				}
				GDESkillTreeData skill3 = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.HONESTY_ROPE_LV4_SLOW_DOWN);
				if (skill3 != null)
				{
					if (_timeScalerStatusEffectEntity == null)
					{
						_timeScalerStatusEffectEntity = base.gameObject.AddComponent<FieStatusEffectsTimeScaler>();
					}
					_timeScalerStatusEffectEntity.isActive = true;
					_timeScalerStatusEffectEntity.duration = skill3.Value1;
					_timeScalerStatusEffectEntity.targetTimeScale = skill3.Value2;
				}
			}
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_ropeBackOrigin.transform.position = initTransform.position;
			}
			else
			{
				_ropeBackOrigin.transform.position = _ropeOrigin.transform.position;
			}
			updateRopeRange(_ropeMesh, _ropeDistanceTweener.UpdateParameterFloat(Time.deltaTime), _ropeScaleTweener.UpdateParameterFloat(Time.deltaTime));
			updateRopeRange(_backRopeMesh, 1f, _ropeScaleTweener.UpdateParameterFloat(Time.deltaTime));
			if (!_isEndUpdate)
			{
				_ropeOrigin.transform.position = initTransform.position;
				Vector3 position = Vector3.zero;
				if (targetTransform != null)
				{
					position = targetTransform.position + _targetPositionScatter;
					if (Vector3.Distance(targetTransform.position, initTransform.position) > ROPE_MAX_RANGE)
					{
						finishTransaction();
					}
				}
				else
				{
					position = _targetPosition;
				}
				position.y += _ropeShotStabirityTweener.UpdateParameterFloat(Time.deltaTime);
				_ropeTop.transform.position = position;
				switch (_ropeState)
				{
				case RopeState.ROPE_READY:
					_ropeState = RopeState.ROPE_SHOOTING;
					break;
				case RopeState.ROPE_SHOOTING:
					if (_lifeTimeCount >= RopeShotTime)
					{
						bool flag = false;
						if (targetTransform != null)
						{
							FieGameCharacter component = targetTransform.GetComponent<FieGameCharacter>();
							if (component != null)
							{
								callHitEvent(component);
								if (base.ownerCharacter != null)
								{
									GDESkillTreeData skill = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.HONESTY_ROPE_LV3_2);
									if (skill != null)
									{
										component.damageSystem.AddDefenceMagni(1006, 0f - skill.Value2, skill.Value1);
									}
								}
								addDamageToCharacter(component, getDefaultDamageObject());
								flag = true;
							}
						}
						if (!flag)
						{
							finishTransaction();
							_ropeState = RopeState.ROPE_END;
						}
						else
						{
							_ropeState = RopeState.ROPE_HITTING;
						}
					}
					break;
				}
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= RopeDuration)
				{
					finishTransaction();
				}
			}
		}

		private void updateRopeRange(QuickRope2Mesh ropeMesh, float ropeCurveRange, float ropeScale = 0.025f)
		{
			float num = Mathf.Max(Mathf.Min(ropeCurveRange, 1f), 0.001f);
			float num2 = Mathf.Max(Mathf.Min(ropeCurveRange + 0.0001f, 1f), 0.001f);
			if (num != _lastRopeCurve || ropeScale != _lastLopeScale)
			{
				ropeMesh.curve = new AnimationCurve();
				ropeMesh.curve.AddKey(0f, ropeScale);
				ropeMesh.curve.AddKey(num, ropeScale);
				if (num2 != num)
				{
					ropeMesh.curve.AddKey(num2, 0f);
				}
				for (int i = 0; i < ropeMesh.curve.keys.Length; i++)
				{
					ropeMesh.curve.SmoothTangents(i, 1f);
				}
				_lastRopeCurve = num;
				_lastLopeScale = ropeScale;
			}
		}

		private void finishTransaction()
		{
			Rigidbody component = _ropeBackOrigin.GetComponent<Rigidbody>();
			bool flag = true;
			_ropeTop.GetComponent<Rigidbody>().useGravity = flag;
			flag = flag;
			_ropeOrigin.GetComponent<Rigidbody>().useGravity = flag;
			component.useGravity = flag;
			Rigidbody component2 = _ropeBackOrigin.GetComponent<Rigidbody>();
			flag = false;
			_ropeTop.GetComponent<Rigidbody>().isKinematic = flag;
			flag = flag;
			_ropeOrigin.GetComponent<Rigidbody>().isKinematic = flag;
			component2.isKinematic = flag;
			destoryEmitObject(RopeDestroyPastTime);
			_ropeScaleTweener.InitTweener(RopeDestroyPastTime, 0.025f, 0f);
			_isEndUpdate = true;
		}
	}
}
