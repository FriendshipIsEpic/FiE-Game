using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ParticlePlayground {
	public class TrailPoint 
	{
		public Vector3 position;
		public Vector3 velocity;
		public float lifetime;
		public float startLifetime;
		public float width;

		bool _canRemove = false;
		float _lastTimeUpdated;
		
		public TrailPoint (Vector3 position, float startLifetime, float creationTime)
		{
			this.position = position;
			this.lifetime = startLifetime;
			this.startLifetime = startLifetime;
			this.width = 0;

			_lastTimeUpdated = creationTime;
		}
		
		public TrailPoint (Vector3 position, float startLifetime, float width, float creationTime)
		{
			this.position = position;
			this.lifetime = startLifetime;
			this.startLifetime = startLifetime;
			this.width = width;

			_lastTimeUpdated = creationTime;
		}

		public TrailPoint (Vector3 position, Vector3 velocity, float startLifetime, float width, float creationTime)
		{
			this.position = position;
			this.lifetime = startLifetime;
			this.startLifetime = startLifetime;
			this.width = width;
			this.velocity = velocity;

			_lastTimeUpdated = creationTime;
		}
		
		public void Update (float updateTime, float width)
		{
			lifetime -= updateTime-_lastTimeUpdated;
			if (lifetime <= 0)
			{
				_canRemove = true;
				lifetime = 0;
			}
			this.width = width;
			_lastTimeUpdated = updateTime;
		}

		/// <summary>
		/// Gets the normalized lifetime of this trail point.
		/// </summary>
		/// <returns>The normalized lifetime.</returns>
		public float GetNormalizedLifetime () {
			return 1f-(lifetime/startLifetime);
		}
		
		/// <summary>
		/// Determines whether this point can be removed.
		/// </summary>
		/// <returns><c>true</c> if this point can be removed; otherwise, <c>false</c>.</returns>
		public bool CanRemove () {
			return _canRemove;
		}
	}
}