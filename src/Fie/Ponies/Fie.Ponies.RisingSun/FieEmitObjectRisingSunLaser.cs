using Fie.Manager;
using Fie.Object;
using ParticlePlayground;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Ponies.RisingSun
{
	[FiePrefabInfo("Prefabs/RisingSun/Power/RisingSunLaser")]
	public class FieEmitObjectRisingSunLaser : FieEmittableObjectBase
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
		private List<PlaygroundParticlesC> _childParticles = new List<PlaygroundParticlesC>();

		private float _lifeTimeCount;

		private float _emitTimeCount;

		private int _emitCount;

		private bool _isEndUpdate;

		private bool _isEndEmit;

		public void Awake()
		{
			_emitTimeCount = LaserEmitInterval;
		}

		public void Update()
		{
			base.transform.position = initTransform.position;
			if (!_isEndUpdate)
			{
				_lifeTimeCount += Time.deltaTime;
				_emitTimeCount += Time.deltaTime;
				if (_emitTimeCount >= LaserEmitInterval && _emitCount < LaserEmitNum)
				{
					emitChild();
					_emitTimeCount = 0f;
				}
				if (_lifeTimeCount > LaserEmitDuration && !_isEndEmit)
				{
					_childParticles.ForEach(delegate(PlaygroundParticlesC particle)
					{
						particle.Emit(setEmission: false);
					});
					_isEndEmit = true;
				}
				if (_lifeTimeCount >= LaserDuration)
				{
					destoryEmitObject();
				}
			}
		}

		private void emitChild()
		{
			Transform targetTransform = base.targetTransform;
			Vector3 directionalVec = base.directionalVec;
			FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectRisingSunLaserChild>(initTransform, directionalVec, targetTransform, base.ownerCharacter);
			_emitCount++;
		}
	}
}
