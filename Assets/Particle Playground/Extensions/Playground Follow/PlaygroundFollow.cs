using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ParticlePlayground {
	public class PlaygroundFollow : MonoBehaviour {

		/// <summary>
		/// Reference to the particle system.
		/// </summary>
		public PlaygroundParticlesC particles;
		/// <summary>
		/// Reference to an existing GameObject. This will be cloned to be used on every particle.
		/// </summary>
		public GameObject referenceObject;
		/// <summary>
		/// The lifetime of the followers. Set 0 to follow during each particle's individual lifetime.
		/// </summary>
		public float followerLifetime = 0;
		/// <summary>
		/// The size of the cache. Set 0 to automatically set the needed amount.
		/// </summary>
		public int cacheSize = 0;

		/// <summary>
		/// Determines if the Playground Followers should broadcast to any event listeners.
		/// </summary>
		public bool sendEvents = false;

		/// <summary>
		/// This event occurs when followers are born and sendEvents are set to true.
		/// </summary>
		public event OnPlaygroundFollower followerEventBirth;
		/// <summary>
		/// This event occurs when follower dies and sendEvents are set to true.
		/// </summary>
		public event OnPlaygroundFollower followerEventDeath;

		/// <summary>
		/// The reference to the trail renderer (if existing)
		/// </summary>
		TrailRenderer referenceTrailRenderer;
		/// <summary>
		/// If the follower has a Trail Renderer component, this sets trail time once the follower is active again.
		/// </summary>
		float trailTime = 0;
		/// <summary>
		/// The list of active followers.
		/// </summary>
		List<PlaygroundFollower> followers = new List<PlaygroundFollower>();
		/// <summary>
		/// As Playground is running in a multithreaded environment we need a queue for instantiation (which cannot be called from a different thread).
		/// </summary>
		List<PlaygroundFollower> waitingFollowers = new List<PlaygroundFollower>();
		PlaygroundFollower[] referenceObjectsCache;
		PlaygroundFollower[] queue = new PlaygroundFollower[0];
		int cacheIndex = 0;

		PlaygroundEventC birthEvent;
		PlaygroundEventC deathEvent;
		Transform followerParent;

		void Start () 
		{
			if (referenceObject == null || particles == null)
				return;

			// Create and setup the birth event
			birthEvent = PlaygroundC.CreateEvent(particles);
			birthEvent.broadcastType = EVENTBROADCASTC.EventListeners;
			birthEvent.eventType = EVENTTYPEC.Birth;

			// Create and setup the death event
			deathEvent = PlaygroundC.CreateEvent(particles);
			deathEvent.broadcastType = EVENTBROADCASTC.EventListeners;
			deathEvent.eventType = EVENTTYPEC.Death;

			// Hook up the event listeners to the delegates
			birthEvent.particleEvent += OnParticleDidBirth;
			deathEvent.particleEvent += OnParticleDidDie;

			// Create a parent for all followers (for Hierarchy convenience)
			followerParent = new GameObject("Followers").transform;
			followerParent.parent = transform;

			// Get the trail renderer (if available) and its time
			referenceTrailRenderer = referenceObject.GetComponent<TrailRenderer>();
			if (referenceTrailRenderer!=null)
				trailTime = referenceTrailRenderer.time;

			// Set an extra amount of followers if required (a trail's time will exceed a particle's)
			int extra = followerLifetime<=0? 
				Mathf.CeilToInt(Mathf.Abs (particles.lifetime-trailTime)+(trailTime-particles.lifetime))+2 : 
					Mathf.CeilToInt(Mathf.Abs (particles.lifetime-followerLifetime)+(followerLifetime-particles.lifetime))+2 ;
			if (particles.lifetime<=1f) extra++;

			// Create the follower cache (this will be iterated through and reused whenever a particle rebirths)
			referenceObjectsCache = new PlaygroundFollower[cacheSize>0? cacheSize : particles.particleCount+Mathf.CeilToInt(particles.particleCount*extra)];
			for (int i = 0; i<referenceObjectsCache.Length; i++) {
				GameObject clone = (GameObject)Instantiate(referenceObject);
				referenceObjectsCache[i] = new PlaygroundFollower(clone.transform, clone, clone.GetComponent<TrailRenderer>(), 0, 0);
				referenceObjectsCache[i].transform.parent = followerParent;
				if (referenceObjectsCache[i].trailRenderer!=null)
					referenceObjectsCache[i].trailRenderer.time = 0;
				referenceObjectsCache[i].gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Event listener for particle birth.
		/// </summary>
		/// <param name="particle">Particle.</param>
		void OnParticleDidBirth (PlaygroundEventParticle particle) 
		{
			waitingFollowers.Add (new PlaygroundFollower(null, null, null, followerLifetime<=0? particle.totalLifetime+trailTime : followerLifetime, particle.particleId));
		}

		/// <summary>
		/// Event listener for particle death.
		/// </summary>
		/// <param name="particle">Particle.</param>
		void OnParticleDidDie (PlaygroundEventParticle particle) 
		{
			int followerId = GetFollowerWithId(particle.particleId);
			if (followerId<0) return;
			followers[followerId].enabled = false;
		}

		/// <summary>
		/// Gets the follower which has the passed in particle identifier.
		/// </summary>
		/// <returns>The follower with particle identifier.</returns>
		/// <param name="particleId">Particle identifier.</param>
		int GetFollowerWithId (int particleId) 
		{
			float lowestLife = 999f;
			int returnIndex = -1;
			for (int i = 0; i<followers.Count; i++)
				if (followers[i].particleId==particleId && followers[i].lifetime<lowestLife)
					returnIndex = i;
			return returnIndex;
		}

		void Update () 
		{
			if (waitingFollowers.Count>0) 
			{
				queue = waitingFollowers.ToArray();
			}
		}
		void LateUpdate () 
		{
			UpdateFollowers();
		}

		void UpdateFollowers () 
		{

			// Follow, lifetime, remove
			for (int i = 0; i<followers.Count; i++) 
			{
				// Follow particle
				if (followers[i].enabled)
					followers[i].transform.position = particles.particleCache[followers[i].particleId].position;
				
				// Subtract lifetime
				followers[i].lifetime -= Time.deltaTime;

				// Remove if no lifetime left
				if (followers[i].lifetime<=0) {
					RemoveFollower(i);
					continue;
				}
			}

			// Add any waiting followers to the live follower list. The waiting list may change during iteration!
			if (queue.Length>0) 
			{
				if (queue.Length!=waitingFollowers.Count) return;
				int inQueueThisFrame = waitingFollowers.Count;

				foreach (PlaygroundFollower wFollower in queue) 
				{
					AddFollower (wFollower, followers.Count-1);
				}
				if (inQueueThisFrame==waitingFollowers.Count)
					waitingFollowers = new List<PlaygroundFollower>();
				else waitingFollowers.RemoveRange (0, inQueueThisFrame-1);
				queue = new PlaygroundFollower[0];
			}
		}

		void AddFollower (PlaygroundFollower follower, int i) 
		{
			if (follower==null) return;
			followers.Add (follower.Clone());
			followers[followers.Count-1].enabled = true;
			followers[followers.Count-1].gameObject = referenceObjectsCache[cacheIndex].gameObject;
			followers[followers.Count-1].gameObject.SetActive(true);
			followers[followers.Count-1].transform = referenceObjectsCache[cacheIndex].transform;
			followers[followers.Count-1].trailRenderer = referenceObjectsCache[cacheIndex].trailRenderer;
			followers[followers.Count-1].particleId = follower.particleId;
			followers[followers.Count-1].transform.position = particles.playgroundCache.position[followers[followers.Count-1].particleId];
			if (followers[followers.Count-1].trailRenderer!=null)
				followers[followers.Count-1].trailRenderer.time = trailTime;
			if (sendEvents && followerEventBirth!=null)
				followerEventBirth(followers[followers.Count-1]);
			NextCacheIndex();
		} 

		void RemoveFollower (int i) 
		{
			if (sendEvents && followerEventDeath!=null)
				followerEventDeath(followers[i]);
			followers[i].enabled = false;
			if (followers[i].trailRenderer!=null)
				followers[i].trailRenderer.time = 0;
			followers[i].gameObject.SetActive(false);
			followers.RemoveAt(i);
		}

		void NextCacheIndex () 
		{
			cacheIndex = (cacheIndex+1)%referenceObjectsCache.Length;
		}

		/// <summary>
		/// Gets an active follower at index. This will only return active followers.
		/// </summary>
		/// <returns>The active follower.</returns>
		/// <param name="index">Index.</param>
		public PlaygroundFollower GetActiveFollower (int index)
		{
			index = Mathf.Clamp (index, 0, followers.Count);
			return followers[index];
		}

		/// <summary>
		/// Gets a cached follower at index. This can return inactive followers waiting for their turn to be enabled.
		/// </summary>
		/// <returns>The cached follower.</returns>
		/// <param name="index">Index.</param>
		public PlaygroundFollower GetCachedFollower (int index)
		{
			index = Mathf.Clamp (index, 0, referenceObjectsCache.Length);
			return referenceObjectsCache[index];
		}

		/// <summary>
		/// Gets the amount of active followers.
		/// </summary>
		/// <returns>The active followers count.</returns>
		public int GetActiveFollowersCount ()
		{
			return followers.Count;
		}

		/// <summary>
		/// Gets the amount of cached followers.
		/// </summary>
		/// <returns>The cached followers count.</returns>
		public int GetCachedFollowersCount ()
		{
			return referenceObjectsCache.Length;
		}
	}

	/// <summary>
	/// Playground follower class.
	/// </summary>
	public class PlaygroundFollower {
		public bool enabled = true;
		public float lifetime;
		public Transform transform;
		public GameObject gameObject;
		public TrailRenderer trailRenderer;
		public int particleId;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlaygroundFollower"/> class.
		/// </summary>
		/// <param name="setTransform">Transform to reposition.</param>
		/// <param name="setLifetime">Start lifetifetime.</param>
		/// <param name="setParticleId">Particle identifier to follow.</param>
		public PlaygroundFollower (Transform setTransform, GameObject setGameObject, TrailRenderer setTrailRenderer, float setLifetime, int setParticleId) 
		{
			transform = setTransform;
			gameObject = setGameObject;
			trailRenderer = setTrailRenderer;
			lifetime = setLifetime;
			particleId = setParticleId;
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		public PlaygroundFollower Clone () 
		{
			return new PlaygroundFollower (transform, gameObject, trailRenderer, lifetime, particleId);
		} 
	}

	/// <summary>
	/// Event delegate for sending a PlaygroundFollower to any event listeners.
	/// </summary>
	public delegate void OnPlaygroundFollower(PlaygroundFollower follower);
}