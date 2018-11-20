using Fie.Object;
using ParticlePlayground;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.Flightling
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/Flightling/Power/FlightlingConcentration")]
	public class FieEmitObjectFlightlingConcentration : FieEmittableObjectBase
	{
		private const float EFFECT_DURATION = 1.5f;

		private const float DURATION = 2.5f;

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
				lifeTime = 1.5f;
			}
			lifeTime += Time.deltaTime;
			if (lifeTime > 1.5f && !isStopEffects && effects != null)
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
			if (lifeTime > 2.5f)
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
