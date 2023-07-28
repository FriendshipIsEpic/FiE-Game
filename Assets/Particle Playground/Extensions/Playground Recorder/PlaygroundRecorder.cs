using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ParticlePlayground {

	/// <summary>
	/// The Particle Playground Recorder enables playback and scrubbing of recorded particles. 
	/// The method used is storing built-in particle arrays as keyframes, then interpolate between current keyframe and next upon playback.
	/// 
	/// Usage:
	/// Drag the PlaygroundRecorder.cs onto a particle system you wish to record.
	/// Use StartRecording(float keyframeInterval) to start record the particle system then StopRecording() to stop.
	/// Use RecordOneFrame() to add a recorded frame, use InsertOneFrame(int frame) to insert a recorded frame.
	/// To replay a recording use Play(float fromNormalizedTime, float speed, bool repeat) then Stop() to stop. This is essentially the same as using Scrub() with an increasing time step.
	/// Use Pause() to pause during replay.
	/// Use Scrub() to scrub to a normalized time (where all recorded frames are measured between 0f to 1f).
	/// </summary>
	[ExecuteInEditMode()]
	public class PlaygroundRecorder : MonoBehaviour {

		/// <summary>
		/// Determines if the particle playback and serialization should run asynchronously on another thread.
		/// </summary>
		public bool multithreading = true;
		/// <summary>
		/// The interval between keyframes while recording.
		/// </summary>
		public float keyframeInterval = .1f;
		/// <summary>
		/// The speed of the playback. Set this to a negative value to make the playback go backwards.
		/// </summary>
		public float playbackSpeed = 1f;
		/// <summary>
		/// The current position of the playhead (scrubber).
		/// </summary>
		public float playHead = 0;
		/// <summary>
		/// Determines if the playback should loop when reaching the end of the recording.
		/// </summary>
		public bool loopPlayback = true;
		/// <summary>
		/// Determines if playback particles should fade in at appearance. This will interpolate from birth color with 0 alpha to the upcoming keyframe's color.
		/// </summary>
		public bool fadeIn = true;
		/// <summary>
		/// Determines if playback particles should grow from 0 size at appearance. This will interpolate from 0 size to the upcoming keyframe's size.
		/// </summary>
		public bool sizeIn = true;
		/// <summary>
		/// Determines if interpolation should be skipped on a recorded end-frame. This will make the playback let particles instantly jump/cut to the next recorded frame.
		/// </summary>
		public bool skipInterpolationOnEndFrames = true;
		/// <summary>
		/// Determines if the particle system should switch simulation space to local upon playback. This enabled you to move the recording around freely in the scene based on its transform.
		/// </summary>
		public bool localSpaceOnPlayback = true;


		/// <summary>
		/// The playground system to record and playback into.
		/// </summary>
		[HideInInspector] public PlaygroundParticlesC playgroundSystem;
		/// <summary>
		/// The recorded data for serialization (data storage).
		/// </summary>
		[HideInInspector] public PlaygroundRecorderData recorderData;
		/// <summary>
		/// The list of recorded frames. Each frame has its own list of particle data, where playback will interpolate between these based on the playhead.
		/// </summary>
		[NonSerialized] public List<RecordedFrame> recordedFrames;

		private bool _inPlayback = false;
		private bool _isReplaying = false;
		private bool _isRecording = false;
		private bool _hasRecorderData = false;
		private bool _hasPlaygroundSystem = false;
		private bool _hasEditedRecordData = false;
		private float _recordingStarted;
		private float _lastRecordedFrameTime;
		private int _recordingStartFrame;
		private int _recordingEndFrame;
		private ParticleSystem.Particle[] _playbackParticles;
		private ParticleSystemSimulationSpace _previousSimulationSpace;

		private object locker = new object();


		/****************************************************************************
			Monobehaviours
		 ****************************************************************************/

		void OnEnable () 
		{
			if (playgroundSystem == null)
				playgroundSystem = GetComponent<PlaygroundParticlesC>();
			if (Application.isPlaying && recordedFrames == null)
			{
				if (recorderData != null)
				{
					_hasRecorderData = true;
					if (multithreading)
						LoadAsync();
					else
						Load();
				}
				else
				{
					recordedFrames = new List<RecordedFrame>();
					_hasEditedRecordData = true;
				}
			}

			_hasPlaygroundSystem = playgroundSystem != null;
		}

		void OnDisable ()
		{
			if (!_hasPlaygroundSystem)
				return;
			playgroundSystem.inPlayback = false;
			playgroundSystem.calculate = false;
		}


		/****************************************************************************
		 	Public functions
		 ****************************************************************************/

		/// <summary>
		/// Determines if this Playground Recorder has recorded frames.
		/// </summary>
		/// <returns><c>true</c> if this Playground Recorder has recorded frames; otherwise, <c>false</c>.</returns>
		public bool HasRecordedFrames ()
		{
			return recordedFrames!=null && recordedFrames.Count>0;
		}

		/// <summary>
		/// Determines if this Playground Recorder is recording.
		/// </summary>
		/// <returns><c>true</c> if this Playground Recorder is recording; otherwise, <c>false</c>.</returns>
		public bool IsRecording ()
		{
			return _isRecording;
		}

		/// <summary>
		/// Determines if this Playground Recorder is currently replaying recorded data.
		/// </summary>
		/// <returns><c>true</c> if this Playground Recorder is replaying; otherwise, <c>false</c>.</returns>
		public bool IsReplaying ()
		{
			return _isReplaying;
		}

		/// <summary>
		/// Determines whether this Playground Recorder is in playback mode (_isReplaying can be in a stopped state but _inPlayback can still be true).
		/// </summary>
		/// <returns><c>true</c> if this Playground Recorder is in playback; otherwise, <c>false</c>.</returns>
		public bool IsInPlayback ()
		{
			return _inPlayback;
		}

		/// <summary>
		/// Returns the amount of recorded frames.
		/// </summary>
		/// <returns>The number of recorded frames.</returns>
		public int FrameCount ()
		{
			if (recordedFrames == null)
				return -1;
			return recordedFrames.Count-1<0? 0 : recordedFrames.Count-1;
		}

		/// <summary>
		/// Gets the frame at normalized time. This is always a floored value to the closest frame of the normalizedTime.
		/// </summary>
		/// <returns>The frame at time.</returns>
		/// <param name="normalizedTime">Normalized time.</param>
		public int GetFrameAtTime (float normalizedTime)
		{
			if (recordedFrames == null || recordedFrames.Count==0)
				return 0;
			return Mathf.Clamp (Mathf.FloorToInt((recordedFrames.Count-1) * Mathf.Clamp01(normalizedTime)), 0, recordedFrames.Count-1);
		}

		/// <summary>
		/// Gets the floating frame number at normalized time. Example: Passing in normalizedTime of 0.5 and total FrameCount is 3, the floating frame would return 1.5.
		/// </summary>
		/// <returns>The floating frame at time.</returns>
		/// <param name="normalizedTime">Normalized time.</param>
		public float GetFloatingFrameAtTime (float normalizedTime)
		{
			if (recordedFrames == null || recordedFrames.Count<1)
				return 0;

			return (recordedFrames.Count-1) * Mathf.Clamp01(normalizedTime);
		}

		/// <summary>
		/// Gets the normalized time at specified recorded frame.
		/// </summary>
		/// <returns>The normalized time at frame.</returns>
		/// <param name="frame">The recorded frame.</param>
		public float GetTimeAtFrame (int frame)
		{
			frame = Mathf.Clamp (frame, 0, recordedFrames.Count);
			return (frame*1f)/(recordedFrames.Count-1);
		}

		/// <summary>
		/// Gets the keyframe interval at specified frame.
		/// </summary>
		/// <returns>The keyframe interval at frame.</returns>
		/// <param name="frame">Frame.</param>
		public float GetKeyframeIntervalAtFrame (int frame)
		{
			if (recordedFrames==null || frame<0 || frame>=recordedFrames.Count)
				return 0;
			return recordedFrames[frame].keyframeInterval;
		}

		/// <summary>
		/// Returns when the recording started using Time.realtimeSinceStartup.
		/// </summary>
		/// <returns>The started.</returns>
		public float RecordingStarted ()
		{
			return _recordingStarted;
		}

		/// <summary>
		/// Gets the time (real time since startup) when the last frame was recorded.
		/// </summary>
		/// <returns>The last recorded frame time.</returns>
		public float GetLastRecordedFrameTime ()
		{
			return _lastRecordedFrameTime;
		}

		/// <summary>
		/// Starts a recording until StopRecording() is called. This overload will by default use the previously set keyframe interval during recording.
		/// </summary>
		public void StartRecording ()
		{
			if (playgroundSystem == null)
				return;
			playgroundSystem.inPlayback = false;
			if (_isRecording)
				StopRecording();
			_isRecording = true;
			_isReplaying = false;
			_inPlayback = false;
			_recordingStarted = Time.realtimeSinceStartup;

			if (Application.isPlaying)
				StartCoroutine (RecordInternal(keyframeInterval));

		}

		/// <summary>
		/// Starts a recording until StopRecording() is called. This overload takes a keyframe interval as parameter.
		/// </summary>
		/// <param name="keyframeInterval">The Keyframe Interval determines the rate of created keyframes (measured in seconds where 1f is 1 second).</param>
		public void StartRecording (float keyframeInterval)
		{
			this.keyframeInterval = keyframeInterval;
			StartRecording();
		}

		/// <summary>
		/// Starts a recording with specified length or until StopRecording() is called. This overload takes a recording length and keyframe interval as parameter.
		/// </summary>
		/// <param name="recordingLength">The amount of seconds the recording should be.</param>
		/// <param name="keyframeInterval">The Keyframe Interval determines the rate of created keyframes (measured in seconds where 1f is 1 second).</param>
		public void StartRecording (float recordingLength, float keyframeInterval)
		{
			if (playgroundSystem == null)
				return;
			StartCoroutine(StartRecordingInternal(recordingLength, keyframeInterval));
		}

		/// <summary>
		/// Records one frame. This can be useful if you want exact control of when keyframes should be created.
		/// </summary>
		public void RecordOneFrame ()
		{
			RecFrame();
		}

		/// <summary>
		/// Inserts a recorded frame into the specified frame index. This can be useful if you want to add frames into the recording that shouldn't be placed last. Use FrameCount() to determine how many frames you currently have.
		/// </summary>
		/// <param name="frame">The index of where the frame should be inserted.</param>
		/// <param name="frameType">The type of the inserted frame (by default FrameType.Middle).</param>
		public void InsertOneFrame (int frame, FrameType frameType = FrameType.Middle)
		{
			InsertRecFrame(frame, frameType);
		}

		/// <summary>
		/// Stops the ongoing recording.
		/// </summary>
		public void StopRecording ()
		{
			if (_isRecording)
			{
				_isRecording = false;
				_recordingEndFrame = recordedFrames.Count-1;
				CancelInvoke("RecFrame");
			
				if (recordedFrames.Count > 0)
				{
					_recordingStartFrame = Mathf.Clamp (_recordingStartFrame, 0, recordedFrames.Count);
					_recordingEndFrame = Mathf.Clamp (_recordingEndFrame, 0, recordedFrames.Count);
					recordedFrames[_recordingStartFrame].frameType = FrameType.Start;
					recordedFrames[_recordingEndFrame].frameType = FrameType.End;
				}
			}
		}

		/// <summary>
		/// Clears out the current recorded frames.
		/// </summary>
		public void ClearRecording ()
		{
			_isReplaying = false;
			StopRecording();
			StopPlayback();

			recordedFrames = null;
			recordedFrames = new List<RecordedFrame>();
			if (recorderData != null)
				recorderData.Clear();
			_hasEditedRecordData = true;

			_recordingStartFrame = 0;
			_recordingEndFrame = 0;

			if (_hasPlaygroundSystem)
			{
				playgroundSystem.inPlayback = false;
				playgroundSystem.calculate = false;
			}
		}

		/// <summary>
		/// Starts the playback of this Playground Recorder.
		/// </summary>
		public void Play ()
		{
			Play (playHead, playbackSpeed, loopPlayback);
		}

		/// <summary>
		/// Starts the playback of this Playground Recorder with specified playback speed.
		/// </summary>
		/// <param name="speed">The speed of the playback.</param>
		public void Play (float speed)
		{
			playbackSpeed = speed;
			Play (playHead, speed, loopPlayback);
		}

		/// <summary>
		/// Starts the playback of this Playground Recorder with specified starting point, playback speed and if looping should occur.
		/// </summary>
		/// <param name="fromNormalizedTime">From normalized time in recording.</param>
		/// <param name="speed">The speed of the playback.</param>
		/// <param name="repeat">If set to <c>true</c> then enable looping.</param>
		public void Play (float fromNormalizedTime, float speed, bool repeat)
		{
			if (!_hasPlaygroundSystem)
				return;
			if (!_isReplaying && localSpaceOnPlayback)
			{
				_previousSimulationSpace = playgroundSystem.shurikenParticleSystem.simulationSpace;
				playgroundSystem.shurikenParticleSystem.simulationSpace = ParticleSystemSimulationSpace.Local;
			}

			playgroundSystem.inPlayback = true;
			playbackSpeed = speed;
			loopPlayback = repeat;
			playHead = fromNormalizedTime;
			_isReplaying = true;
			StopRecording();
			StartPlayback();

			if (playHead >= 1f)
				playHead = 0;

			if (Application.isPlaying)
				StartCoroutine(PlayRecordedFrames(playHead));
		}

		/// <summary>
		/// Pauses the playback of this Playground Recorder.
		/// </summary>
		public void Pause ()
		{
			if (!_hasPlaygroundSystem)
				return;
			playgroundSystem.inPlayback = true;
			_isReplaying = false;
			if (_isRecording)
				StopRecording();
			StartPlayback();
		}

		/// <summary>
		/// Stops the playback and recording of this Playground Recorder.
		/// </summary>
		public void Stop ()
		{
			if (!_hasPlaygroundSystem)
				return;
			if (_isReplaying && localSpaceOnPlayback)
				playgroundSystem.shurikenParticleSystem.simulationSpace = _previousSimulationSpace;
			playgroundSystem.inPlayback = false;
			_isReplaying = false;
			if (_isRecording)
				StopRecording();
			StopPlayback();
		}

		/// <summary>
		/// Stops the playback and recording of this Playground Recorder and serializes data into Recorder Data. If multithreading is enabled then the serialization will be asynchronous.
		/// </summary>
		public void StopAndSerialize ()
		{
			if (!_hasPlaygroundSystem)
				return;
			playgroundSystem.inPlayback = false;
			_isReplaying = false;
			if (_isRecording)
			{
				StopRecording();
				Serialize();
			}
			StopPlayback();
		}

		/// <summary>
		/// Serializes the current recorded frames into the Recorder Data. If multithreading is enabled then the serialization will be asynchronous.
		/// </summary>
		public void Serialize ()
		{
			if (_hasRecorderData && _hasEditedRecordData)
			{
				if (multithreading)
					recorderData.SerializeAsync(recordedFrames);
				else
					recorderData.Serialize(recordedFrames);

				_hasEditedRecordData = false;
			}
		}

		/// <summary>
		/// Loads frames from the Recorder Data.
		/// </summary>
		public void Load ()
		{
			if (recorderData != null)
			{
				recordedFrames = recorderData.CloneAsRecordedFrames();
				_hasEditedRecordData = true;
			}
			else
				Debug.Log ("No Playground Recorder Data to load from!", gameObject);
		}

		/// <summary>
		/// Loads frames from the Recorder Data asynchronously.
		/// </summary>
		public void LoadAsync ()
		{
			if (recorderData == null)
			{
				Debug.Log ("No Playground Recorder Data to load from!", gameObject);
				return;
			}
			PlaygroundC.RunAsync(() => {
				recordedFrames = recorderData.CloneAsRecordedFrames();
				_hasEditedRecordData = true;
			});
		}

		/// <summary>
		/// Scrub to specified time in particle recording. This will linearly interpolate between the closest recorded frames of the passed in time (normalized between 0f - 1f).
		/// </summary>
		/// <param name="normalizedTime">The normalized time (0f to 1f).</param>
		public void Scrub (float normalizedTime)
		{
			if (!HasRecordedFrames())
				return;
			if (_isRecording)
				StopRecording();
			if (!_inPlayback)
				StartPlayback();
			if (multithreading)
				PlaygroundC.RunAsync(() => {
					lock (locker)
					{
						ScrubInternal(normalizedTime);
					}
				});
			else
				ScrubInternal (normalizedTime);
		}

		/// <summary>
		/// Trims (remove) the specified frames outside of normalized leftTime to rightTime. Returns true if trimming occurred.
		/// </summary>
		/// <param name="leftTime">The normalized left time (0 - 1).</param>
		/// <param name="rightTime">The normalized right time (0 - 1).</param>
		public bool Trim (float leftTime, float rightTime)
		{
			if (recordedFrames.Count == 0)
				return false;

			int leftFrame = GetFrameAtTime(leftTime);
			int rightFrame = GetFrameAtTime(rightTime);
			bool didTrim = false;

			if (leftFrame>0)
			{
				recordedFrames.RemoveRange(0, leftFrame);
				didTrim = true;
			}
			if (rightFrame<recordedFrames.Count-1)
			{
				int rFrames = (rightFrame - leftFrame);
				recordedFrames.RemoveRange(rFrames, recordedFrames.Count-rFrames);
				didTrim = true;
			}

			if (didTrim)
			{
				_hasEditedRecordData = true;
				Serialize();
			}

			return didTrim;
		}

		/// <summary>
		/// Trims (removes) the specified frames inside of normalized leftTime to rightTime.
		/// </summary>
		/// <param name="leftTime">The normalized left time (0 - 1).</param>
		/// <param name="rightTime">The normalized right time (0 - 1).</param>
		public void TrimInner (float leftTime, float rightTime)
		{
			if (recordedFrames.Count == 0)
				return;

			int leftFrame = GetFrameAtTime(leftTime);
			int rightFrame = GetFrameAtTime(rightTime);

			recordedFrames.RemoveRange(leftFrame, rightFrame-leftFrame);

			_hasEditedRecordData = true;
			Serialize();
		}

		/// <summary>
		/// Sets the particle system's live particles at the current position in playback (using the playhead) of this Playground Recorder. If multithreading is enabled this operation will run asynchronously.
		/// </summary>
		public void SetParticleSystemAsCurrentPlayback ()
		{
			SetParticleSystemAsRecording (playHead);
		}

		/// <summary>
		/// Sets the particle system's live particles at normalized time of the recorded frames. If multithreading is enabled this operation will run asynchronously.
		/// </summary>
		public void SetParticleSystemAsRecording (float normalizedTime)
		{
			if (playgroundSystem == null || _playbackParticles==null)
				return;
			if (multithreading)
			{
				PlaygroundC.RunAsync(() => {
					lock (locker)
					{
						SetParticleSystemAsRecordingInternal(normalizedTime);
					}
				});
			}
			else
			{
				SetParticleSystemAsRecordingInternal(normalizedTime);
			}
		}


		/****************************************************************************
			Internal functions
		 ****************************************************************************/

#if UNITY_EDITOR
		float lastFrameTime;
		void Update ()
		{
			_hasPlaygroundSystem = playgroundSystem != null;
			_hasRecorderData = recorderData != null;
			if (!_hasRecorderData)
				_hasEditedRecordData = true;
 
			// Enables recording in Editor non Play-mode
			if (!Application.isPlaying)
			{
				if (_isRecording)
				{
					if (Time.realtimeSinceStartup >= _lastRecordedFrameTime + keyframeInterval)
						RecFrame();
				} 
				else if (_isReplaying)
				{
					playHead += (playbackSpeed/(FrameCount()*keyframeInterval))*(Time.realtimeSinceStartup-lastFrameTime);
					if (playHead>1f)
						playHead = 1f;
					else if (playHead<0)
						playHead = 0;
					
					Scrub (playHead);
					
					if (playHead == 1f)
					{
						if (loopPlayback)
							playHead = 0;
						else
							_isReplaying = false;
					}
					else if (playHead == 0)
					{
						if (loopPlayback)
							playHead = 1f;
						else
							_isReplaying = false;
					}
				}
			}
			lastFrameTime = Time.realtimeSinceStartup;
		}
#endif

		void RecFrame () {
			if (!_isRecording)
				return;
			if (!playgroundSystem.calculate)
			{
				CancelInvoke ("RecFrame");
				return;
			}
			_lastRecordedFrameTime = Time.realtimeSinceStartup;
			if (playgroundSystem.particleCache != null)
			{
				if (recordedFrames == null)
					recordedFrames = new List<RecordedFrame>();
				recordedFrames.Add (new RecordedFrame(playgroundSystem, keyframeInterval));
				_hasEditedRecordData = true;
			}
		}

		void InsertRecFrame (int frame, FrameType frameType) {
			if (playgroundSystem.particleCache != null)
			{
				if (recordedFrames == null)
					recordedFrames = new List<RecordedFrame>();
				recordedFrames.Insert (frame, new RecordedFrame(playgroundSystem, keyframeInterval));
				recordedFrames[frame].frameType = frameType;
				_hasEditedRecordData = true;
			}
		}

		void StartPlayback ()
		{
			if (!_inPlayback && recordedFrames!=null && recordedFrames.Count>0 && playgroundSystem!=null)
			{
				_inPlayback = true;
				playgroundSystem.calculate = false;
				playgroundSystem.inPlayback = true;
				if (recordedFrames[0].particles != null)
					_playbackParticles = (ParticleSystem.Particle[])recordedFrames[0].CloneAsParticles();
				StartCoroutine (Playback());
			}
		}
		
		void StopPlayback ()
		{
			_inPlayback = false;
			if (_hasPlaygroundSystem)
			{
				playgroundSystem.calculate = true;
			}
		}

		void ScrubInternal (float normalizedTime) 
		{
			// Set time parameters
			normalizedTime = Mathf.Clamp01(normalizedTime);
			int normalizedFrame = GetFrameAtTime(normalizedTime);

			// Interpolation skipping if this is an end-frame
			if (skipInterpolationOnEndFrames && normalizedTime < 1f && recordedFrames[normalizedFrame].frameType == FrameType.End)
			{
				normalizedTime = Mathf.Clamp01(normalizedTime + (GetTimeAtFrame(normalizedFrame+1)-normalizedTime));
				playHead = normalizedTime;
				normalizedFrame = GetFrameAtTime(normalizedTime);
			}

			int targetFrame = Mathf.Clamp (normalizedFrame+1, 0, recordedFrames.Count);
			int rebirthRotationFrame = Mathf.Clamp (targetFrame+1, 0, recordedFrames.Count);

			// Set live particles in case the length doesn't add up
			if (_playbackParticles == null || _playbackParticles.Length != recordedFrames[normalizedFrame].particles.Length)
			{
				if (recordedFrames[normalizedFrame].particles != null)
					_playbackParticles = (ParticleSystem.Particle[])recordedFrames[normalizedFrame].CloneAsParticles();
				else return;
			}

			// No need to interpolate
			if (normalizedFrame >= recordedFrames.Count-1 || normalizedTime == 0 && !loopPlayback)
				return;

			// Delta time is the time between the normalized frame and next based on normalized time
//			float deltaTime = Mathf.Lerp (0, (recordedFrames.Count-1)*1f, normalizedTime)%1f;
			float deltaTime = 1f-(targetFrame-(GetFloatingFrameAtTime(normalizedTime)));

			// Set particle values
			for (int i = 0; i<_playbackParticles.Length; i++) 
			{
				// If particle is between death/birth blend in differently
				if (recordedFrames[normalizedFrame].particles[i].lifetime < recordedFrames[targetFrame].particles[i].lifetime)
				{
					Color32 inColor = fadeIn? new Color32(recordedFrames[normalizedFrame].particles[i].color.r, recordedFrames[normalizedFrame].particles[i].color.g, recordedFrames[normalizedFrame].particles[i].color.b, 0) : new Color32();
					_playbackParticles[i].position = Vector3.Lerp (recordedFrames[normalizedFrame].particles[i].sourcePosition, recordedFrames[targetFrame].particles[i].position, deltaTime);
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
					_playbackParticles[i].size = Mathf.Lerp (!sizeIn? recordedFrames[normalizedFrame].particles[i].startingSize : 0, recordedFrames[targetFrame].particles[i].size, deltaTime);
					_playbackParticles[i].color = Color.Lerp (!fadeIn? recordedFrames[normalizedFrame].particles[i].color : inColor, recordedFrames[targetFrame].particles[i].color, deltaTime);
#else
					_playbackParticles[i].startSize = Mathf.Lerp (!sizeIn? recordedFrames[normalizedFrame].particles[i].startingSize : 0, recordedFrames[targetFrame].particles[i].size, deltaTime);
					_playbackParticles[i].startColor = Color.Lerp (!fadeIn? recordedFrames[normalizedFrame].particles[i].color : inColor, recordedFrames[targetFrame].particles[i].color, deltaTime);
#endif
					_playbackParticles[i].rotation = Mathf.Lerp (recordedFrames[targetFrame].particles[i].rotation - (recordedFrames[rebirthRotationFrame].particles[i].rotation * deltaTime), recordedFrames[targetFrame].particles[i].rotation, deltaTime);
				}
				
				// ...otherwise interpolate
				else
				{
					_playbackParticles[i].position = Vector3.Lerp (recordedFrames[normalizedFrame].particles[i].position, recordedFrames[targetFrame].particles[i].position, deltaTime);
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
					_playbackParticles[i].size = Mathf.Lerp (recordedFrames[normalizedFrame].particles[i].size, recordedFrames[targetFrame].particles[i].size, deltaTime);
					_playbackParticles[i].color = Color.Lerp (recordedFrames[normalizedFrame].particles[i].color, recordedFrames[targetFrame].particles[i].color, deltaTime);
#else
					_playbackParticles[i].startSize = Mathf.Lerp (recordedFrames[normalizedFrame].particles[i].size, recordedFrames[targetFrame].particles[i].size, deltaTime);
					_playbackParticles[i].startColor = Color.Lerp (recordedFrames[normalizedFrame].particles[i].color, recordedFrames[targetFrame].particles[i].color, deltaTime);
#endif
					_playbackParticles[i].rotation = Mathf.Lerp (recordedFrames[normalizedFrame].particles[i].rotation, recordedFrames[targetFrame].particles[i].rotation, deltaTime);
					_playbackParticles[i].lifetime = Mathf.Lerp (recordedFrames[normalizedFrame].particles[i].lifetime, recordedFrames[targetFrame].particles[i].lifetime, deltaTime);
				}
				
			}
		}

		void SetParticleSystemAsRecordingInternal (float normalizedTime)
		{
			normalizedTime = Mathf.Clamp01(normalizedTime);
			int normalizedFrame = GetFrameAtTime(normalizedTime);
			int targetFrame = Mathf.Clamp (normalizedFrame+1, 0, recordedFrames.Count-1);
			float deltaTime = 1f-(targetFrame-(GetFloatingFrameAtTime(normalizedTime)));

			float tStamp = recordedFrames[normalizedFrame].timeStamp;
			float currentTime = PlaygroundC.globalTime;

			int pCount = playgroundSystem.playgroundCache.position.Length;

			for (int i = 0; i<_playbackParticles.Length; i++)
			{
				playgroundSystem.playgroundCache.position[i%pCount] = Vector3.Lerp (recordedFrames[normalizedFrame].particles[i].position, recordedFrames[targetFrame].particles[i].position, deltaTime);
				playgroundSystem.playgroundCache.velocity[i%pCount] = Vector3.Lerp (recordedFrames[normalizedFrame].particles[i].velocity, recordedFrames[targetFrame].particles[i].velocity, deltaTime);
				playgroundSystem.playgroundCache.size[i%pCount] = Mathf.Lerp (recordedFrames[normalizedFrame].particles[i].size, recordedFrames[targetFrame].particles[i].size, deltaTime);
				playgroundSystem.playgroundCache.color[i%pCount] = Color.Lerp (recordedFrames[normalizedFrame].particles[i].color, recordedFrames[targetFrame].particles[i].color, deltaTime);
				playgroundSystem.playgroundCache.rotation[i%pCount] = Mathf.Lerp (recordedFrames[normalizedFrame].particles[i].rotation, recordedFrames[targetFrame].particles[i].rotation, deltaTime);
				playgroundSystem.playgroundCache.life[i%pCount] = Mathf.Lerp (recordedFrames[normalizedFrame].particles[i].playgroundLife, recordedFrames[targetFrame].particles[i].playgroundLife, deltaTime);
				playgroundSystem.playgroundCache.lifetimeSubtraction[i%pCount] = Mathf.Lerp (recordedFrames[normalizedFrame].particles[i].playgroundLifetimeSubtraction, recordedFrames[targetFrame].particles[i].playgroundLifetimeSubtraction, deltaTime);

				playgroundSystem.playgroundCache.birth[i%pCount] = currentTime + (recordedFrames[normalizedFrame].particles[i].playgroundStartLifetime-tStamp);
				playgroundSystem.playgroundCache.death[i%pCount] = currentTime + (recordedFrames[normalizedFrame].particles[i].playgroundEndLifetime-tStamp);
			}
			
			playgroundSystem.localTime = currentTime;
			playgroundSystem.LastTimeUpdated = currentTime;
			playgroundSystem.LocalDeltaTime = .001f;
			playgroundSystem.cameFromNonCalculatedFrame = false;
			playgroundSystem.cameFromNonEmissionFrame = false;
			playgroundSystem.loopExceeded = false;
			playgroundSystem.loopExceededOnParticle = -1;
			playgroundSystem.hasActiveParticles = true;

			StopAndSerialize();
		}


		/****************************************************************************
			Internal enumerators
		 ****************************************************************************/

		IEnumerator StartRecordingInternal (float recordingLength, float keyframeInterval)
		{
			_isReplaying = false;
			_inPlayback = false;
			_isRecording = true;
			playgroundSystem.inPlayback = false;
			StartCoroutine(RecordInternal(keyframeInterval));
			yield return new WaitForSeconds(recordingLength);
			StopRecording();
		}

		IEnumerator RecordInternal (float keyframeInterval)
		{
			_recordingStartFrame = recordedFrames.Count>0? recordedFrames.Count-1 : 0;
			while (_isRecording)
			{
				RecFrame();
				yield return new WaitForSeconds(keyframeInterval);
			}
		}

		IEnumerator Playback ()
		{
			while (_inPlayback)
			{
				if (_playbackParticles != null)
					playgroundSystem.shurikenParticleSystem.SetParticles(_playbackParticles, _playbackParticles.Length);
				yield return null;
			}
		}

		IEnumerator PlayRecordedFrames (float fromNormalizedTime)
		{
			float t = fromNormalizedTime;
			while (_isReplaying)
			{
				t += (playbackSpeed/(FrameCount()*keyframeInterval))*Time.deltaTime;
				if (t>1f)
					t = 1f;
				else if (t<0)
					t = 0;

				Scrub (t);

				if (t == 1f)
				{
					if (loopPlayback)
						t = 0;
					else
						_isReplaying = false;
				}
				else if (t == 0)
				{
					if (loopPlayback)
						t = 1f;
					else
						_isReplaying = false;
				}
				playHead = t;
				yield return null;
			}
		}
	}

	/// <summary>
	/// A Playback Particle is a struct for keeping information about one single particle in a Playground Recorder's recorded data.
	/// </summary>
	public struct PlaybackParticle
	{
		[HideInInspector] public Vector3 position;
		[HideInInspector] public Vector3 velocity;
		[HideInInspector] public float rotation;
		[HideInInspector] public float size;
		[HideInInspector] public float lifetime;
		[HideInInspector] public float startLifetime;
		[HideInInspector] public float playgroundLife;
		[HideInInspector] public float playgroundStartLifetime;
		[HideInInspector] public float playgroundEndLifetime;
		[HideInInspector] public float playgroundLifetimeSubtraction;
		[HideInInspector] public Color32 color;

		[HideInInspector] public Vector3 sourcePosition;
		[HideInInspector] public float startingSize;

		public PlaybackParticle (Vector3 position, 
		                         Vector3 velocity, 
		                         float rotation, 
		                         float size, 
		                         float lifetime, 
		                         float startLifetime, 
		                         float playgroundLife, 
		                         float playgroundStartLifetime, 
		                         float playgroundEndLifetime, 
		                         float playgroundLifetimeSubtraction, 
		                         Color32 color,

		                         Vector3 sourcePosition,
		                         float startingSize
		                        )
		{
			this.position = position;
			this.velocity = velocity;
			this.rotation = rotation;
			this.size = size;
			this.lifetime = lifetime;
			this.startLifetime = startLifetime;
			this.playgroundLife = playgroundLife;
			this.playgroundStartLifetime = playgroundStartLifetime;
			this.playgroundEndLifetime = playgroundEndLifetime;
			this.playgroundLifetimeSubtraction = playgroundLifetimeSubtraction;
			this.color = color;

			this.sourcePosition = sourcePosition;
			this.startingSize = startingSize;
		}

		public ParticleSystem.Particle CloneAsParticle (bool includeVelocity = false)
		{
			ParticleSystem.Particle particle = new ParticleSystem.Particle();
			particle.position = position;
			particle.rotation = rotation;
			particle.lifetime = lifetime;
			particle.startLifetime = startLifetime;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			particle.size = size;
			particle.color = color;
#else
			particle.startSize = size;
			particle.startColor = color;
#endif

			if (includeVelocity)
				particle.velocity = velocity;

			return particle;
		}

		public SerializedParticle CloneAsSerializedParticle ()
		{
			SerializedParticle particle = new SerializedParticle(
				position,
				velocity,
				rotation,
				size,
				lifetime,
				startLifetime,
				playgroundLife,
				playgroundStartLifetime,
				playgroundEndLifetime,
				playgroundLifetimeSubtraction,
				color,

				sourcePosition,
				startingSize
			);
			return particle;
		}

		public PlaybackParticle Clone ()
		{
			PlaybackParticle particle = new PlaybackParticle(
				position,
				velocity,
				rotation,
				size,
				lifetime,
				startLifetime,
				playgroundLife,
				playgroundStartLifetime,
				playgroundEndLifetime,
				playgroundLifetimeSubtraction,
				color,

				sourcePosition,
				startingSize
			);
			return particle;
		}
	}

	/// <summary>
	/// A frame type describes where in the total recording the recorded frame was made.
	/// </summary>
	public enum FrameType
	{
		/// <summary>
		/// The frame was created at the start of recording.
		/// </summary>
		Start,
		/// <summary>
		/// The frame was created at the middle of recording.
		/// </summary>
		Middle,
		/// <summary>
		/// The frame was created at the end of recording.
		/// </summary>
		End
	}
}