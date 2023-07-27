using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using ParticlePlayground;
using ParticlePlaygroundLanguage;
using PlaygroundSplines;

[CustomEditor (typeof(PlaygroundParticlesC))]
class PlaygroundParticleSystemInspectorC : Editor {
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundParticles variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public static PlaygroundParticlesC playgroundParticlesScriptReference;
	public static SerializedObject playgroundParticles;				// PlaygroundParticlesC
	public static SerializedProperty source;						// SOURCEC
	public static SerializedProperty sorting;						// SORTINGC
	public static SerializedProperty lifetimeSorting;				// AnimationCurve
	public static SerializedProperty activeState;					// int
	public static SerializedProperty particleCount;					// int
	public static SerializedProperty emissionRate;					// float
	public static SerializedProperty updateRate;					// int
	public static SerializedProperty worldObjectUpdateVertices;		// bool
	public static SerializedProperty worldObjectUpdateNormals;		// bool
	public static SerializedProperty emit;							// bool
	public static SerializedProperty loop;							// bool
	public static SerializedProperty disableOnDone;					// bool
	public static SerializedProperty disableOnDoneRoutine;			// ONDONE
	public static SerializedProperty calculate;						// bool
	public static SerializedProperty deltaMovementStrength;			// float
	public static SerializedProperty particleTimescale;				// float
	public static SerializedProperty sizeMin;						// float
	public static SerializedProperty sizeMax;						// float
	public static SerializedProperty lifetime;						// float
	public static SerializedProperty lifetimeSize;					// AnimationCurve
	public static SerializedProperty arraySize;						// AnimationCurve
	public static SerializedProperty onlySourcePositioning;			// bool
	public static SerializedProperty applyLifetimeVelocity;			// bool
	public static SerializedProperty applyInitialVelocity;			// bool
	public static SerializedProperty applyInitialLocalVelocity;		// bool
	public static SerializedProperty applyVelocityBending;			// bool
	public static SerializedProperty velocityBendingType;			// VELOCITYBENDINGTYPE
	public static SerializedProperty lifetimeVelocity;				// Vector3AnimationCurveC
	public static SerializedProperty initialVelocityShape;			// Vector3AnimationCurveC
	public static SerializedProperty overflowOffset;				// Vector3
	public static SerializedProperty overflowMode;					// OVERFLOWMODEC
	public static SerializedProperty initialVelocityMin;			// Vector3
	public static SerializedProperty initialVelocityMax;			// Vector3
	public static SerializedProperty initialLocalVelocityMin;		// Vector3
	public static SerializedProperty initialLocalVelocityMax;		// Vector3
	public static SerializedProperty turbulenceLifetimeStrength;	// AnimationCurve
	public static SerializedProperty lifetimeColor;					// Gradient
	public static SerializedProperty lifetimeColors;				// List<Gradient>
	public static SerializedProperty arrayColor;					// Gradient
	public static SerializedProperty colorSource;					// COLORSOURCEC
	public static SerializedProperty collision;						// bool
	public static SerializedProperty affectRigidbodies;				// bool
	public static SerializedProperty mass;							// float
	public static SerializedProperty collisionRadius;				// float
	public static SerializedProperty collisionMask;					// LayerMask
	public static SerializedProperty stickyCollisionMask;			// LayerMask
	public static SerializedProperty collisionType;					// COLLISIONTYPE
	public static SerializedProperty bounciness;					// float
	public static SerializedProperty lifetimeStretching;			// AnimationCurve
	public static UnityEngine.Object particleMaterial;				// Material
	public static SerializedProperty threadMethod;					// ThreadMethodLocal
	
	public static SerializedProperty states;						// List<ParticleStateC>
	public static SerializedProperty worldObject;					// WorldObjectC
	public static SerializedProperty worldObjectGameObject;			// GameObject
	public static SerializedProperty skinnedWorldObject;			// SkinnedWorldObjectC
	public static SerializedProperty skinnedWorldObjectGameObject; 	// GameObject
	public static SerializedProperty forceSkinnedMeshUpdateOnMainThread; // bool
	public static SerializedProperty sourceTransform;				// Transform
	public static SerializedProperty sourcePaint;					// PaintObjectC
	public static SerializedProperty sourceProjection;				// ParticleProjectionC
	public static SerializedProperty sourceSplines;					// List<PlaygroundSpline>
	public static SerializedProperty sourceTransforms;				// List<PlaygroundTransformC>
	
	public static SerializedProperty lifetimePositioning;			// Vector3AnimationCurveC
	public static SerializedProperty lifetimePositioningX;			// AnimationCurve
	public static SerializedProperty lifetimePositioningY;			// AnimationCurve
	public static SerializedProperty lifetimePositioningZ;			// AnimationCurve
	public static SerializedProperty lifetimePositioningTimeScale;	// AnimationCurve
	public static SerializedProperty lifetimePositioningPositionScale; // AnimationCurve
	
	public static SerializedProperty lifeTimeVelocityX;				// AnimationCurve
	public static SerializedProperty lifeTimeVelocityY;				// AnimationCurve
	public static SerializedProperty lifeTimeVelocityZ;				// AnimationCurve
	
	public static SerializedProperty initialVelocityShapeX;			// AnimationCurve
	public static SerializedProperty initialVelocityShapeY;			// AnimationCurve
	public static SerializedProperty initialVelocityShapeZ;			// AnimationCurve
	
	public static SerializedProperty movementCompensationLifetimeStrength; // AnimationCurve
	
	public static SerializedProperty manipulators;					// List<ManipulatorObjectC>
	public static SerializedProperty events;						// List<PlaygroundEventC>
	public static SerializedProperty snapshots;						// List<PlaygroundSave>
	public static ParticleSystemRenderer shurikenRenderer;			// ParticleSystemRenderer
	public static string[] rendererSortingLayers;					// string[]
	public static int selectedSortingLayer = 0;						// int
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Playground variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public static PlaygroundC playgroundScriptReference;			// PlaygroundC
	public static SerializedObject playground;						// PlaygroundC
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PlaygroundParticleSystemInspector variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	// States
	public static int meshOrImage;
	public static string addStateName = "";
	public static UnityEngine.Object addStateMesh;
	public static UnityEngine.Object addStateTexture;
	public static UnityEngine.Object addStateTransform;
	public static UnityEngine.Object addStateDepthmap;
	public static float addStateDepthmapStrength = 1f;
	public static float addStateSize = 1f;
	public static float addStateScale = 1f;
	public static Vector3 addStateOffset;
	
	// Foldout lists (all others are taken care of the Playground Settings)
	public static  List<bool> statesListFoldout;
	public static List<bool> eventListFoldout;
	
	// Paint variables
	public static int brushListStyle = 0;
	public static Color32 paintColor = new Color(1,1,1,1);
	public static bool useBrushColor = true;
	public static int selectedPaintMode;
	public static GUIStyle sceneBrushStyle;
	public static List<UnityEngine.Object> brushPrefabs;
	public static string[] brushNames;
	public static float[] paintSpacings;
	public static bool[] exceedMaxStopsPaintList;
	public static bool inPaintMode = false;
	public static UnityEngine.Object paintTexture;
	public static PlaygroundBrushC[] brushPresets;
	public static int selectedBrushPreset = -1;
	public static bool brushPresetFoldout = false;
	public static SerializedProperty paintLayerMask;
	public static SerializedProperty paintCollisionType;
	public static UnityEditor.Tool lastActiveTool = UnityEditor.Tool.None;
	public static float eraserRadius = 1f;
	private bool showNoAlphaWarning = false;
	
	// Projection variables
	public static SerializedProperty projectionMask;
	public static SerializedProperty projectionCollisionType;
	
	// GUI
	public static GUIStyle boxStyle;
	public static PlaygroundSettingsC playgroundSettings;
	public static PlaygroundLanguageC playgroundLanguage;
	
	public static bool currentWireframe;
	private Keyframe[] prevLifetimeSortingKeys;
	private SOURCEC previousSource;
	public static string saveName = "New Snapshot";
	
	public static bool isEditingInHierarchy;
	
	public static SerializedObject shuriken;
	public static SerializedObject shurikenRendererSO;
	
	public static SerializedProperty uvModule;
	public static SerializedProperty uvModule_enabled;							// Bool
	public static SerializedProperty uvModule_frameOverTime_scalar;				// Float
	public static SerializedProperty uvModule_frameOverTime_minCurve;			// AnimationCurve
	public static SerializedProperty uvModule_frameOverTime_maxCurve;			// AnimationCurve
	public static SerializedProperty uvModule_frameOverTime_minMaxState;		// Int
	public static SerializedProperty uvModule_tilesX;							// Int
	public static SerializedProperty uvModule_tilesY;							// Int
	public static SerializedProperty uvModule_animationType;					// Int
	public static SerializedProperty uvModule_rowIndex;							// Int
	public static SerializedProperty uvModule_cycles;							// Float
	public static SerializedProperty uvModule_randomRow;						// Bool
	public AnimationType uv_animationType;
	public MinMaxState uv_minMaxState;
	public AnimationCurve prev_uvModule_frameOverTime_minCurve;
	public AnimationCurve prev_uvModule_frameOverTime_maxCurve;

	public static SerializedProperty initialModule;
	public static SerializedProperty initialModule_maxNumParticles;				// Int
	
	public static SerializedProperty sortingMode;
	public static SerializedProperty sortingFudge;
	public SortMode sortMode;
	
	void OnEnable () {
		
		lastActiveTool = UnityEditor.Tools.current;
		isEditingInHierarchy = Selection.activeTransform!=null;
		
		// Load settings
		playgroundSettings = PlaygroundSettingsC.GetReference();
		
		// Load language
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();
		
		// Playground Particles
		playgroundParticlesScriptReference = target as PlaygroundParticlesC;
		if (playgroundParticlesScriptReference==null) return;
		playgroundParticles = new SerializedObject(playgroundParticlesScriptReference);
		
		shurikenRenderer = playgroundParticlesScriptReference.particleSystemGameObject.GetComponent<ParticleSystem>().GetComponent<Renderer>() as ParticleSystemRenderer;
		
		// Sorting layers
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		rendererSortingLayers = (string[])sortingLayersProperty.GetValue(null, new object[0]);
		for (int i = 0; i<rendererSortingLayers.Length; i++) {
			if (shurikenRenderer.sortingLayerName == rendererSortingLayers[i])
				selectedSortingLayer = i;
		}
		
		// UV Module (Texture Sheet Animation)
		shuriken = new SerializedObject(playgroundParticlesScriptReference.shurikenParticleSystem);
		uvModule = shuriken.FindProperty("UVModule");
		uvModule_enabled = uvModule.FindPropertyRelative("enabled");
		uvModule_frameOverTime_scalar = uvModule.FindPropertyRelative("frameOverTime.scalar");
		uvModule_frameOverTime_minCurve = uvModule.FindPropertyRelative("frameOverTime.minCurve");
		uvModule_frameOverTime_maxCurve = uvModule.FindPropertyRelative("frameOverTime.maxCurve");
		uvModule_frameOverTime_minMaxState = uvModule.FindPropertyRelative("frameOverTime.minMaxState");
		uvModule_tilesX = uvModule.FindPropertyRelative("tilesX");
		uvModule_tilesY = uvModule.FindPropertyRelative("tilesY");
		uvModule_animationType = uvModule.FindPropertyRelative("animationType");
		uvModule_rowIndex = uvModule.FindPropertyRelative("rowIndex");
		uvModule_cycles = uvModule.FindPropertyRelative("cycles");
		uvModule_randomRow = uvModule.FindPropertyRelative("randomRow");
		uv_animationType = (AnimationType)uvModule_animationType.intValue;
		uv_minMaxState = (MinMaxState)uvModule_frameOverTime_minMaxState.intValue;

		prev_uvModule_frameOverTime_minCurve = uvModule_frameOverTime_minCurve.animationCurveValue;
		prev_uvModule_frameOverTime_maxCurve = uvModule_frameOverTime_maxCurve.animationCurveValue;

		if (uv_minMaxState == MinMaxState.RandomBetweenTwoConstants) {
			AnimationCurve updCurve = new AnimationCurve();
			Keyframe[] updKeys = uvModule_frameOverTime_maxCurve.animationCurveValue.keys;
			updKeys[0].value = uvModule_frameOverTime_scalar.floatValue;
			updCurve.keys = updKeys;
			uvModule_frameOverTime_maxCurve.animationCurveValue = updCurve;
			EditorUtility.SetDirty(playgroundParticlesScriptReference.shurikenParticleSystem);
			shuriken.ApplyModifiedProperties();
		}

		// Shuriken particle count
		initialModule = shuriken.FindProperty("InitialModule");
		initialModule_maxNumParticles = initialModule.FindPropertyRelative("maxNumParticles");
		initialModule_maxNumParticles.intValue = playgroundParticlesScriptReference.particleCount;
		shuriken.ApplyModifiedProperties();
		
		shurikenRendererSO = new SerializedObject(shurikenRenderer);
		sortingMode = shurikenRendererSO.FindProperty("m_SortMode");
		sortingFudge = shurikenRendererSO.FindProperty("m_SortingFudge");
		sortMode = (SortMode)sortingMode.intValue;
		
		manipulators = playgroundParticles.FindProperty("manipulators");
		events = playgroundParticles.FindProperty("events");
		snapshots = playgroundParticles.FindProperty("snapshots");
		source = playgroundParticles.FindProperty("source");
		sorting = playgroundParticles.FindProperty("sorting");
		lifetimeSorting = playgroundParticles.FindProperty("lifetimeSorting");
		activeState = playgroundParticles.FindProperty("activeState");
		particleCount = playgroundParticles.FindProperty("particleCount");
		emissionRate = playgroundParticles.FindProperty("emissionRate");
		updateRate = playgroundParticles.FindProperty("updateRate");
		emit = playgroundParticles.FindProperty("emit");
		loop = playgroundParticles.FindProperty("loop");
		disableOnDone = playgroundParticles.FindProperty("disableOnDone");
		disableOnDoneRoutine = playgroundParticles.FindProperty("disableOnDoneRoutine");
		calculate = playgroundParticles.FindProperty("calculate");
		deltaMovementStrength = playgroundParticles.FindProperty("deltaMovementStrength");
		particleTimescale = playgroundParticles.FindProperty("particleTimescale");
		sizeMin = playgroundParticles.FindProperty("sizeMin");
		sizeMax = playgroundParticles.FindProperty("sizeMax");
		overflowOffset = playgroundParticles.FindProperty("overflowOffset");
		overflowMode = playgroundParticles.FindProperty("overflowMode");
		lifetime = playgroundParticles.FindProperty("lifetime");
		lifetimeSize = playgroundParticles.FindProperty("lifetimeSize");
		arraySize = playgroundParticles.FindProperty("particleArraySize");
		turbulenceLifetimeStrength = playgroundParticles.FindProperty("turbulenceLifetimeStrength");
		lifetimeVelocity = playgroundParticles.FindProperty("lifetimeVelocity");
		initialVelocityShape = playgroundParticles.FindProperty("initialVelocityShape");
		initialVelocityMin = playgroundParticles.FindProperty("initialVelocityMin");
		initialVelocityMax = playgroundParticles.FindProperty("initialVelocityMax");
		initialLocalVelocityMin = playgroundParticles.FindProperty("initialLocalVelocityMin");
		initialLocalVelocityMax = playgroundParticles.FindProperty("initialLocalVelocityMax");
		lifetimeColor = playgroundParticles.FindProperty("lifetimeColor");
		lifetimeColors = playgroundParticles.FindProperty ("lifetimeColors");
		arrayColor = playgroundParticles.FindProperty("arrayColorAlpha");
		colorSource = playgroundParticles.FindProperty("colorSource");
		collision = playgroundParticles.FindProperty("collision");
		affectRigidbodies = playgroundParticles.FindProperty("affectRigidbodies");
		mass = playgroundParticles.FindProperty("mass");
		collisionRadius = playgroundParticles.FindProperty("collisionRadius");
		collisionMask = playgroundParticles.FindProperty("collisionMask");
		stickyCollisionMask = playgroundParticles.FindProperty ("stickyCollisionsMask");
		collisionType = playgroundParticles.FindProperty("collisionType");
		bounciness = playgroundParticles.FindProperty("bounciness");
		states = playgroundParticles.FindProperty("states");
		worldObject = playgroundParticles.FindProperty("worldObject");
		skinnedWorldObject = playgroundParticles.FindProperty("skinnedWorldObject");
		forceSkinnedMeshUpdateOnMainThread = playgroundParticles.FindProperty ("forceSkinnedMeshUpdateOnMainThread");
		sourceTransform = playgroundParticles.FindProperty("sourceTransform");
		worldObjectUpdateVertices = playgroundParticles.FindProperty ("worldObjectUpdateVertices");
		worldObjectUpdateNormals = playgroundParticles.FindProperty("worldObjectUpdateNormals");
		sourcePaint = playgroundParticles.FindProperty("paint");
		sourceProjection = playgroundParticles.FindProperty("projection");
		sourceSplines = playgroundParticles.FindProperty("splines");
		sourceTransforms = playgroundParticles.FindProperty("sourceTransforms");
		lifetimeStretching = playgroundParticles.FindProperty("stretchLifetime");
		threadMethod = playgroundParticles.FindProperty("threadMethod");
		
		playgroundParticlesScriptReference.shurikenParticleSystem = playgroundParticlesScriptReference.GetComponent<ParticleSystem>();
		playgroundParticlesScriptReference.particleSystemRenderer = playgroundParticlesScriptReference.shurikenParticleSystem.GetComponent<Renderer>();
		particleMaterial = playgroundParticlesScriptReference.particleSystemRenderer.sharedMaterial;
		
		onlySourcePositioning = playgroundParticles.FindProperty("onlySourcePositioning");
		
		lifetimePositioning = playgroundParticles.FindProperty("lifetimePositioning");
		lifetimePositioningX = lifetimePositioning.FindPropertyRelative("x");
		lifetimePositioningY = lifetimePositioning.FindPropertyRelative("y");
		lifetimePositioningZ = lifetimePositioning.FindPropertyRelative("z");
		lifetimePositioningTimeScale = playgroundParticles.FindProperty ("lifetimePositioningTimeScale");
		lifetimePositioningPositionScale = playgroundParticles.FindProperty ("lifetimePositioningPositionScale");
		
		applyLifetimeVelocity = playgroundParticles.FindProperty("applyLifetimeVelocity");
		lifeTimeVelocityX = lifetimeVelocity.FindPropertyRelative("x");
		lifeTimeVelocityY = lifetimeVelocity.FindPropertyRelative("y");
		lifeTimeVelocityZ = lifetimeVelocity.FindPropertyRelative("z");
		
		initialVelocityShapeX = initialVelocityShape.FindPropertyRelative("x");
		initialVelocityShapeY = initialVelocityShape.FindPropertyRelative("y");
		initialVelocityShapeZ = initialVelocityShape.FindPropertyRelative("z");
		
		applyInitialVelocity = playgroundParticles.FindProperty("applyInitialVelocity");
		applyInitialLocalVelocity = playgroundParticles.FindProperty("applyInitialLocalVelocity");
		applyVelocityBending = playgroundParticles.FindProperty("applyVelocityBending");
		velocityBendingType = playgroundParticles.FindProperty("velocityBendingType");
		
		movementCompensationLifetimeStrength = playgroundParticles.FindProperty ("movementCompensationLifetimeStrength");
		
		worldObjectGameObject = worldObject.FindPropertyRelative("gameObject");
		skinnedWorldObjectGameObject = skinnedWorldObject.FindPropertyRelative("gameObject");
		
		// Lifetime colors
		if (playgroundParticlesScriptReference.lifetimeColors==null)
			playgroundParticlesScriptReference.lifetimeColors = new List<PlaygroundGradientC>();
		
		// Sorting
		prevLifetimeSortingKeys = playgroundParticlesScriptReference.lifetimeSorting.keys;
		
		// Events list
		eventListFoldout = new List<bool>();
		eventListFoldout.AddRange(new bool[playgroundParticlesScriptReference.events.Count]);
		
		// States foldout
		statesListFoldout = new List<bool>();
		statesListFoldout.AddRange(new bool[playgroundParticlesScriptReference.states.Count]);
		
		previousSource = playgroundParticlesScriptReference.source;
		
		// Playground
		playgroundScriptReference = FindObjectOfType<PlaygroundC>();
		
		
		// Create a manager if no existing instance is in the scene
		if (!playgroundScriptReference && Selection.activeTransform!=null) {
			PlaygroundC.ResourceInstantiate("Playground Manager");
			playgroundScriptReference = FindObjectOfType<PlaygroundC>();
		}
		
		if (playgroundScriptReference!=null) {
			
			PlaygroundC.reference = playgroundScriptReference;
			
			// Serialize Playground
			playground = new SerializedObject(playgroundScriptReference);
			
			PlaygroundInspectorC.Initialize(playgroundScriptReference);
			
			
			// Add this PlaygroundParticles if not existing in Playground list
			if (!playgroundParticlesScriptReference.isSnapshot && !playgroundScriptReference.particleSystems.Contains(playgroundParticlesScriptReference) && Selection.activeTransform!=null)
				playgroundScriptReference.particleSystems.Add(playgroundParticlesScriptReference);
			
			// Cache components
			playgroundParticlesScriptReference.particleSystemGameObject = playgroundParticlesScriptReference.gameObject;
			playgroundParticlesScriptReference.particleSystemTransform = playgroundParticlesScriptReference.transform;
			playgroundParticlesScriptReference.particleSystemRenderer = playgroundParticlesScriptReference.GetComponent<Renderer>();
			playgroundParticlesScriptReference.shurikenParticleSystem = playgroundParticlesScriptReference.particleSystemGameObject.GetComponent<ParticleSystem>();
			playgroundParticlesScriptReference.particleSystemRenderer2 = playgroundParticlesScriptReference.particleSystemGameObject.GetComponent<ParticleSystem>().GetComponent<Renderer>() as ParticleSystemRenderer;
			
			// Set manager as parent 
			//if (PlaygroundC.reference.autoGroup && playgroundParticlesScriptReference.particleSystemTransform!=null && playgroundParticlesScriptReference.particleSystemTransform.parent == null && Selection.activeTransform!=null)
			//	playgroundParticlesScriptReference.particleSystemTransform.parent = PlaygroundC.referenceTransform;
			
			// Issue a quick refresh
			
			if (!EditorApplication.isPlaying && isEditingInHierarchy) {
				foreach (PlaygroundParticlesC p in PlaygroundC.reference.particleSystems) {
					p.Start();
				}
			}
		}
		
		selectedSort = sorting.intValue;
		
		// State initial values
		if (addStateTransform==null)
			addStateTransform = (Transform)playgroundParticlesScriptReference.particleSystemTransform;
		
		// Visiblity of Shuriken component in Inspector
		if (!playgroundScriptReference || playgroundScriptReference && !playgroundScriptReference.showShuriken)
			playgroundParticlesScriptReference.shurikenParticleSystem.hideFlags = HideFlags.HideInInspector;
		else
			playgroundParticlesScriptReference.shurikenParticleSystem.hideFlags = HideFlags.None;
		
		SetWireframeVisibility();
		
		// Set paint init
		paintLayerMask = sourcePaint.FindPropertyRelative("layerMask");
		paintCollisionType = sourcePaint.FindPropertyRelative("collisionType");
		
		// Set projection init
		projectionMask = sourceProjection.FindPropertyRelative("projectionMask");
		projectionCollisionType = sourceProjection.FindPropertyRelative("collisionType");
		
		// Snapshots
		if (playgroundParticlesScriptReference.snapshots.Count>0) {
			if (playgroundParticlesScriptReference.snapshots.Count>0) {
				for (int i = 0; i<playgroundParticlesScriptReference.snapshots.Count; i++)
					if (playgroundParticlesScriptReference.snapshots[i].settings==null)
						playgroundParticlesScriptReference.snapshots.RemoveAt(i);
			}
			saveName += " "+(playgroundParticlesScriptReference.snapshots.Count+1).ToString();
		}
		
		SetMissingKeys();
	}
	
	public static void SetMissingKeys () {
		if (playgroundParticlesScriptReference.lifetimeSize==null || playgroundParticlesScriptReference.lifetimeSize.keys.Length==0)
			playgroundParticlesScriptReference.lifetimeSize.Reset1();
		if (playgroundParticlesScriptReference.particleArraySize==null || playgroundParticlesScriptReference.particleArraySize.keys.Length==0)
			playgroundParticlesScriptReference.particleArraySize.Reset1();
		if (playgroundParticlesScriptReference.lifetimePositioningTimeScale==null || playgroundParticlesScriptReference.lifetimePositioningTimeScale.keys.Length==0)
			playgroundParticlesScriptReference.lifetimePositioningTimeScale.Reset1();
		if (playgroundParticlesScriptReference.lifetimePositioningPositionScale==null || playgroundParticlesScriptReference.lifetimePositioningPositionScale.keys.Length==0)
			playgroundParticlesScriptReference.lifetimePositioningPositionScale.Reset1();
		if (playgroundParticlesScriptReference.transitionBackToSourceAmount==null || playgroundParticlesScriptReference.transitionBackToSourceAmount.keys.Length==0)
			playgroundParticlesScriptReference.transitionBackToSourceAmount.Reset01();
		
		if (!playgroundParticlesScriptReference.lifetimePositioning.HasKeys())
			playgroundParticlesScriptReference.lifetimePositioning.Reset();
		if (!playgroundParticlesScriptReference.lifetimeVelocity.HasKeys())
			playgroundParticlesScriptReference.lifetimeVelocity.Reset();
		if (!playgroundParticlesScriptReference.initialVelocityShape.HasKeys())
			playgroundParticlesScriptReference.initialVelocityShape.Reset1();
	}
	
	public static void SetWireframeVisibility () {
		
		if (Selection.activeTransform==null) return;
		
		// Wireframes in Scene View
		EditorUtility.SetSelectedWireframeHidden(playgroundParticlesScriptReference.particleSystemRenderer, !PlaygroundC.reference.drawWireframe);
		currentWireframe = PlaygroundC.reference.drawWireframe;
	}
	
	public static void LoadBrushes () {
		
		// Set brush presets and custom brush texture
		brushPrefabs = new List<UnityEngine.Object>();
		
		string assetsDataPath = Application.dataPath;
		string editorBrushPath = assetsDataPath+"/"+playgroundSettings.playgroundPath+playgroundSettings.brushPath;
		string[] editorBrushPaths = Directory.GetFiles (editorBrushPath);
		
		foreach (string thisBrushPath in editorBrushPaths) {
			string convertedBrushPath = thisBrushPath.Substring(assetsDataPath.Length-6);
			UnityEngine.Object brushPathObject = (UnityEngine.Object)AssetDatabase.LoadAssetAtPath(convertedBrushPath, typeof(UnityEngine.Object));
			if (brushPathObject!=null && (brushPathObject.GetType().Name)=="GameObject") {
				brushPrefabs.Add (brushPathObject);
			}
		}
		
		
		brushNames = new string[brushPrefabs.Count];
		paintSpacings = new float[brushPrefabs.Count];
		brushPresets = new PlaygroundBrushC[brushPrefabs.Count];
		exceedMaxStopsPaintList = new bool[brushPrefabs.Count];
		for (int i = 0; i<brushPresets.Length; i++) {
			GameObject thisBrushGO = (GameObject)Instantiate (brushPrefabs[i]);
			PlaygroundBrushPresetC thisBrushPrefab = thisBrushGO.GetComponent<PlaygroundBrushPresetC>();
			brushNames[i] = thisBrushPrefab.presetName;
			brushPresets[i] = new PlaygroundBrushC();
			brushPresets[i].texture = thisBrushPrefab.texture as Texture2D;
			brushPresets[i].detail = thisBrushPrefab.detail;
			brushPresets[i].scale = thisBrushPrefab.scale;
			brushPresets[i].distance = thisBrushPrefab.distance;
			
			paintSpacings[i] = thisBrushPrefab.spacing;
			exceedMaxStopsPaintList[i] = thisBrushPrefab.exceedMaxStopsPaint;
			DestroyImmediate (thisBrushGO);
		}
		
		if (source.intValue==5 && paintTexture!=null)
			SetBrush(selectedBrushPreset);
		
		
		if (playgroundParticlesScriptReference.paint!=null && playgroundParticlesScriptReference.paint.brush!=null && playgroundParticlesScriptReference.paint.brush.texture!=null) {
			paintTexture = playgroundParticlesScriptReference.paint.brush.texture;
		}
	}
	
	public static void SetBrush (int i) {
		if (i>=0 && i<brushPresets.Length) {
			TextureImporter tAssetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(brushPresets[i].texture as UnityEngine.Object)) as TextureImporter;
			if (!tAssetImporter.isReadable) {
				Debug.Log(tAssetImporter.assetPath+" "+playgroundLanguage.notReadable);
				return; 
			}
			selectedBrushPreset = i;
			paintTexture = brushPresets[selectedBrushPreset].texture;
			playgroundParticlesScriptReference.paint.brush.SetTexture(brushPresets[selectedBrushPreset].texture);
			playgroundParticlesScriptReference.paint.brush.scale = brushPresets[selectedBrushPreset].scale;
			playgroundParticlesScriptReference.paint.brush.detail = brushPresets[selectedBrushPreset].detail;
			playgroundParticlesScriptReference.paint.brush.distance = brushPresets[selectedBrushPreset].distance;
			
			playgroundParticlesScriptReference.paint.spacing = paintSpacings[selectedBrushPreset];
			playgroundParticlesScriptReference.paint.exceedMaxStopsPaint = exceedMaxStopsPaintList[selectedBrushPreset];
		} else {
			playgroundParticlesScriptReference.paint.brush.SetTexture(paintTexture as Texture2D);
		}
		
		// Set brush preview style
		sceneBrushStyle = new GUIStyle();
		sceneBrushStyle.imagePosition = ImagePosition.ImageOnly;
		sceneBrushStyle.border = new RectOffset(0,0,0,0);
		sceneBrushStyle.stretchWidth = true;
		sceneBrushStyle.stretchHeight = true;
		SetBrushStyle();
	}
	
	public static void SetBrushStyle () {
		if (playgroundParticlesScriptReference.paint.brush==null || playgroundParticlesScriptReference.paint.brush.texture==null || sceneBrushStyle==null) return;
		float brushScale = playgroundParticlesScriptReference.paint.brush.scale;
		sceneBrushStyle.fixedWidth = playgroundParticlesScriptReference.paint.brush.texture.width*brushScale;
		sceneBrushStyle.fixedHeight = playgroundParticlesScriptReference.paint.brush.texture.height*brushScale;
		sceneBrushStyle.contentOffset = -new Vector2(playgroundParticlesScriptReference.paint.brush.texture.width/2, playgroundParticlesScriptReference.paint.brush.texture.height/2)*brushScale;
	}
	
	void OnDestroy () {
		brushPresets = null;
		inPaintMode = false;
		UnityEditor.Tools.current = lastActiveTool;
	}
	
	public override void OnInspectorGUI () {
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");
		if (PlaygroundInspectorC.boxStyle==null)
			PlaygroundInspectorC.boxStyle = GUI.skin.FindStyle("box");
		isEditingInHierarchy = Selection.activeTransform!=null;
		/*
		if (isEditingInHierarchy) {
			EditorGUILayout.LabelField(playgroundLanguage.editFromHierarchyOnly);
			return;
		}
		*/
		
		
		if (Event.current.type == EventType.ValidateCommand &&
		    Event.current.commandName == "UndoRedoPerformed") {			
			LifetimeSorting();
		}
		
		playgroundParticles.Update();
		
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(playgroundLanguage.playgroundName+" "+PlaygroundC.version+PlaygroundC.specialVersion, EditorStyles.largeLabel, GUILayout.Height(20));
		
		EditorGUILayout.Separator();
		
		if(GUILayout.Button(playgroundLanguage.playgroundWizard, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
			PlaygroundParticleWindowC.ShowWindow();
		}
		if(GUILayout.Button(playgroundLanguage.createPreset, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
			PlaygroundCreatePresetWindowC.ShowWindow();
		}
		GUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		
		if (playgroundParticlesScriptReference.particleSystemTransform.localScale.x != 1f ||
		    playgroundParticlesScriptReference.particleSystemTransform.localScale.y != 1f ||
		    playgroundParticlesScriptReference.particleSystemTransform.localScale.z != 1f) {
			EditorGUILayout.BeginVertical(boxStyle);
			EditorGUILayout.HelpBox(playgroundLanguage.localScaleWarning, MessageType.Warning);
			if (GUILayout.Button (playgroundLanguage.fix, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
				playgroundParticlesScriptReference.particleSystemTransform.localScale = new Vector3(1f,1f,1f);
			EditorGUILayout.EndVertical();
		}
		
		// Particles
		EditorGUILayout.BeginVertical(boxStyle);
		if (playgroundParticlesScriptReference.eventControlledBy.Count>0)
			playgroundSettings.particlesFoldout = GUILayout.Toggle(playgroundSettings.particlesFoldout, playgroundLanguage.playgroundParticles+" ("+playgroundLanguage.eventControlled+")", EditorStyles.foldout);
		else if (playgroundParticlesScriptReference.isSnapshot)
			playgroundSettings.particlesFoldout = GUILayout.Toggle(playgroundSettings.particlesFoldout, playgroundLanguage.playgroundParticles+" ("+playgroundLanguage.snapshot+")", EditorStyles.foldout);
		else
			playgroundSettings.particlesFoldout = GUILayout.Toggle(playgroundSettings.particlesFoldout, playgroundLanguage.playgroundParticles, EditorStyles.foldout);
		if (playgroundSettings.particlesFoldout) {
			
			EditorGUILayout.BeginVertical(boxStyle);
			
			// Source Settings
			if (GUILayout.Button(playgroundLanguage.source+" ("+playgroundParticlesScriptReference.source.ToString()+")", EditorStyles.toolbarDropDown)) playgroundSettings.sourceFoldout=!playgroundSettings.sourceFoldout;
			if (playgroundSettings.sourceFoldout) {
				
				EditorGUILayout.Separator();
				
				if (previousSource!=playgroundParticlesScriptReference.source) {
					LifetimeSorting();
				}
				EditorGUILayout.PropertyField(source, new GUIContent(
					playgroundLanguage.source, 
					playgroundLanguage.sourceDescription
				));
				
				EditorGUILayout.Separator();
				
				// Source is State
				if (source.intValue == 0) {
					RenderStateSettings();
					
					// Source is Projection
				} else if (source.intValue == 6) {
					RenderProjectionSettings();
					
					// Source is Transforms
				} else if (source.intValue == 1) {
					
					playgroundParticlesScriptReference.treatAsOneTransform = EditorGUILayout.Toggle (playgroundLanguage.treatAsOneTransform, playgroundParticlesScriptReference.treatAsOneTransform);
					
					EditorGUILayout.Separator();
					
					EditorGUILayout.BeginVertical(boxStyle);
					GUILayout.BeginHorizontal();
					playgroundSettings.transformListFoldout = GUILayout.Toggle(playgroundSettings.transformListFoldout, playgroundLanguage.transforms, EditorStyles.foldout);
					GUILayout.Label (playgroundParticlesScriptReference.sourceTransforms.Count.ToString(), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
					GUILayout.EndHorizontal();
					if (playgroundSettings.transformListFoldout) {
						bool hasDisabledTransform = false;
						if (playgroundParticlesScriptReference.sourceTransforms.Count>0) {
							for (int i = 0; i<playgroundParticlesScriptReference.sourceTransforms.Count; i++) {
								if (playgroundParticlesScriptReference.sourceTransforms[i].transform==null) hasDisabledTransform = true;
								EditorGUILayout.BeginVertical(boxStyle);
								EditorGUILayout.BeginHorizontal();
								GUILayout.Label(i.ToString(), EditorStyles.miniLabel, new GUILayoutOption[]{GUILayout.Width(18)});
								playgroundParticlesScriptReference.sourceTransforms[i].unfolded = GUILayout.Toggle(playgroundParticlesScriptReference.sourceTransforms[i].unfolded, playgroundLanguage.transform, EditorStyles.foldout);
								playgroundParticlesScriptReference.sourceTransforms[i].transform = (Transform)EditorGUILayout.ObjectField(playgroundParticlesScriptReference.sourceTransforms[i].transform, typeof(Transform), true);
								
								GUI.enabled = playgroundParticlesScriptReference.sourceTransforms[i].transform!=null;
								if (GUILayout.Button (playgroundLanguage.edit, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
									Selection.activeTransform = playgroundParticlesScriptReference.sourceTransforms[i].transform;
								GUI.enabled = true;
								GUI.enabled = (playgroundParticlesScriptReference.sourceTransforms.Count>1);
								if(GUILayout.Button(playgroundLanguage.upSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
									sourceTransforms.MoveArrayElement(i, i==0?playgroundParticlesScriptReference.sourceTransforms.Count-1:i-1);
								}
								if(GUILayout.Button(playgroundLanguage.downSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
									sourceTransforms.MoveArrayElement(i, i<playgroundParticlesScriptReference.sourceTransforms.Count-1?i+1:0);
								}
								GUI.enabled = true;
								if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
									Transform newT = PlaygroundC.CreateTransform();
									newT.parent = playgroundParticlesScriptReference.particleSystemTransform;
									newT.position = playgroundParticlesScriptReference.sourceTransforms[i].transform.position;
									newT.name = "Source Transform "+(playgroundParticlesScriptReference.sourceTransforms.Count+1);
									playgroundParticlesScriptReference.sourceTransforms.Insert(i+1, new PlaygroundTransformC());
									playgroundParticlesScriptReference.sourceTransforms[i+1].transform = newT;
									playgroundParticles.ApplyModifiedProperties();
								}
								if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
									playgroundParticlesScriptReference.sourceTransforms.RemoveAt(i);
									playgroundParticles.ApplyModifiedProperties();
									return;
								}
								EditorGUILayout.EndHorizontal();
								if (playgroundParticlesScriptReference.sourceTransforms[i].unfolded) {
									if (playgroundParticlesScriptReference.sourceTransforms[i].transform!=null) {
										playgroundParticlesScriptReference.sourceTransforms[i].transform.position = EditorGUILayout.Vector3Field (playgroundLanguage.position, playgroundParticlesScriptReference.sourceTransforms[i].transform.position);
										playgroundParticlesScriptReference.sourceTransforms[i].transform.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field(playgroundLanguage.rotation, playgroundParticlesScriptReference.sourceTransforms[i].transform.rotation.eulerAngles));
										playgroundParticlesScriptReference.sourceTransforms[i].transform.localScale = EditorGUILayout.Vector3Field(playgroundLanguage.scale, playgroundParticlesScriptReference.sourceTransforms[i].transform.localScale);
									} else {
										EditorGUILayout.HelpBox(playgroundLanguage.assignTransformMessage, MessageType.Info);
									}
								}
								EditorGUILayout.EndVertical();
							}
							EditorGUILayout.Separator();
							
						}
						
						if (hasDisabledTransform)
							EditorGUILayout.HelpBox(playgroundLanguage.allTransformsMustBeAssignedMessage, MessageType.Warning);
						
						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button (playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
							PlaygroundC.CreatePlaygroundTransform(playgroundParticlesScriptReference);
						}
						if (GUILayout.Button (playgroundLanguage.newEmpty, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
							playgroundParticlesScriptReference.sourceTransforms.Add (new PlaygroundTransformC());
						}
						EditorGUILayout.Separator();
						if (GUILayout.Button (playgroundLanguage.reverse, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
							playgroundParticlesScriptReference.sourceTransforms.Reverse();
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndVertical();
					
					
					/*
					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel(playgroundLanguage.transform);
					sourceTransform.objectReferenceValue = EditorGUILayout.ObjectField(sourceTransform.objectReferenceValue, typeof(Transform), true);
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel(playgroundLanguage.points+":");
					EditorGUILayout.SelectableLabel(sourceTransform.objectReferenceValue!=null?"1":"0", GUILayout.MaxWidth(80));
					EditorGUILayout.Separator();
					if(GUILayout.Button(playgroundLanguage.setParticleCount, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)) && isEditingInHierarchy){
						playgroundParticlesScriptReference.particleCount = sourceTransform.objectReferenceValue!=null?1:0;
					}
					GUI.enabled = (sourceTransform.objectReferenceValue!=null);
					if(GUILayout.Button("++", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}))
						particleCount.intValue = particleCount.intValue+1;
					GUI.enabled = true;
					GUILayout.EndHorizontal();
					*/
					
					// Source is World Object
				} else if (source.intValue == 2) {
					playgroundParticlesScriptReference.worldObject.gameObject = (GameObject)EditorGUILayout.ObjectField(playgroundLanguage.worldObject, playgroundParticlesScriptReference.worldObject.gameObject, typeof(GameObject), true);
					playgroundParticlesScriptReference.worldObject.scaleMethod = (ScaleMethod)EditorGUILayout.EnumPopup(playgroundLanguage.scaleMethod, playgroundParticlesScriptReference.worldObject.scaleMethod);

					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel(playgroundLanguage.points+":");
					EditorGUILayout.SelectableLabel((playgroundParticlesScriptReference.worldObject.vertexPositions!=null && playgroundParticlesScriptReference.worldObject.vertexPositions.Length>0)?playgroundParticlesScriptReference.worldObject.vertexPositions.Length.ToString():playgroundLanguage.noMesh, GUILayout.MaxWidth(80));
					
					EditorGUILayout.Separator();
					
					if(GUILayout.Button(playgroundLanguage.setParticleCount, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)) && playgroundParticlesScriptReference.worldObject.vertexPositions!=null && isEditingInHierarchy){
						playgroundParticlesScriptReference.particleCount = playgroundParticlesScriptReference.worldObject.vertexPositions.Length;
					}
					if(GUILayout.Button("++", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}) && playgroundParticlesScriptReference.worldObject.vertexPositions!=null)
						particleCount.intValue = particleCount.intValue+playgroundParticlesScriptReference.worldObject.vertexPositions.Length;
					GUILayout.EndHorizontal();
					
					GUILayout.BeginVertical(boxStyle);
					EditorGUILayout.LabelField(playgroundLanguage.proceduralOptions);
					EditorGUILayout.PropertyField(worldObjectUpdateVertices, new GUIContent(
						playgroundLanguage.meshVerticesUpdate,
						playgroundLanguage.meshVerticesUpdateDescription
						));
					EditorGUILayout.PropertyField(worldObjectUpdateNormals, new GUIContent(
						playgroundLanguage.meshNormalsUpdate,
						playgroundLanguage.meshNormalsUpdateDescription
						));
					GUILayout.EndVertical();
					
					// Source is Skinned World Object
				} else if (source.intValue == 3) {
					playgroundParticlesScriptReference.skinnedWorldObject.gameObject = (GameObject)EditorGUILayout.ObjectField(playgroundLanguage.skinnedWorldObject, playgroundParticlesScriptReference.skinnedWorldObject.gameObject, typeof(GameObject), true);
					if (playgroundParticlesScriptReference.skinnedWorldObject.gameObject!=null && playgroundParticlesScriptReference.skinnedWorldObject.renderer!=null) {
						if (playgroundParticlesScriptReference.skinnedWorldObject.renderer.bones.Length==0)
							EditorGUILayout.HelpBox(playgroundLanguage.skinnedMeshOptimizeGameObjectsMessage, MessageType.Warning);
					}
					
					if (playgroundParticlesScriptReference.skinnedWorldObject.mesh != null) {
						int prevDownResolutionSkinned = playgroundParticlesScriptReference.skinnedWorldObject.downResolution;
						playgroundParticlesScriptReference.skinnedWorldObject.downResolution = EditorGUILayout.IntSlider(playgroundLanguage.sourceDownResolution, playgroundParticlesScriptReference.skinnedWorldObject.downResolution, 1, Mathf.RoundToInt (playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length/2));
						if (prevDownResolutionSkinned!=playgroundParticlesScriptReference.skinnedWorldObject.downResolution)
							LifetimeSorting();
					}
					
					EditorGUILayout.PropertyField(forceSkinnedMeshUpdateOnMainThread, new GUIContent(
						playgroundLanguage.forceUpdateOnMainThread,
						playgroundLanguage.forceUpdateOnMainThreadDescription
						));
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel(playgroundLanguage.points+":");
					if (playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions!=null && playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length>0) {
						if (playgroundParticlesScriptReference.skinnedWorldObject.downResolution<=1)
							EditorGUILayout.SelectableLabel(playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length.ToString(), GUILayout.MaxWidth(80));
						else
							EditorGUILayout.SelectableLabel((playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length/playgroundParticlesScriptReference.skinnedWorldObject.downResolution).ToString()+" ("+playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length.ToString()+")", GUILayout.MaxWidth(160));
					} else EditorGUILayout.SelectableLabel(playgroundLanguage.noMesh);
					EditorGUILayout.Separator();
					if(GUILayout.Button(playgroundLanguage.setParticleCount, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)) && playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions!=null && isEditingInHierarchy){
						playgroundParticlesScriptReference.particleCount = playgroundParticlesScriptReference.skinnedWorldObject.downResolution<=1?playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length:playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length/playgroundParticlesScriptReference.skinnedWorldObject.downResolution;
					}
					if(GUILayout.Button("++", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}) && playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions!=null)
						particleCount.intValue = particleCount.intValue+(playgroundParticlesScriptReference.skinnedWorldObject.vertexPositions.Length/playgroundParticlesScriptReference.skinnedWorldObject.downResolution);
					GUILayout.EndHorizontal();
					
					GUILayout.BeginVertical(boxStyle);
					EditorGUILayout.LabelField(playgroundLanguage.proceduralOptions);
					EditorGUILayout.PropertyField(worldObjectUpdateVertices, new GUIContent(
						playgroundLanguage.meshVerticesUpdate,
						playgroundLanguage.meshVerticesUpdateDescription
						));
					EditorGUILayout.PropertyField(worldObjectUpdateNormals, new GUIContent(
						playgroundLanguage.meshNormalsUpdate,
						playgroundLanguage.meshNormalsUpdateDescription
						));
					GUILayout.EndVertical();
					
					// Source is Script
				} else if (source.intValue == 4) {
					
					// Controlled by events
					if (playgroundParticlesScriptReference.eventControlledBy.Count>0) {
						GUILayout.BeginVertical (boxStyle);
						int eventCount = 0;
						EditorGUILayout.HelpBox(playgroundLanguage.controlledByAnotherEvent, MessageType.Info);
						for (int i = 0; i<playgroundParticlesScriptReference.eventControlledBy.Count; i++) {
							eventCount = 0;
							for (int x = 0; x<playgroundParticlesScriptReference.eventControlledBy[i].events.Count; x++)
								if (playgroundParticlesScriptReference.eventControlledBy[i].events[x].target==playgroundParticlesScriptReference) eventCount++;
							GUILayout.BeginHorizontal (boxStyle);
							GUILayout.Label (i.ToString(), EditorStyles.miniLabel, new GUILayoutOption[]{GUILayout.Width(18)});
							if (GUILayout.Button (playgroundParticlesScriptReference.eventControlledBy[i].name+" ("+eventCount+")", EditorStyles.label)) 
								Selection.activeGameObject = playgroundParticlesScriptReference.eventControlledBy[i].particleSystemGameObject;
							if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								if (EditorUtility.DisplayDialog(
									playgroundLanguage.removeEvent+" "+playgroundParticlesScriptReference.eventControlledBy[i].name+"?",
									playgroundLanguage.removeEventsText1+" "+playgroundParticlesScriptReference.eventControlledBy[i].name+" "+playgroundLanguage.removeEventsText2, 
									playgroundLanguage.yes, playgroundLanguage.no)) {
									for (int x = 0; x<playgroundParticlesScriptReference.eventControlledBy[i].events.Count; x++) {
										if (playgroundParticlesScriptReference.eventControlledBy[i].events[x].target == playgroundParticlesScriptReference) {
											playgroundParticlesScriptReference.eventControlledBy[i].events.RemoveAt(x);
											x--;
										}
									}
									playgroundParticlesScriptReference.eventControlledBy.RemoveAt (i);
								}
							}
							GUILayout.EndHorizontal ();
						}
						GUILayout.EndVertical ();
						GUI.enabled = false;
					} else EditorGUILayout.HelpBox(playgroundLanguage.controlledByScript, MessageType.Info);
					
					
					EditorGUILayout.Separator();
					
					EditorGUILayout.BeginVertical(boxStyle);
					EditorGUILayout.BeginHorizontal();
					playgroundParticlesScriptReference.scriptedEmissionIndex = EditorGUILayout.IntField(playgroundLanguage.emissionIndex, Mathf.Clamp(playgroundParticlesScriptReference.scriptedEmissionIndex, 0, playgroundParticlesScriptReference.particleCount-1));
					if(GUILayout.RepeatButton(playgroundLanguage.emit, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Height(16), GUILayout.ExpandWidth(false)})) {
						PlaygroundC.Emit(playgroundParticlesScriptReference);
					}
					EditorGUILayout.EndHorizontal();
					
					playgroundParticlesScriptReference.scriptedEmissionPosition = EditorGUILayout.Vector3Field(playgroundLanguage.position, playgroundParticlesScriptReference.scriptedEmissionPosition);
					playgroundParticlesScriptReference.scriptedEmissionVelocity = EditorGUILayout.Vector3Field(playgroundLanguage.velocity, playgroundParticlesScriptReference.scriptedEmissionVelocity);
					playgroundParticlesScriptReference.scriptedEmissionColor = EditorGUILayout.ColorField(playgroundLanguage.color, playgroundParticlesScriptReference.scriptedEmissionColor);
					
					EditorGUILayout.EndVertical();
					
					GUI.enabled = true;
					
					// Source is Paint
				} else if (source.intValue == 5) {
					
					if (playgroundParticlesScriptReference.paint==null) {
						PlaygroundC.PaintObject(playgroundParticlesScriptReference);
					}
					
					// Paint Mode
					EditorGUILayout.BeginVertical(boxStyle);
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel(playgroundLanguage.paintMode);
					selectedPaintMode = GUILayout.Toolbar (selectedPaintMode, new string[]{playgroundLanguage.dot,playgroundLanguage.brush,playgroundLanguage.eraser}, EditorStyles.toolbarButton);
					EditorGUILayout.EndHorizontal();
					
					// Dot
					if (selectedPaintMode!=0) {
						EditorGUILayout.Separator();
					}
					
					// Brush
					if (selectedPaintMode==1) {
						EditorGUI.indentLevel++;
						EditorGUILayout.BeginVertical(boxStyle);
						brushPresetFoldout = GUILayout.Toggle(brushPresetFoldout, playgroundLanguage.brushPresets, EditorStyles.foldout);
						EditorGUI.indentLevel--;
						if (brushPresetFoldout) {
							if (brushPresets==null)
								LoadBrushes();
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.Separator();
							brushListStyle = GUILayout.Toolbar (brushListStyle, new string[]{playgroundLanguage.icons,playgroundLanguage.list}, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.Separator();
							int i;
							
							// Icons
							if (brushListStyle==0) {
								GUILayout.BeginHorizontal();
								for (i = 0; i<brushPresets.Length; i++) {
									EditorGUILayout.BeginVertical(new GUILayoutOption[]{GUILayout.Width(50), GUILayout.Height(62)});
									
									if (GUILayout.Button(brushPresets[i].texture, new GUILayoutOption[]{GUILayout.Width(32), GUILayout.Height(32)})){
										selectedBrushPreset = i;
										SetBrush(i);
									}
									if (brushNames.Length>0) {
										EditorGUILayout.LabelField(brushNames[i], EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[]{GUILayout.Width(50), GUILayout.Height(30)});
									}
									EditorGUILayout.EndVertical();
									if (i%(Screen.width/80)==0 && i>0) {
										EditorGUILayout.EndHorizontal();
										EditorGUILayout.BeginHorizontal();
									}
								}
								EditorGUILayout.EndHorizontal();
								
								
								// List
							} else {
								for (i = 0; i<brushPresets.Length; i++) {
									EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(22));
									EditorGUILayout.BeginHorizontal();
									if (GUILayout.Button(brushNames[i], EditorStyles.label)) {
										selectedBrushPreset = i;
										SetBrush(i);
									}
									EditorGUILayout.Separator();
									if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
										if (EditorUtility.DisplayDialog(playgroundLanguage.deleteBrush, 
										                                brushNames[i]+" "+playgroundLanguage.deleteBrushText, 
										                                playgroundLanguage.yes, 
										                                playgroundLanguage.no)) {
											AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(brushPrefabs[i] as UnityEngine.Object));
											LoadBrushes();
										}
									}
									EditorGUILayout.EndHorizontal();
									EditorGUILayout.EndVertical();
								}
							}
							
							// Create new brush
							if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
								PlaygroundCreateBrushWindowC.ShowWindow();
							}
						}
						
						EditorGUILayout.EndVertical();
						EditorGUILayout.Separator();
						
						GUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel(playgroundLanguage.brushShape);
						paintTexture = EditorGUILayout.ObjectField(paintTexture, typeof(Texture2D), false) as Texture2D;
						GUILayout.EndHorizontal();
						playgroundParticlesScriptReference.paint.brush.detail = (BRUSHDETAILC)EditorGUILayout.EnumPopup(playgroundLanguage.detail, playgroundParticlesScriptReference.paint.brush.detail);
						playgroundParticlesScriptReference.paint.brush.scale = EditorGUILayout.Slider(playgroundLanguage.brushScale, playgroundParticlesScriptReference.paint.brush.scale, playgroundSettings.minimumAllowedBrushScale, playgroundSettings.maximumAllowedBrushScale);
						playgroundParticlesScriptReference.paint.brush.distance = EditorGUILayout.FloatField(playgroundLanguage.brushDistance, playgroundParticlesScriptReference.paint.brush.distance);
						
						if (paintTexture!=null && paintTexture!=playgroundParticlesScriptReference.paint.brush.texture) {
							playgroundParticlesScriptReference.paint.brush.SetTexture(paintTexture as Texture2D);
							selectedBrushPreset = -1;
						}
						
						useBrushColor = EditorGUILayout.Toggle(playgroundLanguage.useBrushColor, useBrushColor);
					}
					
					
					// Eraser
					if (selectedPaintMode==2) {
						eraserRadius = EditorGUILayout.Slider(playgroundLanguage.eraserRadius, eraserRadius, playgroundSettings.minimumEraserRadius, playgroundSettings.maximumEraserRadius);
					}
					
					EditorGUILayout.EndVertical();
					EditorGUILayout.Separator();
					
					if (selectedPaintMode==1 && useBrushColor) GUI.enabled = false;
					paintColor = EditorGUILayout.ColorField(playgroundLanguage.color, paintColor);
					GUI.enabled = true;
					if (showNoAlphaWarning && !useBrushColor) {
						EditorGUILayout.HelpBox(playgroundLanguage.noAlphaColorInPaint, MessageType.Warning);
					}
					showNoAlphaWarning = (paintColor.a == 0);
					
					EditorGUILayout.PropertyField(paintCollisionType, new GUIContent(playgroundLanguage.paintCollisionType));
					if (paintCollisionType.enumValueIndex==1) {
						GUILayout.BeginHorizontal();
						GUILayout.Space (16);
						GUILayout.Label(playgroundLanguage.depth);
						EditorGUILayout.Separator();
						float minDepth = playgroundParticlesScriptReference.paint.minDepth;
						float maxDepth = playgroundParticlesScriptReference.paint.maxDepth;
						EditorGUILayout.MinMaxSlider(ref minDepth, ref maxDepth, -playgroundSettings.maximumAllowedDepth, playgroundSettings.maximumAllowedDepth, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
						playgroundParticlesScriptReference.paint.minDepth = Mathf.Clamp (minDepth, -playgroundSettings.maximumAllowedDepth, playgroundSettings.maximumAllowedDepth);
						playgroundParticlesScriptReference.paint.maxDepth = Mathf.Clamp (maxDepth, -playgroundSettings.maximumAllowedDepth, playgroundSettings.maximumAllowedDepth);
						playgroundParticlesScriptReference.paint.minDepth = EditorGUILayout.FloatField(playgroundParticlesScriptReference.paint.minDepth, GUILayout.Width(50));
						playgroundParticlesScriptReference.paint.maxDepth = EditorGUILayout.FloatField(playgroundParticlesScriptReference.paint.maxDepth, GUILayout.Width(50));
						GUILayout.EndHorizontal();
					}
					EditorGUILayout.PropertyField(paintLayerMask, new GUIContent(playgroundLanguage.paintMask));
					playgroundParticlesScriptReference.paint.spacing = EditorGUILayout.Slider(playgroundLanguage.paintSpacing, playgroundParticlesScriptReference.paint.spacing, .0f, playgroundSettings.maximumAllowedPaintSpacing);
					if (isEditingInHierarchy)
						PlaygroundC.reference.paintMaxPositions = EditorGUILayout.IntSlider(playgroundLanguage.maxPaintPositions, PlaygroundC.reference.paintMaxPositions, 0, playgroundSettings.maximumAllowedPaintPositions);
					playgroundParticlesScriptReference.paint.exceedMaxStopsPaint = EditorGUILayout.Toggle(playgroundLanguage.exceedMaxStopsPaint, playgroundParticlesScriptReference.paint.exceedMaxStopsPaint);
					if (playgroundParticlesScriptReference.paint.exceedMaxStopsPaint && playgroundParticlesScriptReference.paint.positionLength>=PlaygroundC.reference.paintMaxPositions) {
						EditorGUILayout.HelpBox(playgroundLanguage.exceededMaxPaint, MessageType.Warning);
					}
					
					if (isEditingInHierarchy) {
						GUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel(playgroundLanguage.paint+":");
						ProgressBar((playgroundParticlesScriptReference.paint.positionLength*1f)/PlaygroundC.reference.paintMaxPositions, playgroundParticlesScriptReference.paint.positionLength+"/"+PlaygroundC.reference.paintMaxPositions, Mathf.FloorToInt(Screen.width/2.2f)-65);
						EditorGUILayout.Separator();
						if(GUILayout.Button(playgroundLanguage.setParticleCount, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)) && isEditingInHierarchy){
							playgroundParticlesScriptReference.particleCount = playgroundParticlesScriptReference.paint.positionLength;
						}
						if(GUILayout.Button("++", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}))
							particleCount.intValue = particleCount.intValue+playgroundParticlesScriptReference.paint.positionLength;
						GUILayout.EndHorizontal();
					}
					EditorGUILayout.Separator();
					
					GUILayout.BeginHorizontal();
					GUI.enabled = GUI.enabled&&isEditingInHierarchy;
					if (inPaintMode && GUI.enabled)
						GUI.backgroundColor = paintActiveColor;
					if (GUILayout.Button((inPaintMode?playgroundLanguage.stop:playgroundLanguage.start)+" "+playgroundLanguage.paint+" ", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
						StartStopPaint();
					}
					GUI.enabled = (playgroundParticlesScriptReference.paint.positionLength>0);
					if(GUILayout.Button(playgroundLanguage.clear, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
						ClearPaint();
					}
					GUI.enabled = true;
					GUILayout.EndHorizontal();
					EditorGUILayout.Separator();
					GUI.backgroundColor = whiteColor;
					if (inPaintMode && selectedPaintMode==1 && paintTexture==null)
						EditorGUILayout.HelpBox(playgroundLanguage.assignATexture, MessageType.Warning);
					if (playgroundParticlesScriptReference.paint.positionLength-1>playgroundParticlesScriptReference.particleCount)
						EditorGUILayout.HelpBox(playgroundLanguage.morePaintThanPositions, MessageType.Warning);
					
					if (GUI.changed) {
						SetBrushStyle();
					}
					
					// Source is Spline
				} else if (source.intValue == 7) {
					playgroundParticlesScriptReference.splineTimeOffset = EditorGUILayout.Slider (playgroundLanguage.timeOffset, playgroundParticlesScriptReference.splineTimeOffset, 0, 1f);
					GUI.enabled = playgroundParticlesScriptReference.splines.Count>1;
					playgroundParticlesScriptReference.treatAsOneSpline = EditorGUILayout.Toggle (playgroundLanguage.treatAsOneSpline, playgroundParticlesScriptReference.treatAsOneSpline);
					GUI.enabled = true;
					
					EditorGUILayout.Separator();
					
					EditorGUILayout.BeginVertical(boxStyle);
					GUILayout.BeginHorizontal();
					playgroundSettings.splineListFoldout = GUILayout.Toggle(playgroundSettings.splineListFoldout, playgroundLanguage.splines, EditorStyles.foldout);
					GUILayout.Label (playgroundParticlesScriptReference.splines.Count.ToString(), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
					GUILayout.EndHorizontal();
					if (playgroundSettings.splineListFoldout) {
						bool hasDisabledSpline = false;
						if (playgroundParticlesScriptReference.splines.Count>0) {
							for (int i = 0; i<playgroundParticlesScriptReference.splines.Count; i++) {
								if (playgroundParticlesScriptReference.splines[i]==null || !playgroundParticlesScriptReference.splines[i].enabled) hasDisabledSpline = true;
								EditorGUILayout.BeginHorizontal(boxStyle);
								
								GUILayout.Label(i.ToString(), EditorStyles.miniLabel, new GUILayoutOption[]{GUILayout.Width(18)});
								PlaygroundSpline currentSpline = playgroundParticlesScriptReference.splines[i];
								playgroundParticlesScriptReference.splines[i] = (PlaygroundSpline)EditorGUILayout.ObjectField(playgroundLanguage.spline, playgroundParticlesScriptReference.splines[i], typeof(PlaygroundSpline), true);
								if (playgroundParticlesScriptReference.splines[i]!=null) {
									if (currentSpline!=playgroundParticlesScriptReference.splines[i]) {
										playgroundParticlesScriptReference.splines[i].AddUser (playgroundParticlesScriptReference.transform);
									}
								}
								GUI.enabled = playgroundParticlesScriptReference.splines[i]!=null;
								if (GUILayout.Button (playgroundLanguage.edit, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
									Selection.activeGameObject = playgroundParticlesScriptReference.splines[i].gameObject;
								GUI.enabled = true;
								GUI.enabled = (playgroundParticlesScriptReference.splines.Count>1);
								if(GUILayout.Button(playgroundLanguage.upSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
									sourceSplines.MoveArrayElement(i, i==0?playgroundParticlesScriptReference.splines.Count-1:i-1);
								}
								if(GUILayout.Button(playgroundLanguage.downSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
									sourceSplines.MoveArrayElement(i, i<playgroundParticlesScriptReference.splines.Count-1?i+1:0);
								}
								GUI.enabled = true;
								if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
									playgroundParticlesScriptReference.splines.RemoveAt(i);
									playgroundParticles.ApplyModifiedProperties();
									return;
								}
								EditorGUILayout.EndHorizontal();
							}
							EditorGUILayout.Separator();
							
						} else {
							EditorGUILayout.HelpBox(playgroundLanguage.noSplinesCreated, MessageType.Info);
						}
						if (playgroundParticlesScriptReference.splines.Count==1 && playgroundParticlesScriptReference.splines[0]==null)
							EditorGUILayout.HelpBox(playgroundLanguage.newSplineMessage, MessageType.Info);
						else if (hasDisabledSpline)
							EditorGUILayout.HelpBox(playgroundLanguage.allSplinesMustBeAssignedMessage, MessageType.Warning);
						
						EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button (playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
							PlaygroundC.CreateSpline(playgroundParticlesScriptReference);
							Selection.activeGameObject = playgroundParticlesScriptReference.splines[playgroundParticlesScriptReference.splines.Count-1].gameObject;
						}
						if (GUILayout.Button (playgroundLanguage.newEmpty, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
							playgroundParticlesScriptReference.splines.Add (null);
						}
						EditorGUILayout.Separator();
						if (GUILayout.Button (playgroundLanguage.reverse, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
							playgroundParticlesScriptReference.splines.Reverse();
						}
						EditorGUILayout.EndHorizontal();
					}
					EditorGUILayout.EndVertical();
				
					// Source is Playground Shape
				} else if (source.intValue == 8) {


					// Source is other particle system
				} else if (source.intValue == 9) {
					playgroundParticlesScriptReference.otherParticleSource = (PlaygroundParticlesC)EditorGUILayout.ObjectField(playgroundLanguage.sourceParticleSystem, playgroundParticlesScriptReference.otherParticleSource, typeof(PlaygroundParticlesC), true);
					playgroundParticlesScriptReference.otherParticleSourceMethod = (SOURCEBIRTHMETHOD)EditorGUILayout.EnumPopup(playgroundLanguage.sourceBirthMethod, playgroundParticlesScriptReference.otherParticleSourceMethod);
				}
				
				EditorGUILayout.Separator();
				
			}
			
			// Particle Settings
			if (GUILayout.Button(playgroundLanguage.particleSettings+" ("+playgroundParticlesScriptReference.particleCount+")", EditorStyles.toolbarDropDown)) playgroundSettings.particleSettingsFoldout=!playgroundSettings.particleSettingsFoldout;
			if (playgroundSettings.particleSettingsFoldout) {
				
				if (source.intValue==4)
					EditorGUILayout.HelpBox(playgroundLanguage.someFeaturesInScript, MessageType.Info);
				
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.particleSettingsParticleCountFoldout = GUILayout.Toggle(playgroundSettings.particleSettingsParticleCountFoldout, playgroundLanguage.particleCount, EditorStyles.foldout);
				GUILayout.Label (playgroundParticlesScriptReference.particleCount.ToString(), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.particleSettingsParticleCountFoldout) {
					GUILayout.BeginHorizontal();
					GUI.enabled = !playgroundParticlesScriptReference.isSnapshot;
					particleCount.intValue = EditorGUILayout.IntSlider(playgroundLanguage.particleCount, particleCount.intValue, 0, playgroundSettings.maximumAllowedParticles);
					if(GUILayout.Button("x2", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}))
						particleCount.intValue *= 2;
					if (initialModule_maxNumParticles.intValue != playgroundParticlesScriptReference.particleCount)
					{
						initialModule_maxNumParticles.intValue = playgroundParticlesScriptReference.particleCount;
						shuriken.ApplyModifiedProperties();
					}
					GUI.enabled = true;
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				
				// Overflow Offset
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.particleSettingsOverflowOffsetFoldout = GUILayout.Toggle(playgroundSettings.particleSettingsOverflowOffsetFoldout, playgroundLanguage.overflowOffset, EditorStyles.foldout);
				GUILayout.Label ((playgroundParticlesScriptReference.HasOverflow()?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.particleSettingsOverflowOffsetFoldout) {
					GUI.enabled=(source.intValue!=4&&source.intValue!=7);
					EditorGUILayout.PropertyField(overflowMode, new GUIContent(
						playgroundLanguage.overflowMode, 
						playgroundLanguage.overflowModeDescription)
					                              );
					overflowOffset.vector3Value = EditorGUILayout.Vector3Field(playgroundLanguage.overflowOffset, overflowOffset.vector3Value);
					GUI.enabled=true;
					if (source.intValue==7)
						EditorGUILayout.HelpBox(playgroundLanguage.overflowOffsetSplineMessage, MessageType.Info);
				}
				GUILayout.EndVertical();
				
				// Source Scattering
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.particleSettingsScatterFoldout = GUILayout.Toggle(playgroundSettings.particleSettingsScatterFoldout, playgroundLanguage.sourceScatter, EditorStyles.foldout);
				GUILayout.Label ((playgroundParticlesScriptReference.applySourceScatter?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.particleSettingsScatterFoldout) {
					GUI.enabled=(source.intValue!=4);
					bool prevScatterEnabled = playgroundParticlesScriptReference.applySourceScatter;
					MINMAXVECTOR3METHOD prevScatterMethod = playgroundParticlesScriptReference.sourceScatterMethod;
					Vector3 prevScatterMin = playgroundParticlesScriptReference.sourceScatterMin;
					Vector3 prevScatterMax = playgroundParticlesScriptReference.sourceScatterMax;
					
					EditorGUILayout.BeginHorizontal();
					playgroundParticlesScriptReference.applySourceScatter = EditorGUILayout.ToggleLeft(playgroundLanguage.sourceScatter, playgroundParticlesScriptReference.applySourceScatter);
					GUI.enabled = (source.intValue!=4 && playgroundParticlesScriptReference.applySourceScatter);
					if (GUILayout.Button (playgroundLanguage.reset, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						playgroundParticlesScriptReference.sourceScatterMin = Vector3.zero;
						playgroundParticlesScriptReference.sourceScatterMax = Vector3.zero;
					}
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.sourceScatterMethod = (MINMAXVECTOR3METHOD)EditorGUILayout.EnumPopup (playgroundLanguage.method, playgroundParticlesScriptReference.sourceScatterMethod);
					EditorGUI.indentLevel--;
					
					// X
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label(playgroundParticlesScriptReference.sourceScatterMethod==MINMAXVECTOR3METHOD.Rectangular||playgroundParticlesScriptReference.sourceScatterMethod==MINMAXVECTOR3METHOD.RectangularLinear?"X":playgroundLanguage.range, GUILayout.Width(50));
					EditorGUILayout.Separator();
					float sourceScatterMinX = playgroundParticlesScriptReference.sourceScatterMin.x;
					float sourceScatterMaxX = playgroundParticlesScriptReference.sourceScatterMax.x;
					EditorGUILayout.MinMaxSlider(ref sourceScatterMinX, ref sourceScatterMaxX, -playgroundSettings.maximumAllowedSourceScatter, playgroundSettings.maximumAllowedSourceScatter, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
					playgroundParticlesScriptReference.sourceScatterMin.x = Mathf.Clamp (sourceScatterMinX, -playgroundSettings.maximumAllowedSourceScatter, playgroundSettings.maximumAllowedSourceScatter);
					playgroundParticlesScriptReference.sourceScatterMax.x = Mathf.Clamp (sourceScatterMaxX, -playgroundSettings.maximumAllowedSourceScatter, playgroundSettings.maximumAllowedSourceScatter);
					playgroundParticlesScriptReference.sourceScatterMin.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sourceScatterMin.x, GUILayout.Width(50));
					playgroundParticlesScriptReference.sourceScatterMax.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sourceScatterMax.x, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					
					if (playgroundParticlesScriptReference.sourceScatterMethod==MINMAXVECTOR3METHOD.Rectangular || playgroundParticlesScriptReference.sourceScatterMethod==MINMAXVECTOR3METHOD.RectangularLinear) {
						// Y
						GUILayout.BeginHorizontal();
						GUILayout.Space(16);
						GUILayout.Label("Y");
						EditorGUILayout.Separator();
						float sourceScatterMinY = playgroundParticlesScriptReference.sourceScatterMin.y;
						float sourceScatterMaxY = playgroundParticlesScriptReference.sourceScatterMax.y;
						EditorGUILayout.MinMaxSlider(ref sourceScatterMinY, ref sourceScatterMaxY, -playgroundSettings.maximumAllowedSourceScatter, playgroundSettings.maximumAllowedSourceScatter, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
						playgroundParticlesScriptReference.sourceScatterMin.y = Mathf.Clamp (sourceScatterMinY, -playgroundSettings.maximumAllowedSourceScatter, playgroundSettings.maximumAllowedSourceScatter);
						playgroundParticlesScriptReference.sourceScatterMax.y = Mathf.Clamp (sourceScatterMaxY, -playgroundSettings.maximumAllowedSourceScatter, playgroundSettings.maximumAllowedSourceScatter);
						playgroundParticlesScriptReference.sourceScatterMin.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sourceScatterMin.y, GUILayout.Width(50));
						playgroundParticlesScriptReference.sourceScatterMax.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sourceScatterMax.y, GUILayout.Width(50));
						GUILayout.EndHorizontal();
						// Z
						GUILayout.BeginHorizontal();
						GUILayout.Space(16);
						GUILayout.Label("Z");
						EditorGUILayout.Separator();
						float sourceScatterMinZ = playgroundParticlesScriptReference.sourceScatterMin.z;
						float sourceScatterMaxZ = playgroundParticlesScriptReference.sourceScatterMax.z;
						EditorGUILayout.MinMaxSlider(ref sourceScatterMinZ, ref sourceScatterMaxZ, -playgroundSettings.maximumAllowedSourceScatter, playgroundSettings.maximumAllowedSourceScatter, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
						playgroundParticlesScriptReference.sourceScatterMin.z = Mathf.Clamp (sourceScatterMinZ, -playgroundSettings.maximumAllowedSourceScatter, playgroundSettings.maximumAllowedSourceScatter);
						playgroundParticlesScriptReference.sourceScatterMax.z = Mathf.Clamp (sourceScatterMaxZ, -playgroundSettings.maximumAllowedSourceScatter, playgroundSettings.maximumAllowedSourceScatter);
						playgroundParticlesScriptReference.sourceScatterMin.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sourceScatterMin.z, GUILayout.Width(50));
						playgroundParticlesScriptReference.sourceScatterMax.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sourceScatterMax.z, GUILayout.Width(50));
						GUILayout.EndHorizontal();
					}
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.scatterScale = EditorGUILayout.Vector3Field (playgroundLanguage.scale, playgroundParticlesScriptReference.scatterScale);
					EditorGUI.indentLevel--;
					/*
					if (playgroundParticlesScriptReference.sourceScatterMethod==MINMAXVECTOR3METHOD.SphericalSector || playgroundParticlesScriptReference.sourceScatterMethod==MINMAXVECTOR3METHOD.SphericalSectorLinear) {
						EditorGUI.indentLevel++;
						playgroundParticlesScriptReference.sourceScatterMin.y = EditorGUILayout.Slider(playgroundLanguage.sectorA, playgroundParticlesScriptReference.sourceScatterMin.y, -1f, 1f);
						playgroundParticlesScriptReference.sourceScatterMax.y = EditorGUILayout.Slider(playgroundLanguage.sectorB, playgroundParticlesScriptReference.sourceScatterMax.y, 0, 1f);
						EditorGUI.indentLevel--;
					}
					*/
					GUI.enabled = true;
					
					if (prevScatterEnabled!=playgroundParticlesScriptReference.applySourceScatter || prevScatterMin!=playgroundParticlesScriptReference.sourceScatterMin || prevScatterMax!=playgroundParticlesScriptReference.sourceScatterMax || prevScatterMethod!=playgroundParticlesScriptReference.sourceScatterMethod) {
						if (isEditingInHierarchy) {
							LifetimeSorting();
							playgroundParticlesScriptReference.RefreshScatter();
						}
					}
				}
				GUILayout.EndVertical();
				
				// Emission
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.particleSettingsEmissionFoldout = GUILayout.Toggle(playgroundSettings.particleSettingsEmissionFoldout, playgroundLanguage.emission, EditorStyles.foldout);
				GUILayout.Label ((playgroundParticlesScriptReference.emit?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.particleSettingsEmissionFoldout) {
					bool prevEmit = playgroundParticlesScriptReference.emit;
					bool prevLoop = playgroundParticlesScriptReference.loop;
					playgroundParticlesScriptReference.emit = EditorGUILayout.Toggle(playgroundLanguage.emitParticles, playgroundParticlesScriptReference.emit);
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.loop = EditorGUILayout.Toggle(playgroundLanguage.loop, playgroundParticlesScriptReference.loop);
					if (prevEmit!=playgroundParticlesScriptReference.emit || prevLoop!=playgroundParticlesScriptReference.loop&&playgroundParticlesScriptReference.loop && !playgroundParticlesScriptReference.IsPrewarming()) {
						//playgroundParticlesScriptReference.simulationStarted = PlaygroundC.globalTime;
						//playgroundParticlesScriptReference.loopExceeded = false;
						//playgroundParticlesScriptReference.loopExceededOnParticle = -1;
						playgroundParticlesScriptReference.particleSystemGameObject.SetActive(true);
					}
					GUI.enabled = !loop.boolValue;
					
					EditorGUI.indentLevel++;
					EditorGUILayout.BeginHorizontal();
					disableOnDone.boolValue = EditorGUILayout.Toggle(playgroundLanguage.disableOnDone, disableOnDone.boolValue, GUILayout.ExpandWidth(false));
					GUI.enabled = GUI.enabled&&disableOnDone.boolValue;
					EditorGUILayout.PropertyField(disableOnDoneRoutine, new GUIContent(""), GUILayout.ExpandWidth(true));
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel--;
					
					GUI.enabled = true;
					playgroundParticlesScriptReference.clearParticlesOnEmissionStop = EditorGUILayout.Toggle (playgroundLanguage.clearOnStop, playgroundParticlesScriptReference.clearParticlesOnEmissionStop);
					EditorGUI.indentLevel--;
					GUI.enabled=(source.intValue!=4);
					emissionRate.floatValue = EditorGUILayout.Slider(playgroundLanguage.emissionRate, emissionRate.floatValue, 0, 1f);
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
				
				// Size
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.particleSettingsSizeFoldout = GUILayout.Toggle(playgroundSettings.particleSettingsSizeFoldout, playgroundLanguage.size, EditorStyles.foldout);
				GUILayout.Label (playgroundParticlesScriptReference.sizeMin.ToString("f1")+"-"+playgroundParticlesScriptReference.sizeMax.ToString("f1")+" ("+playgroundParticlesScriptReference.scale.ToString("f1")+")", EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.particleSettingsSizeFoldout) {
					GUILayout.BeginHorizontal();
					GUILayout.Label(playgroundLanguage.size);
					EditorGUILayout.Separator();
					float sizeMin = playgroundParticlesScriptReference.sizeMin;
					float sizeMax = playgroundParticlesScriptReference.sizeMax;
					EditorGUILayout.MinMaxSlider(ref sizeMin, ref sizeMax, 0, playgroundSettings.maximumAllowedSize, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
					playgroundParticlesScriptReference.sizeMin = Mathf.Clamp(sizeMin, 0, playgroundSettings.maximumAllowedSize);
					playgroundParticlesScriptReference.sizeMax = Mathf.Clamp(sizeMax, 0, playgroundSettings.maximumAllowedSize);
					playgroundParticlesScriptReference.sizeMin = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sizeMin, GUILayout.Width(50));
					playgroundParticlesScriptReference.sizeMax = EditorGUILayout.FloatField(playgroundParticlesScriptReference.sizeMax, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					
					playgroundParticlesScriptReference.scale = EditorGUILayout.Slider(playgroundLanguage.particleScale, playgroundParticlesScriptReference.scale, 0, playgroundSettings.maximumAllowedScale);
					
					EditorGUILayout.BeginHorizontal();
					playgroundParticlesScriptReference.applyLifetimeSize = EditorGUILayout.ToggleLeft (playgroundLanguage.lifetimeSize, playgroundParticlesScriptReference.applyLifetimeSize, GUILayout.Width (120));
					GUILayout.FlexibleSpace();
					GUI.enabled = playgroundParticlesScriptReference.applyLifetimeSize;
					lifetimeSize.animationCurveValue = EditorGUILayout.CurveField(lifetimeSize.animationCurveValue, GUILayout.Width (Mathf.CeilToInt(Screen.width/1.805f)));
					GUI.enabled = true;
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					playgroundParticlesScriptReference.applyParticleArraySize = EditorGUILayout.ToggleLeft (playgroundLanguage.arraySize, playgroundParticlesScriptReference.applyParticleArraySize, GUILayout.Width (120));
					GUILayout.FlexibleSpace();
					GUI.enabled = playgroundParticlesScriptReference.applyParticleArraySize;
					arraySize.animationCurveValue = EditorGUILayout.CurveField(arraySize.animationCurveValue, GUILayout.Width (Mathf.CeilToInt(Screen.width/1.805f)));
					GUI.enabled = true;
					EditorGUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				
				// Rotation
				GUILayout.BeginVertical(boxStyle);
				bool hasRotation = playgroundParticlesScriptReference.initialRotationMin!=0||playgroundParticlesScriptReference.initialRotationMax!=0||playgroundParticlesScriptReference.rotationSpeedMin!=0||playgroundParticlesScriptReference.rotationSpeedMax!=0||playgroundParticlesScriptReference.rotateTowardsDirection;
				GUILayout.BeginHorizontal();
				playgroundSettings.particleSettingsRotationFoldout = GUILayout.Toggle(playgroundSettings.particleSettingsRotationFoldout, playgroundLanguage.rotation, EditorStyles.foldout);
				GUILayout.Label ((hasRotation?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.particleSettingsRotationFoldout) {
					GUILayout.BeginHorizontal();
					GUILayout.Label(playgroundLanguage.initialRotation);
					EditorGUILayout.Separator();
					float initialRotationMin = playgroundParticlesScriptReference.initialRotationMin;
					float initialRotationMax = playgroundParticlesScriptReference.initialRotationMax;
					EditorGUILayout.MinMaxSlider(ref initialRotationMin, ref initialRotationMax, -playgroundSettings.maximumAllowedRotation, playgroundSettings.maximumAllowedRotation, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
					playgroundParticlesScriptReference.initialRotationMin = Mathf.Clamp (initialRotationMin, -playgroundSettings.maximumAllowedRotation, playgroundSettings.maximumAllowedRotation);
					playgroundParticlesScriptReference.initialRotationMax = Mathf.Clamp (initialRotationMax, -playgroundSettings.maximumAllowedRotation, playgroundSettings.maximumAllowedRotation);
					playgroundParticlesScriptReference.initialRotationMin = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialRotationMin, GUILayout.Width(50));
					playgroundParticlesScriptReference.initialRotationMax = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialRotationMax, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					
					GUI.enabled = !playgroundParticlesScriptReference.rotateTowardsDirection;
					GUILayout.BeginHorizontal();
					GUILayout.Label(playgroundLanguage.rotation);
					EditorGUILayout.Separator();
					float rotationSpeedMin = playgroundParticlesScriptReference.rotationSpeedMin;
					float rotationSpeedMax = playgroundParticlesScriptReference.rotationSpeedMax;
					EditorGUILayout.MinMaxSlider(ref rotationSpeedMin, ref rotationSpeedMax, -playgroundSettings.maximumAllowedRotation, playgroundSettings.maximumAllowedRotation, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
					playgroundParticlesScriptReference.rotationSpeedMin = Mathf.Clamp (rotationSpeedMin, -playgroundSettings.maximumAllowedRotation, playgroundSettings.maximumAllowedRotation);
					playgroundParticlesScriptReference.rotationSpeedMax = Mathf.Clamp (rotationSpeedMax, -playgroundSettings.maximumAllowedRotation, playgroundSettings.maximumAllowedRotation);
					playgroundParticlesScriptReference.rotationSpeedMin = EditorGUILayout.FloatField(playgroundParticlesScriptReference.rotationSpeedMin, GUILayout.Width(50));
					playgroundParticlesScriptReference.rotationSpeedMax = EditorGUILayout.FloatField(playgroundParticlesScriptReference.rotationSpeedMax, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					GUI.enabled = true;
					
					playgroundParticlesScriptReference.rotateTowardsDirection = EditorGUILayout.Toggle(playgroundLanguage.rotateTowardsDirection, playgroundParticlesScriptReference.rotateTowardsDirection);
					GUI.enabled = playgroundParticlesScriptReference.rotateTowardsDirection;
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.rotationNormal = EditorGUILayout.Vector3Field(playgroundLanguage.rotationNormal, playgroundParticlesScriptReference.rotationNormal);
					playgroundParticlesScriptReference.rotationNormal.x = Mathf.Clamp(playgroundParticlesScriptReference.rotationNormal.x, -1, 1);
					playgroundParticlesScriptReference.rotationNormal.y = Mathf.Clamp(playgroundParticlesScriptReference.rotationNormal.y, -1, 1);
					playgroundParticlesScriptReference.rotationNormal.z = Mathf.Clamp(playgroundParticlesScriptReference.rotationNormal.z, -1, 1);
					EditorGUI.indentLevel--;
					
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
				
				// Lifetime
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.particleSettingsLifetimeFoldout = GUILayout.Toggle(playgroundSettings.particleSettingsLifetimeFoldout, playgroundLanguage.lifetime, EditorStyles.foldout);
				GUILayout.Label ((playgroundParticlesScriptReference.lifetimeValueMethod==VALUEMETHOD.Constant?playgroundParticlesScriptReference.lifetime.ToString("f1"):playgroundParticlesScriptReference.lifetimeMin.ToString("f1")+"-"+playgroundParticlesScriptReference.lifetime.ToString("f1"))+" ("+playgroundParticlesScriptReference.sorting.ToString()+")", EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.particleSettingsLifetimeFoldout) {
					EditorGUILayout.BeginHorizontal();
					if (playgroundParticlesScriptReference.lifetimeValueMethod==VALUEMETHOD.Constant) {
						lifetime.floatValue = EditorGUILayout.Slider(playgroundLanguage.lifetime, lifetime.floatValue, 0, playgroundSettings.maximumAllowedLifetime);
					} else {
						GUILayout.Label(playgroundLanguage.lifetime);
						EditorGUILayout.Separator();
						float lifetimeMin = playgroundParticlesScriptReference.lifetimeMin;
						float lifetimeMax = playgroundParticlesScriptReference.lifetime;
						EditorGUILayout.MinMaxSlider(ref lifetimeMin, ref lifetimeMax, 0, playgroundSettings.maximumAllowedLifetime, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-125));
						playgroundParticlesScriptReference.lifetimeMin = Mathf.Clamp (lifetimeMin, 0, playgroundSettings.maximumAllowedLifetime);
						playgroundParticlesScriptReference.lifetime = Mathf.Clamp (lifetimeMax, 0, playgroundSettings.maximumAllowedLifetime);
						playgroundParticlesScriptReference.lifetimeMin = EditorGUILayout.FloatField(playgroundParticlesScriptReference.lifetimeMin, GUILayout.Width(50));
						playgroundParticlesScriptReference.lifetime = EditorGUILayout.FloatField(playgroundParticlesScriptReference.lifetime, GUILayout.Width(50));
					}
					playgroundParticlesScriptReference.lifetimeValueMethod = (VALUEMETHOD)EditorGUILayout.EnumPopup(playgroundParticlesScriptReference.lifetimeValueMethod, EditorStyles.toolbarDropDown, GUILayout.MaxWidth (12));
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel++;
					// Sorting
					GUI.enabled=(source.intValue!=4);
					selectedSort = sorting.intValue;
					EditorGUILayout.PropertyField(sorting, new GUIContent(
						playgroundLanguage.lifetimeSorting, 
						playgroundLanguage.lifetimeSortingDescription)
					                              );
					
					if (sorting.intValue==5||sorting.intValue==6) {
						EditorGUI.indentLevel++;
						EditorGUILayout.BeginHorizontal();
						
						switch (playgroundParticlesScriptReference.nearestNeighborOriginMethod) {
						case NEARESTNEIGHBORORIGINMETHOD.SourcePoint:
							playgroundParticlesScriptReference.nearestNeighborOrigin = EditorGUILayout.IntSlider(playgroundLanguage.sortOrigin, playgroundParticlesScriptReference.nearestNeighborOrigin, 0, playgroundParticlesScriptReference.particleCount>0?playgroundParticlesScriptReference.particleCount-1:0);
							break;
						case NEARESTNEIGHBORORIGINMETHOD.Vector3:
							playgroundParticlesScriptReference.nearestNeighborOriginVector3 = EditorGUILayout.Vector3Field (playgroundLanguage.sortOrigin, playgroundParticlesScriptReference.nearestNeighborOriginVector3);
							break;
						case NEARESTNEIGHBORORIGINMETHOD.Transform:
							playgroundParticlesScriptReference.nearestNeighborOriginTransform = (Transform)EditorGUILayout.ObjectField(playgroundLanguage.sortOrigin, playgroundParticlesScriptReference.nearestNeighborOriginTransform, typeof(Transform), true);
							break;
						}
						if (GUILayout.Button(playgroundParticlesScriptReference.nearestNeighborOriginMethod==NEARESTNEIGHBORORIGINMETHOD.SourcePoint?playgroundLanguage.sourcePoint:playgroundParticlesScriptReference.nearestNeighborOriginMethod==NEARESTNEIGHBORORIGINMETHOD.Vector3?playgroundLanguage.vector3:playgroundLanguage.transform, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
							if (playgroundParticlesScriptReference.nearestNeighborOriginMethod==NEARESTNEIGHBORORIGINMETHOD.SourcePoint)
								playgroundParticlesScriptReference.nearestNeighborOriginMethod = NEARESTNEIGHBORORIGINMETHOD.Vector3;
							else if (playgroundParticlesScriptReference.nearestNeighborOriginMethod==NEARESTNEIGHBORORIGINMETHOD.Vector3)
								playgroundParticlesScriptReference.nearestNeighborOriginMethod = NEARESTNEIGHBORORIGINMETHOD.Transform;
							else if (playgroundParticlesScriptReference.nearestNeighborOriginMethod==NEARESTNEIGHBORORIGINMETHOD.Transform)
								playgroundParticlesScriptReference.nearestNeighborOriginMethod = NEARESTNEIGHBORORIGINMETHOD.SourcePoint;
						}
						EditorGUILayout.EndHorizontal();
						EditorGUI.indentLevel--;
					}
					
					// Custom lifetime sorting
					if (sorting.intValue==7) {
						EditorGUI.indentLevel++;
						playgroundParticlesScriptReference.lifetimeSorting = EditorGUILayout.CurveField(playgroundLanguage.customSorting, playgroundParticlesScriptReference.lifetimeSorting);
						EditorGUI.indentLevel--;
						bool changed = prevLifetimeSortingKeys.Length!=playgroundParticlesScriptReference.lifetimeSorting.keys.Length;
						if (!changed)
						for (int k = 0; k<prevLifetimeSortingKeys.Length; k++) {
							if (playgroundParticlesScriptReference.lifetimeSorting.keys[k].value != prevLifetimeSortingKeys[k].value || playgroundParticlesScriptReference.lifetimeSorting.keys[k].time != prevLifetimeSortingKeys[k].time) {
								changed = true;
							}
						}
						if (changed) {
							LifetimeSorting();
							prevLifetimeSortingKeys = playgroundParticlesScriptReference.lifetimeSorting.keys;
						}
					}
					
					float prevLifetimeOffset = playgroundParticlesScriptReference.lifetimeOffset;
					playgroundParticlesScriptReference.lifetimeOffset = EditorGUILayout.Slider(playgroundLanguage.lifetimeOffset, playgroundParticlesScriptReference.lifetimeOffset, -playgroundParticlesScriptReference.lifetime, playgroundParticlesScriptReference.lifetime);
					if (prevLifetimeOffset!=playgroundParticlesScriptReference.lifetimeOffset) {
						LifetimeSortingAll();
					}
					EditorGUI.indentLevel--;
					GUI.enabled = GUI.enabled&&playgroundParticlesScriptReference.sorting!=SORTINGC.Burst;
					playgroundParticlesScriptReference.lifetimeEmission = EditorGUILayout.Slider (playgroundLanguage.lifetimeEmission, playgroundParticlesScriptReference.lifetimeEmission, 0, 1f);
					GUI.enabled=true;
				}
				GUILayout.EndVertical();
				
				// Particle Mask
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.particleSettingsMaskFoldout = GUILayout.Toggle(playgroundSettings.particleSettingsMaskFoldout, playgroundLanguage.particleMask, EditorStyles.foldout);
				GUILayout.Label ((playgroundParticlesScriptReference.applyParticleMask?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.particleSettingsMaskFoldout) {
					playgroundParticlesScriptReference.applyParticleMask = EditorGUILayout.Toggle (playgroundLanguage.particleMask, playgroundParticlesScriptReference.applyParticleMask);
					GUI.enabled = playgroundParticlesScriptReference.applyParticleMask;
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.particleMask = EditorGUILayout.IntSlider(playgroundLanguage.particleMask, playgroundParticlesScriptReference.particleMask, 0, particleCount.intValue);
					playgroundParticlesScriptReference.particleMaskTime = EditorGUILayout.Slider(playgroundLanguage.maskTime, playgroundParticlesScriptReference.particleMaskTime, 0, playgroundSettings.maximumAllowedTransitionTime);
					playgroundParticlesScriptReference.particleMaskSorting = (MASKSORTINGC)EditorGUILayout.EnumPopup(playgroundLanguage.maskSorting, playgroundParticlesScriptReference.particleMaskSorting);
					EditorGUI.indentLevel--;
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
			}
			
			// Force Settings
			if (GUILayout.Button(onlySourcePositioning.boolValue||playgroundParticlesScriptReference.onlyLifetimePositioning||(playgroundParticlesScriptReference.axisConstraints.x&&playgroundParticlesScriptReference.axisConstraints.y&&playgroundParticlesScriptReference.axisConstraints.z)?playgroundLanguage.forces+" ("+playgroundLanguage.off+")":playgroundParticlesScriptReference.turbulenceType==TURBULENCETYPE.None?playgroundLanguage.forces:playgroundLanguage.forces+" ("+playgroundParticlesScriptReference.turbulenceType+" "+playgroundLanguage.turbulence+")", EditorStyles.toolbarDropDown)) 
				playgroundSettings.forcesFoldout=!playgroundSettings.forcesFoldout;
			if (playgroundSettings.forcesFoldout) {
				
				// Force annihilation messages
				if (onlySourcePositioning.boolValue) {
					GUI.enabled = true;
					EditorGUILayout.HelpBox(playgroundLanguage.onlySourcePositionsDescription, MessageType.Info);
				}
				if (playgroundParticlesScriptReference.onlyLifetimePositioning && !onlySourcePositioning.boolValue) {
					GUI.enabled = true;
					EditorGUILayout.HelpBox(playgroundLanguage.onlyLifetimePositioningDescription, MessageType.Info);
				}
				if (playgroundParticlesScriptReference.axisConstraints.x&&playgroundParticlesScriptReference.axisConstraints.y&&playgroundParticlesScriptReference.axisConstraints.z && !playgroundParticlesScriptReference.onlyLifetimePositioning && !onlySourcePositioning.boolValue) {
					GUI.enabled = true;
					EditorGUILayout.HelpBox(playgroundLanguage.axisConstraintsDescription, MessageType.Info);
				}
				
				// Force Annihilation
				EditorGUILayout.BeginVertical (boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.forceAnnihilationFoldout = GUILayout.Toggle(playgroundSettings.forceAnnihilationFoldout, playgroundLanguage.forceAnnihilation, EditorStyles.foldout);
				GUILayout.Label (((playgroundParticlesScriptReference.onlyLifetimePositioning||onlySourcePositioning.boolValue||playgroundParticlesScriptReference.axisConstraints.x||playgroundParticlesScriptReference.axisConstraints.y||playgroundParticlesScriptReference.axisConstraints.z||playgroundParticlesScriptReference.maxVelocity==0||playgroundParticlesScriptReference.transitionBackToSource)?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.forceAnnihilationFoldout) {
					
					EditorGUILayout.Separator();
					
					onlySourcePositioning.boolValue = EditorGUILayout.Toggle(playgroundLanguage.onlySourcePositions, onlySourcePositioning.boolValue);
					
					EditorGUILayout.Separator();
					
					GUI.enabled = !onlySourcePositioning.boolValue;
					
					// Lifetime positioning
					EditorGUILayout.BeginHorizontal();
					playgroundParticlesScriptReference.onlyLifetimePositioning = EditorGUILayout.ToggleLeft(playgroundLanguage.lifetimePositioning, playgroundParticlesScriptReference.onlyLifetimePositioning, GUILayout.MaxWidth(Mathf.CeilToInt(EditorGUIUtility.labelWidth)-14));
					GUI.enabled = (playgroundParticlesScriptReference.onlyLifetimePositioning&&!onlySourcePositioning.boolValue);
					EditorGUILayout.Separator();
					if (GUILayout.Button (playgroundLanguage.reset, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						playgroundParticlesScriptReference.lifetimePositioning.Reset();
						playgroundParticlesScriptReference.lifetimePositioningTimeScale.Reset1();
						playgroundParticlesScriptReference.lifetimePositioningPositionScale.Reset1();
					}
					
					EditorGUILayout.EndHorizontal();
					
					EditorGUI.indentLevel++;
					
					EditorGUILayout.BeginHorizontal();
					lifetimePositioningX.animationCurveValue = EditorGUILayout.CurveField("X", lifetimePositioningX.animationCurveValue);
					playgroundParticlesScriptReference.lifetimePositioning.xRepeat = EditorGUILayout.FloatField (playgroundParticlesScriptReference.lifetimePositioning.xRepeat, GUILayout.Width(48));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					lifetimePositioningY.animationCurveValue = EditorGUILayout.CurveField("Y", lifetimePositioningY.animationCurveValue);
					playgroundParticlesScriptReference.lifetimePositioning.yRepeat = EditorGUILayout.FloatField (playgroundParticlesScriptReference.lifetimePositioning.yRepeat, GUILayout.Width(48));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					lifetimePositioningZ.animationCurveValue = EditorGUILayout.CurveField("Z", lifetimePositioningZ.animationCurveValue);
					playgroundParticlesScriptReference.lifetimePositioning.zRepeat = EditorGUILayout.FloatField (playgroundParticlesScriptReference.lifetimePositioning.zRepeat, GUILayout.Width(48));
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					playgroundParticlesScriptReference.applyLifetimePositioningPositionScale = EditorGUILayout.ToggleLeft (playgroundLanguage.positionScale, playgroundParticlesScriptReference.applyLifetimePositioningPositionScale, GUILayout.Width (Mathf.CeilToInt(Screen.width/4f)));
					GUI.enabled = GUI.enabled&&playgroundParticlesScriptReference.applyLifetimePositioningPositionScale;
					GUILayout.FlexibleSpace();
					lifetimePositioningPositionScale.animationCurveValue = EditorGUILayout.CurveField(lifetimePositioningPositionScale.animationCurveValue, GUILayout.Width (Mathf.CeilToInt(Screen.width/1.805f)));
					GUI.enabled = (playgroundParticlesScriptReference.onlyLifetimePositioning&&!onlySourcePositioning.boolValue);
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					playgroundParticlesScriptReference.applyLifetimePositioningTimeScale = EditorGUILayout.ToggleLeft (playgroundLanguage.timeScale, playgroundParticlesScriptReference.applyLifetimePositioningTimeScale, GUILayout.Width (Mathf.CeilToInt(Screen.width/4f)));
					GUI.enabled = GUI.enabled&&playgroundParticlesScriptReference.applyLifetimePositioningTimeScale;
					GUILayout.FlexibleSpace();
					lifetimePositioningTimeScale.animationCurveValue = EditorGUILayout.CurveField(lifetimePositioningTimeScale.animationCurveValue, GUILayout.Width (Mathf.CeilToInt(Screen.width/1.805f)));
					GUI.enabled = (playgroundParticlesScriptReference.onlyLifetimePositioning&&!onlySourcePositioning.boolValue);
					EditorGUILayout.EndHorizontal();
					
					playgroundParticlesScriptReference.lifetimePositioningScale = EditorGUILayout.FloatField (playgroundLanguage.scale, playgroundParticlesScriptReference.lifetimePositioningScale);
					playgroundParticlesScriptReference.lifetimePositioningUsesSourceDirection = EditorGUILayout.Toggle (playgroundLanguage.useSourceDirection, playgroundParticlesScriptReference.lifetimePositioningUsesSourceDirection);
					
					
					EditorGUI.indentLevel--;
					GUI.enabled = !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning;
					
					EditorGUILayout.Separator();
					
					// Transition back to source
					playgroundParticlesScriptReference.transitionBackToSource = EditorGUILayout.Toggle(playgroundLanguage.transitionBackToSource, playgroundParticlesScriptReference.transitionBackToSource);
					EditorGUI.indentLevel++;
					GUI.enabled = GUI.enabled&&playgroundParticlesScriptReference.transitionBackToSource;
					playgroundParticlesScriptReference.transitionBackToSourceAmount = EditorGUILayout.CurveField(playgroundLanguage.transitionAmount, playgroundParticlesScriptReference.transitionBackToSourceAmount);
					GUI.enabled = !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning;
					EditorGUI.indentLevel--;
					
					EditorGUILayout.Separator();
					
					// Axis constraints
					GUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(playgroundLanguage.axisConstraints, GUILayout.Width(Mathf.FloorToInt(Screen.width/2.2f)-46));
					
					GUILayout.Label("X", GUILayout.Width(10));
					playgroundParticlesScriptReference.axisConstraints.x = EditorGUILayout.Toggle(playgroundParticlesScriptReference.axisConstraints.x, GUILayout.Width(16));
					GUILayout.Label("Y", GUILayout.Width(10));
					playgroundParticlesScriptReference.axisConstraints.y = EditorGUILayout.Toggle(playgroundParticlesScriptReference.axisConstraints.y, GUILayout.Width(16));
					GUILayout.Label("Z", GUILayout.Width(10));
					playgroundParticlesScriptReference.axisConstraints.z = EditorGUILayout.Toggle(playgroundParticlesScriptReference.axisConstraints.z, GUILayout.Width(16));
					GUILayout.EndHorizontal();
					playgroundParticlesScriptReference.maxVelocity = EditorGUILayout.Slider(playgroundLanguage.maxVelocity, playgroundParticlesScriptReference.maxVelocity, 0, playgroundSettings.maximumAllowedVelocity);
				}
				EditorGUILayout.EndVertical();
				
				GUI.enabled = true;
				
				// Initial Velocity
				EditorGUILayout.BeginVertical (boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.forceInitialVelocityFoldout = GUILayout.Toggle(playgroundSettings.forceInitialVelocityFoldout, playgroundLanguage.initialVelocity, EditorStyles.foldout);
				GUILayout.Label (((playgroundParticlesScriptReference.calculateDeltaMovement||applyInitialVelocity.boolValue||applyInitialLocalVelocity.boolValue)?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.forceInitialVelocityFoldout) {
					
					// Delta Movement
					if (playgroundParticlesScriptReference.source==SOURCEC.State && playgroundParticlesScriptReference.states!=null && playgroundParticlesScriptReference.states.Count>0 && playgroundParticlesScriptReference.states[playgroundParticlesScriptReference.activeState].stateTransform==null) {
						EditorGUILayout.HelpBox(playgroundLanguage.assignTransformDeltaMovement, MessageType.Info);
						GUI.enabled = false;
					} else GUI.enabled = (source.intValue!=4 && !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning);
					playgroundParticlesScriptReference.calculateDeltaMovement = EditorGUILayout.ToggleLeft(playgroundLanguage.deltaMovement, playgroundParticlesScriptReference.calculateDeltaMovement);
					GUI.enabled = (GUI.enabled && playgroundParticlesScriptReference.calculateDeltaMovement && !onlySourcePositioning.boolValue);
					//EditorGUI.indentLevel++;
					//deltaMovementStrength.floatValue = EditorGUILayout.Slider(playgroundLanguage.deltaMovementStrength, deltaMovementStrength.floatValue, 0, playgroundSettings.maximumAllowedDeltaMovementStrength);
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space (16f);
					if (playgroundParticlesScriptReference.deltaMovementStrengthValueMethod==VALUEMETHOD.Constant) {
						deltaMovementStrength.floatValue = EditorGUILayout.Slider(playgroundLanguage.deltaMovementStrength, deltaMovementStrength.floatValue, 0, playgroundSettings.maximumAllowedDeltaMovementStrength);
					} else {
						GUILayout.Label(playgroundLanguage.deltaMovementStrength);
						EditorGUILayout.Separator();
						float deltaMin = playgroundParticlesScriptReference.minDeltaMovementStrength;
						float deltaMax = playgroundParticlesScriptReference.deltaMovementStrength;
						EditorGUILayout.MinMaxSlider(ref deltaMin, ref deltaMax, 0, playgroundSettings.maximumAllowedDeltaMovementStrength, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-125));
						playgroundParticlesScriptReference.minDeltaMovementStrength = Mathf.Clamp (deltaMin, 0, playgroundSettings.maximumAllowedDeltaMovementStrength);
						playgroundParticlesScriptReference.deltaMovementStrength = Mathf.Clamp (deltaMax, 0, playgroundSettings.maximumAllowedDeltaMovementStrength);
						playgroundParticlesScriptReference.minDeltaMovementStrength = EditorGUILayout.FloatField(playgroundParticlesScriptReference.minDeltaMovementStrength, GUILayout.Width(50));
						playgroundParticlesScriptReference.deltaMovementStrength = EditorGUILayout.FloatField(playgroundParticlesScriptReference.deltaMovementStrength, GUILayout.Width(50));

					}
					playgroundParticlesScriptReference.deltaMovementStrengthValueMethod = (VALUEMETHOD)EditorGUILayout.EnumPopup(playgroundParticlesScriptReference.deltaMovementStrengthValueMethod, EditorStyles.toolbarDropDown, GUILayout.MaxWidth (12));
					EditorGUILayout.EndHorizontal();
					//EditorGUI.indentLevel--;
					GUI.enabled = !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning;
					
					// Initial Velocity
					EditorGUILayout.Separator();
					EditorGUILayout.BeginHorizontal();
					applyInitialVelocity.boolValue = EditorGUILayout.ToggleLeft(playgroundLanguage.initialGlobalVelocity, applyInitialVelocity.boolValue);
					GUI.enabled = (applyInitialVelocity.boolValue&&!onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning);
					if (GUILayout.Button (playgroundLanguage.reset, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						playgroundParticlesScriptReference.initialVelocityMin = Vector3.zero;
						playgroundParticlesScriptReference.initialVelocityMax = Vector3.zero;
					}
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.initialVelocityMethod = (MINMAXVECTOR3METHOD)EditorGUILayout.EnumPopup (playgroundLanguage.method, playgroundParticlesScriptReference.initialVelocityMethod);
					EditorGUI.indentLevel--;
					// X
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label(playgroundParticlesScriptReference.initialVelocityMethod==MINMAXVECTOR3METHOD.Rectangular||playgroundParticlesScriptReference.initialVelocityMethod==MINMAXVECTOR3METHOD.RectangularLinear?"X":playgroundLanguage.range, GUILayout.Width(50));
					EditorGUILayout.Separator();
					float initialVelocityMinX = playgroundParticlesScriptReference.initialVelocityMin.x;
					float initialVelocityMaxX = playgroundParticlesScriptReference.initialVelocityMax.x;
					EditorGUILayout.MinMaxSlider(ref initialVelocityMinX, ref initialVelocityMaxX, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
					playgroundParticlesScriptReference.initialVelocityMin.x = Mathf.Clamp (initialVelocityMinX, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
					playgroundParticlesScriptReference.initialVelocityMax.x = Mathf.Clamp (initialVelocityMaxX, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
					playgroundParticlesScriptReference.initialVelocityMin.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialVelocityMin.x, GUILayout.Width(50));
					playgroundParticlesScriptReference.initialVelocityMax.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialVelocityMax.x, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					if (playgroundParticlesScriptReference.initialVelocityMethod==MINMAXVECTOR3METHOD.Rectangular || playgroundParticlesScriptReference.initialVelocityMethod==MINMAXVECTOR3METHOD.RectangularLinear) {
						// Y
						GUILayout.BeginHorizontal();
						GUILayout.Space(16);
						GUILayout.Label("Y");
						EditorGUILayout.Separator();
						float initialVelocityMinY = playgroundParticlesScriptReference.initialVelocityMin.y;
						float initialVelocityMaxY = playgroundParticlesScriptReference.initialVelocityMax.y;
						EditorGUILayout.MinMaxSlider(ref initialVelocityMinY, ref initialVelocityMaxY, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
						playgroundParticlesScriptReference.initialVelocityMin.y = Mathf.Clamp (initialVelocityMinY, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
						playgroundParticlesScriptReference.initialVelocityMax.y = Mathf.Clamp (initialVelocityMaxY, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
						playgroundParticlesScriptReference.initialVelocityMin.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialVelocityMin.y, GUILayout.Width(50));
						playgroundParticlesScriptReference.initialVelocityMax.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialVelocityMax.y, GUILayout.Width(50));
						GUILayout.EndHorizontal();
						// Z
						GUILayout.BeginHorizontal();
						GUILayout.Space(16);
						GUILayout.Label("Z");
						EditorGUILayout.Separator();
						float initialVelocityMinZ = playgroundParticlesScriptReference.initialVelocityMin.z;
						float initialVelocityMaxZ = playgroundParticlesScriptReference.initialVelocityMax.z;
						EditorGUILayout.MinMaxSlider(ref initialVelocityMinZ, ref initialVelocityMaxZ, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
						playgroundParticlesScriptReference.initialVelocityMin.z = Mathf.Clamp (initialVelocityMinZ, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
						playgroundParticlesScriptReference.initialVelocityMax.z = Mathf.Clamp (initialVelocityMaxZ, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
						playgroundParticlesScriptReference.initialVelocityMin.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialVelocityMin.z, GUILayout.Width(50));
						playgroundParticlesScriptReference.initialVelocityMax.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialVelocityMax.z, GUILayout.Width(50));
						GUILayout.EndHorizontal();
					}
					/*
				if (playgroundParticlesScriptReference.initialVelocityMethod==MINMAXVECTOR3METHOD.SphericalSector || playgroundParticlesScriptReference.initialVelocityMethod==MINMAXVECTOR3METHOD.SphericalSectorLinear) {
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.initialVelocityMin.y = EditorGUILayout.Slider(playgroundLanguage.sectorA, playgroundParticlesScriptReference.initialVelocityMin.y, -1f, 1f);
					playgroundParticlesScriptReference.initialVelocityMax.y = EditorGUILayout.Slider(playgroundLanguage.sectorB, playgroundParticlesScriptReference.initialVelocityMax.y, 0, 1f);
					EditorGUI.indentLevel--;
				}
				*/
					GUI.enabled = !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning;
					
					// Initial Local Velocity
					EditorGUILayout.Separator();
					GUI.enabled=(source.intValue!=4 && !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning);
					
					if (source.intValue==4) {
						GUI.enabled = true;
						EditorGUILayout.HelpBox(playgroundLanguage.initialLocalVelocityControlledByScript, MessageType.Info);
						GUI.enabled = false;
					}
					EditorGUILayout.BeginHorizontal();
					applyInitialLocalVelocity.boolValue = EditorGUILayout.ToggleLeft(playgroundLanguage.initialLocalVelocity, applyInitialLocalVelocity.boolValue);
					if (playgroundParticlesScriptReference.source==SOURCEC.State && playgroundParticlesScriptReference.states!=null && playgroundParticlesScriptReference.states.Count>0 && playgroundParticlesScriptReference.states[playgroundParticlesScriptReference.activeState].stateTransform==null) {
						EditorGUILayout.HelpBox(playgroundLanguage.assignTransformLocalVelocity, MessageType.Info);
						GUI.enabled = false;
					} else GUI.enabled = (applyInitialLocalVelocity.boolValue&&!onlySourcePositioning.boolValue&&source.intValue!=4&&!playgroundParticlesScriptReference.onlyLifetimePositioning);
					
					if (GUILayout.Button (playgroundLanguage.reset, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						playgroundParticlesScriptReference.initialLocalVelocityMin = Vector3.zero;
						playgroundParticlesScriptReference.initialLocalVelocityMax = Vector3.zero;
					}
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.initialLocalVelocityMethod = (MINMAXVECTOR3METHOD)EditorGUILayout.EnumPopup (playgroundLanguage.method, playgroundParticlesScriptReference.initialLocalVelocityMethod);
					EditorGUI.indentLevel--;
					// X
					GUILayout.BeginHorizontal();
					GUILayout.Space(16);
					GUILayout.Label(playgroundParticlesScriptReference.initialLocalVelocityMethod==MINMAXVECTOR3METHOD.Rectangular||playgroundParticlesScriptReference.initialLocalVelocityMethod==MINMAXVECTOR3METHOD.RectangularLinear?"X":playgroundLanguage.range, GUILayout.Width(50));
					EditorGUILayout.Separator();
					float initialLocalVelocityMinX = playgroundParticlesScriptReference.initialLocalVelocityMin.x;
					float initialLocalVelocityMaxX = playgroundParticlesScriptReference.initialLocalVelocityMax.x;
					EditorGUILayout.MinMaxSlider(ref initialLocalVelocityMinX, ref initialLocalVelocityMaxX, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
					playgroundParticlesScriptReference.initialLocalVelocityMin.x = Mathf.Clamp (initialLocalVelocityMinX, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
					playgroundParticlesScriptReference.initialLocalVelocityMax.x = Mathf.Clamp (initialLocalVelocityMaxX, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
					playgroundParticlesScriptReference.initialLocalVelocityMin.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialLocalVelocityMin.x, GUILayout.Width(50));
					playgroundParticlesScriptReference.initialLocalVelocityMax.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialLocalVelocityMax.x, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					if (playgroundParticlesScriptReference.initialLocalVelocityMethod==MINMAXVECTOR3METHOD.Rectangular || playgroundParticlesScriptReference.initialLocalVelocityMethod==MINMAXVECTOR3METHOD.RectangularLinear) {
						// Y
						GUILayout.BeginHorizontal();
						GUILayout.Space(16);
						GUILayout.Label("Y");
						EditorGUILayout.Separator();
						float initialLocalVelocityMinY = playgroundParticlesScriptReference.initialLocalVelocityMin.y;
						float initialLocalVelocityMaxY = playgroundParticlesScriptReference.initialLocalVelocityMax.y;
						EditorGUILayout.MinMaxSlider(ref initialLocalVelocityMinY, ref initialLocalVelocityMaxY, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
						playgroundParticlesScriptReference.initialLocalVelocityMin.y = Mathf.Clamp (initialLocalVelocityMinY, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
						playgroundParticlesScriptReference.initialLocalVelocityMax.y = Mathf.Clamp (initialLocalVelocityMaxY, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
						playgroundParticlesScriptReference.initialLocalVelocityMin.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialLocalVelocityMin.y, GUILayout.Width(50));
						playgroundParticlesScriptReference.initialLocalVelocityMax.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialLocalVelocityMax.y, GUILayout.Width(50));
						GUILayout.EndHorizontal();
						// Z
						GUILayout.BeginHorizontal();
						GUILayout.Space(16);
						GUILayout.Label("Z");
						EditorGUILayout.Separator();
						float initialLocalVelocityMinZ = playgroundParticlesScriptReference.initialLocalVelocityMin.z;
						float initialLocalVelocityMaxZ = playgroundParticlesScriptReference.initialLocalVelocityMax.z;
						EditorGUILayout.MinMaxSlider(ref initialLocalVelocityMinZ, ref initialLocalVelocityMaxZ, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
						playgroundParticlesScriptReference.initialLocalVelocityMin.z = Mathf.Clamp (initialLocalVelocityMinZ, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
						playgroundParticlesScriptReference.initialLocalVelocityMax.z = Mathf.Clamp (initialLocalVelocityMaxZ, -playgroundSettings.maximumAllowedInitialVelocity, playgroundSettings.maximumAllowedInitialVelocity);
						playgroundParticlesScriptReference.initialLocalVelocityMin.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialLocalVelocityMin.z, GUILayout.Width(50));
						playgroundParticlesScriptReference.initialLocalVelocityMax.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.initialLocalVelocityMax.z, GUILayout.Width(50));
						GUILayout.EndHorizontal();
					}
					/*
				if (playgroundParticlesScriptReference.initialLocalVelocityMethod==MINMAXVECTOR3METHOD.SphericalSector || playgroundParticlesScriptReference.initialLocalVelocityMethod==MINMAXVECTOR3METHOD.SphericalSectorLinear) {
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.initialLocalVelocityMin.y = EditorGUILayout.Slider(playgroundLanguage.sectorA, playgroundParticlesScriptReference.initialLocalVelocityMin.y, -1f, 1f);
					playgroundParticlesScriptReference.initialLocalVelocityMax.y = EditorGUILayout.Slider(playgroundLanguage.sectorB, playgroundParticlesScriptReference.initialLocalVelocityMax.y, 0, 1f);
					EditorGUI.indentLevel--;
				}*/
					
					EditorGUILayout.Separator();
					GUI.enabled = !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning;
					
					// Initial velocity shape
					EditorGUILayout.BeginHorizontal();
					playgroundParticlesScriptReference.applyInitialVelocityShape = EditorGUILayout.ToggleLeft(playgroundLanguage.initialVelocityShape, playgroundParticlesScriptReference.applyInitialVelocityShape);
					GUI.enabled = (playgroundParticlesScriptReference.applyInitialVelocityShape&&!onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning);
					if (GUILayout.Button (playgroundLanguage.reset, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
						playgroundParticlesScriptReference.initialVelocityShape.Reset1();
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel++;
					initialVelocityShapeX.animationCurveValue = EditorGUILayout.CurveField("X", initialVelocityShapeX.animationCurveValue);
					initialVelocityShapeY.animationCurveValue = EditorGUILayout.CurveField("Y", initialVelocityShapeY.animationCurveValue);
					initialVelocityShapeZ.animationCurveValue = EditorGUILayout.CurveField("Z", initialVelocityShapeZ.animationCurveValue);
					playgroundParticlesScriptReference.initialVelocityShapeScale = EditorGUILayout.FloatField (playgroundLanguage.scale, playgroundParticlesScriptReference.initialVelocityShapeScale);
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.EndVertical();
				
				GUI.enabled = true;
				
				// Lifetime velocity
				EditorGUILayout.BeginVertical (boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.forceLifetimeVelocityFoldout = GUILayout.Toggle(playgroundSettings.forceLifetimeVelocityFoldout, playgroundLanguage.lifetimeVelocity, EditorStyles.foldout);
				GUILayout.Label ((applyLifetimeVelocity.boolValue?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.forceLifetimeVelocityFoldout) {
					EditorGUILayout.BeginHorizontal();
					GUI.enabled = !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning;
					applyLifetimeVelocity.boolValue = EditorGUILayout.ToggleLeft(playgroundLanguage.lifetimeVelocity, applyLifetimeVelocity.boolValue);
					GUI.enabled = (applyLifetimeVelocity.boolValue&&!onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning);
					if (GUILayout.Button (playgroundLanguage.reset, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
						playgroundParticlesScriptReference.lifetimeVelocity.Reset();
					EditorGUILayout.EndHorizontal();
					
					EditorGUI.indentLevel++;
					lifeTimeVelocityX.animationCurveValue = EditorGUILayout.CurveField("X", lifeTimeVelocityX.animationCurveValue);
					lifeTimeVelocityY.animationCurveValue = EditorGUILayout.CurveField("Y", lifeTimeVelocityY.animationCurveValue);
					lifeTimeVelocityZ.animationCurveValue = EditorGUILayout.CurveField("Z", lifeTimeVelocityZ.animationCurveValue);
					playgroundParticlesScriptReference.lifetimeVelocityScale = EditorGUILayout.FloatField (playgroundLanguage.scale, playgroundParticlesScriptReference.lifetimeVelocityScale);
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.EndVertical();
				
				GUI.enabled = true;
				
				// Velocity Bending
				EditorGUILayout.BeginVertical (boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.forceVelocityBendingFoldout = GUILayout.Toggle(playgroundSettings.forceVelocityBendingFoldout, playgroundLanguage.velocityBending, EditorStyles.foldout);
				GUILayout.Label ((applyVelocityBending.boolValue?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.forceVelocityBendingFoldout) {
					GUI.enabled = !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning;
					applyVelocityBending.boolValue = EditorGUILayout.ToggleLeft(playgroundLanguage.velocityBending, applyVelocityBending.boolValue);
					GUI.enabled = (applyVelocityBending.boolValue&&!onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning);
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField (velocityBendingType, new GUIContent(playgroundLanguage.type));
					playgroundParticlesScriptReference.velocityBending = EditorGUILayout.Vector3Field(playgroundLanguage.bending, playgroundParticlesScriptReference.velocityBending);
					EditorGUI.indentLevel--;
					EditorGUILayout.Separator();
				}
				EditorGUILayout.EndVertical();
				
				GUI.enabled = true;
				
				// Turbulence
				EditorGUILayout.BeginVertical (boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.forceTurbulenceFoldout = GUILayout.Toggle(playgroundSettings.forceTurbulenceFoldout, playgroundLanguage.turbulence, EditorStyles.foldout);
				GUILayout.Label ((playgroundParticlesScriptReference.turbulenceType==TURBULENCETYPE.None?playgroundLanguage.off:playgroundLanguage.on), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.forceTurbulenceFoldout) {
					GUI.enabled = !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning;
					playgroundParticlesScriptReference.turbulenceType = (TURBULENCETYPE)EditorGUILayout.EnumPopup(playgroundLanguage.turbulence, playgroundParticlesScriptReference.turbulenceType);
					GUI.enabled = (playgroundParticlesScriptReference.turbulenceType!=TURBULENCETYPE.None && !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning);
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.turbulenceStrength = EditorGUILayout.Slider(playgroundLanguage.strength, playgroundParticlesScriptReference.turbulenceStrength, 0f, playgroundSettings.maximumAllowedTurbulenceStrength);
					playgroundParticlesScriptReference.turbulenceScale = EditorGUILayout.Slider(playgroundLanguage.scale, playgroundParticlesScriptReference.turbulenceScale, 0f, playgroundSettings.maximumAllowedTurbulenceScale);
					playgroundParticlesScriptReference.turbulenceTimeScale = EditorGUILayout.Slider(playgroundLanguage.timeScale, playgroundParticlesScriptReference.turbulenceTimeScale, 0f, playgroundSettings.maximumAllowedTurbulenceTimeScale);
					EditorGUILayout.BeginHorizontal();
					playgroundParticlesScriptReference.turbulenceApplyLifetimeStrength = EditorGUILayout.ToggleLeft (playgroundLanguage.lifetimeStrength, playgroundParticlesScriptReference.turbulenceApplyLifetimeStrength, GUILayout.MaxWidth(Mathf.CeilToInt(EditorGUIUtility.labelWidth)-28));
					GUI.enabled = (playgroundParticlesScriptReference.turbulenceApplyLifetimeStrength && playgroundParticlesScriptReference.turbulenceType!=TURBULENCETYPE.None && !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning);
					turbulenceLifetimeStrength.animationCurveValue = EditorGUILayout.CurveField(turbulenceLifetimeStrength.animationCurveValue);
					EditorGUILayout.EndHorizontal();
					EditorGUI.indentLevel--;
					
				}
				EditorGUILayout.EndVertical();
				
				GUI.enabled = true;
				
				// Constant Force
				EditorGUILayout.BeginVertical (boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.forceConstantForceFoldout = GUILayout.Toggle(playgroundSettings.forceConstantForceFoldout, playgroundLanguage.constantForce, EditorStyles.foldout);
				GUILayout.Label ((playgroundParticlesScriptReference.gravity==Vector3.zero&&playgroundParticlesScriptReference.damping==0?playgroundLanguage.off:playgroundLanguage.on), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.forceConstantForceFoldout) {
					GUI.enabled = !onlySourcePositioning.boolValue&&!playgroundParticlesScriptReference.onlyLifetimePositioning;
					playgroundParticlesScriptReference.gravity = EditorGUILayout.Vector3Field(playgroundLanguage.gravity, playgroundParticlesScriptReference.gravity);
					playgroundParticlesScriptReference.damping = EditorGUILayout.Slider(playgroundLanguage.damping, playgroundParticlesScriptReference.damping, 0f, playgroundSettings.maximumAllowedDamping);
					playgroundParticlesScriptReference.velocityScale = EditorGUILayout.Slider(playgroundLanguage.velocityScale, playgroundParticlesScriptReference.velocityScale, 0f, playgroundSettings.maximumAllowedVelocity);
				}
				EditorGUILayout.EndVertical();
				GUI.enabled = true;
			}
			
			// Collision Settings
			if (GUILayout.Button(collision.boolValue?collisionType.intValue==0?playgroundLanguage.collision+" ("+playgroundLanguage.threeDimensional+")":playgroundLanguage.collision+" ("+playgroundLanguage.twoDimensional+")":playgroundLanguage.collision+" ("+playgroundLanguage.off+")", EditorStyles.toolbarDropDown)) playgroundSettings.collisionFoldout=!playgroundSettings.collisionFoldout;
			if (playgroundSettings.collisionFoldout) {
				
				if (playgroundParticlesScriptReference.onlySourcePositioning||playgroundParticlesScriptReference.onlyLifetimePositioning) {
					EditorGUILayout.HelpBox(playgroundLanguage.collisionDisabledDescription, MessageType.Info);
					EditorGUILayout.Separator();
				}
				
				EditorGUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.collisionSettingsFoldout = GUILayout.Toggle(playgroundSettings.collisionSettingsFoldout, playgroundLanguage.collisionSettings, EditorStyles.foldout);
				GUILayout.Label ((collision.boolValue?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.collisionSettingsFoldout) {
					GUI.enabled = !playgroundParticlesScriptReference.onlySourcePositioning&&!playgroundParticlesScriptReference.onlyLifetimePositioning;
					collision.boolValue = EditorGUILayout.ToggleLeft(playgroundLanguage.collision, collision.boolValue);
					EditorGUI.indentLevel++;
					GUI.enabled = GUI.enabled&&collision.boolValue;
					EditorGUILayout.PropertyField(collisionType, new GUIContent(playgroundLanguage.collisionType));
					if (collisionType.enumValueIndex==1) {
						GUILayout.BeginHorizontal();
						GUILayout.Space (16);
						GUILayout.Label(playgroundLanguage.depth);
						EditorGUILayout.Separator();
						float minDepth = playgroundParticlesScriptReference.minCollisionDepth;
						float maxDepth = playgroundParticlesScriptReference.maxCollisionDepth;
						EditorGUILayout.MinMaxSlider(ref minDepth, ref maxDepth, -playgroundSettings.maximumAllowedDepth, playgroundSettings.maximumAllowedDepth, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
						playgroundParticlesScriptReference.minCollisionDepth = Mathf.Clamp (minDepth, -playgroundSettings.maximumAllowedDepth, playgroundSettings.maximumAllowedDepth);
						playgroundParticlesScriptReference.maxCollisionDepth = Mathf.Clamp (maxDepth, -playgroundSettings.maximumAllowedDepth, playgroundSettings.maximumAllowedDepth);
						playgroundParticlesScriptReference.minCollisionDepth = EditorGUILayout.FloatField(playgroundParticlesScriptReference.minCollisionDepth, GUILayout.Width(50));
						playgroundParticlesScriptReference.maxCollisionDepth = EditorGUILayout.FloatField(playgroundParticlesScriptReference.maxCollisionDepth, GUILayout.Width(50));
						GUILayout.EndHorizontal();
					}
					EditorGUILayout.PropertyField(collisionMask, new GUIContent(playgroundLanguage.collisionMask));
					affectRigidbodies.boolValue = EditorGUILayout.Toggle(playgroundLanguage.collideWithRigidbodies, affectRigidbodies.boolValue);
					playgroundParticlesScriptReference.inverseRigidbodyCollision = EditorGUILayout.Toggle (playgroundLanguage.inverseRigidbodyCollision, playgroundParticlesScriptReference.inverseRigidbodyCollision);
					playgroundParticlesScriptReference.maskedParticlesBypassCollision = EditorGUILayout.Toggle (playgroundLanguage.bypassMaskedParticles, playgroundParticlesScriptReference.maskedParticlesBypassCollision);
					mass.floatValue = EditorGUILayout.Slider(playgroundLanguage.mass, mass.floatValue, 0, playgroundSettings.maximumAllowedMass);
					collisionRadius.floatValue = EditorGUILayout.Slider(playgroundLanguage.collisionRadius, collisionRadius.floatValue, 0, playgroundSettings.maximumAllowedCollisionRadius);
					playgroundParticlesScriptReference.lifetimeLoss = EditorGUILayout.Slider(playgroundLanguage.lifetimeLoss, playgroundParticlesScriptReference.lifetimeLoss, 0f, 1f);
					
					EditorGUILayout.Separator();
					playgroundParticlesScriptReference.stickyCollisions = EditorGUILayout.Toggle (playgroundLanguage.sticky, playgroundParticlesScriptReference.stickyCollisions);
					if (playgroundParticlesScriptReference.stickyCollisions) {
						EditorGUI.indentLevel++;
						playgroundParticlesScriptReference.stickyCollisionsSurfaceOffset = EditorGUILayout.FloatField (playgroundLanguage.surfaceOffset, playgroundParticlesScriptReference.stickyCollisionsSurfaceOffset);
						EditorGUILayout.PropertyField(stickyCollisionMask, new GUIContent(playgroundLanguage.stickyMask));
						EditorGUI.indentLevel--;
					}
					GUI.enabled = GUI.enabled&&collision.boolValue;
					EditorGUILayout.Separator ();
					bounciness.floatValue = EditorGUILayout.Slider(playgroundLanguage.bounciness, bounciness.floatValue, 0, playgroundSettings.maximumAllowedBounciness);
					GUI.enabled = collision.boolValue;
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel(playgroundLanguage.randomBounce);
					EditorGUILayout.Separator ();
					if (GUILayout.Button (playgroundLanguage.reset, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
						playgroundParticlesScriptReference.bounceRandomMin = Vector3.zero;
						playgroundParticlesScriptReference.bounceRandomMax = Vector3.zero;
					}
					EditorGUILayout.EndHorizontal();
					// X
					GUILayout.BeginHorizontal();
					GUILayout.Space(32);
					GUILayout.Label("X", GUILayout.Width(50));
					EditorGUILayout.Separator();
					float bounceRandomMinX = playgroundParticlesScriptReference.bounceRandomMin.x;
					float bounceRandomMaxX = playgroundParticlesScriptReference.bounceRandomMax.x;
					EditorGUILayout.MinMaxSlider(ref bounceRandomMinX, ref bounceRandomMaxX, -1f, 1f, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-100));
					playgroundParticlesScriptReference.bounceRandomMin.x = Mathf.Clamp (bounceRandomMinX, -1f, 1f);
					playgroundParticlesScriptReference.bounceRandomMax.x = Mathf.Clamp (bounceRandomMaxX, -1f, 1f);
					playgroundParticlesScriptReference.bounceRandomMin.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.bounceRandomMin.x, GUILayout.Width(50));
					playgroundParticlesScriptReference.bounceRandomMax.x = EditorGUILayout.FloatField(playgroundParticlesScriptReference.bounceRandomMax.x, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					// Y
					GUILayout.BeginHorizontal();
					GUILayout.Space(32);
					GUILayout.Label("Y");
					EditorGUILayout.Separator();
					float bounceRandomMinY = playgroundParticlesScriptReference.bounceRandomMin.y;
					float bounceRandomMaxY = playgroundParticlesScriptReference.bounceRandomMax.y;
					EditorGUILayout.MinMaxSlider(ref bounceRandomMinY, ref bounceRandomMaxY, -1f, 1f, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-100));
					playgroundParticlesScriptReference.bounceRandomMin.y = Mathf.Clamp (bounceRandomMinY, -1f, 1f);
					playgroundParticlesScriptReference.bounceRandomMax.y = Mathf.Clamp (bounceRandomMaxY, -1f, 1f);
					playgroundParticlesScriptReference.bounceRandomMin.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.bounceRandomMin.y, GUILayout.Width(50));
					playgroundParticlesScriptReference.bounceRandomMax.y = EditorGUILayout.FloatField(playgroundParticlesScriptReference.bounceRandomMax.y, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					// Z
					GUILayout.BeginHorizontal();
					GUILayout.Space(32);
					GUILayout.Label("Z");
					EditorGUILayout.Separator();
					float bounceRandomMinZ = playgroundParticlesScriptReference.bounceRandomMin.z;
					float bounceRandomMaxZ = playgroundParticlesScriptReference.bounceRandomMax.z;
					EditorGUILayout.MinMaxSlider(ref bounceRandomMinZ, ref bounceRandomMaxZ, -1f, 1f, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-100));
					playgroundParticlesScriptReference.bounceRandomMin.z = Mathf.Clamp (bounceRandomMinZ, -1f, 1f);
					playgroundParticlesScriptReference.bounceRandomMax.z = Mathf.Clamp (bounceRandomMaxZ, -1f, 1f);
					playgroundParticlesScriptReference.bounceRandomMin.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.bounceRandomMin.z, GUILayout.Width(50));
					playgroundParticlesScriptReference.bounceRandomMax.z = EditorGUILayout.FloatField(playgroundParticlesScriptReference.bounceRandomMax.z, GUILayout.Width(50));
					GUILayout.EndHorizontal();
					
					EditorGUILayout.Separator();
					playgroundParticlesScriptReference.collisionPrecision = EditorGUILayout.Toggle (playgroundLanguage.collisionPrecision, playgroundParticlesScriptReference.collisionPrecision);
					playgroundParticlesScriptReference.forceCollisionCaching = EditorGUILayout.Toggle (playgroundLanguage.forceCollisionCaching, playgroundParticlesScriptReference.forceCollisionCaching);
					EditorGUILayout.Separator();
					
					EditorGUI.indentLevel--;
				}

				EditorGUILayout.EndVertical();
				
				// Collision planes List
				GUI.enabled = true;
				EditorGUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.collisionPlanesFoldout = GUILayout.Toggle(playgroundSettings.collisionPlanesFoldout, playgroundLanguage.collisionPlanes, EditorStyles.foldout);
				GUILayout.Label (playgroundParticlesScriptReference.colliders.Count.ToString(), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.collisionPlanesFoldout) {
					if (playgroundParticlesScriptReference.colliders.Count>0) {
						for (int c = 0; c<playgroundParticlesScriptReference.colliders.Count; c++) {
							EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(26));
							EditorGUILayout.BeginHorizontal();
							
							playgroundParticlesScriptReference.colliders[c].enabled = EditorGUILayout.Toggle("", playgroundParticlesScriptReference.colliders[c].enabled, GUILayout.Width(16));
							GUI.enabled = (GUI.enabled&&playgroundParticlesScriptReference.colliders[c].enabled);
							playgroundParticlesScriptReference.colliders[c].transform = EditorGUILayout.ObjectField("", playgroundParticlesScriptReference.colliders[c].transform, typeof(Transform), true) as Transform;
							playgroundParticlesScriptReference.colliders[c].offset = EditorGUILayout.Vector3Field("", playgroundParticlesScriptReference.colliders[c].offset, GUILayout.Width(Mathf.FloorToInt(Screen.width/1.8f)-142));
							GUI.enabled = true;
							
							EditorGUILayout.Separator();
							if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								playgroundParticlesScriptReference.colliders.RemoveAt(c);
							}
							
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.EndVertical();
						}
					} else {
						EditorGUILayout.HelpBox(playgroundLanguage.noCollisionPlanes, MessageType.Info);
					}
					
					if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth (false))){
						playgroundParticlesScriptReference.colliders.Add(new PlaygroundColliderC());
					}
					
					EditorGUILayout.Separator();
					playgroundScriptReference.collisionPlaneScale = EditorGUILayout.Slider(playgroundLanguage.gizmoScale, playgroundScriptReference.collisionPlaneScale, 0, 1);
					EditorGUILayout.Separator();
				}
				EditorGUILayout.EndVertical();
				
				// Collision Exclusion list
				GUI.enabled = true;
				EditorGUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.collisionExclusionFoldout = GUILayout.Toggle(playgroundSettings.collisionExclusionFoldout, playgroundLanguage.collisionExclusion, EditorStyles.foldout);
				GUILayout.Label (playgroundParticlesScriptReference.collisionExclusion.Count.ToString(), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.collisionExclusionFoldout) {
					if (playgroundParticlesScriptReference.collisionExclusion.Count>0) {
						for (int c = 0; c<playgroundParticlesScriptReference.collisionExclusion.Count; c++) {
							EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(26));
							EditorGUILayout.BeginHorizontal();
							playgroundParticlesScriptReference.collisionExclusion[c] = EditorGUILayout.ObjectField("", playgroundParticlesScriptReference.collisionExclusion[c], typeof(Transform), true) as Transform;
							EditorGUILayout.Separator();
							if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								playgroundParticlesScriptReference.collisionExclusion.RemoveAt(c);
							}
							
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.EndVertical();
						}
					} else {
						EditorGUILayout.HelpBox(playgroundLanguage.collisionExclusionMessage, MessageType.Info);
					}
					
					if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth (false))){
						playgroundParticlesScriptReference.collisionExclusion.Add(null);
					}
				}
				EditorGUILayout.EndVertical();
				
				GUI.enabled = true;				
			}
			
			// Render Settings
			if (GUILayout.Button(playgroundLanguage.rendering+" ("+playgroundParticlesScriptReference.colorSource+")", EditorStyles.toolbarDropDown)) playgroundSettings.renderingFoldout=!playgroundSettings.renderingFoldout;
			if (playgroundSettings.renderingFoldout) {
				
				// Material
				EditorGUILayout.BeginVertical (boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.renderingMaterialFoldout = GUILayout.Toggle(playgroundSettings.renderingMaterialFoldout, playgroundLanguage.material, EditorStyles.foldout);
				GUILayout.Label ((playgroundParticlesScriptReference.particleSystemRenderer.sharedMaterial!=null?playgroundParticlesScriptReference.particleSystemRenderer.sharedMaterial.name:playgroundLanguage.none), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.renderingMaterialFoldout) {
					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel(playgroundLanguage.material);
					Material currentMat = particleMaterial as Material;
					particleMaterial = EditorGUILayout.ObjectField(particleMaterial, typeof(Material), false);
					if (currentMat!=particleMaterial) 
						PlaygroundParticlesC.SetMaterial(playgroundParticlesScriptReference, particleMaterial as Material);
					GUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
				
				// Color
				EditorGUILayout.BeginVertical (boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.renderingColorFoldout = GUILayout.Toggle(playgroundSettings.renderingColorFoldout, playgroundLanguage.color, EditorStyles.foldout);
				GUILayout.Label (playgroundParticlesScriptReference.colorSource.ToString(), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.renderingColorFoldout) {
					playgroundParticlesScriptReference.colorSource = (COLORSOURCEC)EditorGUILayout.EnumPopup(playgroundLanguage.colorSource, playgroundParticlesScriptReference.colorSource);
					playgroundParticlesScriptReference.colorMethod = (COLORMETHOD)EditorGUILayout.EnumPopup(playgroundLanguage.colorMethod, playgroundParticlesScriptReference.colorMethod);
					switch (playgroundParticlesScriptReference.colorSource) {
					case COLORSOURCEC.Source: 
						EditorGUILayout.PropertyField(lifetimeColor, new GUIContent(playgroundParticlesScriptReference.colorMethod==COLORMETHOD.Lifetime?playgroundLanguage.lifetimeColor:playgroundLanguage.arrayColor));
						playgroundParticlesScriptReference.sourceUsesLifetimeAlpha = EditorGUILayout.Toggle(playgroundLanguage.sourceUsesLifetimeAlpha, playgroundParticlesScriptReference.sourceUsesLifetimeAlpha);
						break;
					case COLORSOURCEC.LifetimeColor:
						EditorGUILayout.PropertyField(lifetimeColor, new GUIContent(playgroundParticlesScriptReference.colorMethod==COLORMETHOD.Lifetime?playgroundLanguage.lifetimeColor:playgroundLanguage.arrayColor));
						break;
					case COLORSOURCEC.LifetimeColors:
						if (lifetimeColors.arraySize>0) {
							
							SerializedProperty thisLifetimeColor;
							for (int c = 0; c<lifetimeColors.arraySize; c++) {
								thisLifetimeColor = lifetimeColors.GetArrayElementAtIndex(c).FindPropertyRelative("gradient");
								GUILayout.BeginHorizontal(boxStyle);
								EditorGUILayout.PropertyField (thisLifetimeColor);
								if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})) {
									lifetimeColors.DeleteArrayElementAtIndex(c);
									playgroundParticles.ApplyModifiedProperties();
								}
								GUILayout.EndHorizontal();
							}
							
						} else {
							EditorGUILayout.HelpBox(playgroundLanguage.noLifetimeColors, MessageType.Info);
						}
						if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
							playgroundParticlesScriptReference.lifetimeColors.Add (new PlaygroundGradientC());
							playgroundParticlesScriptReference.lifetimeColors[playgroundParticlesScriptReference.lifetimeColors.Count-1].gradient = new Gradient();
							playgroundParticles.ApplyModifiedProperties();
						}
						break;
					}
					if (playgroundParticlesScriptReference.colorMethod==COLORMETHOD.ParticleArray) {
						EditorGUILayout.PropertyField(arrayColor, new GUIContent(playgroundLanguage.arrayAlpha));
						playgroundParticlesScriptReference.arrayColorUsesAlpha = EditorGUILayout.Toggle(playgroundLanguage.applyArrayAlpha, playgroundParticlesScriptReference.arrayColorUsesAlpha);
					}
				}
				EditorGUILayout.EndVertical ();
				
				// Render mode
				EditorGUILayout.BeginVertical (boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.renderingRenderModeFoldout = GUILayout.Toggle(playgroundSettings.renderingRenderModeFoldout, playgroundLanguage.renderMode, EditorStyles.foldout);
				GUILayout.Label (playgroundParticlesScriptReference.particleSystemRenderer2.renderMode.ToString(), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.renderingRenderModeFoldout) {
					shurikenRenderer.renderMode = (ParticleSystemRenderMode)EditorGUILayout.EnumPopup(playgroundLanguage.renderMode, shurikenRenderer.renderMode);
					switch (shurikenRenderer.renderMode) {
					case ParticleSystemRenderMode.Stretch:
						EditorGUI.indentLevel++;
						shurikenRenderer.cameraVelocityScale = EditorGUILayout.Slider(playgroundLanguage.cameraScale, shurikenRenderer.cameraVelocityScale, -playgroundSettings.maximumRenderSliders, playgroundSettings.maximumRenderSliders);
						shurikenRenderer.velocityScale = EditorGUILayout.Slider(playgroundLanguage.speedScale, shurikenRenderer.velocityScale, -playgroundSettings.maximumRenderSliders, playgroundSettings.maximumRenderSliders);
						shurikenRenderer.lengthScale = EditorGUILayout.Slider(playgroundLanguage.lengthScale, shurikenRenderer.lengthScale, -playgroundSettings.maximumRenderSliders, playgroundSettings.maximumRenderSliders);
						playgroundParticlesScriptReference.stretchSpeed = EditorGUILayout.Slider(playgroundLanguage.stretchSpeed, playgroundParticlesScriptReference.stretchSpeed, 0, playgroundSettings.maximumAllowedStretchSpeed);
						
						EditorGUILayout.BeginHorizontal();
						playgroundParticlesScriptReference.applyStretchStartDirection = EditorGUILayout.ToggleLeft (playgroundLanguage.startStretch, playgroundParticlesScriptReference.applyStretchStartDirection, GUILayout.Width (140));
						GUILayout.FlexibleSpace();
						playgroundParticlesScriptReference.stretchStartDirection = EditorGUILayout.Vector3Field ("", playgroundParticlesScriptReference.stretchStartDirection);
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						playgroundParticlesScriptReference.applyLifetimeStretching = EditorGUILayout.ToggleLeft (playgroundLanguage.lifetimeStretch, playgroundParticlesScriptReference.applyLifetimeStretching, GUILayout.Width (140));
						GUILayout.FlexibleSpace();
						GUI.enabled = (playgroundParticlesScriptReference.applyLifetimeStretching);
						lifetimeStretching.animationCurveValue = EditorGUILayout.CurveField(lifetimeStretching.animationCurveValue);
						EditorGUILayout.EndHorizontal();
						EditorGUI.indentLevel--;
						break;
					case ParticleSystemRenderMode.Mesh:
						shurikenRenderer.mesh = EditorGUILayout.ObjectField(shurikenRenderer.mesh, typeof(Mesh), false) as Mesh;
						break;
					}
					EditorGUILayout.Separator();
					GUI.enabled = true;
					shurikenRenderer.maxParticleSize = EditorGUILayout.FloatField(playgroundLanguage.maxParticleSize, shurikenRenderer.maxParticleSize);
					shurikenRenderer.enabled = EditorGUILayout.Toggle(playgroundLanguage.renderer, shurikenRenderer.enabled);
				}
				EditorGUILayout.EndVertical();
				GUI.enabled = true;
				
				// Shadows
				EditorGUILayout.BeginVertical (boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.renderingShadowsFoldout = GUILayout.Toggle(playgroundSettings.renderingShadowsFoldout, playgroundLanguage.shadows, EditorStyles.foldout);
				#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
				GUILayout.Label (playgroundParticlesScriptReference.particleSystemRenderer2.castShadows||playgroundParticlesScriptReference.particleSystemRenderer2.receiveShadows?playgroundLanguage.on:playgroundLanguage.off, EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				#else
				GUILayout.Label (shurikenRenderer.shadowCastingMode!=UnityEngine.Rendering.ShadowCastingMode.Off||playgroundParticlesScriptReference.particleSystemRenderer2.receiveShadows?playgroundLanguage.on:playgroundLanguage.off, EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				#endif
				GUILayout.EndHorizontal();
				if (playgroundSettings.renderingShadowsFoldout) {
					#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
					shurikenRenderer.castShadows = EditorGUILayout.Toggle (playgroundLanguage.castShadows, shurikenRenderer.castShadows);
					#else
					shurikenRenderer.shadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)EditorGUILayout.EnumPopup (playgroundLanguage.castShadows, shurikenRenderer.shadowCastingMode);
					#endif
					shurikenRenderer.receiveShadows = EditorGUILayout.Toggle (playgroundLanguage.receiveShadows, shurikenRenderer.receiveShadows);
				}
				EditorGUILayout.EndVertical();
				
				// Sorting
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.sortingFoldout = GUILayout.Toggle(playgroundSettings.sortingFoldout, playgroundLanguage.sorting, EditorStyles.foldout);
				GUILayout.Label (sortMode.ToString(), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.sortingFoldout) {
					
					// Sort order
					sortMode = (SortMode)EditorGUILayout.EnumPopup(playgroundLanguage.sortMode, sortMode);
					sortingMode.intValue = (int)sortMode;
					EditorGUILayout.PropertyField(sortingFudge, new GUIContent(playgroundLanguage.sortingFudge));
					shurikenRendererSO.ApplyModifiedProperties();
					
					EditorGUILayout.Separator();
					
					// Sorting Layers
					int prevSelectedSortingLayer = selectedSortingLayer;
					selectedSortingLayer = EditorGUILayout.Popup(playgroundLanguage.sortingLayer, selectedSortingLayer, rendererSortingLayers);
					if (prevSelectedSortingLayer!=selectedSortingLayer) {
						shurikenRenderer.sortingLayerName = rendererSortingLayers[selectedSortingLayer];
					}
					shurikenRenderer.sortingOrder = EditorGUILayout.IntField (playgroundLanguage.orderInLayer, shurikenRenderer.sortingOrder);
				}
				GUILayout.EndVertical();
				
				// Texture Sheet Animation
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.textureSheetAnimationFoldout = GUILayout.Toggle(playgroundSettings.textureSheetAnimationFoldout, playgroundLanguage.textureSheetAnimation, EditorStyles.foldout);
				GUILayout.Label (uvModule_enabled.boolValue?playgroundLanguage.on:playgroundLanguage.off, EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.textureSheetAnimationFoldout) {
					shuriken.UpdateIfDirtyOrScript();
					uvModule_enabled.boolValue = EditorGUILayout.ToggleLeft(playgroundLanguage.enabled, uvModule_enabled.boolValue);
					EditorGUI.indentLevel++;
					EditorGUILayout.BeginHorizontal();
					GUI.enabled = uvModule_enabled.boolValue;
					EditorGUILayout.PrefixLabel(playgroundLanguage.tiles);
					EditorGUIUtility.labelWidth = 16f;
					EditorGUI.indentLevel--;
					EditorGUILayout.PropertyField(uvModule_tilesX, new GUIContent("X"));
					EditorGUILayout.PropertyField(uvModule_tilesY, new GUIContent("Y"));
					EditorGUI.indentLevel++;
					EditorGUIUtility.labelWidth = 0;
					EditorGUILayout.EndHorizontal();
					uv_animationType = (AnimationType)EditorGUILayout.EnumPopup(playgroundLanguage.animation, uv_animationType);
					uvModule_animationType.intValue = (int)uv_animationType;
					if (uvModule_animationType.intValue==1) {
						EditorGUILayout.PropertyField(uvModule_randomRow, new GUIContent(playgroundLanguage.randomRow));
						if (!uvModule_randomRow.boolValue)
							EditorGUILayout.PropertyField(uvModule_rowIndex, new GUIContent(playgroundLanguage.row));
					}
					MinMaxState prev_uvMinMax = uv_minMaxState;
					uv_minMaxState = (MinMaxState)EditorGUILayout.EnumPopup(playgroundLanguage.animationMethod, uv_minMaxState);
					uvModule_frameOverTime_minMaxState.intValue = (int)uv_minMaxState;
					switch (uvModule_frameOverTime_minMaxState.intValue) {
					case 0:
						float scalarFot = EditorGUILayout.FloatField(playgroundLanguage.frameOverTime, uvModule_frameOverTime_scalar.floatValue * ((uvModule_tilesX.intValue*1f)*(uvModule_tilesY.intValue*1f)));
						uvModule_frameOverTime_scalar.floatValue = scalarFot / ((uvModule_tilesX.intValue*1f)*(uvModule_tilesY.intValue*1f));
						break;
					case 1:
						EditorGUILayout.PropertyField (uvModule_frameOverTime_maxCurve, new GUIContent(playgroundLanguage.frameOverTime));
						break;
					case 2:
						EditorGUILayout.PropertyField (uvModule_frameOverTime_maxCurve, new GUIContent(playgroundLanguage.frameOverTime));
						EditorGUILayout.PropertyField (uvModule_frameOverTime_minCurve, new GUIContent(" "));
						break;
					case 3:
						uvModule_frameOverTime_scalar.floatValue = 1f;
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.PrefixLabel(playgroundLanguage.frameOverTime);
						EditorGUIUtility.labelWidth = 16f;
						EditorGUI.indentLevel--;
						EditorGUI.indentLevel--;
						float minFotVal = (float)uvModule_frameOverTime_minCurve.animationCurveValue[0].value;
						float maxFotVal = (float)uvModule_frameOverTime_maxCurve.animationCurveValue[0].value;
						minFotVal = EditorGUILayout.FloatField(" ", minFotVal * ((uvModule_tilesX.intValue*1f)*(uvModule_tilesY.intValue*1f)));
						maxFotVal = EditorGUILayout.FloatField(" ", maxFotVal * ((uvModule_tilesX.intValue*1f)*(uvModule_tilesY.intValue*1f)));
						minFotVal /= uvModule_tilesX.intValue*uvModule_tilesY.intValue;
						maxFotVal /= uvModule_tilesX.intValue*uvModule_tilesY.intValue;
						if (!Mathf.Approximately(uvModule_frameOverTime_minCurve.animationCurveValue[0].value, minFotVal)) {
							AnimationCurve updCurve = new AnimationCurve();
							Keyframe[] updKeys = uvModule_frameOverTime_minCurve.animationCurveValue.keys;
							updKeys[0].value = Mathf.Clamp01(minFotVal);
							updCurve.keys = updKeys;

							uvModule_frameOverTime_minCurve.animationCurveValue = updCurve;
							EditorUtility.SetDirty(playgroundParticlesScriptReference.shurikenParticleSystem);
						}
						if (!Mathf.Approximately(uvModule_frameOverTime_maxCurve.animationCurveValue[0].value, maxFotVal)) {
							AnimationCurve updCurve = new AnimationCurve();
							Keyframe[] updKeys = uvModule_frameOverTime_maxCurve.animationCurveValue.keys;
							updKeys[0].value = Mathf.Clamp01(maxFotVal);
							updCurve.keys = updKeys;
							uvModule_frameOverTime_maxCurve.animationCurveValue = updCurve;
							EditorUtility.SetDirty(playgroundParticlesScriptReference.shurikenParticleSystem);
						}

						EditorGUI.indentLevel++;
						EditorGUI.indentLevel++;
						EditorGUIUtility.labelWidth = 0;
						EditorGUILayout.EndHorizontal();
						break;
					}

					if (prev_uvMinMax != uv_minMaxState) {
						if (uv_minMaxState == MinMaxState.RandomBetweenTwoConstants) {

							// Store the curves to not loose them when working with random between two constants
							if (prev_uvMinMax == MinMaxState.Curve || prev_uvMinMax == MinMaxState.RandomBetweenTwoCurves)
								prev_uvModule_frameOverTime_maxCurve = uvModule_frameOverTime_maxCurve.animationCurveValue;
							if (prev_uvMinMax == MinMaxState.RandomBetweenTwoCurves)
								prev_uvModule_frameOverTime_minCurve = uvModule_frameOverTime_minCurve.animationCurveValue;

							/*
							AnimationCurve updCurve = new AnimationCurve();
							Keyframe[] updKeys = uvModule_frameOverTime_maxCurve.animationCurveValue.keys;
							updKeys[0].value = uvModule_frameOverTime_minCurve.animationCurveValue[0].value;
							updCurve.keys = updKeys;
							uvModule_frameOverTime_maxCurve.animationCurveValue = updCurve;
							EditorUtility.SetDirty(playgroundParticlesScriptReference.shurikenParticleSystem);
							shuriken.ApplyModifiedProperties();
							*/

						} else if (uv_minMaxState == MinMaxState.Curve || uv_minMaxState == MinMaxState.RandomBetweenTwoCurves) {
							uvModule_frameOverTime_maxCurve.animationCurveValue = prev_uvModule_frameOverTime_maxCurve;
							uvModule_frameOverTime_minCurve.animationCurveValue = prev_uvModule_frameOverTime_minCurve;
						}
					}
					
					GUI.enabled = true;
					EditorGUILayout.Separator();
					playgroundParticlesScriptReference.minShurikenLifetime = EditorGUILayout.FloatField (playgroundLanguage.minimumShurikenLifetime, playgroundParticlesScriptReference.minShurikenLifetime);
					if (playgroundParticlesScriptReference.minShurikenLifetime<0) playgroundParticlesScriptReference.minShurikenLifetime = 0;
					EditorGUI.indentLevel--;
					shuriken.ApplyModifiedProperties();
				}
				EditorGUILayout.EndVertical();
			}
			
			// Manipulators Settings
			if (GUILayout.Button(playgroundLanguage.manipulators+" ("+playgroundParticlesScriptReference.manipulators.Count+")", EditorStyles.toolbarDropDown)) playgroundSettings.manipulatorsFoldout=!playgroundSettings.manipulatorsFoldout;
			if (playgroundSettings.manipulatorsFoldout) {
				
				EditorGUILayout.Separator();
				
				if (playgroundParticlesScriptReference.manipulators.Count>0) {
					if (PlaygroundInspectorC.playgroundSettings==null)
						PlaygroundInspectorC.playgroundSettings = PlaygroundSettingsC.GetReference();
					if (PlaygroundInspectorC.playgroundLanguage==null)
						PlaygroundInspectorC.playgroundLanguage = PlaygroundSettingsC.GetLanguage();
					string mName;
					for (int i = 0; i<playgroundParticlesScriptReference.manipulators.Count; i++) {
						if (!playgroundParticlesScriptReference.manipulators[i].enabled)
							GUI.contentColor = Color.gray;
						if (playgroundParticlesScriptReference.manipulators[i].transform.available && playgroundParticlesScriptReference.manipulators[i].transform.transform!=null) {
							mName = playgroundParticlesScriptReference.manipulators[i].transform.transform.name;
							if (mName.Length>24)
								mName = mName.Substring(0, 24)+"...";
						} else {
							GUI.color = Color.red;
							mName = "("+playgroundLanguage.missingTransform+")";
						}
						EditorGUILayout.BeginVertical("box");
						
						EditorGUILayout.BeginHorizontal();
						
						GUILayout.Label(i.ToString(), EditorStyles.miniLabel, GUILayout.Width(18));
						playgroundParticlesScriptReference.manipulators[i].unfolded = GUILayout.Toggle(playgroundParticlesScriptReference.manipulators[i].unfolded, PlaygroundInspectorC.ManipulatorTypeName(playgroundParticlesScriptReference.manipulators[i].type), EditorStyles.foldout, GUILayout.Width(Screen.width/4));
						if (playgroundParticlesScriptReference.manipulators[i].transform.available && playgroundParticlesScriptReference.manipulators[i].transform.transform!=null) {
							if (GUILayout.Button(" ("+mName+")", EditorStyles.label)) {
								Selection.activeGameObject = playgroundParticlesScriptReference.manipulators[i].transform.transform.gameObject;
							}
						} else {
							GUILayout.Button(PlaygroundInspectorC.ManipulatorTypeName(playgroundParticlesScriptReference.manipulators[i].type)+" ("+playgroundLanguage.missingTransform+")", EditorStyles.label);
						}
						GUI.contentColor = Color.white;
						EditorGUILayout.Separator();
						GUI.enabled = (playgroundParticlesScriptReference.manipulators.Count>1);
						if(GUILayout.Button(playgroundLanguage.upSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							manipulators.MoveArrayElement(i, i==0?playgroundParticlesScriptReference.manipulators.Count-1:i-1);
						}
						if(GUILayout.Button(playgroundLanguage.downSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							manipulators.MoveArrayElement(i, i<playgroundParticlesScriptReference.manipulators.Count-1?i+1:0);
						}
						GUI.enabled = true;
						if(GUILayout.Button("+", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							playgroundParticlesScriptReference.manipulators.Add(playgroundParticlesScriptReference.manipulators[i].Clone());
						}
						if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							
							if (playgroundParticlesScriptReference.manipulators[i].transform.transform==null || EditorUtility.DisplayDialog(
								playgroundLanguage.remove+" "+PlaygroundInspectorC.ManipulatorTypeName(playgroundParticlesScriptReference.manipulators[i].type)+" "+playgroundLanguage.manipulator+" "+i+"?",
								playgroundLanguage.removeManipulator+mName+"? "+playgroundLanguage.gameObjectIntact, 
								playgroundLanguage.yes, playgroundLanguage.no)) {
								manipulators.DeleteArrayElementAtIndex(i);
								playgroundParticles.ApplyModifiedProperties();
								return;
							}
						}
						
						GUI.color = Color.white;
						
						EditorGUILayout.EndHorizontal();
						
						if (playgroundParticlesScriptReference.manipulators[i].unfolded && i<manipulators.arraySize) {
							PlaygroundInspectorC.RenderManipulatorSettings(playgroundParticlesScriptReference.manipulators[i], manipulators.GetArrayElementAtIndex(i), false);
						}
						
						GUI.enabled = true;
						EditorGUILayout.Separator();
						EditorGUILayout.EndVertical();
					}
					
				} else {
					EditorGUILayout.HelpBox(playgroundLanguage.noManipulators, MessageType.Info);
				}
				
				if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
					if (Selection.gameObjects.Length>0) {
						Transform mTrans = new GameObject().transform;
						mTrans.parent = playgroundParticlesScriptReference.particleSystemTransform;
						mTrans.localPosition = Vector3.up;
						if (playgroundParticlesScriptReference.manipulators.Count>0)
							mTrans.name = "Manipulator "+(playgroundParticlesScriptReference.manipulators.Count+1)+" ("+playgroundParticlesScriptReference.name+")";
						else mTrans.name = "Manipulator "+"("+playgroundParticlesScriptReference.name+")";
						PlaygroundC.ManipulatorObject(mTrans, playgroundParticlesScriptReference);
					} else {
						manipulators.InsertArrayElementAtIndex(manipulators.arraySize);
					}
					SceneView.RepaintAll();
				}
				
				EditorGUILayout.Separator();
			}
			
			// Event Settings
			if (GUILayout.Button(playgroundLanguage.events+" ("+playgroundParticlesScriptReference.events.Count+")", EditorStyles.toolbarDropDown)) playgroundSettings.eventsFoldout=!playgroundSettings.eventsFoldout;
			if (playgroundSettings.eventsFoldout) {
				
				EditorGUILayout.Separator();
				
				if (playgroundParticlesScriptReference.events.Count>0) {
					
					if (playgroundParticlesScriptReference.events.Count!=eventListFoldout.Count) {
						eventListFoldout = new List<bool>();
						eventListFoldout.AddRange(new bool[playgroundParticlesScriptReference.events.Count]);
					}
					
					string eName;
					for (int i = 0; i<playgroundParticlesScriptReference.events.Count; i++) {
						if (playgroundParticlesScriptReference.events[i].broadcastType!=EVENTBROADCASTC.EventListeners) {
							if (playgroundParticlesScriptReference.events[i].target!=null) {
								eName = playgroundParticlesScriptReference.events[i].target.name;
								if (eName.Length>24)
									eName = eName.Substring(0, 24)+"...";
							} else eName = "("+playgroundLanguage.noTarget+")";
						} else eName = "("+playgroundLanguage.eventListener+")";
						
						EditorGUILayout.BeginVertical("box");
						
						EditorGUILayout.BeginHorizontal();
						
						GUILayout.Label(i.ToString(), EditorStyles.miniLabel, GUILayout.Width(18));
						eventListFoldout[i] = GUILayout.Toggle(eventListFoldout[i], playgroundParticlesScriptReference.events[i].eventType.ToString(), EditorStyles.foldout, GUILayout.Width(Screen.width/4));
						if (playgroundParticlesScriptReference.events[i].target!=null) {
							if (GUILayout.Button(" ("+eName+")", EditorStyles.label)) {
								Selection.activeGameObject = playgroundParticlesScriptReference.events[i].target.gameObject;
							}
						} else {
							GUILayout.Button(eName, EditorStyles.label);
						}
						
						EditorGUILayout.Separator();
						GUI.enabled = (playgroundParticlesScriptReference.events.Count>1);
						if(GUILayout.Button(playgroundLanguage.upSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							events.MoveArrayElement(i, i==0?playgroundParticlesScriptReference.events.Count-1:i-1);
						}
						if(GUILayout.Button(playgroundLanguage.downSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							events.MoveArrayElement(i, i<playgroundParticlesScriptReference.events.Count-1?i+1:0);
						}
						GUI.enabled = true;
						if(GUILayout.Button("+", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							playgroundParticlesScriptReference.events.Add(playgroundParticlesScriptReference.events[i].Clone());
							eventListFoldout.Add(eventListFoldout[i]);
						}
						if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							
							if (playgroundParticlesScriptReference.events[i].target==null || EditorUtility.DisplayDialog(
								playgroundLanguage.remove+" "+playgroundParticlesScriptReference.events[i].eventType.ToString()+" "+playgroundLanguage.eventName+" "+i+"?",
								playgroundLanguage.removeEventInEventList, 
								playgroundLanguage.yes, playgroundLanguage.no)) {
								PlaygroundC.RemoveEvent (i, playgroundParticlesScriptReference);
								eventListFoldout.RemoveAt(i);
								playgroundParticles.ApplyModifiedProperties();
								return;
							}
						}
						
						EditorGUILayout.EndHorizontal();
						
						if (eventListFoldout[i] && i<events.arraySize) {
							RenderEventSettings(playgroundParticlesScriptReference.events[i], events.GetArrayElementAtIndex(i));
						}
						GUI.enabled = true;
						EditorGUILayout.Separator();
						EditorGUILayout.EndVertical();
					}
				} else {
					EditorGUILayout.HelpBox(playgroundLanguage.noEvents, MessageType.Info);
				}
				
				if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
					PlaygroundC.CreateEvent(playgroundParticlesScriptReference);
					eventListFoldout.Add(true);
				}
				
				EditorGUILayout.Separator();
			}
			
			// Snapshot Settings
			if (!playgroundParticlesScriptReference.isSnapshot) {
				if (GUILayout.Button(playgroundLanguage.snapshots+" ("+playgroundParticlesScriptReference.snapshots.Count+")", EditorStyles.toolbarDropDown)) playgroundSettings.saveLoadFoldout=!playgroundSettings.saveLoadFoldout;
				if (playgroundSettings.saveLoadFoldout) {
					
					EditorGUILayout.Separator();
					bool setThisLoadFrom = false;
					string loadModeButton = "";
					if (playgroundParticlesScriptReference.snapshots.Count>0 && isEditingInHierarchy) {
						GUILayout.BeginHorizontal();
						playgroundParticlesScriptReference.loadTransition = EditorGUILayout.ToggleLeft(playgroundLanguage.transitionTime, playgroundParticlesScriptReference.loadTransition, GUILayout.Width (Mathf.CeilToInt((Screen.width-140)/2)));
						GUI.enabled = playgroundParticlesScriptReference.loadTransition;
						playgroundParticlesScriptReference.loadTransitionTime = EditorGUILayout.Slider(playgroundParticlesScriptReference.loadTransitionTime, 0, playgroundSettings.maximumAllowedTransitionTime);
						GUILayout.EndHorizontal();
						EditorGUI.indentLevel++;
						playgroundParticlesScriptReference.loadTransitionType = (TRANSITIONTYPEC)EditorGUILayout.EnumPopup(playgroundLanguage.transitionType, playgroundParticlesScriptReference.loadTransitionType);
						EditorGUI.indentLevel--;
						GUI.enabled = true;
						EditorGUILayout.Separator();
						playgroundParticlesScriptReference.loadFromStart = EditorGUILayout.ToggleLeft(playgroundLanguage.loadFromStart, playgroundParticlesScriptReference.loadFromStart);
						
						for (int i = 0; i<playgroundParticlesScriptReference.snapshots.Count; i++) {
							setThisLoadFrom = false;
							GUILayout.BeginVertical (boxStyle);
							GUILayout.BeginHorizontal();
							
							if (playgroundParticlesScriptReference.loadFrom == i) {
								EditorGUILayout.Toggle (true, EditorStyles.radioButton, GUILayout.Width(14));
							} else
								setThisLoadFrom = EditorGUILayout.Toggle (setThisLoadFrom, EditorStyles.radioButton, GUILayout.Width(14));
							if (setThisLoadFrom)
								playgroundParticlesScriptReference.loadFrom = i;
							GUILayout.Label(i.ToString(), EditorStyles.miniLabel, GUILayout.Width(18));
							if (PlaygroundC.reference.showSnapshotsInHierarchy) {
								playgroundParticlesScriptReference.snapshots[i].unfolded = GUILayout.Toggle(playgroundParticlesScriptReference.snapshots[i].unfolded, "", EditorStyles.foldout);
								playgroundParticlesScriptReference.snapshots[i].settings.name = EditorGUILayout.TextField(playgroundParticlesScriptReference.snapshots[i].settings.name, EditorStyles.toolbarTextField);
								playgroundParticlesScriptReference.snapshots[i].name = playgroundParticlesScriptReference.snapshots[i].settings.name;
							} else {
								if (GUILayout.Button(playgroundParticlesScriptReference.snapshots[i].settings.name, EditorStyles.label, GUILayout.MinWidth (100)))
									playgroundParticlesScriptReference.loadFrom = i;
							}
							
							playgroundParticlesScriptReference.snapshots[i].loadTransform = EditorGUILayout.ToggleLeft (playgroundLanguage.transform, playgroundParticlesScriptReference.snapshots[i].loadTransform, EditorStyles.miniButton, GUILayout.MaxWidth(playgroundLanguage.transform.Length*10));
							playgroundParticlesScriptReference.snapshots[i].loadMaterial = EditorGUILayout.ToggleLeft (playgroundLanguage.material, playgroundParticlesScriptReference.snapshots[i].loadMaterial, EditorStyles.miniButton, GUILayout.MaxWidth(playgroundLanguage.material.Length*10));
							switch (playgroundParticlesScriptReference.snapshots[i].loadMode) {
							case 0: loadModeButton = playgroundLanguage.settingsAndParticles; break;
							case 1: loadModeButton = playgroundLanguage.settingsOnly; break;
							case 2: loadModeButton = playgroundLanguage.particlesOnly; break;
							default: loadModeButton = playgroundLanguage.settingsAndParticles; break;
							}
							if (GUILayout.Button(loadModeButton, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
								playgroundParticlesScriptReference.snapshots[i].loadMode++;
								playgroundParticlesScriptReference.snapshots[i].loadMode = playgroundParticlesScriptReference.snapshots[i].loadMode%3;
							}
							GUI.enabled = (playgroundParticlesScriptReference.snapshots.Count>1);
							if(GUILayout.Button(playgroundLanguage.upSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								snapshots.MoveArrayElement(i, i==0?playgroundParticlesScriptReference.snapshots.Count-1:i-1);
							}
							if(GUILayout.Button(playgroundLanguage.downSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								snapshots.MoveArrayElement(i, i<playgroundParticlesScriptReference.snapshots.Count-1?i+1:0);
							}
							GUI.enabled = true;
							if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								if (EditorUtility.DisplayDialog(
									playgroundLanguage.remove+" "+playgroundParticlesScriptReference.snapshots[i].name+"?",
									playgroundLanguage.removeSnapshot+" "+playgroundParticlesScriptReference.snapshots[i].name+" ("+i.ToString()+")?", 
									playgroundLanguage.yes, playgroundLanguage.no)) {
									DestroyImmediate (playgroundParticlesScriptReference.snapshots[i].settings.gameObject);
									playgroundParticlesScriptReference.snapshots.RemoveAt(i);
									if (playgroundParticlesScriptReference.loadFrom>=playgroundParticlesScriptReference.snapshots.Count)
										playgroundParticlesScriptReference.loadFrom = playgroundParticlesScriptReference.snapshots.Count-1;
									return;
								}
							}
							GUILayout.EndHorizontal();
							if (PlaygroundC.reference.showSnapshotsInHierarchy && playgroundParticlesScriptReference.snapshots[i].unfolded) {
								EditorGUILayout.Separator();
								EditorGUI.indentLevel+=3;
								PlaygroundParticlesC currentSnapshot = playgroundParticlesScriptReference.snapshots[i].settings;
								playgroundParticlesScriptReference.snapshots[i].settings = (PlaygroundParticlesC)EditorGUILayout.ObjectField(playgroundLanguage.snapshot, playgroundParticlesScriptReference.snapshots[i].settings, typeof(PlaygroundParticlesC), true);
								if (playgroundParticlesScriptReference.snapshots[i].settings!=currentSnapshot) {
									if (!playgroundParticlesScriptReference.snapshots[i].settings.isSnapshot) {
										EditorUtility.DisplayDialog(
											playgroundParticlesScriptReference.snapshots[i].settings.name+" "+playgroundLanguage.notSnapshot,
											playgroundLanguage.notSnapshotText,
											playgroundLanguage.ok
											);
										playgroundParticlesScriptReference.snapshots[i].settings = currentSnapshot;
										continue;
									}
								}
								GUI.enabled = playgroundParticlesScriptReference.loadTransition;
								playgroundParticlesScriptReference.snapshots[i].transitionType = (INDIVIDUALTRANSITIONTYPEC)EditorGUILayout.EnumPopup(playgroundLanguage.transitionType, playgroundParticlesScriptReference.snapshots[i].transitionType);
								playgroundParticlesScriptReference.snapshots[i].transitionMultiplier = EditorGUILayout.FloatField(playgroundLanguage.transitionTimeMultiplier, playgroundParticlesScriptReference.snapshots[i].transitionMultiplier);
								playgroundParticlesScriptReference.snapshots[i].setMaterialAfterTransition = EditorGUILayout.Toggle(playgroundLanguage.setMaterialAfterTransition, playgroundParticlesScriptReference.snapshots[i].setMaterialAfterTransition);
								GUI.enabled = true;
								EditorGUI.indentLevel-=3;
								EditorGUILayout.Separator();
							}
							GUILayout.EndVertical ();
						}
						EditorGUILayout.Separator();
					} else {
						if (playgroundParticlesScriptReference.snapshots.Count>0)
							EditorGUILayout.HelpBox(playgroundLanguage.editFromHierarchyOnly, MessageType.Info);
						else
							EditorGUILayout.HelpBox(playgroundLanguage.noSnapshots, MessageType.Info);
					}
					if (isEditingInHierarchy) {
						GUILayout.BeginHorizontal();
						if(GUILayout.Button(playgroundLanguage.save, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
							saveName = playgroundLanguage.newSnapshotName+" "+(playgroundParticlesScriptReference.snapshots.Count+1).ToString();
							playgroundParticlesScriptReference.SaveAsynchronous(saveName);
							if (playgroundParticlesScriptReference.loadFrom>=playgroundParticlesScriptReference.snapshots.Count && playgroundParticlesScriptReference.snapshots.Count>0)
								playgroundParticlesScriptReference.loadFrom = playgroundParticlesScriptReference.snapshots.Count-1;
						}
						GUI.enabled = (playgroundParticlesScriptReference.snapshots.Count>0);
						if(GUILayout.Button(playgroundLanguage.load, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
							playgroundParticlesScriptReference.Load(playgroundParticlesScriptReference.loadFrom);
						}
						GUILayout.FlexibleSpace();
						if (playgroundScriptReference!=null) {
							if(GUILayout.Button(playgroundScriptReference.showSnapshotsInHierarchy?playgroundLanguage.simple:playgroundLanguage.advanced, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
								playgroundScriptReference.showSnapshotsInHierarchy = !playgroundScriptReference.showSnapshotsInHierarchy;
								PlaygroundInspectorC.UpdateSnapshots();
							}
						}
						if(GUILayout.Button(playgroundLanguage.removeAll, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
							if (EditorUtility.DisplayDialog(
								playgroundLanguage.removeAllSnapshots,
								playgroundLanguage.removeAllSnapshotsText, 
								playgroundLanguage.yes, playgroundLanguage.no)) {
								for (int s = 0; s<playgroundParticlesScriptReference.snapshots.Count; s++) {
									DestroyImmediate (playgroundParticlesScriptReference.snapshots[s].settings.gameObject);
								}
								playgroundParticlesScriptReference.snapshots.Clear ();
								return;
							}
						}
						GUI.enabled = true;
						GUILayout.EndHorizontal();
					}
					EditorGUILayout.Separator();
				}
			}
			
			// Advanced Settings
			string localSimulationSpaceName = playgroundParticlesScriptReference.GetComponent<ParticleSystem>().simulationSpace==ParticleSystemSimulationSpace.Local?playgroundLanguage.localSpace:playgroundLanguage.globalSpace;
			if (GUILayout.Button(playgroundLanguage.advanced+" ("+localSimulationSpaceName+")", EditorStyles.toolbarDropDown)) playgroundSettings.advancedFoldout=!playgroundSettings.advancedFoldout;
			if (playgroundSettings.advancedFoldout) {
				
				// Simulation space
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.advancedSimulationFoldout = GUILayout.Toggle(playgroundSettings.advancedSimulationFoldout, playgroundLanguage.simulationSpace, EditorStyles.foldout);
				GUILayout.Label (playgroundParticlesScriptReference.GetComponent<ParticleSystem>().simulationSpace.ToString(), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.advancedSimulationFoldout) {
					GUI.enabled = (playgroundParticlesScriptReference.source!=SOURCEC.Projection);
					playgroundParticlesScriptReference.GetComponent<ParticleSystem>().simulationSpace = (ParticleSystemSimulationSpace)EditorGUILayout.EnumPopup(playgroundLanguage.simulationSpace, playgroundParticlesScriptReference.GetComponent<ParticleSystem>().simulationSpace);
					GUI.enabled = true;
					if (playgroundParticlesScriptReference.GetComponent<ParticleSystem>().simulationSpace==ParticleSystemSimulationSpace.Local && playgroundParticlesScriptReference.source!=SOURCEC.Projection) {
						
						playgroundParticlesScriptReference.applyLocalSpaceMovementCompensation = EditorGUILayout.ToggleLeft (playgroundLanguage.movementCompensation, playgroundParticlesScriptReference.applyLocalSpaceMovementCompensation);
						GUI.enabled = playgroundParticlesScriptReference.applyLocalSpaceMovementCompensation;
						GUILayout.BeginHorizontal();
						EditorGUI.indentLevel++;
						playgroundParticlesScriptReference.applyMovementCompensationLifetimeStrength = EditorGUILayout.ToggleLeft (playgroundLanguage.movementCompensationLifetimeStrength, playgroundParticlesScriptReference.applyMovementCompensationLifetimeStrength, GUILayout.MaxWidth(Mathf.CeilToInt(EditorGUIUtility.labelWidth)-20));
						GUI.enabled = playgroundParticlesScriptReference.applyLocalSpaceMovementCompensation && playgroundParticlesScriptReference.applyMovementCompensationLifetimeStrength;
						movementCompensationLifetimeStrength.animationCurveValue = EditorGUILayout.CurveField(movementCompensationLifetimeStrength.animationCurveValue);
						GUILayout.EndHorizontal();
						GUI.enabled = true;
						EditorGUI.indentLevel--;
						if (playgroundParticlesScriptReference.applyLocalSpaceMovementCompensation && playgroundParticlesScriptReference.source==SOURCEC.Transform && playgroundParticlesScriptReference.sourceTransforms!=null && playgroundParticlesScriptReference.sourceTransforms[0].transform!=null && playgroundParticlesScriptReference.particleSystemTransform==playgroundParticlesScriptReference.sourceTransforms[0].transform)
							EditorGUILayout.HelpBox(playgroundLanguage.useAnotherSourceTransform, MessageType.Warning);
						
						
					} else if (playgroundParticlesScriptReference.source==SOURCEC.Projection) {
						EditorGUILayout.HelpBox(playgroundLanguage.projectionOnlyWorldSpace, MessageType.Info);
					}
				}
				GUILayout.EndVertical();
				
				EditorGUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.advancedTimeFoldout = GUILayout.Toggle(playgroundSettings.advancedTimeFoldout, playgroundLanguage.time, EditorStyles.foldout);
				GUILayout.Label (playgroundParticlesScriptReference.particleTimescale.ToString("F1"), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.advancedTimeFoldout) {
					
					// Update rate
					updateRate.intValue = EditorGUILayout.IntSlider(playgroundLanguage.updateRate, updateRate.intValue, playgroundSettings.minimumAllowedUpdateRate, 1);
					
					// Time scale
					playgroundParticlesScriptReference.particleTimescale = EditorGUILayout.Slider (playgroundLanguage.timeScale, playgroundParticlesScriptReference.particleTimescale, 0, playgroundSettings.maximumAllowedTimescale);
				}
				EditorGUILayout.EndVertical();
				
				
				// Rebirth Options
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				bool hasRebirthOptions = 
					playgroundParticlesScriptReference.applyRandomLifetimeOnRebirth||
						playgroundParticlesScriptReference.applyRandomSizeOnRebirth||
						playgroundParticlesScriptReference.applyRandomRotationOnRebirth||
						playgroundParticlesScriptReference.applyRandomScatterOnRebirth||
						playgroundParticlesScriptReference.applyRandomInitialVelocityOnRebirth||
						playgroundParticlesScriptReference.applyInitialColorOnRebirth||
						playgroundParticlesScriptReference.applyDeltaOnRebirth||
						playgroundParticlesScriptReference.calculateManipulatorOnRebirth;
				playgroundSettings.advancedRebirthOptionsFoldout = GUILayout.Toggle(playgroundSettings.advancedRebirthOptionsFoldout, playgroundLanguage.rebirthOptions, EditorStyles.foldout);
				GUILayout.Label (hasRebirthOptions?playgroundLanguage.on:playgroundLanguage.off, EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.advancedRebirthOptionsFoldout) {
					playgroundParticlesScriptReference.applyRandomLifetimeOnRebirth = EditorGUILayout.Toggle (playgroundLanguage.randomLifetime, playgroundParticlesScriptReference.applyRandomLifetimeOnRebirth);
					playgroundParticlesScriptReference.applyRandomSizeOnRebirth = EditorGUILayout.Toggle (playgroundLanguage.randomSize, playgroundParticlesScriptReference.applyRandomSizeOnRebirth);
					playgroundParticlesScriptReference.applyRandomRotationOnRebirth = EditorGUILayout.Toggle (playgroundLanguage.randomRotation, playgroundParticlesScriptReference.applyRandomRotationOnRebirth);
					playgroundParticlesScriptReference.applyRandomScatterOnRebirth = EditorGUILayout.Toggle (playgroundLanguage.randomScatter, playgroundParticlesScriptReference.applyRandomScatterOnRebirth);
					playgroundParticlesScriptReference.applyRandomInitialVelocityOnRebirth = EditorGUILayout.Toggle (playgroundLanguage.randomVelocity, playgroundParticlesScriptReference.applyRandomInitialVelocityOnRebirth);
					playgroundParticlesScriptReference.applyInitialColorOnRebirth = EditorGUILayout.Toggle (playgroundLanguage.forceInitialColor, playgroundParticlesScriptReference.applyInitialColorOnRebirth);
					playgroundParticlesScriptReference.applyDeltaOnRebirth = EditorGUILayout.Toggle (playgroundLanguage.deltaPositionAdjustment, playgroundParticlesScriptReference.applyDeltaOnRebirth);
					playgroundParticlesScriptReference.calculateManipulatorOnRebirth = EditorGUILayout.Toggle (playgroundLanguage.calculateManipulator, playgroundParticlesScriptReference.calculateManipulatorOnRebirth);
				}
				GUILayout.EndVertical();
				
				// Locks
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.advancedLocksFoldout = GUILayout.Toggle(playgroundSettings.advancedLocksFoldout, playgroundLanguage.locks, EditorStyles.foldout);
				GUILayout.Label ((playgroundParticlesScriptReference.applyLockPosition||playgroundParticlesScriptReference.applyLockRotation||playgroundParticlesScriptReference.applyLockScale?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.advancedLocksFoldout) {
					playgroundParticlesScriptReference.applyLockPosition = EditorGUILayout.ToggleLeft(playgroundLanguage.lockPosition, playgroundParticlesScriptReference.applyLockPosition);
					GUI.enabled = playgroundParticlesScriptReference.applyLockPosition;
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.lockPosition = EditorGUILayout.Vector3Field (playgroundLanguage.position, playgroundParticlesScriptReference.lockPosition);
					playgroundParticlesScriptReference.lockPositionIsLocal = EditorGUILayout.Toggle(playgroundLanguage.positionIsLocal, playgroundParticlesScriptReference.lockPositionIsLocal);
					EditorGUI.indentLevel--;
					GUI.enabled = true;
					EditorGUILayout.Separator();
					playgroundParticlesScriptReference.applyLockRotation = EditorGUILayout.ToggleLeft(playgroundLanguage.lockRotation, playgroundParticlesScriptReference.applyLockRotation);
					GUI.enabled = playgroundParticlesScriptReference.applyLockRotation;
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.lockRotation = EditorGUILayout.Vector3Field (playgroundLanguage.rotation, playgroundParticlesScriptReference.lockRotation);
					playgroundParticlesScriptReference.lockRotationIsLocal = EditorGUILayout.Toggle(playgroundLanguage.rotationIsLocal, playgroundParticlesScriptReference.lockRotationIsLocal);
					EditorGUI.indentLevel--;
					GUI.enabled = true;
					EditorGUILayout.Separator();
					playgroundParticlesScriptReference.applyLockScale = EditorGUILayout.ToggleLeft(playgroundLanguage.lockScale, playgroundParticlesScriptReference.applyLockScale);
					GUI.enabled = playgroundParticlesScriptReference.applyLockScale;
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.lockScale = EditorGUILayout.Vector3Field (playgroundLanguage.scale, playgroundParticlesScriptReference.lockScale);
					EditorGUI.indentLevel--;
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
				
				// On Enable (Prewarm)
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.advancedOnEnableFoldout = GUILayout.Toggle(playgroundSettings.advancedOnEnableFoldout, playgroundLanguage.onEnable, EditorStyles.foldout);
				GUILayout.Label (playgroundLanguage.prewarm+" "+(playgroundParticlesScriptReference.prewarm?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.advancedOnEnableFoldout) {
					bool previousPrewarm = playgroundParticlesScriptReference.prewarm;
					float previousPrewarmTime = playgroundParticlesScriptReference.prewarmTime;
					int previousPrewarmCycles = playgroundParticlesScriptReference.prewarmCycles;
					playgroundParticlesScriptReference.prewarm = EditorGUILayout.ToggleLeft (playgroundLanguage.prewarm, playgroundParticlesScriptReference.prewarm);
					GUI.enabled = playgroundParticlesScriptReference.prewarm;
					EditorGUI.indentLevel++;
					playgroundParticlesScriptReference.prewarmTime = EditorGUILayout.Slider (playgroundLanguage.prewarmLifetimeCycles, playgroundParticlesScriptReference.prewarmTime, 0, 2f);
					playgroundParticlesScriptReference.prewarmCycles = EditorGUILayout.IntField (playgroundLanguage.prewarmCyclesResolution, playgroundParticlesScriptReference.prewarmCycles);
					playgroundParticlesScriptReference.prewarmCycles = Mathf.Clamp (playgroundParticlesScriptReference.prewarmCycles, 8, playgroundSettings.maximumAllowedPrewarmCycles);
					EditorGUI.indentLevel--;
					if (previousPrewarm != playgroundParticlesScriptReference.prewarm || previousPrewarmTime != playgroundParticlesScriptReference.prewarmTime || previousPrewarmCycles != playgroundParticlesScriptReference.prewarmCycles)
						LifetimeSorting();
					GUI.enabled = true;
				}
				GUILayout.EndVertical();
				
				// Auto Pause
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.advancedAutoPauseFoldout = GUILayout.Toggle(playgroundSettings.advancedAutoPauseFoldout, playgroundLanguage.outOfView, EditorStyles.foldout);
				GUILayout.Label (playgroundLanguage.autoPause+" "+(playgroundParticlesScriptReference.pauseCalculationWhenInvisible?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.advancedAutoPauseFoldout) {
					playgroundParticlesScriptReference.pauseCalculationWhenInvisible = EditorGUILayout.ToggleLeft (playgroundLanguage.autoPauseCalculation, playgroundParticlesScriptReference.pauseCalculationWhenInvisible);
					EditorGUI.indentLevel++;
					GUI.enabled = playgroundParticlesScriptReference.pauseCalculationWhenInvisible;
					playgroundParticlesScriptReference.calculationTriggerTransform = (Transform)EditorGUILayout.ObjectField(playgroundLanguage.calculationTrigger, playgroundParticlesScriptReference.calculationTriggerTransform, typeof(Transform), true);
					playgroundParticlesScriptReference.calculationTriggerSize = EditorGUILayout.Vector3Field(playgroundLanguage.calculationTriggerSize, playgroundParticlesScriptReference.calculationTriggerSize);
					playgroundParticlesScriptReference.calculationTriggerOffset = EditorGUILayout.Vector3Field(playgroundLanguage.calculationTriggerOffset, playgroundParticlesScriptReference.calculationTriggerOffset);
					playgroundParticlesScriptReference.calculationTriggerSizeGizmo = EditorGUILayout.Toggle(playgroundLanguage.drawGizmo, playgroundParticlesScriptReference.calculationTriggerSizeGizmo);
					GUI.enabled = true;
					EditorGUI.indentLevel--;
				}
				EditorGUILayout.EndVertical();
				
				// Misc
				GUILayout.BeginVertical(boxStyle);
				GUILayout.BeginHorizontal();
				playgroundSettings.advancedMiscFoldout = GUILayout.Toggle(playgroundSettings.advancedMiscFoldout, playgroundLanguage.misc, EditorStyles.foldout);
				GUILayout.Label (playgroundLanguage.sync+" "+(playgroundParticlesScriptReference.syncPositionsOnMainThread?playgroundLanguage.on:playgroundLanguage.off), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
				GUILayout.EndHorizontal();
				if (playgroundSettings.advancedMiscFoldout) {
					playgroundParticlesScriptReference.syncPositionsOnMainThread = EditorGUILayout.Toggle (playgroundLanguage.syncParticlesToMainThread, playgroundParticlesScriptReference.syncPositionsOnMainThread);
					EditorGUILayout.PropertyField(threadMethod, new GUIContent(playgroundLanguage.particleThreadMethod, playgroundLanguage.threadMethodDescription));
					playgroundParticlesScriptReference.multithreadedStartup = EditorGUILayout.Toggle (playgroundLanguage.multithreadedStartup, playgroundParticlesScriptReference.multithreadedStartup);

					EditorGUILayout.Separator();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel(playgroundLanguage.particlePool);
					
					// Clear
					if(GUILayout.Button(playgroundLanguage.clear, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)) && isEditingInHierarchy){
						PlaygroundParticlesC.Clear(playgroundParticlesScriptReference);
					}
					
					// Rebuild
					if(GUILayout.Button(playgroundLanguage.rebuild, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)) && isEditingInHierarchy){
						PlaygroundParticlesC.SetParticleCount(playgroundParticlesScriptReference, playgroundParticlesScriptReference.particleCount);
						playgroundParticlesScriptReference.Start();
					}
					GUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
			}
			
			EditorGUILayout.EndVertical();
			
			previousSource = playgroundParticlesScriptReference.source;
			if (playgroundParticles.ApplyModifiedProperties())
				playgroundParticlesScriptReference.IsDoneThread = true;
		}
		
		EditorGUILayout.EndVertical();
		
		// Playground Manager - Particle Systems, Manipulators
		PlaygroundInspectorC.RenderPlaygroundSettings();
		
		// Wireframes in Scene View
		if (isEditingInHierarchy)
			if (currentWireframe!=PlaygroundC.reference.drawWireframe)
				SetWireframeVisibility();
	}
	
	public void ProgressBar (float val, string label, float width) {
		Rect rect = GUILayoutUtility.GetRect (18, 18, "TextField");
		rect.width = width;
		rect.height = 16;
		if (val<0) val = 0;
		EditorGUI.ProgressBar (rect, val, label);
		EditorGUILayout.Space ();
	}
	
	bool triedToAssignSelfTarget = false;
	bool triedToAssignSnapshot = false;
	public void RenderEventSettings (PlaygroundEventC thisEvent, SerializedProperty serializedEvent) {
		thisEvent.enabled = EditorGUILayout.ToggleLeft(playgroundLanguage.enabled, thisEvent.enabled);
		GUI.enabled = thisEvent.enabled;
		
		// Event Broadcast Type
		EditorGUILayout.PropertyField(serializedEvent.FindPropertyRelative("broadcastType"), new GUIContent(playgroundLanguage.broadcastType, playgroundLanguage.broadcastTypeDescription));
		
		// Target
		if (thisEvent.broadcastType!=EVENTBROADCASTC.EventListeners) {
			PlaygroundParticlesC currentTarget = thisEvent.target;
			thisEvent.target = EditorGUILayout.ObjectField(playgroundLanguage.target, thisEvent.target, typeof(PlaygroundParticlesC), true) as PlaygroundParticlesC;
			if (currentTarget!=thisEvent.target && thisEvent.target!=null) {
				
				// Assign new target
				if (thisEvent.target == playgroundParticlesScriptReference) {
					thisEvent.target = null;
					triedToAssignSelfTarget = true;
					triedToAssignSnapshot = false;
				} else if (thisEvent.target.isSnapshot) {
					thisEvent.target = null;
					triedToAssignSnapshot = true;
					triedToAssignSelfTarget = false;
				} else {
					triedToAssignSelfTarget = false;
					triedToAssignSnapshot = false;
					if (thisEvent.target.source!=SOURCEC.Script && EditorUtility.DisplayDialog(playgroundLanguage.switchToScriptMode, playgroundLanguage.switchToScriptModeText1+thisEvent.target.name+" "+playgroundLanguage.switchToScriptModeText2+" "+thisEvent.target.source.ToString()+" "+playgroundLanguage.switchToScriptModeText3, playgroundLanguage.switchText, playgroundLanguage.cancel))
						thisEvent.target.source = SOURCEC.Script;
				}
			}
			if (triedToAssignSelfTarget)
				EditorGUILayout.HelpBox(playgroundLanguage.particleSystemEventAssignErrorSelf, MessageType.Warning);
			else if (triedToAssignSnapshot)
				EditorGUILayout.HelpBox(playgroundLanguage.particleSystemEventAssignErrorSnapshot, MessageType.Warning);
		}
		
		if (thisEvent.broadcastType!=EVENTBROADCASTC.Target) {
			thisEvent.sendToManager = EditorGUILayout.Toggle(playgroundLanguage.sendToManager, thisEvent.sendToManager);
		}
		
		EditorGUILayout.Separator();
		
		// Type
		EditorGUILayout.PropertyField(serializedEvent.FindPropertyRelative("eventType"), new GUIContent(playgroundLanguage.type, playgroundLanguage.typeOfEvent));
		
		// Type: Collision
		if (thisEvent.eventType==EVENTTYPEC.Collision) {
			if (!playgroundParticlesScriptReference.collision)
				EditorGUILayout.HelpBox(playgroundLanguage.enableCollisionToSendEvents, MessageType.Info);
			thisEvent.collisionThreshold = EditorGUILayout.FloatField (playgroundLanguage.collisionThreshold, thisEvent.collisionThreshold);
		}
		
		// Type: Time
		if (thisEvent.eventType == EVENTTYPEC.Time)
			thisEvent.eventTime = EditorGUILayout.FloatField (playgroundLanguage.time, thisEvent.eventTime);
		
		EditorGUILayout.Separator();
		
		// Settings with inheritance options
		EditorGUILayout.PropertyField(serializedEvent.FindPropertyRelative("eventInheritancePosition"), new GUIContent(playgroundLanguage.position, playgroundLanguage.inheritancePosition));
		if (thisEvent.eventInheritancePosition == EVENTINHERITANCEC.User) {
			EditorGUI.indentLevel++;
			thisEvent.eventPosition = EditorGUILayout.Vector3Field (" ", thisEvent.eventPosition);
			EditorGUI.indentLevel--;
		}
		
		EditorGUILayout.Separator();
		
		EditorGUILayout.PropertyField(serializedEvent.FindPropertyRelative("eventInheritanceVelocity"), new GUIContent(playgroundLanguage.velocity, playgroundLanguage.inheritanceVelocity));
		if (thisEvent.eventInheritanceVelocity == EVENTINHERITANCEC.User)
			thisEvent.eventVelocity = EditorGUILayout.Vector3Field (" ", thisEvent.eventVelocity);
		thisEvent.velocityMultiplier = EditorGUILayout.FloatField(playgroundLanguage.velocityMultiplier, thisEvent.velocityMultiplier);
		
		EditorGUILayout.Separator();
		
		EditorGUILayout.PropertyField(serializedEvent.FindPropertyRelative("eventInheritanceColor"), new GUIContent(playgroundLanguage.color, playgroundLanguage.inheritanceColor));
		if (thisEvent.eventInheritanceColor == EVENTINHERITANCEC.User) {
			EditorGUI.indentLevel++;
			thisEvent.eventColor = EditorGUILayout.ColorField(" ", thisEvent.eventColor);
			EditorGUI.indentLevel--;
		}
		
		GUI.enabled = true;
	}
	
	public void RenderStateSettings () {
		
		GUI.enabled = (states.arraySize>0);
		activeState.intValue = EditorGUILayout.IntSlider(playgroundLanguage.activeState, activeState.intValue, 0, states.arraySize-1);
		GUI.enabled = true;
		
		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginVertical(boxStyle);
		GUILayout.BeginHorizontal();
		playgroundSettings.statesFoldout = GUILayout.Toggle(playgroundSettings.statesFoldout, playgroundLanguage.states, EditorStyles.foldout);
		GUILayout.Label (states.arraySize.ToString(), EditorStyles.miniLabel, GUILayout.ExpandWidth(false));
		GUILayout.EndHorizontal();
		if (playgroundSettings.statesFoldout) {
			if (states.arraySize>0) {
				SerializedProperty thisState;
				SerializedProperty thisName;
				SerializedProperty thisPoints;
				SerializedProperty thisTexture;
				SerializedProperty thisMesh;
				SerializedProperty thisDepthmap;
				SerializedProperty thisDepthmapStrength;
				SerializedProperty thisTransform;
				SerializedProperty thisStateScale;
				SerializedProperty thisStateOffset;
				SerializedProperty thisStateScaleMethod;
				
				for (int i = 0; i<states.arraySize; i++) {
					thisState = states.GetArrayElementAtIndex(i);
					
					GUILayout.BeginVertical(boxStyle);
					GUILayout.BeginHorizontal(GUILayout.MinHeight(20));
					
					// State title with foldout
					if (playgroundParticlesScriptReference.activeState==i) GUILayout.BeginHorizontal(boxStyle);
					
					GUI.enabled = (playgroundParticlesScriptReference.states.Count>1);
					if (GUILayout.Button(i.ToString(), EditorStyles.toolbarButton, GUILayout.Width(20))) playgroundParticlesScriptReference.activeState=i;
					GUI.enabled = true;
					
					statesListFoldout[i] = GUILayout.Toggle(statesListFoldout[i], playgroundParticlesScriptReference.states[i].stateName, EditorStyles.foldout);
					
					EditorGUILayout.Separator();
					GUI.enabled = (playgroundParticlesScriptReference.states.Count>1);
					if(GUILayout.Button(playgroundLanguage.upSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						int moveUp = i==0?playgroundParticlesScriptReference.states.Count-1:i-1;
						if (playgroundParticlesScriptReference.activeState==i) playgroundParticlesScriptReference.activeState = moveUp;
						playgroundParticlesScriptReference.previousActiveState = playgroundParticlesScriptReference.activeState;
						states.MoveArrayElement(i, moveUp);
						playgroundParticles.ApplyModifiedProperties();
						
						playgroundParticlesScriptReference.states[i].Initialize();
						playgroundParticlesScriptReference.states[moveUp].Initialize();
						
					}
					if(GUILayout.Button(playgroundLanguage.downSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						int moveDown = i<playgroundParticlesScriptReference.states.Count-1?i+1:0;
						if (playgroundParticlesScriptReference.activeState==i) playgroundParticlesScriptReference.activeState = moveDown;
						playgroundParticlesScriptReference.previousActiveState = playgroundParticlesScriptReference.activeState;
						states.MoveArrayElement(i, moveDown);
						playgroundParticles.ApplyModifiedProperties();
						
						playgroundParticlesScriptReference.states[i].Initialize();
						playgroundParticlesScriptReference.states[moveDown].Initialize();
					}
					GUI.enabled = true;
					if(GUILayout.Button("+", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						PlaygroundC.Add(playgroundParticlesScriptReference, playgroundParticlesScriptReference.states[i].Clone());
						statesListFoldout.Add(statesListFoldout[i]);
						if (!playgroundParticlesScriptReference.states[playgroundParticlesScriptReference.states.Count-1].stateName.Contains("(Clone)"))
							playgroundParticlesScriptReference.states[playgroundParticlesScriptReference.states.Count-1].stateName = playgroundParticlesScriptReference.states[playgroundParticlesScriptReference.states.Count-1].stateName+" (Clone)";
					}
					if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						if (EditorUtility.DisplayDialog(
							playgroundLanguage.remove+" "+playgroundParticlesScriptReference.states[i].stateName+"?",
							playgroundLanguage.removeState+" "+playgroundParticlesScriptReference.states[i].stateName+" ("+i.ToString()+")?", 
							playgroundLanguage.yes, playgroundLanguage.no)) {
							RemoveState(i);
							statesListFoldout.RemoveAt(i);
							playgroundParticles.ApplyModifiedProperties();
							return;
						}
					}
					if (playgroundParticlesScriptReference.activeState==i) GUILayout.EndHorizontal();
					GUILayout.EndHorizontal();
					
					if (statesListFoldout[i]) {
						
						if (i<states.arraySize) {
							
							EditorGUILayout.Separator();
							
							thisName = thisState.FindPropertyRelative("stateName");
							EditorGUILayout.PropertyField(thisName, new GUIContent(playgroundLanguage.nameText));
							
							thisMesh = thisState.FindPropertyRelative("stateMesh");
							EditorGUILayout.PropertyField(thisMesh, new GUIContent(playgroundLanguage.mesh, playgroundLanguage.stateMeshDescription));
							
							thisTexture = thisState.FindPropertyRelative("stateTexture");
							EditorGUILayout.PropertyField(thisTexture, new GUIContent(playgroundLanguage.texture, playgroundLanguage.stateTextureDescription));
							
							thisDepthmap = thisState.FindPropertyRelative("stateDepthmap");
							EditorGUILayout.PropertyField(thisDepthmap, new GUIContent(playgroundLanguage.depthmap, playgroundLanguage.stateDepthmapDescription));
							if (thisDepthmap.objectReferenceValue!=null) {
								thisDepthmapStrength = thisState.FindPropertyRelative("stateDepthmapStrength");
								float currentDS = thisDepthmapStrength.floatValue;
								EditorGUILayout.PropertyField(thisDepthmapStrength, new GUIContent(playgroundLanguage.depthmapStrength, playgroundLanguage.stateDepthmapStrengthDescription));
								if (currentDS!=thisDepthmapStrength.floatValue)
									playgroundParticlesScriptReference.states[i].Initialize();
							}
							
							thisTransform = thisState.FindPropertyRelative("stateTransform");
							EditorGUILayout.PropertyField(thisTransform, new GUIContent(playgroundLanguage.transform, playgroundLanguage.stateTransformDescription));
							
							thisStateScale = thisState.FindPropertyRelative("stateScale");
							EditorGUILayout.PropertyField(thisStateScale, new GUIContent(playgroundLanguage.scale, playgroundLanguage.stateScaleDescription));

							thisStateScaleMethod = thisState.FindPropertyRelative("stateScaleMethod");
							EditorGUILayout.PropertyField(thisStateScaleMethod, new GUIContent(playgroundLanguage.scaleMethod));
							
							thisStateOffset = thisState.FindPropertyRelative("stateOffset");
							EditorGUILayout.PropertyField(thisStateOffset, new GUIContent(playgroundLanguage.offset, playgroundLanguage.stateOffsetDescription));

							
							if (playgroundParticlesScriptReference.states[i].stateMesh==null) {
								GUILayout.BeginHorizontal();
								bool currentApplyChromaKey = playgroundParticlesScriptReference.states[i].applyChromaKey;
								playgroundParticlesScriptReference.states[i].applyChromaKey = EditorGUILayout.Toggle (playgroundLanguage.chromaKey, playgroundParticlesScriptReference.states[i].applyChromaKey);
								GUI.enabled = playgroundParticlesScriptReference.states[i].applyChromaKey;
								EditorGUIUtility.labelWidth = 1f;
								Color currentChroma = new Color(playgroundParticlesScriptReference.states[i].chromaKey.r,playgroundParticlesScriptReference.states[i].chromaKey.g, playgroundParticlesScriptReference.states[i].chromaKey.b);
								playgroundParticlesScriptReference.states[i].chromaKey = (Color32)EditorGUILayout.ColorField((Color)playgroundParticlesScriptReference.states[i].chromaKey);
								EditorGUIUtility.labelWidth = 50f;
								float currentSpread = playgroundParticlesScriptReference.states[i].chromaKeySpread;
								playgroundParticlesScriptReference.states[i].chromaKeySpread = EditorGUILayout.Slider(playgroundLanguage.spread, playgroundParticlesScriptReference.states[i].chromaKeySpread, 0, 1f);
								if (currentChroma!=new Color(playgroundParticlesScriptReference.states[i].chromaKey.r,playgroundParticlesScriptReference.states[i].chromaKey.g, playgroundParticlesScriptReference.states[i].chromaKey.b) || currentSpread!=playgroundParticlesScriptReference.states[i].chromaKeySpread || currentApplyChromaKey!=playgroundParticlesScriptReference.states[i].applyChromaKey)
									playgroundParticlesScriptReference.states[i].Initialize();
								GUI.enabled = true;
								EditorGUIUtility.labelWidth = 0;
								GUILayout.EndHorizontal();
							}
							
							GUILayout.BeginHorizontal();
							EditorGUILayout.PrefixLabel(playgroundLanguage.points+":");
							thisPoints = thisState.FindPropertyRelative("positionLength");
							EditorGUILayout.SelectableLabel(thisPoints.intValue.ToString(), GUILayout.MaxWidth(80));
							EditorGUILayout.Separator();
							if(GUILayout.Button(playgroundLanguage.refresh, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
								ParticleStateC thisStateClass;
								thisStateClass = playgroundParticlesScriptReference.states[i];
								thisStateClass.Initialize();
							}
							if(GUILayout.Button(playgroundLanguage.setParticleCount, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)) && isEditingInHierarchy){
								playgroundParticlesScriptReference.particleCount = thisPoints.intValue;
							}
							if(GUILayout.Button("++", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}))
								particleCount.intValue = particleCount.intValue+thisPoints.intValue;
							GUILayout.EndHorizontal();
							
						}
					}
					GUILayout.EndVertical();
				}
			} else {
				EditorGUILayout.HelpBox(playgroundLanguage.noStates, MessageType.Info);
			}
		}
		
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.BeginVertical(boxStyle);
		playgroundSettings.createNewStateFoldout = GUILayout.Toggle(playgroundSettings.createNewStateFoldout, playgroundLanguage.createState, EditorStyles.foldout);
		if (playgroundSettings.createNewStateFoldout) {
			EditorGUILayout.Separator();
			meshOrImage = GUILayout.Toolbar (meshOrImage, new string[]{playgroundLanguage.image,playgroundLanguage.mesh}, EditorStyles.toolbarButton);
			EditorGUILayout.Separator();
			// Add image or mesh
			if (meshOrImage==1)
				addStateMesh = EditorGUILayout.ObjectField(playgroundLanguage.mesh, addStateMesh, typeof(Mesh), true);
			addStateTexture = EditorGUILayout.ObjectField(playgroundLanguage.texture, addStateTexture, typeof(Texture2D), true);
			if (meshOrImage==0) {
				addStateDepthmap = EditorGUILayout.ObjectField(playgroundLanguage.depthmap, addStateDepthmap, typeof(Texture2D), true);
				if (addStateDepthmap!=null)
					addStateDepthmapStrength = EditorGUILayout.FloatField(playgroundLanguage.depthmapStrength, addStateDepthmapStrength);
			}
			addStateTransform = EditorGUILayout.ObjectField(playgroundLanguage.transform, addStateTransform, typeof(Transform), true);
			addStateName = EditorGUILayout.TextField(playgroundLanguage.nameText, addStateName);
			addStateScale = EditorGUILayout.FloatField(playgroundLanguage.scale, addStateScale);
			addStateOffset = EditorGUILayout.Vector3Field(playgroundLanguage.offset, addStateOffset);
			
			EditorGUILayout.Separator();
			
			if (meshOrImage==0)
				GUI.enabled = (addStateTexture!=null);
			else
				GUI.enabled = (addStateMesh!=null);
			
			if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
				
				// Check read/write
				if (addStateTexture!=null) {
					TextureImporter tAssetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(addStateTexture as UnityEngine.Object)) as TextureImporter;
					
					// If no Import Settings are found
					if (!tAssetImporter) {
						Debug.Log(playgroundLanguage.couldNotReadTexture);
						return; 
					}
					
					// If the texture isn't readable
					if (!tAssetImporter.isReadable) {
						Debug.Log(tAssetImporter.assetPath+" "+playgroundLanguage.notReadable);
						return; 
					}
				}
				if (addStateMesh!=null) {
					ModelImporter mAssetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(addStateMesh as UnityEngine.Object)) as ModelImporter;
					if (mAssetImporter==null) {
						Debug.Log(playgroundLanguage.couldNotReadMesh);
						return; 
					}
					if (!mAssetImporter.isReadable) {
						Debug.Log(mAssetImporter.assetPath+" "+playgroundLanguage.notReadable);
						return; 
					}
				}
				
				if (addStateName=="" || addStateName==null) addStateName = playgroundLanguage.state+" "+(states.arraySize).ToString();
				if (meshOrImage==0) {
					if (addStateDepthmap==null)
						PlaygroundC.Add(playgroundParticlesScriptReference, addStateTexture as Texture2D, addStateScale, addStateOffset, addStateName, addStateTransform as Transform);
					else
						PlaygroundC.Add(playgroundParticlesScriptReference, addStateTexture as Texture2D, addStateDepthmap as Texture2D, addStateDepthmapStrength, addStateScale, addStateOffset, addStateName, addStateTransform as Transform);
				} else {
					if (addStateTexture==null)
						PlaygroundC.Add(playgroundParticlesScriptReference, addStateMesh as Mesh, addStateScale, addStateOffset, addStateName, addStateTransform as Transform);
					else
						PlaygroundC.Add(playgroundParticlesScriptReference, addStateMesh as Mesh, addStateTexture as Texture2D, addStateScale, addStateOffset, addStateName, addStateTransform as Transform);
				}
				if (isEditingInHierarchy)
					playgroundParticlesScriptReference.Start();
				
				playgroundSettings.statesFoldout = true;
				statesListFoldout.Add(true);
				
				addStateName = "";
				addStateMesh = null;
				addStateTexture = null;
				addStateTransform = null;
				addStateDepthmap = null;
				addStateDepthmapStrength = 1f;
				addStateScale = 1f;
				addStateOffset = Vector3.zero;
			}
			GUI.enabled = true;
		}
		EditorGUILayout.EndVertical();
	}
	
	public void RenderProjectionSettings () {
		
		if (playgroundParticlesScriptReference.projection==null) {
			playgroundParticlesScriptReference.projection = new ParticleProjectionC();
			playgroundParticlesScriptReference.projection.projectionTransform = playgroundParticlesScriptReference.particleSystemTransform;
		}
		
		// Projection texture
		Texture2D prevTexture = playgroundParticlesScriptReference.projection.projectionTexture;
		playgroundParticlesScriptReference.projection.projectionTexture = EditorGUILayout.ObjectField(playgroundLanguage.projectionTexture, playgroundParticlesScriptReference.projection.projectionTexture, typeof(Texture2D), true) as Texture2D;
		
		// Texture changed
		if (prevTexture!=playgroundParticlesScriptReference.projection.projectionTexture) {
			TextureImporter tAssetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(playgroundParticlesScriptReference.projection.projectionTexture as UnityEngine.Object)) as TextureImporter;
			
			// If no Import Settings are found
			if (!tAssetImporter) {
				Debug.Log(playgroundLanguage.couldNotReadTexture);
				playgroundParticlesScriptReference.projection.projectionTexture = null;
				return; 
			}
			
			// If the texture isn't readable
			if (!tAssetImporter.isReadable) {
				Debug.Log(tAssetImporter.assetPath+" "+playgroundLanguage.notReadable);
				playgroundParticlesScriptReference.projection.projectionTexture = null;
				return; 
			}
			
			playgroundParticlesScriptReference.projection.Construct(playgroundParticlesScriptReference.projection.projectionTexture, playgroundParticlesScriptReference.projection.projectionTransform);
		}
		
		playgroundParticlesScriptReference.projection.projectionTransform = EditorGUILayout.ObjectField(playgroundLanguage.transform, playgroundParticlesScriptReference.projection.projectionTransform, typeof(Transform), true) as Transform;
		playgroundParticlesScriptReference.projection.liveUpdate = EditorGUILayout.Toggle(playgroundLanguage.liveUpdate, playgroundParticlesScriptReference.projection.liveUpdate);
		playgroundParticlesScriptReference.projection.projectionOrigin = EditorGUILayout.Vector2Field(playgroundLanguage.originOffset, playgroundParticlesScriptReference.projection.projectionOrigin);
		playgroundParticlesScriptReference.projection.projectionDistance = EditorGUILayout.FloatField(playgroundLanguage.projectionDistance, playgroundParticlesScriptReference.projection.projectionDistance);
		playgroundParticlesScriptReference.projection.projectionScale = EditorGUILayout.FloatField(playgroundLanguage.projectionScale, playgroundParticlesScriptReference.projection.projectionScale);
		playgroundParticlesScriptReference.projection.surfaceOffset = EditorGUILayout.FloatField(playgroundLanguage.surfaceOffset, playgroundParticlesScriptReference.projection.surfaceOffset);
		EditorGUILayout.PropertyField(projectionMask, new GUIContent(playgroundLanguage.projectionMask));
		EditorGUILayout.PropertyField(projectionCollisionType, new GUIContent(playgroundLanguage.projectionCollisionType));
		if (projectionCollisionType.enumValueIndex==1) {
			GUILayout.BeginHorizontal();
			GUILayout.Space (16);
			GUILayout.Label(playgroundLanguage.depth);
			EditorGUILayout.Separator();
			float minDepth = playgroundParticlesScriptReference.projection.minDepth;
			float maxDepth = playgroundParticlesScriptReference.projection.maxDepth;
			EditorGUILayout.MinMaxSlider(ref minDepth, ref maxDepth, -playgroundSettings.maximumAllowedDepth, playgroundSettings.maximumAllowedDepth, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
			playgroundParticlesScriptReference.projection.minDepth = Mathf.Clamp (minDepth, -playgroundSettings.maximumAllowedDepth, playgroundSettings.maximumAllowedDepth);
			playgroundParticlesScriptReference.projection.maxDepth = Mathf.Clamp (maxDepth, -playgroundSettings.maximumAllowedDepth, playgroundSettings.maximumAllowedDepth);
			playgroundParticlesScriptReference.projection.minDepth = EditorGUILayout.FloatField(playgroundParticlesScriptReference.projection.minDepth, GUILayout.Width(50));
			playgroundParticlesScriptReference.projection.maxDepth = EditorGUILayout.FloatField(playgroundParticlesScriptReference.projection.maxDepth, GUILayout.Width(50));
			GUILayout.EndHorizontal();
		}
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(playgroundLanguage.points+":");
		EditorGUILayout.SelectableLabel(playgroundParticlesScriptReference.projection.positionLength.ToString(), GUILayout.MaxWidth(80));
		EditorGUILayout.Separator();
		if(GUILayout.Button(playgroundLanguage.refresh, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
			playgroundParticlesScriptReference.projection.Initialize();
		}
		if(GUILayout.Button(playgroundLanguage.setParticleCount, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)) && isEditingInHierarchy){
			playgroundParticlesScriptReference.particleCount = playgroundParticlesScriptReference.projection.positionLength;
		}
		if(GUILayout.Button("++", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(24), GUILayout.Height(16)}))
			particleCount.intValue = particleCount.intValue+playgroundParticlesScriptReference.projection.positionLength;
		GUILayout.EndHorizontal();
	}
	
	public static int selectedSort;
	
	public void LifetimeSorting () {
		if (playgroundParticlesScriptReference.source!=SOURCEC.Script && isEditingInHierarchy)
			playgroundParticlesScriptReference.Start();
	}
	
	public void LifetimeSortingAll () {
		if (isEditingInHierarchy)
			foreach (PlaygroundParticlesC p in PlaygroundC.reference.particleSystems)
				p.Start();
	}
	
	public void RemoveState (int i) {
		playgroundParticlesScriptReference.RemoveState(i);
	}
	
	
	public void StartStopPaint () {
		if (!isEditingInHierarchy) return;
		inPaintMode = !inPaintMode;
		playgroundParticlesScriptReference.Start();
		if (inPaintMode) {
			if (selectedPaintMode==1)
				SetBrush(selectedBrushPreset);
			
			UnityEditor.Tools.current = UnityEditor.Tool.None;
		} else {
			UnityEditor.Tools.current = lastActiveTool;
		}
	}
	
	public void ClearPaint () {
		if (EditorUtility.DisplayDialog(
			playgroundLanguage.clearPaint,
			playgroundLanguage.clearPaintText, 
			playgroundLanguage.yes, playgroundLanguage.no)) {
			inPaintMode = false;
			PlaygroundC.ClearPaint(playgroundParticlesScriptReference);
			PlaygroundParticlesC.SetParticleCount(playgroundParticlesScriptReference, playgroundParticlesScriptReference.particleCount);
		}
	}
	
	public void DrawCollisionPlane (PlaygroundColliderC pc) {
		float scale = playgroundScriptReference.collisionPlaneScale;
		if (scale<=0) return;
		Vector3 p1;
		Vector3 p2;
		Handles.color = pc.enabled?new Color(0f,.8f,.1f,.25f):new Color(0f,.8f,.1f,.05f);
		for (int x = 0; x<11; x++) {
			p1 = pc.transform.TransformPoint(new Vector3((x*10f)-50f, 0f, 50f)*scale)+pc.offset;
			p2 = pc.transform.TransformPoint(new Vector3((x*10f)-50f, 0f, -50f)*scale)+pc.offset;
			Handles.DrawLine(p1, p2);
		}
		for (int y = 0; y<11; y++) {
			p1 = pc.transform.TransformPoint(new Vector3(50f, 0f, (y*10f)-50f)*scale)+pc.offset;
			p2 = pc.transform.TransformPoint(new Vector3(-50f, 0f, (y*10f)-50f)*scale)+pc.offset;
			Handles.DrawLine(p1, p2);
		}
	}
	
	bool keyPressed = false;
	int foldoutHeight = 0;
	Quaternion cameraRotation;
	RaycastHit eraserHit = new RaycastHit();
	RaycastHit2D eraserHit2d = new RaycastHit2D();
	Color paintActiveColor = new Color(0,1f,.7f,1f);
	Color whiteColor = Color.white;
	void OnSceneGUI () {
		
		cameraRotation = Camera.current.transform.rotation;
		
		// Source Transforms
		if (playgroundScriptReference.drawGizmos && playgroundSettings.sourceFoldout && playgroundParticlesScriptReference.source==SOURCEC.Transform && playgroundParticlesScriptReference.sourceTransforms.Count>0) {
			for (int i = 0; i<playgroundParticlesScriptReference.sourceTransforms.Count; i++) {
				
				if (playgroundParticlesScriptReference.sourceTransforms[i].transform==null) continue;				
				
				// Position
				if (UnityEditor.Tools.current==UnityEditor.Tool.Move) {
					EditorGUI.BeginChangeCheck();
					Vector3 pos = Handles.PositionHandle(playgroundParticlesScriptReference.sourceTransforms[i].transform.position, UnityEditor.Tools.pivotRotation==PivotRotation.Global? Quaternion.identity : playgroundParticlesScriptReference.sourceTransforms[i].transform.rotation);
					if (EditorGUI.EndChangeCheck()) {
						Undo.RecordObject(playgroundParticlesScriptReference, "Move Source Transform");
						EditorUtility.SetDirty(playgroundParticlesScriptReference);
						playgroundParticlesScriptReference.sourceTransforms[i].transform.position = pos;
					}
					// Rotation
				} else if (UnityEditor.Tools.current==UnityEditor.Tool.Rotate) {
					EditorGUI.BeginChangeCheck();
					Quaternion rot = Handles.RotationHandle(playgroundParticlesScriptReference.sourceTransforms[i].transform.rotation, playgroundParticlesScriptReference.sourceTransforms[i].transform.position);
					if (EditorGUI.EndChangeCheck()) {
						Undo.RecordObject(playgroundParticlesScriptReference, "Rotate Source Transform");
						EditorUtility.SetDirty(playgroundParticlesScriptReference);
						playgroundParticlesScriptReference.sourceTransforms[i].transform.rotation = rot;
					}
					// Scale
				} else if (UnityEditor.Tools.current==UnityEditor.Tool.Scale) {
					EditorGUI.BeginChangeCheck();
					Vector3 sca = Handles.ScaleHandle(playgroundParticlesScriptReference.sourceTransforms[i].transform.localScale, playgroundParticlesScriptReference.sourceTransforms[i].transform.position, playgroundParticlesScriptReference.sourceTransforms[i].transform.rotation, HandleUtility.GetHandleSize(playgroundParticlesScriptReference.sourceTransforms[i].transform.position));
					if (EditorGUI.EndChangeCheck()) {
						Undo.RecordObject(playgroundParticlesScriptReference, "Scale Source Transform");
						EditorUtility.SetDirty(playgroundParticlesScriptReference);
						playgroundParticlesScriptReference.sourceTransforms[i].transform.localScale = sca;
					}
				}
			}
			
		}
		
		// Collision Planes
		if (playgroundScriptReference.drawGizmos && playgroundSettings.collisionFoldout && playgroundParticlesScriptReference.collision && playgroundParticlesScriptReference.colliders.Count>0) {
			for (int c = 0; c<playgroundParticlesScriptReference.colliders.Count; c++) {
				
				if (playgroundParticlesScriptReference.colliders[c].transform==null) continue;
				
				DrawCollisionPlane(playgroundParticlesScriptReference.colliders[c]);
				
				if (playgroundParticlesScriptReference.colliders[c].enabled) {
					// Position
					if (UnityEditor.Tools.current==UnityEditor.Tool.Move)
						playgroundParticlesScriptReference.colliders[c].transform.position = Handles.PositionHandle(playgroundParticlesScriptReference.colliders[c].transform.position, UnityEditor.Tools.pivotRotation==PivotRotation.Global? Quaternion.identity : playgroundParticlesScriptReference.colliders[c].transform.rotation);
					// Rotation
					else if (UnityEditor.Tools.current==UnityEditor.Tool.Rotate)
						playgroundParticlesScriptReference.colliders[c].transform.rotation = Handles.RotationHandle(playgroundParticlesScriptReference.colliders[c].transform.rotation, playgroundParticlesScriptReference.colliders[c].transform.position);
					// Scale
					else if (UnityEditor.Tools.current==UnityEditor.Tool.Scale)
						playgroundParticlesScriptReference.colliders[c].transform.localScale = Handles.ScaleHandle(playgroundParticlesScriptReference.colliders[c].transform.localScale, playgroundParticlesScriptReference.colliders[c].transform.position, playgroundParticlesScriptReference.colliders[c].transform.rotation, HandleUtility.GetHandleSize(playgroundParticlesScriptReference.colliders[c].transform.position));
				}
			}
		}
		
		// Source position hilight
		if (playgroundScriptReference.drawGizmos && playgroundScriptReference.drawSourcePositions) {
			Handles.color = new Color(1f,1f,.2f,.2f);
			for (int pos = 0; pos<playgroundParticlesScriptReference.playgroundCache.targetPosition.Length; pos++) {
				Handles.DotCap(0, playgroundParticlesScriptReference.playgroundCache.targetPosition[pos], cameraRotation, .025f);
			}
		}
		
		// Nearest neighbor sorting highlight
		if (playgroundScriptReference.drawGizmos && playgroundSettings.particleSettingsFoldout && (playgroundParticlesScriptReference.sorting==SORTINGC.NearestNeighbor || playgroundParticlesScriptReference.sorting==SORTINGC.NearestNeighborReversed) && playgroundParticlesScriptReference.playgroundCache!=null && playgroundParticlesScriptReference.playgroundCache.targetPosition!=null && playgroundParticlesScriptReference.playgroundCache.targetPosition.Length>0 && playgroundParticlesScriptReference.particleCount>0) {
			Vector3 sortOriginPosition = Vector3.zero;
			bool drawPoint = true;
			switch (playgroundParticlesScriptReference.nearestNeighborOriginMethod) {
			case NEARESTNEIGHBORORIGINMETHOD.SourcePoint:
				sortOriginPosition = playgroundParticlesScriptReference.shurikenParticleSystem.simulationSpace==ParticleSystemSimulationSpace.World?
					playgroundParticlesScriptReference.playgroundCache.targetPosition[playgroundParticlesScriptReference.nearestNeighborOrigin%playgroundParticlesScriptReference.playgroundCache.targetPosition.Length]:
						playgroundParticlesScriptReference.particleSystemTransform.TransformPoint(playgroundParticlesScriptReference.playgroundCache.targetPosition[playgroundParticlesScriptReference.nearestNeighborOrigin%playgroundParticlesScriptReference.playgroundCache.targetPosition.Length]);
				break;
			case NEARESTNEIGHBORORIGINMETHOD.Vector3:
				sortOriginPosition = playgroundParticlesScriptReference.shurikenParticleSystem.simulationSpace==ParticleSystemSimulationSpace.World?
					playgroundParticlesScriptReference.nearestNeighborOriginVector3:
						playgroundParticlesScriptReference.particleSystemTransform.TransformPoint(playgroundParticlesScriptReference.nearestNeighborOriginVector3);
				break;
			case NEARESTNEIGHBORORIGINMETHOD.Transform:
				if (playgroundParticlesScriptReference.nearestNeighborOriginTransform!=null) {
					sortOriginPosition = playgroundParticlesScriptReference.nearestNeighborOriginTransform.position;
				} else drawPoint = false;
				break;
			}
			if (drawPoint) {
				Handles.color = new Color(1f,1f,.2f,.6f);
				Handles.CircleCap(0, sortOriginPosition, cameraRotation, HandleUtility.GetHandleSize(sortOriginPosition)*.05f);
				Handles.color = new Color(1f,.7f,.2f,.2f);
				Handles.DrawSolidDisc(sortOriginPosition, Camera.current.transform.forward, HandleUtility.GetHandleSize(sortOriginPosition)*.2f);
			}
		}
		
		// Projection mode
		if (playgroundParticlesScriptReference.source == SOURCEC.Projection) {
			
			// Projector preview
			if (playgroundScriptReference.drawGizmos && playgroundParticlesScriptReference.projection!=null && playgroundParticlesScriptReference.projection.projectionTexture!=null && playgroundParticlesScriptReference.projection.projectionTransform!=null) {
				RaycastHit projectorHit;
				Vector3 p2 = playgroundParticlesScriptReference.projection.projectionTransform.position+(playgroundParticlesScriptReference.projection.projectionTransform.forward*playgroundParticlesScriptReference.projection.projectionDistance);
				bool projectorHasSurface = false;
				if (Physics.Raycast(playgroundParticlesScriptReference.projection.projectionTransform.position, playgroundParticlesScriptReference.projection.projectionTransform.forward, out projectorHit, playgroundParticlesScriptReference.projection.projectionDistance, playgroundParticlesScriptReference.projection.projectionMask)) {
					p2 = projectorHit.point;
					projectorHasSurface = true;
				}
				Handles.color = projectorHasSurface?new Color(1f,1f,.25f,.6f):new Color(1f,1f,.25f,.2f);
				Handles.DrawLine(playgroundParticlesScriptReference.projection.projectionTransform.position, p2);
			}
		}
		
		// Paint mode
		if (playgroundParticlesScriptReference.source == SOURCEC.Paint) {
			Event e = Event.current;
			
			// Paint Toolbox in Scene View
			Rect toolboxRect = new Rect(10f,Screen.height-(138f+foldoutHeight),300f,103f+foldoutHeight);
			if (PlaygroundC.reference.paintToolbox) {
				if (!playgroundSettings.paintToolboxSettingsFoldout) {
					foldoutHeight = 0;
				} else {
					switch (selectedPaintMode) {
					case 0: foldoutHeight = 54; break;
					case 1: foldoutHeight = 144; break;
					case 2: foldoutHeight = 36; break;
					}
				}
				if (!playgroundSettings.toolboxFoldout) foldoutHeight=-69;
				
				// Don't deselect upon click
				if (e.type == EventType.Layout && toolboxRect.Contains (e.mousePosition)) {
					HandleUtility.AddDefaultControl(0);
				}
				
				Handles.BeginGUI();
				GUILayout.BeginArea(toolboxRect);
				if (boxStyle==null)
					boxStyle = GUI.skin.FindStyle("box");
				if (inPaintMode)
					GUI.backgroundColor = paintActiveColor;
				GUILayout.BeginVertical(boxStyle);
				playgroundSettings.toolboxFoldout = GUILayout.Toggle(playgroundSettings.toolboxFoldout, playgroundLanguage.playgroundPaint, EditorStyles.foldout);
				if (playgroundSettings.toolboxFoldout) {
					selectedPaintMode = GUILayout.Toolbar (selectedPaintMode, new string[]{playgroundLanguage.dot,playgroundLanguage.brush,playgroundLanguage.eraser}, EditorStyles.toolbarButton);
					
					// Settings
					GUILayout.BeginVertical(boxStyle);
					playgroundSettings.paintToolboxSettingsFoldout = GUILayout.Toggle(playgroundSettings.paintToolboxSettingsFoldout, playgroundLanguage.settings, EditorStyles.foldout);
					if (playgroundSettings.paintToolboxSettingsFoldout) {
						switch (selectedPaintMode) {
						case 0:
							GUI.backgroundColor = whiteColor;
							paintColor = EditorGUILayout.ColorField(playgroundLanguage.color, paintColor);
							if (inPaintMode)
								GUI.backgroundColor = paintActiveColor;
							playgroundParticlesScriptReference.paint.spacing = EditorGUILayout.Slider(playgroundLanguage.paintSpacing, playgroundParticlesScriptReference.paint.spacing, .0f, playgroundSettings.maximumAllowedPaintSpacing);
							EditorGUILayout.PropertyField(paintLayerMask, new GUIContent(playgroundLanguage.paintMask));
							break;
						case 1:
							GUILayout.BeginHorizontal();
							EditorGUILayout.PrefixLabel(playgroundLanguage.brushShape);
							GUI.backgroundColor = whiteColor;
							paintTexture = EditorGUILayout.ObjectField(paintTexture, typeof(Texture2D), false) as Texture2D;
							if (inPaintMode)
								GUI.backgroundColor = paintActiveColor;
							GUILayout.EndHorizontal();
							if (paintTexture!=null && paintTexture!=playgroundParticlesScriptReference.paint.brush.texture) {
								selectedBrushPreset = -1;
								SetBrush(selectedBrushPreset);
							}
							playgroundParticlesScriptReference.paint.brush.detail = (BRUSHDETAILC)EditorGUILayout.EnumPopup(playgroundLanguage.detail, playgroundParticlesScriptReference.paint.brush.detail);
							playgroundParticlesScriptReference.paint.brush.scale = EditorGUILayout.Slider(playgroundLanguage.scale, playgroundParticlesScriptReference.paint.brush.scale, playgroundSettings.minimumAllowedBrushScale, playgroundSettings.maximumAllowedBrushScale);
							playgroundParticlesScriptReference.paint.brush.distance = EditorGUILayout.FloatField(playgroundLanguage.distance, playgroundParticlesScriptReference.paint.brush.distance);
							useBrushColor = EditorGUILayout.Toggle(playgroundLanguage.useBrushColor, useBrushColor);
							GUI.enabled = !useBrushColor;
							GUI.backgroundColor = whiteColor;
							paintColor = EditorGUILayout.ColorField(playgroundLanguage.color, paintColor);
							if (inPaintMode)
								GUI.backgroundColor = paintActiveColor;
							GUI.enabled = true;
							playgroundParticlesScriptReference.paint.spacing = EditorGUILayout.Slider(playgroundLanguage.paintSpacing, playgroundParticlesScriptReference.paint.spacing, .0f, playgroundSettings.maximumAllowedPaintSpacing);
							EditorGUILayout.PropertyField(paintLayerMask, new GUIContent(playgroundLanguage.paintMask));
							break;
						case 2:
							eraserRadius = EditorGUILayout.Slider(playgroundLanguage.radius, eraserRadius, playgroundSettings.minimumEraserRadius, playgroundSettings.maximumEraserRadius);
							EditorGUILayout.PropertyField(paintLayerMask, new GUIContent(playgroundLanguage.paintMask));
							break;
						}
					}
					GUILayout.EndVertical();
					GUILayout.BeginHorizontal();
					//GUI.enabled = !(selectedPaintMode==1 && paintTexture==null);
					if(GUILayout.Button((inPaintMode?playgroundLanguage.stop:playgroundLanguage.start)+" "+playgroundLanguage.paint, EditorStyles.toolbarButton))
						StartStopPaint();
					GUI.enabled = (playgroundParticlesScriptReference.paint.positionLength>0);
					if(GUILayout.Button(playgroundLanguage.clear, EditorStyles.toolbarButton))
						ClearPaint();
					GUI.enabled = true;
					ProgressBar((playgroundParticlesScriptReference.paint.positionLength*1f)/PlaygroundC.reference.paintMaxPositions, playgroundParticlesScriptReference.paint.positionLength+"/"+PlaygroundC.reference.paintMaxPositions, 115f);
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
				GUILayout.EndArea();
				Handles.EndGUI();
			}
			
			if (inPaintMode) {
				if (e.type == EventType.Layout) {
					HandleUtility.AddDefaultControl(0);
				}
				
				Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
				
				// Brush preview
				if (selectedPaintMode==1 && playgroundParticlesScriptReference.paint.brush.texture!=null && sceneBrushStyle!=null && !toolboxRect.Contains(e.mousePosition)) {
					Handles.Label(mouseRay.origin, new GUIContent(playgroundParticlesScriptReference.paint.brush.texture as Texture2D), sceneBrushStyle);
				}
				
				// Eraser preview
				if (selectedPaintMode==2 && !toolboxRect.Contains(e.mousePosition)) {
					if (playgroundParticlesScriptReference.paint.collisionType==COLLISIONTYPEC.Physics3D) {
						if (Physics.Raycast(mouseRay, out eraserHit, 10000f, playgroundParticlesScriptReference.paint.layerMask)) {
							Handles.color = new Color(0f,0f,0f,.4f);
							Handles.CircleCap(-1, eraserHit.point, Quaternion.LookRotation(mouseRay.direction), eraserRadius);
						}
					} else {
						eraserHit2d = Physics2D.Raycast (mouseRay.origin, mouseRay.direction, 100000f, playgroundParticlesScriptReference.paint.layerMask, playgroundParticlesScriptReference.paint.minDepth, playgroundParticlesScriptReference.paint.maxDepth);
						if (eraserHit2d.collider!=null) {
							Handles.color = new Color(0f,0f,0f,.4f);
							Handles.CircleCap(-1, eraserHit2d.point, Quaternion.LookRotation(mouseRay.direction), eraserRadius);
						}
					}
				}
				
				
				// Spacing preview
				if (selectedPaintMode!=2) {
					Handles.color = new Color(.3f,1f,.3f,.3f);
					Handles.CircleCap(-1, playgroundParticlesScriptReference.paint.lastPaintPosition, Quaternion.LookRotation(Camera.current.transform.forward), playgroundParticlesScriptReference.paint.spacing);
				}
				
				if (e.type  == EventType.KeyDown)
					keyPressed = true;
				else if (e.type == EventType.KeyUp)
					keyPressed = false;
				
				// Paint from the Brush's texture into the Scene View
				if (!keyPressed && e.button == 0 && e.isMouse && !e.alt) {
					if (e.type == EventType.MouseDrag || e.type == EventType.MouseDown) {
						switch (selectedPaintMode) {
							// Dot
						case 0:
							if (playgroundParticlesScriptReference.paint.exceedMaxStopsPaint && playgroundParticlesScriptReference.paint.positionLength>=PlaygroundC.reference.paintMaxPositions) return;
							if (playgroundParticlesScriptReference.paint.collisionType==COLLISIONTYPEC.Physics3D) {
								RaycastHit dotHit;
								if (Physics.Raycast(mouseRay, out dotHit, 10000f, playgroundParticlesScriptReference.paint.layerMask)) {
									if (e.type != EventType.MouseDown)
										if (Vector3.Distance(dotHit.point, playgroundParticlesScriptReference.paint.lastPaintPosition)<=playgroundParticlesScriptReference.paint.spacing) return;
									PlaygroundC.Paint(playgroundParticlesScriptReference, dotHit.point, dotHit.normal, dotHit.transform, paintColor);
									playgroundParticlesScriptReference.paint.lastPaintPosition = dotHit.point;
									playgroundParticlesScriptReference.SetHasActiveParticles();
								}
							} else {
								RaycastHit2D dotHit2d = Physics2D.Raycast (mouseRay.origin, mouseRay.direction, 10000f, playgroundParticlesScriptReference.paint.layerMask, playgroundParticlesScriptReference.paint.minDepth, playgroundParticlesScriptReference.paint.maxDepth);
								if (dotHit2d.collider!=null) {
									if (e.type != EventType.MouseDown)
										if (Vector3.Distance(dotHit2d.point, playgroundParticlesScriptReference.paint.lastPaintPosition)<=playgroundParticlesScriptReference.paint.spacing) return;
									PlaygroundC.Paint(playgroundParticlesScriptReference, dotHit2d.point, dotHit2d.normal, dotHit2d.transform, paintColor);
									playgroundParticlesScriptReference.paint.lastPaintPosition = dotHit2d.point;
									playgroundParticlesScriptReference.SetHasActiveParticles();
								}
							}
							break;
							// Brush
						case 1:
							if (playgroundParticlesScriptReference.paint.exceedMaxStopsPaint && playgroundParticlesScriptReference.paint.positionLength>=PlaygroundC.reference.paintMaxPositions || !playgroundParticlesScriptReference.paint.brush.texture || playgroundParticlesScriptReference.paint.brush.colorLength <= 0) return;
							if (e.type != EventType.MouseDown) {
								if (playgroundParticlesScriptReference.paint.collisionType==COLLISIONTYPEC.Physics3D) {
									RaycastHit brushHit;
									if (Physics.Raycast(mouseRay, out brushHit, 10000f, playgroundParticlesScriptReference.paint.layerMask))
										if (Vector3.Distance(brushHit.point, playgroundParticlesScriptReference.paint.lastPaintPosition)<=playgroundParticlesScriptReference.paint.spacing) return;
								} else {
									RaycastHit2D brushHit2d = Physics2D.Raycast(mouseRay.origin, mouseRay.direction, 10000f, playgroundParticlesScriptReference.paint.layerMask, playgroundParticlesScriptReference.paint.minDepth, playgroundParticlesScriptReference.paint.maxDepth);
									if (brushHit2d.collider!=null)
										if (Vector3.Distance(brushHit2d.point, playgroundParticlesScriptReference.paint.lastPaintPosition)<=playgroundParticlesScriptReference.paint.spacing) return;
								}
							}
							int detail = 0;
							switch (playgroundParticlesScriptReference.paint.brush.detail) {
							case BRUSHDETAILC.Perfect: detail=0; break;
							case BRUSHDETAILC.High: detail=2; break;
							case BRUSHDETAILC.Medium: detail=4; break;
							case BRUSHDETAILC.Low: detail=6; break;
							}
							Color32 pixelColor;
							for (int x = 0; x<playgroundParticlesScriptReference.paint.brush.texture.width; x++) {
								for (int y = 0; y<playgroundParticlesScriptReference.paint.brush.texture.height; y++) {
									if (detail==0 || ((x+1)*(y+1)-1)%detail==0) {
										pixelColor = playgroundParticlesScriptReference.paint.brush.GetColor((x+1)*(y+1)-1);
										if (!useBrushColor) pixelColor = new Color(paintColor.r, paintColor.g, paintColor.b, pixelColor.a);
										if (pixelColor.a!=0) {
											mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition+new Vector2((-playgroundParticlesScriptReference.paint.brush.texture.width/2f)+x,(-playgroundParticlesScriptReference.paint.brush.texture.height/2)+y)*playgroundParticlesScriptReference.paint.brush.scale);
											playgroundParticlesScriptReference.paint.Paint(mouseRay, pixelColor);
											playgroundParticlesScriptReference.SetHasActiveParticles();
										}
									}
								}
							}
							break;
							// Eraser
						case 2:
							if (playgroundParticlesScriptReference.paint.collisionType==COLLISIONTYPEC.Physics3D && eraserHit.collider!=null || playgroundParticlesScriptReference.paint.collisionType==COLLISIONTYPEC.Physics2D && eraserHit2d.collider!=null) {
								playgroundParticlesScriptReference.paint.Erase((playgroundParticlesScriptReference.paint.collisionType==COLLISIONTYPEC.Physics3D)?eraserHit.point:new Vector3(eraserHit2d.point.x, eraserHit2d.point.y), eraserRadius);
							}
							break;
						}
						
					}
					SceneView.RepaintAll();
				}
				
				
				if (e.type == EventType.MouseUp) {
					playgroundParticlesScriptReference.paint.lastPaintPosition = PlaygroundC.initialTargetPosition;
					
					// No positions to emit from, reset particle system by rebuilding
					if ((eraserHit.collider!=null || eraserHit2d.collider!=null) && playgroundParticlesScriptReference.paint.positionLength==0) {
						PlaygroundC.SetParticleCount(playgroundParticlesScriptReference, playgroundParticlesScriptReference.particleCount);
					}
				}
			}
		}
		
		// Render global manipulators
		if (PlaygroundInspectorC.playgroundSettings!=null) {
			int i = 0;
			if (playgroundScriptReference!=null && playgroundScriptReference.drawGizmos && (PlaygroundInspectorC.playgroundSettings.manipulatorsFoldout || PlaygroundInspectorC.playgroundSettings.globalManipulatorsFoldout))
				for (; i<playgroundScriptReference.manipulators.Count; i++)
					PlaygroundInspectorC.RenderManipulatorInScene(playgroundScriptReference.manipulators[i], playgroundScriptReference.manipulators[i].shape == MANIPULATORSHAPEC.Infinite? new Color(1f,.1f,.1f,1f) : playgroundScriptReference.manipulators[i].inverseBounds?new Color(1f,.6f,.4f,1f):new Color(.4f,.6f,1f,1f));
			// Render local manipulators
			if (playgroundScriptReference.drawGizmos && playgroundSettings.manipulatorsFoldout)
				for (i = 0; i<playgroundParticlesScriptReference.manipulators.Count; i++)
					PlaygroundInspectorC.RenderManipulatorInScene(playgroundParticlesScriptReference.manipulators[i], playgroundParticlesScriptReference.manipulators[i].shape == MANIPULATORSHAPEC.Infinite? new Color(1f,.1f,.1f,1f) : playgroundParticlesScriptReference.manipulators[i].inverseBounds?new Color(1f,1f,.4f,1f):new Color(.4f,1f,1f,1f));
		}
		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}
	
}

enum SortMode {
	None=0,
	ByDistance=1,
	YoungestFirst=2,
	OldestFirst=3
}
enum AnimationType {
	WholeSheet=0,
	SingleRow=1
}
enum MinMaxState {
	Constant=0,
	Curve=1,
	RandomBetweenTwoCurves=2,
	RandomBetweenTwoConstants=3
}