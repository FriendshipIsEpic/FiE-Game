using Fie.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RainbowDash
{
	[FiePrefabInfo("Prefabs/RainbowDash/Power/RainbowDashRainblowEmitEffect")]
	public class FieEmitObjectRainbowDashRainblowSeed : FieEmittableObjectBase
	{
		[SerializeField]
		private float seedDuration = 4f;

		[SerializeField]
		private float seedDestroyDuration = 0.5f;

		[SerializeField]
		private float entytyInstantiateDelay;

		[SerializeField]
		private float seedVelocity = 3f;

		[SerializeField]
		private List<PKFxFX> _childParticles = new List<PKFxFX>();

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		private IEnumerator EmitRainblowCoroutine()
		{
			yield return (object)new WaitForSeconds(entytyInstantiateDelay);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		private IEnumerator StopEffectCoroutine()
		{
			_childParticles.ForEach(delegate(PKFxFX particle)
			{
				particle.SetAttribute(new PKFxManager.Attribute("PositionMagni", 5f));
			});
			yield return (object)new WaitForSeconds(0.1f);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public override void awakeEmitObject()
		{
			base.transform.position = initTransform.position;
			_childParticles.ForEach(delegate(PKFxFX particle)
			{
				particle.StopEffect();
				particle.SetAttribute(new PKFxManager.Attribute("PositionMagni", 0.5f));
				particle.StartEffect();
			});
		}

		public void Update()
		{
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				Vector3 vector = directionalVec * seedVelocity * Time.deltaTime;
				base.transform.position += vector;
				base.transform.rotation.SetLookRotation(vector);
				if (_lifeTimeCount >= seedDuration - seedDestroyDuration)
				{
					stopParticleEmitting();
					destoryEmitObject(seedDestroyDuration);
					_isEndUpdate = true;
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > seedDuration - seedDestroyDuration) && other.gameObject.tag == "Floor")
			{
				emitRainBlowEntity();
				stopParticleEmitting();
				destoryEmitObject(seedDestroyDuration);
				_isEndUpdate = true;
			}
		}

		private void stopParticleEmitting()
		{
			StartCoroutine(StopEffectCoroutine());
		}

		private void emitRainBlowEntity()
		{
			StartCoroutine(EmitRainblowCoroutine());
		}
	}
}
