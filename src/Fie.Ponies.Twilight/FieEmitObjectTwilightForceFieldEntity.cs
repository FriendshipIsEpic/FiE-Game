using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using GameDataEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightForceFieldEntity")]
	public class FieEmitObjectTwilightForceFieldEntity : FieEmittableObjectBase
	{
		[SerializeField]
		private Transform _forceFieldCenterTransform;

		[SerializeField]
		private float ForceFieldDuration = 12f;

		[SerializeField]
		private float ForceFieldDestroyDuration = 0.5f;

		[SerializeField]
		private float ForceFieldScaleAnimationDuration = 0.5f;

		[SerializeField]
		private float ShieldReginInterval = 1f;

		[SerializeField]
		private float ShieldReginPerInterval = 0.05f;

		[SerializeField]
		private GameObject _sphereObject;

		[SerializeField]
		private GameObject _normalSphereObject;

		[SerializeField]
		private AnimationCurve damageCurvePerLifeRatio;

		[SerializeField]
		private Texture2D defaultSphereTexture;

		[SerializeField]
		private Texture2D disruptorSphereTexture;

		[SerializeField]
		private Texture2D healingSphereTexture;

		private Tweener<TweenTypesInOutSine> _sphereScaleTweener = new Tweener<TweenTypesInOutSine>();

		private Tweener<TweenTypesOutSine> _sphereDeathTweener = new Tweener<TweenTypesOutSine>();

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		private float _angle;

		private Vector3 _initScale = Vector3.one;

		private float _currentDuration;

		private float _healingDelay;

		private float _healingRate;

		private Dictionary<int, Coroutine> _healRoutine = new Dictionary<int, Coroutine>();

		private Material _emissiveMaterial;

		private Material _normalMaterial;

		private FieStatusEffectsBuffAndDebuffToDeffence _debuffToDeffenceBuff;

		private IEnumerator HealingEffectCoroutine(int instanceID, float delay, float healingRate, FieGameCharacter targetCharacter)
		{
			if (targetCharacter != null)
			{
				targetCharacter.damageSystem.Regen((targetCharacter.healthStats.maxHitPoint + targetCharacter.healthStats.maxShield) * healingRate);
			}
			yield return (object)new WaitForSeconds(delay);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public void Awake()
		{
			_initScale = base.transform.localScale;
		}

		public override void awakeEmitObject()
		{
			_healRoutine = new Dictionary<int, Coroutine>();
			base.transform.localScale = _initScale;
			Vector3 vector = Vector3.one;
			Vector3 one = Vector3.one;
			float num = ForceFieldDuration;
			float forceFieldDuration = ForceFieldDuration;
			float num2 = 0f;
			float num3 = 1f;
			bool flag = false;
			bool flag2 = false;
			_debuffToDeffenceBuff = base.gameObject.GetComponent<FieStatusEffectsBuffAndDebuffToDeffence>();
			if (_debuffToDeffenceBuff != null)
			{
				_debuffToDeffenceBuff.isActive = false;
			}
			if (base.ownerCharacter != null)
			{
				GDESkillTreeData skill = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_MAGIC_BUBBLE_LV2_1);
				if (skill != null)
				{
					vector += one * skill.Value1;
				}
				GDESkillTreeData skill2 = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_MAGIC_BUBBLE_LV3_2);
				if (skill2 != null)
				{
					vector += one * skill2.Value1;
				}
				GDESkillTreeData skill3 = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_MAGIC_BUBBLE_LV1_2);
				if (skill3 != null)
				{
					num += forceFieldDuration * skill3.Value1;
				}
				GDESkillTreeData skill4 = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_MAGIC_BUBBLE_LV2_2);
				if (skill4 != null)
				{
					num += forceFieldDuration * skill4.Value1;
				}
				GDESkillTreeData skill5 = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_MAGIC_BUBBLE_LV4_HEALING_BUBBLE);
				if (skill5 != null)
				{
					num2 += skill5.Value1;
					num3 += skill5.Value2;
					flag = true;
				}
				else
				{
					GDESkillTreeData skill6 = base.ownerCharacter.GetSkill(FieConstValues.FieSkill.MAGIC_MAGIC_BUBBLE_LV4_DISRUPTOR_BUBBLE);
					if (skill6 != null)
					{
						if (_debuffToDeffenceBuff == null)
						{
							_debuffToDeffenceBuff = base.gameObject.AddComponent<FieStatusEffectsBuffAndDebuffToDeffence>();
						}
						_debuffToDeffenceBuff.isActive = true;
						_debuffToDeffenceBuff.duration = base.HitInterval;
						_debuffToDeffenceBuff.magni = 0f - skill6.Value1;
						_debuffToDeffenceBuff.skillID = skill6.ID;
						_debuffToDeffenceBuff.isEnableStack = true;
						flag2 = true;
					}
				}
			}
			_sphereScaleTweener.InitTweener(ForceFieldScaleAnimationDuration, _initScale, vector);
			_currentDuration = Mathf.Max(1f, num);
			_healingRate = num2;
			_healingDelay = num3;
			_emissiveMaterial = _sphereObject.GetComponent<Renderer>().material;
			_normalMaterial = _normalSphereObject.GetComponent<Renderer>().material;
			_emissiveMaterial.SetFloat("_Brightness", 1f);
			_emissiveMaterial.SetTexture("_Gradient_Color", flag ? healingSphereTexture : ((!flag2) ? defaultSphereTexture : disruptorSphereTexture));
			_normalMaterial.SetColor(0, Color.white);
		}

		public void Update()
		{
			if (_isEndUpdate)
			{
				float num = _sphereDeathTweener.UpdateParameterFloat(Time.deltaTime);
				if (_emissiveMaterial != null)
				{
					_emissiveMaterial.SetFloat("_Brightness", num);
				}
				if (_normalMaterial != null)
				{
					_normalMaterial.SetColor(0, new Color(num, num, num, num));
				}
			}
			else
			{
				base.transform.localScale = _sphereScaleTweener.UpdateParameterVec3(Time.deltaTime);
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= _currentDuration - ForceFieldDestroyDuration)
				{
					destoryEmitObject(ForceFieldDestroyDuration);
					_sphereDeathTweener.InitTweener(ForceFieldDestroyDuration, 1f, 0f);
					_isEndUpdate = true;
				}
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate)
			{
				FieEmittableObjectBase component = collider.GetComponent<FieEmittableObjectBase>();
				if (component != null && reflectEmitObject(component))
				{
					Vector3 vector = collider.ClosestPointOnBounds(base.transform.position);
					Vector3 vector2 = vector - _forceFieldCenterTransform.position;
					FieEmitObjectTwilightForceFieldReflectEffect fieEmitObjectTwilightForceFieldReflectEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightForceFieldReflectEffect>(base.transform, vector2.normalized);
					if (fieEmitObjectTwilightForceFieldReflectEffect != null)
					{
						fieEmitObjectTwilightForceFieldReflectEffect.transform.position = vector;
					}
				}
				FieCollider component2 = collider.gameObject.GetComponent<FieCollider>();
				if (!(component2 == null) && component2.isRoot)
				{
					FieGameCharacter parentGameCharacter = component2.getParentGameCharacter();
					if (!(parentGameCharacter == null))
					{
					}
				}
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			if (!_isEndUpdate)
			{
				if (collider.gameObject.tag == getHostileTagString() && _healingRate <= 0f)
				{
					FieDamage defaultDamageObject = getDefaultDamageObject();
					float num = damageCurvePerLifeRatio.Evaluate(_lifeTimeCount / (_currentDuration - ForceFieldDestroyDuration));
					defaultDamageObject.damage *= num;
					defaultDamageObject.stagger *= num;
					FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, defaultDamageObject);
				}
				else if (collider.gameObject.tag == getAllyTagString())
				{
					FieCollider component = collider.gameObject.GetComponent<FieCollider>();
					if (!(component == null))
					{
						FieGameCharacter parentGameCharacter = component.getParentGameCharacter();
						if (!(parentGameCharacter == null))
						{
							int instanceID = parentGameCharacter.GetInstanceID();
							_healRoutine.TryGetValue(instanceID, out Coroutine value);
							if (value == null)
							{
								value = StartCoroutine(HealingEffectCoroutine(instanceID, _healingDelay, _healingRate, parentGameCharacter));
								_healRoutine.Add(instanceID, value);
							}
						}
					}
				}
			}
		}
	}
}
