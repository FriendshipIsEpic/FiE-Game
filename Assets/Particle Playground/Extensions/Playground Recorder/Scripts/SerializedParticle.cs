using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ParticlePlayground {

	/// <summary>
	/// A serialized particle holds information for one single serializable particle inside a recorded frame for the Playground Recorder's recorded data.
	/// </summary>
	[Serializable]
	public class SerializedParticle
	{
		public Vector3 position;
		public Vector3 velocity;
		public float rotation;
		public float size;
		public float lifetime;
		public float startLifetime;
		public float playgroundLife;
		public float playgroundStartLifetime;
		public float playgroundEndLifetime;
		public float playgroundLifetimeSubtraction;
		public Color32 color;
		
		public Vector3 sourcePosition;
		public float startingSize;
		
		public SerializedParticle (Vector3 position, 
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
		
		public PlaybackParticle CloneAsPlaybackParticle ()
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
}