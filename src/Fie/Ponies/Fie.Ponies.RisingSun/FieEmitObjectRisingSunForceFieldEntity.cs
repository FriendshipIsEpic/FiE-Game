using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/Power/RisingSunForceFieldEntity")]
	public class FieEmitObjectRisingSunForceFieldEntity : FieEmittableObjectBase
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
		private GameObject _sphereObject;

		[SerializeField]
		private GameObject _normalSphereObject;

		[SerializeField]
		private AnimationCurve damageCurvePerLifeRatio;

		[SerializeField]
		private float reflectScceedFriendshipGaindMagnify = 10f;

		private Tweener<TweenTypesInOutSine> _sphereScaleTweener = new Tweener<TweenTypesInOutSine>();

		private Tweener<TweenTypesOutSine> _sphereDeathTweener = new Tweener<TweenTypesOutSine>();

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		private float _angle;

		private Vector3 _initScale = Vector3.one;

		public void Awake()
		{
			_initScale = base.transform.localScale;
		}

		public override void awakeEmitObject()
		{
			base.transform.localScale = _initScale;
			_sphereObject.GetComponent<Renderer>().material.SetFloat("_Brightness", 1f);
			_sphereScaleTweener.InitTweener(ForceFieldScaleAnimationDuration, _initScale, Vector3.one);
		}

		public void Update()
		{
			if (_isEndUpdate)
			{
				float num = _sphereDeathTweener.UpdateParameterFloat(Time.deltaTime);
				_sphereObject.GetComponent<Renderer>().material.SetFloat("_Brightness", num);
				_normalSphereObject.GetComponent<Renderer>().material.SetColor(0, new Color(num, num, num, num));
			}
			else
			{
				base.transform.localScale = _sphereScaleTweener.UpdateParameterVec3(Time.deltaTime);
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= ForceFieldDuration - ForceFieldDestroyDuration)
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
					FieEmitObjectRisingSunForceFieldReflectEffect fieEmitObjectRisingSunForceFieldReflectEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRisingSunForceFieldReflectEffect>(base.transform, vector2.normalized);
					if (fieEmitObjectRisingSunForceFieldReflectEffect != null)
					{
						fieEmitObjectRisingSunForceFieldReflectEffect.transform.position = vector;
					}
					if (base.gainedFriendshipPoint > 0f && base.ownerCharacter != null)
					{
						base.ownerCharacter.friendshipStats.safeAddFriendship(base.gainedFriendshipPoint * reflectScceedFriendshipGaindMagnify);
					}
				}
			}
		}

		private void OnTriggerStay(Collider collider)
		{
			if (!_isEndUpdate && collider.gameObject.tag == getHostileTagString())
			{
				FieDamage defaultDamageObject = getDefaultDamageObject();
				float num = damageCurvePerLifeRatio.Evaluate(_lifeTimeCount / (ForceFieldDuration - ForceFieldDestroyDuration));
				defaultDamageObject.damage *= num;
				defaultDamageObject.stagger *= num;
				FieGameCharacter fieGameCharacter = addDamageToCollisionCharacter(collider, defaultDamageObject);
			}
		}
	}
}
