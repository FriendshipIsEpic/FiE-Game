using Fie.Manager;
using Fie.Object;
using ParticlePlayground;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.Twilight
{
	[FiePrefabInfo("Prefabs/Twilight/Power/TwilightForceField")]
	public class FieEmitObjectTwilightForceField : FieEmittableObjectBase
	{
		[SerializeField]
		private float ForceFieldSeedDuration = 4f;

		[SerializeField]
		private float ForceFieldSeedDestroyDuration = 1f;

		[SerializeField]
		private float entytyInstantiateDelay = 0.3f;

		[SerializeField]
		private List<PlaygroundParticlesC> _childParticles = new List<PlaygroundParticlesC>();

		private float _lifeTimeCount;

		private bool _isEndUpdate;

		private IEnumerator EmitSphereCoroutine()
		{
			yield return (object)new WaitForSeconds(entytyInstantiateDelay);
			/*Error: Unable to find new state assignment for yield return*/;
		}

		public override void awakeEmitObject()
		{
			_childParticles.ForEach(delegate(PlaygroundParticlesC particle)
			{
				particle.Emit(setEmission: true);
			});
		}

		public void Update()
		{
			base.transform.localRotation = Quaternion.identity;
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= ForceFieldSeedDuration - ForceFieldSeedDestroyDuration)
				{
					stopParticleEmitting();
					destoryEmitObject(ForceFieldSeedDestroyDuration);
					_isEndUpdate = true;
				}
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!_isEndUpdate && !(_lifeTimeCount > ForceFieldSeedDuration - ForceFieldSeedDestroyDuration) && collision.gameObject.tag == "Floor")
			{
				emitForceFieldEntity();
				stopParticleEmitting();
				destoryEmitObject(ForceFieldSeedDestroyDuration);
				_isEndUpdate = true;
			}
		}

		private void stopParticleEmitting()
		{
			_childParticles.ForEach(delegate(PlaygroundParticlesC particle)
			{
				particle.Emit(setEmission: false);
			});
		}

		private void emitForceFieldEntity()
		{
			Transform targetTransform = null;
			FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectTwilightForceFieldEmitEffect>(base.transform, Vector3.zero, targetTransform, base.ownerCharacter);
			StartCoroutine(EmitSphereCoroutine());
		}
	}
}
