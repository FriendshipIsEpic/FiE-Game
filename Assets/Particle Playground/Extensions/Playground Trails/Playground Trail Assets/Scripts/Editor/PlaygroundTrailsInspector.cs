using UnityEditor;
using UnityEngine;
using System.Collections;
using ParticlePlayground;
using ParticlePlaygroundLanguage;

[CustomEditor (typeof(PlaygroundTrails))]
public class PlaygroundTrailsInspector : Editor {

	PlaygroundTrails trails;
	
	SerializedObject s_trails;
	SerializedProperty s_playgroundSystem;
	SerializedProperty s_lifetimeColor;
	SerializedProperty s_material;
	SerializedProperty s_renderMode;
	SerializedProperty s_colorMode;
	SerializedProperty s_uvMode;
	SerializedProperty s_pointArrayAlpha;
	SerializedProperty s_billboardTransform;
	SerializedProperty s_customRenderScale;
	SerializedProperty s_time;
	SerializedProperty s_timeWidth;
	SerializedProperty s_widthScale;
	SerializedProperty s_minVertexDistance;
	SerializedProperty s_maxVertexDistance;
	SerializedProperty s_maxPathDeviation;
	SerializedProperty s_createPointsOnCollision;
	SerializedProperty s_maxPoints;
	SerializedProperty s_createFirstPointOnParticleBirth;
	SerializedProperty s_createLastPointOnParticleDeath;
	SerializedProperty s_multithreading;
	SerializedProperty s_receiveShadows;
	SerializedProperty s_castShadows;

	// GUI
	public static GUIStyle boxStyle;
	public static PlaygroundSettingsC playgroundSettings;
	public static PlaygroundLanguageC playgroundLanguage;

	public void OnEnable () 
	{
		trails = target as PlaygroundTrails;
		s_trails = new SerializedObject(trails);

		s_time = s_trails.FindProperty("time");

		s_material = s_trails.FindProperty("material");
		s_lifetimeColor = s_trails.FindProperty("lifetimeColor");
		s_colorMode = s_trails.FindProperty("colorMode");
		s_uvMode = s_trails.FindProperty("uvMode");
		s_pointArrayAlpha = s_trails.FindProperty("pointArrayAlpha");
		s_renderMode = s_trails.FindProperty("renderMode");
		s_billboardTransform = s_trails.FindProperty("billboardTransform");
		s_customRenderScale = s_trails.FindProperty("customRenderScale");

		s_timeWidth = s_trails.FindProperty("timeWidth");
		s_widthScale = s_trails.FindProperty("widthScale");

		s_minVertexDistance = s_trails.FindProperty("minVertexDistance");
		s_maxVertexDistance = s_trails.FindProperty("maxVertexDistance");
		s_maxPathDeviation = s_trails.FindProperty("maxPathDeviation");
		s_createPointsOnCollision = s_trails.FindProperty("createPointsOnCollision");
		s_maxPoints = s_trails.FindProperty("maxPoints");
		s_createFirstPointOnParticleBirth = s_trails.FindProperty("createFirstPointOnParticleBirth");
		s_createLastPointOnParticleDeath = s_trails.FindProperty("createLastPointOnParticleDeath");

		s_playgroundSystem = s_trails.FindProperty("playgroundSystem");
		s_multithreading = s_trails.FindProperty("multithreading");

		s_receiveShadows = s_trails.FindProperty("receiveShadows");
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
		s_castShadows = s_trails.FindProperty("castShadows");
#else
		s_castShadows = s_trails.FindProperty("shadowCastingMode");
#endif
		playgroundSettings = PlaygroundSettingsC.GetReference();
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();

		// Issue a quick refresh
		if (!EditorApplication.isPlaying && Selection.activeTransform!=null)
		{
			trails.ResetTrails();
		}
	}

	public override void OnInspectorGUI ()
	{
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");

		s_trails.UpdateIfDirtyOrScript();

		bool hasParticleSystem = trails.playgroundSystem != null;
		if (!hasParticleSystem)
			EditorGUILayout.HelpBox(playgroundLanguage.missingParticleSystemWarning, MessageType.Warning);

		EditorGUILayout.BeginVertical (boxStyle);
		playgroundSettings.playgroundTrailsFoldout = GUILayout.Toggle(playgroundSettings.playgroundTrailsFoldout, playgroundLanguage.playgroundTrails, EditorStyles.foldout);
		if (playgroundSettings.playgroundTrailsFoldout) 
		{
			EditorGUILayout.BeginVertical (boxStyle);

			// Time foldout
			if (GUILayout.Button(playgroundLanguage.time+" ("+s_time.floatValue.ToString ("F1")+")", EditorStyles.toolbarDropDown)) playgroundSettings.trailsTimeFoldout=!playgroundSettings.trailsTimeFoldout;
			if (playgroundSettings.trailsTimeFoldout) 
			{
				EditorGUILayout.Separator();
				EditorGUILayout.PropertyField(s_time, new GUIContent(playgroundLanguage.time));
				EditorGUILayout.Separator();
			}

			// Width foldout
			if (GUILayout.Button(playgroundLanguage.width+" ("+s_widthScale.floatValue.ToString ("F1")+")", EditorStyles.toolbarDropDown)) playgroundSettings.trailsWidthFoldout=!playgroundSettings.trailsWidthFoldout;
			if (playgroundSettings.trailsWidthFoldout) 
			{
				EditorGUILayout.Separator();
				EditorGUILayout.PropertyField(s_timeWidth, new GUIContent(playgroundLanguage.timeWidth));
				EditorGUILayout.PropertyField(s_widthScale, new GUIContent(playgroundLanguage.widthScale));
				EditorGUILayout.Separator();
			}

			// Rendering foldout
			if (GUILayout.Button(playgroundLanguage.rendering+" ("+trails.renderMode+")", EditorStyles.toolbarDropDown)) playgroundSettings.trailsRenderingFoldout=!playgroundSettings.trailsRenderingFoldout;
			if (playgroundSettings.trailsRenderingFoldout) 
			{
				EditorGUILayout.Separator();
				EditorGUILayout.PropertyField(s_material, new GUIContent(playgroundLanguage.material));
				EditorGUILayout.PropertyField(s_lifetimeColor, new GUIContent(playgroundLanguage.lifetimeColor));
				EditorGUILayout.PropertyField(s_colorMode, new GUIContent(playgroundLanguage.colorMode));
				if (trails.colorMode == TrailColorMode.Lifetime)
					EditorGUILayout.PropertyField(s_pointArrayAlpha, new GUIContent(playgroundLanguage.pointArrayAlpha));
				EditorGUILayout.PropertyField(s_uvMode, new GUIContent(playgroundLanguage.uvMode));
				EditorGUILayout.PropertyField(s_renderMode, new GUIContent(playgroundLanguage.renderMode));
				if (trails.renderMode == TrailRenderMode.Billboard)
					EditorGUILayout.PropertyField(s_billboardTransform, new GUIContent(playgroundLanguage.billboardTransform));
				else if (trails.renderMode == TrailRenderMode.CustomRenderScale)
					EditorGUILayout.PropertyField(s_customRenderScale, new GUIContent(playgroundLanguage.customRenderScale));
				EditorGUILayout.PropertyField(s_castShadows, new GUIContent(playgroundLanguage.castShadows));
				EditorGUILayout.PropertyField(s_receiveShadows, new GUIContent(playgroundLanguage.receiveShadows));
				EditorGUILayout.Separator();
			}

			// Point creation foldout
			if (GUILayout.Button(playgroundLanguage.pointCreation+" ("+s_maxPoints.intValue+")", EditorStyles.toolbarDropDown)) playgroundSettings.trailsPointCreationFoldout=!playgroundSettings.trailsPointCreationFoldout;
			if (playgroundSettings.trailsPointCreationFoldout) 
			{
				EditorGUILayout.Separator();
				EditorGUILayout.PropertyField(s_maxPoints, new GUIContent(playgroundLanguage.maximumPoints));
				EditorGUILayout.PropertyField(s_minVertexDistance, new GUIContent(playgroundLanguage.minimumVertexDistance));
				EditorGUILayout.PropertyField(s_maxVertexDistance, new GUIContent(playgroundLanguage.maximumVertexDistance));
				EditorGUILayout.PropertyField(s_maxPathDeviation, new GUIContent(playgroundLanguage.maximumPathDeviation));
				EditorGUILayout.PropertyField(s_createFirstPointOnParticleBirth, new GUIContent(playgroundLanguage.createFirstPointOnParticleBirth));
				EditorGUILayout.PropertyField(s_createLastPointOnParticleDeath, new GUIContent(playgroundLanguage.createLastPointOnParticleDeath));
				EditorGUILayout.PropertyField(s_createPointsOnCollision, new GUIContent(playgroundLanguage.createPointsOnCollision));
				EditorGUILayout.Separator();
			}

			// Advanced foldout
			if (GUILayout.Button(playgroundLanguage.advanced, EditorStyles.toolbarDropDown)) playgroundSettings.trailsAdvancedFoldout=!playgroundSettings.trailsAdvancedFoldout;
			if (playgroundSettings.trailsAdvancedFoldout) 
			{
				EditorGUILayout.Separator();
				EditorGUILayout.PropertyField(s_playgroundSystem, new GUIContent(playgroundLanguage.particleSystem));
				EditorGUILayout.PropertyField(s_multithreading, new GUIContent(playgroundLanguage.multithreading));
			}

			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();

		s_trails.ApplyModifiedProperties();
	}
}
