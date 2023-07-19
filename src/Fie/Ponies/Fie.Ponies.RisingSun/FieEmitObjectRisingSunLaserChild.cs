using Fie.Object;
using Fie.Utility;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/Power/RisingSunLaserChild")]
	public class FieEmitObjectRisingSunLaserChild : FieEmittableObjectBase
	{
		[SerializeField]
		private float LaserChildDuration = 0.3f;

		[SerializeField]
		private float LaserChildMaxScale = 1.2f;

		private Tweener<TweenTypesOutSine> _startScaleTweener = new Tweener<TweenTypesOutSine>();

		private Tweener<TweenTypesOutSine> _endScaleTweener = new Tweener<TweenTypesOutSine>();

		private float _lifeTimeCount;

		public void Awake()
		{
			_startScaleTweener.InitTweener(LaserChildDuration * 0.5f, 0f, LaserChildMaxScale);
			_endScaleTweener.InitTweener(LaserChildDuration * 0.5f, LaserChildMaxScale, 0f);
		}

		public void Update()
		{
			_lifeTimeCount += Time.deltaTime;
			if (_lifeTimeCount > LaserChildDuration)
			{
				destoryEmitObject();
			}
			Vector3 a = base.transform.position + directionalVec;
			Vector3 zero = Vector3.zero;
			zero = ((!(targetTransform != null)) ? (Quaternion.LookRotation(a - base.transform.position) * Vector3.forward) : (Quaternion.LookRotation(targetTransform.position - base.transform.position) * Vector3.forward));
			base.transform.rotation = Quaternion.LookRotation(zero);
			float num = 0f;
			num = (_startScaleTweener.IsEnd() ? _endScaleTweener.UpdateParameterFloat(Time.deltaTime) : _startScaleTweener.UpdateParameterFloat(Time.deltaTime));
			base.transform.localScale = new Vector3(num, num, 1f);
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (collider.gameObject.tag == getHostileTagString())
			{
				addDamageToCollisionCharacter(collider, getDefaultDamageObject());
			}
		}
	}
}
