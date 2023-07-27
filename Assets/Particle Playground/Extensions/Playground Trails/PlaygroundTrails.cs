using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ParticlePlayground {

	[ExecuteInEditMode()]
	public class PlaygroundTrails : MonoBehaviour {

		/// <summary>
		/// The particle system this Playground Trail will follow.
		/// </summary>
		[HideInInspector] public PlaygroundParticlesC playgroundSystem;

		/// <summary>
		/// The material of each trail.
		/// </summary>
		[HideInInspector] public Material material;
		/// <summary>
		/// The lifetime color of all trails.
		/// </summary>
		[HideInInspector] public Gradient lifetimeColor = new Gradient();
		/// <summary>
		/// The point array alpha determines the alpha level over the trail points. This is a normalized value where 1 on the x-axis means all points, 1 on the y-axis means full alpha.
		/// </summary>
		[HideInInspector] public AnimationCurve pointArrayAlpha;
		/// <summary>
		/// The mode to color the trails with. If TrailColorMode.Lifetime is selected the coloring will be based on each point's lifetime. If TrailColorMode.PointArray is selected the coloring will be based on the points in the array.
		/// </summary>
		[HideInInspector] public TrailColorMode colorMode;
		/// <summary>
		/// The uv mode.
		/// </summary>
		[HideInInspector] public TrailUvMode uvMode;
		/// <summary>
		/// Determines the render mode of the trail. This sets the rotation direction of the trail points.
		/// </summary>
		[HideInInspector] public TrailRenderMode renderMode;
		/// <summary>
		/// The transform to billboard towards if renderMode is set to TrailRenderMode.Billboard. If none is set this will default to the Main Camera transform.
		/// </summary>
		[HideInInspector] public Transform billboardTransform;
		/// <summary>
		/// The custom render scale if renderMode is set to TrailRenderMode.CustomRenderScale. This ables you to set the normal direction (with multiplier) of the trails.
		/// </summary>
		[HideInInspector] public Vector3 customRenderScale = Vector3.one;
		/// <summary>
		/// Determines if the trails should receive shadows. Note that the shader of the material needs to support this.
		/// </summary>
		[HideInInspector] public bool receiveShadows;
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
		/// <summary>
		/// Determines if the trails should cast shadows (Unity 4). Note that the shader of the material needs to support this.
		/// </summary>
		[HideInInspector] public bool castShadows = false;
#else
		/// <summary>
		/// Determines if the trails should cast shadows. Note that the shader of the material needs to support this.
		/// </summary>
		[HideInInspector] public UnityEngine.Rendering.ShadowCastingMode shadowCastingMode;
#endif
		/// <summary>
		/// The time vertices is living on the trail (determines length).
		/// </summary>
		[HideInInspector] public float time = 3f;
		/// <summary>
		/// The width over normalized lifetime.
		/// </summary>
		public AnimationCurve timeWidth;
		/// <summary>
		/// The scale of start- and end width.
		/// </summary>
		public float widthScale = .1f;

		/// <summary>
		/// The minimum distance before new vertices can be created.
		/// </summary>
		public float minVertexDistance = .1f;
		/// <summary>
		/// The maximum distance before forcing new vertices.
		/// </summary>
		public float maxVertexDistance = 100f;
		/// <summary>
		/// The maximum forward path deviation before forcing new vertices.
		/// </summary>
		public float maxPathDeviation = 1f;
		/// <summary>
		/// Determines if points should be created upon particle collision.
		/// </summary>
		public bool createPointsOnCollision = false;
		/// <summary>
		/// The maximum available points able to be created by this Playground Trail. This will determine the generation of built-in arrays needed to remain efficient in memory consumption.
		/// The trail is made out of points where vertices are drawn in between, two points is the minimum to be able to draw a trail, this represents 4 vertices and 6 triangles.
		/// </summary>
		public int maxPoints = 100;
		/// <summary>
		/// Determines if first point should be created immediately on particle birth, otherwise this will be created during the trail calculation routine.
		/// This has affect on when the trail starts as the particle may have moved when the first point is created. If your particle source is moving you may want to leave this setting off to not create a first skewed trail point.
		/// </summary>
		public bool createFirstPointOnParticleBirth = false;
		/// <summary>
		/// Determines if a last point on the trail should be created when its assigned particle dies.
		/// </summary>
		public bool createLastPointOnParticleDeath = false;
		/// <summary>
		/// Determines if the Playground Trails should run asynchronously on a separate thread. This will go through the selected Thread Pool Method in the Playground Manager (PlaygroundC).
		/// </summary>
		public bool multithreading = true;

		/// <summary>
		/// The reference to the birth event on the assigned Particle Playground system.
		/// </summary>
		[HideInInspector] public PlaygroundEventC birthEvent;
		/// <summary>
		/// The reference to the death event on the assigned Particle Playground system.
		/// </summary>
		[HideInInspector] public PlaygroundEventC deathEvent;
		/// <summary>
		/// The reference to the collision event on the assigned Particle Playground system.
		/// </summary>
		[HideInInspector] public PlaygroundEventC collisionEvent;

		/// <summary>
		/// The list of trails following each particle.
		/// </summary>
		private List<ParticlePlaygroundTrail> _trails = new List<ParticlePlaygroundTrail>();

		private Transform _parentTransform;
		private GameObject _parentGameObject;
		private Material _materialCache;
		private float _calculationStartTime;
		private int _currentParticleCount;
		private float _currentParticleMinLifetime;
		private float _currentParticleMaxLifetime;
		private bool _localSpace;
		private Vector3 _billboardTransformPosition;
		private object _locker = new object();
		private bool _isDoneThread = true;
		private Matrix4x4 _localMatrix;

		/// <summary>
		/// The birth queue of trails. This will be added to whenever a particle births. As a Particle Playground system can birth particles and send particle events asynchronously a thread safe queue is needed to create the trails.
		/// </summary>
		readonly Queue<TrailParticleInfo> _birthQueue = new Queue<TrailParticleInfo>();


		/****************************************************************************
			Monobehaviours
		 ****************************************************************************/

		void OnEnable ()
		{
			// Cache reference to the Particle Playground system
			if (playgroundSystem == null)
				playgroundSystem = GetComponent<PlaygroundParticlesC>();

			// Cache a reference to the Main Camera if billboardTransform isn't assigned
			if (billboardTransform == null)
				billboardTransform = Camera.main.transform;

			// Set the initial material
			if (material == null)
			{
				material = new Material(Shader.Find("Playground/Vertex Color"));
				_materialCache = material;
			}

			// Reset the trails
			ResetTrails();

			// Add the required birth/death/collision events
			AddRequiredParticleEvents();
			
			// Setup default time width keys
			if (timeWidth == null)
				timeWidth = new AnimationCurve(DefaultWidthKeys());

			// Setup default point array alpha keys 
			if (pointArrayAlpha == null)
				pointArrayAlpha = new AnimationCurve(DefaultWidthKeys());

			_isDoneThread = true;
		}

		void OnDisable ()
		{
			// Destroy all trails
			DestroyAllTrails();
			
			// Remove the required events
			RemoveRequiredEvents();
		}

		void OnDestroy ()
		{
			// Destroy all trails
			DestroyAllTrails();

			// Remove the required events
			RemoveRequiredEvents();
		}

		void Update ()
		{
			// Clamp values
			maxPoints = Mathf.Clamp (maxPoints, 2, 32767);

			// Set asynchronous available values
			if (billboardTransform != null)
				_billboardTransformPosition = billboardTransform.position;

			// Early out if no particles exist yet
			if (playgroundSystem == null || !playgroundSystem.IsReady() || playgroundSystem.IsSettingParticleCount() || playgroundSystem.IsSettingLifetime() || playgroundSystem.particleCache == null || playgroundSystem.particleCache.Length == 0)
				return;

			// Reset trails if a crucial state is changed
			if (_currentParticleCount != playgroundSystem.particleCount || _currentParticleMinLifetime != playgroundSystem.lifetimeMin || _currentParticleMaxLifetime != playgroundSystem.lifetime || _localSpace != (playgroundSystem.shurikenParticleSystem.simulationSpace == ParticleSystemSimulationSpace.Local))
				ResetTrails();

			// Set calculation matrix if this is local space
			if (_localSpace)
				_localMatrix.SetTRS(playgroundSystem.particleSystemTransform.position, playgroundSystem.particleSystemTransform.rotation, playgroundSystem.particleSystemTransform.lossyScale);

			// Check material
			if (material != _materialCache)
				SetMaterial(material);

			// Remove any trails that has ended
			if (_isDoneThread)
			{
				for (int i = 0; i<_trails.Count; i++)
				{
					if (_trails[i].trailPoints != null && _trails[i].trailPoints.Count > 1 && _trails[i].trailPoints[_trails[i].trailPoints.Count-1] != null && _trails[i].CanRemoveTrail())
					{
						RemoveTrail(i);
						i--;
						if (i<0) i = 0;
						continue;
					}
				}
			}

			// Consume the particle birth queue
			while (_birthQueue.Count>0)
				AddTrail(_birthQueue.Dequeue());

			// Update all trail meshes and their render settings
			for (int i = 0; i<_trails.Count; i++)
			{
				ParticlePlaygroundTrail trail = _trails[i];
				// Set shadow casting/receiving
				trail.trailRenderer.receiveShadows = receiveShadows;
				#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
				trail.trailRenderer.castShadows = castShadows;
				#else
				trail.trailRenderer.shadowCastingMode = shadowCastingMode;
				#endif
				if (_isDoneThread)
					trail.UpdateMesh();
			}
			// Finally calculate all trails
			if (multithreading)
			{
				if (_isDoneThread)
				{
					_calculationStartTime = Application.isPlaying? Time.time : Time.realtimeSinceStartup;
					_isDoneThread = false;
					PlaygroundC.RunAsync(()=>{
						lock (_locker)
						{
							if (_isDoneThread) return;
							CalculateTrail();
							_isDoneThread = true;
						}
					});
				}
			} 
			else 
			{
				_calculationStartTime = Application.isPlaying? Time.time : Time.realtimeSinceStartup;
				CalculateTrail();
			}
		}

		// Prevent build-up of the birth queue while Editor is out of focus
		#if UNITY_EDITOR
		public void OnApplicationPause (bool pauseStatus) 
		{
			if (!pauseStatus && !UnityEditor.EditorApplication.isPlaying) 
			{
				_birthQueue.Clear();
			}
		}
		#endif

		/****************************************************************************
			Event Listeners
		 ****************************************************************************/

		/// <summary>
		/// This function will be called whenever a particle is birthed.
		/// </summary>
		/// <param name="particle">The birthed particle.</param>
		void OnParticleBirthEvent (PlaygroundEventParticle particle)
		{
			_birthQueue.Enqueue (new TrailParticleInfo(particle.particleId, particle.position, particle.velocity));
		}
		/// <summary>
		/// This function will be called whenever a particle has died.
		/// </summary>
		/// <param name="particle">The particle which died.</param>
		void OnParticleDeathEvent (PlaygroundEventParticle particle)
		{
			int trailIndex = GetOldestTrailWithParticleId(particle.particleId);
			if (trailIndex > -1)
			{
				if (createLastPointOnParticleDeath)
				{
					_trails[trailIndex].SetLastPoint(particle.position, particle.velocity, EvaluateWidth(0), time, _calculationStartTime);
				}
				else
				{
					_trails[trailIndex].SetParticlePosition(particle.position);
					_trails[trailIndex].Die();
				}
			}
		}
		/// <summary>
		/// This function will be called whenever a particle is colliding.
		/// </summary>
		/// <param name="particle">The collided particle.</param>
		void OnParticleCollisionEvent (PlaygroundEventParticle particle)
		{
			if (createPointsOnCollision)
			{
				int trailIndex = GetNewestTrailWithParticleId (particle.particleId);
				if (trailIndex < 0)
					return;
				ParticlePlaygroundTrail trailAtIndex = _trails[trailIndex];
				trailAtIndex.AddPoint (playgroundSystem.particleCache[particle.particleId].position, EvaluateWidth(0), time, _calculationStartTime);
			}
		}

		/// <summary>
		/// Gets the birth event this Playground Trail is listening to.
		/// </summary>
		/// <returns>The particle birth event.</returns>
		public PlaygroundEventC GetBirthEvent () {return birthEvent;}
		/// <summary>
		/// Gets the death event this Playground Trail is listening to.
		/// </summary>
		/// <returns>The particle death event.</returns>
		public PlaygroundEventC GetDeathEvent () {return deathEvent;}
		/// <summary>
		/// Gets the collision event this Playground Trail is listening to.
		/// </summary>
		/// <returns>The particle collision event.</returns>
		public PlaygroundEventC GetCollisionEvent () {return collisionEvent;}

		/// <summary>
		/// Adds the required particle events to track particles.
		/// </summary>
		public void AddRequiredParticleEvents ()
		{
			if (playgroundSystem != null)
			{
				// Hookup events
				birthEvent = GetEventFromType(EVENTTYPEC.Birth);
				if (birthEvent == null)
				{
					birthEvent = PlaygroundC.CreateEvent(playgroundSystem);
					birthEvent.broadcastType = EVENTBROADCASTC.EventListeners;
					birthEvent.eventType = EVENTTYPEC.Birth;
				}
				birthEvent.particleEvent += OnParticleBirthEvent;
				
				deathEvent = GetEventFromType(EVENTTYPEC.Death);
				if (deathEvent == null)
				{
					deathEvent = PlaygroundC.CreateEvent(playgroundSystem);
					deathEvent.broadcastType = EVENTBROADCASTC.EventListeners;
					deathEvent.eventType = EVENTTYPEC.Death;
				}
				deathEvent.particleEvent += OnParticleDeathEvent;
				
				collisionEvent = GetEventFromType(EVENTTYPEC.Collision);
				if (collisionEvent == null)
				{
					collisionEvent = PlaygroundC.CreateEvent(playgroundSystem);
					collisionEvent.broadcastType = EVENTBROADCASTC.EventListeners;
					collisionEvent.eventType = EVENTTYPEC.Collision;
				}
				collisionEvent.particleEvent += OnParticleCollisionEvent;
			}
		}

		/// <summary>
		/// Removes the required events to track particles.
		/// </summary>
		public void RemoveRequiredEvents ()
		{
			if (playgroundSystem != null)
			{
				if (birthEvent != null)
				{
					birthEvent.particleEvent -= OnParticleBirthEvent;
					birthEvent = null;
				}
				if (deathEvent != null)
				{
					deathEvent.particleEvent -= OnParticleDeathEvent;
					deathEvent = null;
				}
				if (collisionEvent != null)
				{
					collisionEvent.particleEvent -= OnParticleCollisionEvent;
					collisionEvent = null;
				}
			}
		}

		/// <summary>
		/// Gets the type of event based on the passed in EVETTTYPEC.
		/// </summary>
		/// <returns>The event of type specified.</returns>
		/// <param name="eventType">The event type.</param>
		public PlaygroundEventC GetEventFromType (EVENTTYPEC eventType)
		{
			for (int i = 0; i<playgroundSystem.events.Count; i++)
				if (playgroundSystem.events[i].eventType == eventType)
					return playgroundSystem.events[i];
			return null;
		}


		/****************************************************************************
			Misc functions
		 ****************************************************************************/

		/// <summary>
		/// Returns a default pair of AnimationCurve Keyframes in X 0 and X 1 at value Y 1.
		/// </summary>
		/// <returns>The default width keys.</returns>
		public Keyframe[] DefaultWidthKeys () {
			Keyframe[] keys = new Keyframe[2];
			keys[0].time = 0;
			keys[1].time = 1f;
			keys[0].value = 1f;
			keys[1].value = 1f;
			return keys;
		}

		/// <summary>
		/// Sets the material of all trails.
		/// </summary>
		/// <param name="material">The material all trails should get.</param>
		public void SetMaterial (Material material) {
			for (int i = 0; i<_trails.Count; i++) {
				if (_trails[i] != null && _trails[i].trailRenderer != null)
					_trails[i].trailRenderer.sharedMaterial = material;
			}
			_materialCache = material;
		}

		/// <summary>
		/// Evaluates the width at normalized trail time.
		/// </summary>
		/// <returns>The width at normalized trail time.</returns>
		/// <param name="normalizedTime">Normalized time.</param>
		public float EvaluateWidth (float normalizedTime) {
			return timeWidth.Evaluate(normalizedTime)*widthScale;
		}


		public Color32 EvaluateColor (float normalizedTime)
		{
			return lifetimeColor.Evaluate(normalizedTime);
		}

		public Color32 EvaluateColor (int trailIndex, int trailPointIndex)
		{
			return lifetimeColor.Evaluate((trailPointIndex*1f) / (_trails[trailIndex].GetBirthIterator()-1));
		}


		/****************************************************************************
			Trail functions
		 ****************************************************************************/

		/// <summary>
		/// Creates a trail and assigns it to a particle.
		/// </summary>
		/// <param name="particleInfo">Information about the particle.</param>
		public void AddTrail (TrailParticleInfo particleInfo)
		{
			// Check parent object
			if (_parentGameObject == null)
			{
				_parentGameObject = new GameObject("Playground Trails ("+playgroundSystem.name+")", typeof(PlaygroundTrailParent));
				_parentTransform = _parentGameObject.transform;
				_parentGameObject.GetComponent<PlaygroundTrailParent>().trailsReference = this;
			}

			ParticlePlaygroundTrail newTrail = new ParticlePlaygroundTrail(maxPoints);
			newTrail.trailGameObject = new GameObject("Playground Trail "+particleInfo.particleId);
			newTrail.trailTransform = newTrail.trailGameObject.transform;
			newTrail.trailTransform.parent = _parentTransform;
			newTrail.trailRenderer = newTrail.trailGameObject.AddComponent<MeshRenderer>();
			newTrail.trailMeshFilter = newTrail.trailGameObject.AddComponent<MeshFilter>();
			newTrail.trailMesh = new Mesh();
			newTrail.trailMesh.MarkDynamic();
			newTrail.trailMeshFilter.sharedMesh = newTrail.trailMesh;
			newTrail.trailRenderer.sharedMaterial = material;
			
			newTrail.particleId = particleInfo.particleId;

			if (createFirstPointOnParticleBirth)
				newTrail.SetFirstPoint(particleInfo.position, particleInfo.velocity, EvaluateWidth(0), time, _calculationStartTime);

			_trails.Add (newTrail);
		}

		/// <summary>
		/// Gets the oldest trail following the particle id. If the trail is already dead or doesn't contain the particle id -1 will be returned.
		/// </summary>
		/// <returns>The trail with particle id (-1 if not found).</returns>
		/// <param name="particleId">Particle identifier.</param>
		public int GetOldestTrailWithParticleId (int particleId)
		{
			for (int i = 0; i<_trails.Count; i++)
				if (_trails[i].particleId == particleId && !_trails[i].IsDead())
					return i;
			return -1;
		}

		/// <summary>
		/// Gets the newest trail following the particle id. If the trail is already dead or doesn't contain the particle id -1 will be returned.
		/// </summary>
		/// <returns>The trail with particle id (-1 if not found).</returns>
		/// <param name="particleId">Particle identifier.</param>
		public int GetNewestTrailWithParticleId (int particleId)
		{
			for (int i = _trails.Count-1; i>=0; --i)
				if (_trails[i].particleId == particleId && !_trails[i].IsDead())
					return i;
			return -1;
		}

		/// <summary>
		/// Gets the cached parent transform of the trails.
		/// </summary>
		/// <returns>The parent transform.</returns>
		public Transform GetParentTransform ()
		{
			return _parentTransform;
		}

		/// <summary>
		/// Gets the cached parent game object of the trails.
		/// </summary>
		/// <returns>The parent game object.</returns>
		public GameObject GetParentGameObject ()
		{
			return _parentGameObject;
		}

		/// <summary>
		/// Stopping the trail will make the trail stop following its assigned particle.
		/// </summary>
		/// <param name="trailNumber">Trail number.</param>
		public void StopTrail (int trailNumber) 
		{
			if (trailNumber < 0)
			{
				return;
			}
			_trails[trailNumber].Die();
		}

		/// <summary>
		/// Stops the oldest trail with particle identifier.
		/// </summary>
		/// <param name="particleId">Particle identifier.</param>
		public void StopOldestTrailWithParticleId (int particleId)
		{
			StopTrail (GetOldestTrailWithParticleId (particleId));
		}

		/// <summary>
		/// Stops the newest trail with particle identifier.
		/// </summary>
		/// <param name="particleId">Particle identifier.</param>
		public void StopNewestTrailWithParticleId (int particleId)
		{
			StopTrail (GetNewestTrailWithParticleId (particleId));
		}

		/// <summary>
		/// Resets all trails.
		/// </summary>
		public void ResetTrails () {
			DestroyAllTrails();
			if (playgroundSystem != null && gameObject.activeInHierarchy)
			{
				_currentParticleCount = playgroundSystem.particleCount;
				_currentParticleMinLifetime = playgroundSystem.lifetimeMin;
				_currentParticleMaxLifetime = playgroundSystem.lifetime;
				_localSpace = playgroundSystem.shurikenParticleSystem.simulationSpace == ParticleSystemSimulationSpace.Local;
			}

			_isDoneThread = true;
		}

		/// <summary>
		/// Removes the trail at index.
		/// </summary>
		/// <param name="index">The trail index.</param>
		public void RemoveTrail (int index) {
			if (Application.isPlaying)
				Destroy(_trails[index].trailGameObject);
			else
				DestroyImmediate(_trails[index].trailGameObject);

			_trails.RemoveAt(index);
		}

		/// <summary>
		/// Destroys all trails and clears out trail list.
		/// </summary>
		public void DestroyAllTrails () {

			foreach (ParticlePlaygroundTrail trail in _trails)
			{
				if (Application.isPlaying)
					Destroy(trail.trailGameObject);
				else
					DestroyImmediate(trail.trailGameObject);
			}

			if (_parentGameObject != null)
			{
				if (Application.isPlaying)
					Destroy (_parentGameObject);
				else
					DestroyImmediate(_parentGameObject);
			}

			_trails.Clear();
			_birthQueue.Clear();
		}


		/****************************************************************************
			Internal
		 ****************************************************************************/
		
		void CalculateTrail ()
		{
			// Iterate through all trails
			for (int i = 0; i<_trails.Count; i++)
			{
				ParticlePlaygroundTrail trail = _trails[i];

				// Skip this trail if it's prepared to be removed
				if (trail.CanRemoveTrail())
					continue;

				if (trail.particleId >= 0 && !trail.IsDead())
				{
					if (trail.GetBirthIterator()>0)
					{
						// New point creation
						float pointDistance = Vector3.Distance(trail.GetParticlePosition(), trail.GetLastAddedPointPosition());
						if (pointDistance>minVertexDistance) {
							float pathDeviationAngle = trail.GetPathDeviation();
							if (pointDistance>maxVertexDistance || pathDeviationAngle>maxPathDeviation) {
								trail.AddPoint(playgroundSystem.particleCache[trail.particleId].position, EvaluateWidth(0), time, _calculationStartTime);
							}
						}
					}
					else
					{
						// First point creation
						trail.SetFirstPoint(playgroundSystem.particleCache[trail.particleId].position, playgroundSystem.particleCache[trail.particleId].velocity, EvaluateWidth(0), time, _calculationStartTime);
					}

					// Set the particle position info
					trail.SetParticlePosition(playgroundSystem.particleCache[trail.particleId].position);
				}

				// Update the trail points
				for (int x = 0; x<trail.trailPoints.Count; x++)
				{
					TrailPoint trailPoint = trail.trailPoints[x];

					if (trailPoint.CanRemove())
					{
						trail.RemovePoint(x);

						if (!_localSpace)
							continue;
					}

					float normalizedLifetime = trailPoint.GetNormalizedLifetime();

					// Update trail points data
					trailPoint.Update (
						_calculationStartTime,
						EvaluateWidth(normalizedLifetime)
					);

					// Set end point to follow particle
					if (!trail.IsDead() && x==trail.trailPoints.Count-1)
						trailPoint.position = trail.GetParticlePosition();
					
					// Rotation of trail points
					Vector3 currentPosition = trailPoint.position;
					Vector3 nextPosition = x<trail.trailPoints.Count-1? trail.trailPoints[x+1].position : currentPosition + (currentPosition - trail.trailPoints[x-1].position);

					Vector3 lookDirection = Vector3.up;
					switch (renderMode)
					{
						case TrailRenderMode.Vertical: 
							lookDirection = Vector3.forward; 
						break;
						case TrailRenderMode.Billboard: 
							lookDirection = (_billboardTransformPosition - currentPosition).normalized; 
						break;
					}

					// If this is local space then recompute current & next position based on the local matrix
					if (_localSpace)
					{
						currentPosition = _localMatrix.MultiplyPoint3x4(currentPosition);
						nextPosition = _localMatrix.MultiplyPoint3x4(nextPosition);
					}

					Vector3 dir = renderMode != TrailRenderMode.CustomRenderScale? (Vector3.Cross(lookDirection, nextPosition - currentPosition)).normalized : customRenderScale;
					Vector3 lPoint = currentPosition + (dir * (trailPoint.width*.5f));
					Vector3 rPoint = currentPosition - (dir * (trailPoint.width*.5f));

					// Set mesh vertices into the rotated position
					trail.meshVerticesCache[x*2] = lPoint;
					trail.meshVerticesCache[(x*2)+1] = rPoint;

					// Set uv
					float uvRatio = uvMode == TrailUvMode.Lifetime? normalizedLifetime : (x*1f) / (trail.GetBirthIterator()-1);
					trail.meshUvsCache[x*2] = new Vector2(uvRatio, 0);
					trail.meshUvsCache[(x*2)+1] = new Vector2(uvRatio, 1f);

					// Update colors
					if (colorMode == TrailColorMode.Lifetime)
					{
						Color32 color = EvaluateColor(normalizedLifetime);
						color.a = (byte)(color.a*(pointArrayAlpha.Evaluate((x*1f) / (trail.GetBirthIterator()-1))));
						trail.SetColor(x, color);
					}
					else
						trail.SetColor(x, EvaluateColor(i, x));
				}
			}
		}
	}

	/// <summary>
	/// The trail render mode determines how the trail will be rotated. 
	/// Using billboard will rotate towards the assigned transform position, this is by default the main camera.
	/// Horizontal will rotate the points flat on X-Z axis.
	/// Vertical will rotate the points flat on X-Y axis.
	/// CustomRenderScale is a global world space normal which will multiply the scale on each axis. 
	/// </summary>
	public enum TrailRenderMode 
	{
		/// <summary>
		/// Rotate points towards assigned billboard transform.
		/// </summary>
		Billboard,
		/// <summary>
		/// Rotate points flat X-Z.
		/// </summary>
		Horizontal,
		/// <summary>
		/// Rotate points flat X-Y.
		/// </summary>
		Vertical,
		/// <summary>
		/// Creates a custom render rotation/scale.
		/// </summary>
		CustomRenderScale
	}

	/// <summary>
	/// The trail color mode determines how color should be distributed over a trail.
	/// </summary>
	public enum TrailColorMode
	{
		/// <summary>
		/// When using TrailColorMode.Lifetime the colors will be set depending on each point's normalized lifetime.
		/// </summary>
		Lifetime,
		/// <summary>
		/// When using TrailColorMode.PointArray the colors will be set depending on all the points within the trail, where each point is a normalized value linearly towards the total points.
		/// </summary>
		PointArray
	}

	/// <summary>
	/// The trail uv mode determines how uv will be distributed over a trail.
	/// </summary>
	public enum TrailUvMode
	{
		/// <summary>
		/// When using TrailUvMode.Lifetime the uvs will be set depending on each point's normalized lifetime.
		/// </summary>
		Lifetime,
		/// <summary>
		/// When using TrailUvMode.PointArray the uvs will be set depending on all the points within the trail, where each point is a normalized value linearly towards the total points.
		/// </summary>
		PointArray
	}

	/// <summary>
	/// The trail particle info struct contains data about particles to be read by a Playground Trail.
	/// </summary>
	public struct TrailParticleInfo {
		/// <summary>
		/// The particle identifier linearly towards the particle system's cached particles.
		/// </summary>
		public int particleId;
		/// <summary>
		/// The position of this trail particle.
		/// </summary>
		public Vector3 position;
		/// <summary>
		/// The velocity of this trail particle.
		/// </summary>
		public Vector3 velocity;

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticlePlayground.TrailParticleInfo"/> struct.
		/// </summary>
		/// <param name="particleId">Particle identifier.</param>
		/// <param name="position">Particle position.</param>
		/// <param name="velocity">Particle velocity.</param>
		public TrailParticleInfo (int particleId, Vector3 position, Vector3 velocity)
		{
			this.particleId = particleId;
			this.position = position;
			this.velocity = velocity;
		}
	}
}