using Fie.Manager;
using Fie.Object;
using Fie.Utility;
using ParticlePlayground;
using UnityEngine;

namespace Fie.Enemies.HoovesRaces.QueenChrysalis
{
	[FiePrefabInfo("Prefabs/Enemies/ChangelingForces/QueenChrysalis/Power/QueenChrysalisMeteor")]
	public class FieEmitObjectQueenChrysalisMeteorShowerMeteor : FieEmittableObjectBase
	{
		[SerializeField]
		private float meteorDuration = 3f;

		[SerializeField]
		private float meteorVelocityMax = 2f;

		[SerializeField]
		private float meteorAccelTime = 0.1f;

		[SerializeField]
		private float meteorDestroyDuration = 1f;

		[SerializeField]
		private AudioSource _loopAudio;

		[SerializeField]
		private float _audioFadeTime = 0.2f;

		[SerializeField]
		private GameObject effectModel;

		[SerializeField]
		private PlaygroundParticlesC _flameParticle;

		private Tweener<TweenTypesInSine> _velocityTweener = new Tweener<TweenTypesInSine>();

		private Tweener<TweenTypesOutSine> _scaleTweener = new Tweener<TweenTypesOutSine>();

		private Tweener<TweenTypesOutSine> _audioFadeTweener = new Tweener<TweenTypesOutSine>();

		private Vector3 _lastDirectionalVec = Vector3.zero;

		private Vector3 _initDirectionalVec = Vector3.zero;

		private float _lifeTimeCount;

		private float _minDistance = 3.40282347E+38f;

		private bool _isEndUpdate;

		private Vector3 _initEffectModelScale = Vector3.zero;

		private float _initialAudioVolume = 1f;

		public void Awake()
		{
			if (effectModel != null)
			{
				_initEffectModelScale = effectModel.transform.localScale;
			}
			base.reflectEvent += delegate
			{
				FieEmitObjectQueenChrysalisMeteorShowerBurst fieEmitObjectQueenChrysalisMeteorShowerBurst = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisMeteorShowerBurst>(base.transform, Vector3.forward);
				FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_SMALL);
				effectModel.transform.localScale = Vector3.zero;
				destoryEmitObject(meteorDestroyDuration);
				if (_flameParticle != null)
				{
					_flameParticle.emit = false;
				}
				_audioFadeTweener.InitTweener(_audioFadeTime, _initialAudioVolume, 0f);
			};
			if (_loopAudio != null)
			{
				_initialAudioVolume = _loopAudio.volume;
			}
		}

		public override void awakeEmitObject()
		{
			_velocityTweener.InitTweener(meteorAccelTime, 0f, meteorVelocityMax);
			_scaleTweener.InitTweener(meteorAccelTime, Vector3.zero, Vector3.one);
			_initDirectionalVec = new Vector3(0f, -1f, 0f).normalized;
			effectModel.transform.localScale = _initEffectModelScale;
			effectModel.transform.rotation = Quaternion.Euler((float)Random.Range(0, 360), (float)Random.Range(0, 360), (float)Random.Range(0, 360));
			if (_flameParticle != null)
			{
				_flameParticle.emit = true;
			}
			_audioFadeTweener.InitTweener(0.5f, _initialAudioVolume, _initialAudioVolume);
		}

		public void Update()
		{
			_loopAudio.volume = _audioFadeTweener.UpdateParameterFloat(Time.deltaTime);
			if (!_isEndUpdate)
			{
				Vector3 vector = _initDirectionalVec * _velocityTweener.UpdateParameterFloat(Time.deltaTime);
				base.transform.position += vector * Time.deltaTime;
				base.transform.localScale = _scaleTweener.UpdateParameterVec3(Time.deltaTime);
				_lifeTimeCount += Time.deltaTime;
				if (_lifeTimeCount >= meteorDuration)
				{
					destoryEmitObject(meteorDestroyDuration);
					if (_flameParticle != null)
					{
						_flameParticle.emit = false;
					}
					if (effectModel != null)
					{
						effectModel.transform.localScale = Vector3.zero;
					}
					_isEndUpdate = true;
				}
				base.transform.rotation = Quaternion.LookRotation(vector);
			}
		}

		private void OnTriggerEnter(Collider collider)
		{
			if (!_isEndUpdate && (collider.gameObject.tag == "Floor" || collider.gameObject.tag == getHostileTagString()))
			{
				FieEmitObjectQueenChrysalisMeteorShowerBurst fieEmitObjectQueenChrysalisMeteorShowerBurst = FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisMeteorShowerBurst>(base.transform, Vector3.forward);
				if (fieEmitObjectQueenChrysalisMeteorShowerBurst != null)
				{
					fieEmitObjectQueenChrysalisMeteorShowerBurst.setAllyTag(getArrayTag());
					fieEmitObjectQueenChrysalisMeteorShowerBurst.setHostileTag(getHostileTag());
				}
				if (collider.gameObject.tag == getHostileTagString())
				{
					addDamageToCollisionCharacter(collider, getDefaultDamageObject());
					FieManagerBehaviour<FieEmittableObjectManager>.I.EmitObject<FieEmitObjectQueenChrysalisHitEffectSmall>(base.transform, Vector3.zero);
				}
				FieManagerBehaviour<FieGameCameraManager>.I.gameCamera.setWiggler(Wiggler.WiggleTemplate.WIGGLE_TYPE_SMALL);
				effectModel.transform.localScale = Vector3.zero;
				destoryEmitObject(meteorDestroyDuration);
				if (_flameParticle != null)
				{
					_flameParticle.emit = false;
				}
				_audioFadeTweener.InitTweener(_audioFadeTime, _initialAudioVolume, 0f);
				_isEndUpdate = true;
			}
		}

		private void OnDisable()
		{
			if (effectModel != null)
			{
				effectModel.transform.localScale = Vector3.zero;
			}
		}
	}
}
