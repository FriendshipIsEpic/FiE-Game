using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using ParticlePlayground;
using System.Collections.Generic;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisMeteorConcentration")]
	public class FieEmitObjectQueenChrysalisMeteorShowerConcentration : FieEmittableObjectBase
	{
		private const float MAXIMUM_EMITTING_DURATION = 6f;

		private const float MAXIMUM_DURATION = 8f;

		[SerializeField]
		private AudioSource _loopAudio;

		[SerializeField]
		private float _audioFadeTime = 0.5f;

		[SerializeField]
		private List<PlaygroundParticlesC> _ppParticleList = new List<PlaygroundParticlesC>();

		[SerializeField]
		private List<PKFxFX> _pkfxParticleList = new List<PKFxFX>();

		private float _lifeTime;

		private float _initialAudioVolume = 1f;

		private bool _isEndUpdate;

		private Tweener<TweenTypesOutSine> _audioFadeTweener = new Tweener<TweenTypesOutSine>();

		private void Awake()
		{
			_initialAudioVolume = _loopAudio.volume;
		}

		public override void awakeEmitObject()
		{
			if (_ppParticleList.Count > 0)
			{
				foreach (PlaygroundParticlesC ppParticle in _ppParticleList)
				{
					ppParticle.emit = true;
				}
			}
			if (_pkfxParticleList.Count > 0)
			{
				foreach (PKFxFX pkfxParticle in _pkfxParticleList)
				{
					pkfxParticle.StopEffect();
					pkfxParticle.StartEffect();
				}
			}
			_loopAudio.Play();
			_audioFadeTweener.InitTweener(0.5f, _initialAudioVolume, _initialAudioVolume);
		}

		public void StopEffect()
		{
			StopAllEffect();
		}

		private void Update()
		{
			_loopAudio.volume = _audioFadeTweener.UpdateParameterFloat(Time.deltaTime);
			if (!_isEndUpdate)
			{
				_lifeTime += Time.deltaTime;
				if (_lifeTime > 6f)
				{
					StopAllEffect();
					_isEndUpdate = true;
				}
			}
		}

		private void StopAllEffect()
		{
			if (!_isEndUpdate)
			{
				_audioFadeTweener.InitTweener(_audioFadeTime, _initialAudioVolume, 0f);
				if (_ppParticleList.Count > 0)
				{
					foreach (PlaygroundParticlesC ppParticle in _ppParticleList)
					{
						ppParticle.emit = false;
					}
				}
				if (_pkfxParticleList.Count > 0)
				{
					foreach (PKFxFX pkfxParticle in _pkfxParticleList)
					{
						pkfxParticle.StopEffect();
					}
				}
				destoryEmitObject(2f);
				_isEndUpdate = true;
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate)
			{
				FieEmittableObjectBase component = collider.GetComponent<FieEmittableObjectBase>();
				if (component != null && reflectEmitObject(component))
				{
					Vector3 vector = collider.ClosestPointOnBounds(base.transform.position);
					Vector3 vector2 = vector - base.transform.position;
					FieEmitObjectQueenChrysalisReflectionEffect fieEmitObjectQueenChrysalisReflectionEffect = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisReflectionEffect>(base.transform, vector2.normalized);
					if (fieEmitObjectQueenChrysalisReflectionEffect != null)
					{
						fieEmitObjectQueenChrysalisReflectionEffect.transform.position = vector;
					}
				}
			}
		}
	}
}
