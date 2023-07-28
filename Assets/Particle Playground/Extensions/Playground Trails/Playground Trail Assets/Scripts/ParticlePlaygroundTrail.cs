using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ParticlePlayground {

	[Serializable]
	public class ParticlePlaygroundTrail 
	{
		/// <summary>
		/// Determines if this ParticlePlaygroundTrail should update.
		/// </summary>
		[HideInInspector] public bool update = true;
		/// <summary>
		/// The GameObject of this trail.
		/// </summary>
		[HideInInspector] public GameObject trailGameObject;
		/// <summary>
		/// The Transform of this trail.
		/// </summary>
		[HideInInspector] public Transform trailTransform;
		/// <summary>
		/// The Mesh Renderer component of this trail.
		/// </summary>
		[HideInInspector] public MeshRenderer trailRenderer;
		/// <summary>
		/// The Mesh Filter component of this trail.
		/// </summary>
		[HideInInspector] public MeshFilter trailMeshFilter;
		/// <summary>
		/// The Mesh component of this trail.
		/// </summary>
		[HideInInspector] public Mesh trailMesh;
		/// <summary>
		/// The particle this trail is following.
		/// </summary>
		[HideInInspector] public int particleId;

		/// <summary>
		/// The minimum point cache limit.
		/// </summary>
		[HideInInspector] public int minPointCache = 2;
		/// <summary>
		/// The maximum point cache limit.
		/// </summary>
		[HideInInspector] public int maxPointCache = 32767;
		
		[NonSerialized] public List<TrailPoint> trailPoints = new List<TrailPoint>();
		[NonSerialized] public Vector3[] meshVerticesCache;
		[NonSerialized] public Vector3[] meshNormalsCache;
		[NonSerialized] public Vector2[] meshUvsCache;
		[NonSerialized] public int[] meshTrianglesCache;
		[NonSerialized] public Color32[] meshColorsCache;
		
		private int _pointCache = 200;
		private int _birthIterator;
		private int _deathIterator;
		private float _particleTime;
		private Vector3 _particlePosition;
		private Vector3 _previousParticlePosition;
		private Vector3 _particleDirection;
		private Vector3 _lastAddedPointPosition;
		private Vector3 _lastAddedPointDirection;
		private bool _isDead = false;
		private bool _isReady = false;
		
		private float _timeCached;

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticlePlayground.ParticlePlaygroundTrail"/> class.
		/// </summary>
		public ParticlePlaygroundTrail ()
		{
			UpdateCache();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParticlePlayground.ParticlePlaygroundTrail"/> class and specifies the available point cache. 
		/// The mesh's available vertices, normals, uvs, triangles and colors will be based on the specified point cache.
		/// </summary>
		/// <param name="pointCache">Mesh point cache (vertices, normals and uvs will be multiplied by two, triangles by six).</param>
		public ParticlePlaygroundTrail (int pointCache)
		{
			_pointCache = Mathf.Clamp (pointCache, minPointCache, maxPointCache);
			UpdateCache();
		}

		/// <summary>
		/// Updates the mesh cache. This is done upon creation where cache lengths will be based on the point cache.
		/// </summary>
		public void UpdateCache ()
		{
			_timeCached = PlaygroundC.globalTime;
			meshVerticesCache = new Vector3[_pointCache*2];
			meshNormalsCache = new Vector3[_pointCache*2];
			meshUvsCache = new Vector2[_pointCache*2];
			meshTrianglesCache = new int[(_pointCache-1)*6];
			meshColorsCache = new Color32[_pointCache*2];
			_isReady = true;
		}
		
		/// <summary>
		/// Updates the trail mesh. This is done each frame from the main-thread.
		/// </summary>
		public void UpdateMesh () 
		{
			trailMesh.Clear();
			trailMesh.vertices = meshVerticesCache;
			trailMesh.uv = meshUvsCache;
			trailMesh.triangles = meshTrianglesCache;
			trailMesh.colors32 = meshColorsCache;
			trailMesh.normals = meshNormalsCache;
		}

		/// <summary>
		/// Recalculates the bounds on the trail mesh. This is done automatically when triangles are set into a mesh.
		/// </summary>
		public void RecalculateBounds ()
		{
			trailMesh.RecalculateBounds();
		}
		
		/// <summary>
		/// Clears the trail mesh.
		/// </summary>
		public void ClearMesh () 
		{
			_birthIterator = 0;
			_deathIterator = 0;
			meshVerticesCache = new Vector3[0];
			meshNormalsCache = new Vector3[0];
			meshUvsCache = new Vector2[0];
			meshTrianglesCache = new int[0];
			meshColorsCache = new Color32[0];
		}
		
		/// <summary>
		/// Sets the first point in the trail. This will add two initial points, one starting point and one ending point which will follow the assigned particle.
		/// </summary>
		/// <param name="pos">Position.</param>
		/// <param name="dir">Direction.</param>
		/// <param name="startWidth">Start width.</param>
		/// <param name="lifetime">Lifetime.</param>
		public void SetFirstPoint (Vector3 pos, Vector3 dir, float startWidth, float lifetime, float creationTime) 
		{
			_particleDirection = dir;
			AddPoint (pos, startWidth, lifetime, creationTime);
			AddPoint (pos, startWidth, lifetime, creationTime);
			_birthIterator = 2;
			_deathIterator = 0;
		}

		/// <summary>
		/// Sets the last point in the trail and then kills it. This will add one last point which won't follow the assigned particle.
		/// </summary>
		/// <param name="pos">Position.</param>
		/// <param name="dir">Direction.</param>
		/// <param name="startWidth">Start width.</param>
		/// <param name="lifetime">Lifetime.</param>
		public void SetLastPoint (Vector3 pos, Vector3 dir, float startWidth, float lifetime, float creationTime)
		{
			_particleDirection = dir;
			AddPoint (pos, startWidth, lifetime, creationTime);
			Die ();
		}

		/// <summary>
		/// Adds a point into the end of the trail.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="width">Width.</param>
		/// <param name="lifetime">Lifetime.</param>
		public void AddPoint (Vector3 position, float width, float lifetime, float creationTime)
		{
			if (_birthIterator >= _pointCache || _isDead)
				return;
			trailPoints.Add (new TrailPoint(position, lifetime, width, creationTime));
			AddPoint(position);
		}

		/// <summary>
		/// Adds a point into the end of the trail.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="velocity">Initial Velocity.</param>
		/// <param name="width">Width.</param>
		/// <param name="lifetime">Lifetime.</param>
		public void AddPoint (Vector3 position, Vector3 velocity, float width, float lifetime, float creationTime)
		{
			if (_birthIterator >= _pointCache || _isDead)
				return;
			trailPoints.Add (new TrailPoint(position, velocity, lifetime, width, creationTime));
			AddPoint(position);
		}


		void AddPoint (Vector3 position) 
		{
			_lastAddedPointDirection = position-_previousParticlePosition;
			_lastAddedPointPosition = position;

			meshVerticesCache[_birthIterator*2] = position;
			meshVerticesCache[(_birthIterator*2)+1] = position;
			
			meshNormalsCache[_birthIterator*2] = -Vector3.forward;
			meshNormalsCache[(_birthIterator*2)+1] = -Vector3.forward;
			
			meshUvsCache[_birthIterator*2] = new Vector2(1f, 0f);
			meshUvsCache[(_birthIterator*2)+1] = new Vector2(0f, 1f);
			
			meshColorsCache[_birthIterator*2] = new Color32();
			meshColorsCache[(_birthIterator*2)+1] = new Color32();
			
			
			if (trailPoints.Count>1) {
				int vertexIndex = (_birthIterator)*2;
				int triIndex = (_birthIterator-1)*6;

				meshTrianglesCache[triIndex] = vertexIndex -2;
				meshTrianglesCache[triIndex+1] = vertexIndex -1;
				meshTrianglesCache[triIndex+2] = vertexIndex;
				meshTrianglesCache[triIndex+3] = vertexIndex;
				meshTrianglesCache[triIndex+4] = vertexIndex -1;
				meshTrianglesCache[triIndex+5] = vertexIndex +1;
			}
			
			NextPoint();
		}

		public void RemovePoint (int index)
		{
			if (_deathIterator < _birthIterator-2)
			{
				meshVerticesCache[_deathIterator*2] = meshVerticesCache[(_deathIterator*2)+2];
				meshVerticesCache[(_deathIterator*2)+1] = meshVerticesCache[(_deathIterator*2)+3];
				meshColorsCache[_deathIterator*2] = new Color32();
				meshColorsCache[(_deathIterator*2)+1] = new Color32();

				_deathIterator++;
			}
		}

		/// <summary>
		/// Sets the color at specified index. This is done automatically through the trail calculation loop.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="color">Color.</param>
		public void SetColor (int index, Color32 color)
		{
			meshColorsCache[index*2] = color;
			meshColorsCache[(index*2)+1] = color;
		}
		
		/// <summary>
		/// Makes the iterator jump to next point. The iterator controls which point is following the particle.
		/// </summary>
		public void NextPoint () 
		{
			_birthIterator++;
		}
		
		/// <summary>
		/// Gets the paired particle's current time.
		/// </summary>
		/// <returns>The paired particle's current time.</returns>
		public float GetParticleTime () 
		{
			return _particleTime;
		}
		
		/// <summary>
		/// Sets the paired particle's current time.
		/// </summary>
		/// <param name="time">Time.</param>
		public void SetParticleTime (float time) 
		{
			_particleTime = time;
		}
		
		/// <summary>
		/// Gets the paired particle's current position.
		/// </summary>
		/// <returns>The paired particle's position.</returns>
		public Vector3 GetParticlePosition () 
		{
			return _particlePosition;
		}
		
		/// <summary>
		/// Sets the paired particle's position.
		/// </summary>
		/// <param name="position">Position.</param>
		public void SetParticlePosition (Vector3 position) 
		{
			if (_isDead)
				return;
			_previousParticlePosition = _particlePosition;
			_particlePosition = position;
			_particleDirection = (position-_previousParticlePosition).normalized;
		}

		/// <summary>
		/// Sets the paired particle's direction.
		/// </summary>
		/// <param name="direction">Direction.</param>
		public void SetParticleDirection (Vector3 direction)
		{
			_particleDirection = direction;
		}
		
		/// <summary>
		/// Gets the last added point position.
		/// </summary>
		/// <returns>The last added point position.</returns>
		public Vector3 GetLastAddedPointPosition () 
		{
			return _lastAddedPointPosition;
		}

		/// <summary>
		/// Gets the direction of the particle.
		/// </summary>
		/// <returns>The particle direction.</returns>
		public Vector3 GetParticleDirection () 
		{
			return _particleDirection;
		}

		public Vector3 GetLastAddedPointDirection ()
		{
			return _lastAddedPointDirection;
		}

		/// <summary>
		/// Gets the current path deviation angle based on the last added point direction and the current direction of the particle.
		/// </summary>
		/// <returns>The path deviation angle.</returns>
		public float GetPathDeviation () {
			return Vector3.Angle(_lastAddedPointDirection, _particleDirection);
		}

		/// <summary>
		/// Gets the birth iterator used for iterating through the mesh arrays to set new vertex positions.
		/// </summary>
		/// <returns>The birth iterator.</returns>
		public int GetBirthIterator ()
		{
			return _birthIterator;
		}

		/// <summary>
		/// Gets the death iterator used for iterating through the mesh arrays to remove old vertex positions.
		/// </summary>
		/// <returns>The death iterator.</returns>
		public int GetDeathIterator ()
		{
			return _deathIterator;
		}

		/// <summary>
		/// Gets the point cache amount. The point cache is set at the trail's creation and determines how many vertices, triangles, uvs, normals and colors the trail mesh can have.
		/// </summary>
		/// <returns>The point cache amount.</returns>
		public int GetPointCache ()
		{
			return _pointCache;
		}

		/// <summary>
		/// Gets the time this trail was cached.
		/// </summary>
		/// <returns>The cached time.</returns>
		public float TimeCached ()
		{
			return _timeCached;
		}

		/// <summary>
		/// Determines whether this trail can remove a point at specified index.
		/// </summary>
		/// <returns><c>true</c> if this trail can remove a point at the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="point">Point.</param>
		public bool CanRemovePoint (int point) 
		{
			return trailPoints[point].CanRemove();
		}

		/// <summary>
		/// Determines whether this trail can be removed. This will check if the last point in the trail has reached the end of its lifetime.
		/// </summary>
		/// <returns><c>true</c> if this trail can be removed; otherwise, <c>false</c>.</returns>
		public bool CanRemoveTrail ()
		{
			if (trailPoints.Count == 0 || !_isReady)
				return false;
			return trailPoints[trailPoints.Count-1].CanRemove();
		}

		/// <summary>
		/// Wakes up the trail from being dead.
		/// </summary>
		public void WakeUp () 
		{
			_isDead = false;
		}

		/// <summary>
		/// Makes the trail stop following its assigned particle.
		/// </summary>
		public void Die () 
		{
			_isDead = true;
		}

		/// <summary>
		/// Determines whether this trail is dead.
		/// </summary>
		/// <returns><c>true</c> if this trail is dead; otherwise, <c>false</c>.</returns>
		public bool IsDead () 
		{
			return _isDead;
		}
	}
}