using Fie.Object;
using ParticlePlayground;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisHornEffect")]
	public class FieEmitObjectQueenChrysalisHornEffect : FieEmittableObjectBase
	{
		[SerializeField]
		private float HORN_EFFECT_MAXIMUM_DURATION = 10f;

		[SerializeField]
		private float HORN_EFFECT_DESTROY_DURATION = 1f;

		[SerializeField]
		private PlaygroundParticlesC _hornParticle;

		private float _lifeCount;

		private bool _isEnd;

		public void Kill()
		{
			if (!_isEnd)
			{
				if (_hornParticle != null)
				{
					_hornParticle.emit = false;
				}
				destoryEmitObject(HORN_EFFECT_DESTROY_DURATION);
				_isEnd = true;
			}
		}

		public override void awakeEmitObject()
		{
			_hornParticle.emit = true;
		}

		private void Update()
		{
			if (!_isEnd)
			{
				_lifeCount += Time.deltaTime;
				if (_lifeCount > HORN_EFFECT_MAXIMUM_DURATION)
				{
					Kill();
				}
			}
		}

		private void LateUpdate()
		{
			if (!(initTransform == null))
			{
				base.transform.position = initTransform.position;
				base.transform.rotation = initTransform.rotation;
			}
		}
	}
}
