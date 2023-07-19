using Fie.Object;
using Fie.Utility;
using System;
using UnityEngine;

namespace Fie.UI
{
	[RequireComponent(typeof(PKFxFX))]
	public class FieGameUIDamageCounter : FieGameUIBase
	{
		private const string FX_DAMAGE_VALUE_NAME = "Value";

		private const string FX_SIZE_RATIO_NAME = "SizeRatio";

		private const string FX_IS_EFFECTIVE_NAME = "IsEffective";

		private const string FX_WIGGLE_VEC_NAME = "WiggleVec";

		private const string FX_RGB_NAME = "RGB";

		private const string FX_EFFECTIVE_RGB_NAME = "EffectiveRGB";

		[SerializeField]
		private Color defaultDamageColor;

		[SerializeField]
		private Color earthDamageColor;

		[SerializeField]
		private Color magicDamageColor;

		[SerializeField]
		private Color wingDamageColor;

		[SerializeField]
		private Color playerDamageColor;

		[Range(0f, 10f)]
		[SerializeField]
		private float defaultSizeRate = 1.25f;

		[Range(0f, 10f)]
		[SerializeField]
		private float effectiveSizeRate = 1.5f;

		[Range(0f, 1f)]
		[SerializeField]
		private float nonEffectiveColorRate = 0.1f;

		[Range(0f, 10f)]
		[SerializeField]
		private float nonEffectiveSizeRate = 1f;

		[SerializeField]
		private float addDigitSizeRatio = 0.25f;

		[NonSerialized]
		private PKFxFX damageCounterFx;

		private Tweener<TweenTypesInOutSine> _wigglerTweener = new Tweener<TweenTypesInOutSine>();

		private Vector3 _wiggleRange = Vector3.zero;

		private PKFxManager.Attribute _damageValueAttr;

		private PKFxManager.Attribute _sizeRatioAttr;

		private PKFxManager.Attribute _isEffectiveAttr;

		private PKFxManager.Attribute _wiggleAttr;

		private PKFxManager.Attribute _colorAttr;

		private PKFxManager.Attribute _isEffectiveColorAttr;

		protected void Awake()
		{
			damageCounterFx = base.gameObject.GetComponent<PKFxFX>();
			_damageValueAttr = new PKFxManager.Attribute("Value", 0f);
			_sizeRatioAttr = new PKFxManager.Attribute("SizeRatio", 0f);
			_isEffectiveAttr = new PKFxManager.Attribute("IsEffective", 0f);
			_wiggleAttr = new PKFxManager.Attribute("WiggleVec", Vector3.zero);
			_colorAttr = new PKFxManager.Attribute("RGB", Vector3.zero);
			_isEffectiveColorAttr = new PKFxManager.Attribute("EffectiveRGB", Vector3.zero);
		}

		protected void LateUpdate()
		{
			if (base.ownerCharacter == null)
			{
				base.uiActive = false;
			}
			else
			{
				if (!_wigglerTweener.IsEnd())
				{
					_wiggleRange = _wigglerTweener.UpdateParameterVec3(Time.deltaTime);
				}
				base.transform.position = base.ownerCharacter.guiPointTransform.position;
			}
		}

		public override void Initialize()
		{
			if (!(base.ownerCharacter == null))
			{
				base.ownerCharacter.damageSystem.damagedEvent += HealthSystem_damageEvent;
				damageCounterFx = base.gameObject.GetComponent<PKFxFX>();
				damageCounterFx.BaseInitialize();
			}
		}

		private void OnDestroy()
		{
			if (!(base.ownerCharacter == null))
			{
				base.ownerCharacter.damageSystem.damagedEvent -= HealthSystem_damageEvent;
				damageCounterFx.KillEffect();
				damageCounterFx.StopEffect();
			}
		}

		private void HealthSystem_damageEvent(FieGameCharacter attacker, FieDamage damage)
		{
			if (!(base.ownerCharacter == null) && damage != null)
			{
				int num = Mathf.RoundToInt(Mathf.Clamp(damage.finallyDamage, 0f, 99999f));
				if (num > 0)
				{
					Color a = defaultDamageColor;
					float num2 = defaultSizeRate;
					bool flag = false;
					if (base.ownerCharacter.forces != 0)
					{
						switch (damage.attribute)
						{
						case FieAttribute.EARTH:
							a = earthDamageColor;
							break;
						case FieAttribute.MAGIC:
							a = magicDamageColor;
							break;
						case FieAttribute.WING:
							a = wingDamageColor;
							break;
						}
					}
					else
					{
						a = playerDamageColor;
					}
					if (damage.attributeDamageState == FieDamage.FieAttributeDamageState.EFFECTIVE)
					{
						num2 = effectiveSizeRate;
						flag = true;
					}
					else if (damage.attributeDamageState == FieDamage.FieAttributeDamageState.NONEFFECTIVE)
					{
						num2 = nonEffectiveSizeRate;
						a *= nonEffectiveColorRate;
					}
					float num3 = 1f;
					if (num >= 100)
					{
						num3 += addDigitSizeRatio;
					}
					if (num >= 1000)
					{
						num3 += addDigitSizeRatio;
					}
					num2 *= num3;
					_damageValueAttr.ValueFloat = (float)num;
					_sizeRatioAttr.ValueFloat = num2;
					_isEffectiveAttr.ValueFloat = ((!flag) ? 0f : 1f);
					_wiggleAttr.ValueFloat3 = _wiggleRange;
					_colorAttr.m_Value0 = a.r;
					_colorAttr.m_Value1 = a.g;
					_colorAttr.m_Value2 = a.b;
					_isEffectiveColorAttr.m_Value0 = a.r;
					_isEffectiveColorAttr.m_Value1 = a.g;
					_isEffectiveColorAttr.m_Value2 = a.b;
					damageCounterFx.SetAttribute(_damageValueAttr);
					damageCounterFx.SetAttribute(_sizeRatioAttr);
					damageCounterFx.SetAttribute(_isEffectiveAttr);
					damageCounterFx.SetAttribute(_wiggleAttr);
					damageCounterFx.SetAttribute(_colorAttr);
					damageCounterFx.SetAttribute(_isEffectiveColorAttr);
					damageCounterFx.StopEffect();
					damageCounterFx.StartEffect();
					_wigglerTweener.InitTweener(0.5f, new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(0f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f)), Vector3.zero);
				}
			}
		}
	}
}
