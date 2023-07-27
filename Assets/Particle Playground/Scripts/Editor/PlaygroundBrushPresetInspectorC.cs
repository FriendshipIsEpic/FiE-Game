using UnityEditor;
using UnityEngine;
using ParticlePlaygroundLanguage;

[CustomEditor(typeof(PlaygroundBrushPresetC))]
public class PlaygroundBrushPresetInspectorC : Editor {
	
	public static SerializedObject brushPreset;					// PlaygroundBrushPreset
	
	public static SerializedProperty presetName;				// String
	
	public static SerializedProperty texture;					// Texture2D
	public static SerializedProperty scale;						// float
	public static SerializedProperty detail;					// BRUSHDETAIL
	public static SerializedProperty distance;					// float
	
	public static SerializedProperty spacing;					// float
	public static SerializedProperty exceedMaxStopsPaint; 		// boolean

	public static PlaygroundLanguageC playgroundLanguage;
	
	void OnEnable () {
		playgroundLanguage = PlaygroundSettingsC.GetLanguage();

		brushPreset = new SerializedObject(target);
		texture = brushPreset.FindProperty("texture");
		presetName = brushPreset.FindProperty("presetName");
		scale = brushPreset.FindProperty("scale");
		detail = brushPreset.FindProperty("detail");
		distance = brushPreset.FindProperty("distance");
		spacing = brushPreset.FindProperty("spacing");
		exceedMaxStopsPaint = brushPreset.FindProperty("exceedMaxStopsPaint");
	}
	
	public override void OnInspectorGUI () {
		
		brushPreset.Update();
		
		GUILayout.BeginVertical(EditorStyles.textField);
		EditorGUILayout.Space();
		
		// Name
		EditorGUILayout.PropertyField(presetName, new GUIContent(
			playgroundLanguage.nameText, 
			playgroundLanguage.brushNameDescription)
		);
		
		EditorGUILayout.Space();
		
		// Texture
		EditorGUILayout.PropertyField(texture, new GUIContent(
			playgroundLanguage.brushShape, 
			playgroundLanguage.brushShapeDescription)
		);
		
		// Scale
		EditorGUILayout.PropertyField(scale, new GUIContent(
			playgroundLanguage.brushScale, 
			playgroundLanguage.brushScaleDescription)
		);
		
		// Detail
		EditorGUILayout.PropertyField(detail, new GUIContent(
			playgroundLanguage.brushDetail, 
			playgroundLanguage.brushDetailDescription)
		);
		
		// Distance
		EditorGUILayout.PropertyField(distance, new GUIContent(
			playgroundLanguage.brushDistance, 
			playgroundLanguage.brushDistanceDescription)
		);
		
		EditorGUILayout.Space();
		
		// Spacing
		EditorGUILayout.PropertyField(spacing, new GUIContent(
			playgroundLanguage.paintSpacing, 
			playgroundLanguage.paintSpacingDescription)
		);
		
		EditorGUILayout.Space();
		
		// Exceeding max stops paint
		EditorGUILayout.PropertyField(exceedMaxStopsPaint, new GUIContent(
			playgroundLanguage.exceedMaxStopsPaint, 
			playgroundLanguage.exceedMaxStopsPaintDescription)
		);
		
		EditorGUILayout.Space();
		GUILayout.EndVertical();
		
		brushPreset.ApplyModifiedProperties();
	}
}