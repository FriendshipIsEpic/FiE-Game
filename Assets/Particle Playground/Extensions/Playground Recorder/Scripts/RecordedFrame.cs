using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ParticlePlayground {
	/// <summary>
	/// The RecordedFrame class contain information of a single frame of recorded particles for the Playground Recorder's recorded data.
	/// </summary>
	[Serializable]
	public class RecordedFrame {
		/// <summary>
		/// The array of particle data. PlaybackParticle is a struct and therefore not serialized due to performance.
		/// </summary>
		[HideInInspector] public PlaybackParticle[] particles;
		/// <summary>
		/// The time during simulation this frame was made.
		/// </summary>
		[HideInInspector] public float timeStamp;
		/// <summary>
		/// The keyframe interval setting when this frame was made.
		/// </summary>
		[HideInInspector] public float keyframeInterval;
		/// <summary>
		/// The type of this frame (FrameType.Start, FrameType.Middle or FrameType.End).
		/// </summary>
		[HideInInspector] public FrameType frameType = FrameType.Middle;
		
		public RecordedFrame () {}
		
		public RecordedFrame (PlaygroundParticlesC playgroundParticles, float keyframeInterval) {
			particles = new PlaybackParticle[playgroundParticles.particleCache.Length];
			for (int i = 0; i<particles.Length; i++)
			{
				particles[i] = new PlaybackParticle(
					playgroundParticles.playgroundCache.position[i],
					playgroundParticles.playgroundCache.velocity[i],
					playgroundParticles.playgroundCache.rotation[i],
					playgroundParticles.playgroundCache.size[i],
					playgroundParticles.particleCache[i].lifetime,
					playgroundParticles.particleCache[i].startLifetime,
					playgroundParticles.playgroundCache.life[i],
					playgroundParticles.playgroundCache.birth[i],
					playgroundParticles.playgroundCache.death[i],
					playgroundParticles.playgroundCache.lifetimeSubtraction[i],
					playgroundParticles.playgroundCache.color[i],
					
					playgroundParticles.playgroundCache.targetPosition[i],
					playgroundParticles.playgroundCache.initialSize[i]
					);
			}
			timeStamp = Time.realtimeSinceStartup;
			this.keyframeInterval = keyframeInterval;
		}
		
		public RecordedFrame Clone ()
		{
			RecordedFrame recordedFrame = new RecordedFrame();
			recordedFrame.particles = (PlaybackParticle[])particles.Clone();
			recordedFrame.timeStamp = timeStamp;
			recordedFrame.frameType = frameType;
			return recordedFrame;
		}
		
		public ParticleSystem.Particle[] CloneAsParticles ()
		{
			ParticleSystem.Particle[] p = new ParticleSystem.Particle[particles.Length];
			for (int i = 0; i<p.Length; i++)
			{
				p[i] = particles[i].CloneAsParticle();
			}
			return p;
		}
		
		public SerializedFrame CloneAsSerializedFrame ()
		{
			SerializedFrame serializedFrame = new SerializedFrame();
			serializedFrame.serializedParticles = CloneAsSerializedParticles();
			serializedFrame.keyframeInterval = keyframeInterval;
			serializedFrame.timeStamp = timeStamp;
			serializedFrame.frameType = frameType;
			return serializedFrame;
		}
		
		public SerializedParticle[] CloneAsSerializedParticles ()
		{
			SerializedParticle[] serializedParticles = new SerializedParticle[particles.Length];
			for (int i = 0; i<serializedParticles.Length; i++)
				serializedParticles[i] = particles[i].CloneAsSerializedParticle();
			return serializedParticles;
		}
	}
}