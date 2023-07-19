using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using PigeonCoopToolkit.Effects.Trails;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/ChangelingAlpha/Power/ChangelingAlphaShot")]
	public class FieEmitObjectChangelingAlphaShot : FieEmittableObjectBase
	{
		[SerializeField]
		private float shotDuration = 1f;

		[SerializeField]
		private float shotVelocityMax = 4f;

		[SerializeField]
		private float shotAccelTime = 0.1f;

		[SerializeField]
		private float shoutDestroyDuration = 0.5f;

		[SerializeField]
		private GameObject effectModel;

		[SerializeField]
		private Light shotLight;

		[SerializeField]
		private List<SmokeTrail> trailList = new List<SmokeTrail>();

		private Tweener<TweenTypesInSine> _velocityTweener = new Tweener<TweenTypesInSine>();

		private Tweener<TweenTypesOutSine> _scaleTweener = new Tweener<TweenTypesOutSine>();

		private Vector3 _lastDirectionalVec = Vector3.zero;

		private Vector3 _initDirectionalVec = Vector3.zero;

		private float _lifeTimeCount;

		private float _minDistance = 3.40282347E+38f;

		private bool _isEndUpdate;

		private Vector3 _initEffectModelScale = Vector3.zero;

		public void Awake()
		{
			_initEffectModelScale = effectModel.transform.localScale;
		}

		public override void awakeEmitObject()
		{
			_velocityTweener.InitTweener(shotAccelTime, 0f, shotVelocityMax);
			_scaleTweener.InitTweener(shotAccelTime, Vector3.zero, Vector3.one);
			_initDirectionalVec = directionalVec;
			if (targetTransform != null && targetTransform.root != null)
			{
				Vector3 a = targetTransform.position;
				FieGameCharacter component = targetTransform.root.GetComponent<FieGameCharacter>();
				if (component != null && component.groundState == FieObjectGroundState.Grounding)
				{
					int layerMask = 512;
					if (Physics.Raycast(targetTransform.position + Vector3.up * 2f, Vector3.down, out RaycastHit hitInfo, 1024f, layerMask) && hitInfo.collider.tag == "Floor")
					{
						a = hitInfo.point;
					}
				}
				_initDirectionalVec = a - base.transform.position;
				_initDirectionalVec.Normalize();
				directionalVec = _initDirectionalVec;
			}
			if (trailList.Count > 0)
			{
				foreach (SmokeTrail trail in trailList)
				{
					trail.ClearSystem(emitState: true);
				}
			}
			effectModel.transform.localScale = _initEffectModelScale;
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				Vector3 vector = directionalVec * _velocityTweener.UpdateParameterFloat(Time.deltaTime);
				base.transform.position += vector * Time.deltaTime;
				base.transform.localScale = _scaleTweener.UpdateParameterVec3(Time.deltaTime);
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= shotDuration)
				{
					if (trailList.Count > 0)
					{
						foreach (SmokeTrail trail in trailList)
						{
							trail.Emit = false;
							trail.ClearSystem(emitState: false);
						}
					}
					destoryEmitObject(shoutDestroyDuration);
					_isEndUpdate = true;
				}
				base.transform.rotation = Quaternion.LookRotation(vector);
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && (collider.gameObject.tag == getHostileTagString() || collider.gameObject.tag == "Floor"))
			{
				FieEmitObjectChangelingAlphaBurst fieEmitObjectChangelingAlphaBurst = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectChangelingAlphaBurst>(base.transform, Vector3.zero);
				if (collider.gameObject.tag == getHostileTagString())
				{
					addDamageToCollisionCharacter(collider, getDefaultDamageObject());
				}
				if (fieEmitObjectChangelingAlphaBurst != null)
				{
					fieEmitObjectChangelingAlphaBurst.setAllyTag(getArrayTag());
					fieEmitObjectChangelingAlphaBurst.setHostileTag(getHostileTag());
				}
				FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_BIG);
				if (trailList.Count > 0)
				{
					foreach (SmokeTrail trail in trailList)
					{
						trail.Emit = false;
					}
				}
				shotLight.intensity = 0f;
				effectModel.transform.localScale = Vector3.zero;
				destoryEmitObject(shotDuration - _lifeTimeCount);
				_isEndUpdate = true;
			}
		}

		private void OnDisable()
		{
			if (trailList.Count > 0)
			{
				foreach (SmokeTrail trail in trailList)
				{
					trail.ClearSystem(emitState: false);
				}
			}
		}
	}
}
