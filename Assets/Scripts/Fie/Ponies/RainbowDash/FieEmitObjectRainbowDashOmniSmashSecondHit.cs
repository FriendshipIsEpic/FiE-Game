using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using System.Collections;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashOmniSmashSecondHit")]
	public class FieEmitObjectRainbowDashOmniSmashSecondHit : FieEmittableObjectBase
	{
		[SerializeField]
		private PKFxFX trailFx;

		[SerializeField]
		private float DURATION = 1.5f;

		[SerializeField]
		private float DISTANCE = 2f;

		[SerializeField]
		private float TRAIL_EMIT_DURATION = 0.28f;

		[SerializeField]
		private float DAMAGE_DURATION = 0.2f;

		private float _lifeTimeCount;

		private bool _isEndTrail;

		private bool _isEndUpdate;

		private Vector3 _targetPosition = Vector3.zero;

		private Vector3 _hormingDirectionalVec = Vector3.zero;

		private Tweener<TweenTypesOutSine> _positionTweener = new Tweener<TweenTypesOutSine>();

		private IEnumerator StopEffectCoroutine()
		{
			yield return (object)new WaitForSeconds(TRAIL_EMIT_DURATION);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public override void awakeEmitObject()
		{
			if (initTransform != null)
			{
				base.transform.position = initTransform.position;
			}
			if (trailFx != null)
			{
				trailFx.StopEffect();
				trailFx.StartEffect();
			}
			destoryEmitObject(DURATION);
			StartCoroutine(StopEffectCoroutine());
		}

		public void SetInitializePosition(Vector3 worldPosition)
		{
			base.transform.position = worldPosition;
			_hormingDirectionalVec = initTransform.position - base.transform.position;
			_targetPosition = initTransform.position + _hormingDirectionalVec.normalized * DISTANCE;
			_positionTweener.InitTweener(DAMAGE_DURATION, base.transform.position, _targetPosition);
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				UpdateTransform();
			}
		}

		private void UpdateTransform()
		{
			if (!_positionTweener.IsEnd())
			{
				base.transform.position = _positionTweener.UpdateParameterVec3(Time.deltaTime);
				base.transform.rotation = Quaternion.LookRotation(_hormingDirectionalVec);
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > DAMAGE_DURATION) && collider.gameObject.tag == getHostileTagString())
			{
				FieGameCharacter x = addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				if (x != null)
				{
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRainbowDashOmniSmashHitEffect>(base.transform, collider.ClosestPointOnBounds(base.transform.position));
					FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_SMALL);
				}
			}
		}
	}
}
