using GameDataEditor;
using RogoDigital.Lipsync;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fie.Voice
{
	public class FieVoiceController : MonoBehaviour
	{
		public delegate void FieVoiceControllerEventCallback(FieVoiceController controller);

		[SerializeField]
		private FieGameCharacter _actorCharacter;

		[SerializeField]
		private FieVoiceSpeaker _speaker;

		[SerializeField]
		public List<FiePhonemeAnimationData> phonemeAnimation;

		[SerializeField]
		public List<FieEmotionAnimationData> emotionAnimation;

		private Dictionary<Phoneme, string> phonemesAnimations;

		private Dictionary<string, string> emotionsAnimations;

		public int phonemeTrackID;

		public int emotionTrackID = 1;

		public float restTime = 0.025f;

		private Dictionary<string, LipSyncData> _lipSyncData = new Dictionary<string, LipSyncData>();

		private List<PhonemeMarker> _phonemeMarkers;

		private List<EmotionMarker> _emotionMarkers;

		private AudioClip _currentAudioClip;

		private int _currentPhonemeMarker = -1;

		private int _currentEmotionMarker = -1;

		private int _currentFileID = -1;

		private float _masterTimer;

		private bool _isEndPreload;

		[CompilerGenerated]
		private static Comparison<PhonemeMarker> _003C_003Ef__mg_0024cache0;

		public bool isPlaying
		{
			get;
			private set;
		}

		public bool isPaused
		{
			get;
			private set;
		}

		public FieGameCharacter actorCharacter => _actorCharacter;

		public bool isEndPreload => _isEndPreload;

		public event FieVoiceControllerEventCallback startEvent;

		public event FieVoiceControllerEventCallback endEvent;

		private IEnumerator AsyncLoadVoiceDatas(GDEGameCharacterTypeData gameCharacterTypeData)
		{
			_003CAsyncLoadVoiceDatas_003Ec__Iterator0 _003CAsyncLoadVoiceDatas_003Ec__Iterator = (_003CAsyncLoadVoiceDatas_003Ec__Iterator0)/*Error near IL_0034: stateMachine*/;
			if (gameCharacterTypeData != null)
			{
				List<GDEWordScriptsListData> voiceDataList = FieMasterData<GDEWordScriptsListData>.FindMasterDataList(delegate(GDEWordScriptsListData data)
				{
					if (data.Actor.Key != gameCharacterTypeData.Key)
					{
						return false;
					}
					return true;
				});
				if (voiceDataList.Count > 0)
				{
					foreach (GDEWordScriptsListData item in voiceDataList)
					{
						ResourceRequest loadRequest = Resources.LoadAsync<LipSyncData>(item.VoiceAssetPath);
						float time = 0f;
						if (time < 3f)
						{
							if (!loadRequest.isDone)
							{
								float num = time + Time.deltaTime;
								yield return (object)null;
								/*Error: Unable to find new state assignment for yield return*/;
							}
							LipSyncData lipSyncData = loadRequest.asset as LipSyncData;
							if (!(lipSyncData == null))
							{
								_lipSyncData[item.Key] = lipSyncData;
							}
						}
					}
					_isEndPreload = true;
				}
			}
			yield break;
			IL_01c0:
			/*Error near IL_01c1: Unexpected return in MoveNext()*/;
		}

		private void Awake()
		{
			if (_actorCharacter == null)
			{
				Debug.LogError("FieVoiceController : An actor character not binded.");
			}
			else if (_speaker == null)
			{
				Debug.LogError("FieVoiceController : A speaker not binded.");
			}
			else
			{
				StartCoroutine(AsyncLoadVoiceDatas(_actorCharacter.getCharacterTypeData()));
				phonemesAnimations = new Dictionary<Phoneme, string>();
				emotionsAnimations = new Dictionary<string, string>();
				if (phonemeAnimation != null && phonemeAnimation.Count > 0)
				{
					foreach (FiePhonemeAnimationData item in phonemeAnimation)
					{
						phonemesAnimations[item.phoneme] = item.animationName;
					}
				}
				if (emotionAnimation != null && emotionAnimation.Count > 0)
				{
					foreach (FieEmotionAnimationData item2 in emotionAnimation)
					{
						emotionsAnimations[item2.emotion] = item2.animationName;
					}
				}
			}
		}

		private void Update()
		{
			if (isPlaying && !isPaused)
			{
				if (_phonemeMarkers == null)
				{
					Stop(stopAudio: false);
				}
				else
				{
					_masterTimer += Time.deltaTime;
					float time = _masterTimer / _currentAudioClip.length;
					UpdateAnimation(time);
				}
			}
		}

		private void OnDestroy()
		{
			Interrupt();
		}

		public void Play(string voiceDataName, bool isForceSet = false)
		{
			if (_lipSyncData.ContainsKey(voiceDataName))
			{
				PlayWithLipSyncData(_lipSyncData[voiceDataName], isForceSet);
			}
		}

		public void PlayWithLipSyncData(LipSyncData dataFile, bool isForceSet = false)
		{
			if (!isPlaying || isForceSet)
			{
				if (isPlaying && isForceSet)
				{
					Stop(stopAudio: true);
				}
				LoadData(dataFile);
				if (_actorCharacter != null && _actorCharacter.emotionController != null && dataFile.emotionData != null && dataFile.emotionData.Length > 0)
				{
					_actorCharacter.emotionController.StopAutoAnimation();
				}
				isPlaying = true;
				isPaused = false;
				_masterTimer = 0f;
				SetCurrentPhoneme(0);
				SetCurrentEmotion(0);
				_speaker.audioSource.Play();
				if (this.startEvent != null)
				{
					this.startEvent(this);
					this.startEvent = null;
				}
			}
		}

		public void Pause()
		{
			if (isPlaying && !isPaused)
			{
				isPaused = true;
				_speaker.audioSource.Pause();
			}
		}

		public void Resume()
		{
			if (isPlaying && isPaused)
			{
				isPaused = false;
				_speaker.audioSource.UnPause();
			}
		}

		public void Stop(bool stopAudio)
		{
			if (isPlaying)
			{
				isPlaying = false;
				isPaused = false;
				if (stopAudio)
				{
					_speaker.audioSource.Stop();
				}
			}
			if (_actorCharacter != null && _actorCharacter.emotionController != null)
			{
				_actorCharacter.skeletonUtility.skeletonAnimation.state.ClearTrack(emotionTrackID);
				_actorCharacter.emotionController.RestoreEmotionFromDefaultData();
				_actorCharacter.emotionController.RestartAutoAnimation();
			}
			if (this.endEvent != null)
			{
				this.endEvent(this);
				this.endEvent = null;
			}
		}

		public void Interrupt()
		{
			Stop(stopAudio: true);
		}

		private void UpdateAnimation(float time)
		{
			if (_currentPhonemeMarker + 1 < _phonemeMarkers.Count)
			{
				if (time >= _phonemeMarkers[_currentPhonemeMarker + 1].time)
				{
					SetCurrentPhoneme(_currentPhonemeMarker + 1);
				}
			}
			else if (time >= _phonemeMarkers[_currentPhonemeMarker].time + restTime / _currentAudioClip.length)
			{
				Stop(stopAudio: false);
			}
			if (_currentEmotionMarker + 1 < _emotionMarkers.Count && time >= _emotionMarkers[_currentEmotionMarker + 1].startTime)
			{
				SetCurrentEmotion(_currentEmotionMarker + 1);
			}
		}

		private void UpdateEmotion(float time)
		{
		}

		private void SetCurrentPhoneme(int index)
		{
			if (_phonemeMarkers != null && _phonemeMarkers.Count > 0 && index < _phonemeMarkers.Count)
			{
				_currentPhonemeMarker = index;
				Phoneme phoneme = _phonemeMarkers[index].phoneme;
				if (phonemesAnimations.ContainsKey(phoneme))
				{
					TrackEntry trackEntry = actorCharacter.skeletonUtility.skeletonAnimation.state.SetAnimation(phonemeTrackID, phonemesAnimations[phoneme], loop: false);
					if (trackEntry != null)
					{
						float mixDuration = 0f;
						if (_currentPhonemeMarker + 1 < _phonemeMarkers.Count)
						{
							mixDuration = _phonemeMarkers[_currentPhonemeMarker + 1].time - _phonemeMarkers[_currentPhonemeMarker].time;
						}
						trackEntry.mixDuration = mixDuration;
					}
				}
			}
		}

		private void SetCurrentEmotion(int index)
		{
			if (_emotionMarkers != null && _emotionMarkers.Count > 0 && index < _emotionMarkers.Count)
			{
				_currentEmotionMarker = index;
				string emotion = _emotionMarkers[index].emotion;
				if (emotionsAnimations.ContainsKey(emotion))
				{
					TrackEntry trackEntry = actorCharacter.skeletonUtility.skeletonAnimation.state.SetAnimation(emotionTrackID, emotionsAnimations[emotion], loop: false);
					if (trackEntry != null)
					{
						trackEntry.mixDuration = 0.2f;
					}
				}
			}
		}

		private bool LoadData(LipSyncData dataFile)
		{
			if (dataFile.phonemeData.Length > 0 || dataFile.emotionData.Length > 0)
			{
				_currentAudioClip = dataFile.clip;
				_phonemeMarkers = new List<PhonemeMarker>();
				_emotionMarkers = new List<EmotionMarker>();
				PhonemeMarker[] phonemeData = dataFile.phonemeData;
				foreach (PhonemeMarker item in phonemeData)
				{
					_phonemeMarkers.Add(item);
				}
				EmotionMarker[] emotionData = dataFile.emotionData;
				foreach (EmotionMarker item2 in emotionData)
				{
					_emotionMarkers.Add(item2);
				}
				_phonemeMarkers.Sort(SortTime);
				_speaker.audioSource.clip = _currentAudioClip;
				_currentFileID = dataFile.GetInstanceID();
				return true;
			}
			Debug.Log("A LipSync Data dosen't exist : " + dataFile.ToString());
			return false;
		}

		private static int SortTime(PhonemeMarker a, PhonemeMarker b)
		{
			float time = a.time;
			float time2 = b.time;
			return time.CompareTo(time2);
		}
	}
}
