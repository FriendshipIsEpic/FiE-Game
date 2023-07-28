using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using ParticlePlayground;
using ParticlePlaygroundLanguage;
using PlaygroundSplines;

[CustomEditor (typeof(PlaygroundC))]
public class PlaygroundInspectorC : Editor {
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Playground variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public static PlaygroundC playgroundScriptReference;
	public static SerializedObject playground;
	public static SerializedProperty calculate;
	public static SerializedProperty pixelFilterMode;
	public static SerializedProperty globalTimeScale;
	public static SerializedProperty autoGroup;
	public static SerializedProperty buildZeroAlphaPixels;
	public static SerializedProperty drawGizmos;
	public static SerializedProperty drawSourcePositions;
	public static SerializedProperty drawWireframe;
	public static SerializedProperty drawSplinePreview;
	public static SerializedProperty paintToolbox;
	public static SerializedProperty showShuriken;
	public static SerializedProperty showSnapshots;
	public static SerializedProperty threadPool;
	public static SerializedProperty threads;
	public static SerializedProperty threadsTurbulence;
	public static SerializedProperty threadsSkinned;
	public static SerializedProperty maxThreads;
	
	public static SerializedProperty particleSystems;
	public static SerializedProperty manipulators;
	
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Internal variables
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public static Vector3 manipulatorHandlePosition;
	public static GUIStyle boxStyle;
	public static bool showSnapshotsSettings;
	
	public static PlaygroundSettingsC playgroundSettings;
	public static PlaygroundLanguageC playgroundLanguage;
	
	public void OnEnable () {
		Initialize(target as PlaygroundC);
	}
	
	public static void Initialize (PlaygroundC targetRef) {
		playgroundScriptReference = targetRef;
		PlaygroundC.reference = targetRef;
		if (playgroundScriptReference==null) return;
		playground = new SerializedObject(playgroundScriptReference);
		particleSystems = playground.FindProperty("particleSystems");
		manipulators = playground.FindProperty("manipulators");
		calculate = playground.FindProperty("calculate");
		pixelFilterMode = playground.FindProperty("pixelFilterMode");
		globalTimeScale = playground.FindProperty("globalTimeScale");
		autoGroup = playground.FindProperty("autoGroup");
		buildZeroAlphaPixels = playground.FindProperty("buildZeroAlphaPixels");
		drawGizmos = playground.FindProperty("drawGizmos");
		drawSourcePositions = playground.FindProperty("drawSourcePositions");
		drawWireframe = playground.FindProperty("drawWireframe");
		drawSplinePreview = playground.FindProperty("drawSplinePreview");
		paintToolbox = playground.FindProperty("paintToolbox");
		showShuriken = playground.FindProperty("showShuriken");
		showSnapshots = playground.FindProperty("showSnapshotsInHierarchy");
		threadPool = playground.FindProperty("threadPoolMethod");
		threads = playground.FindProperty("threadMethod");
		threadsTurbulence = playground.FindProperty("turbulenceThreadMethod");
		threadsSkinned = playground.FindProperty("skinnedMeshThreadMethod");
		maxThreads = playground.FindProperty("maxThreads");
		
		playgroundSettings = PlaygroundSettingsC.GetReference();
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();
	}
	
	public override void OnInspectorGUI () {
		if (PlaygroundC.reference==null) return;
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");
		
		EditorGUILayout.Separator();
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(playgroundLanguage.playgroundName+" "+PlaygroundC.version+PlaygroundC.specialVersion, EditorStyles.largeLabel, GUILayout.Height(20));
		
		EditorGUILayout.Separator();
		
		if(GUILayout.Button(playgroundLanguage.openPlaygroundWizard, EditorStyles.toolbarButton, GUILayout.Width(130))) {
			PlaygroundParticleWindowC.ShowWindow();
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();
		RenderPlaygroundSettings();
		
		if (Event.current.type == EventType.ValidateCommand &&
		    Event.current.commandName == "UndoRedoPerformed") {
			foreach (PlaygroundParticlesC p in playgroundScriptReference.particleSystems) {
				p.Boot();
			}
		}
		
	}
	
	void OnSceneGUI () {
		if (playgroundScriptReference!=null && playgroundScriptReference.drawGizmos && playgroundSettings.globalManipulatorsFoldout)
			for (int i = 0; i<playgroundScriptReference.manipulators.Count; i++)
				RenderManipulatorInScene(playgroundScriptReference.manipulators[i], playgroundScriptReference.manipulators[i].shape == MANIPULATORSHAPEC.Infinite? new Color(1f,.1f,.1f,1f) : playgroundScriptReference.manipulators[i].inverseBounds? new Color(1f,.6f,.4f,1f) : new Color(.4f,.6f,1f,1f));
		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}
	
	public static void RenderManipulatorInScene (ManipulatorObjectC thisManipulator, Color manipulatorColor) {
		// Draw Manipulators in Scene View
		if (thisManipulator.transform.available && thisManipulator.transform.transform!=null) {
			Handles.color = new Color(manipulatorColor.r,manipulatorColor.g,manipulatorColor.b,Mathf.Clamp(Mathf.Abs(thisManipulator.strength),.25f,1f));
			Handles.color = thisManipulator.enabled? Handles.color : new Color(manipulatorColor.r,manipulatorColor.g,manipulatorColor.b,.2f);
			
			// Position
			if (UnityEditor.Tools.current==UnityEditor.Tool.Move)
				thisManipulator.transform.transform.position = Handles.PositionHandle(thisManipulator.transform.transform.position, UnityEditor.Tools.pivotRotation==PivotRotation.Global? Quaternion.identity : thisManipulator.transform.transform.rotation);
			// Rotation
			else if (UnityEditor.Tools.current==UnityEditor.Tool.Rotate)
				thisManipulator.transform.transform.rotation = Handles.RotationHandle(thisManipulator.transform.transform.rotation, thisManipulator.transform.transform.position);
			// Scale
			else if (UnityEditor.Tools.current==UnityEditor.Tool.Scale)
				thisManipulator.transform.transform.localScale = Handles.ScaleHandle(thisManipulator.transform.transform.localScale, thisManipulator.transform.transform.position, thisManipulator.transform.transform.rotation, HandleUtility.GetHandleSize(thisManipulator.transform.transform.position));
			
			// Sphere Size
			if (thisManipulator.shape==MANIPULATORSHAPEC.Sphere) {
				thisManipulator.size = Handles.RadiusHandle (Quaternion.identity, thisManipulator.transform.position, thisManipulator.size);
				if (thisManipulator.enabled && GUIUtility.hotControl>0)
					Handles.Label(thisManipulator.transform.transform.position+new Vector3(thisManipulator.size+1f,1f,0f), playgroundLanguage.size+" "+thisManipulator.size.ToString("f2"));
				
			// Box Bounds
			} else if (thisManipulator.shape==MANIPULATORSHAPEC.Box) {
				DrawManipulatorBox(thisManipulator);
			}
			
			// Strength
			manipulatorHandlePosition = thisManipulator.transform.transform.position+new Vector3(0f,thisManipulator.strength,0f);
			
			// Event particles
			if (thisManipulator.trackParticles) {
				
				Handles.Label(thisManipulator.transform.transform.position+new Vector3(0f,-(thisManipulator.size+1f),0f), thisManipulator.particles.Count+" "+playgroundLanguage.particles);
				
			}
			
			Handles.DrawLine(thisManipulator.transform.transform.position, manipulatorHandlePosition);
			thisManipulator.strength = Handles.ScaleValueHandle(thisManipulator.strength, manipulatorHandlePosition, Quaternion.identity, HandleUtility.GetHandleSize(manipulatorHandlePosition), Handles.SphereCap, 1);      
			if (thisManipulator.enabled && GUIUtility.hotControl>0)
				Handles.Label(manipulatorHandlePosition+new Vector3(1f,1f,0f), playgroundLanguage.strength+" "+thisManipulator.strength.ToString("f2"));
			
			Handles.color = new Color(.4f,.6f,1f,.025f);
			Handles.DrawSolidDisc(thisManipulator.transform.transform.position, Camera.current.transform.forward, thisManipulator.strength);
			Handles.color = new Color(.4f,.6f,1f,.5f);
			Handles.DrawSolidDisc(thisManipulator.transform.transform.position, Camera.current.transform.forward, HandleUtility.GetHandleSize(thisManipulator.transform.transform.position)*.05f);
		}
		
	}
	
	// Draws a Manipulator bounding box with handles in scene view
	public static void DrawManipulatorBox (ManipulatorObjectC manipulator) {
		Vector3 boxFrontTopLeft;
		Vector3 boxFrontTopRight;
		Vector3 boxFrontBottomLeft;
		Vector3 boxFrontBottomRight;
		Vector3 boxBackTopLeft;
		Vector3 boxBackTopRight;
		Vector3 boxBackBottomLeft;
		Vector3 boxBackBottomRight;
		Vector3 boxFrontDot;
		Vector3 boxLeftDot;
		Vector3 boxUpDot;
		
		// Always set positive values of bounds
		manipulator.bounds.extents = new Vector3(Mathf.Abs(manipulator.bounds.extents.x), Mathf.Abs(manipulator.bounds.extents.y), Mathf.Abs(manipulator.bounds.extents.z));
		
		// Set positions from bounds
		boxFrontTopLeft 		= new Vector3(manipulator.bounds.center.x - manipulator.bounds.extents.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z - manipulator.bounds.extents.z);
		boxFrontTopRight 		= new Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z - manipulator.bounds.extents.z);
		boxFrontBottomLeft 		= new Vector3(manipulator.bounds.center.x - manipulator.bounds.extents.x, manipulator.bounds.center.y - manipulator.bounds.extents.y, manipulator.bounds.center.z - manipulator.bounds.extents.z);
		boxFrontBottomRight 	= new Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y - manipulator.bounds.extents.y, manipulator.bounds.center.z - manipulator.bounds.extents.z);
		boxBackTopLeft 			= new Vector3(manipulator.bounds.center.x - manipulator.bounds.extents.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		boxBackTopRight 		= new Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		boxBackBottomLeft 		= new Vector3(manipulator.bounds.center.x - manipulator.bounds.extents.x, manipulator.bounds.center.y - manipulator.bounds.extents.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		boxBackBottomRight 		= new Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y - manipulator.bounds.extents.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		
		boxFrontDot				= new Vector3(manipulator.bounds.center.x + manipulator.bounds.extents.x, manipulator.bounds.center.y, manipulator.bounds.center.z);
		boxUpDot				= new Vector3(manipulator.bounds.center.x, manipulator.bounds.center.y + manipulator.bounds.extents.y, manipulator.bounds.center.z);
		boxLeftDot				= new Vector3(manipulator.bounds.center.x, manipulator.bounds.center.y, manipulator.bounds.center.z + manipulator.bounds.extents.z);
		
		// Apply transform positioning
		boxFrontTopLeft			= manipulator.transform.transform.TransformPoint(boxFrontTopLeft);
		boxFrontTopRight		= manipulator.transform.transform.TransformPoint(boxFrontTopRight);
		boxFrontBottomLeft		= manipulator.transform.transform.TransformPoint(boxFrontBottomLeft);
		boxFrontBottomRight		= manipulator.transform.transform.TransformPoint(boxFrontBottomRight);
		boxBackTopLeft			= manipulator.transform.transform.TransformPoint(boxBackTopLeft);
		boxBackTopRight			= manipulator.transform.transform.TransformPoint(boxBackTopRight);
		boxBackBottomLeft		= manipulator.transform.transform.TransformPoint(boxBackBottomLeft);
		boxBackBottomRight		= manipulator.transform.transform.TransformPoint(boxBackBottomRight);
		
		boxFrontDot				= manipulator.transform.transform.TransformPoint(boxFrontDot);
		boxLeftDot				= manipulator.transform.transform.TransformPoint(boxLeftDot);
		boxUpDot				= manipulator.transform.transform.TransformPoint(boxUpDot);
		
		// Draw front lines
		Handles.DrawLine(boxFrontTopLeft, boxFrontTopRight);
		Handles.DrawLine(boxFrontTopRight, boxFrontBottomRight);
		Handles.DrawLine(boxFrontBottomLeft, boxFrontTopLeft);
		Handles.DrawLine(boxFrontBottomRight, boxFrontBottomLeft);
		
		// Draw back lines
		Handles.DrawLine(boxBackTopLeft, boxBackTopRight);
		Handles.DrawLine(boxBackTopRight, boxBackBottomRight);
		Handles.DrawLine(boxBackBottomLeft, boxBackTopLeft);
		Handles.DrawLine(boxBackBottomRight, boxBackBottomLeft);
		
		// Draw front to back lines
		Handles.DrawLine(boxFrontTopLeft, boxBackTopLeft);
		Handles.DrawLine(boxFrontTopRight, boxBackTopRight);
		Handles.DrawLine(boxFrontBottomLeft, boxBackBottomLeft);
		Handles.DrawLine(boxFrontBottomRight, boxBackBottomRight);
		
		// Draw extents handles
		boxFrontDot = Handles.Slider(boxFrontDot, manipulator.transform.right, HandleUtility.GetHandleSize(boxFrontDot)*.03f, Handles.DotCap, 0f);
		boxUpDot = Handles.Slider(boxUpDot, manipulator.transform.up, HandleUtility.GetHandleSize(boxUpDot)*.03f, Handles.DotCap, 0f);
		boxLeftDot = Handles.Slider(boxLeftDot, manipulator.transform.forward, HandleUtility.GetHandleSize(boxLeftDot)*.03f, Handles.DotCap, 0f);
		
		manipulator.bounds.extents = new Vector3(
			manipulator.transform.transform.InverseTransformPoint(boxFrontDot).x-manipulator.bounds.center.x,
			manipulator.transform.transform.InverseTransformPoint(boxUpDot).y-manipulator.bounds.center.y,
			manipulator.transform.transform.InverseTransformPoint(boxLeftDot).z-manipulator.bounds.center.z
			);	
	}
	
	public static void RenderPlaygroundSettings () {
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");
		EditorGUILayout.BeginVertical(boxStyle);
		
		if (playgroundSettings==null) {
			playgroundSettings = PlaygroundSettingsC.GetReference();
			playgroundLanguage = PlaygroundSettingsC.GetLanguage();
		}
		playgroundSettings.playgroundManagerFoldout = GUILayout.Toggle(playgroundSettings.playgroundManagerFoldout, playgroundLanguage.playgroundManager, EditorStyles.foldout);
		if (playgroundSettings.playgroundManagerFoldout) {
			
			EditorGUILayout.BeginVertical(boxStyle);
			if (playgroundScriptReference==null) {
				playgroundScriptReference = GameObject.FindObjectOfType<PlaygroundC>();
				if (playgroundScriptReference)
					Initialize(playgroundScriptReference);
			}
			
			if (playgroundSettings.playgroundFoldout && playgroundScriptReference!=null) {
				playground.Update();
				
				// Particle System List
				if (GUILayout.Button(playgroundLanguage.particleSystems+" ("+playgroundScriptReference.particleSystems.Count+")", EditorStyles.toolbarDropDown)) playgroundSettings.particleSystemsFoldout=!playgroundSettings.particleSystemsFoldout;
				if (playgroundSettings.particleSystemsFoldout) {
					
					EditorGUILayout.Separator();
					
					if (playgroundScriptReference.particleSystems.Count>0) {
						for (int ps = 0; ps<playgroundScriptReference.particleSystems.Count; ps++) {
							
							EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(26));
							EditorGUILayout.BeginHorizontal();
							
							if (playgroundScriptReference.particleSystems[ps].particleSystemGameObject == Selection.activeGameObject) GUILayout.BeginHorizontal(boxStyle);
							
							GUILayout.Label(ps.ToString(), EditorStyles.miniLabel, new GUILayoutOption[]{GUILayout.Width(18)});
							if (GUILayout.Button(playgroundScriptReference.particleSystems[ps].particleSystemGameObject.name+" ("+playgroundScriptReference.particleSystems[ps].particleCount+")", EditorStyles.label)) {
								Selection.activeGameObject = playgroundScriptReference.particleSystems[ps].particleSystemGameObject;
							}
							EditorGUILayout.Separator();
							if (playgroundScriptReference.particleSystems[ps].threadMethod!=ThreadMethodLocal.Inherit) {
								GUILayout.Label(playgroundLanguage.thread+": "+playgroundScriptReference.particleSystems[ps].threadMethod.ToString(), EditorStyles.miniLabel);
							}
							GUI.enabled = (playgroundScriptReference.particleSystems.Count>1);
							if(GUILayout.Button(playgroundLanguage.upSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								particleSystems.MoveArrayElement(ps, ps==0?playgroundScriptReference.particleSystems.Count-1:ps-1);
							}
							if(GUILayout.Button(playgroundLanguage.downSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								particleSystems.MoveArrayElement(ps, ps<playgroundScriptReference.particleSystems.Count-1?ps+1:0);
							}
							GUI.enabled = true;
							
							// Clone
							if(GUILayout.Button("+", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								GameObject ppsDuplicateGo = Instantiate(playgroundScriptReference.particleSystems[ps].particleSystemGameObject, playgroundScriptReference.particleSystems[ps].particleSystemTransform.position, playgroundScriptReference.particleSystems[ps].particleSystemTransform.rotation) as GameObject;
								PlaygroundParticlesC ppsDuplicate = ppsDuplicateGo.GetComponent<PlaygroundParticlesC>();
								
								// Add this to Manager
								if (PlaygroundC.reference!=null) {
									PlaygroundC.particlesQuantity++;
									ppsDuplicate.particleSystemId = PlaygroundC.particlesQuantity;
									if (PlaygroundC.reference.autoGroup && ppsDuplicate.particleSystemTransform.parent==null)
										ppsDuplicate.particleSystemTransform.parent = PlaygroundC.referenceTransform;
								}
							}
							if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								if (EditorUtility.DisplayDialog(
									playgroundLanguage.remove+" "+playgroundScriptReference.particleSystems[ps].particleSystemGameObject.name+"?",
									playgroundLanguage.removeParticlePlaygroundSystem, 
									playgroundLanguage.yes, playgroundLanguage.no)) {
									if (Selection.activeGameObject==playgroundScriptReference.particleSystems[ps].particleSystemGameObject)
										Selection.activeGameObject = PlaygroundC.referenceTransform.gameObject;
									PlaygroundC.Destroy(playgroundScriptReference.particleSystems[ps]);
									playground.ApplyModifiedProperties();
									return;
								}
							}
							
							if (playgroundScriptReference.particleSystems[ps].particleSystemGameObject == Selection.activeGameObject) GUILayout.EndHorizontal();
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.EndVertical();
						}
					} else {
						EditorGUILayout.HelpBox(playgroundLanguage.noParticleSystems, MessageType.Info);
					}
					
					if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
						PlaygroundParticlesC createdParticles = PlaygroundC.Particle();
						Selection.activeGameObject = createdParticles.particleSystemGameObject;
					}
					
					EditorGUILayout.Separator();
				}
				
				// Manipulators
				if (GUILayout.Button(playgroundLanguage.globalManipulators+" ("+playgroundScriptReference.manipulators.Count+")", EditorStyles.toolbarDropDown)) playgroundSettings.globalManipulatorsFoldout=!playgroundSettings.globalManipulatorsFoldout;
				if (playgroundSettings.globalManipulatorsFoldout) {
					
					EditorGUILayout.Separator();
					
					if (manipulators.arraySize>0) {
						
						for (int i = 0; i<manipulators.arraySize; i++) {
							if (!playgroundScriptReference.manipulators[i].enabled)
								GUI.contentColor = Color.gray;
							string mName;
							if (playgroundScriptReference.manipulators[i].transform.available) {
								mName = playgroundScriptReference.manipulators[i].transform.transform.name;
								if (mName.Length>24)
									mName = mName.Substring(0, 24)+"...";
							} else {
								GUI.color = Color.red;
								mName = "("+playgroundLanguage.missingTransform+")";
							}
							
							EditorGUILayout.BeginVertical(boxStyle);
							
							EditorGUILayout.BeginHorizontal();
							
							GUILayout.Label(i.ToString(), EditorStyles.miniLabel, GUILayout.Width(18));
							playgroundScriptReference.manipulators[i].unfolded = GUILayout.Toggle(playgroundScriptReference.manipulators[i].unfolded, ManipulatorTypeName(playgroundScriptReference.manipulators[i].type), EditorStyles.foldout, GUILayout.Width(Screen.width/4));
							if (playgroundScriptReference.manipulators[i].transform.available) {
								if (GUILayout.Button(" ("+mName+")", EditorStyles.label)) {
									Selection.activeGameObject = playgroundScriptReference.manipulators[i].transform.transform.gameObject;
								}
							} else {
								GUILayout.Button(ManipulatorTypeName(playgroundScriptReference.manipulators[i].type)+" ("+playgroundLanguage.missingTransform+")", EditorStyles.label);
							}
							GUI.contentColor = Color.white;
							EditorGUILayout.Separator();
							GUI.enabled = (playgroundScriptReference.manipulators.Count>1);
							if(GUILayout.Button(playgroundLanguage.upSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								manipulators.MoveArrayElement(i, i==0?playgroundScriptReference.manipulators.Count-1:i-1);
							}
							if(GUILayout.Button(playgroundLanguage.downSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								manipulators.MoveArrayElement(i, i<playgroundScriptReference.manipulators.Count-1?i+1:0);
							}
							GUI.enabled = true;
							if(GUILayout.Button("+", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								PlaygroundC.reference.manipulators.Add(playgroundScriptReference.manipulators[i].Clone());
							}
							if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
								
								if (EditorUtility.DisplayDialog(
									playgroundLanguage.remove+" "+ManipulatorTypeName(playgroundScriptReference.manipulators[i].type)+" "+playgroundLanguage.manipulator+" "+i+"?",
									playgroundLanguage.removeManipulator+" "+mName+"? "+playgroundLanguage.gameObjectIntact, 
									playgroundLanguage.yes, playgroundLanguage.no)) {
									manipulators.DeleteArrayElementAtIndex(i);
									playground.ApplyModifiedProperties();
									return;
								}
							}
							
							GUI.color = Color.white;
							
							EditorGUILayout.EndHorizontal();
							
							if (playgroundScriptReference.manipulators[i].unfolded && i<manipulators.arraySize) {
								RenderManipulatorSettings(playgroundScriptReference.manipulators[i], manipulators.GetArrayElementAtIndex(i), true);
							}
							
							GUI.enabled = true;
							EditorGUILayout.Separator();
							EditorGUILayout.EndVertical();
						}
						
					} else {
						EditorGUILayout.HelpBox(playgroundLanguage.noManipulators, MessageType.Info);
					}
					
					if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
						
						if (Selection.gameObjects.Length>0 && Selection.activeGameObject.transform && Selection.activeTransform!=null) {
							Transform mTrans = new GameObject().transform;
							mTrans.parent = PlaygroundC.referenceTransform;
							mTrans.position = Selection.activeGameObject.transform.position+Vector3.up;
							if (manipulators.arraySize>0)
								mTrans.name = "Global Manipulator "+(manipulators.arraySize+1);
							else mTrans.name = "Global Manipulator";
							PlaygroundC.ManipulatorObject(mTrans);
						} else
							manipulators.InsertArrayElementAtIndex(manipulators.arraySize);
						SceneView.RepaintAll();
					}
					
					EditorGUILayout.Separator();
					
				}
				
				// Advanced Settings
				if (GUILayout.Button(playgroundLanguage.advanced, EditorStyles.toolbarDropDown)) playgroundSettings.advancedSettingsFoldout=!playgroundSettings.advancedSettingsFoldout;
				if (playgroundSettings.advancedSettingsFoldout) {
					
					showSnapshotsSettings = PlaygroundC.reference.showSnapshotsInHierarchy;
					
					EditorGUILayout.Separator();
					EditorGUILayout.PropertyField(calculate, new GUIContent(playgroundLanguage.calculateParticles, playgroundLanguage.calculateParticlesDescription));
					EditorGUILayout.PropertyField(autoGroup, new GUIContent(playgroundLanguage.groupAutomatically, playgroundLanguage.groupAutomaticallyDescription));
					EditorGUILayout.PropertyField(buildZeroAlphaPixels, new GUIContent(playgroundLanguage.buildZeroAlphaPixels, playgroundLanguage.buildZeroAlphaPixelsDescription));
					EditorGUILayout.PropertyField(drawGizmos, new GUIContent(playgroundLanguage.sceneGizmos, playgroundLanguage.sceneGizmosDescription));
					GUI.enabled = drawGizmos.boolValue;
					EditorGUILayout.PropertyField(drawSourcePositions, new GUIContent(playgroundLanguage.sourcePositions, playgroundLanguage.sourcePositionsDescription));
					EditorGUILayout.PropertyField(drawSplinePreview, new GUIContent(playgroundLanguage.drawSplinePreview, playgroundLanguage.drawSplinePreviewDescription));
					PlaygroundSpline.drawSplinePreviews = drawSplinePreview.boolValue;
					GUI.enabled = true;
					EditorGUILayout.PropertyField(drawWireframe, new GUIContent(playgroundLanguage.wireframes, playgroundLanguage.wireframesDescription));
					EditorGUILayout.PropertyField(paintToolbox, new GUIContent(playgroundLanguage.paintToolbox, playgroundLanguage.paintToolboxDescription));
					EditorGUILayout.PropertyField(showShuriken, new GUIContent(playgroundLanguage.showShuriken, playgroundLanguage.showShurikenDescription));
					EditorGUILayout.PropertyField(showSnapshots, new GUIContent(playgroundLanguage.advancedSnapshots, playgroundLanguage.advancedSnapshotsDescription));
					EditorGUILayout.PropertyField(pixelFilterMode, new GUIContent(playgroundLanguage.pixelFilterMode, playgroundLanguage.pixelFilterModeDescription));
					EditorGUILayout.PropertyField(globalTimeScale, new GUIContent(playgroundLanguage.globalTimeScale, playgroundLanguage.globalTimeScaleDescription));
					EditorGUILayout.Separator();
					GUILayout.BeginVertical(boxStyle);
					EditorGUILayout.LabelField(playgroundLanguage.multithreading+" ("+PlaygroundC.ActiveThreads().ToString()+" "+playgroundLanguage.activeThreads+", "+PlaygroundC.ProcessorCount()+" "+playgroundLanguage.processors+")", EditorStyles.miniLabel);
					EditorGUILayout.PropertyField(threadPool, new GUIContent(playgroundLanguage.threadPoolMethod, playgroundLanguage.threadPoolMethodDescription));
					EditorGUILayout.PropertyField(threads, new GUIContent(playgroundLanguage.particleThreadMethod, playgroundLanguage.threadMethodDescription));
					EditorGUILayout.PropertyField(threadsTurbulence, new GUIContent(playgroundLanguage.turbulenceThreadMethod, playgroundLanguage.threadMethodDescription));
					EditorGUILayout.PropertyField(threadsSkinned, new GUIContent(playgroundLanguage.skinnedMeshThreadMethod, playgroundLanguage.threadMethodDescription));
					GUI.enabled = playgroundScriptReference.threadMethod==ThreadMethod.Automatic; 
					EditorGUILayout.PropertyField(maxThreads, new GUIContent(playgroundLanguage.maxThreads, playgroundLanguage.maxThreadsDescription));
					GUI.enabled = true;
					switch (playgroundScriptReference.threadMethod) {
					case ThreadMethod.NoThreads:EditorGUILayout.HelpBox(playgroundLanguage.threadInfo01, MessageType.Info);break;
					case ThreadMethod.OnePerSystem:EditorGUILayout.HelpBox(playgroundLanguage.threadInfo02, MessageType.Info);break;
					case ThreadMethod.OneForAll:EditorGUILayout.HelpBox(playgroundLanguage.threadInfo03, MessageType.Info);break;
					case ThreadMethod.Automatic:EditorGUILayout.HelpBox(playgroundLanguage.threadInfo04, MessageType.Info);break;
					}
					GUILayout.EndHorizontal();
					
					EditorGUILayout.Separator();
					
					// Update snapshot visibility
					if (showSnapshots.boolValue != showSnapshotsSettings) {
						UpdateSnapshots();
					}
					
					// Time reset
					GUILayout.BeginHorizontal();
					EditorGUILayout.PrefixLabel(playgroundLanguage.timeSimulation);
					if(GUILayout.Button(playgroundLanguage.reset, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
						PlaygroundC.TimeReset();
						for (int p = 0; p<playgroundScriptReference.particleSystems.Count; p++)
							playgroundScriptReference.particleSystems[p].Boot();
					}
					GUILayout.EndHorizontal();
					
				}
				
				
				
				playground.ApplyModifiedProperties();
				EditorGUILayout.EndVertical();
			} else {
				EditorGUILayout.HelpBox(playgroundLanguage.noPlaygroundManager, MessageType.Info);
				if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
					PlaygroundC.ResourceInstantiate("Playground Manager");
				}
				EditorGUILayout.EndVertical();
			}
			
		}
		EditorGUILayout.EndVertical();
	}
	
	public static void UpdateSnapshots () {
		foreach (PlaygroundParticlesC p in PlaygroundC.reference.particleSystems) {
			foreach (PlaygroundSave snapshot in p.snapshots) {
				snapshot.settings.gameObject.hideFlags = PlaygroundC.reference.showSnapshotsInHierarchy?HideFlags.None:HideFlags.HideInHierarchy;
				if (Selection.activeGameObject==snapshot.settings.gameObject)
					Selection.activeGameObject = null;
			}
		}
		EditorApplication.RepaintHierarchyWindow();
	}
	
	public static void RenderManipulatorSettings (ManipulatorObjectC thisManipulator, SerializedProperty serializedManipulator, bool isPlayground) {
		SerializedProperty serializedManipulatorAffects = serializedManipulator.FindPropertyRelative("affects");
		SerializedProperty serializedManipulatorSize;
		SerializedProperty serializedManipulatorStrength;
		SerializedProperty serializedManipulatorStrengthSmoothing;
		SerializedProperty serializedManipulatorStrengthDistance;
		SerializedProperty serializedManipulatorApplyLifetimeStrength;
		SerializedProperty serializedManipulatorLifetimeStrength;
		
		thisManipulator.enabled = EditorGUILayout.ToggleLeft(playgroundLanguage.enabled, thisManipulator.enabled);
		GUI.enabled = thisManipulator.enabled;
		
		EditorGUILayout.PropertyField(serializedManipulator.FindPropertyRelative("transform").FindPropertyRelative("transform"), new GUIContent(playgroundLanguage.transform));
		if (thisManipulator.transform.available && thisManipulator.transform.transform!=null) {
			EditorGUI.indentLevel++;
			thisManipulator.transform.transform.position = EditorGUILayout.Vector3Field(playgroundLanguage.position, thisManipulator.transform.transform.position);
			thisManipulator.transform.transform.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field(playgroundLanguage.rotation, thisManipulator.transform.transform.rotation.eulerAngles));
			thisManipulator.transform.transform.localScale = EditorGUILayout.Vector3Field(playgroundLanguage.scale, thisManipulator.transform.transform.localScale);
			EditorGUI.indentLevel--;
		}
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(serializedManipulator.FindPropertyRelative("type"), new GUIContent(playgroundLanguage.type));
		
		// Render properties
		if (thisManipulator.type==MANIPULATORTYPEC.Property)
		{
			GUI.enabled = thisManipulator.enabled;
			RenderManipulatorProperty(thisManipulator, thisManipulator.property, serializedManipulator.FindPropertyRelative("property"));
		}
		if (thisManipulator.type==MANIPULATORTYPEC.Combined) {
			if (thisManipulator.properties.Count>0) {
				SerializedProperty serializedManipulatorProperties = serializedManipulator.FindPropertyRelative("properties");
				int prevPropertyCount = thisManipulator.properties.Count;
				for (int i = 0; i<thisManipulator.properties.Count; i++) {
					if (thisManipulator.properties.Count!=prevPropertyCount) return;
					EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(26));
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label(i.ToString(), EditorStyles.miniLabel, GUILayout.Width(18));
					thisManipulator.properties[i].unfolded = GUILayout.Toggle(thisManipulator.properties[i].unfolded, thisManipulator.properties[i].type.ToString(), EditorStyles.foldout);
					
					EditorGUILayout.Separator();
					GUI.enabled = (thisManipulator.enabled&&thisManipulator.properties.Count>1);
					if(GUILayout.Button(playgroundLanguage.upSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						serializedManipulatorProperties.MoveArrayElement(i, i==0?thisManipulator.properties.Count-1:i-1);
					}
					if(GUILayout.Button(playgroundLanguage.downSymbol, EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						serializedManipulatorProperties.MoveArrayElement(i, i<thisManipulator.properties.Count-1?i+1:0);
					}
					GUI.enabled = thisManipulator.enabled;
					if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
						thisManipulator.properties.RemoveAt(i);
						return;
					}
					EditorGUILayout.EndHorizontal();
					
					if (thisManipulator.properties[i].unfolded)
						RenderManipulatorProperty(thisManipulator, thisManipulator.properties[i], serializedManipulatorProperties.GetArrayElementAtIndex(i));
					EditorGUILayout.EndVertical();
				}
			} else {
				EditorGUILayout.HelpBox(playgroundLanguage.noProperties, MessageType.Info);
			}
			if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
				thisManipulator.properties.Add(new ManipulatorPropertyC());
			}
		}
		GUI.enabled = thisManipulator.enabled;
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(serializedManipulator.FindPropertyRelative("shape"), new GUIContent(playgroundLanguage.shape));
		
		if (thisManipulator.shape==MANIPULATORSHAPEC.Sphere) {
			serializedManipulatorSize = serializedManipulator.FindPropertyRelative("size");
			serializedManipulatorSize.floatValue = EditorGUILayout.Slider(playgroundLanguage.size, serializedManipulatorSize.floatValue, 0, playgroundSettings.maximumAllowedManipulatorSize);
		} else if (thisManipulator.shape==MANIPULATORSHAPEC.Box) {
			EditorGUILayout.PropertyField(serializedManipulator.FindPropertyRelative("bounds"), new GUIContent(playgroundLanguage.bounds));
		}
		
		EditorGUILayout.Separator();
		serializedManipulatorStrength = serializedManipulator.FindPropertyRelative("strength");
		serializedManipulatorStrength.floatValue = EditorGUILayout.Slider(playgroundLanguage.manipulatorStrength, serializedManipulatorStrength.floatValue, 0, playgroundSettings.maximumAllowedManipulatorStrength);
		EditorGUI.indentLevel++;
		serializedManipulatorStrengthSmoothing = serializedManipulator.FindPropertyRelative("strengthSmoothing");
		serializedManipulatorStrengthSmoothing.floatValue = EditorGUILayout.Slider(playgroundLanguage.smoothingEffect, serializedManipulatorStrengthSmoothing.floatValue, 0, playgroundSettings.maximumAllowedManipulatorStrengthEffectors);
		serializedManipulatorStrengthDistance = serializedManipulator.FindPropertyRelative("strengthDistanceEffect");
		serializedManipulatorStrengthDistance.floatValue = EditorGUILayout.Slider(playgroundLanguage.distanceEffect, serializedManipulatorStrengthDistance.floatValue, 0, playgroundSettings.maximumAllowedManipulatorStrengthEffectors);
		EditorGUI.indentLevel--;
		EditorGUILayout.BeginHorizontal();
		serializedManipulatorApplyLifetimeStrength = serializedManipulator.FindPropertyRelative("applyParticleLifetimeStrength");
		serializedManipulatorLifetimeStrength = serializedManipulator.FindPropertyRelative("particleLifetimeStrength");
		serializedManipulatorApplyLifetimeStrength.boolValue = EditorGUILayout.ToggleLeft(playgroundLanguage.particleLifetimeStrength, serializedManipulatorApplyLifetimeStrength.boolValue, GUILayout.Width (150));
		GUILayout.FlexibleSpace();
		GUI.enabled = serializedManipulatorApplyLifetimeStrength.boolValue && thisManipulator.enabled;
		EditorGUILayout.PropertyField(serializedManipulatorLifetimeStrength, new GUIContent(""), GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-16));
		GUI.enabled = thisManipulator.enabled;
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Separator();
		
		GUILayout.BeginHorizontal();
		thisManipulator.applyLifetimeFilter = EditorGUILayout.ToggleLeft (playgroundLanguage.lifetimeFilter, thisManipulator.applyLifetimeFilter, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
		float minFilter = thisManipulator.lifetimeFilterMinimum;
		float maxFilter = thisManipulator.lifetimeFilterMaximum;
		EditorGUILayout.MinMaxSlider (ref minFilter, ref maxFilter, 0f, 1f);
		thisManipulator.lifetimeFilterMinimum = EditorGUILayout.FloatField(Mathf.Clamp01(minFilter), GUILayout.Width(50));
		thisManipulator.lifetimeFilterMaximum = EditorGUILayout.FloatField(Mathf.Clamp01(maxFilter), GUILayout.Width(50));
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		thisManipulator.applyParticleFilter = EditorGUILayout.ToggleLeft (playgroundLanguage.particleFilter, thisManipulator.applyParticleFilter, GUILayout.Width(Mathf.CeilToInt(Screen.width/1.805f)-110));
		minFilter = thisManipulator.particleFilterMinimum;
		maxFilter = thisManipulator.particleFilterMaximum;
		EditorGUILayout.MinMaxSlider (ref minFilter, ref maxFilter, 0f, 1f);
		thisManipulator.particleFilterMinimum = EditorGUILayout.FloatField(Mathf.Clamp01(minFilter), GUILayout.Width(50));
		thisManipulator.particleFilterMaximum = EditorGUILayout.FloatField(Mathf.Clamp01(maxFilter), GUILayout.Width(50));
		GUILayout.EndHorizontal();
		
		EditorGUILayout.Separator();

		if (thisManipulator.shape != MANIPULATORSHAPEC.Infinite)
			thisManipulator.inverseBounds = EditorGUILayout.Toggle(playgroundLanguage.inverseBounds, thisManipulator.inverseBounds);
		
		EditorGUILayout.Separator();
		
		// Axis constraints
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(playgroundLanguage.axisConstraints, GUILayout.Width(Mathf.FloorToInt(Screen.width/2.2f)-46));
		
		GUILayout.Label("X", GUILayout.Width(10));
		thisManipulator.axisConstraints.x = EditorGUILayout.Toggle(thisManipulator.axisConstraints.x, GUILayout.Width(16));
		GUILayout.Label("Y", GUILayout.Width(10));
		thisManipulator.axisConstraints.y = EditorGUILayout.Toggle(thisManipulator.axisConstraints.y, GUILayout.Width(16));
		GUILayout.Label("Z", GUILayout.Width(10));
		thisManipulator.axisConstraints.z = EditorGUILayout.Toggle(thisManipulator.axisConstraints.z, GUILayout.Width(16));
		GUILayout.EndHorizontal();
		
		if (isPlayground) {
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(serializedManipulatorAffects, new GUIContent(playgroundLanguage.affects));
		}
		
		EditorGUILayout.Separator();
		
		GUILayout.BeginVertical(boxStyle);
		GUILayout.BeginHorizontal();
		thisManipulator.sendEventsUnfolded = GUILayout.Toggle(thisManipulator.sendEventsUnfolded, playgroundLanguage.events, EditorStyles.foldout, GUILayout.ExpandWidth(false));
		if (thisManipulator.trackParticles) {
			thisManipulator.sendEventsUnfolded =  GUILayout.Toggle(thisManipulator.sendEventsUnfolded, thisManipulator.particles.Count+" "+playgroundLanguage.particles, EditorStyles.label, GUILayout.ExpandWidth(true));
		} else {
			thisManipulator.sendEventsUnfolded =  GUILayout.Toggle(thisManipulator.sendEventsUnfolded, "("+playgroundLanguage.off+")", EditorStyles.label, GUILayout.ExpandWidth(true));
		}
		GUILayout.EndHorizontal();
		if (thisManipulator.sendEventsUnfolded) {
			thisManipulator.trackParticles = EditorGUILayout.Toggle (playgroundLanguage.trackParticles, thisManipulator.trackParticles);
			EditorGUI.indentLevel++;
			GUI.enabled = thisManipulator.trackParticles;
			thisManipulator.trackingMethod = (TrackingMethod)EditorGUILayout.EnumPopup(playgroundLanguage.trackingMethod, thisManipulator.trackingMethod);
			thisManipulator.sendEventEnter = EditorGUILayout.Toggle (playgroundLanguage.sendEnterEvents, thisManipulator.sendEventEnter);
			thisManipulator.sendEventExit = EditorGUILayout.Toggle (playgroundLanguage.sendExitEvents, thisManipulator.sendEventExit);
			thisManipulator.sendEventBirth = EditorGUILayout.Toggle (playgroundLanguage.sendBirthEvents, thisManipulator.sendEventBirth);
			thisManipulator.sendEventDeath = EditorGUILayout.Toggle (playgroundLanguage.sendDeathEvents, thisManipulator.sendEventDeath);
			thisManipulator.sendEventCollision = EditorGUILayout.Toggle (playgroundLanguage.sendCollisionEvents, thisManipulator.sendEventCollision);
			GUI.enabled = true;
			EditorGUI.indentLevel--;
		}
		GUILayout.EndVertical();
		
	}
	
	public static void RenderManipulatorProperty (ManipulatorObjectC thisManipulator, ManipulatorPropertyC thisManipulatorProperty, SerializedProperty serializedManipulatorProperty) {
		if (thisManipulatorProperty == null) thisManipulatorProperty = new ManipulatorPropertyC();
		
		thisManipulatorProperty.type = (MANIPULATORPROPERTYTYPEC)EditorGUILayout.EnumPopup(playgroundLanguage.propertyType, thisManipulatorProperty.type);
		bool guiEnabled = GUI.enabled;
		bool hasRange = thisManipulator.shape != MANIPULATORSHAPEC.Infinite;
		
		EditorGUILayout.Separator();
		
		// Velocity
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Velocity || thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.AdditiveVelocity) {
			thisManipulatorProperty.velocity = EditorGUILayout.Vector3Field(playgroundLanguage.particleVelocity, thisManipulatorProperty.velocity);
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.velocityStrength, thisManipulatorProperty.strength);
			thisManipulatorProperty.useLocalRotation = EditorGUILayout.Toggle(playgroundLanguage.localRotation, thisManipulatorProperty.useLocalRotation);
		} else 
			// Color
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Color) {
			thisManipulatorProperty.color = EditorGUILayout.ColorField(playgroundLanguage.particleColor, thisManipulatorProperty.color);
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.colorStrength, thisManipulatorProperty.strength);
			thisManipulatorProperty.onlyColorInRange = EditorGUILayout.Toggle(playgroundLanguage.onlyColorInRange, thisManipulatorProperty.onlyColorInRange);
			thisManipulatorProperty.keepColorAlphas = EditorGUILayout.Toggle(playgroundLanguage.keepColorAlphas, thisManipulatorProperty.keepColorAlphas);
		} else
			// Size
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Size) {
			thisManipulatorProperty.size = EditorGUILayout.FloatField(playgroundLanguage.particleSize, thisManipulatorProperty.size);
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.sizeStrength, thisManipulatorProperty.strength);
			thisManipulatorProperty.onlySizeInRange = EditorGUILayout.Toggle(playgroundLanguage.onlySizeInRange, thisManipulatorProperty.onlySizeInRange);
		} else
			// Target
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Target) {
			
			// Target List
			bool hasNull = false;
			EditorGUILayout.BeginVertical(boxStyle);
			playgroundSettings.manipulatorTargetsFoldout = GUILayout.Toggle(playgroundSettings.manipulatorTargetsFoldout, playgroundLanguage.targets+" ("+thisManipulatorProperty.targets.Count+")", EditorStyles.foldout);
			if (playgroundSettings.manipulatorTargetsFoldout) {
				if (thisManipulatorProperty.targets.Count>0) {
					for (int t = 0; t<thisManipulatorProperty.targets.Count; t++) {
						EditorGUILayout.BeginVertical(boxStyle, GUILayout.MinHeight(26));
						EditorGUILayout.BeginHorizontal();
						
						GUILayout.Label(t.ToString(), EditorStyles.miniLabel, GUILayout.Width(18));
						thisManipulatorProperty.targets[t].transform = EditorGUILayout.ObjectField("", thisManipulatorProperty.targets[t].transform, typeof(Transform), true) as Transform;
						if (!thisManipulatorProperty.targets[t].available) hasNull = true;
						
						EditorGUILayout.Separator();
						if(GUILayout.Button("-", EditorStyles.toolbarButton, new GUILayoutOption[]{GUILayout.Width(18), GUILayout.Height(16)})){
							thisManipulatorProperty.targets.RemoveAt(t);
						}
						
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndVertical();
					}
				} else {
					EditorGUILayout.HelpBox(playgroundLanguage.noTargets, MessageType.Info);
				}
				
				if (hasNull)
					EditorGUILayout.HelpBox(playgroundLanguage.allTargets, MessageType.Warning);
				
				if(GUILayout.Button(playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
					PlaygroundTransformC newPlaygroundTransform = new PlaygroundTransformC();
					newPlaygroundTransform.transform = thisManipulator.transform.transform;
					newPlaygroundTransform.Update ();
					thisManipulatorProperty.targets.Add(newPlaygroundTransform);
				}
				EditorGUILayout.Separator();
			}
			EditorGUILayout.EndVertical();
			
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.targetStrength, thisManipulatorProperty.strength);
			if (hasRange)
				thisManipulatorProperty.onlyPositionInRange = EditorGUILayout.Toggle(playgroundLanguage.onlyPositionInRange, thisManipulatorProperty.onlyPositionInRange);
			thisManipulatorProperty.zeroVelocityStrength = EditorGUILayout.Slider(playgroundLanguage.manipulatorZeroVelocityStrength, thisManipulatorProperty.zeroVelocityStrength, 0, playgroundSettings.maximumAllowedManipulatorZeroVelocity);
		} else
			// Death
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Death) {
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.deathStrength, thisManipulatorProperty.strength);
		} else
			// Attractor
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Attractor) {
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.attractorStrength, thisManipulatorProperty.strength);
		} else
			// Gravitational
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Gravitational) {
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.gravitationalStrength, thisManipulatorProperty.strength);
		} else
			// Repellent
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Repellent) {
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.repellentStrength, thisManipulatorProperty.strength);
		} else 
			// Vortex
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Vortex) {
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.vortexStrength, thisManipulatorProperty.strength);
		} else
			// Turbulence
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Turbulence) {
			thisManipulatorProperty.turbulenceType = (TURBULENCETYPE)EditorGUILayout.EnumPopup (playgroundLanguage.turbulenceType, thisManipulatorProperty.turbulenceType);
			EditorGUI.indentLevel++;
			thisManipulatorProperty.strength = EditorGUILayout.Slider(playgroundLanguage.strength, thisManipulatorProperty.strength, 0f, playgroundSettings.maximumAllowedTurbulenceStrength);
			thisManipulatorProperty.turbulenceScale = EditorGUILayout.Slider(playgroundLanguage.scale, thisManipulatorProperty.turbulenceScale, 0f, playgroundSettings.maximumAllowedTurbulenceScale);
			thisManipulatorProperty.turbulenceTimeScale = EditorGUILayout.Slider(playgroundLanguage.timeScale, thisManipulatorProperty.turbulenceTimeScale, 0f, playgroundSettings.maximumAllowedTurbulenceTimeScale);
			EditorGUILayout.BeginHorizontal();
			thisManipulatorProperty.turbulenceApplyLifetimeStrength = EditorGUILayout.ToggleLeft (playgroundLanguage.lifetimeStrength, thisManipulatorProperty.turbulenceApplyLifetimeStrength, GUILayout.Width (140));
			GUILayout.FlexibleSpace();
			GUI.enabled = (thisManipulatorProperty.turbulenceApplyLifetimeStrength && thisManipulatorProperty.turbulenceType!=TURBULENCETYPE.None);
			serializedManipulatorProperty.FindPropertyRelative("turbulenceLifetimeStrength").animationCurveValue = EditorGUILayout.CurveField(serializedManipulatorProperty.FindPropertyRelative("turbulenceLifetimeStrength").animationCurveValue);
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
		} else
			// Lifetime Color
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.LifetimeColor) {
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("lifetimeColor"), new GUIContent(playgroundLanguage.lifetimeColor));
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.colorStrength, thisManipulatorProperty.strength);
			if (hasRange)
				thisManipulatorProperty.onlyColorInRange = EditorGUILayout.Toggle(playgroundLanguage.onlyColorInRange, thisManipulatorProperty.onlyColorInRange);
		} else
			// Mesh target
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.MeshTarget) {
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("meshTarget").FindPropertyRelative("gameObject"), new GUIContent(playgroundLanguage.meshTarget));
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.LabelField(playgroundLanguage.proceduralOptions);
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("meshTargetIsProcedural"), new GUIContent(
				playgroundLanguage.meshVerticesUpdate,
				playgroundLanguage.meshVerticesUpdateDescription
				));
			
			GUI.enabled = guiEnabled&&serializedManipulatorProperty.FindPropertyRelative("meshTargetIsProcedural").boolValue;
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("meshTarget").FindPropertyRelative("updateNormals"), new GUIContent(
				playgroundLanguage.meshNormalsUpdate,
				playgroundLanguage.meshNormalsUpdateDescription
				));
			GUI.enabled = guiEnabled;
			GUILayout.EndVertical();
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("targetSorting"), new GUIContent(playgroundLanguage.targetSorting));
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.targetStrength, thisManipulatorProperty.strength);
			if (hasRange)
				thisManipulatorProperty.onlyPositionInRange = EditorGUILayout.Toggle(playgroundLanguage.onlyPositionInRange, thisManipulatorProperty.onlyPositionInRange);
			thisManipulatorProperty.zeroVelocityStrength = EditorGUILayout.Slider(playgroundLanguage.manipulatorZeroVelocityStrength, thisManipulatorProperty.zeroVelocityStrength, 0, playgroundSettings.maximumAllowedManipulatorZeroVelocity);
		} else
			// Skinned mesh target
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.SkinnedMeshTarget) {
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("skinnedMeshTarget").FindPropertyRelative("gameObject"), new GUIContent(playgroundLanguage.skinnedMeshTarget));
			GUILayout.BeginVertical(boxStyle);
			EditorGUILayout.LabelField(playgroundLanguage.proceduralOptions);
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("meshTargetIsProcedural"), new GUIContent(
				playgroundLanguage.meshVerticesUpdate,
				playgroundLanguage.meshVerticesUpdateDescription
				));
			GUI.enabled = guiEnabled&&serializedManipulatorProperty.FindPropertyRelative("meshTargetIsProcedural").boolValue;
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("skinnedMeshTarget").FindPropertyRelative("updateNormals"), new GUIContent(
				playgroundLanguage.meshNormalsUpdate,
				playgroundLanguage.meshNormalsUpdateDescription
				));
			GUI.enabled = guiEnabled;
			GUILayout.EndVertical();
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("targetSorting"), new GUIContent(playgroundLanguage.targetSorting));
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.targetStrength, thisManipulatorProperty.strength);
			if (hasRange)
				thisManipulatorProperty.onlyPositionInRange = EditorGUILayout.Toggle(playgroundLanguage.onlyPositionInRange, thisManipulatorProperty.onlyPositionInRange);
			thisManipulatorProperty.zeroVelocityStrength = EditorGUILayout.Slider(playgroundLanguage.manipulatorZeroVelocityStrength, thisManipulatorProperty.zeroVelocityStrength, 0, playgroundSettings.maximumAllowedManipulatorZeroVelocity);
		} else
			// State target
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.StateTarget) {
			if (thisManipulatorProperty.stateTarget.stateTransform==null)
				thisManipulatorProperty.stateTarget.stateTransform = thisManipulator.transform.transform;
			
			GUILayout.BeginVertical(boxStyle);
			Texture2D currentStateTexture = thisManipulatorProperty.stateTarget.stateTexture;
			Mesh currentStateMesh = thisManipulatorProperty.stateTarget.stateMesh;
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("stateTarget").FindPropertyRelative("stateTransform"), new GUIContent(playgroundLanguage.transform));
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("stateTarget").FindPropertyRelative("stateMesh"), new GUIContent(playgroundLanguage.mesh));
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("stateTarget").FindPropertyRelative("stateTexture"), new GUIContent(playgroundLanguage.texture));
			if (currentStateTexture!=thisManipulatorProperty.stateTarget.stateTexture || currentStateMesh!=thisManipulatorProperty.stateTarget.stateMesh)
				thisManipulatorProperty.stateTarget.Initialize();
			if (thisManipulatorProperty.stateTarget.stateTexture!=null)
				EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("stateTarget").FindPropertyRelative("stateDepthmap"), new GUIContent(playgroundLanguage.depthmap));
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("stateTarget").FindPropertyRelative("stateScale"), new GUIContent(playgroundLanguage.scale, playgroundLanguage.stateScaleDescription));
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("stateTarget").FindPropertyRelative("stateOffset"), new GUIContent(playgroundLanguage.offset, playgroundLanguage.stateOffsetDescription));
			if (thisManipulatorProperty.stateTarget.stateMesh==null) {
				GUILayout.BeginHorizontal();
				bool currentApplyChromaKey = thisManipulatorProperty.stateTarget.applyChromaKey;
				thisManipulatorProperty.stateTarget.applyChromaKey = EditorGUILayout.Toggle (playgroundLanguage.chromaKey, thisManipulatorProperty.stateTarget.applyChromaKey);
				GUI.enabled = guiEnabled&&thisManipulatorProperty.stateTarget.applyChromaKey;
				EditorGUIUtility.labelWidth = 1f;
				Color currentChroma = new Color(thisManipulatorProperty.stateTarget.chromaKey.r,thisManipulatorProperty.stateTarget.chromaKey.g, thisManipulatorProperty.stateTarget.chromaKey.b);
				thisManipulatorProperty.stateTarget.chromaKey = (Color32)EditorGUILayout.ColorField((Color)thisManipulatorProperty.stateTarget.chromaKey);
				EditorGUIUtility.labelWidth = 50f;
				float currentSpread = thisManipulatorProperty.stateTarget.chromaKeySpread;
				thisManipulatorProperty.stateTarget.chromaKeySpread = EditorGUILayout.Slider(playgroundLanguage.spread, thisManipulatorProperty.stateTarget.chromaKeySpread, 0, 1f);
				if (currentChroma!=new Color(thisManipulatorProperty.stateTarget.chromaKey.r,thisManipulatorProperty.stateTarget.chromaKey.g, thisManipulatorProperty.stateTarget.chromaKey.b) || currentSpread!=thisManipulatorProperty.stateTarget.chromaKeySpread || currentApplyChromaKey!=thisManipulatorProperty.stateTarget.applyChromaKey)
					thisManipulatorProperty.stateTarget.Initialize();
				GUI.enabled = guiEnabled;
				EditorGUIUtility.labelWidth = 0;
				GUILayout.EndHorizontal();
			}
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(playgroundLanguage.refresh, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))){
				thisManipulatorProperty.stateTarget.Initialize();
			}
			EditorGUILayout.Separator();
			EditorGUIUtility.labelWidth = 40f;
			EditorGUILayout.PrefixLabel(playgroundLanguage.points+":");
			EditorGUIUtility.labelWidth = 0;
			EditorGUILayout.SelectableLabel(thisManipulatorProperty.stateTarget.positionLength.ToString(), GUILayout.MaxWidth(80));
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			
			EditorGUILayout.PropertyField(serializedManipulatorProperty.FindPropertyRelative("targetSorting"), new GUIContent(playgroundLanguage.targetSorting));
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.targetStrength, thisManipulatorProperty.strength);
			if (hasRange)
				thisManipulatorProperty.onlyPositionInRange = EditorGUILayout.Toggle(playgroundLanguage.onlyPositionInRange, thisManipulatorProperty.onlyPositionInRange);
			thisManipulatorProperty.zeroVelocityStrength = EditorGUILayout.Slider(playgroundLanguage.manipulatorZeroVelocityStrength, thisManipulatorProperty.zeroVelocityStrength, 0, playgroundSettings.maximumAllowedManipulatorZeroVelocity);
		} else
			// Spline target
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.SplineTarget) {
			EditorGUILayout.BeginVertical(boxStyle);
			thisManipulatorProperty.splineTarget = (PlaygroundSpline)EditorGUILayout.ObjectField(playgroundLanguage.spline, thisManipulatorProperty.splineTarget, typeof(PlaygroundSpline), true);
			if (thisManipulatorProperty.splineTarget==null)
				EditorGUILayout.HelpBox(playgroundLanguage.newSplineMessage, MessageType.Info);
			else thisManipulatorProperty.splineTimeOffset = EditorGUILayout.Slider (playgroundLanguage.timeOffset, thisManipulatorProperty.splineTimeOffset, 0, 1f);
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button (playgroundLanguage.create, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false))) {
				PlaygroundSpline spline = PlaygroundC.CreateSpline(thisManipulatorProperty);
				spline.transform.parent = Selection.activeTransform;
				Selection.activeGameObject = thisManipulatorProperty.splineTarget.gameObject;
			}
			GUI.enabled = thisManipulatorProperty.splineTarget!=null;
			if (GUILayout.Button (playgroundLanguage.edit, EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
				Selection.activeGameObject = thisManipulatorProperty.splineTarget.gameObject;
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			EditorGUILayout.Separator();
			thisManipulatorProperty.splineTargetMethod = (SPLINETARGETMETHOD)EditorGUILayout.EnumPopup(playgroundLanguage.targetMethod, thisManipulatorProperty.splineTargetMethod);
			thisManipulatorProperty.strength = EditorGUILayout.FloatField(playgroundLanguage.targetStrength, thisManipulatorProperty.strength);
			if (hasRange)
				thisManipulatorProperty.onlyPositionInRange = EditorGUILayout.Toggle(playgroundLanguage.onlyPositionInRange, thisManipulatorProperty.onlyPositionInRange);
			thisManipulatorProperty.zeroVelocityStrength = EditorGUILayout.Slider(playgroundLanguage.manipulatorZeroVelocityStrength, thisManipulatorProperty.zeroVelocityStrength, 0, playgroundSettings.maximumAllowedManipulatorZeroVelocity);
		} else 
		if (thisManipulatorProperty.type==MANIPULATORPROPERTYTYPEC.Math) {
			thisManipulatorProperty.mathProperty.property = (MATHMANIPULATORPROPERTY)EditorGUILayout.EnumPopup(playgroundLanguage.particleProperty, thisManipulatorProperty.mathProperty.property);
			thisManipulatorProperty.mathProperty.type = (MATHMANIPULATORTYPE)EditorGUILayout.EnumPopup(playgroundLanguage.type, thisManipulatorProperty.mathProperty.type);
			thisManipulatorProperty.mathProperty.clamp = (MATHMANIPULATORCLAMP)EditorGUILayout.EnumPopup(playgroundLanguage.clamp, thisManipulatorProperty.mathProperty.clamp);
			if (thisManipulatorProperty.mathProperty.clamp!=MATHMANIPULATORCLAMP.None) {
				EditorGUI.indentLevel++;
				thisManipulatorProperty.mathProperty.clampFloor = EditorGUILayout.FloatField(playgroundLanguage.floor, thisManipulatorProperty.mathProperty.clampFloor);
				thisManipulatorProperty.mathProperty.clampCeil = EditorGUILayout.FloatField(playgroundLanguage.ceil, thisManipulatorProperty.mathProperty.clampCeil);
				EditorGUI.indentLevel--;
			}
			if (thisManipulatorProperty.mathProperty.property == MATHMANIPULATORPROPERTY.Rotation || thisManipulatorProperty.mathProperty.property == MATHMANIPULATORPROPERTY.Size) {
				thisManipulatorProperty.mathProperty.value = EditorGUILayout.FloatField(playgroundLanguage.value, thisManipulatorProperty.mathProperty.value);
				thisManipulatorProperty.mathProperty.rate = EditorGUILayout.FloatField(playgroundLanguage.rate, thisManipulatorProperty.mathProperty.rate);
			} else {
				thisManipulatorProperty.mathProperty.value3 = EditorGUILayout.Vector3Field(playgroundLanguage.value, thisManipulatorProperty.mathProperty.value3);
				thisManipulatorProperty.mathProperty.rate3 = EditorGUILayout.Vector3Field(playgroundLanguage.rate, thisManipulatorProperty.mathProperty.rate3);
			}
		}
		
		EditorGUILayout.Separator();
		if (thisManipulatorProperty.type!=MANIPULATORPROPERTYTYPEC.None &&
		    thisManipulatorProperty.type!=MANIPULATORPROPERTYTYPEC.Attractor &&
		    thisManipulatorProperty.type!=MANIPULATORPROPERTYTYPEC.Gravitational &&
		    thisManipulatorProperty.type!=MANIPULATORPROPERTYTYPEC.Repellent &&
		    thisManipulatorProperty.type!=MANIPULATORPROPERTYTYPEC.Vortex &&
		    thisManipulatorProperty.type!=MANIPULATORPROPERTYTYPEC.Turbulence &&
		    thisManipulatorProperty.type!=MANIPULATORPROPERTYTYPEC.Math
		    ) {
			GUI.enabled = guiEnabled&&thisManipulatorProperty.type!=MANIPULATORPROPERTYTYPEC.AdditiveVelocity;
			thisManipulatorProperty.transition = (MANIPULATORPROPERTYTRANSITIONC)EditorGUILayout.EnumPopup(playgroundLanguage.transition, thisManipulatorProperty.transition);
			GUI.enabled = guiEnabled;
		}
	}
	
	// Return name of a MANIPULATORTYPE
	public static string ManipulatorTypeName (MANIPULATORTYPEC mType) {
		string returnString;
		switch (mType) {
		case MANIPULATORTYPEC.None: returnString = playgroundLanguage.none; break;
		case MANIPULATORTYPEC.Attractor: returnString = playgroundLanguage.attractor; break;
		case MANIPULATORTYPEC.AttractorGravitational: returnString = playgroundLanguage.gravitational; break;
		case MANIPULATORTYPEC.Repellent: returnString = playgroundLanguage.repellent; break;
		case MANIPULATORTYPEC.Property: returnString = playgroundLanguage.property; break;
		case MANIPULATORTYPEC.Combined: returnString = playgroundLanguage.combined; break;
		case MANIPULATORTYPEC.Vortex: returnString = playgroundLanguage.vortex; break;
		default: returnString = playgroundLanguage.manipulator; break;
		}
		return returnString;
	}
	
	// Return name of a MANIPULATORSHAPE
	public static string ManipulatorTypeName (MANIPULATORSHAPEC mShape) {
		string returnString;
		switch (mShape) {
		case MANIPULATORSHAPEC.Sphere: returnString = playgroundLanguage.sphere; break;
		case MANIPULATORSHAPEC.Box: returnString = playgroundLanguage.box; break;
		case MANIPULATORSHAPEC.Infinite: returnString = playgroundLanguage.infinite; break;
		default: returnString = playgroundLanguage.nullName; break;
		}
		return returnString;
	}
	
}

[InitializeOnLoad()]
class PlaygroundHierarchyIcon {
	static Texture2D iconActive;
	static Texture2D iconInactive;
	static Texture2D iconHeavy;
	static PlaygroundSettingsC playgroundSettings;
	
	static PlaygroundHierarchyIcon () {
		Set();
	}
	
	public static void Set () {
		playgroundSettings = PlaygroundSettingsC.GetReference();
		if (playgroundSettings.hierarchyIcon) {
			iconActive = playgroundSettings.playgroundIcon;
			iconInactive = playgroundSettings.playgroundIconInactive;
			iconHeavy = playgroundSettings.playgroundIconHeavy;
			if (iconActive!=null&&iconInactive!=null&&iconHeavy!=null) {
				EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyIcon;
				EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyIcon;
			}
		} else EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyIcon;
		EditorApplication.RepaintHierarchyWindow();
	}
	
	static void DrawHierarchyIcon (int instanceID, Rect instanceRect) {
		if (PlaygroundC.reference!=null) {
			if (!playgroundSettings.hierarchyIcon) {
				EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyIcon;
				return;
			}
			for (int i = 0; i<PlaygroundC.reference.particleSystems.Count; i++) {
				if (PlaygroundC.reference.particleSystems[i] != null && instanceID==PlaygroundC.reference.particleSystems[i].particleSystemGameObject.GetInstanceID()) {
					Rect rOffset = new Rect (instanceRect); 
					rOffset.x = rOffset.width+14;
					if (PlaygroundC.reference.particleSystems[i].particleSystemGameObject.activeInHierarchy && PlaygroundC.reference.calculate && PlaygroundC.reference.particleSystems[i].calculate && PlaygroundC.reference.particleSystems[i].IsAlive())
						if (EditorApplication.isPlaying && PlaygroundC.reference.particleSystems[i].IsReportingBadUpdateRate())
							GUI.Label (rOffset, iconHeavy);
					else
						GUI.Label (rOffset, iconActive);
					else
						GUI.Label (rOffset, iconInactive);
				}
			}
		}
	}
}
