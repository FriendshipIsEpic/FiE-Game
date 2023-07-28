using UnityEditor;
using UnityEngine;
using System.Collections;
using ParticlePlayground;
using ParticlePlaygroundLanguage;

[CustomEditor (typeof(PlaygroundFollow))]
public class PlaygroundFollowInspector : Editor {

	PlaygroundFollow follow;

	SerializedObject s_follow;
	SerializedProperty s_particles;
	SerializedProperty s_referenceObject;
	SerializedProperty s_followerLifetime;
	SerializedProperty s_cacheSize;
	SerializedProperty s_sendEvents;

	// GUI
	public static GUIStyle boxStyle;
	public static PlaygroundSettingsC playgroundSettings;
	public static PlaygroundLanguageC playgroundLanguage;
	
	public void OnEnable () 
	{
		follow = target as PlaygroundFollow;
		s_follow = new SerializedObject(follow);

		s_particles = s_follow.FindProperty("particles");
		s_referenceObject = s_follow.FindProperty("referenceObject");
		s_followerLifetime = s_follow.FindProperty("followerLifetime");
		s_cacheSize = s_follow.FindProperty("cacheSize");
		s_sendEvents = s_follow.FindProperty("sendEvents");

		playgroundSettings = PlaygroundSettingsC.GetReference();
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();
	}

	public override void OnInspectorGUI ()
	{
		if (boxStyle==null)
			boxStyle = GUI.skin.FindStyle("box");

		s_follow.UpdateIfDirtyOrScript();

		bool hasParticleSystem = follow.particles != null;
		if (!hasParticleSystem)
			EditorGUILayout.HelpBox(playgroundLanguage.missingParticleSystemWarning, MessageType.Warning);

		EditorGUILayout.BeginVertical (boxStyle);
		playgroundSettings.playgroundFollowFoldout = GUILayout.Toggle(playgroundSettings.playgroundFollowFoldout, playgroundLanguage.playgroundFollow, EditorStyles.foldout);
		if (playgroundSettings.playgroundFollowFoldout) 
		{
			EditorGUILayout.BeginVertical (boxStyle);

			// Followers foldout
			int activeFollowers = follow.GetActiveFollowersCount();
			if (GUILayout.Button(playgroundLanguage.followers+" ("+(follow.referenceObject==null?playgroundLanguage.unassigned : activeFollowers.ToString())+")", EditorStyles.toolbarDropDown)) playgroundSettings.followFollowersFoldout=!playgroundSettings.followFollowersFoldout;
			if (playgroundSettings.followFollowersFoldout) 
			{
				EditorGUILayout.Separator();
				EditorGUILayout.PropertyField(s_referenceObject, new GUIContent(playgroundLanguage.referenceObject));
				EditorGUILayout.Separator();
			}

			if (GUILayout.Button(playgroundLanguage.advanced, EditorStyles.toolbarDropDown)) playgroundSettings.followAdvancedFoldout=!playgroundSettings.followAdvancedFoldout;
			if (playgroundSettings.followAdvancedFoldout) 
			{
				EditorGUILayout.Separator();
				EditorGUILayout.PropertyField(s_particles, new GUIContent(playgroundLanguage.particleSystem, "The particle system to follow within the scene."));
				EditorGUILayout.PropertyField(s_followerLifetime, new GUIContent(playgroundLanguage.lifetime, "The lifetime of the followers, when set to 0 the followers will get automatically controlled lifetime based on the particles."));
				EditorGUILayout.PropertyField(s_cacheSize, new GUIContent(playgroundLanguage.cacheSize, "The object pool of the followers, when set to 0 the pool will be sized to match their lifetime."));
				EditorGUILayout.PropertyField(s_sendEvents, new GUIContent(playgroundLanguage.sendEvents, "Determines if the followers should broadcast events upon birth and death."));
			}

			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.EndVertical();

		s_follow.ApplyModifiedProperties();
	}
}
