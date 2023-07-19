using Fie.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Fie.Manager
{
	[FieManagerExists(FieManagerExistSceneFlag.NEVER_DESTROY)]
	public sealed class FieAudioManager : FieManagerBehaviour<FieAudioManager>
	{
		public enum FieAudioMixerType
		{
			BGM,
			SE,
			Voice
		}

		public const float AUDIO_VOLUME_MIN = -50f;

		public const float AUDIO_VOLUME_MAX = 0f;

		public const float AUDIO_VOLUME_LITTLE_BIT_SMALL = -1f;

		private const string MIXER_VOLUME_PROPERTY_NAME = "MasterVolume";

		private const string BGM_MIXER_PATH = "AudioMixer/BGM";

		private const string SE_MIXER_PATH = "AudioMixer/SE";

		private const string VOICE_MIXER_PATH = "AudioMixer/Voice";

		private Dictionary<FieAudioMixerType, float> _currentVolume = new Dictionary<FieAudioMixerType, float>();

		private Dictionary<FieAudioMixerType, AudioMixer> _audioMixers = new Dictionary<FieAudioMixerType, AudioMixer>();

		public Dictionary<FieAudioMixerType, AudioMixer> audioMixers => _audioMixers;

		private IEnumerator ChangeVolumeTask(float volume, float time, FieAudioMixerType type)
		{
			if (_currentVolume.ContainsKey(type) && _audioMixers.ContainsKey(type))
			{
				Tweener<TweenTypesInOutSine> volumeTweener = new Tweener<TweenTypesInOutSine>();
				volumeTweener.InitTweener(time, _currentVolume[type], volume);
				if (!volumeTweener.IsEnd())
				{
					_currentVolume[type] = volumeTweener.UpdateParameterFloat(Time.deltaTime);
					_audioMixers[type].SetFloat("MasterVolume", _currentVolume[type]);
					yield return (object)null;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
		}

		protected override void StartUpEntity()
		{
			_audioMixers[FieAudioMixerType.BGM] = Resources.Load<AudioMixer>("AudioMixer/BGM");
			_audioMixers[FieAudioMixerType.SE] = Resources.Load<AudioMixer>("AudioMixer/SE");
			_audioMixers[FieAudioMixerType.Voice] = Resources.Load<AudioMixer>("AudioMixer/Voice");
			_currentVolume[FieAudioMixerType.BGM] = 1f;
			_currentVolume[FieAudioMixerType.SE] = 1f;
			_currentVolume[FieAudioMixerType.Voice] = 1f;
			foreach (KeyValuePair<FieAudioMixerType, AudioMixer> audioMixer in _audioMixers)
			{
				if (audioMixer.Value == null)
				{
					Debug.LogError("FieAudioManager : Faild to get an AudioMixer object. " + audioMixer.Key.ToString());
				}
				else
				{
					audioMixer.Value.SetFloat("MasterVolume", 1f);
				}
			}
		}

		public void ChangeMixerVolume(float volume, float time, params FieAudioMixerType[] types)
		{
			volume = Mathf.Clamp(volume, -50f, volume);
			if (types.Length > 0)
			{
				foreach (FieAudioMixerType type in types)
				{
					StartCoroutine(ChangeVolumeTask(volume, time, type));
				}
			}
		}
	}
}
