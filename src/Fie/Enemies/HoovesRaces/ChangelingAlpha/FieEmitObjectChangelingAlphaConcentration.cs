using Fie.Object;
using ParticlePlayground;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.ChangelingAlpha
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/ChangelingAlpha/Power/ChangelingAlphaConcentration")]
	public class FieEmitObjectChangelingAlphaConcentration : FieEmittableObjectBase
	{
		private const float EFFECT_DURATION = 2f;

		private const float DURATION = 3f;

		public List<PlaygroundParticlesC> effects;

		public AudioSource soundEffect;

		private float lifeTime;

		private bool isStopEffects;

		public override void awakeEmitObject()
		{
			foreach (PlaygroundParticlesC effect in effects)
			{
				if (effect != null)
				{
					effect.emit = true;
				}
			}
		}

		private void Update()
		{
			if (base.ownerCharacter == null)
			{
				lifeTime = 2f;
			}
			lifeTime += Time.deltaTime;
			if (lifeTime > 2f && !isStopEffects && effects != null)
			{
				foreach (PlaygroundParticlesC effect in effects)
				{
					if (effect != null)
					{
						effect.emit = false;
					}
				}
				if (soundEffect != null)
				{
					soundEffect.Stop();
				}
				isStopEffects = true;
			}
			if (lifeTime > 3f)
			{
				destoryEmitObject();
			}
		}

		private void LateUpdate()
		{
			if (!(initTransform == null))
			{
				base.transform.position = initTransform.position;
			}
		}
	}
}
