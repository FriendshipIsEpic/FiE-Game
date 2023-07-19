using Fie.Manager;
using Fie.Object;
using ParticlePlayground;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightLaser")]
	public class FieEmitObjectTwilightLaser : FieEmittableObjectBase
	{
		[SerializeField]
		private float LaserDuration = 4f;

		[SerializeField]
		private float LaserEmitDuration = 2f;

		[SerializeField]
		private int LaserEmitNum = 17;

		[SerializeField]
		private float LaserEmitInterval = 0.1f;

		[SerializeField]
		private AnimationCurve OutputCurve;

		[SerializeField]
		private List<PlaygroundParticlesC> _childParticles = new List<PlaygroundParticlesC>();

		[SerializeField]
		private GameObject _collisionObject;

		private float _lifeTimeCount;

		private float _emitTimeCount;

		private int _emitCount;

		private bool _isEndUpdate;

		private bool _isEndEmit;

		public void Awake()
		{
			_emitTimeCount = LaserEmitInterval;
		}

		public override void awakeEmitObject()
		{
			base.transform.rotation = Quaternion.LookRotation(directionalVec);
			foreach (PlaygroundParticlesC childParticle in _childParticles)
			{
				childParticle.Emit(setEmission: true);
			}
			if (targetTransform != null)
			{
				Vector3 forward = targetTransform.position - base.transform.position;
				float t = Mathf.Clamp(Vector3.Dot(directionalVec.normalized, forward.normalized), 0f, 1f);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(forward), t);
				directionalVec = base.transform.rotation * Vector3.forward;
			}
		}

		public void Update()
		{
			base.transform.position = initTransform.position;
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				_emitTimeCount += Time.deltaTime;
				if (_emitTimeCount >= LaserEmitInterval && _lifeTimeCount <= LaserEmitDuration)
				{
					emitChild();
					_emitTimeCount = 0f;
				}
				if (_lifeTimeCount > LaserEmitDuration && !_isEndEmit)
				{
					foreach (PlaygroundParticlesC childParticle in _childParticles)
					{
						childParticle.Emit(setEmission: false);
					}
					_isEndEmit = true;
				}
				if (_lifeTimeCount >= LaserDuration)
				{
					destoryEmitObject();
				}
				if (_collisionObject != null)
				{
					float currentOutput = getCurrentOutput();
					_collisionObject.transform.localScale = new Vector3(Mathf.Max(1f, 1f * currentOutput), Mathf.Max(1f, 1f * currentOutput), 1f);
				}
			}
		}

		public float getLaserDuration()
		{
			return LaserEmitDuration;
		}

		private void emitChild()
		{
			FieEmitObjectTwilightLaserChild fieEmitObjectTwilightLaserChild = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightLaserChild>(base.transform, directionalVec, targetTransform, base.ownerCharacter);
			if (fieEmitObjectTwilightLaserChild != null)
			{
				fieEmitObjectTwilightLaserChild.SetOutputRate(getCurrentOutput());
			}
			_emitCount++;
		}

		public float getCurrentOutput()
		{
			return OutputCurve.Evaluate(Mathf.Clamp(_lifeTimeCount / Mathf.Max(LaserEmitDuration, 0.01f), 0f, 1f));
		}

		private void OnTriggerStay(Collider collider)
		{
			if (!(_lifeTimeCount >= LaserEmitDuration) && collider.gameObject.tag == getHostileTagString())
			{
				float currentOutput = getCurrentOutput();
				FieDamage defaultDamageObject = getDefaultDamageObject();
				defaultDamageObject.damage *= currentOutput;
				defaultDamageObject.stagger *= currentOutput;
				addDamageToCollisionCharacter(collider, defaultDamageObject);
			}
		}

		internal void Stop()
		{
			_lifeTimeCount = LaserEmitDuration;
		}
	}
}
