using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ParticlePlayground {

	/// <summary>
	/// A serialized frame holds information about one recorded frame of particles.
	/// </summary>
	[Serializable]
	public class SerializedFrame
	{
		/// <summary>
		/// The array of particle data.
		/// </summary>
		public SerializedParticle[] serializedParticles;
		/// <summary>
		/// The time during simulation this frame was made.
		/// </summary>
		public float timeStamp;
		/// <summary>
		/// The keyframe interval setting when this frame was made.
		/// </summary>
		public float keyframeInterval;
		/// <summary>
		/// The type of this frame (FrameType.Start, FrameType.Middle or FrameType.End).
		/// </summary>
		public FrameType frameType = FrameType.Middle;
		
		public RecordedFrame CloneAsRecordedFrame ()
		{
			RecordedFrame recordedFrame = new RecordedFrame();
			recordedFrame.particles = CloneAsPlaybackParticles();
			recordedFrame.keyframeInterval = keyframeInterval;
			recordedFrame.timeStamp = timeStamp;
			recordedFrame.frameType = frameType;
			return recordedFrame;
		}
		
		public PlaybackParticle[] CloneAsPlaybackParticles ()
		{
			PlaybackParticle[] recordedParticles = new PlaybackParticle[serializedParticles.Length];
			for (int i = 0; i<serializedParticles.Length; i++)
				recordedParticles[i] = serializedParticles[i].CloneAsPlaybackParticle();
			return recordedParticles;
		}
	}
}