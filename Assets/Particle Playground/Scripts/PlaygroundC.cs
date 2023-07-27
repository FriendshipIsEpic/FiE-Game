using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlaygroundSplines;
using System.Threading;

namespace ParticlePlayground {
	/// <summary>
	/// The PlaygroundC class is the Playground Manager which runs all Particle Playground systems and is containing all Global Manipulators. 
	/// The Playground Manager acts as a wrapper class for common operations and contain functions for creating and altering particle systems. 
	/// You will also find the global event delegates (particleEventBirth, particleEventDeath, particleEventCollision and particleEventTime) for any particle systems broadcasting events with "Send To Manager" enabled.
	/// </summary>
	[ExecuteInEditMode()]
	public class PlaygroundC : MonoBehaviour {
		
		/*************************************************************************************************************************************************
			Playground counters
		*************************************************************************************************************************************************/
		
		public static int meshQuantity;
		public static int particlesQuantity;
		public static float version = 3.03f;
		public static string specialVersion = " ";
		
		
		/*************************************************************************************************************************************************
			Playground settings
		*************************************************************************************************************************************************/
		
		// Time variables
		/// <summary>
		/// The global time.
		/// </summary>
		public static float globalTime;
		/// <summary>
		/// Time when globalTime last updated.
		/// </summary>
		public static float lastTimeUpdated;
		/// <summary>
		/// Delta time for globalTime (globalTime-lastTimeUpdated).
		/// </summary>
		public static float globalDeltaTime;
		/// <summary>
		/// Scaling of globalTime.
		/// </summary>
		public static float globalTimescale = 1.0f;											
		
		// Misc settings
		/// <summary>
		/// Initial spawn position when particle is not set to rebirth.
		/// </summary>
		public static Vector3 initialTargetPosition = new Vector3(0,-999.99f,0);
		/// <summary>
		/// Update rate for finding vertices in skinned meshes (1 = Every frame, 2 = Every second frame...).
		/// </summary>
		public static int skinnedUpdateRate = 1;											
		/// <summary>
		/// Let a PlaygroundParticleWindow repaint the scene.
		/// </summary>
		public static bool triggerSceneRepaint = true;										
		/// <summary>
		/// Minimum velocity of a particle before it goes to rest on collision.
		/// </summary>
		public static float collisionSleepVelocity = .01f;
		/// <summary>
		/// Determines how many frames are unsafe before initiating automatic thread bundling.
		/// </summary>
		public static int unsafeAutomaticThreadFrames = 20;
		/// <summary>
		/// The frustum planes of the Main Camera. This is used to disable or enable calculation on particle systems when having PlaygroundParticlesC.pauseCalculationWhenInvisible enabled.
		/// </summary>
		public static Plane[] frustumPlanes;
		
		
		/*************************************************************************************************************************************************
			Playground global event delegates (receives event particles sent from a particle system)
		*************************************************************************************************************************************************/
		
		/// <summary>
		/// The event of a particle birthing. This require that you are using Event Listeners and set your desired particle system(s) to broadcast the event by enabling PlaygroundEventC.sendToManager.
		/// </summary>
		public static event OnPlaygroundParticle particleEventBirth;
		static bool particleEventBirthInitialized = false;
		
		/// <summary>
		/// The event of a particle dying. This require that you are using Event Listeners and set your desired particle system(s) to broadcast the event by enabling PlaygroundEventC.sendToManager.
		/// </summary>
		public static event OnPlaygroundParticle particleEventDeath;
		static bool particleEventDeathInitialized = false;
		
		
		/// <summary>
		/// The event of a particle colliding. This require that you are using Event Listeners and set your desired particle system(s) to broadcast the event by enabling PlaygroundEventC.sendToManager.
		/// </summary>
		public static event OnPlaygroundParticle particleEventCollision;
		static bool particleEventCollisionInitialized = false;
		
		
		/// <summary>
		/// The event of a particle sent by timer. This require that you are using Event Listeners and set your desired particle system(s) to broadcast the event by enabling PlaygroundEventC.sendToManager.
		/// </summary>
		public static event OnPlaygroundParticle particleEventTime;
		static bool particleEventTimeInitialized = false;
		
		/// <summary>
		/// Sends the particle event birth.
		/// </summary>
		/// <param name="eventParticle">Event particle.</param>
		public static void SendParticleEventBirth (PlaygroundEventParticle eventParticle) {
			if (particleEventBirthInitialized)
				particleEventBirth(eventParticle);
		}
		
		/// <summary>
		/// Sends the particle event death.
		/// </summary>
		/// <param name="eventParticle">Event particle.</param>
		public static void SendParticleEventDeath (PlaygroundEventParticle eventParticle) {
			if (particleEventDeathInitialized)
				particleEventDeath(eventParticle);
		}
		
		/// <summary>
		/// Sends the particle event collision.
		/// </summary>
		/// <param name="eventParticle">Event particle.</param>
		public static void SendParticleEventCollision (PlaygroundEventParticle eventParticle) {
			if (particleEventCollisionInitialized)
				particleEventCollision(eventParticle);
		}
		
		/// <summary>
		/// Sends the particle event time.
		/// </summary>
		/// <param name="eventParticle">Event particle.</param>
		public static void SendParticleEventTime (PlaygroundEventParticle eventParticle) {
			if (particleEventTimeInitialized)
				particleEventTime(eventParticle);
		}
		
		
		/*************************************************************************************************************************************************
			Playground cache
		*************************************************************************************************************************************************/
		
		/// <summary>
		/// Static reference to the Playground Manager script.
		/// </summary>
		public static PlaygroundC reference;
		/// <summary>
		/// Static reference to the Playground Manager Transform.
		/// </summary>
		public static Transform referenceTransform;
		/// <summary>
		/// Static reference to the Playground Manager GameObject.
		/// </summary>
		public static GameObject referenceGameObject;
		
		/*************************************************************************************************************************************************
			Playground public variables
		*************************************************************************************************************************************************/
		
		/// <summary>
		/// The particle systems controlled by this Playground Manager.
		/// </summary>
		[SerializeField]
		public List<PlaygroundParticlesC> particleSystems = new List<PlaygroundParticlesC>();
		/// <summary>
		/// The manipulators controlled by this Playground Manager.
		/// </summary>
		[SerializeField]
		public List<ManipulatorObjectC> manipulators = new List<ManipulatorObjectC>();		
		/// <summary>
		/// Maximum amount of emission positions in a PaintObject.
		/// </summary>
		[HideInInspector] public int paintMaxPositions = 10000;								
		/// <summary>
		/// Calculate forces on PlaygroundParticlesC objects.
		/// </summary>
		[HideInInspector] public bool calculate = true;										
		/// <summary>
		/// Color filtering mode.
		/// </summary>
		[HideInInspector] public PIXELMODEC pixelFilterMode = PIXELMODEC.Pixel32;			
		/// <summary>
		/// Automatically parent a PlaygroundParticlesC object to Playground if it has no parent.
		/// </summary>
		[HideInInspector] public bool autoGroup = true;										
		/// <summary>
		/// Turn this on if you want to build particles from 0 alpha pixels into states.
		/// </summary>
		[HideInInspector] public  bool buildZeroAlphaPixels = false;						
		/// <summary>
		/// Draw gizmos for manipulators and other particle system helpers in Scene View.
		/// </summary>
		[HideInInspector] public bool drawGizmos = true;									
		/// <summary>
		/// Draw gizmos for source positions in Scene View.
		/// </summary>
		[HideInInspector] public bool drawSourcePositions = false;							
		/// <summary>
		/// Should the Shuriken particle system component be visible? (This is just a precaution as editing Shuriken can lead to unexpected behavior).
		/// </summary>
		[HideInInspector] public bool showShuriken = false;									
		/// <summary>
		/// Show toolbox in Scene View when Source is set to Paint on PlaygroundParticlesC objects.
		/// </summary>
		[HideInInspector] public bool paintToolbox = true;
		/// <summary>
		/// Scale of collision planes.
		/// </summary>
		[HideInInspector] public float collisionPlaneScale = .1f;
		/// <summary>
		/// Should snapshots be visible in the Hieararchy? (Enable this if you want to edit snapshots).
		/// </summary>
		[HideInInspector] public bool showSnapshotsInHierarchy = false;						
		/// <summary>
		/// Should wireframes be rendered around particles in Scene View?
		/// </summary>
		[HideInInspector] public bool drawWireframe = false;
		/// <summary>
		/// Determines to draw previews of Playground Splines in Scene View.
		/// </summary>
		[HideInInspector] public bool drawSplinePreview = true;
		/// <summary>
		/// Determines if Time.timeScale should affect the particle systems simulation time. If disabled you can keep simulating particles in normal speed detached from the scene's time scale.
		/// </summary>
		[HideInInspector] public bool globalTimeScale = true;
		
		/// <summary>
		/// The thread pool method determines which thread pooling should be used.
		/// ThreadPoolMethod.ThreadPool will use the .NET managed ThreadPool class (standard in Particle Playground in versions prior to 3).
		/// ThreadPoolMethod.PlaygroundPool will use the self-managed PlaygroundQueue which queues tasks on reused threads (standard in Particle Playground 3 and up).
		/// </summary>
		[HideInInspector] public ThreadPoolMethod threadPoolMethod = ThreadPoolMethod.PlaygroundPool;
		/// <summary>
		/// The multithreading method Particle Playground should use. This determines how particle systems calculate over the CPU. Keep in mind each thread will generate memory garbage which will be collected at some point.
		/// Selecting ThreadMethod.NoThreads will make particle systems calculate on the main-thread.
		/// ThreadMethod.OnePerSystem will create one thread per particle system each frame.
		/// ThreadMethod.OneForAll will bundle all calculations into one single thread.
		/// ThreadMethod.Automatic will distribute all particle systems evenly bundled along available CPUs/cores. This is the recommended setting for most user cases.
		/// </summary>
		[HideInInspector] public ThreadMethod threadMethod = ThreadMethod.Automatic;
		[HideInInspector] public ThreadMethodComponent skinnedMeshThreadMethod = ThreadMethodComponent.InsideParticleCalculation;
		[HideInInspector] public ThreadMethodComponent turbulenceThreadMethod = ThreadMethodComponent.InsideParticleCalculation;
		/// <summary>
		/// The maximum amount of threads that can be created. The amount of created threads will never exceed available CPUs.
		/// </summary>
		[HideInInspector] public int maxThreads = 8;
		
		#if UNITY_WSA && !UNITY_EDITOR
		#else
		/// <summary>
		/// The Particle Playground-managed thread pool. The maxThreads value will determine how many threads will be available in the pool.
		/// </summary>
		public static PlaygroundQueue<Action> playgroundPool;
		#endif
		
		bool isDoneThread = true;
		bool isDoneThreadLocal = true;
		bool isDoneThreadSkinned = true;
		bool isDoneThreadTurbulence = true;
		int threads;
		int _prevMaxThreadCount;
		static int processorCount;
		static int activeThreads = 0;
		static bool hasReference = false;
		static bool hasPlaygroundPool = false;
		
		
		/*************************************************************************************************************************************************
			Playground private variables
		*************************************************************************************************************************************************/
		
		static System.Random random = new System.Random();
		static object locker = new object();
		private object lockerLocal = new object();
		
		
		/*************************************************************************************************************************************************
			Playground wrapper
		*************************************************************************************************************************************************/
		
		/// <summary>
		/// Creates a PlaygroundParticlesC object by standard prefab
		/// </summary>
		public static PlaygroundParticlesC Particle () {
			PlaygroundParticlesC playgroundParticles = ResourceInstantiate("Particle Playground System").GetComponent<PlaygroundParticlesC>();
			return playgroundParticles;
		}
		
		/// <summary>
		/// Creates an empty PlaygroundParticlesC object by script
		/// </summary>
		/// <returns>The particle system instance.</returns>
		public static PlaygroundParticlesC ParticleNew () {
			PlaygroundParticlesC playgroundParticles = PlaygroundParticlesC.CreateParticleObject("Particle Playground System "+particlesQuantity,Vector3.zero,Quaternion.identity,1f,new Material(Shader.Find("Playground/Vertex Color")));
			playgroundParticles.particleCache = new ParticleSystem.Particle[playgroundParticles.particleCount];
			PlaygroundParticlesC.OnCreatePlaygroundParticles(playgroundParticles);
			return playgroundParticles;
		}
		
		/// <summary>
		/// Creates a PlaygroundParticlesC object with an image state.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="name">Name.</param>
		/// <param name="position">Position.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="particleSize">Particle size.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="material">Material.</param>
		public static PlaygroundParticlesC Particle (Texture2D image, string name, Vector3 position, Quaternion rotation, Vector3 offset, float particleSize, float scale, Material material) {
			return PlaygroundParticlesC.CreatePlaygroundParticles(new Texture2D[]{image},name,position,rotation,offset,particleSize,scale,material);
		}
		
		/// <summary>
		/// Creates a PlaygroundParticlesC object with an image state.
		/// </summary>
		/// <param name="image">The particle system instance.</param>
		public static PlaygroundParticlesC Particle (Texture2D image) {
			return PlaygroundParticlesC.CreatePlaygroundParticles(new Texture2D[]{image},"Particle Playground System "+particlesQuantity,Vector3.zero,Quaternion.identity,Vector3.zero,1f,1f,new Material(Shader.Find("Playground/Vertex Color")));
		}
		
		/// <summary>
		/// Creates a PlaygroundParticlesC object with several image states.
		/// </summary>
		/// <param name="images">Images.</param>
		/// <param name="name">Name.</param>
		/// <param name="position">Position.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="particleSize">Particle size.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="material">Material.</param>
		public static PlaygroundParticlesC Particle (Texture2D[] images, string name, Vector3 position, Quaternion rotation, Vector3 offset, float particleSize, float scale, Material material) {
			return PlaygroundParticlesC.CreatePlaygroundParticles(images,name,position,rotation,offset,particleSize,scale,material);
		}
		
		/// <summary>
		/// Creates a PlaygroundParticlesC object with several image states.
		/// </summary>
		/// <param name="images">Images.</param>
		public static PlaygroundParticlesC Particle (Texture2D[] images) {
			return PlaygroundParticlesC.CreatePlaygroundParticles(images,"Particle Playground System "+particlesQuantity,Vector3.zero,Quaternion.identity,Vector3.zero,1f,1f,new Material(Shader.Find("Playground/Vertex Color")));
		}
		
		/// <summary>
		/// Creates a PlaygroundParticlesC object with a mesh state.
		/// </summary>
		/// <param name="mesh">Mesh.</param>
		/// <param name="texture">Texture.</param>
		/// <param name="name">Name.</param>
		/// <param name="position">Position.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="particleScale">Particle scale.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="material">Material.</param>
		public static PlaygroundParticlesC Particle (Mesh mesh, Texture2D texture, string name, Vector3 position, Quaternion rotation, float particleScale, Vector3 offset, Material material) {
			return MeshParticles.CreateMeshParticles(new Mesh[]{mesh},new Texture2D[]{texture},null,name,position,rotation,particleScale,new Vector3[]{offset},material);
		}
		
		/// <summary>
		/// Creates a PlaygroundParticlesC object with a mesh state.
		/// </summary>
		/// <param name="mesh">Mesh.</param>
		/// <param name="texture">Texture.</param>
		public static PlaygroundParticlesC Particle (Mesh mesh, Texture2D texture) {
			return MeshParticles.CreateMeshParticles(new Mesh[]{mesh},new Texture2D[]{texture},null,"Particle Playground System "+particlesQuantity,Vector3.zero,Quaternion.identity,1f,new Vector3[]{Vector3.zero},new Material(Shader.Find("Playground/Vertex Color")));
		}
		
		/// <summary>
		/// Creates a PlaygroundParticlesC object with several mesh states.
		/// </summary>
		/// <param name="meshes">Meshes.</param>
		/// <param name="textures">Textures.</param>
		/// <param name="name">Name.</param>
		/// <param name="position">Position.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="particleScale">Particle scale.</param>
		/// <param name="offsets">Offsets.</param>
		/// <param name="material">Material.</param>
		public static PlaygroundParticlesC Particle (Mesh[] meshes, Texture2D[] textures, string name, Vector3 position, Quaternion rotation, float particleScale, Vector3[] offsets, Material material) {
			return MeshParticles.CreateMeshParticles(meshes,textures,null,name,position,rotation,particleScale,offsets,material);
		}
		
		/// <summary>
		/// Creates a PlaygroundParticlesC object with several mesh states.
		/// </summary>
		/// <param name="meshes">Meshes.</param>
		/// <param name="textures">Textures.</param>
		public static PlaygroundParticlesC Particle (Mesh[] meshes, Texture2D[] textures) {
			return MeshParticles.CreateMeshParticles(meshes,textures,null,"Particle Playground System "+particlesQuantity,Vector3.zero,Quaternion.identity,1f,new Vector3[meshes.Length],new Material(Shader.Find("Playground/Vertex Color")));
		}
		
		/// <summary>
		/// Emits next particle - using the particle system as a pool (note that you need to set scriptedEmission-variables on beforehand using this method). Returns emitted particle number.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static int Emit (PlaygroundParticlesC playgroundParticles) {
			return playgroundParticles.Emit(playgroundParticles.scriptedEmissionPosition,playgroundParticles.scriptedEmissionVelocity,playgroundParticles.scriptedEmissionColor);
		}
		
		/// <summary>
		/// Emits next particle while setting scriptedEmission data - using the particle system as a pool. Returns emitted particle number.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="position">Position.</param>
		/// <param name="normal">Normal.</param>
		/// <param name="color">Color.</param>
		public static int Emit (PlaygroundParticlesC playgroundParticles, Vector3 position, Vector3 normal, Color color) {
			return playgroundParticles.Emit(position,normal,color);
		}
		
		/// <summary>
		/// Sets emission on/off.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="setEmission">If set to <c>true</c> set emission.</param>
		public static void Emit (PlaygroundParticlesC playgroundParticles, bool setEmission) {
			playgroundParticles.Emit(setEmission);
		}
		
		/// <summary>
		/// Gets vertices and normals from a skinned world object in Vector3[] format (notice that the array is modified by reference).
		/// </summary>
		/// <param name="particleStateWorldObject">Skinned World Object.</param>
		/// <param name="updateNormals">If set to <c>true</c> update normals.</param>
		public static void GetPosition (SkinnedWorldObject particleStateWorldObject, bool updateNormals) {
			PlaygroundParticlesC.GetPosition(particleStateWorldObject, updateNormals);
		}
		
		/// <summary>
		/// Gets vertices from a world object in Vector3[] format (notice that the array is modified by reference).
		/// </summary>
		/// <param name="vertices">Vertices.</param>
		/// <param name="particleStateWorldObject"World Object.</param>
		public static void GetPosition (Vector3[] vertices, WorldObject particleStateWorldObject) {
			PlaygroundParticlesC.GetPosition(vertices,particleStateWorldObject);
		}
		
		/// <summary>
		/// Gets normals from a world object in Vector3[] format (notice that the array is modified by reference).
		/// </summary>
		/// <param name="normals">Normals.</param>
		/// <param name="particleStateWorldObject">World Object.</param>
		public static void GetNormals (Vector3[] normals, WorldObject particleStateWorldObject) {
			PlaygroundParticlesC.GetNormals(normals,particleStateWorldObject);
		}
		
		/// <summary>
		/// Sets alpha of particles instantly.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="alpha">Alpha.</param>
		public static void SetAlpha (PlaygroundParticlesC playgroundParticles, float alpha) {
			PlaygroundParticlesC.SetAlpha(playgroundParticles,alpha);
		}
		
		/// <summary>
		/// Sets particle size.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="size">Size.</param>
		public static void SetSize (PlaygroundParticlesC playgroundParticles, float size) {
			PlaygroundParticlesC.SetSize(playgroundParticles,size);
		}
		
		/// <summary>
		/// Translates all particles in Particle System.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="direction">Direction.</param>
		public static void Translate (PlaygroundParticlesC playgroundParticles, Vector3 direction) {
			PlaygroundParticlesC.Translate(playgroundParticles,direction);
		}
		
		/// <summary>
		/// Adds a single state.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="state">State.</param>
		public static void Add (PlaygroundParticlesC playgroundParticles, ParticleStateC state) {
			PlaygroundParticlesC.Add(playgroundParticles,state);
		}
		
		/// <summary>
		/// Adds a single state image.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="image">Image.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="stateName">State name.</param>
		public static void Add (PlaygroundParticlesC playgroundParticles, Texture2D image, float scale, Vector3 offset, string stateName) {
			PlaygroundParticlesC.Add(playgroundParticles,image,scale,offset,stateName,null);
		}
		
		/// <summary>
		/// Adds a single state image with transform.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="image">Image.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="stateName">State name.</param>
		/// <param name="stateTransform">State transform.</param>
		public static void Add (PlaygroundParticlesC playgroundParticles, Texture2D image, float scale, Vector3 offset, string stateName, Transform stateTransform) {
			PlaygroundParticlesC.Add(playgroundParticles,image,scale,offset,stateName,stateTransform);
		}
		
		/// <summary>
		/// Adds a single state image with depthmap.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="image">Image.</param>
		/// <param name="depthmap">Depthmap.</param>
		/// <param name="depthmapStrength">Depthmap strength.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="stateName">State name.</param>
		public static void Add (PlaygroundParticlesC playgroundParticles, Texture2D image, Texture2D depthmap, float depthmapStrength, float scale, Vector3 offset, string stateName) {
			PlaygroundParticlesC.Add(playgroundParticles,image,depthmap,depthmapStrength,scale,offset,stateName,null);
		}
		
		/// <summary>
		/// Adds single state image with depthmap and transform.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="image">Image.</param>
		/// <param name="depthmap">Depthmap.</param>
		/// <param name="depthmapStrength">Depthmap strength.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="stateName">State name.</param>
		/// <param name="stateTransform">State transform.</param>
		public static void Add (PlaygroundParticlesC playgroundParticles, Texture2D image, Texture2D depthmap, float depthmapStrength, float scale, Vector3 offset, string stateName, Transform stateTransform) {
			PlaygroundParticlesC.Add(playgroundParticles,image,depthmap,depthmapStrength,scale,offset,stateName,stateTransform);
		}
		
		/// <summary>
		/// Adds a single state mesh.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="mesh">Mesh.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="stateName">State name.</param>
		public static void Add (PlaygroundParticlesC playgroundParticles, Mesh mesh, float scale, Vector3 offset, string stateName) {
			MeshParticles.Add(playgroundParticles,mesh,scale,offset,stateName,null);
		}
		
		/// <summary>
		/// Adds a single state mesh with transform.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="mesh">Mesh.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="stateName">State name.</param>
		/// <param name="stateTransform">State transform.</param>
		public static void Add (PlaygroundParticlesC playgroundParticles, Mesh mesh, float scale, Vector3 offset, string stateName, Transform stateTransform) {
			MeshParticles.Add(playgroundParticles,mesh,scale,offset,stateName,stateTransform);
		}
		
		/// <summary>
		/// Adds a single state mesh with texture.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="mesh">Mesh.</param>
		/// <param name="texture">Texture.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="stateName">State name.</param>
		public static void Add (PlaygroundParticlesC playgroundParticles, Mesh mesh, Texture2D texture, float scale, Vector3 offset, string stateName) {
			MeshParticles.Add(playgroundParticles,mesh,texture,scale,offset,stateName,null);
		}
		
		/// <summary>
		/// Adds a single state mesh with texture and transform.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="mesh">Mesh.</param>
		/// <param name="texture">Texture.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="stateName">State name.</param>
		/// <param name="stateTransform">State transform.</param>
		public static void Add (PlaygroundParticlesC playgroundParticles, Mesh mesh, Texture2D texture, float scale, Vector3 offset, string stateName, Transform stateTransform) {
			MeshParticles.Add(playgroundParticles,mesh,texture,scale,offset,stateName,stateTransform);
		}
		
		/// <summary>
		/// Adds a plane collider.
		/// </summary>
		/// <returns>The PlaygroundColliderC.</returns>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static PlaygroundColliderC AddCollider (PlaygroundParticlesC playgroundParticles) {
			PlaygroundColliderC pCollider = new PlaygroundColliderC();
			playgroundParticles.colliders.Add(pCollider);
			return pCollider;
		}
		
		/// <summary>
		/// Adds a plane collider and assign a transform.
		/// </summary>
		/// <returns>The PlaygroundColliderC.</returns>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="transform">Transform.</param>
		public static PlaygroundColliderC AddCollider (PlaygroundParticlesC playgroundParticles, Transform transform) {
			PlaygroundColliderC pCollider = new PlaygroundColliderC();
			pCollider.transform = transform;
			playgroundParticles.colliders.Add(pCollider);
			return pCollider;
		}
		
		/// <summary>
		/// Sets amount of particles for this Particle System.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="amount">Amount.</param>
		public static void SetParticleCount (PlaygroundParticlesC playgroundParticles, int amount) {
			PlaygroundParticlesC.SetParticleCount(playgroundParticles,amount);
		}
		
		/// <summary>
		/// Sets lifetime for this Particle System.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="time">Time.</param>
		public static void SetLifetime (PlaygroundParticlesC playgroundParticles, float time) {
			PlaygroundParticlesC.SetLifetime(playgroundParticles,playgroundParticles.sorting,time);
		}
		
		/// <summary>
		/// Sets material for this Particle System.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="particleMaterial">Particle material.</param>
		public static void SetMaterial (PlaygroundParticlesC playgroundParticles, Material particleMaterial) {
			PlaygroundParticlesC.SetMaterial(playgroundParticles, particleMaterial);
		}
		
		/// <summary>
		/// Destroys the passed in Particle System.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static void Destroy (PlaygroundParticlesC playgroundParticles) {
			PlaygroundParticlesC.Destroy(playgroundParticles);
		}
		
		/// <summary>
		/// Creates a world object reference (used for live world positioning of particles towards a mesh).
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="meshTransform">Mesh transform.</param>
		public static WorldObject WorldObject (Transform meshTransform) {
			return PlaygroundParticlesC.NewWorldObject(meshTransform);
		}
		
		/// <summary>
		/// Creates a skinned world object reference (used for live world positioning of particles towards a mesh).
		/// </summary>
		/// <returns>The world object.</returns>
		/// <param name="meshTransform">Mesh transform.</param>
		public static SkinnedWorldObject SkinnedWorldObject (Transform meshTransform) {
			return PlaygroundParticlesC.NewSkinnedWorldObject(meshTransform);
		}
		
		/// <summary>
		/// Creates a manipulator object.
		/// </summary>
		/// <returns>The ManipulatorObjectC.</returns>
		/// <param name="type">Type.</param>
		/// <param name="affects">Affects.</param>
		/// <param name="manipulatorTransform">Manipulator transform.</param>
		/// <param name="size">Size.</param>
		/// <param name="strength">Strength.</param>
		public static ManipulatorObjectC ManipulatorObject (MANIPULATORTYPEC type, LayerMask affects, Transform manipulatorTransform, float size, float strength) {
			return PlaygroundParticlesC.NewManipulatorObject(type,affects,manipulatorTransform,size,strength,null);
		}
		
		/// <summary>
		/// Creates a manipulator object by transform.
		/// </summary>
		/// <returns>The ManipulatorObjectC.</returns>
		/// <param name="manipulatorTransform">Manipulator transform.</param>
		public static ManipulatorObjectC ManipulatorObject (Transform manipulatorTransform) {
			LayerMask layerMask = -1;
			return PlaygroundParticlesC.NewManipulatorObject(MANIPULATORTYPEC.Attractor,layerMask,manipulatorTransform,1f,1f,null);
		}
		
		/// <summary>
		/// Create a manipulator in a PlaygroundParticlesC object
		/// </summary>
		/// <returns>The ManipulatorObjectC.</returns>
		/// <param name="type">Type.</param>
		/// <param name="affects">Affects.</param>
		/// <param name="manipulatorTransform">Manipulator transform.</param>
		/// <param name="size">Size.</param>
		/// <param name="strength">Strength.</param>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static ManipulatorObjectC ManipulatorObject (MANIPULATORTYPEC type, LayerMask affects, Transform manipulatorTransform, float size, float strength, PlaygroundParticlesC playgroundParticles) {
			return PlaygroundParticlesC.NewManipulatorObject(type,affects,manipulatorTransform,size,strength,playgroundParticles);
		}
		
		/// <summary>
		/// Creates a manipulator in a PlaygroundParticlesC object by transform.
		/// </summary>
		/// <returns>The ManipulatorObjectC.</returns>
		/// <param name="manipulatorTransform">Manipulator transform.</param>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static ManipulatorObjectC ManipulatorObject (Transform manipulatorTransform, PlaygroundParticlesC playgroundParticles) {
			LayerMask layerMask = -1;
			return PlaygroundParticlesC.NewManipulatorObject(MANIPULATORTYPEC.Attractor,layerMask,manipulatorTransform,1f,1f,playgroundParticles);
		}
		
		/// <summary>
		/// Returns a global manipulator in array position.
		/// </summary>
		/// <returns>The ManipulatorObjectC.</returns>
		/// <param name="i">The index.</param>
		public static ManipulatorObjectC GetManipulator (int i) {
			if (reference.manipulators.Count>0 && reference.manipulators[i%reference.manipulators.Count]!=null)
				return reference.manipulators[i%reference.manipulators.Count];
			else return null;
		}
		
		/// <summary>
		/// Returns a manipulator in a PlaygroundParticlesC object in array position.
		/// </summary>
		/// <returns>The ManipulatorObjectC.</returns>
		/// <param name="i">The index.</param>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static ManipulatorObjectC GetManipulator (int i, PlaygroundParticlesC playgroundParticles) {
			if (playgroundParticles.manipulators.Count>0 && playgroundParticles.manipulators[i%playgroundParticles.manipulators.Count]!=null)
				return playgroundParticles.manipulators[i%playgroundParticles.manipulators.Count];
			else return null;
		}
		
		/// <summary>
		/// Returns all current particles within a local manipulator (in form of an Event Particle, however no event will be needed).
		/// </summary>
		/// <returns>The manipulator particles.</returns>
		/// <param name="manipulator">Manipulator.</param>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static List<PlaygroundEventParticle> GetManipulatorParticles (int manipulator, PlaygroundParticlesC playgroundParticles) {
			if (manipulator<0 || manipulator>=playgroundParticles.manipulators.Count) return null;
			List<PlaygroundEventParticle> particles = new List<PlaygroundEventParticle>();
			PlaygroundEventParticle particle = new PlaygroundEventParticle();
			for (int i = 0; i<playgroundParticles.particleCount; i++) {
				if (playgroundParticles.manipulators[manipulator].Contains(playgroundParticles.playgroundCache.position[i], playgroundParticles.manipulators[manipulator].transform.position)) {
					playgroundParticles.UpdateEventParticle(particle, i);
					particles.Add (particle.Clone());
				}
			}
			return particles;
		}
		
		/// <summary>
		/// Returns all current particles within a global manipulator (in form of an Event Particle, however no event will be needed).
		/// </summary>
		/// <returns>The manipulator particles.</returns>
		/// <param name="manipulator">Manipulator.</param>
		public static List<PlaygroundEventParticle> GetManipulatorParticles (int manipulator) {
			if (manipulator<0 || manipulator>=reference.manipulators.Count) return null;
			List<PlaygroundEventParticle> particles = new List<PlaygroundEventParticle>();
			PlaygroundEventParticle particle = new PlaygroundEventParticle();
			for (int s = 0; s<reference.particleSystems.Count; s++) {
				for (int i = 0; i<reference.particleSystems[s].particleCount; i++) {
					if (reference.particleSystems[s].manipulators[manipulator].Contains(reference.particleSystems[s].playgroundCache.position[i], reference.particleSystems[s].manipulators[manipulator].transform.position)) {
						reference.particleSystems[s].UpdateEventParticle(particle, i);
						particles.Add (particle.Clone());
					}
				}
			}
			return particles;
		}
		
		public static Transform CreateTransform () {
			return (Transform)new GameObject("Source Transform").transform;
		}
		
		public static Transform CreatePlaygroundTransform (PlaygroundParticlesC playgroundParticles) {
			Transform newTransform = CreateTransform();
			PlaygroundTransformC newPlaygroundTransform = new PlaygroundTransformC();
			newPlaygroundTransform.transform = newTransform;
			if (playgroundParticles.sourceTransforms.Count==1 && playgroundParticles.sourceTransforms[0].transform==null)
				playgroundParticles.sourceTransforms[0].transform = newTransform;
			else
				playgroundParticles.sourceTransforms.Add (newPlaygroundTransform);
			newTransform.parent = playgroundParticles.particleSystemTransform;
			newTransform.localPosition = Vector3.zero;
			newTransform.name += " "+playgroundParticles.sourceTransforms.Count.ToString();
			return newTransform;
		}
		
		/// <summary>
		/// Creates a new Playground Spline. The spline will be attached to a new GameObject in your scene by the name "Playground Spline".
		/// </summary>
		/// <returns>The Playground Spline component on the created GameObject.</returns>
		public static PlaygroundSpline CreateSpline () {
			GameObject splineGO = new GameObject("Playground Spline", typeof(PlaygroundSpline));
			return (PlaygroundSpline)splineGO.GetComponent(typeof(PlaygroundSpline));
		}
		
		/// <summary>
		/// Creates a new Playground Spline. The spline will be attached to a new GameObject in your scene by the name "Playground Spline" which is parented to the passed in Particle Playground system. 
		/// The Particle Playground system will have the source spline automatically set.
		/// </summary>
		/// <returns>The Playground Spline component on the created GameObject.</returns>
		/// <param name="playgroundParticles">Particle Playground system.</param>
		public static PlaygroundSpline CreateSpline (PlaygroundParticlesC playgroundParticles) {
			PlaygroundSpline spline = CreateSpline();
			spline.Reset();
			if (playgroundParticles.splines.Count==1 && playgroundParticles.splines[0]==null)
				playgroundParticles.splines[0] = spline;
			else
				playgroundParticles.splines.Add (spline);
			spline.transform.parent = playgroundParticles.particleSystemTransform;
			spline.transform.localPosition = Vector3.zero;
			spline.name += " "+playgroundParticles.splines.Count.ToString();
			spline.AddUser(playgroundParticles.particleSystemTransform);
			return spline;
		}
		
		public static PlaygroundSpline CreateSpline (ManipulatorPropertyC manipulatorProperty) {
			PlaygroundSpline spline = CreateSpline();
			spline.Reset();
			manipulatorProperty.splineTarget = spline;
			return spline;
		}
		
		/// <summary>
		/// Creates an empty event.
		/// </summary>
		/// <returns>The event.</returns>
		public static PlaygroundEventC CreateEvent () {
			return new PlaygroundEventC();
		}
		
		/// <summary>
		/// Creates an event into passed particle system.
		/// </summary>
		/// <returns>The event.</returns>
		/// <param name="playgroundParticles">Particle Playground system.</param>
		public static PlaygroundEventC CreateEvent (PlaygroundParticlesC playgroundParticles) {
			if (playgroundParticles==null) return null;
			if (playgroundParticles.events==null)
				playgroundParticles.events = new List<PlaygroundEventC>();
			playgroundParticles.events.Add (new PlaygroundEventC());
			return playgroundParticles.events[playgroundParticles.events.Count-1];
		}
		
		/// <summary>
		/// Returns an event from PlaygroundParticlesC object in array position.
		/// </summary>
		/// <returns>The event.</returns>
		/// <param name="i">The index.</param>
		/// <param name="playgroundParticles">Particle Playground system.</param>
		public static PlaygroundEventC GetEvent (int i, PlaygroundParticlesC playgroundParticles) {
			if (playgroundParticles.events.Count>0 && playgroundParticles.events[i%playgroundParticles.events.Count]!=null)
				return playgroundParticles.events[i%playgroundParticles.events.Count];
			else return null;
		}
		
		/// <summary>
		/// Removes the event.
		/// </summary>
		/// <param name="i">The index.</param>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static void RemoveEvent (int i, PlaygroundParticlesC playgroundParticles) {
			i = i%playgroundParticles.events.Count;
			if (playgroundParticles.events.Count>0 && playgroundParticles.events[i]!=null) {
				if (playgroundParticles.events[i].target!=null) {
					for (int x = 0; x<playgroundParticles.events[i].target.eventControlledBy.Count; x++)
						if (playgroundParticles.events[i].target.eventControlledBy[x]==playgroundParticles)
							playgroundParticles.events[i].target.eventControlledBy.RemoveAt(x);
				}
				playgroundParticles.events.RemoveAt (i);
			}
		}
		
		/// <summary>
		/// Returns a Particle Playground System in array position.
		/// </summary>
		/// <returns>The particles.</returns>
		/// <param name="i">The index.</param>
		public static PlaygroundParticlesC GetParticles (int i) {
			if (reference.particleSystems.Count>0 && reference.particleSystems[i%reference.particleSystems.Count]!=null)
				return reference.particleSystems[i%reference.particleSystems.Count];
			else return null;
		}
		
		/// <summary>
		/// Creates a projection object reference.
		/// </summary>
		/// <returns>The projection.</returns>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static ParticleProjectionC ParticleProjection (PlaygroundParticlesC playgroundParticles)  {
			return PlaygroundParticlesC.NewProjectionObject(playgroundParticles);
		}
		
		/// <summary>
		/// Creates a paint object reference.
		/// </summary>
		/// <returns>The object.</returns>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static PaintObjectC PaintObject (PlaygroundParticlesC playgroundParticles) {
			return PlaygroundParticlesC.NewPaintObject(playgroundParticles);
		}
		
		/// <summary>
		/// Live paints into a PlaygroundParticlesC PaintObject's positions.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="position">Position.</param>
		/// <param name="normal">Normal.</param>
		/// <param name="parent">Parent.</param>
		/// <param name="color">Color.</param>
		public static int Paint (PlaygroundParticlesC playgroundParticles, Vector3 position, Vector3 normal, Transform parent, Color32 color) {
			return playgroundParticles.paint.Paint(position,normal,parent,color);
		}
		
		/// <summary>
		/// Live paints into a PaintObject's positions directly.
		/// </summary>
		/// <param name="paintObject">Paint object.</param>
		/// <param name="position">Position.</param>
		/// <param name="normal">Normal.</param>
		/// <param name="parent">Parent.</param>
		/// <param name="color">Color.</param>
		public static void Paint (PaintObjectC paintObject, Vector3 position, Vector3 normal, Transform parent, Color32 color) {
			paintObject.Paint(position,normal,parent,color);
		}
		
		/// <summary>
		/// Live erases into a PlaygroundParticlesC PaintObject's positions, returns true if position was erased.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="position">Position.</param>
		/// <param name="radius">Radius.</param>
		public static bool Erase (PlaygroundParticlesC playgroundParticles, Vector3 position, float radius) {
			return playgroundParticles.paint.Erase(position,radius);
		}
		
		/// <summary>
		/// Live erases into a PaintObject's positions directly, returns true if position was erased.
		/// </summary>
		/// <param name="paintObject">Paint object.</param>
		/// <param name="position">Position.</param>
		/// <param name="radius">Radius.</param>
		public static bool Erase (PaintObjectC paintObject, Vector3 position, float radius) {
			return paintObject.Erase(position,radius);
		}
		
		/// <summary>
		/// Live erases into a PlaygroundParticlesC PaintObject's using a specified index, returns true if position was erased.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="index">Index.</param>
		public static bool Erase (PlaygroundParticlesC playgroundParticles, int index) {
			return playgroundParticles.paint.Erase(index);
		}
		
		/// <summary>
		/// Clears out paint in a PlaygroundParticlesC object.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static void ClearPaint (PlaygroundParticlesC playgroundParticles) {
			if (playgroundParticles.paint!=null)
				playgroundParticles.paint.ClearPaint();
		}
		
		/// <summary>
		/// Gets the amount of paint positions in this PlaygroundParticlesC PaintObject.
		/// </summary>
		/// <returns>The paint position length.</returns>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static int GetPaintPositionLength (PlaygroundParticlesC playgroundParticles) {
			return playgroundParticles.paint.positionLength;
		}
		
		/// <summary>
		/// Sets initial target position for this Particle System.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="position">Position.</param>
		public static void SetInitialTargetPosition (PlaygroundParticlesC playgroundParticles, Vector3 position) {
			PlaygroundParticlesC.SetInitialTargetPosition(playgroundParticles,position, true);
		}
		
		/// <summary>
		/// Sets emission for this Particle System.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="emit">If set to <c>true</c> emit.</param>
		public static void Emission (PlaygroundParticlesC playgroundParticles, bool emit) {
			PlaygroundParticlesC.Emission(playgroundParticles,emit,false);
		}
		
		/// <summary>
		/// Set emission for this Particle System controlling to run rest emission
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		/// <param name="emit">If set to <c>true</c> emit.</param>
		/// <param name="restEmission">If set to <c>true</c> rest emission.</param>
		public static void Emission (PlaygroundParticlesC playgroundParticles, bool emit, bool restEmission) {
			PlaygroundParticlesC.Emission(playgroundParticles,emit,restEmission);
		}
		
		/// <summary>
		/// Clears out this Particle System.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static void Clear (PlaygroundParticlesC playgroundParticles) {
			PlaygroundParticlesC.Clear(playgroundParticles);
		}
		
		/// <summary>
		/// Refreshes source scatter for this Particle System.
		/// </summary>
		/// <param name="playgroundParticles">Playground particles.</param>
		public static void RefreshScatter (PlaygroundParticlesC playgroundParticles) {
			playgroundParticles.RefreshScatter();
		}
		
		/// <summary>
		/// Instantiates a preset by name reference.
		/// Any preset you wish to instantiate using the InstantiatePreset method needs to be in a 'Resources/Presets/' folder.
		/// </summary>
		/// <returns>The preset as a PlaygroundParticlesC object.</returns>
		/// <param name="presetName">Preset name.</param>
		public static PlaygroundParticlesC InstantiatePreset (string presetName) {
			GameObject presetGo = ResourceInstantiate("Presets/"+presetName);
			PlaygroundParticlesC presetParticles = presetGo.GetComponent<PlaygroundParticlesC>();
			if (presetParticles!=null) {
				if (reference==null)
					reference = PlaygroundC.ResourceInstantiate("Playground Manager").GetComponent<PlaygroundC>();
				if (reference) {
					if (reference.autoGroup && presetParticles.particleSystemTransform.parent==null)
						presetParticles.particleSystemTransform.parent = referenceTransform;
					particlesQuantity++;
					presetParticles.particleSystemId = particlesQuantity-1;
				}
				presetGo.name = presetName;
				return presetParticles;
			} else {
				if (presetGo.name.Contains("Presets/"))
					presetGo.name = presetGo.name.Remove(0, 8);
				return null;
			}
		}
		
		/// <summary>
		/// Instantiates a preset by category and name reference. The category is the name of the folder.
		/// Any preset you wish to instantiate using this InstantiatePreset overload needs to be in a 'Resources/Presets/categoryName/' folder.
		/// </summary>
		/// <returns>The preset as a PlaygroundParticlesC object.</returns>
		/// <param name="categoryName">Category name.</param>
		/// <param name="presetName">Preset name.</param>
		public static PlaygroundParticlesC InstantiatePreset (string categoryName, string presetName)
		{
			return InstantiatePreset(categoryName+"/"+presetName);
		}
		
		/// <summary>
		/// Determines if the Playground Manager has its reference assigned.
		/// </summary>
		/// <returns><c>true</c> if having a reference; otherwise, <c>false</c>.</returns>
		public static bool HasReference () {
			return hasReference;
		}
		
		/// <summary>
		/// Determines if the Playground Manager has a Playground Pool assigned.
		/// </summary>
		/// <returns><c>true</c> if having a Playground Pool; otherwise, <c>false</c>.</returns>
		public static bool HasPlaygroundPool () {
			return hasPlaygroundPool;
		}
		
		/// <summary>
		/// Determines whether the thread for threadMethod if set to ThreadMethod.OneForAll is finished. This is always true when any other method is selected.
		/// </summary>
		/// <returns><c>true</c> if the OneForAll thread is finished; otherwise, <c>false</c>.</returns>
		public bool IsDoneThread () {
			return isDoneThread;
		}
		
		/// <summary>
		/// Determines whether the thread for turbulence is done.
		/// </summary>
		/// <returns><c>true</c> if thread for turbulence is done; otherwise, <c>false</c>.</returns>
		public bool IsDoneThreadTurbulence () {
			return isDoneThreadTurbulence;
		}
		
		/// <summary>
		/// Determines whether the thread for skinned meshes is done.
		/// </summary>
		/// <returns><c>true</c> if the thread for skinned meshes is done; otherwise, <c>false</c>.</returns>
		public bool IsDoneThreadSkinnedMeshes () {
			return isDoneThreadSkinned;
		}
		
		/// <summary>
		/// Determines whether the multithreading is within the initial first unsafe frames.
		/// </summary>
		/// <returns><c>true</c> if it is the first unsafe automatic frames; otherwise, <c>false</c>.</returns>
		public bool IsFirstUnsafeAutomaticFrames () {
			return isFirstUnsafeAutomaticFrames;
		}
		
		/// <summary>
		/// Determines whether there is enabled global manipulators which should be calculated.
		/// </summary>
		/// <returns><c>true</c> if there is enabled global manipulators; otherwise, <c>false</c>.</returns>
		public bool HasEnabledGlobalManipulators () {
			if (manipulators.Count>0) {
				for (int i = 0; i<manipulators.Count; i++) {
					if (manipulators[i].enabled && manipulators[i].transform!=null && manipulators[i].transform.transform!=null) {
						return true;
					}
				}
			}
			return false;
		}
		
		
		/*************************************************************************************************************************************************
			Shared functions
		*************************************************************************************************************************************************/
		
		/// <summary>
		/// Returns pixels from a texture.
		/// </summary>
		/// <returns>The pixels.</returns>
		/// <param name="image">Image.</param>
		public static Color32[] GetPixels (Texture2D image) {
			if (image == null) return null;
			if (reference && reference.pixelFilterMode==PIXELMODEC.Bilinear) {
				Color32[] pixels = new Color32[image.width*image.height];
				for (int y = 0; y<image.height; y++) {
					for (int x = 0; x<image.width; x++) {
						pixels[(y*image.width)+x] = image.GetPixelBilinear((x*1f)/image.width, (y*1f)/image.height); 
					}
				}
				return pixels;
			} else return image.GetPixels32();
		}
		
		// Returns offset based on image size.
		public static Vector3 Offset (PLAYGROUNDORIGINC origin, int imageWidth, int imageHeight, float meshScale) {
			Vector3 offset = Vector3.zero;
			switch (origin) {
			case PLAYGROUNDORIGINC.TopLeft: 		offset.y = -imageHeight*meshScale; break;
			case PLAYGROUNDORIGINC.TopCenter: 		offset.y = -imageHeight*meshScale; offset.x = (-imageWidth*meshScale)/2; break;
			case PLAYGROUNDORIGINC.TopRight: 		offset.y = -imageHeight*meshScale; offset.x = (-imageWidth*meshScale); break;
			case PLAYGROUNDORIGINC.MiddleLeft: 		offset.y = -imageHeight*meshScale/2; break;
			case PLAYGROUNDORIGINC.MiddleCenter:	offset.y = -imageHeight*meshScale/2; offset.x = (-imageWidth*meshScale)/2; break;
			case PLAYGROUNDORIGINC.MiddleRight:		offset.y = -imageHeight*meshScale/2; offset.x = (-imageWidth*meshScale); break;
			case PLAYGROUNDORIGINC.BottomCenter:	offset.x = (-imageWidth*meshScale)/2; break;
			case PLAYGROUNDORIGINC.BottomRight:		offset.x = (-imageWidth*meshScale); break;
			}
			return offset;
		}
		
		/// <summary>
		/// Returns a random vector3-array.
		/// </summary>
		/// <returns>The vector3.</returns>
		/// <param name="length">Length.</param>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Max.</param>
		public static Vector3[] RandomVector3 (int length, Vector3 min, Vector3 max) {
			Vector3[] v3 = new Vector3[length];
			for (int i = 0; i<length; i++) {
				v3[i] = new Vector3(
					PlaygroundParticlesC.RandomRange(random, min.x, max.x),
					PlaygroundParticlesC.RandomRange(random, min.y, max.y),
					PlaygroundParticlesC.RandomRange(random, min.z, max.z)
					);
			}
			return v3;
		}
		
		/// <summary>
		/// Returns a float array by random values.
		/// </summary>
		/// <returns>The float.</returns>
		/// <param name="length">Length.</param>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Max.</param>
		public static float[] RandomFloat (int length, float min, float max) {
			float[] f = new float[length];
			for (int i = 0; i<length; i++) {
				f[i] = PlaygroundParticlesC.RandomRange (random, min, max);
			}
			return f;
		}
		
		/// <summary>
		/// Shuffles an existing built-in float array.
		/// </summary>
		/// <param name="arr">Arr.</param>
		public static void ShuffleArray (float[] arr) {
			int r;
			float tmp;
			for (int i = arr.Length - 1; i > 0; i--) {
				r = random.Next(0,i);
				tmp = arr[i];
				arr[i] = arr[r];
				arr[r] = tmp;
			}
		}
		
		/// <summary>
		/// Shuffles an existing built-in int array.
		/// </summary>
		/// <param name="arr">Arr.</param>
		public static void ShuffleArray (int[] arr) {
			int r;
			int tmp;
			for (int i = arr.Length - 1; i > 0; i--) {
				r = random.Next(0,i);
				tmp = arr[i];
				arr[i] = arr[r];
				arr[r] = tmp;
			}
		}
		
		/// <summary>
		/// Compares and returns the largest number in an int array.
		/// </summary>
		/// <param name="compare">Ints to compare.</param>
		public static int Largest (int[] compare) {
			int largest = 0;
			for (int i = 0; i<compare.Length; i++)
				if (compare[i]>largest) largest = i;
			return largest;
		}
		
		/// <summary>
		/// Counts the completely transparent pixels in a Texture2D.
		/// </summary>
		/// <returns>The zero alphas in texture.</returns>
		/// <param name="image">Image.</param>
		public static int CountZeroAlphasInTexture (Texture2D image) {
			int alphaCount = 0;
			Color32[] pixels = image.GetPixels32();
			for (int x = 0; x<pixels.Length; x++)
				if (pixels[x].a==0)
					alphaCount++;
			return alphaCount;
		}
		
		/// <summary>
		/// Instantiates from Resources.
		/// </summary>
		/// <returns>The instantiate.</returns>
		/// <param name="n">N.</param>
		public static GameObject ResourceInstantiate (string n) {
			GameObject res = UnityEngine.Resources.Load(n) as GameObject;
			if (!res)
				return null;
			GameObject go = Instantiate(res) as GameObject;
			go.name = n;
			return go;
		}
		
		
		/*************************************************************************************************************************************************
			Playground Manager
		*************************************************************************************************************************************************/
		
		/// <summary>
		/// Reset time
		/// </summary>
		public static void TimeReset () {
			globalDeltaTime = .0f;
			#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying) {
				globalTime = 0f;
			} else
				globalTime = Time.timeSinceLevelLoad;
			#else
			globalTime = Time.timeSinceLevelLoad;
			#endif
			lastTimeUpdated = globalTime;
		}
		
		/// <summary>
		/// Initializes the Playground Manager.
		/// </summary>
		public IEnumerator InitializePlayground () {
			
			// Check for duplicates of the Playground Manager
			if (reference!=null && reference!=this) {
				yield return null;
				Debug.Log("There can only be one instance of the Playground Manager in the scene.");
				
				// Save all children!
				foreach (Transform child in transform)
					child.parent = null;
				DestroyImmediate(gameObject);
				yield break;
			} else if (reference==null)
				reference = this;
			
			// Check events availability
			CheckEvents();
			
			// New random
			random = new System.Random();
			
			// Reset time
			TimeReset();
			
			// Remove any null particle systems
			for (int i = 0; i<particleSystems.Count; i++) {
				if (particleSystems[i]!=null) {
					particleSystems[i].particleSystemId = i;
				} else {
					particleSystems.RemoveAt(i);
					i--;
				}
			}
			
			// Set quantity counter
			particlesQuantity = particleSystems.Count;
			
			// Spline preview
			#if UNITY_EDITOR
			PlaygroundSpline.drawSplinePreviews = drawSplinePreview;
			#endif
			
			// Get ready
			frameCount = 0;
			threadAggregatorRuns = 0;
			isReady = true;
			isDoneThread = true;
			isDoneThreadLocal = true;
			isDoneThreadSkinned = true;
			isDoneThreadTurbulence = true;
			SetMaxThreads(maxThreads);
		}
		int frameCount = 0;
		bool isReady = false;
		public static bool IsReady () {
			return reference.isReady;
		}
		
		/// <summary>
		/// Updates the global time.
		/// </summary>
		public static void SetTime () {
			
			// Set time
			if (!Application.isPlaying || !reference.globalTimeScale)
				globalTime += (Time.realtimeSinceStartup-lastTimeUpdated)*globalTimescale;
			else
				globalTime += (Time.deltaTime)*globalTimescale;
			
			// Set delta time
			globalDeltaTime = globalTime-lastTimeUpdated;
			
			// Set interval stamp
			lastTimeUpdated = globalTime;
		}
		
		/// <summary>
		/// Sets the maximum threads allowed. When using Thread Pool Method: Playground Pool this will set the amount of reused threads.
		/// </summary>
		/// <param name="threadCount">The amount of threads.</param>
		public static void SetMaxThreads (int threadCount) {
			if (!HasReference ())
				return;
			reference.maxThreads = Mathf.Clamp (threadCount, 1, 128);
			processorCount = SystemInfo.processorCount;
			reference.threads = Mathf.Clamp (processorCount>1?processorCount-1:1, 1, reference.maxThreads);
			reference._prevMaxThreadCount = reference.maxThreads;
			
			// Resize the Playground Pool if necessary
			#if UNITY_WSA && !UNITY_EDITOR
			#else
			if (reference.threadPoolMethod == ThreadPoolMethod.PlaygroundPool && playgroundPool != null)
				playgroundPool.SetThreadPool(reference.maxThreads);
			#endif
		}
		
		static void CheckEvents () {
			particleEventBirthInitialized = particleEventBirth!=null;
			particleEventDeathInitialized = particleEventDeath!=null;
			particleEventCollisionInitialized = particleEventCollision!=null;
			particleEventTimeInitialized = particleEventTime!=null;
		}
		
		
		/*************************************************************************************************************************************************
		// MonoBehaviours
		*************************************************************************************************************************************************/
		
		// Initializes the Playground Manager.
		// OnEnable is called in both Edit- and Play Mode.
		void OnEnable () {
			
			// Cache the Playground reference
			referenceTransform = transform;
			referenceGameObject = gameObject;
			
			// Initialize
			StartCoroutine(InitializePlayground());
		}
		
		// Set initial time
		void Start () {SetTime();}
		
		/// <summary>
		/// LateUpdate will prepare and call for calculations on each particle system.
		/// All threaded calculation is done after Update to ensure stable movement of particles.
		/// </summary>
		void LateUpdate () {
			activeThreads = 0;
			if (activeSystems.Count>0)
				activeSystems.Clear();
			frameCount++;
			isFirstUnsafeAutomaticFrames = frameCount<unsafeAutomaticThreadFrames||threadAggregatorRuns<unsafeAutomaticThreadFrames;
			if (reference!=this) {
				InitializePlayground();
				return;
			}
			
			// Update time
			SetTime();
			
			// Check the global event delegates for availability
			CheckEvents();
			
			// Determine that the reference is available
			hasReference = reference != null;
			
			#if UNITY_WSA && !UNITY_EDITOR
			#else
			// Determine that a Playground Pool is available
			hasPlaygroundPool = playgroundPool != null;
			#endif
			
			// Update thread workers if maxThreads change
			if (threadPoolMethod == ThreadPoolMethod.PlaygroundPool && maxThreads!=_prevMaxThreadCount)
				SetMaxThreads (maxThreads);
			
			// Prepare all global manipulators
			for (int m = 0; m<manipulators.Count; m++)
				manipulators[m].Update();
			
			// Return if no particle systems are available
			if (particleSystems.Count==0)
				return;
			
			hasActiveParticleSystems = false;
			hasLocalNoThreads = false;
			hasLocalOnePerSystem = false;
			hasLocalOneForAll = false;
			hasActiveSkinnedMeshes = false;
			hasActiveTurbulence = false;
			if (previousThreadMethod!=threadMethod) {
				isDoneThread = true;
				previousThreadMethod = threadMethod;
			}
			if (threadMethod!=ThreadMethod.OneForAll)
				isDoneThread = true;
			if (previousSkinnedMeshThreadMethod!=skinnedMeshThreadMethod) {
				isDoneThreadSkinned = true;
				previousSkinnedMeshThreadMethod = skinnedMeshThreadMethod;
			}
			if (previousTurbulenceThreadMethod!=turbulenceThreadMethod) {
				isDoneThreadTurbulence = true;
				previousTurbulenceThreadMethod = turbulenceThreadMethod;
			}
			
			// Prepare all particle systems
			Camera mainCamera = Camera.main;
			bool frustumPlanesSet = false;
			bool hasSourceUpdateNow = false;
			int currentParticleSystemCount = particleSystems.Count;
			for (int i = 0; i<particleSystems.Count; i++) {
				if (particleSystems[i]==null)
				{
					particleSystems.RemoveAt (i);
					i--;
					if (i<0) i = 0;
					continue;
				}
				if (!particleSystems[i].isSnapshot &&
				    particleSystems[i].shurikenParticleSystem!=null &&
				    particleSystems[i].particleSystemGameObject.activeInHierarchy &&
				    particleSystems[i].Initialized() &&
				    !particleSystems[i].IsSettingParticleCount() &&
				    !particleSystems[i].IsSettingLifetime() &&
				    !particleSystems[i].IsYieldRefreshing() &&
				    !particleSystems[i].InTransition() &&
				    !particleSystems[i].IsLoading() &&
				    !particleSystems[i].IsPrewarming() &&
				    !particleSystems[i].IsSettingParticleTime() &&
				    !particleSystems[i].inPlayback) {
					
					// Catch up Shuriken particle lifetime with the update rate
					if (Time.frameCount%particleSystems[i].updateRate!=0) {
						for (int p = 0; p<particleSystems[i].particleCount; p++)
							particleSystems[i].particleCache[p].lifetime = (particleSystems[i].playgroundCache.death[p]-particleSystems[i].playgroundCache.birth[p])-particleSystems[i].playgroundCache.lifetimeSubtraction[p];
						particleSystems[i].isReadyForThreadedCalculations = false;
						continue;
					}
					
					// Handle any system additions or removals
					if (currentParticleSystemCount > particleSystems.Count) {
						if (i>0) {
							i--;
							currentParticleSystemCount = particleSystems.Count;
						} else {hasActiveParticleSystems=false; return;}
					} else if (currentParticleSystemCount < particleSystems.Count) {
						currentParticleSystemCount = particleSystems.Count;
					}
					if (particleSystems.Count==0 || i>=particleSystems.Count) {hasActiveParticleSystems=false; return;}
					
					// Update the main camera frustum planes (this is used for culling particle systems)
					if (!frustumPlanesSet && particleSystems[i].pauseCalculationWhenInvisible && mainCamera!=null && mainCamera.enabled) {
						frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
						frustumPlanesSet = true;
					}
					
					// Update any particle system using onlySourcePositioning or onlyLifetimePositioning
					if (particleSystems[i].onlySourcePositioning||particleSystems[i].onlyLifetimePositioning)
						if (!particleSystems[i].UpdateSystem())
							continue;
					
					// Update any changes. Particle system may also be removed here.
					if (particleSystems.Count>0 && i<particleSystems.Count) {
						
						// Prepare for threaded calculations
						particleSystems[i].isReadyForThreadedCalculations = particleSystems[i].PrepareThreadedCalculations();
						if (particleSystems[i].IsAlive()) {
							hasActiveParticleSystems = true;
							if (particleSystems[i].onlySourcePositioning||particleSystems[i].onlyLifetimePositioning)
								hasSourceUpdateNow = true;
							if (threadMethod==ThreadMethod.Automatic)
								activeSystems.Add (i);
							if (!particleSystems[i].forceSkinnedMeshUpdateOnMainThread && particleSystems[i].IsSkinnedWorldObjectReady() && particleSystems[i].calculate)
								hasActiveSkinnedMeshes = true;
							if (particleSystems[i].HasTurbulence() && particleSystems[i].calculate)
								hasActiveTurbulence = true;
							
							switch (particleSystems[i].threadMethod) {
							case ThreadMethodLocal.NoThreads:hasLocalNoThreads=true;break;
							case ThreadMethodLocal.OnePerSystem:hasLocalOnePerSystem=true;break;
							case ThreadMethodLocal.OneForAll:hasLocalOneForAll=true;break;
							}
						} else particleSystems[i].isReadyForThreadedCalculations = false;
					} else if (particleSystems.Count>0 && i<particleSystems.Count && particleSystems[i]!=null) {
						particleSystems[i].isReadyForThreadedCalculations = false;
					}
				} else if (particleSystems[i]!=null) {
					particleSystems[i].isReadyForThreadedCalculations = false;
					if (particleSystems[i].IsPrewarming()) {
						if (particleSystems[i].HasTurbulence()) {
							hasActiveTurbulence = true;
							hasActiveParticleSystems = true;
						}
					}
				}
				
			}
			
			// Proceed if we have any active particle systems
			if (hasActiveParticleSystems) {
				
				// Call for threaded calculations
				ThreadAggregator();
				
				// Call for immediate update on particle systems using onlySourcePositioning or onlyLifetimePositioning
				if (hasSourceUpdateNow) {
					for (int i = 0; i<particleSystems.Count; i++) {
						if (particleSystems[i]!=null &&
						    (particleSystems[i].onlySourcePositioning || particleSystems[i].onlyLifetimePositioning) &&
						    particleSystems[i].isReadyForThreadedCalculations &&
						    !particleSystems[i].InTransition() &&
						    !particleSystems[i].IsLoading() &&
						    particleSystems[i].IsReady()) {
							particleSystems[i].UpdateShuriken();
						}
					}
				}
			}
			
			// Reset hierarchy repaint check
			#if UNITY_EDITOR
			PlaygroundParticlesC.didHierarchyRepaint = false;
			#endif
		}
		
		/// <summary>
		/// Update will sync and put all particles into the Shuriken component of each particle system. 
		/// This runs before the threaded calculations as we otherwise experience teared presentation of their movement.
		/// </summary>
		void Update () {
			#if UNITY_WSA && !UNITY_EDITOR
			// Playground Pool is not compatible with Windows Store/Universal, fall back to the .NET Thread Pool
			threadPoolMethod = ThreadPoolMethod.ThreadPool;
			#endif
			
			int currentParticleSystemCount = particleSystems.Count;
			for (int i = 0; i<particleSystems.Count; i++) {
				
				// Handle any system additions or removals
				if (currentParticleSystemCount > particleSystems.Count) {
					if (i>0) {
						i--;
						currentParticleSystemCount = particleSystems.Count;
					} else {hasActiveParticleSystems=false; return;}
				} else if (currentParticleSystemCount < particleSystems.Count) {
					currentParticleSystemCount = particleSystems.Count;
				}
				if (particleSystems.Count==0 || i>=particleSystems.Count) {hasActiveParticleSystems=false; return;}
				
				// Update Shuriken
				if (particleSystems[i]!=null &&
				    !particleSystems[i].onlySourcePositioning &&
				    !particleSystems[i].onlyLifetimePositioning &&
				    particleSystems[i].UpdateSystem() &&
				    particleSystems[i].isReadyForThreadedCalculations &&
				    !particleSystems[i].InTransition() &&
				    !particleSystems[i].IsLoading() &&
				    !particleSystems[i].IsSettingParticleTime() &&
				    particleSystems[i].IsReady() &&
				    !particleSystems[i].inPlayback) {
					particleSystems[i].UpdateShuriken();
				}
			}
		}
		
		#if UNITY_EDITOR
		void RefreshHieararchy () {
			UnityEditor.EditorApplication.RepaintHierarchyWindow();
		}
		#endif
		
		bool hasActiveParticleSystems = false;
		bool hasLocalNoThreads = false;
		bool hasLocalOnePerSystem = false;
		bool hasLocalOneForAll = false;
		bool hasActiveSkinnedMeshes = false;
		bool hasActiveTurbulence = false;
		bool isFirstUnsafeAutomaticFrames = true;
		int threadAggregatorRuns = 0;
		List<int> activeSystems = new List<int>();
		
		ThreadMethod previousThreadMethod;
		ThreadMethodComponent previousSkinnedMeshThreadMethod;
		ThreadMethodComponent previousTurbulenceThreadMethod;
		
		/// <summary>
		/// Returns the amount of current active threads created.
		/// </summary>
		/// <returns>The thread amount.</returns>
		public static int ActiveThreads () {
			return activeThreads;
		}
		
		public static int ProcessorCount () {
			return processorCount;
		}
		
		/// <summary>
		/// Controls the different methods of distributing calculation threads.
		/// </summary>
		void ThreadAggregator () {
			
			// Run skinned mesh calculation
			if (hasActiveSkinnedMeshes) {
				switch (skinnedMeshThreadMethod) {
				case ThreadMethodComponent.InsideParticleCalculation:
					// Skinned meshes will be prepared inside the particle system's calculation loop before its particles.
					// This doesn't mean that it is calculated on the main-thread unless PlaygroundC.threadMethod is set to NoThreads or PlaygroundParticlesC.forceSkinnedMeshUpdateOnMainThread is enabled.
					break;
				case ThreadMethodComponent.OnePerSystem:
					for(int i = 0; i<particleSystems.Count; i++) {
						if (!particleSystems[i].forceSkinnedMeshUpdateOnMainThread && particleSystems[i].IsSkinnedWorldObjectReady())
							particleSystems[i].skinnedWorldObject.UpdateOnNewThread();
					}
					break;
				case ThreadMethodComponent.OneForAll:
					if (isDoneThreadSkinned) {
						isDoneThreadSkinned = false;
						RunAsync(()=>{
							if (isDoneThreadSkinned) return; 
							for(int i = 0; i<particleSystems.Count; i++) 
								if (!particleSystems[i].forceSkinnedMeshUpdateOnMainThread && particleSystems[i].IsSkinnedWorldObjectReady())
									particleSystems[i].skinnedWorldObject.Update();
							isDoneThreadSkinned=true;
						});
					}
					break;
				}
			}
			
			// Run turbulence calculation
			if (hasActiveTurbulence && turbulenceThreadMethod==ThreadMethodComponent.OneForAll) {
				if (isDoneThreadTurbulence) {
					isDoneThreadTurbulence = false;
					RunAsync(()=>{
						if (isDoneThreadTurbulence) return; 
						for(int i = 0; i<particleSystems.Count; i++)  {
							if (particleSystems[i].HasTurbulence()) {
								for (int p = 0; p<particleSystems[i].particleCount; p++) {
									if (!particleSystems[i].playgroundCache.noForce[p])
										PlaygroundParticlesC.Turbulence (particleSystems[i], particleSystems[i].GetSimplex(), p, particleSystems[i].GetDeltaTime(), particleSystems[i].turbulenceType, particleSystems[i].turbulenceTimeScale, particleSystems[i].turbulenceScale/particleSystems[i].velocityScale, particleSystems[i].turbulenceStrength, particleSystems[i].turbulenceApplyLifetimeStrength, particleSystems[i].turbulenceLifetimeStrength);
								}
							}
							isDoneThreadTurbulence=true;
						}
					});
				}
			}
			
			// Run the independently set thread methods
			if (hasLocalNoThreads) {
				for(int i = 0; i<particleSystems.Count; i++)
					if (particleSystems[i].isReadyForThreadedCalculations && particleSystems[i].threadMethod==ThreadMethodLocal.NoThreads)
						PlaygroundParticlesC.ThreadedCalculations(particleSystems[i]);
			}
			if (hasLocalOnePerSystem) {
				for(int i = 0; i<particleSystems.Count; i++)
					if (particleSystems[i].threadMethod==ThreadMethodLocal.OnePerSystem)
						PlaygroundParticlesC.NewCalculatedThread(particleSystems[i]);
			}
			if (hasLocalOneForAll || (isFirstUnsafeAutomaticFrames && threadMethod==ThreadMethod.Automatic)) {
				if (isDoneThreadLocal) {
					isDoneThreadLocal = false;
					RunAsync(()=>{
						if (isDoneThreadLocal) return; 
						lock (lockerLocal) {
							for(int i = 0; i<particleSystems.Count; i++) 
							if (particleSystems[i].isReadyForThreadedCalculations && (particleSystems[i].threadMethod==ThreadMethodLocal.OneForAll || isFirstUnsafeAutomaticFrames)) {
								particleSystems[i].IsDoneThread = true;
								PlaygroundParticlesC.ThreadedCalculations(particleSystems[i]);
							}
							isDoneThreadLocal=true;
						}
					});
				}
			} 
			
			// Run the globally selected thread method
			switch (threadMethod) {
			case ThreadMethod.NoThreads:
				for(int i = 0; i<particleSystems.Count; i++)
					if (particleSystems[i].isReadyForThreadedCalculations && particleSystems[i].threadMethod==ThreadMethodLocal.Inherit)
						PlaygroundParticlesC.ThreadedCalculations(particleSystems[i]);
				break;
			case ThreadMethod.OnePerSystem:
				for(int i = 0; i<particleSystems.Count; i++)
					if (particleSystems[i].threadMethod==ThreadMethodLocal.Inherit && particleSystems[i].isReadyForThreadedCalculations)
						PlaygroundParticlesC.NewCalculatedThread(particleSystems[i]);
				break;
			case ThreadMethod.OneForAll:
				if (isDoneThread) {
					isDoneThread = false;
					RunAsync(()=>{
						if (isDoneThread) return; 
						lock (locker) {
							for(int i = 0; i<particleSystems.Count; i++) 
							if (particleSystems[i].isReadyForThreadedCalculations && particleSystems[i].threadMethod==ThreadMethodLocal.Inherit) {
								PlaygroundParticlesC.ThreadedCalculations(particleSystems[i]);
							}
							isDoneThread=true;
						}
					});
				}
				break;
			case ThreadMethod.Automatic:
				if (!isFirstUnsafeAutomaticFrames) {
					maxThreads = Mathf.Clamp (maxThreads, 1, 128);
					threads = Mathf.Clamp (processorCount>1?processorCount-1:1, 1, maxThreads);
					
					// Particle systems less than processors
					if (activeSystems.Count<=threads) {
						for(int i = 0; i<particleSystems.Count; i++)
							if (particleSystems[i].isReadyForThreadedCalculations && particleSystems[i].threadMethod==ThreadMethodLocal.Inherit)
								PlaygroundParticlesC.NewCalculatedThread(particleSystems[i]);
						
						// Particle systems more than processors
					} else {
						int index = 0;
						float systemsPerBundle = (activeSystems.Count*1f/threads*1f);
						float currentSystems = 0;
						int oldSystems = 0;
						for (int i = 0; i<threads; i++) {
							currentSystems += systemsPerBundle;
							PlaygroundParticlesC[] systemBundle = new PlaygroundParticlesC[Mathf.RoundToInt(currentSystems-oldSystems)];
							oldSystems += systemBundle.Length;
							for (int s = 0; s<systemBundle.Length; s++) {
								if (index<activeSystems.Count)
									systemBundle[s] = particleSystems[activeSystems[index]];
								index++;
							}
							PlaygroundParticlesC.NewCalculatedThread(systemBundle);
						}
					}
				}
				break;
			}
			
			if (threadMethod!=ThreadMethod.NoThreads && activeThreads==0) activeThreads = 1;
			threadAggregatorRuns++;
		}
		
		#if UNITY_WSA && !UNITY_EDITOR
		/// <summary>
		/// Runs an action asynchrounously on a second thread. This overload is only accessible for Windows Store/Universal compatibility. Use a lambda expression to pass data to another thread.
		/// </summary>
		/// <param name="a">The action.</param>
		public static void RunAsync (Action a) {
			activeThreads++;
			System.Threading.Tasks.Task.Run(() => a());
		}
		
		public static void RunAsync (Action a, ThreadPoolMethod threadPoolMethod) {
			activeThreads++;
			System.Threading.Tasks.Task.Run(() => a());
		}
		#else
		/// <summary>
		/// Runs an action asynchrounously on a second thread. Use a lambda expression to pass data to another thread.
		/// </summary>
		/// <param name="a">The action.</param>
		public static void RunAsync (Action a, ThreadPoolMethod threadPoolMethod) {
			#if UNITY_WEBGL && !UNITY_EDITOR
			a();
			#elif UNITY_WSA && !UNITY_EDITOR
			lock (locker)
				ThreadPool.QueueUserWorkItem(RunAction, a);
			#else
			if (threadPoolMethod == ThreadPoolMethod.ThreadPool)
				lock (locker)
					ThreadPool.QueueUserWorkItem(RunAction, a);
			else
				RunAsyncPlaygroundPool(a);
			#endif
		}
		
		public static void RunAsync (Action a) {
			#if UNITY_WEBGL && !UNITY_EDITOR
			a();
			#elif UNITY_WSA && !UNITY_EDITOR
			lock (locker)
				ThreadPool.QueueUserWorkItem(RunAction, a);
			#else
			if (!HasReference() || reference.threadPoolMethod == ThreadPoolMethod.ThreadPool)
				lock (locker) 
					ThreadPool.QueueUserWorkItem(RunAction, a);
			else
				RunAsyncPlaygroundPool(a);
			#endif
		}
		
		public static void RunAsyncPlaygroundPool (Action a) {
			#if UNITY_WEBGL && !UNITY_EDITOR
			a();
			#else
			if (!HasPlaygroundPool())
			{
				playgroundPool = new PlaygroundQueue<Action>(HasReference()?reference.maxThreads : SystemInfo.processorCount*2, RunAction);
				hasPlaygroundPool = true;
			}
			playgroundPool.EnqueueTask(a);
			#endif
		}
		#endif
		
		private static void RunAction (object a) {
			try {
				((Action)a)();
			} catch {}
		}
		
		// Reset time when turning back to the editor
		#if UNITY_EDITOR
		public void OnApplicationPause (bool pauseStatus) {
			if (!pauseStatus && !UnityEditor.EditorApplication.isPlaying) {
				TimeReset();
				foreach(PlaygroundParticlesC p in particleSystems) {
					if (!p.isSnapshot && !p.IsLoading() && !p.IsYieldRefreshing()) {
						p.Start();
					}
				}
			}
		}
		#endif
	}
	
	#if UNITY_WSA && !UNITY_EDITOR
	#else
	/// <summary>
	/// The Playground Queue class is a self-managed thread pool which consumes actions sent into EnqueueTask(Action).
	/// </summary>
	public class PlaygroundQueue<T> : IDisposable where T : class
	{
		readonly object _locker = new object();
		private List<Thread> _workers;
		readonly Queue<T> _taskQueue = new Queue<T>();
		readonly Action<T> _dequeueAction;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PlaygroundQueue{T}"/> class.
		/// </summary>
		/// <param name="workerCount">The amount of worker threads.</param>
		/// <param name="dequeueAction">The dequeue action.</param>
		public PlaygroundQueue(int workerCount, Action<T> dequeueAction)
		{
			_dequeueAction = dequeueAction;
			_workers = new List<Thread>(workerCount);
			
			
			// Create and start a separate thread for each worker
			for (int i = 0; i<workerCount; i++)
			{
				Thread t = new Thread(Consume) { IsBackground = true, Name = string.Format("PlaygroundQueue worker {0}",i )};
				_workers.Add(t);
				t.Start();
			}
		}
		
		public bool HasPool ()
		{
			return _workers.Count>0;
		}
		
		public bool HasTasks ()
		{
			return _taskQueue.Count>0;
		}
		
		public int ThreadPoolCount ()
		{
			return _workers.Count;
		}
		
		public int TaskCount ()
		{
			return _taskQueue.Count;
		}
		
		/// <summary>
		/// Resizes the thread pool.
		/// </summary>
		/// <param name="amount">The amount of available threads in the pool.</param>
		public void SetThreadPool (int amount)
		{
			if (amount == _workers.Count)
				return;
			amount = Mathf.Clamp (amount, 1, 128);
			if (amount > _workers.Count)
			{
				for (int i = _workers.Count; i<amount; i++)
				{
					Thread t = new Thread(Consume) { IsBackground = true, Name = string.Format("PlaygroundQueue worker {0}",i )};
					_workers.Add(t);
					t.Start();
				}
			}
			else
			{
				Dispose();
				_workers = new List<Thread>(amount);
				for (int i = 0; i<amount; i++)
				{
					Thread t = new Thread(Consume) { IsBackground = true, Name = string.Format("PlaygroundQueue worker {0}",i )};
					_workers.Add(t);
					t.Start();
				}
			}
		}
		
		/// <summary>
		/// Enqueues an asynchronous task to run on a separate thread.
		/// </summary>
		/// <param name="task">The task to be run on a separate thread.</param>
		public void EnqueueTask (T task)
		{
			lock (_locker)
			{
				_taskQueue.Enqueue(task);
				Monitor.Pulse(_locker);
			}
		}
		
		/// <summary>
		/// Consumes the work in the task queue.
		/// </summary>
		void Consume ()
		{
			while (true)
			{
				T item;
				lock (_locker)
				{
					while (_taskQueue.Count == 0) 
						Monitor.Wait(_locker);
					item = _taskQueue.Dequeue();
				}
				if (item == null)
					return;
				
				// Run the action
				_dequeueAction(item);
			}
		}
		
		/// <summary>
		/// Disposes all worker threads.
		/// </summary>
		public void Dispose()
		{
			_workers.ForEach(thread => EnqueueTask(null));
			_workers.ForEach(thread => thread.Join());
		}
	}
	#endif
	
	/// <summary>
	/// Holds information about a Paint source.
	/// </summary>
	[Serializable]
	public class PaintObjectC {
		/// <summary>
		/// Position data for each origin point.
		/// </summary>
		[HideInInspector] public List<PaintPositionC> paintPositions; 	
		/// <summary>
		/// The current length of position array.
		/// </summary>
		[HideInInspector] public int positionLength;					
		/// <summary>
		/// The last painted position.
		/// </summary>
		[HideInInspector] public Vector3 lastPaintPosition;				
		/// <summary>
		/// The collider type this PaintObject sees when painting.
		/// </summary>
		[HideInInspector] public COLLISIONTYPEC collisionType;			
		/// <summary>
		/// Minimum depth for 2D collisions.
		/// </summary>
		public float minDepth = -1f;									
		/// <summary>
		/// Maximum depth for 2D collisions.
		/// </summary>
		public float maxDepth = 1f;										
		/// <summary>
		/// The required space between the last and current paint position.
		/// </summary>
		public float spacing;											
		/// <summary>
		/// The layers this PaintObject sees when painting.
		/// </summary>
		public LayerMask layerMask = -1;								
		/// <summary>
		/// The brush data for this PaintObject.
		/// </summary>
		public PlaygroundBrushC brush;									
		/// <summary>
		/// Should painting stop when paintPositions is equal to maxPositions (if false paint positions will be removed from list when painting new ones).
		/// </summary>
		public bool exceedMaxStopsPaint;								
		/// <summary>
		/// Is this PaintObject initialized yet?
		/// </summary>
		public bool initialized = false;								
		
		/// <summary>
		/// Initializes a PaintObject for painting.
		/// </summary>
		public void Initialize () {
			paintPositions = new List<PaintPositionC>();
			brush = new PlaygroundBrushC();
			lastPaintPosition = PlaygroundC.initialTargetPosition;
			initialized = true;
		}
		
		/// <summary>
		/// Live paints into this PaintObject using a Ray and color information.
		/// </summary>
		/// <param name="ray">Ray.</param>
		/// <param name="color">Color.</param>
		public bool Paint (Ray ray, Color32 color) {
			if (collisionType==COLLISIONTYPEC.Physics3D) {
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, brush.distance, layerMask)) {
					Paint(hit.point, hit.normal, hit.transform, color);
					lastPaintPosition = hit.point;
					return true;
				}
			} else {
				RaycastHit2D hitInfo2D = Physics2D.Raycast(ray.origin, ray.direction, brush.distance, layerMask, minDepth, maxDepth);
				if (hitInfo2D.collider!=null) {
					Paint(hitInfo2D.point, hitInfo2D.normal, hitInfo2D.transform, color);
					lastPaintPosition = hitInfo2D.point;
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Live paints into this PaintObject (single point with information about position, normal, parent and color), returns current painted index.
		/// </summary>
		/// <param name="pos">Position.</param>
		/// <param name="norm">Normal.</param>
		/// <param name="parent">Parent.</param>
		/// <param name="color">Color.</param>
		public int Paint (Vector3 pos, Vector3 norm, Transform parent, Color32 color) {		
			PaintPositionC pPos = new PaintPositionC();
			
			if (parent) {
				pPos.parent = parent;
				pPos.initialPosition = parent.InverseTransformPoint(pos);
				pPos.initialNormal = parent.InverseTransformDirection(norm);
			} else {
				pPos.initialPosition = pos;
				pPos.initialNormal = norm;
			}
			
			pPos.color = color;
			pPos.position = pPos.initialPosition;
			
			paintPositions.Add(pPos);
			
			positionLength = paintPositions.Count;
			if (!exceedMaxStopsPaint && positionLength>PlaygroundC.reference.paintMaxPositions) {
				paintPositions.RemoveAt(0);
				positionLength = paintPositions.Count;
			}
			
			return positionLength-1;
		}
		
		/// <summary>
		/// Removes painted positions in this PaintObject using a position and radius, returns true if position was erased.
		/// </summary>
		/// <param name="pos">Position.</param>
		/// <param name="radius">Radius.</param>
		public bool Erase (Vector3 pos, float radius) {
			bool erased = false;
			if (paintPositions.Count==0) return false;
			for (int i = 0; i<paintPositions.Count; i++) {
				if (Vector3.Distance(paintPositions[i].position, pos)<radius) {
					paintPositions.RemoveAt(i);
					erased = true;
				}
			}
			positionLength = paintPositions.Count;
			return erased;
		}
		
		/// <summary>
		/// Removes painted positions in this PaintObject using a specified index, returns true if position at index was found and removed.
		/// </summary>
		/// <param name="index">Index.</param>
		public bool Erase (int index) {
			if (index>=0 && index<positionLength) {
				paintPositions.RemoveAt(index);
				positionLength = paintPositions.Count;
				return true;
			} else return false;
		}
		
		/// <summary>
		/// Returns position at index of PaintObject's PaintPosition.
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="index">Index.</param>
		public Vector3 GetPosition (int index) {
			index=index%positionLength;
			return paintPositions[index].position;
		}
		
		/// <summary>
		/// Returns color at index of PaintObject's PaintPosition.
		/// </summary>
		/// <returns>The color.</returns>
		/// <param name="index">Index.</param>
		public Color32 GetColor (int index) {
			index=index%positionLength;
			return paintPositions[index].color;
		}
		
		/// <summary>
		/// Returns normal at index of PaintObject's PaintPosition.
		/// </summary>
		/// <returns>The normal.</returns>
		/// <param name="index">Index.</param>
		public Vector3 GetNormal (int index) {
			index=index%positionLength;
			return paintPositions[index].initialNormal;
		}
		
		/// <summary>
		/// Returns parent at index of PaintObject's PaintPosition.
		/// </summary>
		/// <returns>The parent.</returns>
		/// <param name="index">Index.</param>
		public Transform GetParent (int index) {
			index=index%positionLength;
			return paintPositions[index].parent;
		}
		
		/// <summary>
		/// Returns rotation of parent at index of PaintObject's PaintPosition.
		/// </summary>
		/// <returns>The rotation.</returns>
		/// <param name="index">Index.</param>
		public Quaternion GetRotation (int index) {
			index=index%positionLength;
			return paintPositions[index].parentRotation;
		}
		
		/// <summary>
		/// Live positioning of paintPositions regarding their parent.
		/// This happens automatically in PlaygroundParticlesC.Update() - only use this if you need to set all positions at once.
		/// </summary>
		public void Update () {
			for (int i = 0; i<paintPositions.Count; i++) {
				if (paintPositions[i].parent==null)
					continue;
				else
					Update(i);
			}
		}
		
		/// <summary>
		/// Updates specific position.
		/// </summary>
		/// <param name="thisPosition">This position.</param>
		public void Update (int thisPosition) {
			thisPosition = thisPosition%positionLength;
			paintPositions[thisPosition].position = paintPositions[thisPosition].parent.TransformPoint(paintPositions[thisPosition].initialPosition);
			paintPositions[thisPosition].parentRotation = paintPositions[thisPosition].parent.rotation;
		}
		
		/// <summary>
		/// Clears out all emission positions where the parent transform has been removed.
		/// </summary>
		public void RemoveNonParented () {
			for (int i = 0; i<paintPositions.Count; i++)
			if (paintPositions[i].parent==null) {
				paintPositions.RemoveAt(i);
				i--;
			}
		}
		
		/// <summary>
		/// Clears out emission position where the parent transform has been removed.
		/// Returns true if this position didn't have a parent.
		/// </summary>
		/// <returns><c>true</c>, if non parented was removed, <c>false</c> otherwise.</returns>
		/// <param name="thisPosition">This position.</param>
		public bool RemoveNonParented (int thisPosition) {
			thisPosition = thisPosition%positionLength;
			if (paintPositions[thisPosition].parent==null) {
				paintPositions.RemoveAt(thisPosition);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Clears out the painted positions.
		/// </summary>
		public void ClearPaint () {
			paintPositions = new List<PaintPositionC>();
			positionLength = 0;
		}
		
		/// <summary>
		/// Clones this PaintObject.
		/// </summary>
		public PaintObjectC Clone () {
			PaintObjectC paintObject = new PaintObjectC();
			if (paintPositions!=null && paintPositions.Count>0) {
				paintObject.paintPositions = new List<PaintPositionC>();
				paintObject.paintPositions.AddRange(paintPositions);
			}
			paintObject.positionLength = positionLength;
			paintObject.lastPaintPosition = lastPaintPosition;
			paintObject.spacing = spacing;
			paintObject.layerMask = layerMask;
			paintObject.collisionType = collisionType;
			if (brush!=null)
				paintObject.brush = brush.Clone();
			else
				paintObject.brush = new PlaygroundBrushC();
			paintObject.exceedMaxStopsPaint = exceedMaxStopsPaint;
			paintObject.initialized = initialized;
			return paintObject;
		}
	}
	
	/// <summary>
	/// Constructor for a painted position.
	/// </summary>
	[Serializable]
	public class PaintPositionC {
		/// <summary>
		/// Emission spot in local position of parent.
		/// </summary>
		public Vector3 position;								
		/// <summary>
		/// Color of emission spot.
		/// </summary>
		public Color32 color;									
		/// <summary>
		/// The parent transform.
		/// </summary>
		public Transform parent;								
		/// <summary>
		/// The first position where this originally were painted.
		/// </summary>
		public Vector3 initialPosition;							
		/// <summary>
		/// The first normal direction when painted.
		/// </summary>
		public Vector3 initialNormal;							
		/// <summary>
		/// The rotation of the parent.
		/// </summary>
		public Quaternion parentRotation;						
	}
	
	/// <summary>
	/// Holds information about a brush used for source painting.
	/// </summary>
	[Serializable]
	public class PlaygroundBrushC {
		/// <summary>
		/// The texture to construct this Brush from.
		/// </summary>
		public Texture2D texture;								
		/// <summary>
		/// The scale of this Brush (measured in Units).
		/// </summary>
		public float scale = 1f;								
		/// <summary>
		/// The detail level of this brush.
		/// </summary>
		public BRUSHDETAILC detail = BRUSHDETAILC.High;			
		/// <summary>
		/// The distance the brush reaches.
		/// </summary>
		public float distance = 10000;							
		/// <summary>
		/// Color data of this brush.
		/// </summary>
		[HideInInspector] public Color32[] color;				
		/// <summary>
		/// The length of color array.
		/// </summary>
		[HideInInspector] public int colorLength;				
		
		/// <summary>
		/// Sets the texture of this brush.
		/// </summary>
		/// <param name="newTexture">New texture.</param>
		public void SetTexture (Texture2D newTexture) {
			texture = newTexture;
			Construct();
		}
		
		/// <summary>
		/// Caches the color information from this brush.
		/// </summary>
		public void Construct () {
			color = PlaygroundC.GetPixels(texture);
			if (color!=null)
				colorLength = color.Length;
		}
		
		/// <summary>
		/// Returns color at index of Brush.
		/// </summary>
		/// <returns>The color.</returns>
		/// <param name="index">Index.</param>
		public Color32 GetColor (int index) {
			index=index%colorLength;
			return color[index];
		}
		
		/// <summary>
		/// Sets color at index of Brush.
		/// </summary>
		/// <param name="c">Color.</param>
		/// <param name="index">Index.</param>
		public void SetColor (Color32 c, int index) {
			color[index] = c;
		}
		
		/// <summary>
		/// Clones this PlaygroundBrush.
		/// </summary>
		public PlaygroundBrushC Clone () {
			PlaygroundBrushC playgroundBrush = new PlaygroundBrushC();
			playgroundBrush.texture = texture;
			playgroundBrush.scale = scale;
			playgroundBrush.detail = detail;
			playgroundBrush.distance = distance;
			if (playgroundBrush.color!=null)
				playgroundBrush.color = color.Clone() as Color32[];
			playgroundBrush.colorLength = colorLength;
			return playgroundBrush;
		}
	}
	
	/// <summary>
	/// Holds information about a State source.
	/// </summary>
	[Serializable]
	public class ParticleStateC {
		/// <summary>
		/// Color data.
		/// </summary>
		private Color32[] color; 						
		/// <summary>
		/// Position data.
		/// </summary>
		private Vector3[] position;					
		/// <summary>
		/// Normal data.
		/// </summary>
		private Vector3[] normals;					
		/// <summary>
		/// The texture to construct this state from (used to color each vertex if mesh is used).
		/// </summary>
		public Texture2D stateTexture;							
		/// <summary>
		/// The texture to use as depthmap for this state. A grayscale image of the same size as stateTexture is required.
		/// </summary>
		public Texture2D stateDepthmap;							
		/// <summary>
		/// How much the grayscale from stateDepthmap will affect z-value.
		/// </summary>
		public float stateDepthmapStrength = 1f;
		/// <summary>
		/// The mesh used to set this state's positions. Positions will be calculated per vertex.
		/// </summary>
		public Mesh stateMesh;									
		/// <summary>
		/// The name of this state.
		/// </summary>
		public string stateName;								
		/// <summary>
		/// The scale of this state (measured in units).
		/// </summary>
		public float stateScale = 1f;							
		/// <summary>
		/// The offset of this state in world position (measured in units).
		/// </summary>
		public Vector3 stateOffset;								
		/// <summary>
		/// The transform that act as parent to this state.
		/// </summary>
		public Transform stateTransform;
		public Matrix4x4 stateTransformMx = new Matrix4x4();
		public ScaleMethod stateScaleMethod;
		/// <summary>
		/// Is this ParticleState intialized?
		/// </summary>
		[NonSerialized] public bool initialized = false; 		
		/// <summary>
		/// The length of color array.
		/// </summary>
		[HideInInspector] public int colorLength;				
		/// <summary>
		/// The length of position array.
		/// </summary>
		[HideInInspector] public int positionLength;
		[HideInInspector] public bool applyChromaKey;
		[HideInInspector] public Color32 chromaKey;
		[HideInInspector] public float chromaKeySpread;
		bool isInitializing = false;
		
		/// <summary>
		/// Determines whether this State is initializing. This is used for multithreading to determine if a State's Initialize() function is being executed.
		/// </summary>
		/// <returns><c>true</c> if this State is initializing; otherwise, <c>false</c>.</returns>
		public bool IsInitializing () {
			return isInitializing;
		}
		
		/// <summary>
		/// Initializes a ParticleState for construction.
		/// </summary>
		public void Initialize () {
			isInitializing = true;
			if (stateMesh==null && stateTexture!=null) {
				ConstructParticles(stateTexture, stateScale, stateOffset, stateName, stateTransform);
			} else if (stateMesh!=null && stateTexture!=null) {
				ConstructParticles(stateMesh, stateTexture, stateScale, stateOffset, stateName, stateTransform);
			} else if (stateMesh!=null && stateTexture==null) {
				ConstructParticles(stateMesh, stateScale, stateOffset, stateName, stateTransform);
			} else Debug.Log("State Texture or State Mesh returned null. Please assign an object to State: "+stateName+".");
			initialized = position!=null && position.Length>0;
			isInitializing = false;
		}
		
		public void UpdateMatrix (bool isLocal) {
			stateTransformMx.SetTRS (isLocal?Vector3.zero:stateTransform.position, isLocal?Quaternion.identity:stateTransform.rotation, stateScaleMethod == ScaleMethod.Local? stateTransform.localScale : stateTransform.lossyScale);
		}
		
		/// <summary>
		/// Constructs image data.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="newStateName">New state name.</param>
		/// <param name="newStateTransform">New state transform.</param>
		public void ConstructParticles (Texture2D image, float scale, Vector3 offset, string newStateName, Transform newStateTransform) {
			List<Color32> tmpColor = new List<Color32>();
			List<Vector3> tmpPosition = new List<Vector3>();
			color = PlaygroundC.GetPixels(image);
			bool readAlpha = false;
			if (PlaygroundC.reference)
				readAlpha = PlaygroundC.reference.buildZeroAlphaPixels;
			bool readDepthmap = stateDepthmap!=null;
			Color32[] depthMapPixels = readDepthmap?PlaygroundC.GetPixels(stateDepthmap):null;
			int x = 0;
			int y = 0;
			float depthmapRatio = stateDepthmap!=null?((depthMapPixels.Length*1f) / (color.Length*1f)):0;
			byte cSpread = (byte)(chromaKeySpread*255f);
			for (int i = 0; i<color.Length; i++) {
				if (readAlpha || color[i].a!=0) {
					if (!applyChromaKey || applyChromaKey && 
					    ((color[i].r<chromaKey.r-cSpread || color[i].r>chromaKey.r+cSpread) &&
					 (color[i].g<chromaKey.g-cSpread || color[i].g>chromaKey.g+cSpread) &&
					 (color[i].b<chromaKey.b-cSpread || color[i].b>chromaKey.b+cSpread)))
					{
						tmpColor.Add(color[i]);
						if (readDepthmap) {
							
							int iScaled = Mathf.FloorToInt (((i*1f) * depthmapRatio));
							float z = ((((depthMapPixels[iScaled].r*1f)+(depthMapPixels[iScaled].g*1f)+(depthMapPixels[iScaled].b*1f))/3)*((depthMapPixels[iScaled].a*1f)/255)*stateDepthmapStrength)/255;
							tmpPosition.Add(new Vector3(x,y,z));
						} else tmpPosition.Add(new Vector3(x,y,0));
					}
				}
				x++; x=x%image.width;
				if (x==0 && i!=0) y++;
			}
			color = tmpColor.ToArray() as Color32[];
			position = tmpPosition.ToArray() as Vector3[];
			tmpColor = null;
			tmpPosition = null;
			normals = new Vector3[position.Length];
			for (int n = 0; n<normals.Length; n++) normals[n] = Vector3.forward;
			stateTransform = newStateTransform;
			stateTexture = image;
			colorLength = color.Length;
			positionLength = position.Length;
			stateScale = scale;
			stateOffset = offset;
			stateName = newStateName;
			initialized = position.Length>0;
		}
		
		/// <summary>
		/// Constructs Mesh data with texture.
		/// </summary>
		/// <param name="mesh">Mesh.</param>
		/// <param name="texture">Texture.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="newStateName">New state name.</param>
		/// <param name="newStateTransform">New state transform.</param>
		public void ConstructParticles (Mesh mesh, Texture2D texture, float scale, Vector3 offset, string newStateName, Transform newStateTransform) {
			position = mesh.vertices;
			normals = mesh.normals;
			if (normals==null || normals.Length==0) {
				normals = new Vector3[position.Length];
				for (int n = 0; n<normals.Length; n++) normals[n] = Vector3.forward;
			}
			Vector2[] uvs = mesh.uv;
			color = new Color32[uvs.Length];
			for (int i = 0; i<position.Length; i++)
				color[i] = texture.GetPixelBilinear(uvs[i].x, uvs[i].y);
			stateMesh = mesh;
			stateTransform = newStateTransform;
			colorLength = color.Length;
			positionLength = position.Length;
			stateScale = scale;
			stateOffset = offset;
			stateName = newStateName;
			initialized = position.Length>0;
		}
		
		/// <summary>
		/// Constructs Mesh data without texture.
		/// </summary>
		/// <param name="mesh">Mesh.</param>
		/// <param name="scale">Scale.</param>
		/// <param name="offset">Offset.</param>
		/// <param name="newStateName">New state name.</param>
		/// <param name="newStateTransform">New state transform.</param>
		public void ConstructParticles (Mesh mesh, float scale, Vector3 offset, string newStateName, Transform newStateTransform) {
			position = mesh.vertices;
			normals = mesh.normals;
			if (normals==null || normals.Length==0) {
				normals = new Vector3[position.Length];
				for (int n = 0; n<normals.Length; n++) normals[n] = Vector3.forward;
			}
			Vector2[] uvs = mesh.uv;
			color = new Color32[uvs.Length];
			stateMesh = mesh;
			stateTransform = newStateTransform;
			colorLength = color.Length;
			positionLength = position.Length;
			stateScale = scale;
			stateOffset = offset;
			stateName = newStateName;
			initialized = position.Length>0;
		}
		
		/// <summary>
		/// Returns color at index of ParticleState.
		/// </summary>
		/// <returns>The color.</returns>
		/// <param name="index">Index.</param>
		public Color32 GetColor (int index) {
			if (colorLength==0) return new Color32();
			index=index%colorLength;
			return color[index];
		}
		
		/// <summary>
		/// Returns position at index of ParticleState.
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="index">Index.</param>
		public Vector3 GetPosition (int index) {
			if (positionLength==0) return new Vector3();
			index=index%positionLength;
			return (position[index]+stateOffset)*stateScale;
		}
		
		public Vector3 GetLocalPosition (int index) {
			if (positionLength==0) return new Vector3();
			index=index%positionLength;
			return stateTransformMx.MultiplyPoint3x4((position[index]+stateOffset)*stateScale);
		}
		
		/// <summary>
		/// Returns normal at index of ParticleState.
		/// </summary>
		/// <returns>The normal.</returns>
		/// <param name="index">Index.</param>
		public Vector3 GetNormal (int index) {
			if (positionLength==0) return new Vector3();
			index=index%positionLength;
			return normals[index];
		}
		
		public Vector3 GetLocalNormal (int index) {
			if (positionLength==0) return new Vector3();
			index=index%positionLength;
			return stateTransformMx.MultiplyPoint3x4(normals[index]);
		}
		
		/// <summary>
		/// Returns colors in ParticleState.
		/// </summary>
		/// <returns>The colors.</returns>
		public Color32[] GetColors () {
			return color.Clone() as Color32[];
		}
		
		/// <summary>
		/// Returns positions in ParticleState.
		/// </summary>
		/// <returns>The positions.</returns>
		public Vector3[] GetPositions () {
			return position.Clone() as Vector3[];
		}
		
		/// <summary>
		/// Returns normals in ParticleState.
		/// </summary>
		/// <returns>The normals.</returns>
		public Vector3[] GetNormals () {
			return normals.Clone() as Vector3[];
		}
		
		/// <summary>
		/// Sets color at index of ParticleState.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="c">C.</param>
		public void SetColor (int index, Color32 c) {
			color[index] = c;
		}
		
		/// <summary>
		/// Sets position at index of ParticleState.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="v">V.</param>
		public void SetPosition (int index, Vector3 v) {
			position[index] = v;
		}
		
		/// <summary>
		/// Sets normal at index of ParticleState.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="v">V.</param>
		public void SetNormal (int index, Vector3 v) {
			normals[index] = v;
		}
		
		/// <summary>
		/// Returns position from parent's TransformPoint (can only be called from main-thread).
		/// </summary>
		/// <returns>The parented position.</returns>
		/// <param name="thisPosition">This position.</param>
		public Vector3 GetParentedPosition (int thisPosition) {
			thisPosition = thisPosition%positionLength;
			return stateTransform.TransformPoint((position[thisPosition]+stateOffset)*stateScale);
		}
		
		/// <summary>
		/// Returns a copy of this ParticleState.
		/// </summary>
		public ParticleStateC Clone () {
			ParticleStateC particleState = new ParticleStateC();
			particleState.stateName = stateName;
			particleState.stateScale = stateScale;
			particleState.stateTexture = stateTexture;
			particleState.stateDepthmap = stateDepthmap;
			particleState.stateDepthmapStrength = stateDepthmapStrength;
			particleState.stateMesh = stateMesh;
			particleState.stateOffset = stateOffset;
			particleState.colorLength = colorLength;
			particleState.positionLength = positionLength;
			particleState.stateTransform = stateTransform;
			particleState.applyChromaKey = applyChromaKey;
			particleState.chromaKey = chromaKey;
			particleState.chromaKeySpread = chromaKeySpread;
			return particleState;
		}
	}
	
	/// <summary>
	/// Holds information for Source Projection.
	/// </summary>
	[Serializable]
	public class ParticleProjectionC {
		/// <summary>
		/// Color data.
		/// </summary>
		[HideInInspector] private Color32[] sourceColors;					
		/// <summary>
		/// Position data.
		/// </summary>
		[HideInInspector] private Vector3[] sourcePositions;				
		/// <summary>
		/// Projected position data.
		/// </summary>
		[HideInInspector] private Vector3[] targetPositions;			
		/// <summary>
		/// Projected normal data.
		/// </summary>
		[HideInInspector] private Vector3[] targetNormals;				
		/// <summary>
		/// Projection has hit surface at point.
		/// </summary>
		[HideInInspector] private bool[] hasProjected;						
		/// <summary>
		/// Projected transforms.
		/// </summary>
		[HideInInspector] private Transform[] targetParents;				
		/// <summary>
		/// The texture to project.
		/// </summary>
		public Texture2D projectionTexture;									
		/// <summary>
		/// The origin offset in Units.
		/// </summary>
		public Vector2 projectionOrigin; 									
		/// <summary>
		/// Transform to project from.
		/// </summary>
		public Transform projectionTransform;								
		/// <summary>
		/// Matrix to project from.
		/// </summary>
		public Matrix4x4 projectionMatrix;									
		/// <summary>
		/// Position of projection source.
		/// </summary>
		public Vector3 projectionPosition;									
		/// <summary>
		/// Direction of projection source.
		/// </summary>
		public Vector3 projectionDirection;									
		/// <summary>
		/// Rotation of projection source.
		/// </summary>
		public Quaternion projectionRotation;								
		/// <summary>
		/// The distance in Units the projection travels.
		/// </summary>
		public float projectionDistance = 1000f;							
		/// <summary>
		/// The scale of projection in Units.
		/// </summary>
		public float projectionScale = .1f;									
		/// <summary>
		/// Layers seen by projection.
		/// </summary>
		public LayerMask projectionMask = -1;								
		/// <summary>
		/// Determines if 3d- or 2d colliders are seen by projection.
		/// </summary>
		public COLLISIONTYPEC collisionType;								
		/// <summary>
		/// Minimum depth of 2d colliders seen by projection.
		/// </summary>
		public float minDepth = -1f;										
		/// <summary>
		/// Maximum depth of 2d colliders seen by projection.
		/// </summary>
		public float maxDepth = 1f;											
		/// <summary>
		/// The offset from projected surface.
		/// </summary>
		public float surfaceOffset = 0f;									
		/// <summary>
		/// Update this projector each frame.
		/// </summary>
		public bool liveUpdate = true;										
		/// <summary>
		/// Is this projector finished refreshing?
		/// </summary>
		public bool hasRefreshed = false;									
		/// <summary>
		/// Is this projector ready?
		/// </summary>
		[HideInInspector] public bool initialized = false;					
		/// <summary>
		/// The length of color array.
		/// </summary>
		[HideInInspector] public int colorLength;							
		/// <summary>
		/// The length of position array.
		/// </summary>
		[HideInInspector] public int positionLength;						
		
		/// <summary>
		/// Initializes this ParticleProjection object.
		/// </summary>
		public void Initialize () {
			if (!projectionTexture) return;
			Construct(projectionTexture, projectionTransform);
			if (!liveUpdate) {
				UpdateSource();
				Update();
			}
			initialized = true;
			hasRefreshed = false;
		}
		
		/// <summary>
		/// Builds source data.
		/// </summary>
		/// <param name="image">Image.</param>
		/// <param name="transform">Transform.</param>
		public void Construct (Texture2D image, Transform transform) {
			List<Color32> tmpColor = new List<Color32>();
			List<Vector3> tmpPosition = new List<Vector3>();
			sourceColors = PlaygroundC.GetPixels(image);
			bool readAlpha = false;
			if (PlaygroundC.reference)
				readAlpha = PlaygroundC.reference.buildZeroAlphaPixels;
			
			int x = 0;
			int y = 0;
			for (int i = 0; i<sourceColors.Length; i++) {
				if (readAlpha || sourceColors[i].a!=0) {
					tmpColor.Add(sourceColors[i]);
					tmpPosition.Add(new Vector3(x*projectionScale,y*projectionScale,0));			
				}
				x++; x=x%image.width;
				if (x==0 && i!=0) y++;
			}
			sourceColors = tmpColor.ToArray();
			sourcePositions = tmpPosition.ToArray();
			tmpColor = null;
			tmpPosition = null;
			targetPositions = new Vector3[sourcePositions.Length];
			targetNormals = new Vector3[sourcePositions.Length];
			targetParents = new Transform[sourcePositions.Length];
			hasProjected = new bool[sourcePositions.Length];
			positionLength = sourcePositions.Length;
			colorLength = sourceColors.Length;
			projectionTexture = image;
			projectionTransform = transform;
			projectionMatrix = new Matrix4x4();
		}
		
		/// <summary>
		/// Updates source matrix.
		/// </summary>
		public void UpdateSource () {
			projectionPosition = projectionTransform.position;
			projectionDirection = projectionTransform.forward;
			projectionRotation = projectionTransform.rotation;
			projectionMatrix.SetTRS(projectionPosition, projectionRotation, projectionTransform.localScale);
		}
		
		/// <summary>
		/// Projects all particle sources (only call this if you need to set all particles at once).
		/// </summary>
		public void Update () {
			for (int i = 0; i<positionLength; i++)
				Update(i);
		}
		
		/// <summary>
		/// Projects a single particle source position.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Update (int index) {
			index=index%positionLength;
			Vector3 sourcePosition = projectionMatrix.MultiplyPoint3x4(sourcePositions[index]+new Vector3(projectionOrigin.x, projectionOrigin.y, 0));
			if (collisionType==COLLISIONTYPEC.Physics3D) {
				RaycastHit hit;
				if (Physics.Raycast(sourcePosition, projectionDirection, out hit, projectionDistance, projectionMask)) {
					targetPositions[index] = hit.point+(hit.normal*surfaceOffset);
					targetNormals[index] = hit.normal;
					targetParents[index] = hit.transform;
					hasProjected[index] = true;
				} else {
					targetPositions[index] = PlaygroundC.initialTargetPosition;
					targetNormals[index] = Vector3.forward;
					hasProjected[index] = false;
					targetParents[index] = null;
				}
			} else {
				RaycastHit2D hit2d = Physics2D.Raycast (sourcePosition, projectionDirection, projectionDistance, projectionMask, minDepth, maxDepth);
				if (hit2d.collider!=null) {
					targetPositions[index] = hit2d.point+(hit2d.normal*surfaceOffset);
					targetNormals[index] = hit2d.normal;
					targetParents[index] = hit2d.transform;
					hasProjected[index] = true;
				} else {
					targetPositions[index] = PlaygroundC.initialTargetPosition;
					targetNormals[index] = Vector3.forward;
					hasProjected[index] = false;
					targetParents[index] = null;
				}
			}
		}
		
		/// <summary>
		/// Returns color at index of ParticleProjection.
		/// </summary>
		/// <returns>The color.</returns>
		/// <param name="index">Index.</param>
		public Color32 GetColor (int index) {
			index=index%colorLength;
			return sourceColors[index];
		}
		
		/// <summary>
		/// Returns position at index of ParticleProjection.
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="index">Index.</param>
		public Vector3 GetPosition (int index) {
			index=index%positionLength;
			return (targetPositions[index]);
		}
		
		/// <summary>
		/// Returns normal at index of ParticleProjection.
		/// </summary>
		/// <returns>The normal.</returns>
		/// <param name="index">Index.</param>
		public Vector3 GetNormal (int index) {
			index=index%positionLength;
			return targetNormals[index];
		}
		
		/// <summary>
		/// Returns parent at index of ParticleProjection.
		/// </summary>
		/// <returns>The parent.</returns>
		/// <param name="index">Index.</param>
		public Transform GetParent (int index) {
			index=index%positionLength;
			return targetParents[index];
		}
		
		/// <summary>
		/// Returns projection status at index of ParticleProjection.
		/// </summary>
		/// <returns><c>true</c> if this instance has projection the specified index; otherwise, <c>false</c>.</returns>
		/// <param name="index">Index.</param>
		public bool HasProjection (int index) {
			index=index%positionLength;
			return hasProjected[index];
		}
		
		/// <summary>
		/// Returns a copy of this ParticleProjectionC object.
		/// </summary>
		public ParticleProjectionC Clone () {
			ParticleProjectionC particleProjection 	= new ParticleProjectionC();
			if (sourceColors!=null)
				particleProjection.sourceColors		= (Color32[])sourceColors.Clone ();
			if (sourcePositions!=null)
				particleProjection.sourcePositions	= (Vector3[])sourcePositions.Clone ();
			if (targetPositions!=null)
				particleProjection.targetPositions	= (Vector3[])targetPositions.Clone ();
			if (targetNormals!=null)
				particleProjection.targetNormals	= (Vector3[])targetNormals.Clone ();
			if (hasProjected!=null)
				particleProjection.hasProjected		= (bool[])hasProjected.Clone ();
			if (targetParents!=null)
				particleProjection.targetParents	= (Transform[])targetParents.Clone ();
			particleProjection.projectionTexture	= projectionTexture;
			particleProjection.projectionOrigin		= projectionOrigin;
			particleProjection.projectionTransform	= projectionTransform;
			particleProjection.projectionMatrix		= projectionMatrix;
			particleProjection.projectionPosition	= projectionPosition;
			particleProjection.projectionDirection	= projectionDirection;
			particleProjection.projectionRotation	= projectionRotation;
			particleProjection.projectionDistance	= projectionDistance;
			particleProjection.projectionScale		= projectionScale;
			particleProjection.projectionMask		= projectionMask;
			particleProjection.collisionType		= collisionType;
			particleProjection.minDepth				= minDepth;
			particleProjection.maxDepth				= maxDepth;
			particleProjection.surfaceOffset		= surfaceOffset;
			particleProjection.liveUpdate			= liveUpdate;
			particleProjection.hasRefreshed			= hasRefreshed;
			particleProjection.initialized			= initialized;
			particleProjection.colorLength			= colorLength;
			particleProjection.positionLength		= positionLength;
			return particleProjection;
		}
	}
	
	/// <summary>
	/// Holds AnimationCurves in X, Y and Z variables.
	/// </summary>
	[Serializable]
	public class Vector3AnimationCurveC {
		public AnimationCurve x;							// AnimationCurve for X-axis
		public AnimationCurve y;							// AnimationCurve for Y-axis
		public AnimationCurve z;							// AnimationCurve for Z-axis
		
		public float xRepeat = 1f;							// Repetition on X
		public float yRepeat = 1f;							// Repetition on Y
		public float zRepeat = 1f;							// Repetition on Z
		
		/// <summary>
		/// Evaluate the specified time.
		/// </summary>
		/// <param name="time">Time.</param>
		public Vector3 Evaluate (float time) {
			return Evaluate (time, 1f);
		}
		
		/// <summary>
		/// Evaluates the specified time and apply scale.
		/// </summary>
		/// <param name="time">Time.</param>
		/// <param name="scale">Scale.</param>
		public Vector3 Evaluate (float time, float scale) {
			return new Vector3(x.Evaluate(time*(xRepeat)%1f), y.Evaluate(time*(yRepeat)%1f), z.Evaluate(time*(zRepeat)%1f))*scale;
		}
		
		/// <summary>
		/// Determines whether this Vector3AnimationCurveC has keys in X, Y or Z AnimationCurves.
		/// </summary>
		/// <returns><c>true</c> if this instance has keys; otherwise, <c>false</c>.</returns>
		public bool HasKeys () {
			CheckNull();
			return x.keys.Length>0||y.keys.Length>0||z.keys.Length>0;
		}
		
		/// <summary>
		/// Sets the key values.
		/// </summary>
		/// <param name="key">X, Y and Z key.</param>
		/// <param name="value">Value as float.</param>
		public void SetKeyValues (int key, float value) {
			SetKeyValues(key, new Vector3(value, value, value), 0, 0);
		}
		
		/// <summary>
		/// Sets the key values.
		/// </summary>
		/// <param name="key">X, Y and Z key.</param>
		/// <param name="value">Value as Vector3.</param>
		public void SetKeyValues (int key, Vector3 value, float inTangent, float outTangent) {
			CheckNull();
			Keyframe[] xKeys = x.keys;
			Keyframe[] yKeys = y.keys;
			Keyframe[] zKeys = z.keys;
			if (key<x.keys.Length) {
				xKeys[key].value = value.x;
				xKeys[key].inTangent = inTangent;
				xKeys[key].outTangent = outTangent;
			}
			if (key<y.keys.Length) {
				yKeys[key].value = value.y;
				yKeys[key].inTangent = inTangent;
				yKeys[key].outTangent = outTangent;
			}
			if (key<z.keys.Length) {
				zKeys[key].value = value.z;
				zKeys[key].inTangent = inTangent;
				zKeys[key].outTangent = outTangent;
			}
			x.keys = xKeys;
			y.keys = yKeys;
			z.keys = zKeys;
		}
		
		/// <summary>
		/// Resets this instance with value of 0.
		/// </summary>
		public void Reset () {
			CheckNull();
			Keyframe[] reset = new Keyframe[2];
			reset[0].time = 0;
			reset[1].time = 1f;
			x.keys = reset;
			y.keys = reset;
			z.keys = reset;
			xRepeat = 1f;
			yRepeat = 1f;
			zRepeat = 1f;
		}
		
		/// <summary>
		/// Resets this instance with value of 1.
		/// </summary>
		public void Reset1 () {
			CheckNull();
			Keyframe[] reset = new Keyframe[2];
			reset[0].time = 0;
			reset[0].value = 1f;
			reset[1].time = 1f;
			reset[1].value = 1f;
			x.keys = reset;
			y.keys = reset;
			z.keys = reset;
			xRepeat = 1f;
			yRepeat = 1f;
			zRepeat = 1f;
		}
		
		/// <summary>
		/// Resets this instance with three keyframes.
		/// </summary>
		public void ResetWithMidKey () {
			CheckNull();
			Keyframe[] reset = new Keyframe[3];
			reset[0].time = 0;
			reset[1].time = .5f;
			reset[2].time = 1f;
			x.keys = reset;
			y.keys = reset;
			z.keys = reset;
			xRepeat = 1f;
			yRepeat = 1f;
			zRepeat = 1f;
		}
		
		public void CheckNull () {
			if (x==null) x = new AnimationCurve();
			if (y==null) y = new AnimationCurve();
			if (z==null) z = new AnimationCurve();
		}
		
		/// <summary>
		/// Returns a copy of this Vector3AnimationCurve.
		/// </summary>
		public Vector3AnimationCurveC Clone () {
			Vector3AnimationCurveC vector3AnimationCurveClone = new Vector3AnimationCurveC();
			vector3AnimationCurveClone.x = new AnimationCurve(x.keys);
			vector3AnimationCurveClone.y = new AnimationCurve(y.keys);
			vector3AnimationCurveClone.z = new AnimationCurve(z.keys);
			vector3AnimationCurveClone.xRepeat = xRepeat;
			vector3AnimationCurveClone.yRepeat = yRepeat;
			vector3AnimationCurveClone.zRepeat = zRepeat;
			return vector3AnimationCurveClone;
		}
	}
	
	/// <summary>
	/// Extended class for World Objects and Skinned World Objects.
	/// </summary>
	[Serializable]
	public class WorldObjectBaseC {
		/// <summary>
		/// The GameObject of this World Object.
		/// </summary>
		public GameObject gameObject;
		/// <summary>
		/// The Transform of this World Object.
		/// </summary>
		[HideInInspector] public Transform transform;
		/// <summary>
		/// The Rigidbody of this World Object.
		/// </summary>
		[HideInInspector] public Rigidbody rigidbody;
		/// <summary>
		/// The mesh filter of this World Object (will be null for skinned meshes).
		/// </summary>
		[HideInInspector] public MeshFilter meshFilter;
		/// <summary>
		/// The mesh of this World Object.
		/// </summary>
		[HideInInspector] public Mesh mesh;
		/// <summary>
		/// The vertices of this World Object.
		/// </summary>
		[HideInInspector] public Vector3[] vertexPositions;
		/// <summary>
		/// The normals of this World Object.
		/// </summary>
		[HideInInspector] public Vector3[] normals;
		/// <summary>
		/// Determines if normals should update.
		/// </summary>
		[HideInInspector] public bool updateNormals = false;
		/// <summary>
		/// The id of this World Object (used to keep track when this object changes).
		/// </summary>
		[NonSerialized] public int cachedId;
		/// <summary>
		/// Determines if this World Object is initialized.
		/// </summary>
		[NonSerialized] public bool initialized = false;
		/// <summary>
		/// The transform matrix of this World Object. All source positions is calculated based on this matrix.
		/// </summary>
		[HideInInspector] public Matrix4x4 transformMatrix = new Matrix4x4();
		/// <summary>
		/// The scale method to use when updating the transform matrix.
		/// </summary>
		[HideInInspector] public ScaleMethod scaleMethod;
		
		public void UpdateMatrix (bool isLocal) {
			transformMatrix.SetTRS(isLocal?Vector3.zero:transform.position, isLocal?Quaternion.identity:transform.rotation, scaleMethod == ScaleMethod.Local? transform.localScale : transform.lossyScale);
		}
	}
	
	/// <summary>
	/// Holds information about a World object.
	/// </summary>
	[Serializable]
	public class WorldObject : WorldObjectBaseC {
		[HideInInspector] public Renderer renderer;
		
		/// <summary>
		/// Initializes this WorldObject and prepares it for extracting the mesh data.
		/// </summary>
		public void Initialize () {
			gameObject = transform.gameObject;
			rigidbody = gameObject.GetComponent<Rigidbody>();
			renderer = transform.GetComponentInChildren<Renderer>();
			meshFilter = transform.GetComponentInChildren<MeshFilter>();
			if (meshFilter!=null) {
				mesh = meshFilter.sharedMesh;
				if (mesh!=null) {
					vertexPositions = mesh.vertices;
					normals = mesh.normals;
					initialized = true;
				} else {
					initialized = false;
				}
			}
			cachedId = gameObject.GetInstanceID();
		}
		
		/// <summary>
		/// Updates this WorldObject.
		/// </summary>
		public void Update () {
			if (mesh!=null) {
				vertexPositions = mesh.vertices;
				if (updateNormals)
					normals = mesh.normals;
			}
		}
		
		/// <summary>
		/// Returns a copy of this WorldObject.
		/// </summary>
		public WorldObject Clone () {
			WorldObject worldObject = new WorldObject();
			worldObject.gameObject = gameObject;
			worldObject.transform = transform;
			worldObject.rigidbody = rigidbody;
			worldObject.renderer = renderer;
			worldObject.meshFilter = meshFilter;
			worldObject.mesh = mesh;
			worldObject.updateNormals = updateNormals;
			return worldObject;
		}
	}
	
	/// <summary>
	/// Holds information about a Skinned world object. Consider to NOT use Optimize Game Objects at the skinned mesh's Import Settings for best performance of extracting its vertices.
	/// </summary>
	[Serializable]
	public class SkinnedWorldObject : WorldObjectBaseC {
		/// <summary>
		/// Downresolution will skip vertices to set fewer target positions in a SkinnedWorldObject.
		/// </summary>
		public int downResolution = 1;							
		/// <summary>
		/// The renderer of this SkinnedWorldObject.
		/// </summary>
		[HideInInspector] public SkinnedMeshRenderer renderer;	
		/// <summary>
		/// The bones of this SkinnedWorldObject.
		/// </summary>
		private Transform[] boneTransforms;						
		/// <summary>
		/// The weights of this SkinnedWorldObject.
		/// </summary>
		private BoneWeight[] weights;							
		/// <summary>
		/// The binding poses of this SkinnedWorldObject.
		/// </summary>
		private Matrix4x4[] bindPoses;							
		/// <summary>
		/// The bone matrices of this SkinnedWorldObject.
		/// </summary>
		private Matrix4x4[] boneMatrices;						
		/// <summary>
		/// The calculated world vertices of this SkinnedWorldObject.
		/// </summary>
		private Vector3[] vertices;								
		/// <summary>
		/// The local vertices to calculate upon of this SkinnedWorldObject.
		/// </summary>
		private Vector3[] localVertices;
		/// <summary>
		/// Determines if the thread for this skinned world object is ready. This is used when Skinned Mesh Thread Method is set to One Per System.
		/// </summary>
		[HideInInspector] public bool isDoneThread = true;
		
		private Mesh bakedMesh;
		private Matrix4x4 vertexMatrix = new Matrix4x4();
		
		/// <summary>
		/// Initializes this Skinned World Object and prepares it for extracting the mesh data.
		/// </summary>
		public void Initialize () {
			gameObject = transform.gameObject;
			cachedId = gameObject.GetInstanceID();
			rigidbody = gameObject.GetComponent<Rigidbody>();
			renderer = transform.GetComponentInChildren<SkinnedMeshRenderer>();
			mesh = renderer.sharedMesh;
			if (mesh!=null) {
				normals = mesh.normals;
				vertices = new Vector3[mesh.vertices.Length];
				localVertices = mesh.vertices;
				weights = mesh.boneWeights;
				boneTransforms = renderer.bones;
				bindPoses = mesh.bindposes;
				boneMatrices = new Matrix4x4[boneTransforms.Length];
				vertexPositions = vertices;
				initialized = true;
				Update();
			} else {
				Debug.Log ("No mesh could be found in the assigned Skinned World Object. Make sure a mesh is assigned to your Skinned Mesh Renderer, if so - make sure the hierarchy under only have one Skinned Mesh Renderer component.", gameObject);
			}
		}
		
		/// <summary>
		/// Updates the mesh.
		/// </summary>
		public void MeshUpdate () {
			if (initialized) {
				if (boneMatrices!=null && boneMatrices.Length>0)
					localVertices = mesh.vertices;
				if (updateNormals)
					normals = mesh.normals;
			}
		}
		
		/// <summary>
		/// Updates the bones. If no bones are found (using Optimize Game Objects) a snapshot of the skinned mesh will be computed instead. Enabling Optimize Game Objects will be a lot slower, consider disabling this for your skinned meshes calculated by a Particle Playground system.
		/// </summary>
		public void BoneUpdate () {
			if (initialized) {
				if (boneMatrices!=null && boneMatrices.Length>0) {
					for (int i = 0; i<boneMatrices.Length; i++) {
						if (boneTransforms[i]!=null)
							boneMatrices[i] = boneTransforms[i].localToWorldMatrix * bindPoses[i];
					}
				} else {
					if (bakedMesh==null)
						bakedMesh = new Mesh();
					renderer.BakeMesh(bakedMesh);
					localVertices = bakedMesh.vertices;
					vertexMatrix.SetTRS(renderer.transform.position, renderer.transform.rotation, renderer.transform.localScale);
				}
			}
		}
		
		/// <summary>
		/// Updates this Skinned World Object.
		/// </summary>
		public void Update () {
			if (initialized) {
				for (int i = 0; i<vertices.Length; i++) {
					if (boneMatrices.Length>0) {
						Matrix4x4 m0 = boneMatrices[weights[i].boneIndex0];
						Matrix4x4 m1 = boneMatrices[weights[i].boneIndex1];
						Matrix4x4 m2 = boneMatrices[weights[i].boneIndex2];
						Matrix4x4 m3 = boneMatrices[weights[i].boneIndex3];
						
						for (int n=0; n<16; n++) {
							vertexMatrix[n] =
								m0[n] * weights[i].weight0 +
									m1[n] * weights[i].weight1 +
									m2[n] * weights[i].weight2 +
									m3[n] * weights[i].weight3;
						}
					}
					vertices[i] = vertexMatrix.MultiplyPoint3x4(localVertices[i]);
				}
				vertexPositions = vertices;
			}
			isDoneThread = true;
		}
		
		/// <summary>
		/// Updates this Skinned World Object on a new thread.
		/// </summary>
		public void UpdateOnNewThread () {
			if (isDoneThread) {
				isDoneThread = false;
				PlaygroundC.RunAsync(()=> {
					if (isDoneThread) return;
					Update();
				});
			}
		}
		
		/// <summary>
		/// Returns a copy of this SkinnedWorldObject.
		/// </summary>
		public SkinnedWorldObject Clone () {
			SkinnedWorldObject skinnedWorldObject = new SkinnedWorldObject();
			skinnedWorldObject.downResolution = downResolution;
			skinnedWorldObject.gameObject = gameObject;
			skinnedWorldObject.transform = transform;
			skinnedWorldObject.rigidbody = rigidbody;
			skinnedWorldObject.renderer = renderer;
			skinnedWorldObject.meshFilter = meshFilter;
			skinnedWorldObject.mesh = mesh;
			skinnedWorldObject.updateNormals = updateNormals;
			return skinnedWorldObject;
		}
	}
	
	/// <summary>
	/// Holds information about a Manipulator Object. A Manipulator can both be Global and Local and will affect all particles within range.
	/// </summary>
	[Serializable]
	public class ManipulatorObjectC {
		/// <summary>
		/// The type of this manipulator.
		/// </summary>
		public MANIPULATORTYPEC type;							
		/// <summary>
		/// The property settings (if type is property).
		/// </summary>
		public ManipulatorPropertyC property = new ManipulatorPropertyC(); 
		/// <summary>
		/// The combined properties (if type is combined).
		/// </summary>
		public List<ManipulatorPropertyC> properties = new List<ManipulatorPropertyC>(); 
		/// <summary>
		/// The layers this manipulator will affect. This only applies to Global Manipulators.
		/// </summary>
		public LayerMask affects;								
		/// <summary>
		/// The transform of this manipulator (wrapped class for threading).
		/// </summary>
		public PlaygroundTransformC transform = new PlaygroundTransformC();
		/// <summary>
		/// The shape of this manipulator.
		/// </summary>
		public MANIPULATORSHAPEC shape;							
		/// <summary>
		/// The size of this manipulator (if shape is sphere).
		/// </summary>
		public float size;										
		/// <summary>
		/// The bounds of this manipulator (if shape is box).
		/// </summary>
		public Bounds bounds;									
		/// <summary>
		/// The strength of this manipulator.
		/// </summary>
		public float strength;									
		/// <summary>
		/// The scale of strength smoothing effector.
		/// </summary>
		public float strengthSmoothing = 1f;					
		/// <summary>
		/// The scale of distance strength effector.
		/// </summary>
		public float strengthDistanceEffect = 1f;
		/// <summary>
		/// Determines if Particle Lifetime Strength should be applied.
		/// </summary>
		public bool applyParticleLifetimeStrength = false;
		/// <summary>
		/// The particle lifetime strength is defined by a normalized Animation Curve.
		/// </summary>
		public AnimationCurve particleLifetimeStrength = new AnimationCurve(new Keyframe[]{new Keyframe(0,1f), new Keyframe(1f,1f)});
		/// <summary>
		/// Is this manipulator enabled?
		/// </summary>
		public bool enabled = true;								
		/// <summary>
		/// Should this manipulator be checking for particles inside or outside the shape's bounds?
		/// </summary>
		public bool inverseBounds = false;						
		/// <summary>
		/// The id of this manipulator.
		/// </summary>
		public int manipulatorId = 0;														
		/// <summary>
		/// Should lifetime filter determine which particles are affected?
		/// </summary>
		public bool applyLifetimeFilter = false;				
		/// <summary>
		/// The minimum normalized lifetime of a particle that is affected by this manipulator.
		/// </summary>
		public float lifetimeFilterMinimum = 0f;				
		/// <summary>
		/// The maximum normalized lifetime of a particle that is affected by this manipulator.
		/// </summary>
		public float lifetimeFilterMaximum = 1f;				
		/// <summary>
		/// Should particle filter determine which particles are affected?
		/// </summary>
		public bool applyParticleFilter = false;				
		/// <summary>
		/// The minimum normalized number in array of a particle that is affected by this manipulator.
		/// </summary>
		public float particleFilterMinimum = 0f;				
		/// <summary>
		/// The maximum normalized number in array of a particle that is affected by this manipulator.
		/// </summary>
		public float particleFilterMaximum = 1f;
		/// <summary>
		/// The axis constraints for this Manipulator. This enables you to set if the calculated forces should not be applied on certain axis.
		/// </summary>
		public PlaygroundAxisConstraintsC axisConstraints = new PlaygroundAxisConstraintsC();
		public bool unfolded = false;
		
		/// <summary>
		/// Should the manipulator be able to send particle events and keep track of its particles?
		/// </summary>
		public bool trackParticles = false;
		/// <summary>
		/// The tracking method when trackParticles is enabled. 
		/// Using TrackingMethod.ManipulatorId will be fast, but may not return correct when having overlapping Manipulators.
		/// Using TrackingMethod.ParticleId will be slow, but will ensure correct compare of particles within any overlapping Manipulators.
		/// </summary>
		public TrackingMethod trackingMethod = TrackingMethod.ManipulatorId;
		/// <summary>
		/// Should enter event be sent?
		/// </summary>
		public bool sendEventEnter = true;						
		/// <summary>
		/// Should exit event be sent?
		/// </summary>
		public bool sendEventExit = true;						
		/// <summary>
		/// Should birth event be sent?
		/// </summary>
		public bool sendEventBirth = true;						
		/// <summary>
		/// Should death event be sent?
		/// </summary>
		public bool sendEventDeath = true;						
		/// <summary>
		/// Should collision event be sent?
		/// </summary>
		public bool sendEventCollision = true;					
		public bool sendEventsUnfolded = false;
		/// <summary>
		/// The entering event of a particle (when using Event Listeners).
		/// </summary>
		public event OnPlaygroundParticle particleEventEnter;	
		/// <summary>
		/// The exit event of a particle (when using Event Listeners).
		/// </summary>
		public event OnPlaygroundParticle particleEventExit;	
		/// <summary>
		/// The birth event of a particle (when using Event Listeners).
		/// </summary>
		public event OnPlaygroundParticle particleEventBirth;	
		/// <summary>
		/// The death event of a particle (when using Event Listeners).
		/// </summary>
		public event OnPlaygroundParticle particleEventDeath;	
		/// <summary>
		/// The collision event of a particle (when using Event Listeners).
		/// </summary>
		public event OnPlaygroundParticle particleEventCollision;
		/// <summary>
		/// The current particles inside this manipulator. This requires that you have enabled trackParticles.
		/// </summary>
		[NonSerialized] public List<ManipulatorParticle> particles = new List<ManipulatorParticle>();
		/// <summary>
		/// The particles which will not be affected by this manipulator. If possible use the Manipulator's particle filtering methods instead as they are faster to compute.
		/// </summary>
		[NonSerialized] public List<ManipulatorParticle> nonAffectedParticles = new List<ManipulatorParticle>();
		/// <summary>
		/// The cached event particle used to send into events.
		/// </summary>
		public PlaygroundEventParticle manipulatorEventParticle = new PlaygroundEventParticle();
		[NonSerialized] bool initializedEventEnter = false;		// Has the enter event initialized?
		[NonSerialized] bool initializedEventExit = false;		// Has the exit event initialized?
		[NonSerialized] bool initializedEventBirth = false;		// Has the birth event initialized?
		[NonSerialized] bool initializedEventDeath = false;		// Has the death event initialized?
		[NonSerialized] bool initializedEventCollision = false;	// Has the collision event initialized?
		
		/// <summary>
		/// Checks if manipulator contains position. The outcome is depending on if you use a sphere (size)- or box (bounds) as shape. The passed in position is your target (particle) position and the mPosition is the Manipulator's origin position.
		/// </summary>
		/// <param name="position">Position of target.</param>
		/// <param name="mPosition">Center position of Manipulator.</param>
		public bool Contains (Vector3 position, Vector3 mPosition) {
			if (shape==MANIPULATORSHAPEC.Infinite) {
				return true;
			} else if (shape==MANIPULATORSHAPEC.Box) {
				if (!inverseBounds)
					return bounds.Contains(transform.inverseRotation*(position-mPosition));
				else
					return !bounds.Contains(transform.inverseRotation*(position-mPosition));
			} else {
				if (!inverseBounds)
					return (Vector3.Distance(position, mPosition)<=size);
				else
					return (Vector3.Distance(position, mPosition)>=size);
			}
		}
		
		public void SendParticleEventEnter () {
			
			if (initializedEventEnter)
				particleEventEnter(manipulatorEventParticle);
		}
		public void SendParticleEventExit () {
			if (initializedEventExit)
				particleEventExit(manipulatorEventParticle);
		}
		public void SendParticleEventBirth () {
			if (initializedEventBirth)
				particleEventBirth(manipulatorEventParticle);
		}
		public void SendParticleEventDeath () {
			if (initializedEventDeath)
				particleEventDeath(manipulatorEventParticle);
		}
		public void SendParticleEventCollision () {
			if (initializedEventCollision)
				particleEventCollision(manipulatorEventParticle);
		}
		
		/// <summary>
		/// Gets the particle at index. Note that the Manipulator must have trackParticles set to true.
		/// </summary>
		/// <returns>The particle.</returns>
		/// <param name="index">Index.</param>
		public PlaygroundEventParticle GetParticle (int index) {
			if (particles==null || particles.Count==0 || index>particles.Count) return null;
			return GetParticle (particles[index].particleSystemId, particles[index].particleId);
		}
		
		/// <summary>
		/// Gets the particle in particle system at index. Note that the Manipulator must have trackParticles set to true.
		/// </summary>
		/// <returns>The particle.</returns>
		/// <param name="particleSystemId">Particle system identifier.</param>
		/// <param name="particleId">Particle identifier.</param>
		public PlaygroundEventParticle GetParticle (int particleSystemId, int particleId) {
			PlaygroundEventParticle returnEventParticle = new PlaygroundEventParticle();
			if (PlaygroundC.reference.particleSystems[particleSystemId].UpdateEventParticle(returnEventParticle, particleId))
				return returnEventParticle;
			else return null;
		}
		
		/// <summary>
		/// Gets all particles within this Manipulator. Note that the Manipulator must have trackParticles set to true.
		/// </summary>
		/// <returns>The particles.</returns>
		public List<PlaygroundEventParticle> GetParticles () {
			List<PlaygroundEventParticle> returnEventParticles = new List<PlaygroundEventParticle>();
			for (int i = 0; i<particles.Count; i++) {
				if (PlaygroundC.reference.particleSystems[particles[i].particleSystemId].UpdateEventParticle(manipulatorEventParticle, particles[i].particleId))
					returnEventParticles.Add (manipulatorEventParticle.Clone());
			}
			return returnEventParticles;
		}
		
		/// <summary>
		/// Check if Manipulator contains particle of particle system.
		/// </summary>
		/// <returns><c>true</c>, if particle was found, <c>false</c> otherwise.</returns>
		/// <param name="particleSystemId">Particle system identifier.</param>
		/// <param name="particleId">Particle identifier.</param>
		public bool ContainsParticle (int particleSystemId, int particleId) {
			for (int i = 0; i<particles.Count; i++) {
				if (particles[i%particles.Count].particleSystemId==particleSystemId && particles[i%particles.Count].particleId==particleId)
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Check if passed in id is the same as this Manipulator id. This is the fastest way to compare particles (playgroundCache.manipulatorId) to Manipulators, but will not work for overlapping areas.
		/// </summary>
		/// <returns><c>true</c>, if same id, <c>false</c> otherwise.</returns>
		/// <param name="compareId">Id to compare with.</param>
		public bool IsSameId (int compareId) {
			if (manipulatorId==compareId)
				return true;
			return false;
		}
		
		/// <summary>
		/// Adds a particle to the list of particles. Note that the Manipulator must have tracking enabled.
		/// </summary>
		/// <param name="particleSystemId">Particle system identifier.</param>
		/// <param name="particleId">Particle identifier.</param>
		public bool AddParticle (int particleSystemId, int particleId) {
			if (particles == null)
				return false;
			for (int i = 0; i<particles.Count; i++) {
				if (particles==null || particles[i]==null)
					return false;
				if (particles[i%particles.Count].particleSystemId==particleSystemId && particles[i%particles.Count].particleId==particleId)
					return false;
			}
			particles.Add (new ManipulatorParticle(particleSystemId, particleId));
			return true;
		}
		
		/// <summary>
		/// Removes a particle from the list of particles. Note that the Manipulator must have tracking enabled.
		/// </summary>
		/// <returns><c>true</c>, if particle was removed, <c>false</c> otherwise.</returns>
		/// <param name="particleSystemId">Particle system identifier.</param>
		/// <param name="particleId">Particle identifier.</param>
		public bool RemoveParticle (int particleSystemId, int particleId) {
			if (particles == null)
				return false;
			bool didRemove = false; // We must execute the full list to remove any created duplicates
			for (int i = 0; i<particles.Count; i++) {
				if (particles==null || particles[i]==null)
					return false;
				if (particles[i%particles.Count].particleSystemId==particleSystemId && particles[i%particles.Count].particleId==particleId) {
					particles.RemoveAt(i%particles.Count);
					didRemove = true;
				}
			}
			return didRemove;
		}
		
		/// <summary>
		/// Kills all particles within the Manipulator. Note that the Manipulator must have tracking enabled.
		/// </summary>
		public void KillAllParticles () {
			if (particles == null)
				return;
			for (int i = 0; i<particles.Count; i++) {
				if (i>=particles.Count || i<0) return;
				PlaygroundC.reference.particleSystems[particles[i].particleSystemId].Kill (particles[i].particleId);
				particles.RemoveAt(i);
				if (i>0) i--;
			}
		}
		
		/// <summary>
		/// Check if the Manipulator contains a particle which won't be affected.
		/// </summary>
		/// <returns><c>true</c>, if non-affected particle was found, <c>false</c> otherwise.</returns>
		/// <param name="particleSystemId">Particle system identifier.</param>
		/// <param name="particleId">Particle identifier.</param>
		public bool ContainsNonAffectedParticle (int particleSystemId, int particleId) {
			for (int i = 0; i<nonAffectedParticles.Count; i++) {
				if (nonAffectedParticles[i].particleSystemId==particleSystemId && nonAffectedParticles[i].particleId==particleId)
					return true;
			}
			return false;
		}
		
		/// <summary>
		/// Adds a non-affected particle. The particle won't be affected by this Manipulator. If possible, use the particle filtering methods on the Manipulator instead, they are faster to compute.
		/// </summary>
		/// <param name="particleSystemId">Particle system identifier.</param>
		/// <param name="particleId">Particle identifier.</param>
		public void AddNonAffectedParticle (int particleSystemId, int particleId) {
			nonAffectedParticles.Add (new ManipulatorParticle(particleSystemId, particleId));
		}
		
		/// <summary>
		/// Removes the non affected particle. If possible, use the particle filtering methods on the Manipulator instead, they are faster to compute.
		/// </summary>
		/// <returns><c>true</c>, if non affected particle was removed, <c>false</c> otherwise.</returns>
		/// <param name="particleSystemId">Particle system identifier.</param>
		/// <param name="particleId">Particle identifier.</param>
		public bool RemoveNonAffectedParticle (int particleSystemId, int particleId) {
			for (int i = 0; i<nonAffectedParticles.Count; i++) {
				if (nonAffectedParticles[i].particleSystemId==particleSystemId && nonAffectedParticles[i].particleId==particleId) {
					nonAffectedParticles.RemoveAt(i);
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Check if lifetime filter will apply.
		/// </summary>
		/// <returns><c>true</c>, if filter was lifetimed, <c>false</c> otherwise.</returns>
		/// <param name="life">Life.</param>
		/// <param name="total">Total.</param>
		public bool LifetimeFilter (float life, float total) {
			if (!applyLifetimeFilter) return true;
			float normalizedLife = life/total;
			return (normalizedLife>=lifetimeFilterMinimum && normalizedLife<=lifetimeFilterMaximum);
		}
		
		/// <summary>
		/// Check if particle filter will apply.
		/// </summary>
		/// <returns><c>true</c>, if filter was particled, <c>false</c> otherwise.</returns>
		/// <param name="p">P.</param>
		/// <param name="total">Total.</param>
		public bool ParticleFilter (int p, int total) {
			if (!applyParticleFilter) return true;
			float normalizedP = (p*1f)/(total*1f);
			return (normalizedP>=particleFilterMinimum && normalizedP<=particleFilterMaximum);
		}
		
		/// <summary>
		/// Update this Manipulator Object. This runs automatically each calculation loop.
		/// </summary>
		public void Update () {
			if (enabled) {
				if (transform.Update()) {
					manipulatorId = transform.instanceID;
					if (trackParticles) {
						initializedEventEnter = (particleEventEnter!=null);
						initializedEventExit = (particleEventExit!=null);
						initializedEventBirth = (particleEventBirth!=null);
						initializedEventDeath = (particleEventDeath!=null);
						initializedEventCollision = (particleEventCollision!=null);
					} else {
						if (particles.Count>0)
							particles.Clear();
					}
					if (type==MANIPULATORTYPEC.Property || type==MANIPULATORTYPEC.Combined) {
						property.Update();
						property.SetLocalVelocity(transform.rotation, property.velocity);
						if (property.type == MANIPULATORPROPERTYTYPEC.Math)
							property.UpdateMathProperty(transform.position);
						for (int i = 0; i<properties.Count; i++) {
							properties[i].Update();
							properties[i].SetLocalVelocity(transform.rotation, properties[i].velocity);
							if (properties[i].type == MANIPULATORPROPERTYTYPEC.Math)
								properties[i].UpdateMathProperty(transform.position);
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Sets a new transform to this Manipulator.
		/// </summary>
		/// <param name="transform">The new Transform that should be assigned to this Manipulator.</param>
		public void SetTransform (Transform transform) {
			this.transform.SetFromTransform(transform);
		}
		
		public void SetLocalTargetsPosition (Transform otherTransform) {
			property.SetLocalTargetsPosition(otherTransform);
			for (int i = 0; i<properties.Count; i++)
				properties[i].SetLocalTargetsPosition (otherTransform);
		}
		
		/// <summary>
		/// Return a copy of this ManipulatorObjectC.
		/// </summary>
		public ManipulatorObjectC Clone () {
			ManipulatorObjectC manipulatorObject = new ManipulatorObjectC();
			manipulatorObject.type = type;
			manipulatorObject.property = property.Clone();
			manipulatorObject.affects = affects;
			manipulatorObject.transform = transform.Clone();
			manipulatorObject.size = size;
			manipulatorObject.shape = shape;
			manipulatorObject.bounds = bounds;
			manipulatorObject.strength = strength;
			manipulatorObject.applyParticleLifetimeStrength = applyParticleLifetimeStrength;
			manipulatorObject.particleLifetimeStrength = new AnimationCurve(particleLifetimeStrength.keys);
			manipulatorObject.strengthSmoothing = strengthSmoothing;
			manipulatorObject.strengthDistanceEffect = strengthDistanceEffect;
			manipulatorObject.enabled = enabled;
			manipulatorObject.applyLifetimeFilter = applyLifetimeFilter;
			manipulatorObject.lifetimeFilterMinimum = lifetimeFilterMinimum;
			manipulatorObject.lifetimeFilterMaximum = lifetimeFilterMaximum;
			manipulatorObject.applyParticleFilter = applyParticleFilter;
			manipulatorObject.particleFilterMinimum = particleFilterMinimum;
			manipulatorObject.particleFilterMaximum = particleFilterMaximum;
			manipulatorObject.inverseBounds = inverseBounds;
			manipulatorObject.trackParticles = trackParticles;
			manipulatorObject.sendEventEnter = sendEventEnter;
			manipulatorObject.sendEventExit = sendEventExit;
			manipulatorObject.sendEventBirth = sendEventBirth;
			manipulatorObject.sendEventDeath = sendEventDeath;
			manipulatorObject.sendEventCollision = sendEventCollision;
			manipulatorObject.properties = new List<ManipulatorPropertyC>();
			manipulatorObject.axisConstraints = axisConstraints.Clone();
			for (int i = 0; i<properties.Count; i++)
				manipulatorObject.properties.Add(properties[i].Clone());
			return manipulatorObject;
		}
	}
	
	/// <summary>
	/// Holds information for a Manipulator Object's different property abilities.
	/// </summary>
	[Serializable]
	public class ManipulatorPropertyC {
		/// <summary>
		/// The type of this manipulator property.
		/// </summary>
		public MANIPULATORPROPERTYTYPEC type;					
		/// <summary>
		/// The transition of this manipulator property.
		/// </summary>
		public MANIPULATORPROPERTYTRANSITIONC transition;
		/// <summary>
		/// The math property. This can be used to apply extensive particle behaviors.
		/// </summary>
		public MathManipulatorProperty mathProperty = new MathManipulatorProperty();
		/// <summary>
		/// The velocity of this manipulator property.
		/// </summary>
		public Vector3 velocity;								
		/// <summary>
		/// The local velocity of this manipulator property (set by SetLocalVelocity).
		/// </summary>
		public Vector3 localVelocity;							
		/// <summary>
		/// The color of this manipulator property.
		/// </summary>
		public Color color = new Color(1,1,1,1);				
		/// <summary>
		/// The lifetime color of this manipulator property.
		/// </summary>
		public Gradient lifetimeColor;							
		/// <summary>
		/// The size of this manipulator property.
		/// </summary>
		public float size = 1f;									
		/// <summary>
		/// The targets to position towards of this manipulator property. Each target is a single point in world space of type PlaygroundTransformC (Transform wrapper).
		/// </summary>
		public List<PlaygroundTransformC> targets = new List<PlaygroundTransformC>();
		/// <summary>
		/// The pointer of targets (used for calculation to determine which particle should get which target).
		/// </summary>
		public int targetPointer;								
		/// <summary>
		/// The Mesh Target (World Object) to target particles to.
		/// </summary>
		public WorldObject meshTarget = new WorldObject();		
		/// <summary>
		/// The Skinned Mesh Target (Skinned World Object) to target particles to.
		/// </summary>
		public SkinnedWorldObject skinnedMeshTarget = new SkinnedWorldObject();
		/// <summary>
		/// The mesh- or skinned mesh target is procedural and changes vertices over time.
		/// </summary>
		public bool meshTargetIsProcedural = false;
		/// <summary>
		/// Transform matrix for mesh targets.
		/// </summary>
		public Matrix4x4 meshTargetMatrix = new Matrix4x4();
		/// <summary>
		/// The State Target in the scene to target particles to.
		/// </summary>
		public ParticleStateC stateTarget = new ParticleStateC();
		/// <summary>
		/// The Playground Spline Target in the scene to target particles to.
		/// </summary>
		public PlaygroundSpline splineTarget;
		/// <summary>
		/// The method to target the Playground Spline assigned to splineTarget.
		/// </summary>
		public SPLINETARGETMETHOD splineTargetMethod;
		public float splineTimeOffset;
		bool splineTargetIsReady;
		/// <summary>
		/// Should the manipulator's transform direction be used to apply velocity?
		/// </summary>
		public bool useLocalRotation = false;					
		/// <summary>
		/// Should the particles go back to original color when out of range?
		/// </summary>
		public bool onlyColorInRange = true;					
		/// <summary>
		/// Should the particles keep their original color alpha?
		/// </summary>
		public bool keepColorAlphas = true;						
		/// <summary>
		/// Should the particles stop positioning towards target when out of range?
		/// </summary>
		public bool onlyPositionInRange = true;					
		/// <summary>
		/// Should the particles go back to original size when out of range?
		/// </summary>
		public bool onlySizeInRange = false;					
		/// <summary>
		/// The strength to zero velocity on target positioning when using transitions.
		/// </summary>
		public float zeroVelocityStrength = 1f;					
		/// <summary>
		/// Individual property strength.
		/// </summary>
		public float strength = 1f;
		public bool unfolded = true;							
		/// <summary>
		/// Target sorting type.
		/// </summary>
		public TARGETSORTINGC targetSorting;					
		/// <summary>
		/// The sorted list for target positions.
		/// </summary>
		[NonSerialized] public int[] targetSortingList;			
		/// <summary>
		/// The previous sorting type.
		/// </summary>
		TARGETSORTINGC previousTargetSorting;					
		
		public TURBULENCETYPE turbulenceType = TURBULENCETYPE.Simplex; // The type of turbulence
		public float turbulenceTimeScale = 1f;					// Time scale of turbulence
		public float turbulenceScale = 1f;						// Resolution scale of turbulence
		public bool turbulenceApplyLifetimeStrength;			// Should lifetime strength apply to turbulence?
		public AnimationCurve turbulenceLifetimeStrength;		// The normalized lifetime strength of turbulence
		public SimplexNoise turbulenceSimplex;					// The Simplex Noise of this manipulator property
		
		
		/// <summary>
		/// Updates this ManipulatorPropertyC.
		/// </summary>
		public void Update () {
			if (type==MANIPULATORPROPERTYTYPEC.Target) {
				for (int i = 0; i<targets.Count; i++)
					targets[i].Update();
			} else
			if (type==MANIPULATORPROPERTYTYPEC.MeshTarget) {
				if (meshTarget.gameObject!=null) {
					if (meshTarget.cachedId!=meshTarget.gameObject.GetInstanceID())
						meshTarget = PlaygroundParticlesC.NewWorldObject (meshTarget.gameObject.transform);
					if (meshTarget.mesh!=null) {
						meshTargetMatrix.SetTRS(meshTarget.transform.position, meshTarget.transform.rotation, meshTarget.transform.localScale);
						if (meshTargetIsProcedural)
							UpdateMeshTarget();
					}
					if (meshTarget.initialized && (targetSortingList==null || targetSortingList.Length!=meshTarget.vertexPositions.Length || previousTargetSorting!=targetSorting))
						TargetSorting();
				}
			} else
			if (type==MANIPULATORPROPERTYTYPEC.SkinnedMeshTarget) {
				if (skinnedMeshTarget.gameObject!=null) {
					if (skinnedMeshTarget.cachedId!=skinnedMeshTarget.gameObject.GetInstanceID() || !skinnedMeshTarget.initialized) {
						skinnedMeshTarget = PlaygroundParticlesC.NewSkinnedWorldObject (skinnedMeshTarget.gameObject.transform);
					}
					if (skinnedMeshTarget.initialized) {
						if (meshTargetIsProcedural)
							UpdateSkinnedMeshTarget();
						skinnedMeshTarget.BoneUpdate();
						PlaygroundC.RunAsync(()=>{
							skinnedMeshTarget.Update();
						});
						if (targetSortingList==null || targetSortingList.Length!=skinnedMeshTarget.vertexPositions.Length || previousTargetSorting!=targetSorting)
							TargetSorting();
					}
				}
			} else
			if (type==MANIPULATORPROPERTYTYPEC.StateTarget) {
				if (!stateTarget.initialized && (stateTarget.stateMesh!=null || stateTarget.stateTexture!=null) && stateTarget.stateTransform!=null) {
					stateTarget.Initialize();
				} else if (stateTarget.initialized && stateTarget.stateTransform!=null) {
					stateTarget.stateTransformMx.SetTRS(stateTarget.stateTransform.position, stateTarget.stateTransform.rotation, stateTarget.stateTransform.localScale);
				}
				if (stateTarget.initialized && (targetSortingList==null || targetSortingList.Length!=stateTarget.positionLength || previousTargetSorting!=targetSorting))
					TargetSorting();
			} else
			if (type==MANIPULATORPROPERTYTYPEC.SplineTarget) {
				splineTargetIsReady = splineTarget!=null&&splineTarget.IsReady();
			} else
			if (type==MANIPULATORPROPERTYTYPEC.Turbulence) {
				if (turbulenceSimplex==null)
					turbulenceSimplex = new SimplexNoise();
			} else
			if (type==MANIPULATORPROPERTYTYPEC.Math) {
				
			}
		}
		
		public void UpdateMathProperty (Vector3 manipulatorPosition) {
			if (mathProperty==null)
				mathProperty = new MathManipulatorProperty();
			mathProperty.Update (manipulatorPosition);
		}
		
		/// <summary>
		/// Refreshes the meshTarget's mesh. Use this whenever you're working with a procedural mesh and want to assign new target vertices for your particles.
		/// </summary>
		public void UpdateMeshTarget () {
			meshTarget.Update();
		}
		
		/// <summary>
		/// Refreshes the skinnedMeshTarget's mesh. Use this whenever you're working with a procedural skinned mesh and want to assign new target vertices for your particles.
		/// </summary>
		public void UpdateSkinnedMeshTarget () {
			skinnedMeshTarget.Update();
		}
		
		/// <summary>
		/// Sets the mesh target. Whenever a new GameObject is assigned to your mesh target a refresh of the World Object will be initiated.
		/// </summary>
		/// <param name="gameObject">The GameObject to create your mesh target's World Object from. A MeshFilter- and mesh component is required on the GameObject.</param>
		public void SetMeshTarget (GameObject gameObject) {
			meshTarget.gameObject = gameObject;
		}
		
		/// <summary>
		/// Sets the skinned mesh target. Whenever a new GameObject is assigned to your skinned mesh target a refresh of the Skinned World Object will be initiated.
		/// </summary>
		/// <param name="gameObject">The GameObject to create your skinned mesh target's Skinned World Object from. A SkinnedMeshRenderer- and mesh component is required on the GameObject.</param>
		public void SetSkinnedMeshTarget (GameObject gameObject) {
			skinnedMeshTarget.gameObject = gameObject;
		}
		
		/// <summary>
		/// Determines if the Playground Spline Target is ready to be read.
		/// </summary>
		/// <returns><c>true</c>, if Playground Spline Target is ready, <c>false</c> otherwise.</returns>
		public bool SplineTargetIsReady () {
			return splineTargetIsReady;
		}
		
		// Sorts the particles for targeting depending on sorting method.
		public void TargetSorting () {
			if (type==MANIPULATORPROPERTYTYPEC.MeshTarget && !meshTarget.initialized || type==MANIPULATORPROPERTYTYPEC.SkinnedMeshTarget && !skinnedMeshTarget.initialized || type==MANIPULATORPROPERTYTYPEC.StateTarget && !stateTarget.initialized) return;
			targetPointer = 0;
			if (type==MANIPULATORPROPERTYTYPEC.MeshTarget)
				targetSortingList = new int[meshTarget.vertexPositions.Length];
			else if (type==MANIPULATORPROPERTYTYPEC.SkinnedMeshTarget)
				targetSortingList = new int[skinnedMeshTarget.vertexPositions.Length];
			else if (type==MANIPULATORPROPERTYTYPEC.StateTarget)
				targetSortingList = new int[stateTarget.positionLength];
			switch (targetSorting) {
			case TARGETSORTINGC.Scrambled:
				for (int i = 0; i<targetSortingList.Length; i++)
					targetSortingList[i]=i;
				PlaygroundC.ShuffleArray(targetSortingList);
				break;
			case TARGETSORTINGC.Linear:
				for (int i = 0; i<targetSortingList.Length; i++)
					targetSortingList[i]=i;
				break;
			case TARGETSORTINGC.Reversed:
				int x = 0;
				for (int i = targetSortingList.Length-1; i>0; i--) {
					targetSortingList[x]=i;
					x++;
				}
				break;
			}
			previousTargetSorting = targetSorting;
		}
		
		public void SetLocalVelocity (Quaternion rotation, Vector3 newVelocity) {
			localVelocity = rotation*newVelocity;
		}
		
		public void SetLocalTargetsPosition (Transform otherTransform) {
			for (int i = 0; i<targets.Count; i++)
				targets[i].SetLocalPosition (otherTransform);
		}
		
		/// <summary>
		/// Update a target, returns availability.
		/// </summary>
		/// <returns><c>true</c>, if target was updated, <c>false</c> otherwise.</returns>
		/// <param name="index">Index.</param>
		public bool UpdateTarget (int index) {
			if (targets.Count>0) {
				index=index%targets.Count;
				targets[index].Update();
			}
			return targets[index].available;
		}
		
		/// <summary>
		/// Return a copy of this ManipulatorPropertyC.
		/// </summary>
		public ManipulatorPropertyC Clone () {
			ManipulatorPropertyC manipulatorProperty = new ManipulatorPropertyC();
			manipulatorProperty.type = type;
			manipulatorProperty.transition = transition;
			manipulatorProperty.velocity = velocity;
			manipulatorProperty.color = color;
			manipulatorProperty.lifetimeColor = lifetimeColor;
			manipulatorProperty.size = size;
			manipulatorProperty.useLocalRotation = useLocalRotation;
			manipulatorProperty.onlyColorInRange = onlyColorInRange;
			manipulatorProperty.keepColorAlphas = keepColorAlphas;
			manipulatorProperty.onlyPositionInRange = onlyPositionInRange;
			manipulatorProperty.zeroVelocityStrength = zeroVelocityStrength;
			manipulatorProperty.strength = strength;
			manipulatorProperty.targetPointer = targetPointer;
			manipulatorProperty.targets = new List<PlaygroundTransformC>();
			for (int i = 0; i<targets.Count; i++)
				manipulatorProperty.targets.Add(targets[i].Clone());
			manipulatorProperty.meshTarget = meshTarget.Clone();
			manipulatorProperty.skinnedMeshTarget = skinnedMeshTarget.Clone();
			manipulatorProperty.stateTarget = stateTarget.Clone();
			
			manipulatorProperty.turbulenceType								= turbulenceType;
			manipulatorProperty.turbulenceApplyLifetimeStrength				= turbulenceApplyLifetimeStrength;
			manipulatorProperty.turbulenceScale								= turbulenceScale;
			manipulatorProperty.turbulenceTimeScale							= turbulenceTimeScale;
			
			manipulatorProperty.turbulenceLifetimeStrength 					= new AnimationCurve(turbulenceLifetimeStrength.keys);
			
			
			return manipulatorProperty;
		}
	}
	
	/// <summary>
	/// Holds information for a Math Manipulator. This can be used to apply extensive particle behaviors, such as positioning or changing sizes using sine, cosine or linear interpolation math.
	/// </summary>
	[Serializable]
	public class MathManipulatorProperty {
		public MATHMANIPULATORTYPE type;
		public MATHMANIPULATORPROPERTY property;
		public MATHMANIPULATORCLAMP clamp;
		public float clampCeil = 10f;
		public float clampFloor = 0;
		public float value = 1f;
		public Vector3 value3 = Vector3.up;
		public float rate = 1f;
		public Vector3 rate3 = Vector3.up;
		
		Vector3 value3Position;
		
		/// <summary>
		/// Updates the position for this MathManipulatorProperty. This happens automatically each frame for a Math Manipulator.
		/// </summary>
		public void Update (Vector3 position) {
			value3Position = position+value3;
		}
		
		/// <summary>
		/// Evaluate the specified in-value and time. 
		/// - When using Sin or Cos a continuous time is expected (such as Time.time).
		/// - Lerp expects a normalized value between 0f to 1f, the framework is using the delta time each frame (resulting in a smooth damp).
		/// - Add or Subtract will scale the value with the passed in time, using it as a multiplier.
		/// </summary>
		/// <param name="inValue">In value.</param>
		/// <param name="time">Time.</param>
		public Vector3 Evaluate (Vector3 inValue, float time) {
			Vector3 returnValue;
			if (clamp==MATHMANIPULATORCLAMP.ClampInValue)
				inValue = Clamp (inValue);
			switch (type) {
			case MATHMANIPULATORTYPE.Sin: 
				returnValue = new Vector3(
					value3.x!=0? ClampOut(inValue.x+Mathf.Sin (time*rate3.x)*value3.x) : inValue.x,
					value3.y!=0? ClampOut(inValue.y+Mathf.Sin (time*rate3.y)*value3.y) : inValue.y,
					value3.z!=0? ClampOut(inValue.z+Mathf.Sin (time*rate3.z)*value3.z) : inValue.z
					);
				break;
			case MATHMANIPULATORTYPE.Cos: 
				returnValue = Clamp(new Vector3(
					value3.x!=0? ClampOut(inValue.x+Mathf.Cos (time*rate3.x)*value3.x) : inValue.x,
					value3.y!=0? ClampOut(inValue.y+Mathf.Cos (time*rate3.y)*value3.y) : inValue.y,
					value3.z!=0? ClampOut(inValue.z+Mathf.Cos (time*rate3.z)*value3.z) : inValue.z
					));
				break;
			case MATHMANIPULATORTYPE.Lerp: 
				returnValue = Clamp(new Vector3(
					value3.x!=0? Mathf.Lerp (inValue.x, ClampOut(value3.x), time*rate3.x) : inValue.x,
					value3.y!=0? Mathf.Lerp (inValue.y, ClampOut(value3.y), time*rate3.y) : inValue.y,
					value3.z!=0? Mathf.Lerp (inValue.z, ClampOut(value3.z), time*rate3.z) : inValue.z
					));
				break;
			case MATHMANIPULATORTYPE.Add:
				returnValue = inValue+ClampOut((Vector3.Scale (value3, rate3)*time));
				break;
			case MATHMANIPULATORTYPE.Subtract:
				returnValue = inValue-ClampOut((Vector3.Scale (value3, rate3)*time));
				break;
			default:
				returnValue = inValue;
				break;
			}
			return returnValue;
		}
		
		/// <summary>
		/// Evaluate the specified in-value and time. This overload is specifically adapted for if you want to clamp position alongside the Manipulator's position.
		/// - When using Sin or Cos a continuous time is expected (such as Time.time).
		/// - Lerp expects a normalized value between 0f to 1f, the framework is using the delta time each frame (resulting in a smooth damp).
		/// - Add or Subtract will scale the value with the passed in time, using it as a multiplier.
		/// </summary>
		/// <param name="inValue">In value.</param>
		/// <param name="time">Time.</param>
		public Vector3 EvaluatePosition (Vector3 inValue, float time) {
			Vector3 returnValue;
			if (clamp==MATHMANIPULATORCLAMP.ClampInValue)
				inValue = Clamp (inValue);
			switch (type) {
			case MATHMANIPULATORTYPE.Sin: 
				returnValue = ClampPosition(new Vector3(
					inValue.x+(Mathf.Sin (time*rate3.x)*value3.x),
					inValue.y+(Mathf.Sin (time*rate3.y)*value3.y),
					inValue.z+(Mathf.Sin (time*rate3.z)*value3.z)
					));
				break;
			case MATHMANIPULATORTYPE.Cos: 
				returnValue = ClampPosition(new Vector3(
					inValue.x+Mathf.Cos (time*rate3.x)*value3.x,
					inValue.y+Mathf.Cos (time*rate3.y)*value3.y,
					inValue.z+Mathf.Cos (time*rate3.z)*value3.z
					));
				break;
			case MATHMANIPULATORTYPE.Lerp: 
				returnValue = ClampPosition(new Vector3(
					Mathf.Lerp (inValue.x, value3Position.x, time*rate3.x),
					Mathf.Lerp (inValue.y, value3Position.y, time*rate3.y),
					Mathf.Lerp (inValue.z, value3Position.z, time*rate3.z)
					));
				break;
			case MATHMANIPULATORTYPE.Add:
				returnValue = ClampPosition(inValue+(Vector3.Scale (value3, rate3)*time));
				break;
			case MATHMANIPULATORTYPE.Subtract:
				returnValue = ClampPosition(inValue-(Vector3.Scale (value3, rate3)*time));
				break;
			default:
				returnValue = inValue;
				break;
			}
			return returnValue;
		}
		
		/// <summary>
		/// Evaluate the specified in-value and time. 
		/// - When using Sin or Cos a continuous time is expected (such as Time.time).
		/// - Lerp expects a normalized value between 0f to 1f, the framework is using the delta time each frame (resulting in a smooth damp).
		/// - Add or Subtract will scale the value with the passed in time, using it as a multiplier.
		/// </summary>
		/// <param name="inValue">In value.</param>
		/// <param name="time">Time.</param>
		public float Evaluate (float inValue, float time) {
			float returnValue;
			if (clamp==MATHMANIPULATORCLAMP.ClampInValue)
				inValue = Clamp (inValue);
			switch (type) {
			case MATHMANIPULATORTYPE.Sin:
				returnValue = value!=0? ClampOut(Mathf.Sin (time*rate)*value) : inValue;
				break;
			case MATHMANIPULATORTYPE.Cos:
				returnValue = value!=0? ClampOut(Mathf.Cos (time*rate)*value) : inValue;
				break;
			case MATHMANIPULATORTYPE.Lerp:
				returnValue = value!=0? Mathf.Lerp (inValue, ClampOut(value), time*rate) : inValue;
				break;
			case MATHMANIPULATORTYPE.Add:
				returnValue = ClampOut(inValue+(value*rate*time));
				break;
			case MATHMANIPULATORTYPE.Subtract:
				returnValue = ClampOut(inValue-(value*rate*time));
				break;
			default:
				returnValue = inValue;
				break;
			}
			return returnValue;
		}
		
		float Clamp (float val) {
			if (clamp==MATHMANIPULATORCLAMP.None) return val;
			return Mathf.Clamp (val, clampFloor, clampCeil);
		}
		
		Vector3 Clamp (Vector3 val) {
			if (clamp==MATHMANIPULATORCLAMP.None) return val;
			val = new Vector3 (
				Mathf.Clamp (val.x, clampFloor, clampCeil), 
				Mathf.Clamp (val.y, clampFloor, clampCeil), 
				Mathf.Clamp (val.z, clampFloor, clampCeil)
				);
			return val;
		}
		
		Vector3 ClampPosition (Vector3 val) {
			if (clamp==MATHMANIPULATORCLAMP.None) return val;
			val = new Vector3 (
				Mathf.Clamp (val.x, value3Position.x+clampFloor, value3Position.x+clampCeil), 
				Mathf.Clamp (val.y, value3Position.y+clampFloor, value3Position.y+clampCeil), 
				Mathf.Clamp (val.z, value3Position.z+clampFloor, value3Position.z+clampCeil)
				);
			return val;
		}
		
		float ClampOut (float val) {
			if (clamp!=MATHMANIPULATORCLAMP.ClampOutValue) return val;
			return Clamp (val);
		}
		
		Vector3 ClampOut (Vector3 val) {
			if (clamp!=MATHMANIPULATORCLAMP.ClampOutValue) return val;
			return Clamp (val);
		}
		
		/// <summary>
		/// Returns a clone of this MathManipulatorProperty.
		/// </summary>
		public MathManipulatorProperty Clone () {
			MathManipulatorProperty clone = new MathManipulatorProperty();
			clone.type = type;
			clone.property = property;
			clone.clamp = clamp;
			clone.clampFloor = clampFloor;
			clone.clampCeil = clampCeil;
			clone.value = value;
			clone.value3 = value3;
			clone.rate = rate;
			clone.rate3 = rate3;
			return clone;
		}
	}
	
	/// <summary>
	/// The Manipulator Particle class is a container for tracking particles in their particle system. When reaching a particle on a Manipulator the particle will convert to a PlaygroundEventParticle.
	/// </summary>
	[Serializable]
	public class ManipulatorParticle {
		public int particleSystemId;
		public int particleId;
		public ManipulatorParticle(int setParticleSystemId, int setParticleId) {
			particleSystemId = setParticleSystemId;
			particleId = setParticleId;
		}
	}
	
	/// <summary>
	/// Wrapper class for the Transform component. This is updated outside- and read inside the multithreaded environment.
	/// </summary>
	[Serializable]
	public class PlaygroundTransformC {
		/// <summary>
		/// The Transform component the Playground Transform wrapper will base its calculation from.
		/// </summary>
		public Transform transform;
		/// <summary>
		/// The instance id.
		/// </summary>
		public int instanceID;
		/// <summary>
		/// Is this Playground Transform available for calculation?
		/// </summary>
		public bool available;
		/// <summary>
		/// The position in units.
		/// </summary>
		public Vector3 position;
		/// <summary>
		/// The local position (if parented) in units.
		/// </summary>
		public Vector3 localPosition;
		/// <summary>
		/// The previous calculated position.
		/// </summary>
		public Vector3 previousPosition;
		/// <summary>
		/// The forward axis (Transform forward).
		/// </summary>
		public Vector3 forward;
		/// <summary>
		/// The upwards axis (Transform up).
		/// </summary>
		public Vector3 up;
		/// <summary>
		/// The right axis (Transform right).
		/// </summary>
		public Vector3 right;
		/// <summary>
		/// The rotation of this Playground Transform.
		/// </summary>
		public Quaternion rotation;
		/// <summary>
		/// The inverse rotation of this Playground Transform (used for bounding boxes).
		/// </summary>
		public Quaternion inverseRotation;
		public ScaleMethod scaleMethod;
		/// <summary>
		/// The local scale of this Playground Transform.
		/// </summary>
		public Vector3 localScale;
		/// <summary>
		/// The lossy scale of this Playground Transform.
		/// </summary>
		public Vector3 lossyScale;
		/// <summary>
		/// The transform matrix. This can be used to set and get transform coordinates from another thread.
		/// </summary>
		public Matrix4x4 transformMatrix = new Matrix4x4();
		public bool unfolded;
		
		/// <summary>
		/// Update this PlaygroundTransformC, returns availability (not thread-safe).
		/// </summary>
		public bool Update () {
			if (transform!=null) {
				instanceID = transform.GetInstanceID();
				previousPosition = position;
				position = transform.position;
				forward = transform.forward;
				up = transform.up;
				right = transform.right;
				rotation = transform.rotation;
				inverseRotation = rotation;
				inverseRotation.x=-inverseRotation.x;
				inverseRotation.y=-inverseRotation.y;
				inverseRotation.z=-inverseRotation.z;
				localScale = transform.localScale;
				lossyScale = transform.lossyScale;
				
				available = true;
			} else {
				available = false;
			}
			return available;
		}
		
		/// <summary>
		/// Updates the transform matrix.
		/// </summary>
		public void UpdateMatrix () {
			if (available)
				transformMatrix.SetTRS(position, rotation, scaleMethod == ScaleMethod.Local? localScale : lossyScale);
		}
		
		public bool IsSameAs (Transform comparer) {
			return (transform==comparer);
		}
		
		public void SetZeroRotation () {
			rotation = Quaternion.identity;
		}
		
		/// <summary>
		/// Sets data from a Transform (not thread-safe).
		/// </summary>
		/// <param name="otherTransform">Other transform.</param>
		public void SetFromTransform (Transform otherTransform) {
			transform = otherTransform;
			Update();
		}
		
		/// <summary>
		/// Sets a Transform's position, rotation and scale from this wrapped Transform (not thread-safe).
		/// </summary>
		/// <param name="otherTransform">Other transform.</param>
		public void GetFromTransform (Transform otherTransform) {
			otherTransform.position = position;
			otherTransform.rotation = rotation;
			otherTransform.localScale = localScale;
		}
		
		/// <summary>
		/// Sets local position from another transform (not thread-safe).
		/// </summary>
		/// <param name="otherTransform">Other transform.</param>
		public void SetLocalPosition (Transform otherTransform) {
			if (available)
				localPosition = otherTransform.InverseTransformPoint(transform.position);
		}
		
		public void SetPostitionAsLocal () {
			position = localPosition;
		}
		
		/// <summary>
		/// Returns the instance id of this PlaygroundTransformC.
		/// </summary>
		/// <returns>The instance id.</returns>
		public int GetInstanceID () {
			return instanceID;
		}
		
		/// <summary>
		/// Returns a copy of this PlaygroundTransformC.
		/// </summary>
		public PlaygroundTransformC Clone () {
			PlaygroundTransformC playgroundTransform = new PlaygroundTransformC();
			playgroundTransform.transform = transform;
			playgroundTransform.available = available;
			playgroundTransform.position = position;
			playgroundTransform.forward = forward;
			playgroundTransform.up = up;
			playgroundTransform.right = right;
			playgroundTransform.rotation = rotation;
			return playgroundTransform;
		}
	}
	
	/// <summary>
	/// Holds information for a Playground Collider (infinite collision plane).
	/// </summary>
	[Serializable]
	public class PlaygroundColliderC {
		/// <summary>
		/// Is this PlaygroundCollider enabled?
		/// </summary>
		public bool enabled = true;								
		/// <summary>
		/// The transform that makes this PlaygroundCollider.
		/// </summary>
		public Transform transform;								
		/// <summary>
		/// The plane of this PlaygroundCollider.
		/// </summary>
		public Plane plane = new Plane();						
		/// <summary>
		/// The offset in world space to this plane.
		/// </summary>
		public Vector3 offset;									
		
		/// <summary>
		/// Update this PlaygroundCollider's plane.
		/// </summary>
		public void UpdatePlane () {
			if (!transform) return;
			plane.SetNormalAndPosition(transform.up, transform.position+offset);
		}
		
		/// <summary>
		/// Clone this PlaygroundColliderC.
		/// </summary>
		public PlaygroundColliderC Clone () {
			PlaygroundColliderC playgroundCollider = new PlaygroundColliderC();
			playgroundCollider.enabled = enabled;
			playgroundCollider.transform = transform;
			playgroundCollider.plane = new Plane(plane.normal, plane.distance);
			playgroundCollider.offset = offset;
			return playgroundCollider;
		}
	}
	
	/// <summary>
	/// Hold information for axis constraints in X-, Y- and Z-values. These contraints are used to annihilate forces on set axis.
	/// </summary>
	[Serializable]
	public class PlaygroundAxisConstraintsC {
		/// <summary>
		/// The constraint on X-axis.
		/// </summary>
		public bool x = false;
		/// <summary>
		/// The constraint on Y-axis.
		/// </summary>
		public bool y = false;
		/// <summary>
		/// The constraint on Z-axis.
		/// </summary>
		public bool z = false;
		
		/// <summary>
		/// Determines whether any constraints has been enabled.
		/// </summary>
		/// <returns><c>true</c> if any constraints are enabled; otherwise, <c>false</c>.</returns>
		public bool HasConstraints () {
			return x||y||z;
		}
		
		/// <summary>
		/// Clone this PlaygroundAxisConstraintsC.
		/// </summary>
		public PlaygroundAxisConstraintsC Clone () {
			PlaygroundAxisConstraintsC axisConstraints = new PlaygroundAxisConstraintsC();
			axisConstraints.x = x;
			axisConstraints.y = y;
			axisConstraints.z = z;
			return axisConstraints;
		}
	}
	
	/// <summary>
	/// Contains information for a color gradient.
	/// </summary>
	[Serializable]
	public class PlaygroundGradientC {
		public Gradient gradient = new Gradient();
		
		/// <summary>
		/// Copies this gradient into the passed reference (copy).
		/// </summary>
		/// <param name="copy">Copy.</param>
		public void CopyTo (PlaygroundGradientC copy) {
			copy.gradient.SetKeys (gradient.colorKeys, gradient.alphaKeys);
		}
	}
	
	/// <summary>
	/// Holds information for a Playground Event.
	/// </summary>
	[Serializable]
	public class PlaygroundEventC {
		/// <summary>
		/// Is this PlaygroundEvent enabled?
		/// </summary>
		public bool enabled = true;
		/// <summary>
		/// Should events be sent to PlaygroundC.particleEvent?
		/// </summary>
		public bool sendToManager = false;
		/// <summary>
		/// Has this PlaygroundEvent initialized with its target?
		/// </summary>
		[NonSerialized] public bool initializedTarget = false;		
		/// <summary>
		/// Does this PlaygroundEvent have any subscribers to the particleEvent?
		/// </summary>
		[NonSerialized] public bool initializedEvent = false;		
		/// <summary>
		/// The target particle system to send events to.
		/// </summary>
		public PlaygroundParticlesC target;							
		/// <summary>
		/// The broadcast type of this event (A PlaygroundParticlesC target and/or Event Listeners).
		/// </summary>
		public EVENTBROADCASTC broadcastType;						
		/// <summary>
		/// The type of event.
		/// </summary>
		public EVENTTYPEC eventType;								
		/// <summary>
		/// The event of a particle (when using Event Listeners).
		/// </summary>
		public event OnPlaygroundParticle particleEvent;			
		/// <summary>
		/// The inheritance method of position.
		/// </summary>
		public EVENTINHERITANCEC eventInheritancePosition;			
		/// <summary>
		/// The inheritance method of velocity.
		/// </summary>
		public EVENTINHERITANCEC eventInheritanceVelocity;			
		/// <summary>
		/// The inheritance method of color.
		/// </summary>
		public EVENTINHERITANCEC eventInheritanceColor;				
		/// <summary>
		/// The position to send (if inheritance is User).
		/// </summary>
		public Vector3 eventPosition;								
		/// <summary>
		/// The velocity to send (if inheritance is User).
		/// </summary>
		public Vector3 eventVelocity;								
		/// <summary>
		/// The color to send (if inheritance is User).
		/// </summary>
		public Color32 eventColor = Color.white;					
		/// <summary>
		/// The time between events (if type is set to Time).
		/// </summary>
		public float eventTime = 1f;								
		/// <summary>
		/// The magnitude threshold to trigger collision events.
		/// </summary>
		public float collisionThreshold = 10f;						
		/// <summary>
		/// The multiplier of velocity.
		/// </summary>
		public float velocityMultiplier = 1f;						
		/// <summary>
		/// Calculated timer to trigger timed events.
		/// </summary>
		float timer = 0f;											
		
		/// <summary>
		/// Initialize this Playground Event.
		/// </summary>
		public void Initialize () {
			initializedTarget = (target!=null && target.gameObject.activeSelf && target.gameObject.activeInHierarchy && target.enabled);
			initializedEvent = (particleEvent!=null);
		}
		
		/// <summary>
		/// Determines if this Playground Event is ready to send a time-based event.
		/// </summary>
		/// <returns><c>true</c>, if time was updated, <c>false</c> otherwise.</returns>
		public bool UpdateTime () {
			bool readyToSend = false;
			timer+=PlaygroundC.globalDeltaTime;
			if (timer>=eventTime) {
				readyToSend = true;
				timer = 0f;
			}
			return readyToSend;
		}
		
		/// <summary>
		/// Sets the timer for time-based events.
		/// </summary>
		/// <param name="newTime">New time.</param>
		public void SetTimer (float newTime) {
			timer = newTime;
		}
		
		/// <summary>
		/// Sends the particle event.
		/// </summary>
		/// <param name="eventParticle">Event particle.</param>
		public void SendParticleEvent (PlaygroundEventParticle eventParticle) {
			if (initializedEvent)
				particleEvent(eventParticle);
		}
		
		/// <summary>
		/// Return a copy of this PlaygroundEventC.
		/// </summary>
		public PlaygroundEventC Clone () {
			PlaygroundEventC playgroundEvent = new PlaygroundEventC();
			playgroundEvent.enabled = enabled;
			playgroundEvent.target = target;
			playgroundEvent.eventType = eventType;
			playgroundEvent.eventInheritancePosition = eventInheritancePosition;
			playgroundEvent.eventInheritanceVelocity = eventInheritanceVelocity;
			playgroundEvent.eventInheritanceColor = eventInheritanceColor;
			playgroundEvent.eventPosition = eventPosition;
			playgroundEvent.eventVelocity = eventVelocity;
			playgroundEvent.eventColor = eventColor;
			playgroundEvent.eventTime = eventTime;
			playgroundEvent.collisionThreshold = collisionThreshold;
			playgroundEvent.velocityMultiplier = velocityMultiplier;
			playgroundEvent.particleEvent = particleEvent;
			playgroundEvent.broadcastType = broadcastType;
			return playgroundEvent;
		}
	}
	
	/// <summary>
	/// The type of Manipulator.
	/// </summary>
	public enum MANIPULATORTYPEC {
		/// <summary>
		/// The Manipulator will remain passive. It will still be able to track particles and send events.
		/// </summary>
		None,
		/// <summary>
		/// Attract particles in a funnel shape.
		/// </summary>
		Attractor,
		/// <summary>
		/// Attract particles with spherical gravitation.
		/// </summary>
		AttractorGravitational,
		/// <summary>
		/// Repel particles away from the Manipulator.
		/// </summary>
		Repellent,
		/// <summary>
		/// The Manipulator will alter chosen property of the affected particles.
		/// </summary>
		Property,
		/// <summary>
		/// Combine properties into one Manipulator call.
		/// </summary>
		Combined,
		/// <summary>
		/// Apply forces with vortex-like features. Manipulator's rotation will determine the direction. 
		/// </summary>
		Vortex
	}
	
	/// <summary>
	/// The type of Manipulator Property.
	/// </summary>
	public enum MANIPULATORPROPERTYTYPEC {
		/// <summary>
		/// No property will be affected. The Manipulator will still be able to track particles and send events.
		/// </summary>
		None,
		/// <summary>
		/// Changes the color of particles within range.
		/// </summary>
		Color,
		/// <summary>
		/// Sets a static velocity of particles within range.
		/// </summary>
		Velocity,
		/// <summary>
		/// Adds velocity over time of particles within range.
		/// </summary>
		AdditiveVelocity,
		/// <summary>
		/// Changes size of particles within range.
		/// </summary>
		Size,
		/// <summary>
		/// Sets Transform targets for particles within range.
		/// </summary>
		Target,
		/// <summary>
		/// Sets particles to a sooner death.
		/// </summary>
		Death,
		/// <summary>
		/// Attract particles in a funnel shape. This is a main feature injected as a property so you can use it inside a Combined Manipulator.
		/// </summary>
		Attractor,
		/// <summary>
		/// Attract particles with spherical gravitation. This is a main feature injected as a property so you can use it inside a Combined Manipulator.
		/// </summary>
		Gravitational,
		/// <summary>
		/// Repel particles away from the Manipulator. This is a main feature injected as a property so you can use it inside a Combined Manipulator.
		/// </summary>
		Repellent,
		/// <summary>
		/// Change the lifetime color of particles within range. This is a main feature injected as a property so you can use it inside a Combined Manipulator.
		/// </summary>
		LifetimeColor,
		/// <summary>
		/// Apply forces with vortex-like features. Manipulator's rotation will determine the direction.
		/// </summary>
		Vortex,
		/// <summary>
		/// Sets a mesh vertices in the scene as target.
		/// </summary>
		MeshTarget,
		/// <summary>
		/// Sets a skinned mesh vertices in the scene as target.
		/// </summary>
		SkinnedMeshTarget,
		/// <summary>
		/// Apply turbulence for particles within range.
		/// </summary>
		Turbulence,
		/// <summary>
		/// Sets a State in the scene as target.
		/// </summary>
		StateTarget,
		/// <summary>
		/// Sets a Playground Spline in the scene as target.
		/// </summary>
		SplineTarget,
		/// <summary>
		/// Alters particle properties by common math operations.
		/// </summary>
		Math
	}
	
	/// <summary>
	/// The math operation to apply within a MathManipulatorProperty class.
	/// </summary>
	public enum MATHMANIPULATORTYPE {
		/// <summary>
		/// Sine algorithm.
		/// </summary>
		Sin,
		/// <summary>
		/// Cosine algorithm.
		/// </summary>
		Cos,
		/// <summary>
		/// Linear interpolation algorithm.
		/// </summary>
		Lerp,
		/// <summary>
		/// Addition.
		/// </summary>
		Add,
		/// <summary>
		/// Subtraction.
		/// </summary>
		Subtract
	}
	
	/// <summary>
	/// The particle property to change within a MathManipulatorProperty class.
	/// </summary>
	public enum MATHMANIPULATORPROPERTY {
		/// <summary>
		/// Particle size.
		/// </summary>
		Size,
		/// <summary>
		/// Particle rotation.
		/// </summary>
		Rotation,
		/// <summary>
		/// Particle velocity.
		/// </summary>
		Velocity,
		/// <summary>
		/// Particle position.
		/// </summary>
		Position
	}
	
	/// <summary>
	/// The clamping (minimum to maximum) value to apply within a MathManipulatorProperty class.
	/// </summary>
	public enum MATHMANIPULATORCLAMP {
		/// <summary>
		/// No clamping will occur.
		/// </summary>
		None,
		/// <summary>
		/// The particle in-data will be clamped, before any changes has been made.
		/// </summary>
		ClampInValue,
		/// <summary>
		/// The particle out-data will be clamped, after the changes has been made.
		/// </summary>
		ClampOutValue
	}
	
	/// <summary>
	/// The type of Manipulator Property Transition.
	/// </summary>
	public enum MANIPULATORPROPERTYTRANSITIONC {
		/// <summary>
		/// No transition of the property will occur.
		/// </summary>
		None,
		/// <summary>
		/// Transition the property from current particle value to the Manipulator Property's effect using linear interpolation.
		/// </summary>
		Lerp,
		/// <summary>
		/// Transition the property from current particle value to the Manipulator Property's effect using MoveTowards interpolation.
		/// </summary>
		Linear
	}
	
	/// <summary>
	/// The shape of a Manipulator.
	/// </summary>
	public enum MANIPULATORSHAPEC {
		/// <summary>
		/// Spherical shape where the Manipulator's size will determine its extents.
		/// </summary>
		Sphere,
		/// <summary>
		/// A bounding box shape where the Manipulator's bounds will determine its extents.
		/// </summary>
		Box,
		/// <summary>
		/// The size of the Manipulator will be infinite over the scene and affect all particles. This option is the most performant as particle distance/containment check won't be necessary.
		/// </summary>
		Infinite
	}
	
	public enum PLAYGROUNDORIGINC {
		TopLeft, TopCenter, TopRight,
		MiddleLeft, MiddleCenter, MiddleRight,
		BottomLeft, BottomCenter, BottomRight
	}
	
	/// <summary>
	/// The mode to read pixels in.
	/// </summary>
	public enum PIXELMODEC {
		/// <summary>
		/// Bilinear pixel filtering.
		/// </summary>
		Bilinear,
		/// <summary>
		/// Pixel32 pixel filtering.
		/// </summary>
		Pixel32
	}
	
	/// <summary>
	/// The mode to set particles color source from.
	/// </summary>
	public enum COLORSOURCEC {
		/// <summary>
		/// Colors will extract from the source such as pixel- or painted color.
		/// </summary>
		Source,
		/// <summary>
		/// Colors will extract from a color gradient over lifetime.
		/// </summary>
		LifetimeColor,
		/// <summary>
		/// Colors will extract from a list of color gradients over lifetime.
		/// </summary>
		LifetimeColors
	}
	
	public enum COLORMETHOD {
		Lifetime,
		ParticleArray
	}
	
	/// <summary>
	/// The mode to overflow particles with when using Overflow Offset.
	/// </summary>
	public enum OVERFLOWMODEC {
		/// <summary>
		/// Overflow in the direction of the set Source Transform.
		/// </summary>
		SourceTransform,
		/// <summary>
		/// Overflow in world space direction.
		/// </summary>
		World,
		/// <summary>
		/// Overflow with normal direction of Source's points.
		/// </summary>
		SourcePoint
	}
	
	/// <summary>
	/// The type of transition used for Snapshots.
	/// </summary>
	public enum TRANSITIONTYPEC {
		/// <summary>
		/// Transition with no easing.
		/// </summary>
		Linear,
		/// <summary>
		/// Transition with slow start - fast finish.
		/// </summary>
		EaseIn,
		/// <summary>
		/// Transition with fast start - slow finish.
		/// </summary>
		EaseOut,
	}
	
	/// <summary>
	/// The individual type of transition used for Snapshots.
	/// </summary>
	public enum INDIVIDUALTRANSITIONTYPEC {
		/// <summary>
		/// Inherit the transition type selected for all Snapshots using INDIVIDUALTRANSITIONTYPEC.Inherit.
		/// </summary>
		Inherit,
		/// <summary>
		/// Transition with no easing.
		/// </summary>
		Linear,
		/// <summary>
		/// Transition with slow start - fast finish.
		/// </summary>
		EaseIn,
		/// <summary>
		/// Transition with fast start - slow finish.
		/// </summary>
		EaseOut,
	}
	
	/// <summary>
	/// The linear interpolition type used for Snapshots.
	/// </summary>
	public enum LERPTYPEC {
		PositionColor,
		Position,
		Color,
	}
	
	/// <summary>
	/// The Source which particle birth positions will distribute from.
	/// </summary>
	public enum SOURCEC {
		/// <summary>
		/// Set birth positions from a mesh vertices or a texture's pixels. Use a Transform to be able to translate, rotate and scale your State.
		/// </summary>
		State,
		/// <summary>
		/// Set birth positions from a single transform.
		/// </summary>
		Transform,
		/// <summary>
		/// Set birth positions from a mesh vertices in the scene.
		/// </summary>
		WorldObject,
		/// <summary>
		/// Set birth positions from a skinned mesh vertices in the scene.
		/// </summary>
		SkinnedWorldObject,
		/// <summary>
		/// Emission will be controlled by script using PlaygroundParticlesC.Emit().
		/// </summary>
		Script,
		/// <summary>
		/// Set birth positions by painting onto colliders (2d/3d) in the scene. 
		/// </summary>
		Paint,
		/// <summary>
		/// Project birth positions onto colliders (2d/3d) in the scene by using a texture. Note that this Source distribution is not multithreaded due to the non thread-safe Raycast method.
		/// </summary>
		Projection,
		/// <summary>
		/// Set birth positions from a Playground Spline.
		/// </summary>
		Spline,
	}
	
	public enum SOURCEBIRTHMETHOD {
		BirthPositions,
		ParticlePositions
	}
	
	/// <summary>
	/// The scale method. This is mainly used for Playground Transforms and determines if local- or lossy scale should be used when setting a transform matrix.
	/// </summary>
	public enum ScaleMethod {
		/// <summary>
		/// Use local scale.
		/// </summary>
		Local,
		/// <summary>
		/// Use lossy scale.
		/// </summary>
		Lossy
	}
	
	/// <summary>
	/// The lifetime sorting method.
	/// </summary>
	public enum SORTINGC {
		/// <summary>
		/// Sort particle emission randomly over their lifetime cycle.
		/// </summary>
		Scrambled,
		/// <summary>
		/// Sort particle emission randomly over their lifetime cycle and ensure consistent rate.
		/// </summary>
		ScrambledLinear,
		/// <summary>
		/// Sort particles to emit all at once.
		/// </summary>
		Burst,
		/// <summary>
		/// Sort particle emission alpha to omega in their Source structure.
		/// </summary>
		Linear,
		/// <summary>
		/// Sort particle emission omega to alpha in their Source structure.
		/// </summary>
		Reversed,
		/// <summary>
		/// Sort particle emission with an origin and use alpha to omega distance.
		/// </summary>
		NearestNeighbor,
		/// <summary>
		/// Sort particle emission with an origin and use omega to alpha distance.
		/// </summary>
		NearestNeighborReversed,
		/// <summary>
		/// Sort particle emission with an AnimationCurve. The X-axis represents the normalized lifetime cycle and Y-axis the normalized emission percentage.
		/// </summary>
		Custom
	}
	
	/// <summary>
	/// The brush detail level.
	/// </summary>
	public enum BRUSHDETAILC {
		/// <summary>
		/// Every pixel will be read (100% of existing texture pixels).
		/// </summary>
		Perfect,
		/// <summary>
		/// Every second pixel will be read (50% of existing texture pixels).
		/// </summary>
		High,
		/// <summary>
		/// Every forth pixel will be read (25% of existing texture pixels).
		/// </summary>
		Medium,
		/// <summary>
		/// Every sixth pixel will be read (16.6% of existing texture pixels).
		/// </summary>
		Low
	}
	
	/// <summary>
	/// The collision method.
	/// </summary>
	public enum COLLISIONTYPEC {
		/// <summary>
		/// Uses Raycast to detect colliders in scene.
		/// </summary>
		Physics3D,
		/// <summary>
		/// Uses Raycast2D to detect colliders in scene.
		/// </summary>
		Physics2D
	}
	
	/// <summary>
	/// The type of velocity bending.
	/// </summary>
	public enum VELOCITYBENDINGTYPEC {
		/// <summary>
		/// Bending will be calculated from each particle's birth position delta (previous and current frame).
		/// </summary>
		SourcePosition,
		/// <summary>
		/// Bending will be calculated from each particle's position delta (previous and current frame).
		/// </summary>
		ParticleDeltaPosition
	}
	
	/// <summary>
	/// The method to sort targets.
	/// </summary>
	public enum TARGETSORTINGC {
		Scrambled,
		Linear,
		Reversed	
	}
	
	/// <summary>
	/// The method to sort particle masking.
	/// </summary>
	public enum MASKSORTINGC {
		Linear,
		Reversed,
		Scrambled
	}
	
	/// <summary>
	/// The method to set the Lifetime Sorting: Nearest Neighbor/Reversed with.
	/// </summary>
	public enum NEARESTNEIGHBORORIGINMETHOD {
		/// <summary>
		/// The origin will be set with an int of a generated source point.
		/// </summary>
		SourcePoint,
		/// <summary>
		/// The origin will be set by a Vector3 in world space.
		/// </summary>
		Vector3,
		/// <summary>
		/// The origin will be set from a Transform's position in the scene. 
		/// </summary>
		Transform
	}
	
	/// <summary>
	/// The method to target a spline.
	/// </summary>
	public enum SPLINETARGETMETHOD {
		/// <summary>
		/// Match each particle linearly within the particle array with the spline's normalized time.
		/// </summary>
		SplineTime,
		/// <summary>
		/// Match each particle's lifetime with the normalized time of the spline.
		/// </summary>
		ParticleTime
	}
	
	/// <summary>
	/// The shape of a minimum to maximum Vector3.
	/// </summary>
	public enum MINMAXVECTOR3METHOD {
		Rectangular,
		RectangularLinear,
		Spherical,
		SphericalLinear,
		//SphericalSector,
		//SphericalSectorLinear
	}
	
	/// <summary>
	/// The type of event.
	/// </summary>
	public enum EVENTTYPEC {
		Birth,
		Death,
		Collision,
		Time
	}
	
	/// <summary>
	/// The type of event broadcast.
	/// </summary>
	public enum EVENTBROADCASTC {
		Target,
		EventListeners,
		Both
	}
	
	/// <summary>
	/// The inheritance method for events.
	/// </summary>
	public enum EVENTINHERITANCEC {
		User,
		Particle,
		Source
	}
	
	/// <summary>
	/// The type of turbulence algorithm to use.
	/// </summary>
	public enum TURBULENCETYPE {
		/// <summary>
		/// No turbulence will apply.
		/// </summary>
		None,
		/// <summary>
		/// Simplex noise will produce a natural branch pattern of turbulence.
		/// </summary>
		Simplex,
		/// <summary>
		/// Perlin noise will produce a confined wave-like pattern of turbulence.
		/// </summary>
		Perlin
	}
	
	/// <summary>
	/// The value method used for lifetime.
	/// </summary>
	public enum VALUEMETHOD {
		Constant,
		RandomBetweenTwoValues
	}
	
	/// <summary>
	/// The method to call when a particle system has finished simulating.
	/// </summary>
	public enum ONDONE {
		/// <summary>
		/// The GameObject of the particle system will inactivate.
		/// </summary>
		Inactivate,
		/// <summary>
		/// The GameObject of the particle system will be destroyed. This will only execute in Play-mode.
		/// </summary>
		Destroy
	}
	
	/// <summary>
	/// The thread pool method determines which thread pooling should be used.
	/// ThreadPoolMethod.ThreadPool will use the .NET managed ThreadPool class (standard in Particle Playground in versions prior to 3).
	/// ThreadPoolMethod.PlaygroundPool will use the self-managed PlaygroundQueue which queues tasks on reused threads (standard in Particle Playground 3 and up).
	/// </summary>
	public enum ThreadPoolMethod {
		/// <summary>
		/// Uses the .NET managed ThreadPool class (standard in Particle Playground in versions prior to 3).
		/// </summary>
		ThreadPool,
		/// <summary>
		/// Uses the self-managed PlaygroundQueue which queues tasks on reused threads (standard in Particle Playground 3 and up).
		/// </summary>
		PlaygroundPool
	}
	
	/// <summary>
	/// Multithreading method. This determines how particle systems calculate over the CPU. Keep in mind each thread will generate memory garbage which will be collected at some point.
	/// Selecting ThreadMethod.NoThreads will make particle systems calculate on the main-thread.
	/// ThreadMethod.OnePerSystem will create one thread per particle system each frame.
	/// ThreadMethod.OneForAll will bundle all calculations into one single thread.
	/// ThreadMethod.Automatic will distribute all particle systems evenly bundled along available CPUs/cores.
	/// </summary>
	public enum ThreadMethod {
		/// <summary>
		/// No calculation threads will be created. This will in most cases have a negative impact on performance as Particle Playground will calculate along all other logic on the main-thread.
		/// Use this for debug purposes or if you know there's no multi- or hyperthreading possibilities on your target platform.
		/// </summary>
		NoThreads,
		/// <summary>
		/// One calculation thread per particle system will be created. Use this when having heavy particle systems in your scene. 
		/// Note that this method will never bundle calculation calls unless specified in each individual particle system’s Particle Thread Method.
		/// </summary>
		OnePerSystem,
		/// <summary>
		/// One calculation thread for all particle systems will be created. Use this if you have other multithreaded logic which has higher performance priority than Particle Playground or your project demands strict use of garbage collection.
		/// Consider using ThreadMethod.Automatic for best performance.
		/// </summary>
		OneForAll,
		/// <summary>
		/// Let calculation threads distribute evenly for all particle systems in your scene. This will bundle calculation calls to match the platform's SystemInfo.processorCount. 
		/// This is the recommended and overall fastest method to calculate particle systems.
		/// Having fewer particle systems than processing units will create one thread per particle system. Having more particle systems than processing units will initiate thread bundling.
		/// </summary>
		Automatic
	}
	
	/// <summary>
	/// The multithreading method for a single particle system. Use this to bypass the selected PlaygroundC.threadMethod.
	/// ThreadMethodLocal.Inherit will let the particle system calculate as set by PlaygroundC.threadMethod. This is the default value.
	/// ThreadMethodLocal.NoThreads will make the particle system calculate on the main-thread.
	/// ThreadMethodLocal.OnePerSystem will create a new thread for this particle system.
	/// ThreadMethodLocal.OneForAll will create a bundled thread for all particle systems using this setting.
	/// </summary>
	public enum ThreadMethodLocal {
		/// <summary>
		/// Let the particle system calculate as set by ThreadMethod. This is the default value.
		/// </summary>
		Inherit,
		/// <summary>
		/// The particle system will be calculated on the main-thread.
		/// </summary>
		NoThreads,
		/// <summary>
		/// Creates a new thread for this particle system.
		/// </summary>
		OnePerSystem,
		/// <summary>
		/// Bundle all particle systems using this setting into a single thread call.
		/// </summary>
		OneForAll
	}
	
	/// <summary>
	/// The multithreading method for a particle system component such as tubulence and skinned meshes. This determines how the components will calculate over the CPU.
	/// </summary>
	public enum ThreadMethodComponent {
		/// <summary>
		/// The component will calculate inside the chosen particle system multithreading method.
		/// </summary>
		InsideParticleCalculation,
		/// <summary>
		/// Each component will create a new calculation thread.
		/// </summary>
		OnePerSystem,
		/// <summary>
		/// All components of type will create a single bundled calculation thread.
		/// </summary>
		OneForAll
	}
	
	/// <summary>
	/// Tracking method determines how particles are compared within a Manipulator. This is used when tracking particles to determine if a particle exists within a Manipulator. 
	/// Using Manipulator id is the fastest but may not work on overlapping Manipulators. Use this method if you have plenty of particles.
	/// Particle id will always return the correct compare, but requires a particle-to-particle lookup. Use this method if you have few particles with several Manipulators overlapping.
	/// </summary>
	public enum TrackingMethod {
		/// <summary>
		/// Using Manipulator id is fast but may not work on overlapping Manipulators. Use this method if you have plenty of particles.
		/// </summary>
		ManipulatorId,
		/// <summary>
		/// Particle id will always return the correct compare, but requires a particle-to-particle lookup. Use this method if you have few particles with several Manipulators overlapping.
		/// </summary>
		ParticleId
	}
	
	/// <summary>
	/// Animation curve extensions.
	/// </summary>
	public static class AnimationCurveExtensions {
		
		/// <summary>
		/// Resets the AnimationCurve with two keyframes at time 0 and 1, values are 0.
		/// </summary>
		/// <param name="animationCurve">Animation Curve.</param>
		public static void Reset (this AnimationCurve animationCurve) {
			if (animationCurve==null)
				animationCurve = new AnimationCurve();
			Keyframe[] keys = new Keyframe[2];
			keys[1].time = 1f;
			animationCurve.keys = keys;
		}
		
		/// <summary>
		/// Resets the AnimationCurve with two keyframes at time 0 and 1, values are 1.
		/// </summary>
		/// <param name="animationCurve">Animation Curve.</param>
		public static void Reset1 (this AnimationCurve animationCurve) {
			if (animationCurve==null)
				animationCurve = new AnimationCurve();
			Keyframe[] keys = new Keyframe[2];
			keys[1].time = 1f;
			keys[0].value = 1f;
			keys[1].value = 1f;
			animationCurve.keys = keys;
		}
		
		public static void Reset01 (this AnimationCurve animationCurve) {
			if (animationCurve==null)
				animationCurve = new AnimationCurve();
			Keyframe[] keys = new Keyframe[2];
			keys[1].time = 1f;
			keys[0].value = 0;
			keys[1].value = 1f;
			animationCurve.keys = keys;
		}
	}
	
	/// <summary>
	/// Event delegate for sending a PlaygroundEventParticle to any event listeners.
	/// </summary>
	public delegate void OnPlaygroundParticle(PlaygroundEventParticle particle);
}
