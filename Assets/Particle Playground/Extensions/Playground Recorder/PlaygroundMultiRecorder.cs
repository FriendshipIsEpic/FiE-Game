using UnityEngine;
using System.Collections;

namespace ParticlePlayground {

	/// <summary>
	/// The Particle Playground Multi Recorder enables playback and scrubbing of multiple recorded synchronized particle systems.
	/// </summary>
	public class PlaygroundMultiRecorder : MonoBehaviour {

		/// <summary>
		/// The Particle Playground Recorders you wish to record/playback from.
		/// </summary>
		public PlaygroundRecorder[] playgroundRecorders;

		public void StartRecording (float frameIntervalInSeconds)
		{
			for (int i = 0; i<playgroundRecorders.Length; i++)
				playgroundRecorders[i].StartRecording(frameIntervalInSeconds);
		}

		public void StartRecording (float recordingLength, float frameIntervalInSeconds)
		{
			for (int i = 0; i<playgroundRecorders.Length; i++)
				playgroundRecorders[i].StartRecording(recordingLength, frameIntervalInSeconds);
		}

		public void RecordOneFrame ()
		{
			for (int i = 0; i<playgroundRecorders.Length; i++)
				playgroundRecorders[i].RecordOneFrame();
		}

		public void InsertOneFrame (int frame)
		{
			for (int i = 0; i<playgroundRecorders.Length; i++)
				playgroundRecorders[i].InsertOneFrame(frame);
		}

		public void StopRecording ()
		{
			for (int i = 0; i<playgroundRecorders.Length; i++)
				playgroundRecorders[i].StopRecording();
		}

		public void ClearRecording ()
		{
			for (int i = 0; i<playgroundRecorders.Length; i++)
				playgroundRecorders[i].ClearRecording();
		}

		public void Play (float speed)
		{
			for (int i = 0; i<playgroundRecorders.Length; i++)
				playgroundRecorders[i].Play(speed);
		}

		public void Play (float fromNormalizedTime, float speed, bool repeat)
		{
			for (int i = 0; i<playgroundRecorders.Length; i++)
				playgroundRecorders[i].Play(fromNormalizedTime, speed, repeat);
		}

		public void Stop ()
		{
			for (int i = 0; i<playgroundRecorders.Length; i++)
				playgroundRecorders[i].Stop();
		}

		public void Pause ()
		{
			for (int i = 0; i<playgroundRecorders.Length; i++)
				playgroundRecorders[i].Pause();
		}

		public void Scrub (float normalizedTime)
		{
			for (int i = 0; i<playgroundRecorders.Length; i++)
				playgroundRecorders[i].Scrub(normalizedTime);
		}
	}
}